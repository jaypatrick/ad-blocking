#!/usr/bin/env python3
"""
Command-line interface for the AdGuard Filter Rules Compiler.
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from rules_compiler import __version__
from rules_compiler.compiler import RulesCompiler, get_version_info
from rules_compiler.config import ConfigurationFormat


def create_parser() -> argparse.ArgumentParser:
    """Create the argument parser."""
    parser = argparse.ArgumentParser(
        prog="rules-compiler",
        description="AdGuard Filter Rules Compiler - Python API",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  rules-compiler                           # Use default config
  rules-compiler -c config.yaml -r         # Use YAML config, copy to rules
  rules-compiler --config config.toml      # Use TOML config
  rules-compiler -v                        # Show version info
        """,
    )

    parser.add_argument(
        "-c", "--config",
        metavar="PATH",
        help="Path to configuration file (default: compiler-config.json)",
    )

    parser.add_argument(
        "-o", "--output",
        metavar="PATH",
        help="Path to output file (default: output/compiled-TIMESTAMP.txt)",
    )

    parser.add_argument(
        "-r", "--copy-to-rules",
        action="store_true",
        help="Copy output to rules directory",
    )

    parser.add_argument(
        "-f", "--format",
        choices=["json", "yaml", "toml"],
        help="Force configuration format (default: auto-detect)",
    )

    parser.add_argument(
        "-v", "--version",
        action="store_true",
        help="Show version information and exit",
    )

    parser.add_argument(
        "-d", "--debug",
        action="store_true",
        help="Enable debug output",
    )

    return parser


def show_version() -> None:
    """Display version information."""
    info = get_version_info()

    print("AdGuard Filter Rules Compiler (Python API)")
    print(f"Version: {info.module_version}")
    print()
    print("Platform Information:")
    print(f"  OS: {info.platform.os_name}")
    print(f"  Architecture: {info.platform.architecture}")
    print(f"  Python: {info.python_version}")
    print()
    print(f"  Node.js: {info.node_version or 'Not found'}")
    print(f"  hostlist-compiler: {info.hostlist_compiler_version or 'Not found'}")


def main(args: list[str] | None = None) -> int:
    """
    Main CLI entry point.

    Args:
        args: Command line arguments (defaults to sys.argv[1:]).

    Returns:
        Exit code (0 for success, 1 for failure).
    """
    parser = create_parser()
    opts = parser.parse_args(args)

    # Handle version
    if opts.version:
        show_version()
        return 0

    # Determine config path
    if opts.config:
        config_path = Path(opts.config).resolve()
    else:
        # Search for default config
        search_paths = [
            Path.cwd() / "compiler-config.json",
            Path.cwd() / "src" / "rules-compiler-typescript" / "compiler-config.json",
        ]

        config_path = None
        for path in search_paths:
            if path.exists():
                config_path = path
                break

        if config_path is None:
            print(f"Error: Configuration file not found. Searched:", file=sys.stderr)
            for path in search_paths:
                print(f"  - {path}", file=sys.stderr)
            print("\nSpecify config path with -c/--config", file=sys.stderr)
            return 1

    # Parse format
    format_map = {
        "json": ConfigurationFormat.JSON,
        "yaml": ConfigurationFormat.YAML,
        "toml": ConfigurationFormat.TOML,
    }
    config_format = format_map.get(opts.format) if opts.format else None

    # Create compiler and run
    compiler = RulesCompiler(debug=opts.debug)

    try:
        print(f"[INFO] Starting compilation with config: {config_path}")

        result = compiler.compile(
            config_path=config_path,
            output_path=opts.output,
            copy_to_rules=opts.copy_to_rules,
            format=config_format,
        )

        if result.success:
            print()
            print("Results:")
            print(f"  Config Name:  {result.config_name}")
            print(f"  Config Ver:   {result.config_version}")
            print(f"  Rule Count:   {result.rule_count:,}")
            print(f"  Output Path:  {result.output_path}")
            print(f"  Hash:         {result.output_hash[:32]}...")
            print(f"  Elapsed:      {result.elapsed_ms}ms")

            if result.copied_to_rules:
                print(f"  Copied To:    {result.rules_destination}")

            print()
            print("[INFO] Done!")
            return 0
        else:
            print(f"[ERROR] Compilation failed: {result.error_message}", file=sys.stderr)
            return 1

    except Exception as e:
        print(f"[ERROR] {e}", file=sys.stderr)
        if opts.debug:
            import traceback
            traceback.print_exc()
        return 1


if __name__ == "__main__":
    sys.exit(main())
