use anyhow::Result;
use adguard_api_lib::apis::dns_servers_api;
use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    loop {
        let choices = vec!["List DNS Servers", "View Server Details", "Back to Main Menu"];
        let selection = MenuHelper::select_from_choices("DNS Server Management", &choices)?;

        match selection {
            0 => list_servers(app_config).await?,
            1 => view_server_details(app_config).await?,
            2 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn list_servers(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;
    
    MenuHelper::status("Fetching DNS servers...");
    
    match dns_servers_api::list_dns_servers(&config).await {
        Ok(servers) => {
            if servers.is_empty() {
                MenuHelper::no_items("DNS servers");
            } else {
                println!();
                println!("{}", console::style("═══ DNS Servers ═══").bold().cyan());
                MenuHelper::table_header(&["ID", "Name", "Default", "Device Count"]);
                
                for server in &servers {
                    let is_default = server.default;
                    let device_count = server.device_ids.len();
                    
                    MenuHelper::table_row(&[
                        server.id.clone(),
                        server.name.clone(),
                        if is_default { "✓" } else { "" }.to_string(),
                        device_count.to_string(),
                    ]);
                }
                
                MenuHelper::success(&format!("Found {} DNS server(s)", servers.len()));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch DNS servers: {:?}", e));
        }
    }
    
    MenuHelper::press_any_key()?;
    Ok(())
}

async fn view_server_details(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;
    
    MenuHelper::status("Fetching DNS servers...");
    
    let servers = match dns_servers_api::list_dns_servers(&config).await {
        Ok(servers) => servers,
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch DNS servers: {:?}", e));
            MenuHelper::press_any_key()?;
            return Ok(());
        }
    };
    
    if servers.is_empty() {
        MenuHelper::no_items("DNS servers");
        MenuHelper::press_any_key()?;
        return Ok(());
    }
    
    let server_names: Vec<String> = servers
        .iter()
        .map(|s| {
            let default_marker = if s.default { " [default]" } else { "" };
            format!("{}{} ({})", s.name, default_marker, s.id)
        })
        .collect();
    
    let selection = MenuHelper::select("Select a DNS server to view details:", &server_names)?;
    let server = &servers[selection];
    
    println!();
    println!("{}", console::style("═══ DNS Server Details ═══").bold().cyan());
    println!();
    println!("📛 Name: {}", server.name);
    println!("🆔 ID: {}", server.id);
    println!("⭐ Default: {}", if server.default { "Yes" } else { "No" });
    println!("📱 Device Count: {}", server.device_ids.len());
    
    if !server.device_ids.is_empty() {
        println!("   Devices: {}", server.device_ids.join(", "));
    }
    
    println!();
    println!("⚙️  Settings:");
    
    let user_rules_enabled = server.settings.user_rules_settings.enabled;
    println!("  • User Rules Enabled: {}", user_rules_enabled);
    let rules_count = server.settings.user_rules_settings.rules.len();
    if rules_count > 0 {
        println!("  • Rules Count: {}", rules_count);
    }
    
    if server.settings.filter_lists_settings.enabled {
        println!("  • Filter Lists Enabled: Yes");
    }
    
    MenuHelper::press_any_key()?;
    Ok(())
}
