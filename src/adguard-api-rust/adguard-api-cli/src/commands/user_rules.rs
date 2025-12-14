use anyhow::Result;
use crate::{config::AppConfig, menu::MenuHelper};

pub async fn show_menu(_app_config: &AppConfig) -> Result<()> {
    MenuHelper::info("User Rules management functionality coming soon!");
    println!();
    println!("This will include:");
    println!("  • View current rules");
    println!("  • Upload rules from file");
    println!("  • Add/remove rules");
    println!("  • Enable/disable rules");
    MenuHelper::press_any_key()?;
    Ok(())
}
