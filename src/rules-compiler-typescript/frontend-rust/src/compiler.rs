//! Compiler service that wraps the TypeScript rules compiler.

use crate::config::{CompilerConfig, ConfigFormat};
use crate::error::{CompilerError, Result};
use chrono::{DateTime, Utc};
use sha2::{Digest, Sha384};
use std::fs;
use std::io::{BufRead, BufReader};
use std::path::{Path, PathBuf};
use std::process::{Command, Stdio};
use std::time::Instant;

/// Result of a compilation operation.
#[derive(Debug, Clone)]
pub struct CompilerResult {
    /// Whether compilation succeeded
    pub success: bool,
    /// Configuration name
    pub config_name: String,
    /// Configuration version
    pub config_version: String,
    /// Number of rules in output
    pub rule_count: usize,
    /// Path to output file
    pub output_path: PathBuf,
    /// SHA-384 hash of output
    pub output_hash: String,
    /// Whether output was copied to rules directory
    pub copied_to_rules: bool,
    /// Destination path if copied
    pub rules_destination: Option<PathBuf>,
    /// Elapsed time in milliseconds
    pub elapsed_ms: u64,
    /// Start time
    pub start_time: DateTime<Utc>,
    /// End time
    pub end_time: DateTime<Utc>,
    /// Error message if failed
    pub error_message: Option<String>,
}

/// Version information.
#[derive(Debug, Clone)]
pub struct VersionInfo {
    /// Module version
    pub module_version: String,
    /// Rust version
    pub rust_version: String,
    /// Platform OS
    pub os: String,
    /// Platform architecture
    pub arch: String,
    /// Node.js version (if available)
    pub node_version: Option<String>,
}

/// Get version information.
pub fn get_version_info() -> VersionInfo {
    let node_version = Command::new("node")
        .arg("--version")
        .output()
        .ok()
        .and_then(|o| String::from_utf8(o.stdout).ok())
        .map(|s| s.trim().to_string());

    VersionInfo {
        module_version: env!("CARGO_PKG_VERSION").to_string(),
        rust_version: format!("{}", rustc_version()),
        os: std::env::consts::OS.to_string(),
        arch: std::env::consts::ARCH.to_string(),
        node_version,
    }
}

fn rustc_version() -> String {
    Command::new("rustc")
        .arg("--version")
        .output()
        .ok()
        .and_then(|o| String::from_utf8(o.stdout).ok())
        .map(|s| s.trim().to_string())
        .unwrap_or_else(|| "unknown".to_string())
}

/// Count non-empty, non-comment lines in a file.
pub fn count_rules(path: &Path) -> Result<usize> {
    let content = fs::read_to_string(path)?;
    let count = content
        .lines()
        .filter(|line| {
            let trimmed = line.trim();
            !trimmed.is_empty() && !trimmed.starts_with('!') && !trimmed.starts_with('#')
        })
        .count();
    Ok(count)
}

/// Compute SHA-384 hash of a file.
pub fn compute_hash(path: &Path) -> Result<String> {
    let content = fs::read(path)?;
    let mut hasher = Sha384::new();
    hasher.update(&content);
    let result = hasher.finalize();
    Ok(hex::encode(result))
}

/// Copy file to rules directory.
pub fn copy_to_rules(source: &Path, dest: &Path) -> Result<()> {
    if let Some(parent) = dest.parent() {
        fs::create_dir_all(parent)?;
    }
    fs::copy(source, dest)?;
    Ok(())
}

/// Options for running the compiler.
pub struct CompileOptions {
    /// Path to configuration file
    pub config_path: PathBuf,
    /// Path to output file
    pub output_path: Option<PathBuf>,
    /// Copy output to rules directory
    pub copy_to_rules: bool,
    /// Custom rules directory
    pub rules_directory: Option<PathBuf>,
    /// Force configuration format
    pub format: Option<ConfigFormat>,
    /// Enable debug output
    pub debug: bool,
}

/// Run the TypeScript compiler via Node.js subprocess.
pub fn compile_via_typescript(options: &CompileOptions) -> Result<CompilerResult> {
    let start_time = Utc::now();
    let instant = Instant::now();

    // Read configuration to get metadata
    let config = CompilerConfig::from_file(&options.config_path, options.format)?;

    // Generate output path
    let output_path = options.output_path.clone().unwrap_or_else(|| {
        let timestamp = Utc::now().format("%Y-%m-%dT%H-%M-%S").to_string();
        let dir = options.config_path.parent().unwrap_or(Path::new("."));
        dir.join("output").join(format!("compiled-{}.txt", timestamp))
    });

    // Ensure output directory exists
    if let Some(parent) = output_path.parent() {
        fs::create_dir_all(parent)?;
    }

    // Find the TypeScript compiler
    let ts_compiler = find_typescript_compiler(&options.config_path)?;

    // Build command arguments
    let mut args = vec![
        ts_compiler.to_string_lossy().to_string(),
        "-c".to_string(),
        options.config_path.to_string_lossy().to_string(),
        "-o".to_string(),
        output_path.to_string_lossy().to_string(),
    ];

    if options.debug {
        args.push("-d".to_string());
    }

    if options.copy_to_rules {
        args.push("-r".to_string());
        if let Some(ref rules_dir) = options.rules_directory {
            args.push("--rules-dir".to_string());
            args.push(rules_dir.to_string_lossy().to_string());
        }
    }

    // Run the TypeScript compiler
    println!("[INFO] Running TypeScript compiler...");
    if options.debug {
        println!("[DEBUG] Command: npx ts-node {}", args.join(" "));
    }

    let mut child = Command::new("npx")
        .arg("ts-node")
        .args(&args)
        .current_dir(ts_compiler.parent().unwrap_or(Path::new(".")))
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .spawn()?;

    // Stream output
    if let Some(stdout) = child.stdout.take() {
        let reader = BufReader::new(stdout);
        for line in reader.lines() {
            if let Ok(line) = line {
                println!("{}", line);
            }
        }
    }

    let status = child.wait()?;
    let elapsed_ms = instant.elapsed().as_millis() as u64;
    let end_time = Utc::now();

    if !status.success() {
        return Ok(CompilerResult {
            success: false,
            config_name: config.name,
            config_version: config.version,
            rule_count: 0,
            output_path,
            output_hash: String::new(),
            copied_to_rules: false,
            rules_destination: None,
            elapsed_ms,
            start_time,
            end_time,
            error_message: Some(format!("Compiler exited with code: {:?}", status.code())),
        });
    }

    // Calculate statistics
    let rule_count = count_rules(&output_path).unwrap_or(0);
    let output_hash = compute_hash(&output_path).unwrap_or_default();

    // Handle copy to rules
    let (copied_to_rules, rules_destination) = if options.copy_to_rules {
        let rules_dir = options.rules_directory.clone().unwrap_or_else(|| {
            options
                .config_path
                .parent()
                .unwrap_or(Path::new("."))
                .join("..")
                .join("..")
                .join("rules")
        });
        let dest = rules_dir.join("adguard_user_filter.txt");
        match copy_to_rules(&output_path, &dest) {
            Ok(()) => (true, Some(dest)),
            Err(e) => {
                eprintln!("[WARN] Failed to copy to rules: {}", e);
                (false, None)
            }
        }
    } else {
        (false, None)
    };

    Ok(CompilerResult {
        success: true,
        config_name: config.name,
        config_version: config.version,
        rule_count,
        output_path,
        output_hash,
        copied_to_rules,
        rules_destination,
        elapsed_ms,
        start_time,
        end_time,
        error_message: None,
    })
}

/// Find the TypeScript compiler entry point.
fn find_typescript_compiler(config_path: &Path) -> Result<PathBuf> {
    // Look for the TypeScript compiler relative to config
    let search_paths = [
        config_path.parent().unwrap_or(Path::new(".")).join("src/cli.ts"),
        config_path.parent().unwrap_or(Path::new(".")).join("cli.ts"),
        PathBuf::from("src/cli.ts"),
        PathBuf::from("../src/cli.ts"),
    ];

    for path in search_paths {
        if path.exists() {
            return Ok(path);
        }
    }

    Err(CompilerError::CompilerNotFound(
        "src/cli.ts".to_string(),
    ))
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::io::Write;
    use tempfile::NamedTempFile;

    #[test]
    fn test_count_rules() {
        let mut file = NamedTempFile::new().unwrap();
        writeln!(file, "! Comment line").unwrap();
        writeln!(file, "# Another comment").unwrap();
        writeln!(file, "||example.com^").unwrap();
        writeln!(file, "").unwrap();
        writeln!(file, "||test.net^").unwrap();
        file.flush().unwrap();

        let count = count_rules(file.path()).unwrap();
        assert_eq!(count, 2);
    }

    #[test]
    fn test_compute_hash() {
        let mut file = NamedTempFile::new().unwrap();
        writeln!(file, "test content").unwrap();
        file.flush().unwrap();

        let hash = compute_hash(file.path()).unwrap();
        assert!(!hash.is_empty());
        assert_eq!(hash.len(), 96); // SHA-384 produces 96 hex chars
    }
}
