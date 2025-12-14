use anyhow::Result;
use console::{style, Term};
use dialoguer::{theme::ColorfulTheme, Confirm, Input, Select};
use std::fmt::Display;

pub struct MenuHelper;

impl MenuHelper {
    /// Display a welcome banner
    pub fn display_banner() -> Result<()> {
        let term = Term::stdout();
        term.clear_screen()?;
        
        println!("{}", style("╔══════════════════════════════════════════╗").green());
        println!("{}", style("║       AdGuard DNS - Console CLI         ║").green().bold());
        println!("{}", style("╚══════════════════════════════════════════╝").green());
        println!();
        
        Ok(())
    }

    /// Show a selection menu and return the selected index
    pub fn select<T: Display>(title: &str, items: &[T]) -> Result<usize> {
        Select::with_theme(&ColorfulTheme::default())
            .with_prompt(title)
            .items(items)
            .default(0)
            .interact()
            .map_err(Into::into)
    }

    /// Show a menu with string choices
    pub fn select_from_choices(title: &str, choices: &[&str]) -> Result<usize> {
        Select::with_theme(&ColorfulTheme::default())
            .with_prompt(title)
            .items(choices)
            .default(0)
            .interact()
            .map_err(Into::into)
    }

    /// Confirm an action
    pub fn confirm(prompt: &str) -> Result<bool> {
        Confirm::with_theme(&ColorfulTheme::default())
            .with_prompt(prompt)
            .default(false)
            .interact()
            .map_err(Into::into)
    }

    /// Get string input from user
    pub fn input(prompt: &str) -> Result<String> {
        Input::with_theme(&ColorfulTheme::default())
            .with_prompt(prompt)
            .interact_text()
            .map_err(Into::into)
    }

    /// Get password/secret input from user
    pub fn input_password(prompt: &str) -> Result<String> {
        dialoguer::Password::with_theme(&ColorfulTheme::default())
            .with_prompt(prompt)
            .interact()
            .map_err(Into::into)
    }

    /// Display a success message
    pub fn success(message: &str) {
        println!("{} {}", style("✓").green().bold(), style(message).green());
    }

    /// Display an error message
    pub fn error(message: &str) {
        eprintln!("{} {}", style("✗").red().bold(), style(message).red());
    }

    /// Display a warning message
    pub fn warning(message: &str) {
        println!("{} {}", style("⚠").yellow().bold(), style(message).yellow());
    }

    /// Display an info message
    pub fn info(message: &str) {
        println!("{} {}", style("ℹ").blue().bold(), style(message).cyan());
    }

    /// Display a status message with spinner
    pub fn status(message: &str) {
        println!("{} {}", style("⟳").cyan().bold(), style(message).cyan());
    }

    /// Show "No items" message
    pub fn no_items(item_type: &str) {
        Self::info(&format!("No {} found.", item_type));
    }

    /// Show "Cancelled" message
    pub fn cancelled() {
        Self::info("Operation cancelled.");
    }

    /// Press any key to continue
    pub fn press_any_key() -> Result<()> {
        println!();
        println!("{}", style("Press Enter to continue...").dim());
        let _ = Input::<String>::new()
            .allow_empty(true)
            .interact_text()?;
        Ok(())
    }

    /// Display a table header
    pub fn table_header(columns: &[&str]) {
        println!();
        println!(
            "{}",
            style(columns.join(" │ ")).bold().underlined()
        );
    }

    /// Display a table row
    pub fn table_row(values: &[String]) {
        println!("{}", values.join(" │ "));
    }

    /// Display a divider
    pub fn divider() {
        println!("{}", style("─".repeat(80)).dim());
    }
}
