using AIQA.Configuration;
using AIQA.Pages;
using Serilog;
using Allure.Net.Commons;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AIQA.Tests
{
    /// <summary>
    /// Base test class for all Saucedemo tests
    /// Contains common browser setup, logging and object initialization
    /// </summary>
    public abstract class BaseTest
    {
        protected IPlaywright? _playwright;
        protected IBrowser? _browser;
        protected IBrowserContext? _context;
        protected IPage? _page;
        protected ILogger _logger = null!;
        protected BrowserConfiguration _browserConfig = null!;
        private ILoggerFactory? _loggerFactory;
        
        // Page Objects
        protected LoginPage _loginPage = null!;
        protected ProductsPage _productsPage = null!;
        protected CartPage _cartPage = null!;

        /// <summary>
        /// Initialize browser environment before each test
        /// Sets up logging, browser and creates necessary objects
        /// </summary>
        [SetUp]
        public async Task BaseSetUp()
        {
            // Configure logging with Serilog
            ConfigureLogging();
            
            _logger.LogInformation("=== TEST INITIALIZATION START ===");
            _logger.LogInformation($"Starting test: {TestContext.CurrentContext.Test.Name}");

            try
            {
                // Initialize browser configuration
                _browserConfig = new BrowserConfiguration(_logger);

                // Create Playwright
                _playwright = await Playwright.CreateAsync();
                _logger.LogInformation("Playwright successfully initialized");

                // Get browser settings from environment variables or default values
                var browserType = Environment.GetEnvironmentVariable("BROWSER") ?? "chromium";
                var headless = bool.Parse(Environment.GetEnvironmentVariable("HEADLESS") ?? "false");

                // Create browser
                _browser = await _browserConfig.CreateBrowser(_playwright, browserType, headless);

                // Create browser context
                _context = await _browserConfig.CreateBrowserContext(_browser);

                // Create page
                _page = await _browserConfig.CreatePage(_context);

                // Initialize Page Objects
                InitializePageObjects();

                _logger.LogInformation("=== TEST INITIALIZATION COMPLETED ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during test initialization");
                await CleanUp();
                throw;
            }
        }

        /// <summary>
        /// Final cleanup after each test
        /// Closes browser, saves logs and creates screenshots
        /// </summary>
        [TearDown]
        public async Task BaseTearDown()
        {
            var testResult = TestContext.CurrentContext.Result.Outcome;
            _logger.LogInformation($"=== TEST COMPLETION: {TestContext.CurrentContext.Test.Name} ===");
            _logger.LogInformation($"Test result: {testResult}");

            if (testResult == NUnit.Framework.Interfaces.ResultState.Failure || 
                testResult == NUnit.Framework.Interfaces.ResultState.Error)
            {
                _logger.LogError($"Test completed with error: {TestContext.CurrentContext.Result.Message}");
                
                // Create screenshot on failure
                await TakeScreenshotOnFailure();
            }

            await CleanUp();
            _logger.LogInformation("=== RESOURCE CLEANUP COMPLETED ===");
            
            // Free logging resources
            _loggerFactory?.Dispose();
        }

        /// <summary>
        /// Configure logging system using Serilog
        /// </summary>
        private void ConfigureLogging()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: 
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/test-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Create Microsoft.Extensions.Logging factory
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
            
            _logger = _loggerFactory.CreateLogger<BaseTest>();
        }

        /// <summary>
        /// Initialize all Page Object classes
        /// </summary>
        private void InitializePageObjects()
        {
            _logger.LogInformation("Initializing Page Objects");
            
            if (_page == null)
                throw new InvalidOperationException("Page was not created");

            _loginPage = new LoginPage(_page, _logger);
            _productsPage = new ProductsPage(_page, _logger);
            _cartPage = new CartPage(_page, _logger);
            
            _logger.LogInformation("All Page Objects successfully initialized");
        }

        /// <summary>
        /// Create screenshot on test failure
        /// </summary>
        private async Task TakeScreenshotOnFailure()
        {
            try
            {
                if (_page != null)
                {
                    var testName = TestContext.CurrentContext.Test.Name;
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    var screenshotPath = $"test-results/screenshots/{testName}_{timestamp}.png";
                    
                    // Create directory if not exists
                    var directory = Path.GetDirectoryName(screenshotPath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var screenshotBytes = await _page.ScreenshotAsync(new PageScreenshotOptions 
                    { 
                        Path = screenshotPath,
                        FullPage = true 
                    });
                    
                    _logger.LogInformation($"Screenshot saved: {screenshotPath}");
                    
                    // Add screenshot to Allure report
                    try 
                    {
                        AllureApi.AddAttachment($"Screenshot_{testName}", "image/png", screenshotBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to add screenshot to Allure report");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating screenshot");
            }
        }

        /// <summary>
        /// Clean up all browser resources
        /// </summary>
        private async Task CleanUp()
        {
            try
            {
                if (_context != null)
                {
                    await _browserConfig.CloseContext(_context);
                    _context = null;
                }

                if (_browser != null)
                {
                    await _browserConfig.CloseBrowser(_browser);
                    _browser = null;
                }

                _playwright?.Dispose();
                _playwright = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resource cleanup");
            }
        }

        /// <summary>
        /// Verify test environment readiness
        /// </summary>
        protected void EnsureTestEnvironmentReady()
        {
            if (_page == null)
                throw new InvalidOperationException("Page is not initialized");
            if (_logger == null)
                throw new InvalidOperationException("Logger is not initialized");
            if (_loginPage == null || _productsPage == null || _cartPage == null)
                throw new InvalidOperationException("Page Objects are not initialized");
        }

        /// <summary>
        /// Skip test for technical and external reasons
        /// </summary>
        /// <param name="reason">Reason for test skip</param>
        protected void SkipTest(string reason)
        {
            _logger.LogWarning($"Test skipped: {reason}");
            Assert.Ignore(reason);
        }
    }
}