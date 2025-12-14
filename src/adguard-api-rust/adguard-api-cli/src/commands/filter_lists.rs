use anyhow::Result;
use adguard_api_lib::apis::filter_lists_api;
use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;
    
    MenuHelper::status("Fetching filter lists...");
    
    match filter_lists_api::list_filter_lists(&config).await {
        Ok(lists) => {
            if lists.is_empty() {
                MenuHelper::no_items("filter lists");
            } else {
                println!();
                println!("{}", console::style("═══ Filter Lists ═══").bold().cyan());
                println!();
                
                for list in &lists {
                    println!("📋 ID: {}", list.filter_id);
                    println!("   Name: {}", list.name);
                    println!("   Description: {}", list.description);
                    
                    if !list.categories.is_empty() {
                        let categories: Vec<String> = list.categories.iter()
                            .map(|cat| format!("{:?}", cat))
                            .collect();
                        println!("   Categories: {}", categories.join(", "));
                    }
                    
                    println!();
                }
                
                MenuHelper::success(&format!("Found {} filter list(s)", lists.len()));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch filter lists: {:?}", e));
        }
    }
    
    MenuHelper::press_any_key()?;
    Ok(())
}
