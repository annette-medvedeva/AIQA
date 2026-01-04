
namespace AIQA.Pages
{
    /// <summary>
    /// Cart page for Saucedemo application
    /// Contains methods for managing items in cart and cart operation workflow
    /// </summary>
    public class CartPage : BasePage
    {
        private readonly string _expectedUrl = "https://www.saucedemo.com/cart.html";
        
        // Cart page element locators
        private ILocator PageTitle => _page.Locator(".title");
        private ILocator CartItems => _page.Locator(".cart_item");
        private ILocator CheckoutButton => _page.Locator("[data-test='checkout']");
        private ILocator ContinueShoppingButton => _page.Locator("[data-test='continue-shopping']");
        private ILocator CartQuantityLabels => _page.Locator(".cart_quantity_label");
        private ILocator CartItemNames => _page.Locator(".inventory_item_name");
        private ILocator CartItemPrices => _page.Locator(".inventory_item_price");
        private ILocator RemoveButtons => _page.Locator("button[data-test*='remove']");

        /// <summary>
        /// Initialize cart page
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="logger">Logger for action recording</param>
        public CartPage(IPage page, ILogger logger) : base(page, logger)
        {
            _logger.LogInformation("Initializing cart page");
        }

        /// <summary>
        /// Check if cart page is loaded correctly
        /// </summary>
        /// <returns>True if cart page loaded successfully</returns>
        public async Task<bool> IsPageLoaded()
        {
            _logger.LogInformation("Checking cart page loading");
            try
            {
                await WaitForElementVisible(PageTitle, "cart page title");
                var currentUrl = _page.Url;
                var isLoaded = currentUrl.Contains("cart.html");
                _logger.LogInformation($"Cart page loaded: {isLoaded}, URL: {currentUrl}");
                return isLoaded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cart page loading");
                return false;
            }
        }

        /// <summary>
        /// Get number of items in cart
        /// </summary>
        /// <returns>Number of items in cart</returns>
        public async Task<int> GetCartItemsCount()
        {
            _logger.LogInformation("Counting items in cart");
            return await GetElementsCount(CartItems, "items in cart");
        }

        /// <summary>
        /// Remove item from cart by its index
        /// </summary>
        /// <param name="index">Item index in cart (starting from 0)</param>
        public async Task RemoveItemFromCart(int index)
        {
            _logger.LogInformation($"Removing item from cart by index: {index}");
            try
            {
                var cartItemsCount = await GetCartItemsCount();
                if (index >= cartItemsCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} exceeds number of items in cart {cartItemsCount}");
                }

                var removeButton = CartItems.Nth(index).Locator("button[data-test*='remove']");
                var productName = await CartItems.Nth(index).Locator(".inventory_item_name").TextContentAsync();
                
                await ClickElement(removeButton, $"remove product '{productName}' (index: {index})");
                _logger.LogInformation($"Product '{productName}' successfully removed from cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing item from cart by index: {index}");
                throw;
            }
        }

        /// <summary>
        /// Remove item from cart by its name
        /// </summary>
        /// <param name="productName">Product name for removal</param>
        public async Task RemoveItemFromCartByName(string productName)
        {
            _logger.LogInformation($"Removing item from cart by name: {productName}");
            try
            {
                var cartItem = _page.Locator($".cart_item:has(.inventory_item_name:has-text('{productName}'))");
                var removeButton = cartItem.Locator("button[data-test*='remove']");
                
                await ClickElement(removeButton, $"remove product '{productName}' by name");
                _logger.LogInformation($"Product '{productName}' successfully removed from cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing product '{productName}' from cart");
                throw;
            }
        }

        /// <summary>
        /// Navigate to checkout process
        /// </summary>
        public async Task ProceedToCheckout()
        {
            _logger.LogInformation("Proceeding to checkout process");
            try
            {
                await ClickElement(CheckoutButton, "checkout button");
                _logger.LogInformation("Successfully proceeded to checkout process");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proceeding to checkout process");
                throw;
            }
        }

        /// <summary>
        /// Continue shopping, return to products page
        /// </summary>
        public async Task ContinueShopping()
        {
            _logger.LogInformation("Continuing shopping");
            try
            {
                await ClickElement(ContinueShoppingButton, "continue shopping button");
                _logger.LogInformation("Successfully continuing shopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error continuing shopping");
                throw;
            }
        }

        /// <summary>
        /// Check if specific product is present in cart by its name
        /// </summary>
        /// <param name="productName">Product name to search</param>
        /// <returns>True if product is present in cart</returns>
        public async Task<bool> IsProductInCart(string productName)
        {
            _logger.LogInformation($"Checking product presence in cart: {productName}");
            try
            {
                var productInCart = _page.Locator($".cart_item .inventory_item_name:has-text('{productName}')");
                var isPresent = await IsElementVisible(productInCart, $"product '{productName}' in cart");
                return isPresent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking product '{productName}' presence in cart");
                return false;
            }
        }

        /// <summary>
        /// Get list of all product names in cart
        /// </summary>
        /// <returns>List of product names in cart</returns>
        public async Task<List<string>> GetCartItemNames()
        {
            _logger.LogInformation("Getting list of products in cart");
            try
            {
                var itemNames = new List<string>();
                var itemsCount = await GetCartItemsCount();
                
                for (int i = 0; i < itemsCount; i++)
                {
                    var itemName = await CartItems.Nth(i).Locator(".inventory_item_name").TextContentAsync();
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        itemNames.Add(itemName);
                    }
                }
                
                _logger.LogInformation($"Found {itemNames.Count} product names in cart: {string.Join(", ", itemNames)}");
                return itemNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting list of products in cart");
                throw;
            }
        }

        /// <summary>
        /// Get detailed information about product in cart by index
        /// </summary>
        /// <param name="index">Product index in cart</param>
        /// <returns>Tuple with name and price of product</returns>
        public async Task<(string name, string price)> GetCartItemInfo(int index)
        {
            _logger.LogInformation($"Getting product information in cart by index: {index}");
            try
            {
                var cartItem = CartItems.Nth(index);
                var name = await GetElementText(cartItem.Locator(".inventory_item_name"), $"product name {index}");
                var price = await GetElementText(cartItem.Locator(".inventory_item_price"), $"product price {index}");
                
                _logger.LogInformation($"Product info {index}: name='{name}', price='{price}'");
                return (name, price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product information by index: {index}");
                throw;
            }
        }

        /// <summary>
        /// Remove all products, clear entire cart
        /// </summary>
        public async Task ClearCart()
        {
            _logger.LogInformation("Clearing cart of all products");
            try
            {
                var itemsCount = await GetCartItemsCount();
                _logger.LogInformation($"Number of items to remove: {itemsCount}");
                
                // Remove items in reverse order to avoid index shifts during removal
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    await RemoveItemFromCart(i);
                    await Task.Delay(500); // Small delay between removals
                }
                
                _logger.LogInformation("Cart successfully cleared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                throw;
            }
        }

        /// <summary>
        /// Check if cart is empty
        /// </summary>
        /// <returns>True if cart is empty</returns>
        public async Task<bool> IsCartEmpty()
        {
            _logger.LogInformation("Checking if cart is empty");
            try
            {
                var itemsCount = await GetCartItemsCount();
                var isEmpty = itemsCount == 0;
                _logger.LogInformation($"Cart is empty: {isEmpty}");
                return isEmpty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cart is empty");
                return false;
            }
        }
    }
}