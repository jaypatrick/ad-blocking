//! CLI tool for AdGuard filter validation.

use adguard_validation::{
    Validator, ValidationConfig, VerificationMode, HashDatabase,
};
use clap::{Parser, Subcommand};
use std::path::PathBuf;

#[derive(Parser)]
#[command(name = "adguard-validate")]
#[command(about = "CLI tool for validating AdGuard filter lists")]
#[command(version)]
struct Cli {
    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    /// Validate a local filter file
    File {
        /// Path to the filter file
        path: PathBuf,
        
        /// Verification mode (strict, warning, disabled)
        #[arg(long, default_value = "warning")]
        mode: String,
    },
    
    /// Validate a remote URL
    Url {
        /// URL to validate
        url: String,
        
        /// Expected SHA-384 hash (optional)
        #[arg(long)]
        hash: Option<String>,
    },
    
    /// Show hash database information
    HashDb {
        /// Path to hash database
        #[arg(long, default_value = "data/input/.hashes.json")]
        path: PathBuf,
    },
}

fn main() -> anyhow::Result<()> {
    let cli = Cli::parse();

    match cli.command {
        Commands::File { path, mode } => {
            let verification_mode = match mode.as_str() {
                "strict" => VerificationMode::Strict,
                "warning" => VerificationMode::Warning,
                "disabled" => VerificationMode::Disabled,
                _ => {
                    eprintln!("Invalid mode: {mode}. Using 'warning' instead.");
                    VerificationMode::Warning
                }
            };

            let config = ValidationConfig::default()
                .with_verification_mode(verification_mode);
            
            let mut validator = Validator::new(config);
            
            println!("Validating file: {}", path.display());
            match validator.validate_local_file(&path) {
                Ok(result) => {
                    println!("✓ Syntax validation: {}", if result.is_valid { "PASSED" } else { "FAILED" });
                    println!("  Format: {:?}", result.format);
                    println!("  Valid rules: {}", result.valid_rules);
                    println!("  Invalid rules: {}", result.invalid_rules);
                    
                    if !result.messages.is_empty() {
                        println!("\nMessages:");
                        for msg in &result.messages {
                            println!("  - {msg}");
                        }
                    }
                    
                    if !result.is_valid {
                        std::process::exit(1);
                    }
                }
                Err(e) => {
                    eprintln!("✗ Validation failed: {e}");
                    std::process::exit(1);
                }
            }
        }
        
        Commands::Url { url, hash } => {
            let config = ValidationConfig::default();
            let validator = Validator::new(config);
            
            println!("Validating URL: {url}");
            match validator.validate_remote_url(&url, hash.as_deref()) {
                Ok(result) => {
                    println!("✓ URL validation: {}", if result.is_valid { "PASSED" } else { "FAILED" });
                    
                    if let Some(size) = result.content_size {
                        println!("  Content size: {} bytes", size);
                    }
                    
                    if let Some(hash) = &result.content_hash {
                        println!("  SHA-384: {hash}");
                    }
                    
                    if !result.messages.is_empty() {
                        println!("\nMessages:");
                        for msg in &result.messages {
                            println!("  - {msg}");
                        }
                    }
                    
                    if !result.is_valid {
                        std::process::exit(1);
                    }
                }
                Err(e) => {
                    eprintln!("✗ Validation failed: {e}");
                    std::process::exit(1);
                }
            }
        }
        
        Commands::HashDb { path } => {
            match HashDatabase::load(&path) {
                Ok(db) => {
                    println!("Hash database: {}", path.display());
                    println!("Entries: {}", db.len());
                    
                    if !db.is_empty() {
                        println!("\nStored hashes:");
                        for (file, entry) in &db.entries {
                            println!("  {file}");
                            println!("    Hash: {}", entry.hash);
                            println!("    Size: {} bytes", entry.size);
                            println!("    Last verified: {}", entry.last_verified);
                        }
                    }
                }
                Err(e) => {
                    eprintln!("Error loading hash database: {e}");
                    std::process::exit(1);
                }
            }
        }
    }

    Ok(())
}
