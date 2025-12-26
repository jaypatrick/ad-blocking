/**
 * Account-related models
 */

/** Limit structure for account resources */
export interface Limit {
  /** Current count */
  current: number;
  /** Maximum allowed */
  max: number;
}

/** Account limits */
export interface AccountLimits {
  /** Access rules limit */
  access_rules: Limit;
  /** Dedicated IPv4 limit */
  dedicated_ipv4: Limit;
  /** Devices limit */
  devices: Limit;
  /** DNS servers limit */
  dns_servers: Limit;
  /** Requests limit */
  requests: Limit;
  /** User rules limit */
  user_rules: Limit;
}
