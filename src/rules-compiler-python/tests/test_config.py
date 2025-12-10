"""Tests for the configuration reader module."""

import json
import tempfile
from pathlib import Path

import pytest

from rules_compiler.config import (
    CompilerConfiguration,
    ConfigurationFormat,
    FilterSource,
    detect_format,
    read_configuration,
    to_json,
)


class TestDetectFormat:
    """Tests for detect_format function."""

    def test_detect_json(self):
        assert detect_format("config.json") == ConfigurationFormat.JSON
        assert detect_format("/path/to/config.JSON") == ConfigurationFormat.JSON

    def test_detect_yaml(self):
        assert detect_format("config.yaml") == ConfigurationFormat.YAML
        assert detect_format("config.yml") == ConfigurationFormat.YAML
        assert detect_format("/path/to/config.YAML") == ConfigurationFormat.YAML

    def test_detect_toml(self):
        assert detect_format("config.toml") == ConfigurationFormat.TOML
        assert detect_format("/path/to/config.TOML") == ConfigurationFormat.TOML

    def test_unknown_extension(self):
        with pytest.raises(ValueError, match="Unknown configuration file extension"):
            detect_format("config.txt")

        with pytest.raises(ValueError, match="Unknown configuration file extension"):
            detect_format("config.xml")


class TestReadConfiguration:
    """Tests for read_configuration function."""

    def test_read_json_config(self, tmp_path: Path):
        config_data = {
            "name": "Test Filter",
            "version": "1.0.0",
            "sources": [
                {"name": "Local", "source": "./rules.txt", "type": "adblock"}
            ],
            "transformations": ["Deduplicate"],
        }
        config_file = tmp_path / "config.json"
        config_file.write_text(json.dumps(config_data))

        config = read_configuration(config_file)

        assert config.name == "Test Filter"
        assert config.version == "1.0.0"
        assert len(config.sources) == 1
        assert config.sources[0].name == "Local"
        assert config._source_format == ConfigurationFormat.JSON

    def test_read_yaml_config(self, tmp_path: Path):
        yaml_content = """
name: YAML Test Filter
version: 2.0.0
sources:
  - name: Remote
    source: https://example.com/rules.txt
    type: adblock
transformations:
  - Validate
"""
        config_file = tmp_path / "config.yaml"
        config_file.write_text(yaml_content)

        config = read_configuration(config_file)

        assert config.name == "YAML Test Filter"
        assert config.version == "2.0.0"
        assert len(config.sources) == 1
        assert config._source_format == ConfigurationFormat.YAML

    def test_file_not_found(self):
        with pytest.raises(FileNotFoundError, match="Configuration file not found"):
            read_configuration("/nonexistent/path/config.json")

    def test_invalid_json(self, tmp_path: Path):
        config_file = tmp_path / "config.json"
        config_file.write_text("not valid json {{{")

        with pytest.raises(ValueError, match="Invalid JSON"):
            read_configuration(config_file)

    def test_format_override(self, tmp_path: Path):
        yaml_content = """
name: Override Test
version: 1.0.0
sources: []
transformations: []
"""
        # Write YAML content with .txt extension
        config_file = tmp_path / "config.txt"
        config_file.write_text(yaml_content)

        # Force YAML format
        config = read_configuration(config_file, format=ConfigurationFormat.YAML)

        assert config.name == "Override Test"
        assert config._source_format == ConfigurationFormat.YAML


class TestFilterSource:
    """Tests for FilterSource class."""

    def test_from_dict(self):
        data = {
            "name": "Test Source",
            "source": "./rules.txt",
            "type": "hosts",
            "inclusions": ["*"],
            "exclusions": ["excluded"],
        }

        source = FilterSource.from_dict(data)

        assert source.name == "Test Source"
        assert source.source == "./rules.txt"
        assert source.type == "hosts"
        assert source.inclusions == ["*"]
        assert source.exclusions == ["excluded"]

    def test_from_dict_defaults(self):
        source = FilterSource.from_dict({})

        assert source.name == ""
        assert source.source == ""
        assert source.type == "adblock"
        assert source.inclusions == []
        assert source.exclusions == []


class TestCompilerConfiguration:
    """Tests for CompilerConfiguration class."""

    def test_from_dict(self):
        data = {
            "name": "My Filter",
            "description": "A filter list",
            "homepage": "https://example.com",
            "license": "MIT",
            "version": "1.0.0",
            "sources": [
                {"name": "Local", "source": "./rules.txt"}
            ],
            "transformations": ["Deduplicate", "Validate"],
            "inclusions": ["*"],
        }

        config = CompilerConfiguration.from_dict(data)

        assert config.name == "My Filter"
        assert config.description == "A filter list"
        assert config.version == "1.0.0"
        assert len(config.sources) == 1
        assert config.transformations == ["Deduplicate", "Validate"]

    def test_to_dict(self):
        config = CompilerConfiguration(
            name="Test",
            version="1.0.0",
            sources=[FilterSource(name="Local", source="./rules.txt")],
            transformations=["Deduplicate"],
        )

        data = config.to_dict()

        assert data["name"] == "Test"
        assert data["version"] == "1.0.0"
        assert len(data["sources"]) == 1
        assert data["sources"][0]["name"] == "Local"


class TestToJson:
    """Tests for to_json function."""

    def test_converts_to_json(self):
        config = CompilerConfiguration(
            name="Test",
            version="1.0.0",
            sources=[FilterSource(name="Local", source="./rules.txt")],
        )

        json_str = to_json(config)
        parsed = json.loads(json_str)

        assert parsed["name"] == "Test"
        assert parsed["version"] == "1.0.0"
        assert "_source_format" not in parsed
        assert "_source_path" not in parsed
