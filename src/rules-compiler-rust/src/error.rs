//! Error types for the rules compiler.

use thiserror::Error;

/// Errors that can occur during compilation.
#[derive(Error, Debug)]
#[non_exhaustive]
pub enum CompilerError {
    /// Configuration file not found
    #[error("Configuration file not found: {0}")]
    ConfigNotFound(String),

    /// Invalid configuration format
    #[error("Invalid configuration format: {0}")]
    InvalidFormat(String),

    /// JSON parsing error
    #[error("Invalid JSON: {0}")]
    JsonError(#[from] serde_json::Error),

    /// YAML parsing error
    #[error("Invalid YAML: {0}")]
    YamlError(#[from] serde_yaml::Error),

    /// TOML parsing error
    #[error("Invalid TOML: {0}")]
    TomlError(#[from] toml::de::Error),

    /// IO error
    #[error("IO error: {0}")]
    IoError(#[from] std::io::Error),

    /// Compiler not found
    #[error("hostlist-compiler not found. Install with: npm install -g @adguard/hostlist-compiler")]
    CompilerNotFound,

    /// Compilation failed
    #[error("Compilation failed: {0}")]
    CompilationFailed(String),

    /// Unknown configuration extension
    #[error("Unknown configuration file extension: {0}")]
    UnknownExtension(String),
}

/// Result type for compiler operations.
pub type Result<T> = std::result::Result<T, CompilerError>;
