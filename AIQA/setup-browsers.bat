@echo off
REM AIQA Setup Script - Batch File Alternative
REM For users who prefer command prompt over PowerShell

setlocal enabledelayedexpansion

echo === AIQA Setup Script (Batch Version) ===

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo .NET SDK is not installed or not found in PATH
    echo Please install .NET 8 SDK from: https://dotnet.microsoft.com/download
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version 2^>nul') do set DOTNET_VERSION=%%i
echo Found .NET SDK version: !DOTNET_VERSION!

REM Build the project
echo Building the project...
dotnet build --verbosity minimal
if errorlevel 1 (
    echo Project build failed!
    echo Try running: dotnet restore
    exit /b 1
)
echo Project built successfully

REM Find Playwright script
set PLAYWRIGHT_SCRIPT=
if exist bin\Debug\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=bin\Debug\net8.0\playwright.ps1
) else if exist bin\Release\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=bin\Release\net8.0\playwright.ps1
) else if exist AIQA\bin\Debug\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=AIQA\bin\Debug\net8.0\playwright.ps1
) else if exist AIQA\bin\Release\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=AIQA\bin\Release\net8.0\playwright.ps1
)

if "!PLAYWRIGHT_SCRIPT!"=="" (
    echo Playwright script not found in expected locations
    echo Trying alternative installation method...
    
    REM Try global tool installation
    dotnet tool install --global Microsoft.Playwright.CLI --ignore-failed-sources >nul 2>&1
    if errorlevel 1 (
        echo Global tool installation failed
    ) else (
        echo Installing browsers with global tool...
        playwright install
        if errorlevel 1 (
            echo Browser installation failed
            goto :manual_steps
        ) else (
            echo Browsers installed successfully using global tool
            goto :success
        )
    )
    
    :manual_steps
    echo.
    echo Manual steps to try:
    echo   1. dotnet build
    echo   2. Check if bin\Debug\net8.0\playwright.ps1 exists
    echo   3. Run: powershell -ExecutionPolicy Bypass -File bin\Debug\net8.0\playwright.ps1 install
    echo   4. Or install Node.js and run: npm install -g playwright; npx playwright install
    exit /b 1
)

echo Found Playwright script at: !PLAYWRIGHT_SCRIPT!

REM Install Playwright browsers
echo Installing Playwright browsers...

REM Try PowerShell methods
set PS_SUCCESS=0

REM Method 1: Try pwsh (PowerShell Core)
where pwsh >nul 2>&1
if not errorlevel 1 (
    echo Trying PowerShell Core...
    pwsh "!PLAYWRIGHT_SCRIPT!" install
    if not errorlevel 1 set PS_SUCCESS=1
)

REM Method 2: Try powershell (Windows PowerShell)
if !PS_SUCCESS!==0 (
    where powershell >nul 2>&1
    if not errorlevel 1 (
        echo Trying Windows PowerShell...
        powershell -ExecutionPolicy Bypass -File "!PLAYWRIGHT_SCRIPT!" install
        if not errorlevel 1 set PS_SUCCESS=1
    )
)

REM Method 3: Try direct execution
if !PS_SUCCESS!==0 (
    echo Trying direct execution...
    "!PLAYWRIGHT_SCRIPT!" install
    if not errorlevel 1 set PS_SUCCESS=1
)

if !PS_SUCCESS!==0 (
    echo All PowerShell methods failed
    echo Trying alternative installation...
    
    REM Try global tool
    dotnet tool install --global Microsoft.Playwright.CLI >nul 2>&1
    playwright install
    if errorlevel 1 (
        echo Alternative installation also failed
        echo.
        echo Try these manual steps:
        echo   1. Install PowerShell Core from: https://github.com/PowerShell/PowerShell
        echo   2. Or run: powershell -ExecutionPolicy Bypass -File "!PLAYWRIGHT_SCRIPT!" install
        echo   3. Or install Node.js and run: npm install -g playwright; npx playwright install
        exit /b 1
    ) else (
        echo Browsers installed successfully using global tool
    )
) else (
    echo Playwright browsers installed successfully
)

REM Verify installation
echo Verifying installation...
if exist "!PLAYWRIGHT_SCRIPT!" (
    powershell -ExecutionPolicy Bypass -File "!PLAYWRIGHT_SCRIPT!" show >nul 2>&1
    if not errorlevel 1 (
        echo Playwright installation verified
    ) else (
        echo Could not verify installation, but browsers should be available
    )
) else (
    echo Verification skipped
)

REM Test compilation
echo Testing project compilation...
dotnet build --verbosity quiet >nul 2>&1
if not errorlevel 1 (
    echo Project compiles successfully
) else (
    echo Warning: Project compilation has issues
)

:success
echo.
echo Setup completed successfully!
echo.
echo Next steps:
echo   - Run tests: dotnet test
echo   - Run with batch: run-tests.bat
echo   - Run with PowerShell: run-tests.ps1
echo   - Run specific tests: dotnet test --filter "Category=smoke"
echo.
echo For troubleshooting, try: troubleshoot.ps1

exit /b 0