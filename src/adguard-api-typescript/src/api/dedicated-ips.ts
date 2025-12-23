/**
 * Dedicated IP Addresses API client
 */

import { BaseApi } from './base.js';
import { ApiConfiguration } from '../helpers/configuration.js';
import { DedicatedIPv4Address } from '../models/index.js';

/** Dedicated IP Addresses API endpoints */
export class DedicatedIpApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * List all allocated dedicated IPv4 addresses
   * @returns Array of dedicated IPv4 addresses
   */
  async listDedicatedIPv4Addresses(): Promise<DedicatedIPv4Address[]> {
    this.logger.debug('Listing dedicated IPv4 addresses');
    return this.get<DedicatedIPv4Address[]>('/oapi/v1/dedicated_addresses/ipv4');
  }

  /**
   * Allocate a new dedicated IPv4 address
   * @returns Newly allocated IPv4 address
   */
  async allocateDedicatedIPv4Address(): Promise<DedicatedIPv4Address> {
    this.logger.debug('Allocating dedicated IPv4 address');
    return this.post<DedicatedIPv4Address>('/oapi/v1/dedicated_addresses/ipv4');
  }
}
