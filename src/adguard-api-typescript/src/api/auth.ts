/**
 * Authentication API client
 */

import { BaseApi } from './base.js';
import { ApiConfiguration } from '../helpers/configuration.js';
import {
  AccessTokenCredentials,
  AccessTokenResponse,
} from '../models/index.js';

/** Authentication API endpoints */
export class AuthApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * Get access token (OAuth login)
   * @param credentials - Login credentials
   * @returns Access token response
   */
  async accessToken(credentials: AccessTokenCredentials): Promise<AccessTokenResponse> {
    this.logger.debug('Requesting access token');
    return this.post<AccessTokenResponse>('/oapi/v1/oauth_token', credentials);
  }

  /**
   * Revoke a refresh token
   * @param refreshToken - The refresh token to revoke
   */
  async revokeToken(refreshToken: string): Promise<void> {
    this.logger.debug('Revoking refresh token');
    await this.post<void>(`/oapi/v1/revoke_token?refresh_token=${encodeURIComponent(refreshToken)}`);
  }
}
