import compile, { IConfiguration } from '@adguard/hostlist-compiler';
import { readFileSync, writeFile } from 'fs';

;(async () => {
    try {
        // read configuration from file
        const config: IConfiguration = JSON.parse(readFileSync('compiler-config.json', 'utf8')) as IConfiguration;

        console.log('Compiling filters...');

        // compile filters
        const result = await compile(config);

        console.log(`Compilation complete. Writing ${result.length} lines to file...`);

        // Write to file
        writeFile('adguard_user_filter.txt', result.join('\n'), (err) => {
            if (err) {
                console.error('Error writing to file:', err);
                process.exit(1);
            }
            console.log('Successfully wrote adguard_user_filter.txt');
        });
    } catch (error) {
        console.error('Error during compilation:', error);
        process.exit(1);
    }
})();
