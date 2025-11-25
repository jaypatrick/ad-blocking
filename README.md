# Welcome to my ad-blocking repository
Various rules lists, scripts, and other goodies I use to block nuisances from my network.

## Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - For C# components
- [Node.js 18+](https://nodejs.org/) - For TypeScript compiler and Gatsby website
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) - For automation scripts
- [AdGuard HostList Compiler](https://github.com/AdguardTeam/HostlistCompiler) - For filter compilation

### Configuration

1. **Environment Variables**: Create a `.env` file or set environment variables:
   ```bash
   ADGUARD_WEBHOOK_URL=https://linkip.adguard-dns.com/linkip/YOUR_DEVICE_ID/YOUR_AUTH_TOKEN
   SECRET_KEY=your-secret-key-here
   ```

   See `.env.example` files in the project for templates.

2. **Install Dependencies**:
   ```bash
   # Install compiler dependencies
   cd AdGuard/Compiler
   npm install

   # Install website dependencies (optional)
   cd ../../www
   npm install

   # Restore .NET packages
   cd ../AdGuard/Source
   dotnet restore
   ```

### Usage

#### Compile Filter Rules
```bash
# Using TypeScript
cd AdGuard/Compiler
npm run build
node invoke-compiler.js

# Using PowerShell
cd AdGuard/Compiler
./invoke-compiler.ps1
```

#### Run C# Application
```bash
cd AdGuard/Source
dotnet run
```

#### Trigger Webhook
```powershell
cd AdGuard/Source/Scripts
./Webhook-Harness.ps1
```

## Project purpose
The internet is full of nuisances, and I've been on a quest to eradicate them from my network for decades.
1. Ads
2. Trackers
3. Malware
### How do I safeguard my network?
There are plenty of great apps that will help, but my experience is that there is no ==one size fits all== solution, especially for those of us on a domain, as well as those of us who have Internet of Things devices like:
- Echoes, HomePods, and other smart devices
- Smart TV's
- Anything else that doesn't have a UI within which to facilitate installation of blocking software
#### Sounds like a lot of work!
It is, which is why I've done all the hard work for you. So, how do I get to blocking ads? First, check out these links to see how you measure up:
- [AdBlock Tester](https://adblock-tester.com/)
- [AdGuard Tester](https://d3ward.github.io/toolz/adblock.html)

*Please note: [The permalink for the test link page](https://bit.ly/jaysonknight) should be used to quickly run tests.

The rules list [can be found here](/AdGuard/Rules/adguard_user_filter.txt).
