//! # AdGuard Validation Core Library
//!
//! Centralized validation library for AdGuard filter compilation with comprehensive
//! security features including hash verification, URL security validation, and syntax checking.
//!
//! This library is designed to be used across multiple compilers (TypeScript, .NET, Python, Rust)
//! through native bindings, FFI, or WebAssembly.
//!
//! ## Features
//!
//! - **At-Rest Hash Verification**: SHA-384 hashing for local files with database management
//! - **In-Flight Hash Verification**: SHA-384 verification for downloaded files (prevents MITM)
//! - **URL Security Validation**: HTTPS enforcement, domain validation, content verification
//! - **Syntax Validation**: Automatic linting for adblock and hosts file formats
//! - **File Conflict Handling**: Automatic renaming, overwrite, or error strategies
//! - **Archiving**: Timestamped archiving with manifest tracking and retention policies
//!
//! ## Quick Start
//!
//! ```no_run
//! use adguard_validation::{Validator, ValidationConfig, VerificationMode};
//!
//! # fn main() -> Result<(), Box<dyn std::error::Error>> {
//! let config = ValidationConfig::default()
//!     .with_verification_mode(VerificationMode::Strict);
//!
//! let mut validator = Validator::new(config);
//!
//! // Validate a local file
//! let _result = validator.validate_local_file("data/input/custom-rules.txt")?;
//!
//! // Validate a remote URL
//! let _result = validator.validate_remote_url("https://example.com/list.txt", None)?;
//! # Ok(())
//! # }
//! ```

pub mod hash;
pub mod url_security;
pub mod syntax;
pub mod archive;
pub mod file_conflict;
pub mod config;
pub mod error;
pub mod validator;

// Re-export main types
pub use config::{
    ValidationConfig,
    VerificationMode,
    ArchivingConfig,
    ArchivingMode,
    OutputConfig,
    ConflictStrategy,
    HashVerificationConfig,
};

pub use error::{ValidationError, Result};
pub use validator::Validator;
pub use hash::{HashDatabase, HashEntry, compute_file_hash, verify_file_hash};
pub use url_security::{validate_url, UrlValidationResult};
pub use syntax::{validate_syntax, SyntaxValidationResult, FilterFormat};
pub use archive::{create_archive, ArchiveManifest};
pub use file_conflict::{resolve_conflict, FileConflictResolver};

/// Library version.
pub const VERSION: &str = env!("CARGO_PKG_VERSION");

/// Library name.
pub const NAME: &str = env!("CARGO_PKG_NAME");

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_version() {
        assert!(!VERSION.is_empty());
    }

    #[test]
    fn test_name() {
        assert_eq!(NAME, "adguard-validation-core");
    }
}
