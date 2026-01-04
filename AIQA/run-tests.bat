@echo off
REM AIQA Test Runner - Batch File Alternative
REM For users who prefer command prompt over PowerShell

setlocal enabledelayedexpansion

echo === AIQA Test Runner (Batch Version) ===

REM Set default values
set BROWSER=chromium
set HEADLESS=false
set WORKERS=2
set FILTER=

REM Parse command line arguments
:parse
if "%1"=="" goto endparse
if "%1"=="-browser" (
    set BROWSER=%2
    shift
    shift
    goto parse
)
if "%1"=="-headless" (
    set HEADLESS=true
    shift
    goto parse
)
if "%1"=="-workers" (
    set WORKERS=%2
    shift
    shift
    goto parse
)
if "%1"=="-filter" (
    set FILTER=%2
    shift
    shift
    goto parse
)
if "%1"=="-help" (
    goto showhelp
)
shift
goto parse
:endparse

echo Browser: !BROWSER!
echo Headless: !HEADLESS!
echo Workers: !WORKERS!
if not "!FILTER!"=="" echo Filter: !FILTER!

REM Set environment variables
set BROWSER=!BROWSER!
set HEADLESS=!HEADLESS!

REM Clean previous results
if exist TestResults (
    echo Cleaning previous test results...
    rmdir /s /q TestResults 2>nul
)

if exist allure-results (
    echo Cleaning previous allure results...
    rmdir /s /q allure-results 2>nul
)

REM Build the project
echo Building project...
dotnet build --verbosity minimal
if errorlevel 1 (
    echo Build failed!
    exit /b 1
)

REM Check if browsers are installed
echo Checking browser installation...
set PLAYWRIGHT_SCRIPT=
if exist bin\Debug\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=bin\Debug\net8.0\playwright.ps1
) else if exist bin\Release\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=bin\Release\net8.0\playwright.ps1
) else if exist AIQA\bin\Debug\net8.0\playwright.ps1 (
    set PLAYWRIGHT_SCRIPT=AIQA\bin\Debug\net8.0\playwright.ps1
)

if not "!PLAYWRIGHT_SCRIPT!"=="" (
    echo Verifying browsers with PowerShell...
    powershell -ExecutionPolicy Bypass -File "!PLAYWRIGHT_SCRIPT!" show >nul 2>&1
    if errorlevel 1 (
        echo Installing browsers...
        powershell -ExecutionPolicy Bypass -File "!PLAYWRIGHT_SCRIPT!" install
        if errorlevel 1 (
            echo Warning: Could not install browsers automatically
            echo Try running: setup-browsers.ps1
        )
    )
) else (
    echo Warning: Playwright script not found, assuming browsers are installed
)

REM Prepare test command
set TEST_CMD=dotnet test --settings test.runsettings --logger console --logger trx

if not "!FILTER!"=="" (
    set TEST_CMD=!TEST_CMD! --filter "!FILTER!"
)

set TEST_CMD=!TEST_CMD! -- NUnit.NumberOfTestWorkers=!WORKERS!

REM Run tests
echo Starting test execution...
echo Command: !TEST_CMD!
echo.

!TEST_CMD!
set TEST_EXIT_CODE=!ERRORLEVEL!

REM Report results
echo.
if !TEST_EXIT_CODE!==0 (
    echo All tests passed!
) else (
    echo Some tests failed ^(Exit Code: !TEST_EXIT_CODE!^)
    echo.
    echo Troubleshooting tips:
    echo   - Run diagnostics: troubleshoot.ps1
    echo   - Check setup: setup-browsers.ps1 -Force
    echo   - View logs in: logs\ and TestResults\
)

echo.
echo === Test Execution Summary ===
echo Browser: !BROWSER!
echo Headless: !HEADLESS!
echo Workers: !WORKERS!
if not "!FILTER!"=="" echo Filter: !FILTER!
echo Exit Code: !TEST_EXIT_CODE!

exit /b !TEST_EXIT_CODE!

:showhelp
echo Usage: run-tests.bat [options]
echo.
echo Options:
echo   -browser ^<type^>     Browser type: chromium, firefox, webkit (default: chromium)
echo   -headless            Run tests in headless mode
echo   -workers ^<number^>   Number of parallel workers (default: 2)
echo   -filter ^<pattern^>   Test filter pattern
echo   -help               Show this help message
echo.
echo Examples:
echo   run-tests.bat
echo   run-tests.bat -browser firefox -headless
echo   run-tests.bat -filter "Category=smoke" -workers 1
echo   run-tests.bat -filter "Name~Login"
echo.
echo For more advanced features, use: run-tests.ps1
exit /b 0