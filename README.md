# AIQA - Automated Testing for Saucedemo

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Playwright](https://img.shields.io/badge/Playwright-Latest-green)](https://playwright.dev/)
[![NUnit](https://img.shields.io/badge/NUnit-4.x-orange)](https://nunit.org/)
[![Allure](https://img.shields.io/badge/Allure-2.x-yellow)](https://docs.qameta.io/allure/)

## Project Description

AIQA represents a comprehensive solution for automated testing of the **Saucedemo** web application using modern tools and approaches.

### Technology Stack

- **Programming Language:** C# 12.0
- **Framework:** .NET 8
- **Automation:** Microsoft Playwright
- **Testing Framework:** NUnit 4.x
- **Reporting:** Allure Framework
- **Logging:** Serilog
- **Architecture Pattern:** Page Object Model (POM)

### Implementation Features

#### 🔍 **Functionality Coverage**
- **User Authentication**
  - Valid login
  - Login error handling

- **Product Management**
  - Catalog browsing
  - Adding products to cart
  - Product removal
  - Sorting by various parameters

- **Shopping Cart**
  - Cart content viewing
  - Quantity and composition management
  - Cart clearing
  - Application navigation

- **Comprehensive E2E Scenarios**
  - Complete purchase cycles
  - Validation and verification with authentication
  - Negative testing

### Test Statistics

| Category | Test Count | Description |
|-----------|------------|-------------|
| **Authentication** | 4 tests | Validation and verification of login process |
| **Products** | 5 tests | Catalog work and cart management |
| **Cart** | 6 tests | Cart item management |
| **E2E** | 4 tests | Comprehensive user scenarios |
| **Total** | **19 tests** | Complete coverage of core functionality |

## Quick Start

### Prerequisites

```bash
# Required components
- .NET 8 SDK
- Visual Studio 2022 / VS Code / Rider
- Git
- PowerShell (for browser setup)
```

### Installation

1. **Clone Repository**
```bash
git clone https://github.com/annette-medvedeva/AIQA.git
cd AIQA
```

2. **Install Dependencies**
```bash
dotnet restore
```

3. **Setup Playwright Browsers**
```powershell
# On development machine
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### Running Tests

#### Basic Commands

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with settings
dotnet test --settings test.runsettings

# Run specific test group
dotnet test --filter "TestCategory=LoginTests"

# Run tests with specific tag
dotnet test --filter "Category=smoke"
```

#### Environment Configuration

```bash
# Browser settings
$env:BROWSER = "chromium"  # chromium, firefox, webkit
$env:HEADLESS = "false"    # true for headless mode

# Run tests with settings
dotnet test
```

#### Parallel Execution

```bash
# Run with parallel execution
dotnet test --parallel

# Run with thread limitation
dotnet test -- NUnit.NumberOfTestWorkers=2
```

## Working with Reporting

### Allure Reports

#### Report Generation
```bash
# Install Allure (if not installed)
npm install -g allure-commandline

# View and generate report
allure serve allure-results

# Generate static report
allure generate allure-results -o allure-report --clean
```

### Configuration

#### Settings Files
- `test.runsettings` - main NUnit settings
- `allureConfig.json` - Allure configuration
- `appsettings.test.json` - application settings

#### Environment Variables
```bash
BROWSER=chromium          # Browser type
HEADLESS=false           # Display mode
BASE_URL=https://...     # Application URL
VIEWPORT_WIDTH=1920      # Viewport width
VIEWPORT_HEIGHT=1080     # Viewport height
```

## Project Architecture

### Project Structure
```
AIQA/
├── Configuration/           # Browser and test configurations
├── Pages/                  # Page Object classes
├── Tests/                  # Test classes
├── TestData/              # Test data
├── Utilities/             # Helper classes
├── test-results/          # Test execution results
└── logs/                  # Log files
```

### Development Principles

1. **Page Object Model** - page element encapsulation
2. **Dependency Injection** - dependency management
3. **Logging** - detailed logging of all actions
4. **Reporting** - Allure Framework integration
5. **Configurability** - settings via files and variables

## Extension and Maintenance

### Adding New Tests

1. Create new class in `Tests` folder
2. Inherit from `BaseTest`
3. Use Page Object pattern for interaction
4. Add appropriate Allure attributes

### Adding New Pages

1. Create class in `Pages` folder
2. Inherit from `BasePage`
3. Define element locators
4. Implement interaction methods

## CI/CD Integration

### GitHub Actions
```yaml
- name: Run Tests
  run: dotnet test --logger trx --results-directory TestResults
  
- name: Generate Allure Report
  uses: simple-elf/allure-report-action@master
```

### Azure DevOps
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    arguments: '--logger trx --collect:"XPlat Code Coverage"'
```

## License

This project is intended for educational and demonstration purposes.

## Contact

For questions and suggestions about the project, create an issue in the repository or contact the author.
