/**
 * @fileoverview Tests for the CLI module
 */

import { parseArgs, getVersionInfo } from './cli';

describe('cli', () => {
    describe('parseArgs', () => {
        it('should parse config path with -c flag', () => {
            const options = parseArgs(['-c', 'config.yaml']);
            expect(options.configPath).toBe('config.yaml');
        });

        it('should parse config path with --config flag', () => {
            const options = parseArgs(['--config', 'config.json']);
            expect(options.configPath).toBe('config.json');
        });

        it('should parse output path with -o flag', () => {
            const options = parseArgs(['-o', 'output.txt']);
            expect(options.outputPath).toBe('output.txt');
        });

        it('should parse output path with --output flag', () => {
            const options = parseArgs(['--output', 'my-rules.txt']);
            expect(options.outputPath).toBe('my-rules.txt');
        });

        it('should parse copy-to-rules flag', () => {
            const options1 = parseArgs(['-r']);
            expect(options1.copyToRules).toBe(true);

            const options2 = parseArgs(['--copy-to-rules']);
            expect(options2.copyToRules).toBe(true);
        });

        it('should parse format flag', () => {
            const options = parseArgs(['-f', 'yaml']);
            expect(options.format).toBe('yaml');
        });

        it('should parse version flag', () => {
            const options1 = parseArgs(['-v']);
            expect(options1.version).toBe(true);

            const options2 = parseArgs(['--version']);
            expect(options2.version).toBe(true);
        });

        it('should parse help flag', () => {
            const options1 = parseArgs(['-h']);
            expect(options1.help).toBe(true);

            const options2 = parseArgs(['--help']);
            expect(options2.help).toBe(true);
        });

        it('should parse debug flag', () => {
            const options1 = parseArgs(['-d']);
            expect(options1.debug).toBe(true);

            const options2 = parseArgs(['--debug']);
            expect(options2.debug).toBe(true);
        });

        it('should parse positional config path', () => {
            const options = parseArgs(['my-config.json']);
            expect(options.configPath).toBe('my-config.json');
        });

        it('should handle multiple flags', () => {
            const options = parseArgs([
                '-c', 'config.yaml',
                '-o', 'output.txt',
                '-r',
                '-f', 'yaml',
                '-d'
            ]);

            expect(options.configPath).toBe('config.yaml');
            expect(options.outputPath).toBe('output.txt');
            expect(options.copyToRules).toBe(true);
            expect(options.format).toBe('yaml');
            expect(options.debug).toBe(true);
        });

        it('should throw for invalid format', () => {
            expect(() => parseArgs(['-f', 'invalid']))
                .toThrow('Invalid format: invalid');
        });

        it('should return defaults for empty args', () => {
            const options = parseArgs([]);

            expect(options.copyToRules).toBe(false);
            expect(options.version).toBe(false);
            expect(options.help).toBe(false);
            expect(options.debug).toBe(false);
            expect(options.configPath).toBeUndefined();
            expect(options.outputPath).toBeUndefined();
            expect(options.format).toBeUndefined();
        });
    });

    describe('getVersionInfo', () => {
        it('should return version information', () => {
            const info = getVersionInfo();

            expect(info.moduleVersion).toBeDefined();
            expect(info.nodeVersion).toBe(process.version);
            expect(info.platform.os).toBe(process.platform);
            expect(info.platform.arch).toBe(process.arch);
        });
    });
});
