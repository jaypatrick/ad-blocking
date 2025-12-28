use crate::{commands::test_connection, config::AppConfig, menu::MenuHelper};
use anyhow::Result;

pub async fn show_menu(app_config: &mut AppConfig) -> Result<()> {
    loop {
        let choices = vec![
            "Change API Key",
            "Test Connection",
            "View Current Configuration",
            "Back to Main Menu",
        ];
        let selection = MenuHelper::select_from_choices("Settings", &choices)?;

        match selection {
            0 => change_api_key(app_config).await?,
            1 => test_api_connection(app_config).await?,
            2 => view_configuration(app_config)?,
            3 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn change_api_key(app_config: &mut AppConfig) -> Result<()> {
    println!();
    MenuHelper::info("Get your API key from: https://adguard-dns.io/dashboard/#/settings/api");
    println!();

    let api_key = MenuHelper::input_password("Enter your API Key:")?;

    if api_key.trim().is_empty() {
        MenuHelper::error("API key cannot be empty.");
        MenuHelper::press_any_key()?;
        return Ok(());
    }

    app_config.set_token(api_key);

    // Save to config file
    match app_config.save() {
        Ok(_) => {
            MenuHelper::success("API key saved successfully!");

            // Test connection
            MenuHelper::status("Testing connection...");
            match test_connection(app_config).await {
                Ok(true) => {
                    MenuHelper::success("Connection successful!");
                }
                Ok(false) => {
                    MenuHelper::error("Connection failed. Please check your API key.");
                }
                Err(e) => {
                    MenuHelper::error(&format!("Error testing connection: {:?}", e));
                }
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to save API key: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}

async fn test_api_connection(app_config: &AppConfig) -> Result<()> {
    if !app_config.has_token() {
        MenuHelper::error("API key not configured. Please set an API key first.");
        MenuHelper::press_any_key()?;
        return Ok(());
    }

    MenuHelper::status("Testing connection...");

    match test_connection(app_config).await {
        Ok(true) => {
            MenuHelper::success("Connection is working!");
        }
        Ok(false) => {
            MenuHelper::error(
                "Connection failed. Please check your API key and network connection.",
            );
        }
        Err(e) => {
            MenuHelper::error(&format!("Error testing connection: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}

fn view_configuration(app_config: &AppConfig) -> Result<()> {
    println!();
    println!(
        "{}",
        console::style("═══ Current Configuration ═══")
            .bold()
            .cyan()
    );
    println!();
    println!("🌐 API URL: {}", app_config.api_url);
    println!(
        "🔑 API Token: {}",
        if app_config.has_token() {
            "Configured ✓"
        } else {
            "Not configured ✗"
        }
    );
    println!();

    MenuHelper::press_any_key()?;
    Ok(())
}
