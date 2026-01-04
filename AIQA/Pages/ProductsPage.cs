
namespace AIQA.Pages
{
    /// <summary>
    /// Products page for Saucedemo application
    /// Contains methods for working with product catalog, adding and sorting
    /// </summary>
    public class ProductsPage : BasePage
    {
        private readonly string _expectedUrl = "https://www.saucedemo.com/inventory.html";
        
        // Products page element locators
        private ILocator PageTitle => _page.Locator(".title");
        private ILocator CartBadge => _page.Locator(".shopping_cart_badge");
        private ILocator CartLink => _page.Locator(".shopping_cart_link");
        private ILocator SortDropdown => _page.Locator("[data-test='product_sort_container']");
        private ILocator ProductItems => _page.Locator(".inventory_item");
        private ILocator MenuButton => _page.Locator("#react-burger-menu-btn");
        private ILocator LogoutLink => _page.Locator("#logout_sidebar_link");
        private ILocator MenuCloseButton => _page.Locator("#react-burger-cross-btn");

        /// <summary>
        /// Initialize products page
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="logger">Logger for action recording</param>
        public ProductsPage(IPage page, ILogger logger) : base(page, logger)
        {
            _logger.LogInformation("Initializing products page");
        }

        /// <summary>
        /// Check if products page is loaded correctly
        /// </summary>
        /// <returns>True if products page loaded successfully</returns>
        public async Task<bool> IsPageLoaded()
        {
            _logger.LogInformation("Checking products page loading");
            try
            {
                await WaitForElementVisible(PageTitle, "products page title");
                var currentUrl = _page.Url;
                var isLoaded = currentUrl.Contains("inventory.html");
                _logger.LogInformation($"Products page loaded: {isLoaded}, URL: {currentUrl}");
                return isLoaded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking products page loading");
                return false;
            }
        }

        /// <summary>
        /// Add product to cart by its identifier (data-test attribute)
        /// </summary>
        /// <param name="productName">Product identifier for adding</param>
        public async Task AddProductToCart(string productName)
        {
            _logger.LogInformation($"Adding product to cart: {productName}");
            try
            {
                var addButton = _page.Locator($"[data-test='add-to-cart-{productName}']");
                await WaitForElementVisible(addButton, $"add product button {productName}");
                await ClickElement(addButton, $"add product '{productName}' to cart");
                _logger.LogInformation($"Product '{productName}' successfully added to cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product to cart: {productName}");
                throw;
            }
        }

        /// <summary>
        /// Remove product from cart by its identifier
        /// </summary>
        /// <param name="productName">Product identifier for removal</param>
        public async Task RemoveProductFromCart(string productName)
        {
            _logger.LogInformation($"Removing product from cart: {productName}");
            try
            {
                var removeButton = _page.Locator($"[data-test='remove-{productName}']");
                await WaitForElementVisible(removeButton, $"remove product button {productName}");
                await ClickElement(removeButton, $"remove product '{productName}' from cart");
                _logger.LogInformation($"Product '{productName}' successfully removed from cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing product from cart: {productName}");
                throw;
            }
        }

        /// <summary>
        /// Get current number of items in cart
        /// </summary>
        /// <returns>Number of items in cart (0 if cart is empty)</returns>
        public async Task<int> GetCartItemsCount()
        {
            _logger.LogInformation("Getting number of items in cart");
            try
            {
                if (!await IsElementVisible(CartBadge, "cart items counter"))
                {
                    _logger.LogInformation("Cart is empty (counter not visible)");
                    return 0;
                }
                
                var countText = await GetElementText(CartBadge, "cart items counter");
                var count = int.TryParse(countText, out var result) ? result : 0;
                _logger.LogInformation($"Number of items in cart: {count}");
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting number of items in cart");
                return 0;
            }
        }

        /// <summary>
        /// Navigate to cart page
        /// </summary>
        public async Task GoToCart()
        {
            _logger.LogInformation("Navigating to cart page");
            try
            {
                await ClickElement(CartLink, "cart link");
                _logger.LogInformation("Successfully navigated to cart page");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to cart page");
                throw;
            }
        }

        /// <summary>
        /// Sort products by specified criteria
        /// </summary>
        /// <param name="sortOption">Sort option (az, za, lohi, hilo)</param>
        public async Task SortProducts(string sortOption)
        {
            _logger.LogInformation($"Applying product sorting: {sortOption}");
            try
            {
                await SortDropdown.SelectOptionAsync(sortOption);
                await Task.Delay(1000); // Wait for sorting animation
                _logger.LogInformation($"Sorting '{sortOption}' successfully applied");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error applying sorting: {sortOption}");
                throw;
            }
        }

        /// <summary>
        /// Get total number of products on page
        /// </summary>
        /// <returns>Number of displayed products</returns>
        public async Task<int> GetProductsCount()
        {
            _logger.LogInformation("Counting products on page");
            return await GetElementsCount(ProductItems, "products on page");
        }

        /// <summary>
        /// Get product name by its position on page
        /// </summary>
        /// <param name="index">Product index (starting from 0)</param>
        /// <returns>Product name</returns>
        public async Task<string> GetProductName(int index)
        {
            _logger.LogInformation($"Getting product name by index: {index}");
            try
            {
                var productName = ProductItems.Nth(index).Locator(".inventory_item_name");
                var name = await GetElementText(productName, $"product name {index}");
                return name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product name by index: {index}");
                throw;
            }
        }

        /// <summary>
        /// Get product price by its position on page
        /// </summary>
        /// <param name="index">Product index (starting from 0)</param>
        /// <returns>Product price as string</returns>
        public async Task<string> GetProductPrice(int index)
        {
            _logger.LogInformation($"Getting product price by index: {index}");
            try
            {
                var productPrice = ProductItems.Nth(index).Locator(".inventory_item_price");
                var price = await GetElementText(productPrice, $"product price {index}");
                return price;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product price by index: {index}");
                throw;
            }
        }

        /// <summary>
        /// Open application main menu
        /// </summary>
        public async Task OpenMenu()
        {
            _logger.LogInformation("Opening main menu");
            try
            {
                await ClickElement(MenuButton, "main menu button");
                await WaitForElementVisible(LogoutLink, "logout button");
                _logger.LogInformation("Main menu successfully opened");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening main menu");
                throw;
            }
        }

        /// <summary>
        /// Close application main menu
        /// </summary>
        public async Task CloseMenu()
        {
            _logger.LogInformation("Closing main menu");
            try
            {
                if (await IsElementVisible(MenuCloseButton, "menu close button"))
                {
                    await ClickElement(MenuCloseButton, "menu close button");
                    _logger.LogInformation("Main menu successfully closed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing main menu");
                throw;
            }
        }

        /// <summary>
        /// Perform user logout from application
        /// </summary>
        public async Task Logout()
        {
            _logger.LogInformation("Performing logout from application");
            try
            {
                await OpenMenu();
                await ClickElement(LogoutLink, "logout button");
                _logger.LogInformation("Logout from application completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout from application");
                throw;
            }
        }
    }
}