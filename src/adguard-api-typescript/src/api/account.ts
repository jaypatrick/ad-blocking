/**
 * Account API client
 */

import { BaseApi } from './base.js';
import { ApiConfiguration } from '../helpers/configuration.js';
import { AccountLimits } from '../models/index.js';

/** Account API endpoints */
export class AccountApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * Get account limits
   * @returns Account limits information
   */
  async getAccountLimits(): Promise<AccountLimits> {
    this.logger.debug('Getting account limits');
    return this.get<AccountLimits>('/oapi/v1/account/limits');
  }
}
