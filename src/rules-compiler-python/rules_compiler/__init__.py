"""
Rules Compiler - Python API for AdGuard filter rule compilation.

This package provides a Python interface for compiling AdGuard filter rules
using the @adguard/hostlist-compiler CLI tool.

Example:
    >>> from rules_compiler import RulesCompiler
    >>> compiler = RulesCompiler()
    >>> result = compiler.compile("compiler-config.yaml", copy_to_rules=True)
    >>> print(f"Compiled {result.rule_count} rules")
"""

# Define __version__ early to avoid circular imports (cli.py imports __version__)
__version__ = "1.0.0"

from rules_compiler.config import (
    ConfigurationFormat,
    CompilerConfiguration,
    FilterSource,
    read_configuration,
    detect_format,
    to_json,
)
from rules_compiler.compiler import (
    CompilerResult,
    VersionInfo,
    PlatformInfo,
    RulesCompiler,
    compile_rules,
    get_version_info,
)
from rules_compiler.cli import main

__all__ = [
    # Config
    "ConfigurationFormat",
    "CompilerConfiguration",
    "FilterSource",
    "read_configuration",
    "detect_format",
    "to_json",
    # Compiler
    "CompilerResult",
    "VersionInfo",
    "PlatformInfo",
    "RulesCompiler",
    "compile_rules",
    "get_version_info",
    # CLI
    "main",
]
