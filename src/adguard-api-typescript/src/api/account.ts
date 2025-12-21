/**
 * Account API client
 */

import { BaseApi } from './base';
import { ApiConfiguration } from '../helpers/configuration';
import { AccountLimits } from '../models';

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
