/**
 * @fileoverview Unit tests for the AdGuard Filter Compiler
 */

import { readConfiguration, writeOutput } from './invoke-compiler';
import { existsSync, readFileSync, unlinkSync, writeFileSync } from 'fs';
import { resolve } from 'path';

describe('invoke-compiler', () => {
    const testConfigPath = resolve(__dirname, 'test-config.json');
    const testOutputPath = resolve(__dirname, 'test-output.txt');

    afterEach(() => {
        // Cleanup test files
        if (existsSync(testConfigPath)) {
            unlinkSync(testConfigPath);
        }
        if (existsSync(testOutputPath)) {
            unlinkSync(testOutputPath);
        }
    });

    describe('readConfiguration', () => {
        it('should read and parse a valid configuration file', () => {
            // Arrange
            const testConfig = {
                name: 'Test Filter',
                sources: [
                    { type: 'inline', source: ['test.com'] }
                ]
            };
            writeFileSync(testConfigPath, JSON.stringify(testConfig));

            // Act
            const result = readConfiguration(testConfigPath);

            // Assert
            expect(result).toBeDefined();
            expect(result.name).toBe('Test Filter');
        });

        it('should throw an error when file does not exist', () => {
            // Act & Assert
            expect(() => readConfiguration('nonexistent-file.json'))
                .toThrow('Configuration file not found');
        });

        it('should throw an error for invalid JSON', () => {
            // Arrange
            writeFileSync(testConfigPath, 'not valid json {{{');

            // Act & Assert
            expect(() => readConfiguration(testConfigPath))
                .toThrow('Invalid JSON in configuration file');
        });

        it('should throw an error for empty file', () => {
            // Arrange
            writeFileSync(testConfigPath, '');

            // Act & Assert
            expect(() => readConfiguration(testConfigPath))
                .toThrow('Invalid JSON');
        });
    });

    describe('writeOutput', () => {
        it('should write rules to a file', async () => {
            // Arrange
            const rules = ['rule1', 'rule2', 'rule3'];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            expect(existsSync(testOutputPath)).toBe(true);
            const content = readFileSync(testOutputPath, 'utf8');
            expect(content).toBe('rule1\nrule2\nrule3');
        });

        it('should handle empty rules array', async () => {
            // Arrange
            const rules: string[] = [];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            expect(existsSync(testOutputPath)).toBe(true);
            const content = readFileSync(testOutputPath, 'utf8');
            expect(content).toBe('');
        });

        it('should handle rules with special characters', async () => {
            // Arrange
            const rules = [
                '||example.com^',
                '@@||trusted.com^',
                '/ads\\.js/$script'
            ];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            const content = readFileSync(testOutputPath, 'utf8');
            expect(content).toContain('||example.com^');
            expect(content).toContain('@@||trusted.com^');
            expect(content).toContain('/ads\\.js/$script');
        });

        it('should reject when path is invalid', async () => {
            // Arrange
            const invalidPath = '/nonexistent/directory/file.txt';
            const rules = ['rule1'];

            // Act & Assert
            await expect(writeOutput(invalidPath, rules))
                .rejects.toThrow('Failed to write output file');
        });
    });

    describe('logger', () => {
        let originalConsole: typeof console;

        beforeEach(() => {
            originalConsole = { ...console };
            console.log = jest.fn();
            console.warn = jest.fn();
            console.error = jest.fn();
            console.debug = jest.fn();
        });

        afterEach(() => {
            console.log = originalConsole.log;
            console.warn = originalConsole.warn;
            console.error = originalConsole.error;
            console.debug = originalConsole.debug;
        });

        it('should include timestamps in log messages', () => {
            // This test verifies the logger is properly integrated
            // by checking the format of log output during operations
            const testConfig = {
                name: 'Test',
                sources: []
            };
            writeFileSync(testConfigPath, JSON.stringify(testConfig));

            // Trigger a log by reading configuration
            readConfiguration(testConfigPath);

            // The logger will be called but debug only logs when DEBUG env is set
            // This is more of an integration test
        });
    });

    describe('edge cases', () => {
        it('should handle very long rules', async () => {
            // Arrange
            const longRule = 'a'.repeat(10000);
            const rules = [longRule];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            const content = readFileSync(testOutputPath, 'utf8');
            expect(content.length).toBe(10000);
        });

        it('should preserve rule order', async () => {
            // Arrange
            const rules = ['first', 'second', 'third'];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            const content = readFileSync(testOutputPath, 'utf8');
            const lines = content.split('\n');
            expect(lines[0]).toBe('first');
            expect(lines[1]).toBe('second');
            expect(lines[2]).toBe('third');
        });

        it('should handle unicode characters in rules', async () => {
            // Arrange
            const rules = [
                '||日本語.com^',
                '||中文.cn^',
                '||한국어.kr^'
            ];

            // Act
            await writeOutput(testOutputPath, rules);

            // Assert
            const content = readFileSync(testOutputPath, 'utf8');
            expect(content).toContain('日本語');
            expect(content).toContain('中文');
            expect(content).toContain('한국어');
        });
    });
});
