//! Configuration handling for the rules compiler.

use crate::error::{CompilerError, Result};
use serde::{Deserialize, Serialize};
use std::fs;
use std::path::Path;

/// Supported configuration formats.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum ConfigFormat {
    Json,
    Yaml,
    Toml,
}

impl ConfigFormat {
    /// Detect format from file extension.
    pub fn from_path(path: &Path) -> Result<Self> {
        match path.extension().and_then(|e| e.to_str()) {
            Some("json") => Ok(Self::Json),
            Some("yaml" | "yml") => Ok(Self::Yaml),
            Some("toml") => Ok(Self::Toml),
            Some(ext) => Err(CompilerError::InvalidFormat(format!(
                "Unknown extension: .{}",
                ext
            ))),
            None => Err(CompilerError::InvalidFormat(
                "No file extension".to_string(),
            )),
        }
    }

    /// Parse from string.
    pub fn from_str(s: &str) -> Result<Self> {
        match s.to_lowercase().as_str() {
            "json" => Ok(Self::Json),
            "yaml" | "yml" => Ok(Self::Yaml),
            "toml" => Ok(Self::Toml),
            _ => Err(CompilerError::InvalidFormat(format!(
                "Unknown format: {}",
                s
            ))),
        }
    }
}

/// A filter source in the configuration.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FilterSource {
    /// Source name
    pub name: String,
    /// Source type (file, url, inline)
    #[serde(rename = "type")]
    pub source_type: String,
    /// Source URL (for url type)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub source: Option<String>,
    /// Inline content (for inline type)
    #[serde(skip_serializing_if = "Option::is_none")]
    pub content: Option<Vec<String>>,
}

/// Compiler configuration.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct CompilerConfig {
    /// Filter name
    pub name: String,
    /// Filter version
    #[serde(default)]
    pub version: String,
    /// Description
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,
    /// License
    #[serde(skip_serializing_if = "Option::is_none")]
    pub license: Option<String>,
    /// Homepage URL
    #[serde(skip_serializing_if = "Option::is_none")]
    pub homepage: Option<String>,
    /// Filter sources
    pub sources: Vec<FilterSource>,
    /// Transformations to apply
    #[serde(default)]
    pub transformations: Vec<String>,
}

impl CompilerConfig {
    /// Read configuration from a file.
    pub fn from_file(path: &Path, format: Option<ConfigFormat>) -> Result<Self> {
        if !path.exists() {
            return Err(CompilerError::ConfigNotFound(
                path.display().to_string(),
            ));
        }

        let content = fs::read_to_string(path)?;
        let format = format.unwrap_or(ConfigFormat::from_path(path)?);

        match format {
            ConfigFormat::Json => Ok(serde_json::from_str(&content)?),
            ConfigFormat::Yaml => Ok(serde_yaml::from_str(&content)?),
            ConfigFormat::Toml => Ok(toml::from_str(&content)?),
        }
    }

    /// Convert to JSON string.
    pub fn to_json(&self) -> Result<String> {
        Ok(serde_json::to_string_pretty(self)?)
    }
}

/// Find default configuration file.
pub fn find_default_config() -> Option<std::path::PathBuf> {
    let search_paths = [
        "compiler-config.json",
        "compiler-config.yaml",
        "compiler-config.yml",
        "compiler-config.toml",
        "../compiler-config.json",
    ];

    for path in search_paths {
        let path = Path::new(path);
        if path.exists() {
            return Some(path.to_path_buf());
        }
    }

    None
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_format_from_extension() {
        assert_eq!(
            ConfigFormat::from_path(Path::new("config.json")).unwrap(),
            ConfigFormat::Json
        );
        assert_eq!(
            ConfigFormat::from_path(Path::new("config.yaml")).unwrap(),
            ConfigFormat::Yaml
        );
        assert_eq!(
            ConfigFormat::from_path(Path::new("config.yml")).unwrap(),
            ConfigFormat::Yaml
        );
        assert_eq!(
            ConfigFormat::from_path(Path::new("config.toml")).unwrap(),
            ConfigFormat::Toml
        );
    }

    #[test]
    fn test_format_from_str() {
        assert_eq!(ConfigFormat::from_str("json").unwrap(), ConfigFormat::Json);
        assert_eq!(ConfigFormat::from_str("YAML").unwrap(), ConfigFormat::Yaml);
        assert_eq!(ConfigFormat::from_str("toml").unwrap(), ConfigFormat::Toml);
    }
}
