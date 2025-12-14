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
    pub fn load() -> Result<Self> {
        let mut config = Self::load_from_file().unwrap_or_default();

        // Override with environment variables
        if let Ok(url) = std::env::var("ADGUARD_API_URL") {
            config.api_url = url;
        }
        if let Ok(token) = std::env::var("ADGUARD_API_TOKEN") {
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
        let config_dir = dirs::config_dir()
            .context("Failed to determine config directory")?;
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
            .context("API token not configured. Set ADGUARD_API_TOKEN environment variable or run settings menu.")
    }
}
