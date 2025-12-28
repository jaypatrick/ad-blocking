mod commands;
mod config;
mod menu;

use anyhow::Result;
use clap::Parser;
use config::AppConfig;
use menu::MenuHelper;

#[derive(Parser)]
#[command(name = "adguard-api-cli")]
#[command(author = "jaypatrick")]
#[command(version = "1.0.0")]
#[command(about = "Interactive CLI client for AdGuard DNS API", long_about = None)]
struct Cli {
    /// Run in non-interactive mode (for scripting)
    #[arg(long)]
    non_interactive: bool,
}

#[tokio::main]
async fn main() -> Result<()> {
    let cli = Cli::parse();

    if cli.non_interactive {
        eprintln!("Non-interactive mode not yet implemented. Use interactive mode.");
        std::process::exit(1);
    }

    run_interactive_mode().await
}

async fn run_interactive_mode() -> Result<()> {
    // Load configuration
    let mut app_config = AppConfig::load()?;

    // Display welcome banner
    MenuHelper::display_banner()?;

    // Check if API token is configured
    if !app_config.has_token() {
        MenuHelper::warning("No API token configured.");
        MenuHelper::info("Get your API key from: https://adguard-dns.io/dashboard/#/settings/api");
        println!();

        let api_key = MenuHelper::input_password("Enter your API Key:")?;
        app_config.set_token(api_key);

        // Save configuration
        if let Err(e) = app_config.save() {
            MenuHelper::warning(&format!("Failed to save configuration: {:?}", e));
        }

        // Test connection
        MenuHelper::status("Testing connection...");
        match commands::test_connection(&app_config).await {
            Ok(true) => {
                MenuHelper::success("Connection successful!");
            }
            Ok(false) => {
                MenuHelper::error("Connection failed. Please check your API key.");
            }
            Err(e) => {
                MenuHelper::error(&format!("Error: {:?}", e));
            }
        }

        println!();
    }

    // Main menu loop
    loop {
        let choices = vec![
            "Account Info",
            "Devices",
            "DNS Servers",
            "User Rules",
            "Query Log",
            "Statistics",
            "Filter Lists",
            "Web Services",
            "Dedicated IP Addresses",
            "Settings",
            "Exit",
        ];

        let selection = MenuHelper::select_from_choices("Main Menu", &choices)?;

        match selection {
            0 => {
                if let Err(e) = commands::account::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            1 => {
                if let Err(e) = commands::devices::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            2 => {
                if let Err(e) = commands::dns_servers::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            3 => {
                if let Err(e) = commands::user_rules::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            4 => {
                if let Err(e) = commands::query_log::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            5 => {
                if let Err(e) = commands::statistics::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            6 => {
                if let Err(e) = commands::filter_lists::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            7 => {
                if let Err(e) = commands::web_services::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            8 => {
                if let Err(e) = commands::dedicated_ips::show_menu(&app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            9 => {
                if let Err(e) = commands::settings::show_menu(&mut app_config).await {
                    MenuHelper::error(&format!("Error: {:?}", e));
                    MenuHelper::press_any_key()?;
                }
            }
            10 => {
                println!();
                MenuHelper::info("Goodbye!");
                break;
            }
            _ => {}
        }
    }

    Ok(())
}
