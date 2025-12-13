//! Command-line interface for the AdGuard Filter Rules Compiler.

use clap::Parser;
use std::path::PathBuf;
use std::process::ExitCode;

use rules_compiler::{
    compile_rules, get_version_info, read_configuration, ConfigurationFormat, VERSION,
};

/// AdGuard Filter Rules Compiler - Rust CLI
#[derive(Parser, Debug)]
#[command(name = "rules-compiler")]
#[command(version = VERSION)]
#[command(about = "Compile AdGuard filter rules using hostlist-compiler")]
#[command(long_about = None)]
struct Cli {
    /// Path to configuration file
    #[arg(short, long, value_name = "PATH")]
    config: Option<PathBuf>,

    /// Path to output file
    #[arg(short, long, value_name = "PATH")]
    output: Option<PathBuf>,

    /// Copy output to rules directory
    #[arg(short = 'r', long)]
    copy_to_rules: bool,

    /// Force configuration format (json, yaml, toml)
    #[arg(short, long, value_name = "FORMAT")]
    format: Option<String>,

    /// Show version information
    #[arg(short = 'V', long = "version-info")]
    version_info: bool,

    /// Enable debug output
    #[arg(short, long)]
    debug: bool,

    /// Show configuration only (don't compile)
    #[arg(long)]
    show_config: bool,
}

fn parse_format(format: &str) -> Option<ConfigurationFormat> {
    match format.to_lowercase().as_str() {
        "json" => Some(ConfigurationFormat::Json),
        "yaml" | "yml" => Some(ConfigurationFormat::Yaml),
        "toml" => Some(ConfigurationFormat::Toml),
        _ => None,
    }
}

fn show_version() {
    let info = get_version_info();

    println!("AdGuard Filter Rules Compiler (Rust API)");
    println!("Version: {}", info.module_version);
    println!();
    println!("Platform Information:");
    println!("  OS: {}", info.platform.os_name);
    println!("  Architecture: {}", info.platform.architecture);
    println!("  Rust: {}", info.rust_version);
    println!();
    println!(
        "  Node.js: {}",
        info.node_version.as_deref().unwrap_or("Not found")
    );
    println!(
        "  hostlist-compiler: {}",
        info.hostlist_compiler_version
            .as_deref()
            .unwrap_or("Not found")
    );
}

fn find_default_config() -> Option<PathBuf> {
    let search_paths = [
        PathBuf::from("compiler-config.json"),
        PathBuf::from("src/filter-compiler/compiler-config.json"),
    ];

    for path in search_paths {
        if path.exists() {
            return Some(path);
        }
    }

    None
}

fn main() -> ExitCode {
    let cli = Cli::parse();

    // Handle version info
    if cli.version_info {
        show_version();
        return ExitCode::SUCCESS;
    }

    // Determine config path
    let config_path = match cli.config {
        Some(path) => path,
        None => match find_default_config() {
            Some(path) => path,
            None => {
                eprintln!("[ERROR] Configuration file not found.");
                eprintln!("Searched:");
                eprintln!("  - compiler-config.json");
                eprintln!("  - src/rules-compiler-typescript/compiler-config.json");
                eprintln!();
                eprintln!("Specify config path with -c/--config");
                return ExitCode::FAILURE;
            }
        },
    };

    // Parse format
    let format = cli.format.as_deref().and_then(parse_format);

    // Show config only
    if cli.show_config {
        match read_configuration(&config_path, format) {
            Ok(config) => {
                println!("Configuration: {}", config_path.display());
                println!();
                println!("  Name: {}", config.name);
                println!("  Version: {}", config.version);
                println!("  License: {}", config.license);
                println!("  Sources: {}", config.sources.len());
                println!(
                    "  Transformations: {}",
                    config.transformations.join(", ")
                );
                return ExitCode::SUCCESS;
            }
            Err(e) => {
                eprintln!("[ERROR] Failed to read configuration: {}", e);
                return ExitCode::FAILURE;
            }
        }
    }

    // Run compilation
    println!("[INFO] Starting compilation with config: {}", config_path.display());

    match compile_rules(
        &config_path,
        cli.output.as_deref(),
        cli.copy_to_rules,
        None,
        format,
        cli.debug,
    ) {
        Ok(result) => {
            if result.success {
                println!();
                println!("Results:");
                println!("  Config Name:  {}", result.config_name);
                println!("  Config Ver:   {}", result.config_version);
                println!("  Rule Count:   {}", result.rule_count);
                println!("  Output Path:  {}", result.output_path);
                println!("  Hash:         {}...", &result.output_hash[..32]);
                println!("  Elapsed:      {}ms", result.elapsed_ms);

                if result.copied_to_rules {
                    println!(
                        "  Copied To:    {}",
                        result.rules_destination.as_deref().unwrap_or("N/A")
                    );
                }

                println!();
                println!("[INFO] Done!");
                ExitCode::SUCCESS
            } else {
                eprintln!(
                    "[ERROR] Compilation failed: {}",
                    result.error_message.as_deref().unwrap_or("Unknown error")
                );
                ExitCode::FAILURE
            }
        }
        Err(e) => {
            eprintln!("[ERROR] {}", e);
            ExitCode::FAILURE
        }
    }
}
