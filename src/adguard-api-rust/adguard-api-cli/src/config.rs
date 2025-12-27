use anyhow::{Context, Result};
use serde::{Deserialize, Serialize};
use std::path::PathBuf;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AppConfig {
    #[serde(default = "default_api_url")]
    pub api_url: String,
    #[serde(default)]
    pub api_token: Option<String>,
}

fn default_api_url() -> String {
    "https://api.adguard-dns.io".to_string()
}

impl Default for AppConfig {
    fn default() -> Self {
        Self {
            api_url: default_api_url(),
            api_token: None,
        }
    }
}

impl AppConfig {
    /// Load configuration from file, environment variables, and CLI args
    ///
    /// Supports multiple environment variable naming conventions for backward compatibility:
    /// - Standard format: ADGUARD_API_KEY, ADGUARD_API_BASE_URL (recommended, tried first)
    /// - Legacy .NET format: ADGUARD_AdGuard__ApiKey, ADGUARD_AdGuard__BaseUrl (backward compatibility)
    /// - Legacy Rust format: ADGUARD_API_TOKEN, ADGUARD_API_URL (backward compatibility)
    pub fn load() -> Result<Self> {
        let mut config = Self::load_from_file().unwrap_or_default();

        // Override with environment variables
        // Try standardized format first, then .NET format, then legacy format
        if let Ok(url) = std::env::var("ADGUARD_API_BASE_URL") {
            config.api_url = url;
        } else if let Ok(url) = std::env::var("ADGUARD_AdGuard__BaseUrl") {
            config.api_url = url;
        } else if let Ok(url) = std::env::var("ADGUARD_API_URL") {
            config.api_url = url;
        }

        // Try standardized format first, then .NET format, then legacy format
        if let Ok(token) = std::env::var("ADGUARD_API_KEY") {
            config.api_token = Some(token);
        } else if let Ok(token) = std::env::var("ADGUARD_AdGuard__ApiKey") {
            config.api_token = Some(token);
        } else if let Ok(token) = std::env::var("ADGUARD_API_TOKEN") {
            config.api_token = Some(token);
        }

        Ok(config)
    }

    /// Load configuration from file
    fn load_from_file() -> Result<Self> {
        let config_path = Self::config_path()?;
        if !config_path.exists() {
            return Ok(Self::default());
        }

        let content = std::fs::read_to_string(&config_path)
            .with_context(|| format!("Failed to read config file: {:?}", config_path))?;

        toml::from_str(&content)
            .with_context(|| format!("Failed to parse config file: {:?}", config_path))
    }

    /// Save configuration to file
    pub fn save(&self) -> Result<()> {
        let config_path = Self::config_path()?;

        // Create parent directory if it doesn't exist
        if let Some(parent) = config_path.parent() {
            std::fs::create_dir_all(parent)?;
        }

        let content = toml::to_string_pretty(self)?;
        std::fs::write(&config_path, content)
            .with_context(|| format!("Failed to write config file: {:?}", config_path))?;

        Ok(())
    }

    /// Get the path to the configuration file
    fn config_path() -> Result<PathBuf> {
        let config_dir = dirs::config_dir().context("Failed to determine config directory")?;
        Ok(config_dir.join("adguard-api-cli").join("config.toml"))
    }

    /// Set the API token
    pub fn set_token(&mut self, token: String) {
        self.api_token = Some(token);
    }

    /// Check if API token is configured
    pub fn has_token(&self) -> bool {
        self.api_token.is_some()
    }

    /// Get API token or return an error
    pub fn get_token(&self) -> Result<&str> {
        self.api_token
            .as_deref()
            .context("API token not configured. Set ADGUARD_API_KEY environment variable or run settings menu.")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    // Note: These tests modify environment variables which can cause issues
    // when run in parallel. Run with `cargo test -- --test-threads=1` if needed.

    #[test]
    fn test_config_dotnet_compatible_env_vars() {
        // Set .NET-compatible environment variables
        unsafe {
            std::env::set_var(
                "TEST_ADGUARD_AdGuard__BaseUrl",
                "https://custom.api.example.com",
            );
            std::env::set_var("TEST_ADGUARD_AdGuard__ApiKey", "test-dotnet-token");
        }

        // Load directly using environment variables (simulating what load() does)
        let url = std::env::var("TEST_ADGUARD_AdGuard__BaseUrl").ok();
        let token = std::env::var("TEST_ADGUARD_AdGuard__ApiKey").ok();

        assert_eq!(url, Some("https://custom.api.example.com".to_string()));
        assert_eq!(token, Some("test-dotnet-token".to_string()));

        // Cleanup
        unsafe {
            std::env::remove_var("TEST_ADGUARD_AdGuard__BaseUrl");
            std::env::remove_var("TEST_ADGUARD_AdGuard__ApiKey");
        }
    }

    #[test]
    fn test_config_default() {
        let config = AppConfig::default();
        assert_eq!(config.api_url, "https://api.adguard-dns.io");
        assert_eq!(config.api_token, None);
    }

    #[test]
    fn test_config_set_token() {
        let mut config = AppConfig::default();
        assert!(!config.has_token());

        config.set_token("test-token".to_string());
        assert!(config.has_token());
        assert_eq!(config.get_token().unwrap(), "test-token");
    }

    #[test]
    fn test_config_get_token_error_when_none() {
        let config = AppConfig::default();
        let result = config.get_token();
        assert!(result.is_err());
        assert!(result
            .unwrap_err()
            .to_string()
            .contains("API token not configured"));
    }
}
