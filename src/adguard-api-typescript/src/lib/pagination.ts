/**
 * Pagination support for list operations
 *
 * Provides a fluent builder pattern for paginating and filtering list results.
 *
 * @example
 * ```typescript
 * // Paginate devices
 * const devices = await client.deviceRepository.list()
 *   .take(10)
 *   .skip(20)
 *   .toArray();
 *
 * // Filter with predicate
 * const filtered = await client.deviceRepository.list()
 *   .filter(device => device.settings?.protectionEnabled)
 *   .take(5)
 *   .toArray();
 *
 * // Get first item
 * const first = await client.deviceRepository.list()
 *   .first();
 *
 * // Count items
 * const count = await client.deviceRepository.list()
 *   .count();
 * ```
 */

/**
 * Paginated list builder interface
 */
export interface PagedList<T> {
  /** Take only the first N items */
  take(limit: number): PagedList<T>;

  /** Skip the first N items */
  skip(offset: number): PagedList<T>;

  /** Filter items with a predicate */
  filter(predicate: (item: T) => boolean): PagedList<T>;

  /** Sort items */
  sort(compareFn: (a: T, b: T) => number): PagedList<T>;

  /** Convert to array */
  toArray(): Promise<T[]>;

  /** Get the first item */
  first(): Promise<T | undefined>;

  /** Get the first item or throw if none */
  firstOrThrow(): Promise<T>;

  /** Count items (after filtering) */
  count(): Promise<number>;

  /** Check if any items exist */
  any(): Promise<boolean>;

  /** Execute callback for each item */
  forEach(callback: (item: T) => void | Promise<void>): Promise<void>;

  /** Map items to a new type */
  map<U>(mapper: (item: T) => U): PagedList<U>;
}

/**
 * Pagination options
 */
export interface PaginationOptions {
  /** Maximum items to return */
  limit?: number;
  /** Items to skip */
  offset?: number;
}

/**
 * Page result with metadata
 */
export interface PageResult<T> {
  /** Items on this page */
  items: T[];
  /** Total count (if known) */
  totalCount?: number;
  /** Current page offset */
  offset: number;
  /** Page size limit */
  limit: number;
  /** Whether there are more items */
  hasMore?: boolean;
}

/**
 * Create a paged list builder from a data source
 */
export class PagedListBuilder<T> implements PagedList<T> {
  private limitValue?: number;
  private offsetValue = 0;
  private filters: Array<(item: T) => boolean> = [];
  private sortFn?: (a: T, b: T) => number;

  constructor(private readonly dataSource: () => Promise<T[]>) {}

  /**
   * Create a paged list from a data source function
   */
  static from<T>(dataSource: () => Promise<T[]>): PagedList<T> {
    return new PagedListBuilder<T>(dataSource);
  }

  /**
   * Create a paged list from an existing array
   */
  static fromArray<T>(items: T[]): PagedList<T> {
    return new PagedListBuilder<T>(() => Promise.resolve(items));
  }

  take(limit: number): PagedList<T> {
    if (limit < 0) {
      throw new Error('Limit must be non-negative');
    }
    this.limitValue = limit;
    return this;
  }

  skip(offset: number): PagedList<T> {
    if (offset < 0) {
      throw new Error('Offset must be non-negative');
    }
    this.offsetValue = offset;
    return this;
  }

  filter(predicate: (item: T) => boolean): PagedList<T> {
    this.filters.push(predicate);
    return this;
  }

  sort(compareFn: (a: T, b: T) => number): PagedList<T> {
    this.sortFn = compareFn;
    return this;
  }

  async toArray(): Promise<T[]> {
    let items = await this.dataSource();

    // Apply filters
    for (const filter of this.filters) {
      items = items.filter(filter);
    }

    // Apply sort
    if (this.sortFn) {
      items = [...items].sort(this.sortFn);
    }

    // Apply pagination
    const start = this.offsetValue;
    const end = this.limitValue !== undefined ? start + this.limitValue : undefined;
    items = items.slice(start, end);

    return items;
  }

  async first(): Promise<T | undefined> {
    const items = await this.take(1).toArray();
    return items[0];
  }

  async firstOrThrow(): Promise<T> {
    const item = await this.first();
    if (item === undefined) {
      throw new Error('No items found');
    }
    return item;
  }

  async count(): Promise<number> {
    // For count, we need all filtered items but not pagination
    let items = await this.dataSource();

    for (const filter of this.filters) {
      items = items.filter(filter);
    }

    return items.length;
  }

  async any(): Promise<boolean> {
    const count = await this.count();
    return count > 0;
  }

  async forEach(callback: (item: T) => void | Promise<void>): Promise<void> {
    const items = await this.toArray();
    for (const item of items) {
      await callback(item);
    }
  }

  map<U>(mapper: (item: T) => U): PagedList<U> {
    return new PagedListBuilder<U>(async () => {
      const items = await this.toArray();
      return items.map(mapper);
    });
  }
}

/**
 * Helper to create a paged list from a repository method
 */
export function createPagedList<T>(dataSource: () => Promise<T[]>): PagedList<T> {
  return PagedListBuilder.from(dataSource);
}

/**
 * Apply pagination to an array
 */
export function paginate<T>(items: T[], options: PaginationOptions): T[] {
  const { limit, offset = 0 } = options;
  const start = offset;
  const end = limit !== undefined ? start + limit : undefined;
  return items.slice(start, end);
}

/**
 * Create a page result from an array
 */
export function createPageResult<T>(
  items: T[],
  options: PaginationOptions,
  totalCount?: number,
): PageResult<T> {
  const { limit = items.length, offset = 0 } = options;
  const paginatedItems = paginate(items, options);

  return {
    items: paginatedItems,
    totalCount,
    offset,
    limit,
    hasMore: totalCount !== undefined ? offset + paginatedItems.length < totalCount : undefined,
  };
}
