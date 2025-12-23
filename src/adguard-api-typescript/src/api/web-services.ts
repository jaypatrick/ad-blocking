/**
 * Web Services API client
 */

import { BaseApi } from './base.js';
import { ApiConfiguration } from '../helpers/configuration.js';
import { WebService } from '../models/index.js';

/** Web Services API endpoints */
export class WebServicesApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * List all web services
   * @returns Array of web services
   */
  async listWebServices(): Promise<WebService[]> {
    this.logger.debug('Listing web services');
    return this.get<WebService[]>('/oapi/v1/web_services');
  }
}
