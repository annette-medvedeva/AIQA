#!/usr/bin/env pwsh

# Test Runner Script for AIQA Project
# Provides various options for running tests with different configurations

param(
    [Parameter(HelpMessage="Browser type: chromium, firefox, webkit")]
    [ValidateSet("chromium", "firefox", "webkit")]
    [string]$Browser = "chromium",
    
    [Parameter(HelpMessage="Run tests in headless mode")]
    [switch]$Headless,
    
    [Parameter(HelpMessage="Test filter pattern")]
    [string]$Filter = "",
    
    [Parameter(HelpMessage="Number of parallel workers")]
    [int]$Workers = 2,
    
    [Parameter(HelpMessage="Generate Allure report after tests")]
    [switch]$AllureReport,
    
    [Parameter(HelpMessage="Open test results in browser")]
    [switch]$OpenResults,
    
    [Parameter(HelpMessage="Skip browser verification")]
    [switch]$SkipBrowserCheck,
    
    [Parameter(HelpMessage="Verbose output")]
    [switch]$Verbose
)

function Write-ColorOutput {
    param($Message, $Color = "White")
    if ($Host.UI.SupportsVirtualTerminal -or $env:TERM -eq "xterm-256color") {
        Write-Host $Message -ForegroundColor $Color
    } else {
        Write-Host $Message
    }
}

function Test-PowerShellCommand {
    param($Command)
    try {
        $null = Get-Command $Command -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

function Invoke-PlaywrightCommand {
    param($Arguments)
    
    # Find Playwright script
    $playwrightPaths = @(
        "bin\Debug\net8.0\playwright.ps1",
        "bin\Release\net8.0\playwright.ps1",
        "AIQA\bin\Debug\net8.0\playwright.ps1",
        "AIQA\bin\Release\net8.0\playwright.ps1"
    )
    
    $playwrightScript = $null
    foreach ($path in $playwrightPaths) {
        if (Test-Path $path) {
            $playwrightScript = $path
            break
        }
    }
    
    if (-not $playwrightScript) {
        Write-ColorOutput "? Playwright script not found. Building project first..." "Red"
        dotnet build
        
        # Try again after build
        foreach ($path in $playwrightPaths) {
            if (Test-Path $path) {
                $playwrightScript = $path
                break
            }
        }
        
        if (-not $playwrightScript) {
            throw "Could not find Playwright script after build"
        }
    }
    
    # Try different PowerShell execution methods
    $methods = @()
    
    if (Test-PowerShellCommand "pwsh") {
        $methods += @{ Name = "PowerShell Core"; Command = "pwsh"; Args = @($playwrightScript) + $Arguments }
    }
    
    if (Test-PowerShellCommand "powershell") {
        $methods += @{ Name = "Windows PowerShell"; Command = "powershell"; Args = @("-File", $playwrightScript) + $Arguments }
    }
    
    # Fallback: direct execution
    $methods += @{ Name = "Direct execution"; Command = $playwrightScript; Args = $Arguments }
    
    foreach ($method in $methods) {
        if ($Verbose) {
            Write-ColorOutput "Trying $($method.Name): $($method.Command) $($method.Args -join ' ')" "Gray"
        }
        
        try {
            & $method.Command @($method.Args)
            if ($LASTEXITCODE -eq 0) {
                if ($Verbose) {
                    Write-ColorOutput "? Successfully executed using $($method.Name)" "Green"
                }
                return $true
            }
        } catch {
            if ($Verbose) {
                Write-ColorOutput "Failed with $($method.Name): $_" "Yellow"
            }
            continue
        }
    }
    
    return $false
}

Write-ColorOutput "=== AIQA Test Runner ===" "Green"
Write-ColorOutput "Browser: $Browser" "Cyan"
Write-ColorOutput "Headless: $($Headless.IsPresent)" "Cyan"
Write-ColorOutput "Workers: $Workers" "Cyan"

# Set environment variables
$env:BROWSER = $Browser
$env:HEADLESS = $Headless.IsPresent.ToString().ToLower()

# Clean previous results
if (Test-Path "TestResults") {
    Write-ColorOutput "?? Cleaning previous test results..." "Yellow"
    Remove-Item -Recurse -Force "TestResults" -ErrorAction SilentlyContinue
}

if (Test-Path "allure-results") {
    Write-ColorOutput "?? Cleaning previous allure results..." "Yellow"
    Remove-Item -Recurse -Force "allure-results" -ErrorAction SilentlyContinue
}

# Build the project
Write-ColorOutput "?? Building project..." "Cyan"
if ($Verbose) {
    dotnet build --verbosity detailed
} else {
    dotnet build --verbosity minimal
}

if ($LASTEXITCODE -ne 0) {
    Write-ColorOutput "? Build failed" "Red"
    exit 1
}

# Verify browsers are installed (unless skipped)
if (-not $SkipBrowserCheck) {
    Write-ColorOutput "?? Verifying browser installation..." "Cyan"
    
    try {
        $browserCheckResult = Invoke-PlaywrightCommand @("show")
        if (-not $browserCheckResult) {
            Write-ColorOutput "?? Browser verification failed. Installing browsers..." "Yellow"
            $installResult = Invoke-PlaywrightCommand @("install")
            if (-not $installResult) {
                Write-ColorOutput "? Failed to install browsers automatically" "Red"
                Write-ColorOutput "?? Try running: ./setup-browsers.ps1 -Force" "Yellow"
                Write-ColorOutput "?? Or skip browser check with: ./run-tests.ps1 -SkipBrowserCheck" "Yellow"
                exit 1
            } else {
                Write-ColorOutput "? Browsers installed successfully" "Green"
            }
        } else {
            Write-ColorOutput "? Browsers are available" "Green"
        }
    } catch {
        Write-ColorOutput "?? Could not verify browsers: $_" "Yellow"
        Write-ColorOutput "?? Continuing with test execution..." "Gray"
    }
}

# Prepare test command
$testCommand = "dotnet test --settings test.runsettings --logger console --logger trx"

if ($Filter) {
    $testCommand += " --filter `"$Filter`""
    Write-ColorOutput "Filter: $Filter" "Cyan"
}

# Add parallel workers
$testCommand += " -- NUnit.NumberOfTestWorkers=$Workers"

# Add verbosity if requested
if ($Verbose) {
    $testCommand += " --verbosity detailed"
}

# Run tests
Write-ColorOutput "?? Starting test execution..." "Green"
if ($Verbose) {
    Write-ColorOutput "Command: $testCommand" "Gray"
}

Invoke-Expression $testCommand
$testExitCode = $LASTEXITCODE

# Report results
if ($testExitCode -eq 0) {
    Write-ColorOutput "? All tests passed!" "Green"
} else {
    Write-ColorOutput "? Some tests failed (Exit Code: $testExitCode)" "Red"
}

# Generate Allure report if requested
if ($AllureReport) {
    Write-ColorOutput "?? Generating Allure report..." "Cyan"
    
    # Check if allure is installed
    $allureVersion = $null
    try {
        $allureVersion = allure --version 2>$null
    } catch {
        # Allure not found
    }
    
    if (-not $allureVersion) {
        Write-ColorOutput "? Allure is not installed." "Yellow"
        
        # Try to install via npm
        $npmVersion = $null
        try {
            $npmVersion = npm --version 2>$null
        } catch {
            # npm not found
        }
        
        if ($npmVersion) {
            Write-ColorOutput "?? Installing Allure via npm..." "Cyan"
            npm install -g allure-commandline
            
            # Check again
            try {
                $allureVersion = allure --version 2>$null
            } catch {
                # Still not working
            }
        } else {
            Write-ColorOutput "? npm not found. Please install Node.js and npm to use Allure reports." "Red"
            Write-ColorOutput "?? Download from: https://nodejs.org/" "Gray"
        }
    }
    
    if ($allureVersion) {
        if (Test-Path "allure-results") {
            try {
                allure generate allure-results -o allure-report --clean
                
                if ($OpenResults) {
                    Write-ColorOutput "?? Opening Allure report in browser..." "Cyan"
                    Start-Sleep -Seconds 2  # Give time for report generation
                    allure serve allure-results
                } else {
                    Write-ColorOutput "?? Allure report generated in: allure-report/" "Green"
                    Write-ColorOutput "?? To view: allure serve allure-results" "Gray"
                }
            } catch {
                Write-ColorOutput "? Failed to generate Allure report: $_" "Red"
            }
        } else {
            Write-ColorOutput "?? No allure results found. Make sure tests ran with Allure attributes." "Yellow"
        }
    } else {
        Write-ColorOutput "? Could not install Allure. Skipping report generation." "Red"
    }
}

# Summary
Write-ColorOutput "`n=== Test Execution Summary ===" "Green"
Write-ColorOutput "Browser: $Browser" "White"
Write-ColorOutput "Headless: $($Headless.IsPresent)" "White"
Write-ColorOutput "Workers: $Workers" "White"
Write-ColorOutput "Filter: $(if($Filter){"$Filter"}else{"None"})" "White"
Write-ColorOutput "Exit Code: $testExitCode" $(if ($testExitCode -eq 0) { "Green" } else { "Red" })

if ($testExitCode -ne 0) {
    Write-ColorOutput "`n?? Troubleshooting tips:" "Yellow"
    Write-ColorOutput "  • Run diagnostics: ./troubleshoot.ps1" "Gray"
    Write-ColorOutput "  • Check setup: ./setup-browsers.ps1 -Force" "Gray"
    Write-ColorOutput "  • View logs in: logs/ and TestResults/" "Gray"
    Write-ColorOutput "  • Run single test: dotnet test --filter 'Name~Login'" "Gray"
}

exit $testExitCode