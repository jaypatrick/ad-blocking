use anyhow::Result;
use clap::{Parser, Subcommand};
use adguard_api_lib::apis::configuration::Configuration;

#[derive(Parser)]
#[command(name = "adguard-api-cli")]
#[command(author = "jaypatrick")]
#[command(version = "1.0.0")]
#[command(about = "CLI client for AdGuard DNS API", long_about = None)]
struct Cli {
    /// API base URL (default: https://api.adguard-dns.io)
    #[arg(long, env = "ADGUARD_API_URL", default_value = "https://api.adguard-dns.io")]
    api_url: String,

    /// API authentication token
    #[arg(long, env = "ADGUARD_API_TOKEN")]
    api_token: Option<String>,

    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    /// Account operations
    Account {
        #[command(subcommand)]
        action: AccountCommands,
    },
    /// Device operations
    Devices {
        #[command(subcommand)]
        action: DeviceCommands,
    },
    /// DNS server operations
    DnsServers {
        #[command(subcommand)]
        action: DnsServerCommands,
    },
    /// Filter list operations
    FilterLists {
        #[command(subcommand)]
        action: FilterListCommands,
    },
    /// Statistics operations
    Statistics {
        #[command(subcommand)]
        action: StatisticsCommands,
    },
    /// Web services operations
    WebServices {
        #[command(subcommand)]
        action: WebServiceCommands,
    },
}

#[derive(Subcommand)]
enum AccountCommands {
    /// Get account limits
    Limits,
}

#[derive(Subcommand)]
enum DeviceCommands {
    /// List all devices
    List,
    /// Get device by ID
    Get {
        /// Device ID
        device_id: String,
    },
    /// Create a new device
    Create {
        /// Device name
        #[arg(long)]
        name: String,
    },
    /// Remove a device
    Remove {
        /// Device ID
        device_id: String,
    },
}

#[derive(Subcommand)]
enum DnsServerCommands {
    /// List DNS servers
    List,
    /// Create a new DNS server
    Create {
        /// Server name
        #[arg(long)]
        name: String,
    },
}

#[derive(Subcommand)]
enum FilterListCommands {
    /// List all filter lists
    List,
}

#[derive(Subcommand)]
enum StatisticsCommands {
    /// Get time-based query statistics
    Time,
}

#[derive(Subcommand)]
enum WebServiceCommands {
    /// List all web services
    List,
}

fn create_configuration(base_url: &str, token: Option<&str>) -> Result<Configuration> {
    let mut config = Configuration::new();
    config.base_path = base_url.to_string();
    
    if let Some(token) = token {
        config.bearer_access_token = Some(token.to_string());
    }
    
    Ok(config)
}

#[tokio::main]
async fn main() -> Result<()> {
    let cli = Cli::parse();

    let config = create_configuration(&cli.api_url, cli.api_token.as_deref())?;

    match cli.command {
        Commands::Account { action } => match action {
            AccountCommands::Limits => {
                println!("Fetching account limits...");
                match adguard_api_lib::apis::account_api::get_account_limits(&config).await {
                    Ok(limits) => {
                        println!("{}", serde_json::to_string_pretty(&limits)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
        },
        Commands::Devices { action } => match action {
            DeviceCommands::List => {
                println!("Fetching devices...");
                match adguard_api_lib::apis::devices_api::list_devices(&config).await {
                    Ok(devices) => {
                        println!("{}", serde_json::to_string_pretty(&devices)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
            DeviceCommands::Get { device_id } => {
                println!("Fetching device: {}", device_id);
                let params = adguard_api_lib::apis::devices_api::GetDeviceParams {
                    device_id: device_id.clone(),
                };
                match adguard_api_lib::apis::devices_api::get_device(&config, params).await {
                    Ok(device) => {
                        println!("{}", serde_json::to_string_pretty(&device)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
            DeviceCommands::Create { name } => {
                println!("Creating device: {}", name);
                println!("Note: Full device creation requires additional parameters");
                println!("This is a placeholder - implement with proper DeviceCreate struct");
            }
            DeviceCommands::Remove { device_id } => {
                println!("Removing device: {}", device_id);
                let params = adguard_api_lib::apis::devices_api::RemoveDeviceParams {
                    device_id: device_id.clone(),
                };
                match adguard_api_lib::apis::devices_api::remove_device(&config, params).await {
                    Ok(_) => {
                        println!("Device removed successfully");
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
        },
        Commands::DnsServers { action } => match action {
            DnsServerCommands::List => {
                println!("Fetching DNS servers...");
                match adguard_api_lib::apis::dns_servers_api::list_dns_servers(&config).await {
                    Ok(servers) => {
                        println!("{}", serde_json::to_string_pretty(&servers)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
            DnsServerCommands::Create { name } => {
                println!("Creating DNS server: {}", name);
                println!("Note: Full DNS server creation requires additional parameters");
                println!("This is a placeholder - implement with proper DNSServerCreate struct");
            }
        },
        Commands::FilterLists { action } => match action {
            FilterListCommands::List => {
                println!("Fetching filter lists...");
                match adguard_api_lib::apis::filter_lists_api::list_filter_lists(&config).await {
                    Ok(lists) => {
                        println!("{}", serde_json::to_string_pretty(&lists)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
        },
        Commands::Statistics { action } => match action {
            StatisticsCommands::Time => {
                println!("Fetching time statistics...");
                // Get stats for the last 24 hours
                let now_ms = std::time::SystemTime::now()
                    .duration_since(std::time::UNIX_EPOCH)
                    .unwrap()
                    .as_millis() as i64;
                let day_ago_ms = now_ms - (24 * 60 * 60 * 1000);
                
                let params = adguard_api_lib::apis::statistics_api::GetTimeQueriesStatsParams {
                    time_from_millis: day_ago_ms,
                    time_to_millis: now_ms,
                    devices: None,
                    countries: None,
                };
                match adguard_api_lib::apis::statistics_api::get_time_queries_stats(&config, params).await {
                    Ok(stats) => {
                        println!("{}", serde_json::to_string_pretty(&stats)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
        },
        Commands::WebServices { action } => match action {
            WebServiceCommands::List => {
                println!("Fetching web services...");
                match adguard_api_lib::apis::web_services_api::list_web_services(&config).await {
                    Ok(services) => {
                        println!("{}", serde_json::to_string_pretty(&services)?);
                    }
                    Err(e) => {
                        eprintln!("Error: {:?}", e);
                    }
                }
            }
        },
    }

    Ok(())
}
