namespace AIQA.TestData
{
    /// <summary>
    /// Constants for test product data in Saucedemo application
    /// Contains product identifiers for automation testing
    /// </summary>
    public static class TestProducts
    {
        /// <summary>
        /// Product identifier for "Sauce Labs Backpack"
        /// </summary>
        public const string BackpackProduct = "sauce-labs-backpack";

        /// <summary>
        /// Product identifier for "Sauce Labs Bike Light"
        /// </summary>
        public const string BikeLight = "sauce-labs-bike-light";

        /// <summary>
        /// Product identifier for "Sauce Labs Bolt T-Shirt"
        /// </summary>
        public const string BoltTShirt = "sauce-labs-bolt-t-shirt";

        /// <summary>
        /// Product identifier for "Sauce Labs Fleece Jacket"
        /// </summary>
        public const string FleeceJacket = "sauce-labs-fleece-jacket";

        /// <summary>
        /// Product identifier for "Sauce Labs Onesie"
        /// </summary>
        public const string Onesie = "sauce-labs-onesie";

        /// <summary>
        /// Product identifier for "Test.allTheThings() T-Shirt (Red)"
        /// </summary>
        public const string RedTShirt = "test.allthethings()-t-shirt-(red)";

        /// <summary>
        /// Array of all available products for bulk testing with cart
        /// </summary>
        public static readonly string[] AllProducts = 
        {
            BackpackProduct,
            BikeLight,
            BoltTShirt,
            FleeceJacket,
            Onesie,
            RedTShirt
        };

        /// <summary>
        /// Product sorting options and filters
        /// </summary>
        public static class SortOptions
        {
            /// <summary>Sort by name from A to Z</summary>
            public const string NameAtoZ = "az";
            
            /// <summary>Sort by name from Z to A</summary>
            public const string NameZtoA = "za";
            
            /// <summary>Sort by price from low to high</summary>
            public const string PriceLowToHigh = "low";
            
            /// <summary>Sort by price from high to low</summary>
            public const string PriceHighToLow = "high";
        }
    }
}