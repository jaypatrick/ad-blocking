/**
 * Authentication-related models
 */

/** Access token credentials for login */
export interface AccessTokenCredentials {
  /** Account email */
  username?: string;
  /** Account password */
  password?: string;
  /** Two-Factor authentication token */
  mfa_token?: string;
  /** Refresh token for renewal */
  refresh_token?: string;
}

/** Access token success response */
export interface AccessTokenResponse {
  /** Access token */
  access_token: string;
  /** Token type (bearer) */
  token_type: string;
  /** Lifetime in seconds */
  expires_in: number;
  /** Refresh token for renewal */
  refresh_token?: string;
}

/** Access token error response */
export interface AccessTokenErrorResponse {
  /** Error type */
  error: string;
  /** Error code */
  error_code?: string;
  /** Error description */
  error_description?: string;
}
