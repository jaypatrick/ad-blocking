use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};
use adguard_api_lib::apis::query_log_api;
use anyhow::Result;
use chrono::{DateTime, Utc};

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    loop {
        let choices = vec![
            "View Recent Queries (Last Hour)",
            "View Today's Queries",
            "View Custom Time Range",
            "Clear Query Log",
            "Back to Main Menu",
        ];
        let selection = MenuHelper::select_from_choices("Query Log", &choices)?;

        match selection {
            0 => view_queries(app_config, 1).await?,
            1 => view_queries(app_config, 24).await?,
            2 => view_queries_custom(app_config).await?,
            3 => clear_log(app_config).await?,
            4 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn view_queries(app_config: &AppConfig, hours_ago: i64) -> Result<()> {
    let config = create_api_config(app_config)?;

    let now = Utc::now().timestamp_millis();
    let time_from = now - (hours_ago * 60 * 60 * 1000);

    MenuHelper::status(&format!(
        "Fetching queries from the last {} hour(s)...",
        hours_ago
    ));

    let params = query_log_api::GetQueryLogParams {
        time_from_millis: time_from,
        time_to_millis: now,
        cursor: None,
        devices: None,
        countries: None,
        companies: None,
        statuses: None,
        categories: None,
        search: None,
        limit: Some(100),
    };

    match query_log_api::get_query_log(&config, params).await {
        Ok(response) => {
            println!();
            println!("{}", console::style("═══ Query Log ═══").bold().cyan());
            println!();

            let queries = &response.items;
            if queries.is_empty() {
                MenuHelper::info("No queries found for the specified time range.");
            } else {
                MenuHelper::info(&format!("Found {} queries", queries.len()));
                println!();

                MenuHelper::table_header(&["Time", "Domain", "Device", "Action"]);

                for query in queries.iter().take(50) {
                    let time_millis = query.time_millis;
                    let dt = DateTime::from_timestamp_millis(time_millis)
                        .unwrap_or_else(|| DateTime::UNIX_EPOCH);
                    let time_str = dt.format("%H:%M:%S").to_string();

                    let domain = query.domain.clone();
                    let device_id = query.device_id.as_deref().unwrap_or("N/A");
                    let action = query
                        .filtering_info
                        .filtering_status
                        .as_ref()
                        .map(|s| format!("{:?}", s))
                        .unwrap_or_else(|| "None".to_string());

                    MenuHelper::table_row(&[time_str, domain, device_id.to_string(), action]);
                }

                if queries.len() > 50 {
                    println!();
                    MenuHelper::info(&format!("Showing first 50 of {} queries", queries.len()));
                }
            }

            if !response.pages.is_empty() {
                println!();
                MenuHelper::info(&format!(
                    "{} page(s) available for pagination",
                    response.pages.len()
                ));
            }
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch query log: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}

async fn view_queries_custom(app_config: &AppConfig) -> Result<()> {
    println!();
    let hours_input = MenuHelper::input("Enter number of hours ago to view:")?;

    let hours: i64 = match hours_input.parse() {
        Ok(h) if h > 0 => h,
        _ => {
            MenuHelper::error("Invalid number of hours. Please enter a positive number.");
            MenuHelper::press_any_key()?;
            return Ok(());
        }
    };

    view_queries(app_config, hours).await
}

async fn clear_log(app_config: &AppConfig) -> Result<()> {
    println!();
    MenuHelper::warning("This will permanently delete all query logs.");

    if !MenuHelper::confirm("Are you sure you want to clear the query log?")? {
        MenuHelper::cancelled();
        return Ok(());
    }

    let config = create_api_config(app_config)?;
    MenuHelper::status("Clearing query log...");

    match query_log_api::clear_query_log(&config).await {
        Ok(_) => {
            MenuHelper::success("Query log cleared successfully!");
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to clear query log: {:?}", e));
        }
    }

    MenuHelper::press_any_key()?;
    Ok(())
}
