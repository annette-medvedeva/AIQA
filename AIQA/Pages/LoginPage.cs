namespace AIQA.Pages
{
    /// <summary>
    /// Login page for Saucedemo application
    /// Contains methods for login functionality and authentication error handling
    /// </summary>
    public class LoginPage : BasePage
    {
        private readonly string _baseUrl = "https://www.saucedemo.com/";
        
        // Login page element locators
        private ILocator UsernameField => _page.Locator("[data-test='username']");
        private ILocator PasswordField => _page.Locator("[data-test='password']");
        private ILocator LoginButton => _page.Locator("[data-test='login-button']");
        private ILocator ErrorMessage => _page.Locator("[data-test='error']");
        private ILocator ErrorButton => _page.Locator(".error-button");

        /// <summary>
        /// Initialize login page
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="logger">Logger for action recording</param>
        public LoginPage(IPage page, ILogger logger) : base(page, logger)
        {
            _logger.LogInformation("Initializing login page");
        }

        /// <summary>
        /// Navigate to application login page
        /// </summary>
        public async Task NavigateToLoginPage()
        {
            _logger.LogInformation($"Navigating to login page: {_baseUrl}");
            try
            {
                await _page.GotoAsync(_baseUrl);
                await WaitForPageLoad(_baseUrl);
                await WaitForElementVisible(LoginButton, "login button");
                _logger.LogInformation("Successfully navigated to login page");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to login page");
                throw;
            }
        }

        /// <summary>
        /// Perform user authentication with credentials
        /// </summary>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">User password</param>
        public async Task LoginUser(string username, string password)
        {
            _logger.LogInformation($"Starting authentication process for user: {username}");
            try
            {
                await FillField(UsernameField, username, "username field");
                await FillField(PasswordField, password, "password field");
                await ClickElement(LoginButton, "login to system button");
                _logger.LogInformation($"Authentication process completed for user: {username}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during authentication for user: {username}");
                throw;
            }
        }

        /// <summary>
        /// Check if error message is present on the page
        /// </summary>
        /// <returns>True if error message is displayed, otherwise False</returns>
        public async Task<bool> IsErrorMessageDisplayed()
        {
            _logger.LogInformation("Checking for authentication error message presence");
            return await IsElementVisible(ErrorMessage, "error message");
        }

        /// <summary>
        /// Get authentication error message text
        /// </summary>
        /// <returns>Error message text</returns>
        public async Task<string> GetErrorMessage()
        {
            _logger.LogInformation("Getting error message text");
            return await GetElementText(ErrorMessage, "authentication error message");
        }

        /// <summary>
        /// Close error message by clicking X button
        /// </summary>
        public async Task CloseErrorMessage()
        {
            _logger.LogInformation("Closing error message");
            if (await IsElementVisible(ErrorButton, "error close button"))
            {
                await ClickElement(ErrorButton, "close error message button");
            }
        }

        /// <summary>
        /// Clear login form fields
        /// </summary>
        public async Task ClearLoginFields()
        {
            _logger.LogInformation("Clearing login fields");
            try
            {
                await FillField(UsernameField, "", "clear username field");
                await FillField(PasswordField, "", "clear password field");
                _logger.LogInformation("Login fields cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing login fields");
                throw;
            }
        }

        /// <summary>
        /// Check if login button is enabled
        /// </summary>
        /// <returns>True if login button is available for clicking</returns>
        public async Task<bool> IsLoginButtonEnabled()
        {
            _logger.LogInformation("Checking login button availability");
            try
            {
                var isEnabled = await LoginButton.IsEnabledAsync();
                _logger.LogInformation($"Login button enabled: {isEnabled}");
                return isEnabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking login button availability");
                return false;
            }
        }
    }
}