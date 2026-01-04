global using Microsoft.Playwright;
global using Microsoft.Playwright.NUnit;
global using NUnit.Framework;
global using Microsoft.Extensions.Logging;
global using Allure.NUnit.Attributes;
global using System.Text.RegularExpressions;

// Global disable warnings for nullable reference types in tests
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor