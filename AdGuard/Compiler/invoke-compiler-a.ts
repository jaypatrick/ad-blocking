import compile, { IConfiguration } from '@adguard/hostlist-compiler';
import { readFileSync, writeFile } from 'node:fs';

;(async () => {
    // read configuration from file
    const config: IConfiguration = JSON.parse(readFileSync('compiler-config.json', 'utf8')) satisfies IConfiguration;

    // const result = await compile({
    //     name: 'Your Hostlist',
    //     sources: [
    //         {
    //             type: 'adblock',
    //             source: 'https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt',
    //             transformations: ['RemoveComments', 'Validate'],
    //         },
    //     ],
    //     transformations: ['Deduplicate'],
    // });

    // compile filters
    const result = await compile(config);

    // Write to file
    writeFile('adguard_user_filter.txt', result.join('\n'), (err) => {
        if (err) {
            console.error('Error writing to file:', err);
        }
    });
})();
