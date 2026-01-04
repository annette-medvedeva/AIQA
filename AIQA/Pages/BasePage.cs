
namespace AIQA.Pages
{
    /// <summary>
    /// Base page with common functionality for all application page objects
    /// Contains methods for element interaction and logging
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IPage _page;
        protected readonly ILogger _logger;

        /// <summary>
        /// Initialize base page with Playwright page instance and logger
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="logger">Logger for action recording</param>
        protected BasePage(IPage page, ILogger logger)
        {
            _page = page;
            _logger = logger;
        }

        /// <summary>
        /// Waits for page to load with specified URL with timeout
        /// </summary>
        /// <param name="url">URL to wait for loading</param>
        /// <param name="timeout">Wait timeout in milliseconds (default 30000)</param>
        protected async Task WaitForPageLoad(string url, float timeout = 30000)
        {
            _logger.LogInformation($"Waiting for page load: {url}");
            try
            {
                await _page.WaitForURLAsync(url, new PageWaitForURLOptions { Timeout = timeout });
                _logger.LogInformation($"Page successfully loaded: {url}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error waiting for page: {url}");
                throw;
            }
        }

        /// <summary>
        /// Waits for element to appear on page
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="description">Element description for logging</param>
        protected async Task WaitForElementVisible(ILocator locator, string description = "element")
        {
            _logger.LogInformation($"Waiting for element to appear: {description}");
            try
            {
                await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                _logger.LogInformation($"Element became visible: {description}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Element did not become visible: {description}");
                throw;
            }
        }

        /// <summary>
        /// Performs click on element with preliminary check
        /// </summary>
        /// <param name="locator">Locator of element to click</param>
        /// <param name="description">Element description for logging</param>
        protected async Task ClickElement(ILocator locator, string description)
        {
            _logger.LogInformation($"Performing click: {description}");
            try
            {
                await locator.ClickAsync();
                _logger.LogInformation($"Click successfully performed: {description}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during click: {description}");
                throw;
            }
        }

        /// <summary>
        /// Fills input field with preliminary clearing and logging
        /// </summary>
        /// <param name="locator">Input field locator</param>
        /// <param name="text">Text to input</param>
        /// <param name="description">Field description for logging</param>
        protected async Task FillField(ILocator locator, string text, string description)
        {
            _logger.LogInformation($"Filling field '{description}' with text: {text}");
            try
            {
                await locator.FillAsync(text);
                _logger.LogInformation($"Field '{description}' successfully filled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filling field '{description}'");
                throw;
            }
        }

        /// <summary>
        /// Gets text content of element
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="description">Element description for logging</param>
        /// <returns>Element text content</returns>
        protected async Task<string> GetElementText(ILocator locator, string description)
        {
            _logger.LogInformation($"Getting element text: {description}");
            try
            {
                var text = await locator.TextContentAsync() ?? string.Empty;
                _logger.LogInformation($"Retrieved text '{text}' from element: {description}");
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting element text: {description}");
                throw;
            }
        }

        /// <summary>
        /// Checks element visibility on page
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="description">Element description for logging</param>
        /// <returns>True if element is visible, otherwise False</returns>
        protected async Task<bool> IsElementVisible(ILocator locator, string description)
        {
            _logger.LogInformation($"Checking element visibility: {description}");
            try
            {
                var isVisible = await locator.IsVisibleAsync();
                _logger.LogInformation($"Element '{description}' visible: {isVisible}");
                return isVisible;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking element visibility: {description}");
                return false;
            }
        }

        /// <summary>
        /// Gets count of elements matching the locator
        /// </summary>
        /// <param name="locator">Elements locator</param>
        /// <param name="description">Elements description for logging</param>
        /// <returns>Number of found elements</returns>
        protected async Task<int> GetElementsCount(ILocator locator, string description)
        {
            _logger.LogInformation($"Counting elements: {description}");
            try
            {
                var count = await locator.CountAsync();
                _logger.LogInformation($"Found {count} elements: {description}");
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error counting elements: {description}");
                throw;
            }
        }
    }
}