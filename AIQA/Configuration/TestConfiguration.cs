using Microsoft.Extensions.Configuration;

namespace AIQA.Configuration
{
    /// <summary>
    /// Test settings configuration
    /// Manages test execution parameters from various sources
    /// </summary>
    public class TestConfiguration
    {
        private readonly IConfiguration _configuration;

        public TestConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.test.json", optional: true);
            
            _configuration = builder.Build();
        }

        /// <summary>
        /// Base URL of application under test
        /// </summary>
        public string BaseUrl => _configuration["BaseUrl"] ?? "https://www.saucedemo.com/";

        /// <summary>
        /// Browser type for running tests
        /// </summary>
        public string BrowserType => _configuration["Browser"] ?? "chromium";

        /// <summary>
        /// Browser mode (headless or with GUI)
        /// </summary>
        public bool IsHeadless => bool.Parse(_configuration["Headless"] ?? "false");

        /// <summary>
        /// Browser viewport width
        /// </summary>
        public int ViewportWidth => int.Parse(_configuration["ViewportWidth"] ?? "1920");

        /// <summary>
        /// Browser viewport height
        /// </summary>
        public int ViewportHeight => int.Parse(_configuration["ViewportHeight"] ?? "1080");

        /// <summary>
        /// Default timeout for operations in milliseconds
        /// </summary>
        public int DefaultTimeout => int.Parse(_configuration["DefaultTimeout"] ?? "30000");

        /// <summary>
        /// Enable video recording for tests
        /// </summary>
        public bool RecordVideo => bool.Parse(_configuration["RecordVideo"] ?? "false");

        /// <summary>
        /// Directory for saving test results
        /// </summary>
        public string TestResultsDirectory => _configuration["TestResultsDirectory"] ?? "test-results";

        /// <summary>
        /// Maximum number of retries on failure
        /// </summary>
        public int MaxRetries => int.Parse(_configuration["MaxRetries"] ?? "2");

        /// <summary>
        /// Enable parallel test execution
        /// </summary>
        public bool EnableParallel => bool.Parse(_configuration["EnableParallel"] ?? "true");
    }
}