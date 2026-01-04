using System.Text.RegularExpressions;

namespace AIQA.Utilities
{
    /// <summary>
    /// Helper methods for working with strings in tests
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Generates unique string with timestamp
        /// </summary>
        /// <param name="prefix">String prefix</param>
        /// <returns>Unique string</returns>
        public static string GenerateUniqueString(string prefix = "test")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            return $"{prefix}_{timestamp}";
        }

        /// <summary>
        /// Cleans string from extra characters and spaces
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Cleaned string</returns>
        public static string CleanString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            
            return input.Trim().Replace("\r", "").Replace("\n", " ");
        }

        /// <summary>
        /// Extracts numeric value from string
        /// </summary>
        /// <param name="input">String containing number</param>
        /// <returns>Numeric value or 0 on error</returns>
        public static decimal ExtractDecimalFromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            var match = Regex.Match(input, @"\d+\.?\d*");
            if (match.Success && decimal.TryParse(match.Value, out var result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Checks if string contains valid dollar price
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>True if string contains price</returns>
        public static bool IsValidPrice(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"\$\d+\.?\d*");
        }

        /// <summary>
        /// Formats filename for safe use in file paths
        /// </summary>
        /// <param name="input">Original name</param>
        /// <returns>Safe filename</returns>
        public static string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "unknown";

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                input = input.Replace(c, '_');
            }
            
            return input.Trim('_');
        }
    }
}