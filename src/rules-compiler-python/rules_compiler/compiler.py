"""
Core compiler functionality for AdGuard filter rules.
"""

from __future__ import annotations

import hashlib
import json
import logging
import os
import platform
import shutil
import subprocess
import tempfile
from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path
from typing import Any

from rules_compiler.config import (
    CompilerConfiguration,
    ConfigurationFormat,
    read_configuration,
    to_json,
)
from rules_compiler.errors import (
    CompilationError,
    CompilerNotFoundError,
    CopyError,
    OutputNotCreatedError,
    TimeoutError as CompilerTimeoutError,
    ValidationError,
)

logger = logging.getLogger(__name__)


@dataclass
class PlatformInfo:
    """Platform-specific information."""
    os_name: str = ""
    os_version: str = ""
    architecture: str = ""
    is_windows: bool = False
    is_linux: bool = False
    is_macos: bool = False


@dataclass
class VersionInfo:
    """Version information for all components."""
    module_version: str = ""
    python_version: str = ""
    node_version: str | None = None
    hostlist_compiler_version: str | None = None
    hostlist_compiler_path: str | None = None
    platform: PlatformInfo = field(default_factory=PlatformInfo)

    def has_node(self) -> bool:
        """Check if Node.js is available."""
        return self.node_version is not None

    def has_compiler(self) -> bool:
        """Check if hostlist-compiler is available."""
        return self.hostlist_compiler_version is not None

    @classmethod
    def collect(cls) -> VersionInfo:
        """Collect version information from the system."""
        return get_version_info()


@dataclass
class CompilerResult:
    """Result of a compilation operation."""
    success: bool = False
    config_name: str = ""
    config_version: str = ""
    rule_count: int = 0
    output_path: str = ""
    output_hash: str = ""
    copied_to_rules: bool = False
    rules_destination: str | None = None
    elapsed_ms: int = 0
    start_time: datetime = field(default_factory=datetime.utcnow)
    end_time: datetime = field(default_factory=datetime.utcnow)
    error_message: str | None = None
    stdout: str = ""
    stderr: str = ""

    def elapsed_formatted(self) -> str:
        """Get elapsed time in human-readable format."""
        if self.elapsed_ms >= 1000:
            return f"{self.elapsed_ms / 1000:.2f}s"
        return f"{self.elapsed_ms}ms"

    def hash_short(self, length: int = 32) -> str:
        """Get shortened hash for display."""
        if len(self.output_hash) > length:
            return self.output_hash[:length]
        return self.output_hash

    def output_path_str(self) -> str:
        """Get output path as string."""
        return str(self.output_path) if self.output_path else ""

    def rules_destination_str(self) -> str | None:
        """Get rules destination as string or None."""
        return str(self.rules_destination) if self.rules_destination else None


def get_platform_info() -> PlatformInfo:
    """Get information about the current platform."""
    system = platform.system().lower()
    return PlatformInfo(
        os_name=platform.system(),
        os_version=platform.version(),
        architecture=platform.machine(),
        is_windows=system == "windows",
        is_linux=system == "linux",
        is_macos=system == "darwin",
    )


def find_command(command: str) -> str | None:
    """Find a command in PATH."""
    return shutil.which(command)


def get_version_info() -> VersionInfo:
    """Get version information for all components."""
    from rules_compiler import __version__

    info = VersionInfo(
        module_version=__version__,
        python_version=platform.python_version(),
        platform=get_platform_info(),
    )

    # Check Node.js
    node_path = find_command("node")
    if node_path:
        try:
            result = subprocess.run(
                [node_path, "--version"],
                capture_output=True,
                text=True,
                timeout=10,
            )
            if result.returncode == 0:
                info.node_version = result.stdout.strip()
        except Exception:
            pass

    # Check hostlist-compiler
    compiler_path = find_command("hostlist-compiler")
    if compiler_path:
        info.hostlist_compiler_path = compiler_path
        try:
            result = subprocess.run(
                [compiler_path, "--version"],
                capture_output=True,
                text=True,
                timeout=10,
            )
            if result.returncode == 0:
                info.hostlist_compiler_version = result.stdout.strip().split("\n")[0]
        except Exception:
            pass
    else:
        # Check npx
        npx_path = find_command("npx")
        if npx_path:
            info.hostlist_compiler_path = f"{npx_path} @adguard/hostlist-compiler"

    return info


def count_rules(file_path: str | Path) -> int:
    """
    Count non-empty, non-comment lines in a file.

    Args:
        file_path: Path to the file.

    Returns:
        Number of rules.
    """
    path = Path(file_path)
    if not path.exists():
        return 0

    count = 0
    with open(path, "r", encoding="utf-8") as f:
        for line in f:
            stripped = line.strip()
            if stripped and not stripped.startswith(("!", "#")):
                count += 1

    return count


def compute_hash(file_path: str | Path) -> str:
    """
    Compute SHA-384 hash of a file.

    Args:
        file_path: Path to the file.

    Returns:
        Hex-encoded hash string.
    """
    sha384 = hashlib.sha384()
    with open(file_path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            sha384.update(chunk)
    return sha384.hexdigest()


def hash_short(hash_value: str, length: int = 32) -> str:
    """
    Get shortened hash for display.

    Args:
        hash_value: Full hash string.
        length: Maximum length to return.

    Returns:
        Shortened hash string.
    """
    if len(hash_value) > length:
        return hash_value[:length]
    return hash_value


def format_elapsed(elapsed_ms: int) -> str:
    """
    Format elapsed time in human-readable format.

    Args:
        elapsed_ms: Elapsed time in milliseconds.

    Returns:
        Formatted string (e.g., "1.50s" or "500ms").
    """
    if elapsed_ms >= 1000:
        return f"{elapsed_ms / 1000:.2f}s"
    return f"{elapsed_ms}ms"


def _get_compiler_command(config_path: str, output_path: str) -> tuple[list[str], str]:
    """
    Get the compiler command and working directory.

    Returns:
        Tuple of (command args, working directory).

    Raises:
        CompilerNotFoundError: If hostlist-compiler is not found.
    """
    compiler_path = find_command("hostlist-compiler")

    if compiler_path:
        return (
            [compiler_path, "--config", config_path, "--output", output_path],
            str(Path(config_path).parent),
        )

    npx_path = find_command("npx")
    if npx_path:
        return (
            [npx_path, "@adguard/hostlist-compiler", "--config", config_path, "--output", output_path],
            str(Path(config_path).parent),
        )

    raise CompilerNotFoundError(["hostlist-compiler", "npx"])


class RulesCompiler:
    """
    Main compiler class for AdGuard filter rules.

    Example:
        >>> compiler = RulesCompiler()
        >>> result = compiler.compile("config.yaml", copy_to_rules=True)
        >>> if result.success:
        ...     print(f"Compiled {result.rule_count} rules")
    """

    def __init__(self, debug: bool = False):
        """
        Initialize the compiler.

        Args:
            debug: Enable debug logging.
        """
        self.debug = debug
        if debug:
            logging.basicConfig(level=logging.DEBUG)

    def compile(
        self,
        config_path: str | Path,
        output_path: str | Path | None = None,
        copy_to_rules: bool = False,
        rules_directory: str | Path | None = None,
        format: ConfigurationFormat | None = None,
        validate: bool = True,
    ) -> CompilerResult:
        """
        Compile filter rules.

        Args:
            config_path: Path to configuration file.
            output_path: Optional output file path.
            copy_to_rules: Copy output to rules directory.
            rules_directory: Custom rules directory path.
            format: Force configuration format.
            validate: Validate configuration before compiling.

        Returns:
            Compilation result.
        """
        return compile_rules(
            config_path=config_path,
            output_path=output_path,
            copy_to_rules=copy_to_rules,
            rules_directory=rules_directory,
            format=format,
            debug=self.debug,
            validate=validate,
        )

    def read_config(
        self,
        config_path: str | Path,
        format: ConfigurationFormat | None = None,
    ) -> CompilerConfiguration:
        """
        Read configuration from a file.

        Args:
            config_path: Path to configuration file.
            format: Force configuration format.

        Returns:
            Parsed configuration.
        """
        return read_configuration(config_path, format)

    def validate_config(
        self,
        config: CompilerConfiguration,
        check_files: bool = False,
    ) -> tuple[bool, list[str], list[str]]:
        """
        Validate a configuration.

        Args:
            config: Configuration to validate.
            check_files: Check if local source files exist.

        Returns:
            Tuple of (is_valid, errors, warnings).
        """
        result = config.validate(check_files=check_files)
        return result.is_valid, result.errors, result.warnings

    def get_version_info(self) -> VersionInfo:
        """Get version information for all components."""
        return get_version_info()


def compile_rules(
    config_path: str | Path,
    output_path: str | Path | None = None,
    copy_to_rules: bool = False,
    rules_directory: str | Path | None = None,
    format: ConfigurationFormat | None = None,
    debug: bool = False,
    validate: bool = True,
) -> CompilerResult:
    """
    Compile filter rules using hostlist-compiler.

    Args:
        config_path: Path to configuration file.
        output_path: Optional output file path.
        copy_to_rules: Copy output to rules directory.
        rules_directory: Custom rules directory path.
        format: Force configuration format.
        debug: Enable debug logging.
        validate: Validate configuration before compiling.

    Returns:
        Compilation result.
    """
    result = CompilerResult(start_time=datetime.utcnow())
    config_path = Path(config_path).resolve()
    temp_config_path = None

    try:
        # Read configuration
        config = read_configuration(config_path, format)
        result.config_name = config.name
        result.config_version = config.version

        # Validate configuration if requested
        if validate:
            validation_result = config.validate()
            if not validation_result.is_valid:
                raise ValidationError(validation_result.errors, validation_result.warnings)
            if validation_result.warnings and debug:
                for warning in validation_result.warnings:
                    logger.warning(f"Config warning: {warning}")

        # Determine output path
        if output_path:
            actual_output = Path(output_path).resolve()
        else:
            timestamp = datetime.utcnow().strftime("%Y%m%d-%H%M%S")
            output_dir = config_path.parent / "output"
            output_dir.mkdir(exist_ok=True)
            actual_output = output_dir / f"compiled-{timestamp}.txt"

        result.output_path = str(actual_output)

        # Convert to JSON if needed (hostlist-compiler only supports JSON)
        detected_format = format or config._source_format
        if detected_format != ConfigurationFormat.JSON:
            temp_config_path = tempfile.NamedTemporaryFile(
                mode="w",
                suffix=".json",
                delete=False,
                encoding="utf-8",
            )
            temp_config_path.write(to_json(config))
            temp_config_path.close()
            compile_config_path = temp_config_path.name
            if debug:
                logger.debug(f"Created temp JSON config: {compile_config_path}")
        else:
            compile_config_path = str(config_path)

        # Get compiler command
        cmd, cwd = _get_compiler_command(compile_config_path, str(actual_output))

        if debug:
            logger.debug(f"Running: {' '.join(cmd)}")
            logger.debug(f"Working directory: {cwd}")

        # Run compilation
        try:
            proc = subprocess.run(
                cmd,
                cwd=cwd,
                capture_output=True,
                text=True,
                timeout=300,  # 5 minute timeout
            )
        except subprocess.TimeoutExpired:
            raise CompilerTimeoutError(300, " ".join(cmd))

        result.stdout = proc.stdout
        result.stderr = proc.stderr

        if proc.returncode != 0:
            raise CompilationError(
                f"Compiler exited with code {proc.returncode}",
                exit_code=proc.returncode,
                stdout=proc.stdout,
                stderr=proc.stderr,
            )

        # Verify output was created
        if not actual_output.exists():
            raise OutputNotCreatedError(str(actual_output))

        # Calculate statistics
        result.rule_count = count_rules(actual_output)
        result.output_hash = compute_hash(actual_output)
        result.success = True

        if debug:
            logger.debug(f"Compiled {result.rule_count} rules")
            logger.debug(f"Output hash: {result.hash_short()}...")

        # Copy to rules directory if requested
        if copy_to_rules:
            if rules_directory:
                rules_dir = Path(rules_directory)
            else:
                rules_dir = config_path.parent.parent.parent / "rules"

            try:
                rules_dir.mkdir(exist_ok=True)
                dest_path = rules_dir / "adguard_user_filter.txt"
                shutil.copy2(actual_output, dest_path)
                result.copied_to_rules = True
                result.rules_destination = str(dest_path)
                if debug:
                    logger.debug(f"Copied to: {dest_path}")
            except (OSError, IOError) as e:
                raise CopyError(str(actual_output), str(dest_path), str(e))

    except (ValidationError, CompilationError, CompilerNotFoundError,
            OutputNotCreatedError, CopyError, CompilerTimeoutError) as e:
        result.success = False
        result.error_message = str(e)
        logger.error(f"Compilation failed: {e}")

    except Exception as e:
        result.success = False
        result.error_message = str(e)
        logger.error(f"Compilation failed: {e}")

    finally:
        # Clean up temp file
        if temp_config_path and os.path.exists(temp_config_path.name):
            os.unlink(temp_config_path.name)

        result.end_time = datetime.utcnow()
        result.elapsed_ms = int((result.end_time - result.start_time).total_seconds() * 1000)

    return result


def validate_configuration(
    config_path: str | Path,
    format: ConfigurationFormat | None = None,
    check_files: bool = False,
) -> tuple[bool, list[str], list[str]]:
    """
    Validate a configuration file without compiling.

    Args:
        config_path: Path to configuration file.
        format: Force configuration format.
        check_files: Check if local source files exist.

    Returns:
        Tuple of (is_valid, errors, warnings).
    """
    config = read_configuration(config_path, format)
    result = config.validate(check_files=check_files)
    return result.is_valid, result.errors, result.warnings
