# Security Policy

## Supported Versions

This project is actively maintained. Security updates are provided for the latest version only.

| Version | Supported          |
| ------- | ------------------ |
| Latest  | :white_check_mark: |
| < Latest| :x:                |

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly:

### How to Report

1. **Do NOT** open a public GitHub issue for security vulnerabilities
2. Send an email to the repository owner via their GitHub profile
3. Include the following information:
   - Description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if any)

### What to Expect

- **Initial Response**: You will receive an acknowledgment within 48 hours
- **Status Updates**: You will receive updates every 5-7 days on the progress
- **Resolution Timeline**: We aim to resolve critical vulnerabilities within 30 days
- **Disclosure**: Once fixed, we will coordinate responsible disclosure with you

### Security Best Practices

When using this project:

1. **Never commit sensitive data**: Always use environment variables for secrets
   - Set `ADGUARD_WEBHOOK_URL` environment variable
   - Set `SECRET_KEY` environment variable
   - See `.env.example` files for configuration templates

2. **Keep dependencies updated**: Regularly update dependencies to patch known vulnerabilities

3. **Review filter rules**: Be cautious when adding new filter rules from untrusted sources

## Security Features

This project implements several security measures:

- Rate limiting for HTTP requests
- Configuration via environment variables (not hardcoded secrets)
- Automated security scanning via GitHub Actions (CodeQL, DevSkim)
- Regular dependency updates

## Out of Scope

The following are considered out of scope for security reports:

- Issues in third-party dependencies (report to the upstream project)
- Social engineering attacks
- Physical attacks
- Denial of service attacks

Thank you for helping keep this project secure!
