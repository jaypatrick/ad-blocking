/**
 * Web Services API client
 */

import { BaseApi } from './base.ts';
import { ApiConfiguration } from '../helpers/configuration.ts';
import { WebService } from '../models/index.ts';

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
