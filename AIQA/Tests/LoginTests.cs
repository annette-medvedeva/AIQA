using AIQA.TestData;
using Allure.Net.Commons;
using Allure.NUnit;

namespace AIQA.Tests
{
    /// <summary>
    /// Authentication functionality tests for Saucedemo application
    /// Validates various login scenarios
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Authentication Tests")]
    [Parallelizable(ParallelScope.Self)]
    public class LoginTests : BaseTest
    {
        /// <summary>
        /// Test successful authentication of standard user
        /// Validates that user with correct credentials can login to system
        /// </summary>
        [Test]
        [AllureTag("authentication", "positive", "smoke")]
        [AllureFeature("User Authentication")]
        [AllureStory("Successful Login")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of successful standard user authentication")]
        public async Task Test01_SuccessfulLogin_StandardUser_ShouldRedirectToProducts()
        {
            // Setup
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== TEST 01: Successful standard user authentication ===");

            try
            {
                // Action: Navigate to login page
                await _loginPage.NavigateToLoginPage();

                // Action: Enter correct credentials and authenticate
                await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);

                // Verification: Successful redirect to products page
                var isProductsPageLoaded = await _productsPage.IsPageLoaded();
                Assert.That(isProductsPageLoaded, Is.True, 
                    "After successful authentication should redirect to products page");

                _logger.LogInformation("✓ Successful authentication test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in successful authentication test");
                throw;
            }
        }

        /// <summary>
        /// Test authentication of blocked user
        /// Validates that blocked user cannot login to system
        /// </summary>
        [Test]
        [AllureTag("authentication", "negative", "security")]
        [AllureFeature("User Authentication")]
        [AllureStory("Blocked User Login Attempt")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of login blocking for blocked user")]
        public async Task Test02_BlockedUserLogin_ShouldShowErrorMessage()
        {
            // Setup
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== TEST 02: Blocked user authentication ===");

            try
            {
                // Action: Navigate to login page
                await _loginPage.NavigateToLoginPage();

                // Action: Attempt authentication with blocked user
                await _loginPage.LoginUser(TestUsers.LockedUser.Username, TestUsers.LockedUser.Password);

                // Verification: Error message display
                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayed();
                Assert.That(isErrorDisplayed, Is.True, 
                    "Error message should be displayed for blocked user");

                // Verification: Message text contains blocking information
                var errorMessage = await _loginPage.GetErrorMessage();
                Assert.That(errorMessage.ToLower(), Does.Contain("locked out"), 
                    "Error message should contain information about user blocking");

                _logger.LogInformation("✓ User blocking test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in user blocking test");
                throw;
            }
        }

        /// <summary>
        /// Test authentication with invalid credentials
        /// Validates error handling for incorrect data input
        /// </summary>
        [Test]
        [AllureTag("authentication", "negative", "validation")]
        [AllureFeature("User Authentication")]
        [AllureStory("Invalid Credentials Login")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of invalid credentials handling")]
        public async Task Test03_InvalidCredentials_ShouldShowErrorMessage()
        {
            // Setup
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== TEST 03: Authentication with invalid credentials ===");

            try
            {
                // Action: Navigate to login page
                await _loginPage.NavigateToLoginPage();

                // Action: Enter invalid credentials
                await _loginPage.LoginUser(TestUsers.InvalidUser.Username, TestUsers.InvalidUser.Password);

                // Verification: Error message display
                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayed();
                Assert.That(isErrorDisplayed, Is.True, 
                    "Error message should be displayed for invalid credentials");

                // Verification: Message is not empty
                var errorMessage = await _loginPage.GetErrorMessage();
                Assert.That(errorMessage, Is.Not.Empty, 
                    "Error message should not be empty");

                _logger.LogInformation("✓ Invalid credentials test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in invalid credentials test");
                throw;
            }
        }

        /// <summary>
        /// Test authentication with empty fields
        /// Validates validation of required login form fields
        /// </summary>
        [Test]
        [AllureTag("authentication", "negative", "validation")]
        [AllureFeature("User Authentication")]
        [AllureStory("Empty Fields Validation")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of empty login fields validation")]
        public async Task Test04_EmptyCredentials_ShouldShowValidationError()
        {
            // Setup
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== TEST 04: Authentication with empty fields ===");

            try
            {
                // Action: Navigate to login page
                await _loginPage.NavigateToLoginPage();

                // Action: Attempt authentication with empty fields
                await _loginPage.LoginUser("", "");

                // Verification: Error message display
                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayed();
                Assert.That(isErrorDisplayed, Is.True, 
                    "Error message should be displayed for empty authentication fields");

                _logger.LogInformation("✓ Empty fields test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in empty fields test");
                throw;
            }
        }
    }
}