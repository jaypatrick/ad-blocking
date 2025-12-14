//! # Rules Compiler
//!
//! Rust API for compiling AdGuard filter rules using `@adguard/hostlist-compiler`.
//!
//! ## Example
//!
//! ```no_run
//! use rules_compiler::{RulesCompiler, ConfigurationFormat};
//!
//! let compiler = RulesCompiler::new();
//! let result = compiler.compile("config.yaml", None, true, None, None)?;
//!
//! if result.success {
//!     println!("Compiled {} rules", result.rule_count);
//! }
//! # Ok::<(), rules_compiler::CompilerError>(())
//! ```

pub mod compiler;
pub mod config;
pub mod error;

pub use config::{
    CompilerConfiguration, ConfigurationFormat, FilterSource, detect_format, read_configuration,
    to_json,
};

pub use compiler::{
    CompilerResult, PlatformInfo, RulesCompiler, VersionInfo, compile_rules, compute_hash,
    count_rules, get_version_info,
};

pub use error::CompilerError;

/// Library version
pub const VERSION: &str = env!("CARGO_PKG_VERSION");
