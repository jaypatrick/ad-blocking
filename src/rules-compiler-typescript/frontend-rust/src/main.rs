//! Rules Compiler Frontend - Rust CLI
//!
//! A native Rust CLI for the TypeScript rules compiler.

use clap::Parser;
use rules_compiler_frontend::{
    compile_via_typescript, find_default_config, get_version_info, CompileOptions, CompilerConfig,
    ConfigFormat, VERSION,
};
use std::path::PathBuf;
use std::process::ExitCode;

/// AdGuard Filter Rules Compiler - Rust Frontend
#[derive(Parser, Debug)]
#[command(name = "rules-compiler-frontend")]
#[command(version = VERSION)]
#[command(about = "Rust frontend for the TypeScript rules compiler")]
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

    /// Custom rules directory path
    #[arg(long, value_name = "PATH")]
    rules_dir: Option<PathBuf>,

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

fn parse_format(format: &str) -> Option<ConfigFormat> {
    ConfigFormat::from_str(format).ok()
}

fn show_version() {
    let info = get_version_info();

    println!("AdGuard Filter Rules Compiler (Rust Frontend)");
    println!("Version: {}", info.module_version);
    println!();
    println!("Platform Information:");
    println!("  OS: {}", info.os);
    println!("  Architecture: {}", info.arch);
    println!("  Rust: {}", info.rust_version);
    println!();
    println!(
        "  Node.js: {}",
        info.node_version.as_deref().unwrap_or("Not found")
    );
}

fn show_config(config_path: &PathBuf, format: Option<ConfigFormat>) {
    match CompilerConfig::from_file(config_path, format) {
        Ok(config) => {
            println!("Configuration: {}", config_path.display());
            println!();
            println!("  Name: {}", config.name);
            println!("  Version: {}", config.version);
            println!(
                "  License: {}",
                config.license.as_deref().unwrap_or("N/A")
            );
            println!("  Sources: {}", config.sources.len());
            println!("  Transformations: {}", config.transformations.join(", "));
            println!();
            println!("JSON representation:");
            if let Ok(json) = config.to_json() {
                println!("{}", json);
            }
        }
        Err(e) => {
            eprintln!("[ERROR] Failed to read configuration: {}", e);
        }
    }
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
                eprintln!("  - compiler-config.yaml");
                eprintln!("  - compiler-config.yml");
                eprintln!("  - compiler-config.toml");
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
        show_config(&config_path, format);
        return ExitCode::SUCCESS;
    }

    // Run compilation
    println!(
        "[INFO] Starting compilation with config: {}",
        config_path.display()
    );

    let options = CompileOptions {
        config_path: config_path.clone(),
        output_path: cli.output,
        copy_to_rules: cli.copy_to_rules,
        rules_directory: cli.rules_dir,
        format,
        debug: cli.debug,
    };

    match compile_via_typescript(&options) {
        Ok(result) => {
            if result.success {
                println!();
                println!("Results:");
                println!("  Config Name:  {}", result.config_name);
                println!("  Config Ver:   {}", result.config_version);
                println!("  Rule Count:   {}", result.rule_count);
                println!("  Output Path:  {}", result.output_path.display());
                println!("  Hash:         {}...", &result.output_hash[..32.min(result.output_hash.len())]);
                println!("  Elapsed:      {}ms", result.elapsed_ms);

                if result.copied_to_rules {
                    if let Some(dest) = &result.rules_destination {
                        println!("  Copied To:    {}", dest.display());
                    }
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
