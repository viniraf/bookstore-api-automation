using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using FluentAssertions;
using Xunit.Abstractions;

namespace Bookstore.Api.Automation.Tests.Catalog
{
    /// <summary>
    /// Tests for retrieving the full list of books from the Bookstore API.
    /// Ensures the CatalogClient correctly fetches and deserializes the response.
    /// </summary>

    [Collection("Auth collection")]
    public class GetAllBooksTest
    {
        private readonly AuthFixture _authFixture;
        private readonly CatalogClient _catalogClient;

        public GetAllBooksTest(AuthFixture authFixture)
        {
            // Initialize client with base URL of bookstore API
            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL_BOOKSTORE")
                          ?? throw new InvalidOperationException("BASE_URL_BOOKSTORE not set");

            _authFixture = authFixture;
            _catalogClient = new CatalogClient(baseUrl, _authFixture.Token);
        }

        [Fact(DisplayName = "GET /Books - Should return list of all books successfully")]
        public async Task Should_Return_All_Books_Successfully()
        {
            Console.WriteLine("\n[STEP] Calling GET /Books endpoint");

            // Arrange
            string token = _authFixture.Token;
            Assert.False(string.IsNullOrWhiteSpace(token), "The token must be valid for authenticated requests");

            // Act
            BookListResponse? result = await _catalogClient.GetAllBooksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Books);
            Assert.NotEmpty(result.Books);

            Console.WriteLine("\n[ASSERT] Expected: Book list not empty");
            Console.WriteLine($"\n[ASSERT] Actual: Book list returned {result?.Books.Count} books");
        }

    }
}
