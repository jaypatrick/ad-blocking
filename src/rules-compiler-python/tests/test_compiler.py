"""Tests for the compiler module."""

import tempfile
from pathlib import Path

import pytest

from rules_compiler.compiler import (
    PlatformInfo,
    VersionInfo,
    count_rules,
    compute_hash,
    get_platform_info,
    get_version_info,
)


class TestCountRules:
    """Tests for count_rules function."""

    def test_counts_non_comment_lines(self, tmp_path: Path):
        content = """! Comment line
# Another comment
||example.com^
||test.org^

@@||allowed.com^
! More comments
||blocked.net^
"""
        file_path = tmp_path / "rules.txt"
        file_path.write_text(content)

        count = count_rules(file_path)

        assert count == 4  # 3 blocking rules + 1 allow rule

    def test_handles_empty_file(self, tmp_path: Path):
        file_path = tmp_path / "empty.txt"
        file_path.write_text("")

        assert count_rules(file_path) == 0

    def test_handles_all_comments(self, tmp_path: Path):
        content = """! Comment 1
! Comment 2
# Comment 3

"""
        file_path = tmp_path / "comments.txt"
        file_path.write_text(content)

        assert count_rules(file_path) == 0

    def test_handles_missing_file(self, tmp_path: Path):
        file_path = tmp_path / "nonexistent.txt"

        assert count_rules(file_path) == 0


class TestComputeHash:
    """Tests for compute_hash function."""

    def test_returns_consistent_hash(self, tmp_path: Path):
        content = "Test content for hashing"
        file_path = tmp_path / "test.txt"
        file_path.write_text(content)

        hash1 = compute_hash(file_path)
        hash2 = compute_hash(file_path)

        assert hash1 == hash2
        assert len(hash1) == 96  # SHA-384 produces 96 hex characters

    def test_different_content_different_hash(self, tmp_path: Path):
        file1 = tmp_path / "file1.txt"
        file2 = tmp_path / "file2.txt"
        file1.write_text("Content A")
        file2.write_text("Content B")

        hash1 = compute_hash(file1)
        hash2 = compute_hash(file2)

        assert hash1 != hash2


class TestGetPlatformInfo:
    """Tests for get_platform_info function."""

    def test_returns_valid_info(self):
        info = get_platform_info()

        assert isinstance(info, PlatformInfo)
        assert info.os_name != ""
        assert info.architecture != ""

        # At least one platform flag should be true
        assert info.is_windows or info.is_linux or info.is_macos


class TestGetVersionInfo:
    """Tests for get_version_info function."""

    def test_returns_version_info(self):
        info = get_version_info()

        assert isinstance(info, VersionInfo)
        assert info.module_version != ""
        assert info.python_version != ""
        assert isinstance(info.platform, PlatformInfo)
