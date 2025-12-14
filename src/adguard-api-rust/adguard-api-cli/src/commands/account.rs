use anyhow::Result;
use adguard_api_lib::apis::account_api;
use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;

    MenuHelper::status("Fetching account limits...");
    
    match account_api::get_account_limits(&config).await {
        Ok(limits) => {
            println!();
            println!("{}", console::style("═══ Account Limits ═══").bold().cyan());
            println!();
            
            println!("📋 Access Rules:");
            println!("  • Limit: {}", limits.access_rules.limit);
            println!("  • Used: {}", limits.access_rules.used);
            println!();
            
            println!("📱 Devices:");
            println!("  • Limit: {}", limits.devices.limit);
            println!("  • Used: {}", limits.devices.used);
            let dev_percentage = if limits.devices.limit > 0 {
                (limits.devices.used as f64 / limits.devices.limit as f64 * 100.0) as u32
            } else {
                0
            };
            println!("  • Available: {} ({}% used)", limits.devices.limit as i64 - limits.devices.used, dev_percentage);
            println!();
            
            println!("🖥️  DNS Servers:");
            println!("  • Limit: {}", limits.dns_servers.limit);
            println!("  • Used: {}", limits.dns_servers.used);
            let dns_percentage = if limits.dns_servers.limit > 0 {
                (limits.dns_servers.used as f64 / limits.dns_servers.limit as f64 * 100.0) as u32
            } else {
                0
            };
            println!("  • Available: {} ({}% used)", limits.dns_servers.limit as i64 - limits.dns_servers.used, dns_percentage);
            println!();
            
            println!("🔍 DNS Requests:");
            println!("  • Limit: {}", limits.requests.limit);
            println!("  • Used: {}", limits.requests.used);
            let req_percentage = if limits.requests.limit > 0 {
                (limits.requests.used as f64 / limits.requests.limit as f64 * 100.0) as u32
            } else {
                0
            };
            println!("  • Available: {} ({}% used)", limits.requests.limit as i64 - limits.requests.used, req_percentage);
            println!();
            
            println!("📜 User Rules:");
            println!("  • Limit: {}", limits.user_rules.limit);
            println!("  • Used: {}", limits.user_rules.used);
            let rules_percentage = if limits.user_rules.limit > 0 {
                (limits.user_rules.used as f64 / limits.user_rules.limit as f64 * 100.0) as u32
            } else {
                0
            };
            println!("  • Available: {} ({}% used)", limits.user_rules.limit as i64 - limits.user_rules.used, rules_percentage);
            println!();
            
            println!("📍 Dedicated IPv4:");
            println!("  • Limit: {}", limits.dedicated_ipv4.limit);
            println!("  • Used: {}", limits.dedicated_ipv4.used);
            
            MenuHelper::success("Account limits retrieved successfully");
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch account limits: {:?}", e));
        }
    }
    
    MenuHelper::press_any_key()?;
    Ok(())
}
