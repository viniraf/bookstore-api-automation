using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using FluentAssertions;
using System.Net;
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
            var response = await _catalogClient.GetAllBooksAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("[ASSERT] Expected Status: OK");
            Console.WriteLine($"[ASSERT] Actual Status: {response.StatusCode}");

            // Assert
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Books);
            Assert.NotEmpty(response.Data.Books);

            Console.WriteLine("\n[ASSERT] Expected: Book list not empty");
            Console.WriteLine($"\n[ASSERT] Actual: Book list returned {response?.Data.Books.Count} books");
        }

        [Fact(DisplayName = "Should validate catalog structure")]
        public async Task ShouldValidateCatalogStructure()
        {
            Console.WriteLine("\n---------------------------------------------------");
            Console.WriteLine("[STEP] Calling GET /Books");

            var response = await _catalogClient.GetAllBooksAsync();

            Console.WriteLine("[ASSERT] Expected Status: OK");
            Console.WriteLine($"[ASSERT] Actual Status: {response.StatusCode}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Console.WriteLine("[STEP] Validating response object");
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Books);
            Assert.NotEmpty(response.Data.Books);

            Console.WriteLine("\n[STEP] Validating structure of each returned book\n");

            int index = 1;

            foreach (var book in response.Data.Books!)
            {
                Assert.False(string.IsNullOrWhiteSpace(book.Isbn));
                Assert.False(string.IsNullOrWhiteSpace(book.Title));
                Assert.False(string.IsNullOrWhiteSpace(book.SubTitle));
                Assert.False(string.IsNullOrWhiteSpace(book.Author));
                Assert.False(string.IsNullOrWhiteSpace(book.Publisher));
                Assert.False(string.IsNullOrWhiteSpace(book.Description));
                Assert.False(string.IsNullOrWhiteSpace(book.Website));
                Assert.True(book.Pages > 0);
                Assert.True(book.PublishDate != default);

                Console.WriteLine($"[INFO] Book #{index} structure validated successfully.");
                index++;
            }

            Console.WriteLine("[INFO] Catalog structure is valid");
            Console.WriteLine("---------------------------------------------------");
        }

    }
}
