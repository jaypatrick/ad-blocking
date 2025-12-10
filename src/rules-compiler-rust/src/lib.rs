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

pub mod config;
pub mod compiler;
pub mod error;

pub use config::{
    CompilerConfiguration,
    ConfigurationFormat,
    FilterSource,
    read_configuration,
    detect_format,
    to_json,
};

pub use compiler::{
    CompilerResult,
    VersionInfo,
    PlatformInfo,
    RulesCompiler,
    compile_rules,
    get_version_info,
    count_rules,
    compute_hash,
};

pub use error::CompilerError;

/// Library version
pub const VERSION: &str = env!("CARGO_PKG_VERSION");
