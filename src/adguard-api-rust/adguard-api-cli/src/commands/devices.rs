use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};
use adguard_api_lib::apis::devices_api;
use anyhow::Result;

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    loop {
        let choices = vec!["List Devices", "View Device Details", "Back to Main Menu"];
        let selection = MenuHelper::select_from_choices("Device Management", &choices)?;

        match selection {
            0 => list_devices(app_config).await?,
            1 => view_device_details(app_config).await?,
            2 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn list_devices(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;

    MenuHelper::status("Fetching devices...");

    match devices_api::list_devices(&config).await {
        Ok(devices) => {
            if devices.is_empty() {
                MenuHelper::no_items("devices");
            } else {
                println!();
                println!("{}", console::style("═══ Devices ═══").bold().cyan());
                MenuHelper::table_header(&["ID", "Name", "Type"]);

                for device in &devices {
                    MenuHelper::table_row(&[
                        device.id.clone(),
                        device.name.clone(),
                        format!("{:?}", device.device_type),
                    ]);
                }

                MenuHelper::success(&format!("Found {} device(s)", devices.len()));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch devices: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}

async fn view_device_details(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;

    MenuHelper::status("Fetching devices...");

    let devices = match devices_api::list_devices(&config).await {
        Ok(devices) => devices,
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch devices: {:?}", e));
            MenuHelper::press_any_key()?;
            return Ok(());
        }
    };

    if devices.is_empty() {
        MenuHelper::no_items("devices");
        MenuHelper::press_any_key()?;
        return Ok(());
    }

    let device_names: Vec<String> = devices
        .iter()
        .map(|d| format!("{} ({})", d.name, d.id))
        .collect();

    let selection = MenuHelper::select("Select a device to view details:", &device_names)?;
    let device = &devices[selection];

    println!();
    println!("{}", console::style("═══ Device Details ═══").bold().cyan());
    println!();
    println!("📱 Name: {}", device.name);
    println!("🆔 ID: {}", device.id);
    println!("🔧 Type: {:?}", device.device_type);
    println!("🖥️  DNS Server ID: {}", device.dns_server_id);

    MenuHelper::press_any_key()?;
    Ok(())
}
