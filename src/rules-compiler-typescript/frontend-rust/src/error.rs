//! Error types for the rules compiler frontend.

use thiserror::Error;

/// Errors that can occur during compilation.
#[derive(Error, Debug)]
pub enum CompilerError {
    /// Configuration file not found
    #[error("Configuration file not found: {0}")]
    ConfigNotFound(String),

    /// Invalid configuration format
    #[error("Invalid configuration format: {0}")]
    InvalidFormat(String),

    /// IO error during file operations
    #[error("IO error: {0}")]
    Io(#[from] std::io::Error),

    /// JSON parsing error
    #[error("JSON parsing error: {0}")]
    Json(#[from] serde_json::Error),

    /// YAML parsing error
    #[error("YAML parsing error: {0}")]
    Yaml(#[from] serde_yaml::Error),

    /// TOML parsing error
    #[error("TOML parsing error: {0}")]
    Toml(#[from] toml::de::Error),

    /// Compilation failed
    #[error("Compilation failed: {0}")]
    CompilationFailed(String),

    /// Node.js not found
    #[error("Node.js not found. Please install Node.js 18+")]
    NodeNotFound,

    /// TypeScript compiler not found
    #[error("TypeScript compiler not found at: {0}")]
    CompilerNotFound(String),
}

/// Result type alias for compiler operations.
pub type Result<T> = std::result::Result<T, CompilerError>;
