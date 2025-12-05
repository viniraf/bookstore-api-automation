using Xunit;

/// <summary>
/// Defines a shared test collection that provides the AuthFixture
/// to all tests that require an authentication token.
/// </summary>
namespace Bookstore.Api.Automation.Fixtures
{
    [CollectionDefinition("Auth collection")]
    public class AuthCollection : ICollectionFixture<AuthFixture> { }
}