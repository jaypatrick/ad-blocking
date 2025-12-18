//! Command-line interface for the AdGuard Filter Rules Compiler.
//!
//! Provides both direct command-line arguments and an interactive menu-driven interface.

use clap::{Parser, Subcommand};
use dialoguer::{theme::ColorfulTheme, Confirm, Input, Select};
use std::path::PathBuf;
use std::process::ExitCode;

use rules_compiler::{
    CompileOptions, ConfigFormat, RulesCompiler, VersionInfo, read_config, VERSION,
};

/// AdGuard Filter Rules Compiler - Rust CLI
#[derive(Parser, Debug)]
#[command(name = "rules-compiler")]
#[command(version = VERSION)]
#[command(about = "Compile AdGuard filter rules using hostlist-compiler")]
#[command(long_about = "A high-performance Rust CLI for compiling AdGuard filter rules.\n\n\
    Supports JSON, YAML, and TOML configuration formats.\n\
    Can run in direct mode with arguments or interactive menu mode.")]
struct Cli {
    #[command(subcommand)]
    command: Option<Commands>,

    /// Path to configuration file
    #[arg(short, long, value_name = "PATH", global = true)]
    config: Option<PathBuf>,

    /// Path to output file
    #[arg(short, long, value_name = "PATH", global = true)]
    output: Option<PathBuf>,

    /// Copy output to rules directory
    #[arg(short = 'r', long, global = true)]
    copy_to_rules: bool,

    /// Force configuration format (json, yaml, toml)
    #[arg(short, long, value_name = "FORMAT", global = true)]
    format: Option<String>,

    /// Enable debug output
    #[arg(short, long, global = true)]
    debug: bool,

    /// Run in interactive menu mode
    #[arg(short, long)]
    interactive: bool,
}

#[derive(Subcommand, Debug)]
enum Commands {
    /// Compile filter rules from configuration
    Compile {
        /// Validate configuration before compiling
        #[arg(long)]
        validate: bool,
    },
    /// Show configuration details without compiling
    Config,
    /// Show version information for all components
    Version,
    /// Run interactive menu
    Menu,
}

/// Parse format string to ConfigFormat.
fn parse_format(format: &str) -> Option<ConfigFormat> {
    match format.to_lowercase().as_str() {
        "json" => Some(ConfigFormat::Json),
        "yaml" | "yml" => Some(ConfigFormat::Yaml),
        "toml" => Some(ConfigFormat::Toml),
        _ => None,
    }
}

/// Display version information.
fn show_version_info() {
    let info = VersionInfo::collect();

    println!();
    println!("╔════════════════════════════════════════════════════════════╗");
    println!("║     AdGuard Filter Rules Compiler (Rust API)               ║");
    println!("╚════════════════════════════════════════════════════════════╝");
    println!();
    println!("  Version:      {}", info.module_version);
    println!("  Rust:         {}", info.rust_version);
    println!();
    println!("  Platform:");
    println!("    OS:         {}", info.platform.os_name);
    println!("    Arch:       {}", info.platform.architecture);
    println!();
    println!("  Dependencies:");
    println!(
        "    Node.js:    {}",
        info.node_version.as_deref().unwrap_or("Not found")
    );
    println!(
        "    Compiler:   {}",
        info.hostlist_compiler_version
            .as_deref()
            .unwrap_or("Not found")
    );
    if let Some(path) = &info.hostlist_compiler_path {
        println!("    Path:       {path}");
    }
    println!();
}

/// Display configuration details.
fn show_config(config_path: &PathBuf, format: Option<ConfigFormat>) -> ExitCode {
    match read_config(config_path, format) {
        Ok(config) => {
            println!();
            println!("╔════════════════════════════════════════════════════════════╗");
            println!("║                    Configuration Details                   ║");
            println!("╚════════════════════════════════════════════════════════════╝");
            println!();
            println!("  File:         {}", config_path.display());
            println!(
                "  Format:       {}",
                config.format().map(|f| f.to_string()).unwrap_or_default()
            );
            println!();
            println!("  Name:         {}", config.name);
            println!("  Version:      {}", config.version);
            println!("  License:      {}", config.license);
            if !config.description.is_empty() {
                println!("  Description:  {}", config.description);
            }
            println!();
            println!("  Sources:      {} total", config.sources.len());
            println!("    Local:      {}", config.local_sources_count());
            println!("    Remote:     {}", config.remote_sources_count());
            println!();

            if !config.transformations.is_empty() {
                println!("  Transformations:");
                for t in &config.transformations {
                    println!("    - {t}");
                }
                println!();
            }

            println!("  Source Details:");
            for (i, source) in config.sources.iter().enumerate() {
                println!("    [{i}] {}", source.name);
                println!("        Type:   {}", source.source_type);
                println!("        Source: {}", source.source);
            }
            println!();

            ExitCode::SUCCESS
        }
        Err(e) => {
            eprintln!("[ERROR] Failed to read configuration: {e}");
            ExitCode::FAILURE
        }
    }
}

/// Run compilation with the given options.
fn run_compile(
    config_path: &PathBuf,
    output: Option<PathBuf>,
    copy_to_rules: bool,
    format: Option<ConfigFormat>,
    debug: bool,
    validate: bool,
) -> ExitCode {
    let options = CompileOptions::new()
        .with_copy_to_rules(copy_to_rules)
        .with_debug(debug)
        .with_validation(validate);

    let options = if let Some(path) = output {
        options.with_output(path)
    } else {
        options
    };

    let options = if let Some(fmt) = format {
        options.with_format(fmt)
    } else {
        options
    };

    let compiler = RulesCompiler::with_options(options);

    println!();
    println!("╔════════════════════════════════════════════════════════════╗");
    println!("║                  Compiling Filter Rules                    ║");
    println!("╚════════════════════════════════════════════════════════════╝");
    println!();
    println!("  Config: {}", config_path.display());
    println!();

    match compiler.compile(config_path) {
        Ok(result) => {
            if result.success {
                println!("  ✓ Compilation successful!");
                println!();
                println!("  Results:");
                println!("    Filter:     {} v{}", result.config_name, result.config_version);
                println!("    Rules:      {}", result.rule_count);
                println!("    Output:     {}", result.output_path_str());
                println!("    Hash:       {}...", result.hash_short());
                println!("    Elapsed:    {}", result.elapsed_formatted());

                if result.copied_to_rules {
                    println!();
                    println!(
                        "  ✓ Copied to:  {}",
                        result.rules_destination_str().unwrap_or_default()
                    );
                }

                println!();
                ExitCode::SUCCESS
            } else {
                eprintln!(
                    "  ✗ Compilation failed: {}",
                    result.error_message.as_deref().unwrap_or("Unknown error")
                );
                if !result.stderr.is_empty() {
                    eprintln!();
                    eprintln!("  Stderr:");
                    for line in result.stderr.lines() {
                        eprintln!("    {line}");
                    }
                }
                eprintln!();
                ExitCode::FAILURE
            }
        }
        Err(e) => {
            eprintln!("  ✗ Error: {e}");
            eprintln!();
            ExitCode::FAILURE
        }
    }
}

/// Find default configuration file.
fn find_default_config() -> Option<PathBuf> {
    let search_paths = [
        PathBuf::from("compiler-config.json"),
        PathBuf::from("compiler-config.yaml"),
        PathBuf::from("compiler-config.toml"),
        PathBuf::from("src/rules-compiler-typescript/compiler-config.json"),
    ];

    search_paths.into_iter().find(|path| path.exists())
}

/// Interactive menu loop.
fn run_interactive_menu(initial_config: Option<PathBuf>) -> ExitCode {
    let theme = ColorfulTheme::default();
    let mut config_path = initial_config.or_else(find_default_config);

    println!();
    println!("╔════════════════════════════════════════════════════════════╗");
    println!("║     AdGuard Filter Rules Compiler - Interactive Mode       ║");
    println!("╚════════════════════════════════════════════════════════════╝");
    println!();

    loop {
        let menu_items = vec![
            "Compile Rules",
            "View Configuration",
            "Change Configuration File",
            "Version Information",
            "Exit",
        ];

        let current_config = config_path
            .as_ref()
            .map(|p| p.display().to_string())
            .unwrap_or_else(|| "Not set".to_string());

        println!("  Current config: {current_config}");
        println!();

        let selection = Select::with_theme(&theme)
            .with_prompt("Select an action")
            .items(&menu_items)
            .default(0)
            .interact();

        let selection = match selection {
            Ok(s) => s,
            Err(_) => {
                println!();
                println!("  Exiting...");
                return ExitCode::SUCCESS;
            }
        };

        println!();

        match selection {
            0 => {
                // Compile Rules
                if let Some(ref path) = config_path {
                    let copy_to_rules = Confirm::with_theme(&theme)
                        .with_prompt("Copy output to rules directory?")
                        .default(false)
                        .interact()
                        .unwrap_or(false);

                    let validate = Confirm::with_theme(&theme)
                        .with_prompt("Validate configuration before compiling?")
                        .default(true)
                        .interact()
                        .unwrap_or(true);

                    run_compile(path, None, copy_to_rules, None, false, validate);
                } else {
                    eprintln!("  No configuration file selected.");
                    eprintln!("  Use 'Change Configuration File' to select one.");
                    eprintln!();
                }
            }
            1 => {
                // View Configuration
                if let Some(ref path) = config_path {
                    show_config(path, None);
                } else {
                    eprintln!("  No configuration file selected.");
                    eprintln!();
                }
            }
            2 => {
                // Change Configuration File
                let input: Result<String, _> = Input::with_theme(&theme)
                    .with_prompt("Enter configuration file path")
                    .with_initial_text(
                        config_path
                            .as_ref()
                            .map(|p| p.display().to_string())
                            .unwrap_or_default(),
                    )
                    .interact_text();

                if let Ok(path_str) = input {
                    let path = PathBuf::from(path_str.trim());
                    if path.exists() {
                        config_path = Some(path);
                        println!("  ✓ Configuration file updated.");
                    } else {
                        eprintln!("  ✗ File not found: {}", path.display());
                    }
                }
                println!();
            }
            3 => {
                // Version Information
                show_version_info();
            }
            4 => {
                // Exit
                println!("  Goodbye!");
                println!();
                return ExitCode::SUCCESS;
            }
            _ => {}
        }
    }
}

fn main() -> ExitCode {
    let cli = Cli::parse();

    // Parse format if provided
    let format = cli.format.as_deref().and_then(parse_format);

    // Handle interactive mode
    if cli.interactive || matches!(cli.command, Some(Commands::Menu)) {
        return run_interactive_menu(cli.config);
    }

    // Handle subcommands
    match cli.command {
        Some(Commands::Version) => {
            show_version_info();
            ExitCode::SUCCESS
        }
        Some(Commands::Config) => {
            let config_path = match cli.config.or_else(find_default_config) {
                Some(path) => path,
                None => {
                    eprintln!("[ERROR] No configuration file specified or found.");
                    eprintln!("Use -c/--config to specify a configuration file.");
                    return ExitCode::FAILURE;
                }
            };
            show_config(&config_path, format)
        }
        Some(Commands::Compile { validate }) => {
            let config_path = match cli.config.or_else(find_default_config) {
                Some(path) => path,
                None => {
                    eprintln!("[ERROR] No configuration file specified or found.");
                    eprintln!();
                    eprintln!("Searched for:");
                    eprintln!("  - compiler-config.json");
                    eprintln!("  - compiler-config.yaml");
                    eprintln!("  - compiler-config.toml");
                    eprintln!("  - src/rules-compiler-typescript/compiler-config.json");
                    eprintln!();
                    eprintln!("Use -c/--config to specify a configuration file,");
                    eprintln!("or -i/--interactive for menu mode.");
                    return ExitCode::FAILURE;
                }
            };

            run_compile(
                &config_path,
                cli.output,
                cli.copy_to_rules,
                format,
                cli.debug,
                validate,
            )
        }
        None => {
            let config_path = match cli.config.or_else(find_default_config) {
                Some(path) => path,
                None => {
                    eprintln!("[ERROR] No configuration file specified or found.");
                    eprintln!();
                    eprintln!("Searched for:");
                    eprintln!("  - compiler-config.json");
                    eprintln!("  - compiler-config.yaml");
                    eprintln!("  - compiler-config.toml");
                    eprintln!("  - src/rules-compiler-typescript/compiler-config.json");
                    eprintln!();
                    eprintln!("Use -c/--config to specify a configuration file,");
                    eprintln!("or -i/--interactive for menu mode.");
                    return ExitCode::FAILURE;
                }
            };

            run_compile(
                &config_path,
                cli.output,
                cli.copy_to_rules,
                format,
                cli.debug,
                false,
            )
        }
        Some(Commands::Menu) => run_interactive_menu(cli.config),
    }
}
