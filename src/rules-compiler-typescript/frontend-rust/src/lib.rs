//! Rules Compiler Frontend - Rust Library
//!
//! A Rust frontend for the TypeScript rules compiler, providing a native CLI
//! interface for compiling AdGuard filter rules.

pub mod compiler;
pub mod config;
pub mod error;

pub use compiler::{
    compile_via_typescript, compute_hash, copy_to_rules, count_rules, get_version_info,
    CompileOptions, CompilerResult, VersionInfo,
};
pub use config::{find_default_config, CompilerConfig, ConfigFormat, FilterSource};
pub use error::{CompilerError, Result};

/// Library version.
pub const VERSION: &str = env!("CARGO_PKG_VERSION");
