using Allure.Net.Commons;
using Microsoft.Extensions.Logging;

namespace AIQA.Utilities
{
    /// <summary>
    /// Helper class for working with Allure reports
    /// Provides methods for adding additional information to reports
    /// </summary>
    public static class AllureHelper
    {
        /// <summary>
        /// Adds text attachment to Allure report
        /// </summary>
        /// <param name="name">Attachment name</param>
        /// <param name="content">Attachment content</param>
        /// <param name="type">Content type</param>
        public static void AddTextAttachment(string name, string content, string type = "text/plain")
        {
            try
            {
                AllureApi.AddAttachment(name, type, System.Text.Encoding.UTF8.GetBytes(content));
            }
            catch (Exception ex)
            {
                // Log error but don't interrupt test execution
                Console.WriteLine($"Failed to add text attachment to Allure: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds JSON data as attachment
        /// </summary>
        /// <param name="name">Attachment name</param>
        /// <param name="jsonContent">JSON content</param>
        public static void AddJsonAttachment(string name, string jsonContent)
        {
            AddTextAttachment(name, jsonContent, "application/json");
        }

        /// <summary>
        /// Adds log file as attachment
        /// </summary>
        /// <param name="name">Attachment name</param>
        /// <param name="logContent">Log content</param>
        public static void AddLogAttachment(string name, string logContent)
        {
            AddTextAttachment(name, logContent, "text/plain");
        }

        /// <summary>
        /// Adds execution step to report
        /// </summary>
        /// <param name="stepName">Step name</param>
        /// <param name="action">Action to execute</param>
        public static async Task Step(string stepName, Func<Task> action)
        {
            try
            {
                AllureApi.Step(stepName);
                await action();
            }
            catch (Exception ex)
            {
                AllureApi.Step($"ERROR in step '{stepName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds step with return value
        /// </summary>
        /// <typeparam name="T">Return value type</typeparam>
        /// <param name="stepName">Step name</param>
        /// <param name="func">Function to execute</param>
        /// <returns>Function execution result</returns>
        public static async Task<T> Step<T>(string stepName, Func<Task<T>> func)
        {
            try
            {
                AllureApi.Step(stepName);
                return await func();
            }
            catch (Exception ex)
            {
                AllureApi.Step($"ERROR in step '{stepName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds environment information
        /// </summary>
        /// <param name="key">Environment key</param>
        /// <param name="value">Environment value</param>
        public static void AddEnvironmentInfo(string key, string value)
        {
            try
            {
                AllureLifecycle.Instance.UpdateTestContainer(container => container.name = key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add environment information to Allure: {ex.Message}");
            }
        }
    }
}