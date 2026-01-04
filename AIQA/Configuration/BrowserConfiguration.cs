using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace AIQA.Configuration
{
    /// <summary>
    /// Browser configuration for Playwright browser management
    /// Contains settings for browser creation and context management
    /// </summary>
    public class BrowserConfiguration
    {
        private readonly ILogger _logger;

        public BrowserConfiguration(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IBrowser> CreateBrowser(IPlaywright playwright, string browserType = "chromium", bool headless = false)
        {
            _logger.LogInformation($"Creating browser: {browserType}, headless: {headless}");
            
            try
            {
                var browserTypeLaunch = GetBrowserType(playwright, browserType);
                var launchOptions = GetBrowserLaunchOptions(headless);
                
                var browser = await browserTypeLaunch.LaunchAsync(launchOptions);
                _logger.LogInformation($"Browser {browserType} successfully created");
                
                return browser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating browser {browserType}");
                throw;
            }
        }

        public async Task<IBrowserContext> CreateBrowserContext(IBrowser browser, int viewportWidth = 1920, int viewportHeight = 1080)
        {
            _logger.LogInformation($"Creating browser context with viewport: {viewportWidth}x{viewportHeight}");
            
            try
            {
                var contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = viewportWidth, Height = viewportHeight },
                    IgnoreHTTPSErrors = true,
                    AcceptDownloads = true,
                    RecordVideoDir = "test-results/videos/",
                    RecordVideoSize = new RecordVideoSize { Width = viewportWidth, Height = viewportHeight }
                };

                var context = await browser.NewContextAsync(contextOptions);
                _logger.LogInformation("Browser context successfully created");
                
                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating browser context");
                throw;
            }
        }

        public async Task<IPage> CreatePage(IBrowserContext context)
        {
            _logger.LogInformation("Creating new browser page");
            
            try
            {
                var page = await context.NewPageAsync();
                
                // Configure timeouts
                page.SetDefaultTimeout(30000);
                page.SetDefaultNavigationTimeout(30000);
                
                _logger.LogInformation("Browser page successfully created");
                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating browser page");
                throw;
            }
        }

        private IBrowserType GetBrowserType(IPlaywright playwright, string browserType)
        {
            return browserType.ToLower() switch
            {
                "chromium" => playwright.Chromium,
                "firefox" => playwright.Firefox,
                "webkit" => playwright.Webkit,
                _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
            };
        }

        private BrowserTypeLaunchOptions GetBrowserLaunchOptions(bool headless)
        {
            return new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = 100, // Slow down automation for easier action observation
                Args = new[] 
                { 
                    "--start-maximized",
                    "--disable-blink-features=AutomationControlled",
                    "--disable-extensions"
                }
            };
        }

        public async Task CloseBrowser(IBrowser browser)
        {
            if (browser != null)
            {
                _logger.LogInformation("Closing browser");
                try
                {
                    await browser.CloseAsync();
                    _logger.LogInformation("Browser successfully closed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing browser");
                    throw;
                }
            }
        }

        public async Task CloseContext(IBrowserContext context)
        {
            if (context != null)
            {
                _logger.LogInformation("Closing browser context");
                try
                {
                    await context.CloseAsync();
                    _logger.LogInformation("Browser context successfully closed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing browser context");
                    throw;
                }
            }
        }
    }
}