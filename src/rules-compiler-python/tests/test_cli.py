"""Tests for the CLI module."""

import pytest

from rules_compiler.cli import create_parser, main


class TestParser:
    """Tests for argument parser."""

    def test_parse_config_short(self):
        parser = create_parser()
        args = parser.parse_args(["-c", "config.yaml"])
        assert args.config == "config.yaml"

    def test_parse_config_long(self):
        parser = create_parser()
        args = parser.parse_args(["--config", "config.json"])
        assert args.config == "config.json"

    def test_parse_output(self):
        parser = create_parser()
        args = parser.parse_args(["-o", "output.txt"])
        assert args.output == "output.txt"

    def test_parse_copy_to_rules(self):
        parser = create_parser()
        args = parser.parse_args(["-r"])
        assert args.copy_to_rules is True

    def test_parse_format(self):
        parser = create_parser()
        args = parser.parse_args(["-f", "yaml"])
        assert args.format == "yaml"

    def test_parse_version(self):
        parser = create_parser()
        args = parser.parse_args(["-v"])
        assert args.version is True

    def test_parse_debug(self):
        parser = create_parser()
        args = parser.parse_args(["-d"])
        assert args.debug is True

    def test_parse_multiple_args(self):
        parser = create_parser()
        args = parser.parse_args(["-c", "config.yaml", "-o", "out.txt", "-r", "-d"])

        assert args.config == "config.yaml"
        assert args.output == "out.txt"
        assert args.copy_to_rules is True
        assert args.debug is True


class TestMain:
    """Tests for main function."""

    def test_version_returns_zero(self):
        result = main(["--version"])
        assert result == 0

    def test_missing_config_returns_one(self, tmp_path, monkeypatch):
        # Change to temp directory with no config
        monkeypatch.chdir(tmp_path)
        result = main([])
        assert result == 1
