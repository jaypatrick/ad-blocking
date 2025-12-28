/// Integration tests for the AdGuard API library
///
/// These tests verify the library structure and basic functionality
/// without requiring actual API credentials.

#[test]
fn test_configuration_creation() {
    use adguard_api_lib::apis::configuration::Configuration;

    let config = Configuration::new();
    assert!(!config.base_path.is_empty());
}

#[test]
fn test_configuration_with_token() {
    use adguard_api_lib::apis::configuration::Configuration;

    let mut config = Configuration::new();
    config.bearer_access_token = Some("test-token".to_string());

    assert_eq!(config.bearer_access_token, Some("test-token".to_string()));
}

#[test]
fn test_configuration_base_path() {
    use adguard_api_lib::apis::configuration::Configuration;

    let mut config = Configuration::new();
    config.base_path = "https://custom-api.example.com".to_string();

    assert_eq!(config.base_path, "https://custom-api.example.com");
}

#[test]
fn test_models_exist() {
    // Verify that key models are accessible
    use adguard_api_lib::models::{AccountLimits, Device, DnsServer};

    // These types should exist and be importable
    let _ = std::any::type_name::<AccountLimits>();
    let _ = std::any::type_name::<Device>();
    let _ = std::any::type_name::<DnsServer>();
}

#[test]
fn test_api_modules_exist() {
    // Verify that API modules are accessible and have expected functions
    // We can't call them without actual API credentials, but we can verify they exist

    // Just importing these modules verifies they exist
    use adguard_api_lib::apis::account_api;
    use adguard_api_lib::apis::devices_api;
    use adguard_api_lib::apis::dns_servers_api;
    use adguard_api_lib::apis::filter_lists_api;
    use adguard_api_lib::apis::statistics_api;
    use adguard_api_lib::apis::web_services_api;

    // The fact that these modules can be imported means they exist
    let _ = account_api::GetAccountLimitsError::UnknownValue(serde_json::Value::Null);
    let _ = devices_api::GetDeviceParams {
        device_id: "test".to_string(),
    };
}
