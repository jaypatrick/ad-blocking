/**
 * @fileoverview Tests for the configuration reader module
 */

import { detectFormat, readConfiguration, toJson } from './config-reader';
import { existsSync, writeFileSync, unlinkSync, mkdirSync, rmdirSync } from 'fs';
import { resolve, join } from 'path';

describe('config-reader', () => {
    const testDir = resolve(__dirname, '..', 'test-fixtures');
    const testJsonPath = join(testDir, 'test-config.json');
    const testYamlPath = join(testDir, 'test-config.yaml');
    const testTomlPath = join(testDir, 'test-config.toml');

    beforeAll(() => {
        if (!existsSync(testDir)) {
            mkdirSync(testDir, { recursive: true });
        }
    });

    afterEach(() => {
        // Cleanup test files
        [testJsonPath, testYamlPath, testTomlPath].forEach(path => {
            if (existsSync(path)) {
                unlinkSync(path);
            }
        });
    });

    afterAll(() => {
        if (existsSync(testDir)) {
            try {
                rmdirSync(testDir);
            } catch {
                // Directory not empty, ignore
            }
        }
    });

    describe('detectFormat', () => {
        it('should detect JSON format', () => {
            expect(detectFormat('config.json')).toBe('json');
            expect(detectFormat('/path/to/config.JSON')).toBe('json');
        });

        it('should detect YAML format', () => {
            expect(detectFormat('config.yaml')).toBe('yaml');
            expect(detectFormat('config.yml')).toBe('yaml');
            expect(detectFormat('/path/to/config.YAML')).toBe('yaml');
        });

        it('should detect TOML format', () => {
            expect(detectFormat('config.toml')).toBe('toml');
            expect(detectFormat('/path/to/config.TOML')).toBe('toml');
        });

        it('should throw for unknown extensions', () => {
            expect(() => detectFormat('config.txt')).toThrow('Unknown configuration file extension');
            expect(() => detectFormat('config.xml')).toThrow('Unknown configuration file extension');
            expect(() => detectFormat('config')).toThrow('Unknown configuration file extension');
        });
    });

    describe('readConfiguration', () => {
        it('should read and parse JSON configuration', () => {
            const config = {
                name: 'Test Filter',
                version: '1.0.0',
                sources: [{ name: 'Local', source: './rules.txt', type: 'adblock' }],
                transformations: ['Deduplicate']
            };
            writeFileSync(testJsonPath, JSON.stringify(config, null, 2));

            const result = readConfiguration(testJsonPath);

            expect(result.name).toBe('Test Filter');
            expect(result._sourceFormat).toBe('json');
            expect(result._sourcePath).toBe(testJsonPath);
        });

        it('should read and parse YAML configuration', () => {
            const yamlContent = `
name: YAML Test Filter
version: 2.0.0
sources:
  - name: Remote
    source: https://example.com/rules.txt
    type: adblock
transformations:
  - Validate
`;
            writeFileSync(testYamlPath, yamlContent);

            const result = readConfiguration(testYamlPath);

            expect(result.name).toBe('YAML Test Filter');
            expect(result._sourceFormat).toBe('yaml');
        });

        it('should read and parse TOML configuration', () => {
            const tomlContent = `
name = "TOML Test Filter"
version = "3.0.0"
transformations = ["Compress"]

[[sources]]
name = "Local"
source = "./rules.txt"
type = "adblock"
`;
            writeFileSync(testTomlPath, tomlContent);

            const result = readConfiguration(testTomlPath);

            expect(result.name).toBe('TOML Test Filter');
            expect(result._sourceFormat).toBe('toml');
        });

        it('should throw for missing file', () => {
            expect(() => readConfiguration('/nonexistent/config.json'))
                .toThrow('Configuration file not found');
        });

        it('should throw for invalid JSON', () => {
            writeFileSync(testJsonPath, 'not valid json {{{');

            expect(() => readConfiguration(testJsonPath))
                .toThrow('Invalid JSON');
        });

        it('should respect format override', () => {
            // Write YAML content with .txt extension
            const yamlContent = `
name: Override Test
version: 1.0.0
sources: []
transformations: []
`;
            const txtPath = join(testDir, 'config.txt');
            writeFileSync(txtPath, yamlContent);

            try {
                const result = readConfiguration(txtPath, 'yaml');
                expect(result.name).toBe('Override Test');
                expect(result._sourceFormat).toBe('yaml');
            } finally {
                if (existsSync(txtPath)) {
                    unlinkSync(txtPath);
                }
            }
        });
    });

    describe('toJson', () => {
        it('should convert configuration to JSON', () => {
            const config = {
                name: 'Test',
                version: '1.0.0',
                sources: [],
                transformations: [],
                _sourceFormat: 'yaml' as const,
                _sourcePath: '/path/to/config.yaml'
            };

            const json = toJson(config);
            const parsed = JSON.parse(json);

            expect(parsed.name).toBe('Test');
            expect(parsed._sourceFormat).toBeUndefined();
            expect(parsed._sourcePath).toBeUndefined();
        });
    });
});
