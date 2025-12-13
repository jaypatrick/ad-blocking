@echo off
REM compile-rules.cmd - Windows batch wrapper for the filter rules compiler
REM
REM This script provides a simple wrapper for Windows users who prefer
REM batch files. It delegates to the PowerShell Core script or falls
REM back to npx if PowerShell Core is not available.
REM
REM Usage:
REM   compile-rules.cmd [OPTIONS]
REM
REM Author: jaypatrick
REM License: GPLv3

setlocal enabledelayedexpansion

REM Script directory
set "SCRIPT_DIR=%~dp0"
set "PROJECT_ROOT=%SCRIPT_DIR%..\.."

REM Check for PowerShell Core (pwsh)
where pwsh >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo Using PowerShell Core...
    pwsh -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%compile-rules.ps1" %*
    exit /b %ERRORLEVEL%
)

REM Check for Windows PowerShell (fallback)
where powershell >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo Using Windows PowerShell...
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%compile-rules.ps1" %*
    exit /b %ERRORLEVEL%
)

REM Fallback to direct npx execution
echo PowerShell not found, using npx directly...

REM Handle version flag
if "%1"=="-v" goto :version
if "%1"=="--version" goto :version
if "%1"=="-h" goto :help
if "%1"=="--help" goto :help

REM Default config path
set "CONFIG_PATH=%PROJECT_ROOT%\src\filter-compiler\compiler-config.json"
set "OUTPUT_PATH="
set "COPY_TO_RULES="

REM Parse arguments
:parse_args
if "%1"=="" goto :compile
if "%1"=="-c" (
    set "CONFIG_PATH=%2"
    shift
    shift
    goto :parse_args
)
if "%1"=="--config" (
    set "CONFIG_PATH=%2"
    shift
    shift
    goto :parse_args
)
if "%1"=="-o" (
    set "OUTPUT_PATH=%2"
    shift
    shift
    goto :parse_args
)
if "%1"=="--output" (
    set "OUTPUT_PATH=%2"
    shift
    shift
    goto :parse_args
)
if "%1"=="-r" (
    set "COPY_TO_RULES=1"
    shift
    goto :parse_args
)
if "%1"=="--copy-to-rules" (
    set "COPY_TO_RULES=1"
    shift
    goto :parse_args
)
shift
goto :parse_args

:compile
REM Generate default output path if not specified
if "%OUTPUT_PATH%"=="" (
    for /f "tokens=1-6 delims=/:. " %%a in ("%date% %time%") do (
        set "OUTPUT_PATH=%PROJECT_ROOT%\src\rules-compiler-typescript\output\compiled-%%c%%a%%b-%%d%%e%%f.txt"
    )
)

echo Compiling filter rules...
echo Config: %CONFIG_PATH%
echo Output: %OUTPUT_PATH%

REM Run hostlist-compiler via npx
where npx >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: npx not found. Please install Node.js.
    exit /b 1
)

npx @adguard/hostlist-compiler --config "%CONFIG_PATH%" --output "%OUTPUT_PATH%"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Compilation failed.
    exit /b 1
)

echo Compilation successful!

REM Copy to rules if requested
if defined COPY_TO_RULES (
    echo Copying to rules directory...
    copy /y "%OUTPUT_PATH%" "%PROJECT_ROOT%\rules\adguard_user_filter.txt"
    if %ERRORLEVEL% equ 0 (
        echo Copied to: %PROJECT_ROOT%\rules\adguard_user_filter.txt
    )
)

echo Done!
exit /b 0

:version
echo AdGuard Filter Rules Compiler (Windows Batch Wrapper)
echo Version: 1.0.0
echo.
echo Platform: Windows
echo.
where node >nul 2>&1 && (
    for /f "tokens=*" %%i in ('node --version') do echo Node.js: %%i
) || (
    echo Node.js: Not found
)
where npx >nul 2>&1 && (
    echo hostlist-compiler: Available via npx
) || (
    echo hostlist-compiler: Not found
)
exit /b 0

:help
echo AdGuard Filter Rules Compiler (Windows Batch Wrapper)
echo.
echo Usage: compile-rules.cmd [OPTIONS]
echo.
echo Options:
echo   -c, --config PATH    Path to configuration file
echo   -o, --output PATH    Path to output file
echo   -r, --copy-to-rules  Copy output to rules directory
echo   -v, --version        Show version information
echo   -h, --help           Show this help message
echo.
echo Examples:
echo   compile-rules.cmd
echo   compile-rules.cmd -c config.json -r
echo   compile-rules.cmd --version
echo.
exit /b 0
