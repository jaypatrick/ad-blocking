//! Core compiler functionality for AdGuard filter rules.

use chrono::{DateTime, Utc};
use sha2::{Digest, Sha384};
use std::fs::{self, File};
use std::io::{BufRead, BufReader, Read};
use std::path::{Path, PathBuf};
use std::process::Command;
use std::time::Instant;

use crate::config::{CompilerConfiguration, ConfigurationFormat, read_configuration, to_json};
use crate::error::{CompilerError, Result};

/// Platform-specific information.
#[derive(Debug, Clone, Default)]
#[must_use]
pub struct PlatformInfo {
    /// Operating system name
    pub os_name: String,
    /// Operating system version
    pub os_version: String,
    /// Processor architecture
    pub architecture: String,
    /// Whether the platform is Windows
    pub is_windows: bool,
    /// Whether the platform is Linux
    pub is_linux: bool,
    /// Whether the platform is macOS
    pub is_macos: bool,
}

/// Version information for all components.
#[derive(Debug, Clone, Default)]
#[must_use]
pub struct VersionInfo {
    /// Module version
    pub module_version: String,
    /// Rust version
    pub rust_version: String,
    /// Node.js version (if available)
    pub node_version: Option<String>,
    /// hostlist-compiler version (if available)
    pub hostlist_compiler_version: Option<String>,
    /// Path to hostlist-compiler
    pub hostlist_compiler_path: Option<String>,
    /// Platform information
    pub platform: PlatformInfo,
}

/// Result of a compilation operation.
#[derive(Debug, Clone)]
#[must_use]
pub struct CompilerResult {
    /// Whether compilation was successful
    pub success: bool,
    /// Name from configuration
    pub config_name: String,
    /// Version from configuration
    pub config_version: String,
    /// Number of rules in output
    pub rule_count: usize,
    /// Path to output file
    pub output_path: String,
    /// SHA-384 hash of output file
    pub output_hash: String,
    /// Whether output was copied to rules
    pub copied_to_rules: bool,
    /// Destination path if copied
    pub rules_destination: Option<String>,
    /// Elapsed time in milliseconds
    pub elapsed_ms: u64,
    /// Start time
    pub start_time: DateTime<Utc>,
    /// End time
    pub end_time: DateTime<Utc>,
    /// Error message if failed
    pub error_message: Option<String>,
    /// Standard output from compiler
    pub stdout: String,
    /// Standard error from compiler
    pub stderr: String,
}

impl Default for CompilerResult {
    fn default() -> Self {
        let now = Utc::now();
        Self {
            success: false,
            config_name: String::new(),
            config_version: String::new(),
            rule_count: 0,
            output_path: String::new(),
            output_hash: String::new(),
            copied_to_rules: false,
            rules_destination: None,
            elapsed_ms: 0,
            start_time: now,
            end_time: now,
            error_message: None,
            stdout: String::new(),
            stderr: String::new(),
        }
    }
}

/// Get platform information.
pub fn get_platform_info() -> PlatformInfo {
    PlatformInfo {
        os_name: std::env::consts::OS.to_string(),
        os_version: String::new(), // Would need platform-specific code
        architecture: std::env::consts::ARCH.to_string(),
        is_windows: cfg!(target_os = "windows"),
        is_linux: cfg!(target_os = "linux"),
        is_macos: cfg!(target_os = "macos"),
    }
}

/// Find command in PATH.
fn find_command(name: &str) -> Option<PathBuf> {
    which::which(name).ok()
}

/// Get version from a command.
fn get_command_version(cmd: &str, args: &[&str]) -> Option<String> {
    Command::new(cmd)
        .args(args)
        .output()
        .ok()
        .and_then(|output| {
            if output.status.success() {
                String::from_utf8(output.stdout)
                    .ok()
                    .map(|s| s.lines().next().unwrap_or("").trim().to_string())
            } else {
                None
            }
        })
}

/// Get version information for all components.
pub fn get_version_info() -> VersionInfo {
    let mut info = VersionInfo {
        module_version: crate::VERSION.to_string(),
        rust_version: format!("{}", rustc_version_runtime::version()),
        platform: get_platform_info(),
        ..Default::default()
    };

    // Check Node.js
    if let Some(node_path) = find_command("node") {
        info.node_version =
            get_command_version(node_path.to_str().unwrap_or("node"), &["--version"]);
    }

    // Check hostlist-compiler
    if let Some(compiler_path) = find_command("hostlist-compiler") {
        info.hostlist_compiler_path = Some(compiler_path.display().to_string());
        info.hostlist_compiler_version = get_command_version(
            compiler_path.to_str().unwrap_or("hostlist-compiler"),
            &["--version"],
        );
    } else if find_command("npx").is_some() {
        info.hostlist_compiler_path = Some("npx @adguard/hostlist-compiler".to_string());
    }

    info
}

/// Count non-empty, non-comment lines in a file.
///
/// Uses `map_while` to stop iteration on the first I/O error rather than
/// continuing indefinitely. This prevents infinite loops on persistent read
/// errors and ensures we only count lines from successfully read data.
#[must_use]
pub fn count_rules<P: AsRef<Path>>(file_path: P) -> usize {
    let file = match File::open(file_path.as_ref()) {
        Ok(f) => f,
        Err(_) => return 0,
    };

    BufReader::new(file)
        .lines()
        .map_while(|line| line.ok())
        .filter(|line| {
            let trimmed = line.trim();
            !trimmed.is_empty() && !trimmed.starts_with('!') && !trimmed.starts_with('#')
        })
        .count()
}

/// Compute SHA-384 hash of a file.
pub fn compute_hash<P: AsRef<Path>>(file_path: P) -> Result<String> {
    let mut file = File::open(file_path.as_ref())?;
    let mut hasher = Sha384::new();
    let mut buffer = [0u8; 8192];

    loop {
        let bytes_read = file.read(&mut buffer)?;
        if bytes_read == 0 {
            break;
        }
        hasher.update(&buffer[..bytes_read]);
    }

    Ok(hex::encode(hasher.finalize()))
}

/// Get compiler command and arguments.
fn get_compiler_command(config_path: &str, output_path: &str) -> Result<(String, Vec<String>)> {
    if let Some(compiler_path) = find_command("hostlist-compiler") {
        return Ok((
            compiler_path.display().to_string(),
            vec![
                "--config".to_string(),
                config_path.to_string(),
                "--output".to_string(),
                output_path.to_string(),
            ],
        ));
    }

    if let Some(npx_path) = find_command("npx") {
        return Ok((
            npx_path.display().to_string(),
            vec![
                "@adguard/hostlist-compiler".to_string(),
                "--config".to_string(),
                config_path.to_string(),
                "--output".to_string(),
                output_path.to_string(),
            ],
        ));
    }

    Err(CompilerError::CompilerNotFound)
}

/// Main compiler for AdGuard filter rules.
///
/// # Examples
///
/// ```no_run
/// use rules_compiler::RulesCompiler;
///
/// let compiler = RulesCompiler::new();
/// let result = compiler.compile("config.json", None, false, None, None)?;
/// assert!(result.success);
/// # Ok::<(), rules_compiler::CompilerError>(())
/// ```
#[derive(Debug, Default)]
pub struct RulesCompiler {
    /// Enable debug output
    pub debug: bool,
}

impl RulesCompiler {
    /// Create a new compiler instance.
    pub fn new() -> Self {
        Self::default()
    }

    /// Create a new compiler with debug enabled.
    pub fn with_debug(debug: bool) -> Self {
        Self { debug }
    }

    /// Compile filter rules.
    pub fn compile<P: AsRef<Path>>(
        &self,
        config_path: P,
        output_path: Option<&Path>,
        copy_to_rules: bool,
        rules_directory: Option<&Path>,
        format: Option<ConfigurationFormat>,
    ) -> Result<CompilerResult> {
        compile_rules(
            config_path,
            output_path,
            copy_to_rules,
            rules_directory,
            format,
            self.debug,
        )
    }

    /// Read configuration from a file.
    pub fn read_config<P: AsRef<Path>>(
        &self,
        config_path: P,
        format: Option<ConfigurationFormat>,
    ) -> Result<CompilerConfiguration> {
        read_configuration(config_path, format)
    }

    /// Get version information.
    pub fn get_version_info(&self) -> VersionInfo {
        get_version_info()
    }
}

/// Compile filter rules using hostlist-compiler.
pub fn compile_rules<P: AsRef<Path>>(
    config_path: P,
    output_path: Option<&Path>,
    copy_to_rules: bool,
    rules_directory: Option<&Path>,
    format: Option<ConfigurationFormat>,
    debug: bool,
) -> Result<CompilerResult> {
    let start = Instant::now();
    let mut result = CompilerResult {
        start_time: Utc::now(),
        ..Default::default()
    };

    let config_path = config_path.as_ref().canonicalize()?;
    let mut temp_config_path: Option<PathBuf> = None;

    // Read configuration
    let config = read_configuration(&config_path, format)?;
    result.config_name = config.name.clone();
    result.config_version = config.version.clone();

    // Determine output path
    let actual_output = match output_path {
        Some(p) => p.to_path_buf(),
        None => {
            let timestamp = Utc::now().format("%Y%m%d-%H%M%S");
            let output_dir = config_path
                .parent()
                .unwrap_or(Path::new("."))
                .join("output");
            fs::create_dir_all(&output_dir)?;
            output_dir.join(format!("compiled-{}.txt", timestamp))
        }
    };
    result.output_path = actual_output.display().to_string();

    // Convert to JSON if needed
    let compile_config_path = if config.source_format != Some(ConfigurationFormat::Json) {
        let temp_path =
            std::env::temp_dir().join(format!("compiler-config-{}.json", uuid::Uuid::new_v4()));
        let json = to_json(&config)?;
        fs::write(&temp_path, json)?;
        temp_config_path = Some(temp_path.clone());

        if debug {
            eprintln!("[DEBUG] Created temp JSON config: {}", temp_path.display());
        }

        temp_path
    } else {
        config_path.clone()
    };

    // Get compiler command
    let (cmd, args) = get_compiler_command(
        compile_config_path.to_str().unwrap_or(""),
        actual_output.to_str().unwrap_or(""),
    )?;

    if debug {
        eprintln!("[DEBUG] Running: {} {}", cmd, args.join(" "));
    }

    // Run compilation
    let output = Command::new(&cmd)
        .args(&args)
        .current_dir(config_path.parent().unwrap_or(Path::new(".")))
        .output()?;

    result.stdout = String::from_utf8_lossy(&output.stdout).to_string();
    result.stderr = String::from_utf8_lossy(&output.stderr).to_string();

    // Clean up temp file
    if let Some(temp_path) = temp_config_path {
        let _ = fs::remove_file(temp_path);
    }

    if !output.status.success() {
        result.error_message = Some(format!(
            "Compiler exited with code {:?}: {}",
            output.status.code(),
            result.stderr
        ));
        result.end_time = Utc::now();
        result.elapsed_ms = start.elapsed().as_millis() as u64;
        return Ok(result);
    }

    // Verify output was created
    if !actual_output.exists() {
        result.error_message =
            Some("Compilation completed but output file was not created".to_string());
        result.end_time = Utc::now();
        result.elapsed_ms = start.elapsed().as_millis() as u64;
        return Ok(result);
    }

    // Calculate statistics
    result.rule_count = count_rules(&actual_output);
    result.output_hash = compute_hash(&actual_output)?;
    result.success = true;

    // Copy to rules directory if requested
    if copy_to_rules {
        let rules_dir = rules_directory.map(|p| p.to_path_buf()).unwrap_or_else(|| {
            config_path
                .parent()
                .unwrap_or(Path::new("."))
                .parent()
                .unwrap_or(Path::new("."))
                .parent()
                .unwrap_or(Path::new("."))
                .join("rules")
        });

        fs::create_dir_all(&rules_dir)?;
        let dest_path = rules_dir.join("adguard_user_filter.txt");
        fs::copy(&actual_output, &dest_path)?;
        result.copied_to_rules = true;
        result.rules_destination = Some(dest_path.display().to_string());
    }

    result.end_time = Utc::now();
    result.elapsed_ms = start.elapsed().as_millis() as u64;

    Ok(result)
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::io::Write;
    use tempfile::TempDir;

    #[test]
    fn test_count_rules() {
        let dir = TempDir::new().unwrap();
        let path = dir.path().join("rules.txt");
        let mut file = File::create(&path).unwrap();
        writeln!(file, "! Comment").unwrap();
        writeln!(file, "# Another comment").unwrap();
        writeln!(file, "||example.com^").unwrap();
        writeln!(file, "||test.org^").unwrap();
        writeln!(file).unwrap();
        writeln!(file, "@@||allowed.com^").unwrap();

        assert_eq!(count_rules(&path), 3);
    }

    #[test]
    fn test_count_rules_empty_file() {
        let dir = TempDir::new().unwrap();
        let path = dir.path().join("empty.txt");
        File::create(&path).unwrap();

        assert_eq!(count_rules(&path), 0);
    }

    #[test]
    fn test_compute_hash() {
        let dir = TempDir::new().unwrap();
        let path = dir.path().join("test.txt");
        let mut file = File::create(&path).unwrap();
        writeln!(file, "Test content").unwrap();

        let hash = compute_hash(&path).unwrap();
        assert_eq!(hash.len(), 96); // SHA-384 = 96 hex chars
    }

    #[test]
    fn test_get_platform_info() {
        let info = get_platform_info();
        assert!(!info.os_name.is_empty());
        assert!(!info.architecture.is_empty());
    }
}
