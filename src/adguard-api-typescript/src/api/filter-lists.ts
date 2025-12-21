/**
 * Filter Lists API client
 */

import { BaseApi } from './base';
import { ApiConfiguration } from '../helpers/configuration';
import { FilterList } from '../models';

/** Filter Lists API endpoints */
export class FilterListsApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * List all available filter lists
   * @returns Array of filter lists
   */
  async listFilterLists(): Promise<FilterList[]> {
    this.logger.debug('Listing filter lists');
    return this.get<FilterList[]>('/oapi/v1/filter_lists');
  }
}
