"""
Rules Compiler - Python API for AdGuard filter rule compilation.

This package provides a Python interface for compiling AdGuard filter rules
using the @adguard/hostlist-compiler CLI tool.

Example:
    >>> from rules_compiler import RulesCompiler
    >>> compiler = RulesCompiler()
    >>> result = compiler.compile("compiler-config.yaml", copy_to_rules=True)
    >>> print(f"Compiled {result.rule_count} rules in {result.elapsed_formatted()}")

Features:
    - Multi-format configuration support (JSON, YAML, TOML)
    - Configuration validation
    - Custom error types for better error handling
    - Transformation presets (recommended, minimal, hosts)
    - Helper methods for result formatting
"""

# Define __version__ early to avoid circular imports (cli.py imports __version__)
__version__ = "2.0.0"

from rules_compiler.config import (
    ConfigurationFormat,
    CompilerConfiguration,
    FilterSource,
    SourceType,
    Transformation,
    read_configuration,
    detect_format,
    to_json,
    to_yaml,
    to_toml,
)
from rules_compiler.compiler import (
    CompilerResult,
    VersionInfo,
    PlatformInfo,
    RulesCompiler,
    compile_rules,
    validate_configuration,
    get_version_info,
    get_platform_info,
    count_rules,
    compute_hash,
    hash_short,
    format_elapsed,
    find_command,
)
from rules_compiler.errors import (
    CompilerError,
    ConfigNotFoundError,
    UnknownExtensionError,
    ParseError,
    ValidationError,
    CompilerNotFoundError,
    CompilationError,
    OutputNotCreatedError,
    CopyError,
    TimeoutError,
    ValidationResult,
    ErrorCode,
)
from rules_compiler.cli import main

__all__ = [
    # Version
    "__version__",
    # Config types
    "ConfigurationFormat",
    "CompilerConfiguration",
    "FilterSource",
    "SourceType",
    "Transformation",
    # Config functions
    "read_configuration",
    "detect_format",
    "to_json",
    "to_yaml",
    "to_toml",
    # Compiler types
    "CompilerResult",
    "VersionInfo",
    "PlatformInfo",
    "RulesCompiler",
    # Compiler functions
    "compile_rules",
    "validate_configuration",
    "get_version_info",
    "get_platform_info",
    "count_rules",
    "compute_hash",
    "hash_short",
    "format_elapsed",
    "find_command",
    # Error types
    "CompilerError",
    "ConfigNotFoundError",
    "UnknownExtensionError",
    "ParseError",
    "ValidationError",
    "CompilerNotFoundError",
    "CompilationError",
    "OutputNotCreatedError",
    "CopyError",
    "TimeoutError",
    "ValidationResult",
    "ErrorCode",
    # CLI
    "main",
]
