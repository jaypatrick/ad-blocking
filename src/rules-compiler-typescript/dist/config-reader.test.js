"use strict";
/**
 * Tests for configuration reader
 */
Object.defineProperty(exports, "__esModule", { value: true });
const config_reader_1 = require("./config-reader");
describe('detectFormat', () => {
    it('should detect JSON format from .json extension', () => {
        expect((0, config_reader_1.detectFormat)('config.json')).toBe('json');
        expect((0, config_reader_1.detectFormat)('/path/to/compiler-config.json')).toBe('json');
    });
    it('should detect YAML format from .yaml extension', () => {
        expect((0, config_reader_1.detectFormat)('config.yaml')).toBe('yaml');
        expect((0, config_reader_1.detectFormat)('/path/to/compiler-config.yaml')).toBe('yaml');
    });
    it('should detect YAML format from .yml extension', () => {
        expect((0, config_reader_1.detectFormat)('config.yml')).toBe('yaml');
        expect((0, config_reader_1.detectFormat)('/path/to/compiler-config.yml')).toBe('yaml');
    });
    it('should detect TOML format from .toml extension', () => {
        expect((0, config_reader_1.detectFormat)('config.toml')).toBe('toml');
        expect((0, config_reader_1.detectFormat)('/path/to/compiler-config.toml')).toBe('toml');
    });
    it('should be case-insensitive for extensions', () => {
        expect((0, config_reader_1.detectFormat)('config.JSON')).toBe('json');
        expect((0, config_reader_1.detectFormat)('config.YAML')).toBe('yaml');
        expect((0, config_reader_1.detectFormat)('config.TOML')).toBe('toml');
    });
    it('should throw error for unknown extension', () => {
        expect(() => (0, config_reader_1.detectFormat)('config.xml')).toThrow('Unknown configuration file extension: .xml');
        expect(() => (0, config_reader_1.detectFormat)('config.txt')).toThrow('Unknown configuration file extension: .txt');
    });
});
//# sourceMappingURL=config-reader.test.js.map