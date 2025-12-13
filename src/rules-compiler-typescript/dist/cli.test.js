"use strict";
/**
 * Tests for CLI argument parsing
 */
Object.defineProperty(exports, "__esModule", { value: true });
const cli_1 = require("./cli");
describe('parseArgs', () => {
    it('should return default options with no arguments', () => {
        const options = (0, cli_1.parseArgs)([]);
        expect(options.copyToRules).toBe(false);
        expect(options.version).toBe(false);
        expect(options.help).toBe(false);
        expect(options.debug).toBe(false);
        expect(options.configPath).toBeUndefined();
        expect(options.outputPath).toBeUndefined();
    });
    it('should parse -c flag for config path', () => {
        const options = (0, cli_1.parseArgs)(['-c', 'config.yaml']);
        expect(options.configPath).toBe('config.yaml');
    });
    it('should parse --config flag for config path', () => {
        const options = (0, cli_1.parseArgs)(['--config', 'config.json']);
        expect(options.configPath).toBe('config.json');
    });
    it('should parse -o flag for output path', () => {
        const options = (0, cli_1.parseArgs)(['-o', 'output.txt']);
        expect(options.outputPath).toBe('output.txt');
    });
    it('should parse --output flag for output path', () => {
        const options = (0, cli_1.parseArgs)(['--output', 'rules.txt']);
        expect(options.outputPath).toBe('rules.txt');
    });
    it('should parse -r flag for copy to rules', () => {
        const options = (0, cli_1.parseArgs)(['-r']);
        expect(options.copyToRules).toBe(true);
    });
    it('should parse --copy-to-rules flag', () => {
        const options = (0, cli_1.parseArgs)(['--copy-to-rules']);
        expect(options.copyToRules).toBe(true);
    });
    it('should parse -f flag for format', () => {
        const options = (0, cli_1.parseArgs)(['-f', 'yaml']);
        expect(options.format).toBe('yaml');
    });
    it('should parse --format flag', () => {
        const options = (0, cli_1.parseArgs)(['--format', 'toml']);
        expect(options.format).toBe('toml');
    });
    it('should throw error for invalid format', () => {
        expect(() => (0, cli_1.parseArgs)(['-f', 'xml'])).toThrow('Invalid format: xml');
    });
    it('should parse -v flag for version', () => {
        const options = (0, cli_1.parseArgs)(['-v']);
        expect(options.version).toBe(true);
    });
    it('should parse --version flag', () => {
        const options = (0, cli_1.parseArgs)(['--version']);
        expect(options.version).toBe(true);
    });
    it('should parse -h flag for help', () => {
        const options = (0, cli_1.parseArgs)(['-h']);
        expect(options.help).toBe(true);
    });
    it('should parse --help flag', () => {
        const options = (0, cli_1.parseArgs)(['--help']);
        expect(options.help).toBe(true);
    });
    it('should parse -d flag for debug', () => {
        const options = (0, cli_1.parseArgs)(['-d']);
        expect(options.debug).toBe(true);
    });
    it('should parse --debug flag', () => {
        const options = (0, cli_1.parseArgs)(['--debug']);
        expect(options.debug).toBe(true);
    });
    it('should parse --show-config flag', () => {
        const options = (0, cli_1.parseArgs)(['--show-config']);
        expect(options.showConfig).toBe(true);
    });
    it('should parse positional config path', () => {
        const options = (0, cli_1.parseArgs)(['config.yaml']);
        expect(options.configPath).toBe('config.yaml');
    });
    it('should parse multiple flags together', () => {
        const options = (0, cli_1.parseArgs)(['-c', 'config.yaml', '-o', 'output.txt', '-r', '-d']);
        expect(options.configPath).toBe('config.yaml');
        expect(options.outputPath).toBe('output.txt');
        expect(options.copyToRules).toBe(true);
        expect(options.debug).toBe(true);
    });
    it('should parse --rules-dir flag', () => {
        const options = (0, cli_1.parseArgs)(['--rules-dir', '/custom/rules']);
        expect(options.rulesDirectory).toBe('/custom/rules');
    });
});
//# sourceMappingURL=cli.test.js.map