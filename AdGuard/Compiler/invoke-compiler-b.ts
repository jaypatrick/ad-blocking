import HostlistCompiler, { IConfiguration as HostlistCompilerConfiguration } from '@adguard/hostlist-compiler'
import { readFileSync, writeFile } from 'node:fs'

;(async () => {
    // read configuration from file
    const config: HostlistCompilerConfiguration = JSON.parse(readFileSync('compiler-config.json', 'utf8')) satisfies HostlistCompilerConfiguration;
    
    // const config: HostlistCompilerConfiguration = {
    //     name: 'Your Hostlist',
    //     sources: [
    //         {
    //             type: 'adblock',
    //             source: 'https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt',
    //             transformations: ['RemoveComments', 'Validate'],
    //         },
    //     ],
    //     transformations: ['Deduplicate'],
    // };

    // Compile filters
    const result = await HostlistCompiler(config);

    // Write to file
    writeFile('adguard_user_filter.txt', result.join('\n'), (err) => {
        if (err) {
            console.error('Error writing to file:', err);
        }
    });
})();