use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};
use adguard_api_lib::apis::dedicated_ip_addresses_api;
use anyhow::Result;

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    loop {
        let choices = vec![
            "List All IP Addresses",
            "Allocate New IP Address",
            "Back to Main Menu",
        ];
        let selection = MenuHelper::select_from_choices("Dedicated IP Addresses", &choices)?;

        match selection {
            0 => list_ips(app_config).await?,
            1 => allocate_ip(app_config).await?,
            2 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn list_ips(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;

    MenuHelper::status("Fetching dedicated IP addresses...");

    match dedicated_ip_addresses_api::list_dedicated_ipv4_addresses(&config).await {
        Ok(addresses) => {
            if addresses.is_empty() {
                MenuHelper::no_items("dedicated IP addresses");
            } else {
                println!();
                println!(
                    "{}",
                    console::style("═══ Dedicated IP Addresses ═══")
                        .bold()
                        .cyan()
                );
                println!();

                MenuHelper::table_header(&["IP Address", "Device ID", "Status"]);

                for addr in &addresses {
                    let ip = addr.ip.clone();
                    let device_id = addr.device_id.as_deref().unwrap_or("Unlinked");
                    let status = if addr.device_id.is_some() {
                        "Linked"
                    } else {
                        "Unlinked"
                    };

                    MenuHelper::table_row(&[ip, device_id.to_string(), status.to_string()]);
                }

                MenuHelper::success(&format!(
                    "Found {} dedicated IP address(es)",
                    addresses.len()
                ));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch dedicated IP addresses: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}

async fn allocate_ip(app_config: &AppConfig) -> Result<()> {
    println!();
    if !MenuHelper::confirm("Allocate a new dedicated IPv4 address?")? {
        MenuHelper::cancelled();
        return Ok(());
    }

    let config = create_api_config(app_config)?;
    MenuHelper::status("Allocating new IP address...");

    match dedicated_ip_addresses_api::allocate_dedicated_ipv4_address(&config).await {
        Ok(address) => {
            println!();
            MenuHelper::success("Successfully allocated dedicated IPv4 address!");
            println!();
            println!("📍 IP Address: {}", address.ip);
            if let Some(device_id) = &address.device_id {
                println!("📱 Device ID: {}", device_id);
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to allocate IP address: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}
