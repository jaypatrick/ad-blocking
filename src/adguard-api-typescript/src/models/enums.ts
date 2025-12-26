/**
 * Enums for AdGuard DNS API
 * Based on OpenAPI spec v1.11
 */

/** Device type enumeration */
export enum DeviceType {
  WINDOWS = 'WINDOWS',
  ANDROID = 'ANDROID',
  MAC = 'MAC',
  IOS = 'IOS',
  LINUX = 'LINUX',
  ROUTER = 'ROUTER',
  SMART_TV = 'SMART_TV',
  GAME_CONSOLE = 'GAME_CONSOLE',
  UNKNOWN = 'UNKNOWN',
}

/** Blocking mode */
export enum BlockingMode {
  NONE = 'NONE',
  NULL_IP = 'NULL_IP',
  REFUSED = 'REFUSED',
  NXDOMAIN = 'NXDOMAIN',
  CUSTOM_IP = 'CUSTOM_IP',
}

/** Category type for statistics */
export enum CategoryType {
  ADS = 'ADS',
  TRACKERS = 'TRACKERS',
  SOCIAL_MEDIA = 'SOCIAL_MEDIA',
  CDN = 'CDN',
  OTHERS = 'OTHERS',
}

/** Day of week */
export enum DayOfWeek {
  MONDAY = 'MONDAY',
  TUESDAY = 'TUESDAY',
  WEDNESDAY = 'WEDNESDAY',
  THURSDAY = 'THURSDAY',
  FRIDAY = 'FRIDAY',
  SATURDAY = 'SATURDAY',
  SUNDAY = 'SUNDAY',
}

/** Filter list category type */
export enum FilterListCategoryType {
  GENERAL = 'GENERAL',
  SECURITY = 'SECURITY',
  REGIONAL = 'REGIONAL',
  OTHER = 'OTHER',
}

/** Filtering action source */
export enum FilteringActionSource {
  FILTERS = 'FILTERS',
  USER_FILTER = 'USER_FILTER',
  SAFEBROWSING = 'SAFEBROWSING',
  PARENTAL_SAFE_SEARCH = 'PARENTAL_SAFE_SEARCH',
  PARENTAL_YOUTUBE = 'PARENTAL_YOUTUBE',
  PARENTAL_ADULT = 'PARENTAL_ADULT',
  PARENTAL_BLOCKED_SERVICE = 'PARENTAL_BLOCKED_SERVICE',
  PARENTAL_SCHEDULE = 'PARENTAL_SCHEDULE',
  NEWLY_REGISTERED_DOMAINS = 'NEWLY_REGISTERED_DOMAINS',
}

/** Filtering action status */
export enum FilteringActionStatus {
  UNKNOWN = 'UNKNOWN',
  NONE = 'NONE',
  REQUEST_BLOCKED = 'REQUEST_BLOCKED',
  RESPONSE_BLOCKED = 'RESPONSE_BLOCKED',
  REQUEST_ALLOWED = 'REQUEST_ALLOWED',
  RESPONSE_ALLOWED = 'RESPONSE_ALLOWED',
  MODIFIED = 'MODIFIED',
}

/** DNS protocol response type */
export enum DnsProtoResponseType {
  RcodeSuccess = 'RcodeSuccess',
  RcodeFormatError = 'RcodeFormatError',
  RcodeServerFailure = 'RcodeServerFailure',
  RcodeNameError = 'RcodeNameError',
  RcodeNotImplemented = 'RcodeNotImplemented',
  RcodeRefused = 'RcodeRefused',
  RcodeYXDomain = 'RcodeYXDomain',
  RcodeYXRrset = 'RcodeYXRrset',
  RcodeNXRrset = 'RcodeNXRrset',
  RcodeNotAuth = 'RcodeNotAuth',
  RcodeNotZone = 'RcodeNotZone',
  RcodeBadSig = 'RcodeBadSig',
  RcodeBadVers = 'RcodeBadVers',
  RcodeBadKey = 'RcodeBadKey',
  RcodeBadTime = 'RcodeBadTime',
  RcodeBadMode = 'RcodeBadMode',
  RcodeBadName = 'RcodeBadName',
  RcodeBadAlg = 'RcodeBadAlg',
  RcodeBadTrunc = 'RcodeBadTrunc',
  RcodeBadCookie = 'RcodeBadCookie',
}

/** Secure DNS protocol type */
export enum SecureDnsProtoType {
  DOH = 'DOH',
  DOT = 'DOT',
  DOQ = 'DOQ',
  DNSCRYPT = 'DNSCRYPT',
}

/** Regular DNS protocol type */
export enum RegularDnsProtoType {
  UDP = 'UDP',
  TCP = 'TCP',
}

/** DNS query type */
export enum DnsQueryType {
  A = 'A',
  AAAA = 'AAAA',
  CNAME = 'CNAME',
  MX = 'MX',
  NS = 'NS',
  PTR = 'PTR',
  SOA = 'SOA',
  SRV = 'SRV',
  TXT = 'TXT',
  HTTPS = 'HTTPS',
  SVCB = 'SVCB',
}

/** Error codes */
export enum ErrorCodes {
  BAD_REQUEST = 'BAD_REQUEST',
  FIELD_REQUIRED = 'FIELD_REQUIRED',
  FIELD_WRONG_VALUE = 'FIELD_WRONG_VALUE',
  FIELD_REACHED_LIMIT = 'FIELD_REACHED_LIMIT',
  UNKNOWN = 'UNKNOWN',
}
