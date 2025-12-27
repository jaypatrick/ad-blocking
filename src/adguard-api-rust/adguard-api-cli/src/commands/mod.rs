pub mod account;
pub mod dedicated_ips;
pub mod devices;
pub mod dns_servers;
pub mod filter_lists;
pub mod query_log;
pub mod settings;
pub mod statistics;
pub mod user_rules;
pub mod web_services;

use crate::config::AppConfig;
use adguard_api_lib::apis::configuration::Configuration;
use anyhow::Result;

/// Create API configuration from app config
pub fn create_api_config(app_config: &AppConfig) -> Result<Configuration> {
    let mut config = Configuration::new();
    config.base_path = app_config.api_url.clone();
    config.bearer_access_token = Some(app_config.get_token()?.to_string());
    Ok(config)
}

/// Test API connection
pub async fn test_connection(app_config: &AppConfig) -> Result<bool> {
    let config = create_api_config(app_config)?;

    match adguard_api_lib::apis::account_api::get_account_limits(&config).await {
        Ok(_) => Ok(true),
        Err(_) => Ok(false),
    }
}
