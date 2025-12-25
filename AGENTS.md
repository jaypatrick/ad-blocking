# Repository Guidelines

## Project Structure & Module Organization

- `rules/` contains the tracked filter list (`rules/adguard_user_filter.txt`) and compiler configuration files (`rules/Config/`).
- `src/` contains the multi-language toolchain:
  - `src/rules-compiler-*` (TypeScript, .NET, Python, Rust, shell) compilers that wrap `@adguard/hostlist-compiler`.
  - `src/adguard-api-dotnet/` and `src/adguard-api-rust/` SDKs + interactive clients for the AdGuard DNS API.
  - `src/adguard-api-powershell/` PowerShell modules and Pester tests.
  - `src/website/` Gatsby site sources and build output inputs.
- `docs/` holds guides and reference documentation.

## Build, Test, and Development Commands

- Compile rules (any platform): `./src/rules-compiler-shell/compile-rules.sh -c rules/Config/config.yaml -r` (see `src/rules-compiler-shell/`).
- TypeScript compiler (`src/rules-compiler-typescript/`):
  - `npm ci` / `npm install` — install deps
  - `npm run build` — `tsc` build to `dist/`
  - `npm run lint` — ESLint
  - `npm test` — Jest
- .NET (`src/rules-compiler-dotnet/`, `src/adguard-api-dotnet/`): `dotnet restore`, `dotnet build`, `dotnet test`
- Python (`src/rules-compiler-python/`): `pip install -e ".[dev]"`, `pytest`, `ruff check .`, `mypy .`
- Rust (`src/rules-compiler-rust/`, `src/adguard-api-rust/`): `cargo build`, `cargo test`, `cargo fmt`, `cargo clippy`
- Website (`src/website/`): `npm ci`, `npm run develop`, `npm run build`
- Docker dev env: `docker build -f Dockerfile.warp .` (use when you want a pre-baked toolchain).

## Coding Style & Naming Conventions

- Follow the conventions of each language and keep changes scoped to the module you’re touching.
- TypeScript: 2-space indentation, `eslint` enforced; tests use `*.test.ts`.
- .NET: match existing casing (PascalCase types/methods); prefer nullable-safe APIs; keep solutions in `.slnx`.
- Python: `ruff` (line length 100) + `mypy` (typed, strict-ish); tests use `tests/test_*.py`.
- PowerShell: use approved verbs and keep functions discoverable (`Verb-Noun`); PSScriptAnalyzer is run in CI.

## Testing Guidelines

- Add/adjust tests alongside changes (unit tests preferred; integration tests where appropriate).
- Run the closest test suite first (e.g., `npm test`, `dotnet test`, `pytest`, `cargo test`, `Invoke-Pester`).

## Commit & Pull Request Guidelines

- Prefer Conventional Commit style when practical (examples: `feat(rules-compiler-python): ...`, `docs(readme): ...`); short imperative messages like `Refactor: ...` are also used.
- PRs should include: a clear description, linked issue(s) when applicable, and test evidence (paste output or CI link). Include screenshots for website/UI changes.

## Security & Configuration Notes

- Follow `SECURITY.md` for vulnerability reporting.
- Secrets (e.g., AdGuard API key) must come from environment variables/config files and never be committed.

