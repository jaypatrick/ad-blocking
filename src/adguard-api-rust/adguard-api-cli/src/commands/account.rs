use anyhow::Result;
use adguard_api_lib::apis::account_api;
use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};

fn calculate_usage_percentage(used: i64, limit: i32) -> u32 {
    if limit > 0 {
        (used as f64 / limit as f64 * 100.0) as u32
    } else {
        0
    }
}

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
            
            let dev_percentage = calculate_usage_percentage(limits.devices.used, limits.devices.limit);
            println!("📱 Devices:");
            println!("  • Limit: {}", limits.devices.limit);
            println!("  • Used: {}", limits.devices.used);
            println!("  • Available: {} ({}% used)", limits.devices.limit as i64 - limits.devices.used, dev_percentage);
            println!();
            
            let dns_percentage = calculate_usage_percentage(limits.dns_servers.used, limits.dns_servers.limit);
            println!("🖥️  DNS Servers:");
            println!("  • Limit: {}", limits.dns_servers.limit);
            println!("  • Used: {}", limits.dns_servers.used);
            println!("  • Available: {} ({}% used)", limits.dns_servers.limit as i64 - limits.dns_servers.used, dns_percentage);
            println!();
            
            let req_percentage = calculate_usage_percentage(limits.requests.used, limits.requests.limit);
            println!("🔍 DNS Requests:");
            println!("  • Limit: {}", limits.requests.limit);
            println!("  • Used: {}", limits.requests.used);
            println!("  • Available: {} ({}% used)", limits.requests.limit as i64 - limits.requests.used, req_percentage);
            println!();
            
            let rules_percentage = calculate_usage_percentage(limits.user_rules.used, limits.user_rules.limit);
            println!("📜 User Rules:");
            println!("  • Limit: {}", limits.user_rules.limit);
            println!("  • Used: {}", limits.user_rules.used);
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
