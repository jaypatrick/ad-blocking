"""
Configuration reader with multi-format support (JSON, YAML, TOML).
"""

from __future__ import annotations

import json
import sys
from dataclasses import dataclass, field
from enum import Enum
from pathlib import Path
from typing import Any


class ConfigurationFormat(Enum):
    """Supported configuration file formats."""
    JSON = "json"
    YAML = "yaml"
    TOML = "toml"


@dataclass
class FilterSource:
    """Represents a source filter list to compile."""
    name: str = ""
    source: str = ""
    type: str = "adblock"
    inclusions: list[str] = field(default_factory=list)
    exclusions: list[str] = field(default_factory=list)

    @classmethod
    def from_dict(cls, data: dict[str, Any]) -> FilterSource:
        """Create a FilterSource from a dictionary."""
        return cls(
            name=data.get("name", ""),
            source=data.get("source", ""),
            type=data.get("type", "adblock"),
            inclusions=data.get("inclusions", []),
            exclusions=data.get("exclusions", []),
        )


@dataclass
class CompilerConfiguration:
    """Configuration for the hostlist-compiler."""
    name: str = ""
    description: str = ""
    homepage: str = ""
    license: str = ""
    version: str = ""
    sources: list[FilterSource] = field(default_factory=list)
    transformations: list[str] = field(default_factory=list)
    inclusions: list[str] = field(default_factory=list)
    exclusions: list[str] = field(default_factory=list)

    # Metadata (not serialized)
    _source_format: ConfigurationFormat | None = field(default=None, repr=False)
    _source_path: str | None = field(default=None, repr=False)

    @classmethod
    def from_dict(cls, data: dict[str, Any]) -> CompilerConfiguration:
        """Create a CompilerConfiguration from a dictionary."""
        sources = [FilterSource.from_dict(s) for s in data.get("sources", [])]
        return cls(
            name=data.get("name", ""),
            description=data.get("description", ""),
            homepage=data.get("homepage", ""),
            license=data.get("license", ""),
            version=data.get("version", ""),
            sources=sources,
            transformations=data.get("transformations", []),
            inclusions=data.get("inclusions", []),
            exclusions=data.get("exclusions", []),
        )

    def to_dict(self) -> dict[str, Any]:
        """Convert to a dictionary for JSON serialization."""
        return {
            "name": self.name,
            "description": self.description,
            "homepage": self.homepage,
            "license": self.license,
            "version": self.version,
            "sources": [
                {
                    "name": s.name,
                    "source": s.source,
                    "type": s.type,
                    "inclusions": s.inclusions,
                    "exclusions": s.exclusions,
                }
                for s in self.sources
            ],
            "transformations": self.transformations,
            "inclusions": self.inclusions,
            "exclusions": self.exclusions,
        }


def detect_format(file_path: str | Path) -> ConfigurationFormat:
    """
    Detect configuration format from file extension.

    Args:
        file_path: Path to the configuration file.

    Returns:
        Detected configuration format.

    Raises:
        ValueError: If the extension is not recognized.
    """
    path = Path(file_path)
    ext = path.suffix.lower()

    format_map = {
        ".json": ConfigurationFormat.JSON,
        ".yaml": ConfigurationFormat.YAML,
        ".yml": ConfigurationFormat.YAML,
        ".toml": ConfigurationFormat.TOML,
    }

    if ext not in format_map:
        raise ValueError(f"Unknown configuration file extension: {ext}")

    return format_map[ext]


def _parse_json(content: str) -> dict[str, Any]:
    """Parse JSON content."""
    try:
        return json.loads(content)
    except json.JSONDecodeError as e:
        raise ValueError(f"Invalid JSON: {e}") from e


def _parse_yaml(content: str) -> dict[str, Any]:
    """Parse YAML content."""
    try:
        import yaml
        data = yaml.safe_load(content)
        if not isinstance(data, dict):
            raise ValueError("Invalid YAML: root must be an object")
        return data
    except ImportError:
        raise ImportError("PyYAML is required for YAML support. Install with: pip install pyyaml")
    except yaml.YAMLError as e:
        raise ValueError(f"Invalid YAML: {e}") from e


def _parse_toml(content: str) -> dict[str, Any]:
    """Parse TOML content."""
    try:
        # Python 3.11+ has tomllib in stdlib
        if sys.version_info >= (3, 11):
            import tomllib
            return tomllib.loads(content)
        else:
            import tomli
            return tomli.loads(content)
    except ImportError:
        raise ImportError(
            "TOML support requires Python 3.11+ or tomli package. "
            "Install with: pip install tomli"
        )
    except Exception as e:
        raise ValueError(f"Invalid TOML: {e}") from e


def read_configuration(
    config_path: str | Path,
    format: ConfigurationFormat | None = None,
) -> CompilerConfiguration:
    """
    Read and parse configuration from a file.

    Args:
        config_path: Path to the configuration file.
        format: Optional format override. If None, detected from extension.

    Returns:
        Parsed compiler configuration.

    Raises:
        FileNotFoundError: If the file doesn't exist.
        ValueError: If parsing fails.
    """
    path = Path(config_path)

    if not path.exists():
        raise FileNotFoundError(f"Configuration file not found: {config_path}")

    detected_format = format or detect_format(path)
    content = path.read_text(encoding="utf-8")

    parsers = {
        ConfigurationFormat.JSON: _parse_json,
        ConfigurationFormat.YAML: _parse_yaml,
        ConfigurationFormat.TOML: _parse_toml,
    }

    data = parsers[detected_format](content)
    config = CompilerConfiguration.from_dict(data)
    config._source_format = detected_format
    config._source_path = str(path)

    return config


def to_json(config: CompilerConfiguration, indent: int = 2) -> str:
    """
    Convert configuration to JSON string.

    Args:
        config: The configuration to convert.
        indent: JSON indentation level.

    Returns:
        JSON string representation.
    """
    return json.dumps(config.to_dict(), indent=indent)
