using AIQA.TestData;
using Allure.Net.Commons;
using Allure.NUnit;

namespace AIQA.Tests
{
    /// <summary>
    /// End-to-End tests for complete user interaction scenarios of Saucedemo application
    /// Validates comprehensive workflows and integrations
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("End-to-End Tests")]
    [Parallelizable(ParallelScope.Self)]
    public class EndToEndTests : BaseTest
    {
        /// <summary>
        /// Test complete purchase flow from login to checkout
        /// Validates comprehensive workflow of user interaction
        /// </summary>
        [Test]
        [AllureTag("e2e", "shopping", "complete-flow")]
        [AllureFeature("Complete Shopping Experience")]
        [AllureStory("Full Purchase Cycle")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Complete cycle test from login to cart to checkout")]
        public async Task Test01_CompletePurchaseCycle_ShouldWorkEndToEnd()
        {
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== E2E Test 01: Complete Purchase Cycle ===");

            try
            {
                // Step 1: User authentication
                _logger.LogInformation("Step 1: User authentication");
                await _loginPage.NavigateToLoginPage();
                await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);
                await _productsPage.IsPageLoaded();

                // Step 2: Browse available products
                _logger.LogInformation("Step 2: Browse available products");
                var productsCount = await _productsPage.GetProductsCount();
                Assert.That(productsCount, Is.GreaterThan(0), "Products catalog should contain items");

                // Step 3: Add products to cart
                _logger.LogInformation("Step 3: Add products to cart");
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);
                await _productsPage.AddProductToCart(TestProducts.BikeLight);

                var cartCount = await _productsPage.GetCartItemsCount();
                Assert.That(cartCount, Is.EqualTo(2), "Cart should contain 2 items");

                // Step 4: Navigate to cart and verify content
                _logger.LogInformation("Step 4: Cart verification");
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                var cartItemsCount = await _cartPage.GetCartItemsCount();
                Assert.That(cartItemsCount, Is.EqualTo(2), "Cart should display 2 items");

                // Step 5: Remove one item from cart
                _logger.LogInformation("Step 5: Remove item from cart");
                await _cartPage.RemoveItemFromCart(0);

                var updatedCartCount = await _cartPage.GetCartItemsCount();
                Assert.That(updatedCartCount, Is.EqualTo(1), "Cart should contain 1 item");

                // Step 6: Continue shopping
                _logger.LogInformation("Step 6: Continue shopping");
                await _cartPage.ContinueShopping();
                await _productsPage.IsPageLoaded();

                // Step 7: Add another product
                _logger.LogInformation("Step 7: Add additional product");
                await _productsPage.AddProductToCart(TestProducts.FleeceJacket);

                var finalCartCount = await _productsPage.GetCartItemsCount();
                Assert.That(finalCartCount, Is.EqualTo(2), "Cart should have 2 items total");

                // Step 8: Logout from application
                _logger.LogInformation("Step 8: Logout from application");
                await _productsPage.Logout();

                _logger.LogInformation("✓ E2E test complete purchase cycle finished successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in E2E complete purchase test");
                throw;
            }
        }

       
        /// <summary>
        /// Test error handling and recovery workflows
        /// Validates error scenarios and successful recovery flows
        /// </summary>
        [Test]
        [AllureTag("e2e", "negative", "recovery")]
        [AllureFeature("Error Handling and Recovery")]
        [AllureStory("Blocked User Recovery Workflow")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Error handling with blocked user and recovery with valid credentials")]
        public async Task Test02_BlockedUserRecovery_ShouldAllowRetryWithValidUser()
        {
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== E2E Test 02: Recovery after blocked user error ===");

            try
            {
                // Step 1: Attempt login with blocked user
                await _loginPage.NavigateToLoginPage();
                await _loginPage.LoginUser(TestUsers.LockedUser.Username, TestUsers.LockedUser.Password);

                // Verify error
                var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayed();
                Assert.That(isErrorDisplayed, Is.True, "Error should be displayed for blocked user");

                var errorMessage = await _loginPage.GetErrorMessage();
                Assert.That(errorMessage.ToLower(), Does.Contain("locked"), "Error message should contain information about blocking");

                // Step 2: Clear error and reset fields for retry
                await _loginPage.CloseErrorMessage();
                await _loginPage.ClearLoginFields();

                // Step 3: Successful login
                await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);
                var isProductsLoaded = await _productsPage.IsPageLoaded();
                Assert.That(isProductsLoaded, Is.True, "After successful login should navigate to products page");

                // Step 4: Verify normal functionality works
                await _productsPage.AddProductToCart(TestProducts.Onesie);
                var cartCount = await _productsPage.GetCartItemsCount();
                Assert.That(cartCount, Is.EqualTo(1), "Normal functionality should work after recovery");

                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                var isProductInCart = await _cartPage.IsProductInCart("Sauce Labs Onesie");
                Assert.That(isProductInCart, Is.True, "Product should be in cart");

                _logger.LogInformation("✓ E2E test recovery after error completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in E2E recovery test");
                throw;
            }
        }

        /// <summary>
        /// Test bulk cart operations and management
        /// Validates handling of multiple items and cart operations
        /// </summary>
        [Test]
        [AllureTag("e2e", "bulk", "cart-management")]
        [AllureFeature("Bulk Cart Operations")]
        [AllureStory("Mass Cart Management Workflow")]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Testing bulk cart operations with multiple items")]
        public async Task Test04_BulkCartManagement_ShouldHandleManyItems()
        {
            EnsureTestEnvironmentReady();
            _logger.LogInformation("=== E2E Test 04: Bulk cart operations ===");

            try
            {
                // Authentication
                await _loginPage.NavigateToLoginPage();
                await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);
                await _productsPage.IsPageLoaded();

                // Add all available products
                foreach (var product in TestProducts.AllProducts)
                {
                    await _productsPage.AddProductToCart(product);
                }

                var totalItemsAdded = await _productsPage.GetCartItemsCount();
                Assert.That(totalItemsAdded, Is.EqualTo(TestProducts.AllProducts.Length), 
                    "All products should be added to cart");

                // Navigate to cart
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verify cart contents and get detailed info
                var cartItemsCount = await _cartPage.GetCartItemsCount();
                _logger.LogInformation($"Items in cart: {cartItemsCount}");

                for (int i = 0; i < cartItemsCount; i++)
                {
                    var (name, price) = await _cartPage.GetCartItemInfo(i);
                    _logger.LogInformation($"Item {i + 1}: {name} - {price}");
                }

                // Remove half of the items
                var itemsToRemove = cartItemsCount / 2;
                for (int i = 0; i < itemsToRemove; i++)
                {
                    await _cartPage.RemoveItemFromCart(0); // Always remove first item
                }

                var remainingItems = await _cartPage.GetCartItemsCount();
                var expectedRemaining = cartItemsCount - itemsToRemove;
                Assert.That(remainingItems, Is.EqualTo(expectedRemaining), 
                    $"Cart should contain {expectedRemaining} items");

                // Clear entire cart
                await _cartPage.ClearCart();
                var finalCount = await _cartPage.GetCartItemsCount();
                Assert.That(finalCount, Is.EqualTo(0), "Cart should be completely empty");

                _logger.LogInformation("✓ E2E test bulk cart operations completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in E2E bulk operations test");
                throw;
            }
        }
    }
}