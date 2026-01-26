using Allure.Net.Commons;
using Allure.Xunit.Attributes;
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
    [AllureSuite("Catalog")]
    [AllureParentSuite("API Automation")]
    [AllureEpic("Bookstore API")]
    [AllureFeature("Catalog")]
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
        [AllureStory("Retrieve All Books")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("QA Automation")]
        [AllureTag("API", "Regression", "Catalog")]
        public async Task GetAllBooks_WhenBooksExist_ShouldReturnBookList()
        {
            // Act
            var response = await _catalogClient.GetAllBooksAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Books);
            Assert.NotEmpty(response.Data.Books);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("Status Code", "200 OK", $"{(int)response.StatusCode} {response.StatusCode}")
                .Add("Books Count > 0", true, response.Data.Books.Count > 0);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "GET /Books - When called, should return valid catalog structure")]
        [AllureStory("Validate Catalog Data Contract")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("QA Automation")]
        [AllureTag("API", "Contract", "Schema")]
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
                .Add("Status Code","200 OK", $"{(int)response.StatusCode} {response.StatusCode}")
                .Add("Books Count > 0", true, response.Data.Books.Count > 0)
                .Add("Validated Books", response.Data.Books.Count, validatedBooks)
                .Add("All Fields Present", true, validatedBooks == response.Data.Books.Count);


            AllureReport.Assertions(assertions);
        }
    }
}
