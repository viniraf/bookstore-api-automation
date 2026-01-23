using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using Bookstore.Api.Automation.Utils;
using System.Net;

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

        [Fact(DisplayName = "GET /Books - When books exist, should return book list")]
        public async Task GetAllBooks_WhenBooksExist_ShouldReturnBookList()
        {
            // Arrange
            string token = _authFixture.Token;
            Assert.False(string.IsNullOrWhiteSpace(token), "The token must be valid for authenticated requests");

            AllureReport.Arrange("Authenticated request", new { Token = "Bearer token" });

            // Act
            var response = await _catalogClient.GetAllBooksAsync();

            // Assert
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Books);
            Assert.NotEmpty(response.Data.Books);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.OK, response.StatusCode)
                .Add("Books Count", ">0", response.Data.Books.Count.ToString());

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "GET /Books - When called, should return valid catalog structure")]
        public async Task GetAllBooks_WhenCalled_ShouldReturnValidCatalogStructure()
        {
            AllureReport.Arrange("Catalog structure validation", null);

            var response = await _catalogClient.GetAllBooksAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Books);
            Assert.NotEmpty(response.Data.Books);

            // Validate each book structure
            int validatedBooks = 0;
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
                validatedBooks++;
            }

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.OK, response.StatusCode)
                .Add("Books Not Empty", true, response.Data.Books.Count > 0)
                .Add("Valid Books Count", ">0", validatedBooks.ToString())
                .Add("All Fields Present", true, validatedBooks == response.Data.Books.Count);

            AllureReport.Assertions(assertions);
        }
    }
}
