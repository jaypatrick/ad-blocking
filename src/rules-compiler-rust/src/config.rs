//! Configuration reader with multi-format support (JSON, YAML, TOML).

use serde::{Deserialize, Serialize};
use std::fs;
use std::path::Path;

use crate::error::{CompilerError, Result};

/// Supported configuration file formats.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum ConfigurationFormat {
    /// JSON format (.json)
    Json,
    /// YAML format (.yaml, .yml)
    Yaml,
    /// TOML format (.toml)
    Toml,
}

impl std::fmt::Display for ConfigurationFormat {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::Json => write!(f, "json"),
            Self::Yaml => write!(f, "yaml"),
            Self::Toml => write!(f, "toml"),
        }
    }
}

/// Represents a source filter list to compile.
#[derive(Debug, Clone, Serialize, Deserialize, Default)]
pub struct FilterSource {
    /// Name of the source
    #[serde(default)]
    pub name: String,

    /// Source URL or file path
    #[serde(default)]
    pub source: String,

    /// Type of the source (e.g., "adblock", "hosts")
    #[serde(default = "default_source_type")]
    #[serde(rename = "type")]
    pub source_type: String,

    /// Inclusions patterns for this source
    #[serde(default)]
    pub inclusions: Vec<String>,

    /// Exclusions patterns for this source
    #[serde(default)]
    pub exclusions: Vec<String>,
}

fn default_source_type() -> String {
    "adblock".to_string()
}

/// Configuration for the hostlist-compiler.
#[derive(Debug, Clone, Serialize, Deserialize, Default)]
pub struct CompilerConfiguration {
    /// Name of the filter list
    #[serde(default)]
    pub name: String,

    /// Description of the filter list
    #[serde(default)]
    pub description: String,

    /// Homepage URL
    #[serde(default)]
    pub homepage: String,

    /// License identifier
    #[serde(default)]
    pub license: String,

    /// Version of the filter list
    #[serde(default)]
    pub version: String,

    /// List of source filter lists to compile
    #[serde(default)]
    pub sources: Vec<FilterSource>,

    /// Transformations to apply during compilation
    #[serde(default)]
    pub transformations: Vec<String>,

    /// Global inclusions patterns
    #[serde(default)]
    pub inclusions: Vec<String>,

    /// Global exclusions patterns
    #[serde(default)]
    pub exclusions: Vec<String>,

    /// Source format (not serialized)
    #[serde(skip)]
    pub source_format: Option<ConfigurationFormat>,

    /// Source file path (not serialized)
    #[serde(skip)]
    pub source_path: Option<String>,
}

/// Detect configuration format from file extension.
///
/// # Arguments
///
/// * `file_path` - Path to the configuration file
///
/// # Returns
///
/// The detected format, or an error if the extension is unknown.
pub fn detect_format<P: AsRef<Path>>(file_path: P) -> Result<ConfigurationFormat> {
    let path = file_path.as_ref();
    let extension = path
        .extension()
        .and_then(|e| e.to_str())
        .map(|e| e.to_lowercase())
        .unwrap_or_default();

    match extension.as_str() {
        "json" => Ok(ConfigurationFormat::Json),
        "yaml" | "yml" => Ok(ConfigurationFormat::Yaml),
        "toml" => Ok(ConfigurationFormat::Toml),
        _ => Err(CompilerError::UnknownExtension(extension)),
    }
}

/// Read and parse configuration from a file.
///
/// # Arguments
///
/// * `config_path` - Path to the configuration file
/// * `format` - Optional format override. If None, detected from extension.
///
/// # Returns
///
/// The parsed compiler configuration.
pub fn read_configuration<P: AsRef<Path>>(
    config_path: P,
    format: Option<ConfigurationFormat>,
) -> Result<CompilerConfiguration> {
    let path = config_path.as_ref();

    if !path.exists() {
        return Err(CompilerError::ConfigNotFound(
            path.display().to_string(),
        ));
    }

    let detected_format = format.unwrap_or_else(|| detect_format(path).unwrap_or(ConfigurationFormat::Json));
    let content = fs::read_to_string(path)?;

    let mut config: CompilerConfiguration = match detected_format {
        ConfigurationFormat::Json => serde_json::from_str(&content)?,
        ConfigurationFormat::Yaml => serde_yaml::from_str(&content)?,
        ConfigurationFormat::Toml => toml::from_str(&content)?,
    };

    config.source_format = Some(detected_format);
    config.source_path = Some(path.display().to_string());

    Ok(config)
}

/// Convert configuration to JSON string.
///
/// # Arguments
///
/// * `config` - The configuration to convert
///
/// # Returns
///
/// JSON string representation.
pub fn to_json(config: &CompilerConfiguration) -> Result<String> {
    Ok(serde_json::to_string_pretty(config)?)
}

#[cfg(test)]
mod tests {
    use super::*;
    use tempfile::TempDir;
    use std::fs::File;
    use std::io::Write;

    #[test]
    fn test_detect_format_json() {
        assert!(matches!(detect_format("config.json"), Ok(ConfigurationFormat::Json)));
        assert!(matches!(detect_format("path/to/config.JSON"), Ok(ConfigurationFormat::Json)));
    }

    #[test]
    fn test_detect_format_yaml() {
        assert!(matches!(detect_format("config.yaml"), Ok(ConfigurationFormat::Yaml)));
        assert!(matches!(detect_format("config.yml"), Ok(ConfigurationFormat::Yaml)));
    }

    #[test]
    fn test_detect_format_toml() {
        assert!(matches!(detect_format("config.toml"), Ok(ConfigurationFormat::Toml)));
    }

    #[test]
    fn test_detect_format_unknown() {
        assert!(matches!(detect_format("config.txt"), Err(CompilerError::UnknownExtension(_))));
    }

    #[test]
    fn test_read_json_config() {
        let dir = TempDir::new().unwrap();
        let path = dir.path().join("config.json");
        let mut file = File::create(&path).unwrap();
        writeln!(file, r#"{{"name": "Test", "version": "1.0.0"}}"#).unwrap();

        let config = read_configuration(&path, None).unwrap();
        assert_eq!(config.name, "Test");
        assert_eq!(config.version, "1.0.0");
    }

    #[test]
    fn test_read_yaml_config() {
        let dir = TempDir::new().unwrap();
        let path = dir.path().join("config.yaml");
        let mut file = File::create(&path).unwrap();
        writeln!(file, "name: YAML Test\nversion: 2.0.0").unwrap();

        let config = read_configuration(&path, None).unwrap();
        assert_eq!(config.name, "YAML Test");
        assert_eq!(config.version, "2.0.0");
    }

    #[test]
    fn test_to_json() {
        let config = CompilerConfiguration {
            name: "Test".to_string(),
            version: "1.0.0".to_string(),
            ..Default::default()
        };

        let json = to_json(&config).unwrap();
        assert!(json.contains("\"name\": \"Test\""));
        assert!(json.contains("\"version\": \"1.0.0\""));
    }
}
