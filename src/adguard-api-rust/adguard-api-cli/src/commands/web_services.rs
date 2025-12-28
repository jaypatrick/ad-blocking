use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};
use adguard_api_lib::apis::web_services_api;
use anyhow::Result;

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    let config = create_api_config(app_config)?;

    MenuHelper::status("Fetching web services...");

    match web_services_api::list_web_services(&config).await {
        Ok(services) => {
            if services.is_empty() {
                MenuHelper::no_items("web services");
            } else {
                println!();
                println!("{}", console::style("═══ Web Services ═══").bold().cyan());
                println!();
                MenuHelper::info(&format!("Found {} web service(s)", services.len()));
                println!();

                MenuHelper::table_header(&["ID", "Name"]);

                for service in &services {
                    MenuHelper::table_row(&[service.id.clone(), service.name.clone()]);
                }

                MenuHelper::success(&format!("Found {} web service(s)", services.len()));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch web services: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}
