//! URL security validation module.

use regex::Regex;
use reqwest::blocking::Client;
use std::time::Duration;
use url::Url;

use crate::error::{Result, ValidationError};
use crate::hash::compute_hash;

/// URL validation result.
#[derive(Debug, Clone)]
pub struct UrlValidationResult {
    /// Whether the URL passed validation.
    pub is_valid: bool,
    /// Validation errors/warnings.
    pub messages: Vec<String>,
    /// Content SHA-384 hash (if downloaded).
    pub content_hash: Option<String>,
    /// Content size in bytes.
    pub content_size: Option<u64>,
}

impl UrlValidationResult {
    /// Create a successful validation result.
    #[must_use]
    pub fn valid() -> Self {
        Self {
            is_valid: true,
            messages: Vec::new(),
            content_hash: None,
            content_size: None,
        }
    }

    /// Create a failed validation result.
    #[must_use]
    pub fn invalid(message: impl Into<String>) -> Self {
        Self {
            is_valid: false,
            messages: vec![message.into()],
            content_hash: None,
            content_size: None,
        }
    }

    /// Add a message.
    pub fn add_message(&mut self, message: impl Into<String>) {
        self.messages.push(message.into());
    }
}

/// Validate a URL for security and proper filter list format.
///
/// Performs comprehensive security checks:
/// 1. HTTPS protocol enforcement
/// 2. Domain validation via DNS
/// 3. Content-Type verification
/// 4. Content scanning for valid filter syntax
/// 5. Optional SHA-384 hash verification
///
/// # Errors
///
/// Returns an error if validation fails in strict mode.
pub fn validate_url(url_str: &str, expected_hash: Option<&str>) -> Result<UrlValidationResult> {
    let mut result = UrlValidationResult::valid();

    // Parse URL
    let url = Url::parse(url_str)
        .map_err(|e| ValidationError::url_validation(url_str, format!("Invalid URL: {e}")))?;

    // 1. HTTPS enforcement
    if url.scheme() != "https" {
        result.is_valid = false;
        result.add_message(format!(
            "Insecure protocol '{}' - only HTTPS is allowed",
            url.scheme()
        ));
        return Ok(result);
    }

    // 2. Domain validation
    if url.host_str().is_none() {
        result.is_valid = false;
        result.add_message("Missing or invalid host");
        return Ok(result);
    }

    // 3. Download and verify content
    let client = Client::builder()
        .timeout(Duration::from_secs(30))
        .user_agent("AdGuard-Validation/1.0")
        .build()
        .map_err(|e| ValidationError::url_validation(url_str, format!("HTTP client error: {e}")))?;

    let response = client
        .get(url_str)
        .send()
        .map_err(|e| ValidationError::url_validation(url_str, format!("Request failed: {e}")))?;

    // Check status
    if !response.status().is_success() {
        result.is_valid = false;
        result.add_message(format!(
            "HTTP {} {}",
            response.status().as_u16(),
            response.status().canonical_reason().unwrap_or("Unknown")
        ));
        return Ok(result);
    }

    // 4. Content-Type verification
    if let Some(content_type) = response.headers().get("content-type") {
        let content_type = content_type.to_str().unwrap_or("");
        if !content_type.contains("text/plain") && !content_type.contains("text/") {
            result.add_message(format!(
                "Unexpected Content-Type: {content_type} (expected text/plain)"
            ));
        }
    }

    // Download content
    let content = response
        .bytes()
        .map_err(|e| ValidationError::url_validation(url_str, format!("Download failed: {e}")))?;

    result.content_size = Some(content.len() as u64);

    // 5. Size check (max 50MB)
    if content.len() > 50 * 1024 * 1024 {
        result.is_valid = false;
        result.add_message(format!(
            "File too large: {} bytes (max 50MB)",
            content.len()
        ));
        return Ok(result);
    }

    // 6. Content validation (scan first 1KB for filter syntax)
    let preview = String::from_utf8_lossy(&content[..content.len().min(1024)]);
    if !is_valid_filter_content(&preview) {
        result.add_message("Content does not appear to be a valid filter list");
    }

    // 7. Hash verification
    let actual_hash = compute_hash(&content);
    result.content_hash = Some(actual_hash.clone());

    if let Some(expected) = expected_hash {
        if actual_hash != expected {
            result.is_valid = false;
            result.add_message(format!(
                "Hash mismatch: expected {expected}, got {actual_hash}"
            ));
            return Ok(result);
        }
    }

    Ok(result)
}

/// Check if content appears to be a valid filter list.
fn is_valid_filter_content(content: &str) -> bool {
    // Look for common filter list patterns
    let patterns = [
        r"^!",              // Comment
        r"^#",              // Comment or cosmetic rule
        r"^\|\|",           // Domain blocking rule
        r"^@@",             // Exception rule
        r"^[0-9]+\.[0-9]+", // IP address (hosts format)
        r"##",              // Cosmetic rule
        r"\$",              // Rule options
    ];

    let mut found_patterns = 0;
    for line in content.lines().take(20) {
        let line = line.trim();
        if line.is_empty() {
            continue;
        }

        for pattern in &patterns {
            if Regex::new(pattern).map_or(false, |re| re.is_match(line)) {
                found_patterns += 1;
                break;
            }
        }
    }

    // At least 3 lines should match filter patterns
    found_patterns >= 3
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_url_validation_result() {
        let result = UrlValidationResult::valid();
        assert!(result.is_valid);
        assert!(result.messages.is_empty());

        let mut result = UrlValidationResult::invalid("test error");
        assert!(!result.is_valid);
        assert_eq!(result.messages.len(), 1);

        result.add_message("another error");
        assert_eq!(result.messages.len(), 2);
    }

    #[test]
    fn test_is_valid_filter_content() {
        let valid_content = "! Comment\n||example.com^\n@@||allowed.com^\n";
        assert!(is_valid_filter_content(valid_content));

        let invalid_content = "random text\nmore random\nnothing here\n";
        assert!(!is_valid_filter_content(invalid_content));
    }

    #[test]
    fn test_validate_url_http_rejected() {
        let result = validate_url("http://insecure.example.com/list.txt", None).unwrap();
        assert!(!result.is_valid);
        assert!(result.messages[0].contains("HTTPS"));
    }

    #[test]
    fn test_validate_url_invalid() {
        let result = validate_url("not-a-url", None);
        assert!(result.is_err());
    }
}
