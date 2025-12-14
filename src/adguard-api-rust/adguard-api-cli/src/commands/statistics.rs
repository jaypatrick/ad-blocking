use anyhow::Result;
use adguard_api_lib::apis::statistics_api;
use crate::{commands::create_api_config, config::AppConfig, menu::MenuHelper};

pub async fn show_menu(app_config: &AppConfig) -> Result<()> {
    loop {
        let choices = vec![
            "Last 24 Hours",
            "Last 7 Days",
            "Last 30 Days",
            "Back to Main Menu",
        ];
        let selection = MenuHelper::select_from_choices("Statistics", &choices)?;

        match selection {
            0 => show_statistics(app_config, 24).await?,
            1 => show_statistics(app_config, 24 * 7).await?,
            2 => show_statistics(app_config, 24 * 30).await?,
            3 => break,
            _ => {}
        }
    }
    Ok(())
}

async fn show_statistics(app_config: &AppConfig, hours: i64) -> Result<()> {
    let config = create_api_config(app_config)?;
    
    let now_ms = std::time::SystemTime::now()
        .duration_since(std::time::UNIX_EPOCH)
        .unwrap()
        .as_millis() as i64;
    let time_from_ms = now_ms - (hours * 60 * 60 * 1000);
    
    MenuHelper::status(&format!("Fetching statistics for the last {} hours...", hours));
    
    let params = statistics_api::GetTimeQueriesStatsParams {
        time_from_millis: time_from_ms,
        time_to_millis: now_ms,
        devices: None,
        countries: None,
    };
    
    match statistics_api::get_time_queries_stats(&config, params).await {
        Ok(stats) => {
            println!();
            println!("{}", console::style("═══ Statistics ═══").bold().cyan());
            println!();
            
            let data = &stats.stats;
            let total_queries: i64 = data.iter()
                .map(|s| s.value.queries)
                .sum();
            let total_blocked: i64 = data.iter()
                .map(|s| s.value.blocked)
                .sum();
            
            let block_rate = if total_queries > 0 {
                (total_blocked as f64 / total_queries as f64 * 100.0)
            } else {
                0.0
            };
            
            println!("📊 Total Queries: {}", total_queries);
            println!("🛡️  Blocked Queries: {}", total_blocked);
            println!("📈 Block Rate: {:.2}%", block_rate);
            
            MenuHelper::success("Statistics retrieved successfully");
        }
        Err(e) => {
            MenuHelper::error(&format!("Failed to fetch statistics: {:?}", e));
        }
    }
    
    MenuHelper::press_any_key()?;
    Ok(())
}
