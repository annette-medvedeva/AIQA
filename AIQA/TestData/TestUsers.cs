namespace AIQA.TestData
{
    /// <summary>
    /// Constants for test user data in Saucedemo application
    /// Contains various types of user accounts for testing different scenarios
    /// </summary>
    public static class TestUsers
    {
        /// <summary>
        /// Standard user data with normal access to application
        /// </summary>
        public static class StandardUser
        {
            /// <summary>Username for standard user</summary>
            public const string Username = "standard_user";
            
            /// <summary>Password for standard user</summary>
            public const string Password = "secret_sauce";
        }

        /// <summary>
        /// Locked user data (for testing negative scenarios)
        /// </summary>
        public static class LockedUser
        {
            /// <summary>Username for locked user</summary>
            public const string Username = "locked_out_user";
            
            /// <summary>Password for locked user</summary>
            public const string Password = "secret_sauce";
        }

        /// <summary>
        /// Problem user data with issues in application behavior
        /// </summary>
        public static class ProblemUser
        {
            /// <summary>Username for problem user</summary>
            public const string Username = "problem_user";
            
            /// <summary>Password for problem user</summary>
            public const string Password = "secret_sauce";
        }

        /// <summary>
        /// Performance glitch user data with issues in application performance
        /// </summary>
        public static class PerformanceGlitchUser
        {
            /// <summary>Username for performance glitch user</summary>
            public const string Username = "performance_glitch_user";
            
            /// <summary>Password for performance user</summary>
            public const string Password = "secret_sauce";
        }

        /// <summary>
        /// Invalid user data (for negative testing)
        /// </summary>
        public static class InvalidUser
        {
            /// <summary>Invalid username</summary>
            public const string Username = "invalid_user";
            
            /// <summary>Invalid password</summary>
            public const string Password = "invalid_password";
        }
    }
}