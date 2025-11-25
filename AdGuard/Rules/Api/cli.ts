import compile, { IConfiguration } from '@adguard/hostlist-compiler';
import { readFileSync, writeFileSync } from 'fs';
import { join } from 'path';

;(async () => {
    try {
        // Read configuration from Compiler directory
        const configPath = join(__dirname, '../../Compiler/compiler-config.json');
        const config: IConfiguration = JSON.parse(readFileSync(configPath, 'utf8')) as IConfiguration;

        console.log('Compiling filters...');

        // Compile filters
        const result = await compile(config);

        console.log(`Compilation complete. Writing ${result.length} lines to file...`);

        // Write to file in Rules directory
        const outputPath = join(__dirname, '../adguard_user_filter.txt');
        writeFileSync(outputPath, result.join('\n'));

        console.log('Successfully wrote adguard_user_filter.txt');
    } catch (error) {
        console.error('Error during compilation:', error);
        process.exit(1);
    }
})();