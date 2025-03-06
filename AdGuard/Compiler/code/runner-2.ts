import HostlistCompiler, { IConfiguration as HostlistCompilerConfiguration } from '@adguard/hostlist-compiler';
import { writeFileSync } from 'fs';

;(async () => {
    // Configuration
    const config: HostlistCompilerConfiguration = {
        name: 'Your Hostlist',
        sources: [
            {
                type: 'adblock',
                source: 'https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt',
                transformations: ['RemoveComments', 'Validate'],
            },
        ],
        transformations: ['Deduplicate'],
    };

    // Compile filters
    const result = await HostlistCompiler(config);

    // Write to file
    writeFileSync('your-hostlist.txt', result.join('\n'));
})();