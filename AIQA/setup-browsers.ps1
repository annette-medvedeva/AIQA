#!/usr/bin/env pwsh

# Install Playwright Browsers Script
# This script installs all required browsers for Playwright testing

param(
    [Parameter(HelpMessage="Force reinstall browsers")]
    [switch]$Force,
    
    [Parameter(HelpMessage="Install system dependencies")]
    [switch]$WithDeps,
    
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

function Invoke-PlaywrightScript {
    param($ScriptPath, $Arguments)
    
    # Try different PowerShell execution methods
    $methods = @()
    
    if (Test-PowerShellCommand "pwsh") {
        $methods += @{ Name = "PowerShell Core"; Command = "pwsh"; Args = @($ScriptPath) + $Arguments }
    }
    
    if (Test-PowerShellCommand "powershell") {
        $methods += @{ Name = "Windows PowerShell"; Command = "powershell"; Args = @("-ExecutionPolicy", "Bypass", "-File", $ScriptPath) + $Arguments }
    }
    
    # Try direct execution as last resort
    if (Test-Path $ScriptPath) {
        $methods += @{ Name = "Direct execution"; Command = $ScriptPath; Args = $Arguments }
    }
    
    foreach ($method in $methods) {
        if ($Verbose) {
            Write-ColorOutput "Trying $($method.Name): $($method.Command) $($method.Args -join ' ')" "Gray"
        }
        
        try {
            if ($method.Args.Count -gt 0) {
                & $method.Command @($method.Args)
            } else {
                & $method.Command
            }
            
            if ($LASTEXITCODE -eq 0) {
                if ($Verbose) {
                    Write-ColorOutput "? Success with $($method.Name)" "Green"
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

Write-ColorOutput "=== Playwright Browser Installation Script ===" "Green"

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version 2>$null
    if (-not $dotnetVersion) {
        throw "dotnet command not found"
    }
    Write-ColorOutput "? Found .NET SDK version: $dotnetVersion" "Green"
}
catch {
    Write-ColorOutput "? .NET SDK is not installed or not found in PATH" "Red"
    Write-ColorOutput "Please install .NET 8 SDK from: https://dotnet.microsoft.com/download" "Yellow"
    exit 1
}

# Check PowerShell availability
$powershellAvailable = $false
if (Test-PowerShellCommand "pwsh") {
    Write-ColorOutput "? PowerShell Core (pwsh) detected" "Green"
    $powershellAvailable = $true
} elseif (Test-PowerShellCommand "powershell") {
    Write-ColorOutput "? Windows PowerShell detected" "Green"
    $powershellAvailable = $true
} else {
    Write-ColorOutput "?? No PowerShell executable detected, will try direct execution" "Yellow"
}

# Clean previous build if Force is specified
if ($Force) {
    Write-ColorOutput "?? Cleaning previous builds..." "Yellow"
    try {
        dotnet clean 2>$null
        if (Test-Path "bin") {
            Remove-Item -Recurse -Force "bin" -ErrorAction SilentlyContinue
        }
        if (Test-Path "obj") {
            Remove-Item -Recurse -Force "obj" -ErrorAction SilentlyContinue
        }
        Write-ColorOutput "? Previous builds cleaned" "Green"
    }
    catch {
        Write-ColorOutput "?? Could not clean previous builds" "Yellow"
    }
}

# Build the project first
Write-ColorOutput "?? Building the project..." "Cyan"
try {
    if ($Verbose) {
        dotnet build --verbosity detailed
    } else {
        dotnet build --verbosity minimal
    }
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-ColorOutput "? Project built successfully" "Green"
}
catch {
    Write-ColorOutput "? Project build failed: $_" "Red"
    Write-ColorOutput "?? Try running: dotnet restore" "Yellow"
    exit 1
}

# Find Playwright executable
$playwrightScript = $null
$possiblePaths = @(
    "bin\Debug\net8.0\playwright.ps1",
    "bin\Release\net8.0\playwright.ps1",
    "AIQA\bin\Debug\net8.0\playwright.ps1",
    "AIQA\bin\Release\net8.0\playwright.ps1"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $playwrightScript = $path
        break
    }
}

if (-not $playwrightScript) {
    Write-ColorOutput "? Playwright script not found in expected locations" "Red"
    Write-ColorOutput "Searched paths:" "Yellow"
    foreach ($path in $possiblePaths) {
        Write-ColorOutput "  - $path" "Gray"
    }
    
    # Try alternative approach with global tool
    Write-ColorOutput "?? Trying alternative installation with global tool..." "Yellow"
    try {
        dotnet tool install --global Microsoft.Playwright.CLI --ignore-failed-sources 2>$null
        playwright install
        
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "? Browsers installed using global Playwright tool" "Green"
            Write-ColorOutput "?? Installation completed successfully!" "Green"
            exit 0
        } else {
            throw "Global tool installation failed"
        }
    } catch {
        Write-ColorOutput "? Alternative installation also failed" "Red"
        Write-ColorOutput "?? Manual steps to try:" "Yellow"
        Write-ColorOutput "   1. dotnet build" "Gray"
        Write-ColorOutput "   2. Check if bin/Debug/net8.0/playwright.ps1 exists" "Gray"
        Write-ColorOutput "   3. Run: powershell -File bin/Debug/net8.0/playwright.ps1 install" "Gray"
        exit 1
    }
}

Write-ColorOutput "? Found Playwright script at: $playwrightScript" "Green"

# Install Playwright browsers
Write-ColorOutput "?? Installing Playwright browsers..." "Cyan"

try {
    $installArgs = @("install")
    
    if ($Force) {
        $installArgs += "--force"
    }
    
    if ($Verbose) {
        Write-ColorOutput "Executing Playwright install command..." "Gray"
    }
    
    $success = Invoke-PlaywrightScript $playwrightScript $installArgs
    
    if ($success) {
        Write-ColorOutput "? Playwright browsers installed successfully" "Green"
    } else {
        throw "All Playwright execution methods failed"
    }
}
catch {
    Write-ColorOutput "? Failed to install Playwright browsers: $_" "Red"
    
    # Try alternative approaches
    Write-ColorOutput "?? Trying alternative installation methods..." "Yellow"
    
    $alternativeMethods = @(
        @{ Name = "Global tool"; Command = { dotnet tool install --global Microsoft.Playwright.CLI 2>$null; playwright install } },
        @{ Name = "Direct dotnet"; Command = { dotnet exec $playwrightScript install } },
        @{ Name = "NPM Playwright"; Command = { npm install -g playwright 2>$null; npx playwright install } }
    )
    
    $success = $false
    foreach ($method in $alternativeMethods) {
        try {
            Write-ColorOutput "Trying $($method.Name)..." "Yellow"
            & $method.Command
            
            if ($LASTEXITCODE -eq 0) {
                Write-ColorOutput "? Success with $($method.Name)" "Green"
                $success = $true
                break
            }
        } catch {
            if ($Verbose) {
                Write-ColorOutput "Failed with $($method.Name): $_" "Gray"
            }
        }
    }
    
    if (-not $success) {
        Write-ColorOutput "? All installation methods failed" "Red"
        Write-ColorOutput "?? Manual troubleshooting:" "Yellow"
        Write-ColorOutput "   • Ensure you have proper permissions" "Gray"
        Write-ColorOutput "   • Try running as Administrator" "Gray"
        Write-ColorOutput "   • Check your internet connection" "Gray"
        Write-ColorOutput "   • Visit: https://playwright.dev/dotnet/docs/installation" "Gray"
        exit 1
    }
}

# Install system dependencies if requested
if ($WithDeps) {
    Write-ColorOutput "?? Installing system dependencies..." "Cyan"
    try {
        $success = Invoke-PlaywrightScript $playwrightScript @("install-deps")
        
        if ($success) {
            Write-ColorOutput "? System dependencies installed" "Green"
        } else {
            Write-ColorOutput "?? System dependencies installation had issues" "Yellow"
            Write-ColorOutput "?? This is usually not critical on Windows" "Gray"
        }
    }
    catch {
        Write-ColorOutput "?? Could not install system dependencies: $_" "Yellow"
        Write-ColorOutput "?? This is usually not critical on Windows" "Gray"
    }
}

# Verify installation
Write-ColorOutput "?? Verifying installation..." "Cyan"
try {
    $success = Invoke-PlaywrightScript $playwrightScript @("show")
    if ($success) {
        Write-ColorOutput "? Playwright installation verified" "Green"
    } else {
        Write-ColorOutput "?? Could not verify installation, but browsers should be available" "Yellow"
    }
}
catch {
    Write-ColorOutput "?? Could not verify installation: $_" "Yellow"
    Write-ColorOutput "?? Try running a test to check if browsers work" "Gray"
}

# Test with a simple build
Write-ColorOutput "?? Testing project compilation..." "Cyan"
try {
    dotnet build --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "? Project compiles successfully" "Green"
    } else {
        Write-ColorOutput "?? Project compilation has issues" "Yellow"
    }
}
catch {
    Write-ColorOutput "?? Could not test compilation: $_" "Yellow"
}

Write-ColorOutput ""
Write-ColorOutput "?? Setup completed!" "Green"
Write-ColorOutput "?? Next steps:" "Cyan"
Write-ColorOutput "   • Run tests: dotnet test" "Gray"
Write-ColorOutput "   • Run with script: ./run-tests.ps1" "Gray"
Write-ColorOutput "   • Run specific tests: dotnet test --filter 'Category=smoke'" "Gray"
Write-ColorOutput "   • Troubleshoot issues: ./troubleshoot.ps1" "Gray"
Write-ColorOutput ""
Write-ColorOutput "?? For more options: Get-Help ./setup-browsers.ps1 -Full" "Gray"