import compile from '@adguard/hostlist-compiler';
import { writeFileSync } from 'fs';

;(async () => {
    // Compile filters
    const result = await compile({
        name: 'Your Hostlist',
        sources: [
            {
                type: 'adblock',
                source: 'https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt',
                transformations: ['RemoveComments', 'Validate'],
            },
        ],
        transformations: ['Deduplicate'],
    });

    // Write to file
    writeFileSync('your-hostlist.txt', result.join('\n'));
})();