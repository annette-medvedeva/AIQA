using AIQA.TestData;
using Allure.Net.Commons;
using Allure.NUnit;

namespace AIQA.Tests
{
    /// <summary>
    /// Shopping cart functionality tests for Saucedemo application
    /// Validates cart management and checkout processes
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Shopping Cart Tests")]
    [Parallelizable(ParallelScope.Self)]
    public class CartTests : BaseTest
    {
        /// <summary>
        /// Performs authentication and cart preparation before each test
        /// </summary>
        [SetUp]
        public async Task SetupCartTests()
        {
            EnsureTestEnvironmentReady();
            await _loginPage.NavigateToLoginPage();
            await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);
            await _productsPage.IsPageLoaded();
        }

        /// <summary>
        /// Test viewing empty cart
        /// Validates correct display of empty cart state
        /// </summary>
        [Test]
        [AllureTag("cart", "empty", "ui")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Empty Cart View")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of empty cart display")]
        public async Task Test01_ViewEmptyCart_ShouldShowNoItems()
        {
            _logger.LogInformation("=== TEST 01: Empty cart view ===");

            try
            {
                // Action: Navigate to cart without adding products
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verification: Cart is empty
                var cartItemsCount = await _cartPage.GetCartItemsCount();
                Assert.That(cartItemsCount, Is.EqualTo(0), "Empty cart should contain 0 items");

                var isCartEmpty = await _cartPage.IsCartEmpty();
                Assert.That(isCartEmpty, Is.True, "Cart should be marked as empty");

                _logger.LogInformation("✓ Empty cart test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in empty cart test");
                throw;
            }
        }

        /// <summary>
        /// Test viewing cart with products
        /// Validates correct display of added products in cart
        /// </summary>
        [Test]
        [AllureTag("cart", "products", "viewing")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Cart with Products View")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of product display in cart")]
        public async Task Test02_ViewCartWithProducts_ShouldDisplayCorrectItems()
        {
            _logger.LogInformation("=== TEST 02: Cart with products view ===");

            try
            {
                // Setup: Add products to cart
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);
                await _productsPage.AddProductToCart(TestProducts.BikeLight);

                // Action: Navigate to cart
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verification: Number of items in cart
                var cartItemsCount = await _cartPage.GetCartItemsCount();
                Assert.That(cartItemsCount, Is.EqualTo(2), "Cart should contain 2 items");

                // Verification: Get cart items list
                var cartItemNames = await _cartPage.GetCartItemNames();
                Assert.That(cartItemNames.Count, Is.EqualTo(2), "Should get 2 product names");
                Assert.That(cartItemNames, Is.Not.Empty, "Product names list should not be empty");

                _logger.LogInformation($"Products in cart: {string.Join(", ", cartItemNames)}");
                _logger.LogInformation("✓ Cart with products view test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in cart with products view test");
                throw;
            }
        }

        /// <summary>
        /// Test removing product from cart
        /// Validates product removal functionality directly in cart
        /// </summary>
        [Test]
        [AllureTag("cart", "removal", "management")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Remove Product from Cart")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of product removal from cart")]
        public async Task Test03_RemoveProductFromCart_ShouldDecreaseItemCount()
        {
            _logger.LogInformation("=== TEST 03: Product removal from cart ===");

            try
            {
                // Setup: Add products to cart
                await _productsPage.AddProductToCart(TestProducts.FleeceJacket);
                await _productsPage.AddProductToCart(TestProducts.Onesie);

                // Navigate to cart
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verify initial state
                var initialCount = await _cartPage.GetCartItemsCount();
                Assert.That(initialCount, Is.EqualTo(2), "Should have 2 items in cart before removal");

                // Action: Remove one item by index
                await _cartPage.RemoveItemFromCart(0);

                // Verification: Decreased item count
                var countAfterRemoval = await _cartPage.GetCartItemsCount();
                Assert.That(countAfterRemoval, Is.EqualTo(1), "Should have 1 item remaining after removal");

                _logger.LogInformation("✓ Product removal from cart test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in product removal from cart test");
                throw;
            }
        }

        /// <summary>
        /// Test continue shopping from cart
        /// Validates navigation back to products page
        /// </summary>
        [Test]
        [AllureTag("cart", "navigation", "workflow")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Continue Shopping from Cart")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of continue shopping from cart")]
        public async Task Test04_ContinueShoppingFromCart_ShouldReturnToProducts()
        {
            _logger.LogInformation("=== TEST 04: Continue shopping from cart ===");

            try
            {
                // Setup: Add product and navigate to cart
                await _productsPage.AddProductToCart(TestProducts.RedTShirt);
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Action: Click continue shopping button
                await _cartPage.ContinueShopping();

                // Verification: Return to products page
                var isProductsPageLoaded = await _productsPage.IsPageLoaded();
                Assert.That(isProductsPageLoaded, Is.True, 
                    "Should return to products page after continue shopping");

                // Verification: Product remains in cart
                var cartCount = await _productsPage.GetCartItemsCount();
                Assert.That(cartCount, Is.EqualTo(1), 
                    "Product should remain in cart after returning to products page");

                _logger.LogInformation("✓ Continue shopping test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in continue shopping test");
                throw;
            }
        }

        /// <summary>
        /// Test clearing entire cart
        /// Validates ability to remove all items from cart
        /// </summary>
        [Test]
        [AllureTag("cart", "clearing", "management")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Clear Entire Cart")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of clearing entire cart")]
        public async Task Test05_ClearEntireCart_ShouldRemoveAllItems()
        {
            _logger.LogInformation("=== TEST 05: Clear entire cart ===");

            try
            {
                // Setup: Add multiple products to cart
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);
                await _productsPage.AddProductToCart(TestProducts.BikeLight);
                await _productsPage.AddProductToCart(TestProducts.BoltTShirt);

                // Navigate to cart
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verify initial state
                var initialCount = await _cartPage.GetCartItemsCount();
                Assert.That(initialCount, Is.EqualTo(3), "Should have 3 items in cart before clearing");

                // Action: Clear entire cart
                await _cartPage.ClearCart();

                // Verification: Cart is empty
                var finalCount = await _cartPage.GetCartItemsCount();
                Assert.That(finalCount, Is.EqualTo(0), "Cart should be empty after clearing");

                var isCartEmpty = await _cartPage.IsCartEmpty();
                Assert.That(isCartEmpty, Is.True, "Cart should be marked as empty");

                _logger.LogInformation("✓ Clear cart test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in clear cart test");
                throw;
            }
        }

        /// <summary>
        /// Test cart item information display
        /// Validates correct display of product names and prices
        /// </summary>
        [Test]
        [AllureTag("cart", "information", "validation")]
        [AllureFeature("Shopping Cart")]
        [AllureStory("Cart Item Information")]
        [AllureSeverity(SeverityLevel.minor)]
        [Description("Validation of correct product information display in cart")]
        public async Task Test06_CartItemInformation_ShouldDisplayCorrectDetails()
        {
            _logger.LogInformation("=== TEST 06: Cart item information ===");

            try
            {
                // Setup: Add product to cart
                await _productsPage.AddProductToCart(TestProducts.FleeceJacket);

                // Navigate to cart
                await _productsPage.GoToCart();
                await _cartPage.IsPageLoaded();

                // Verification: Get product information
                var (productName, productPrice) = await _cartPage.GetCartItemInfo(0);

                Assert.That(productName, Is.Not.Empty, "Product name should not be empty");
                Assert.That(productPrice, Is.Not.Empty, "Product price should not be empty");
                Assert.That(productPrice, Does.Contain("$"), "Price should contain currency symbol");

                _logger.LogInformation($"Product information in cart: {productName} - {productPrice}");
                _logger.LogInformation("✓ Cart item information test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in cart item information test");
                throw;
            }
        }
    }
}