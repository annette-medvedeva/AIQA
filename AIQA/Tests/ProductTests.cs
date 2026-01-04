using AIQA.TestData;
using Allure.Net.Commons;
using Allure.NUnit;


namespace AIQA.Tests
{
    /// <summary>
    /// Product management functionality tests for Saucedemo application
    /// Validates product catalog, cart addition and sorting features
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Product Management Tests")]
    [Parallelizable(ParallelScope.Self)]
    public class ProductTests : BaseTest
    {
        /// <summary>
        /// Performs authentication before each product test
        /// </summary>
        [SetUp]
        public async Task LoginBeforeProductTests()
        {
            EnsureTestEnvironmentReady();
            await _loginPage.NavigateToLoginPage();
            await _loginPage.LoginUser(TestUsers.StandardUser.Username, TestUsers.StandardUser.Password);
            await _productsPage.IsPageLoaded();
        }

        /// <summary>
        /// Test adding single product to cart
        /// Validates basic product addition functionality
        /// </summary>
        [Test]
        [AllureTag("products", "cart", "positive")]
        [AllureFeature("Product Management")]
        [AllureStory("Add Single Product to Cart")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of adding single product to cart")]
        public async Task Test01_AddSingleProduct_ShouldUpdateCartCounter()
        {
            _logger.LogInformation("=== TEST 01: Adding single product to cart ===");

            try
            {
                // Check initial cart state
                var initialCartCount = await _productsPage.GetCartItemsCount();
                Assert.That(initialCartCount, Is.EqualTo(0), "Cart should be empty at test start");

                // Action: Add product to cart
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);

                // Verification: Cart counter update
                var updatedCartCount = await _productsPage.GetCartItemsCount();
                Assert.That(updatedCartCount, Is.EqualTo(1), "Cart should contain 1 item after addition");

                _logger.LogInformation("✓ Single product addition test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in single product addition test");
                throw;
            }
        }

        /// <summary>
        /// Test adding multiple products to cart
        /// Validates cumulative product addition
        /// </summary>
        [Test]
        [AllureTag("products", "cart", "positive")]
        [AllureFeature("Product Management")]
        [AllureStory("Add Multiple Products to Cart")]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Validation of adding multiple products to cart")]
        public async Task Test02_AddMultipleProducts_ShouldAccumulateInCart()
        {
            _logger.LogInformation("=== TEST 02: Adding multiple products to cart ===");

            try
            {
                // Check initial state
                var initialCartCount = await _productsPage.GetCartItemsCount();
                Assert.That(initialCartCount, Is.EqualTo(0), "Cart should be empty at start");

                // Action: Add multiple products
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);
                await _productsPage.AddProductToCart(TestProducts.BikeLight);
                await _productsPage.AddProductToCart(TestProducts.BoltTShirt);

                // Verification: Correct number of products in cart
                var finalCartCount = await _productsPage.GetCartItemsCount();
                Assert.That(finalCartCount, Is.EqualTo(3), "Cart should contain 3 products");

                _logger.LogInformation("✓ Multiple products addition test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in multiple products addition test");
                throw;
            }
        }

        /// <summary>
        /// Test removing product from cart via products page
        /// Validates Remove button functionality
        /// </summary>
        [Test]
        [AllureTag("products", "cart", "removal")]
        [AllureFeature("Product Management")]
        [AllureStory("Remove Product from Products Page")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of product removal from products page")]
        public async Task Test03_RemoveProductFromProductsPage_ShouldDecreaseCartCount()
        {
            _logger.LogInformation("=== TEST 03: Product removal from products page ===");

            try
            {
                // Setup: Add products to cart
                await _productsPage.AddProductToCart(TestProducts.BackpackProduct);
                await _productsPage.AddProductToCart(TestProducts.FleeceJacket);

                var cartCountAfterAdding = await _productsPage.GetCartItemsCount();
                Assert.That(cartCountAfterAdding, Is.EqualTo(2), "Should have 2 products in cart after addition");

                // Action: Remove one product
                await _productsPage.RemoveProductFromCart(TestProducts.BackpackProduct);

                // Verification: Cart counter decrease
                var cartCountAfterRemoval = await _productsPage.GetCartItemsCount();
                Assert.That(cartCountAfterRemoval, Is.EqualTo(1), "Should have 1 product remaining in cart after removal");

                _logger.LogInformation("✓ Product removal test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in product removal test");
                throw;
            }
        }

        
        /// <summary>
        /// Test adding all available products to cart
        /// Validates maximum cart capacity handling
        /// </summary>
        [Test]
        [AllureTag("products", "cart", "stress")]
        [AllureFeature("Product Management")]
        [AllureStory("Add All Products to Cart")]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Validation of adding all available products to cart")]
        public async Task Test05_AddAllProducts_ShouldHandleMaximumCart()
        {
            _logger.LogInformation("=== TEST 05: Adding all products to cart ===");

            try
            {
                // Get total products count on page
                var totalProducts = await _productsPage.GetProductsCount();
                _logger.LogInformation($"Total products on page: {totalProducts}");

                // Add all available test products
                foreach (var product in TestProducts.AllProducts)
                {
                    await _productsPage.AddProductToCart(product);
                }

                // Verification: Cart count matches added products
                var cartCount = await _productsPage.GetCartItemsCount();
                Assert.That(cartCount, Is.EqualTo(TestProducts.AllProducts.Length), 
                    $"Cart should contain {TestProducts.AllProducts.Length} products");

                _logger.LogInformation("✓ All products addition test completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Error in all products addition test");
                throw;
            }
        }
    }
}