using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using Bookstore.Api.Automation.Utils;
using System.Net;
using System.Text.Json;

namespace Bookstore.Api.Automation.Tests.Catalog
{
    [Collection("Auth collection")]
    [AllureSuite("Catalog")]
    [AllureParentSuite("API Automation")]
    [AllureEpic("Bookstore API")]
    [AllureFeature("Catalog")]
    public class GetBookByIsbnTest
    {
        private readonly AuthFixture _authFixture;
        private readonly CatalogClient _catalogClient;

        public GetBookByIsbnTest(AuthFixture authFixture)
        {
            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL_BOOKSTORE")
                          ?? throw new InvalidOperationException("BASE_URL_BOOKSTORE not set");

            _authFixture = authFixture;
            _catalogClient = new CatalogClient(baseUrl, _authFixture.Token);
        }

        [Fact(DisplayName = "GET /Book?ISBN=valid - When ISBN is valid, should return book")]
        [AllureStory("Retrieve Book Details")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("QA Automation")]
        [AllureTag("API", "Regression", "Catalog")]
        public async Task GetBookByIsbn_WhenIsbnIsValid_ShouldReturnBook()
        {
            string isbn = "9781449325862";

            AllureReport.Arrange("Valid ISBN request", new { ISBN = isbn });

            var response = await _catalogClient.GetBookByIsbnAsync(isbn);

            Assert.False(string.IsNullOrEmpty(response.Content));

            var book = JsonSerializer.Deserialize<Book>(response.Content);

            Assert.NotNull(book);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            

            var assertions = new AllureReport.AssertionBuilder()
                .Add("Status Code","200 OK", $"{(int)response.StatusCode} {response.StatusCode}")
                .Add("ISBN", isbn, book.Isbn);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "GET /Book?ISBN=invalid - When ISBN is invalid, should return bad request")]
        [AllureStory("Handle Invalid Book Lookup")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("QA Automation")]
        [AllureTag("API", "Negative", "Validation")]
        public async Task GetBookByIsbn_WhenIsbnIsInvalid_ShouldReturnBadRequest()
        {
            string invalidIsbn = "invalid";

            AllureReport.Arrange("Invalid ISBN request", new { ISBN = invalidIsbn });

            var response = await _catalogClient.GetBookByIsbnAsync(invalidIsbn);

            var error = JsonSerializer.Deserialize<ErrorResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(error);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("1205", error.Code);
            Assert.Equal("ISBN supplied is not available in Books Collection!", error.Message);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("Status Code","400 BadRequest", $"{(int)response.StatusCode} {response.StatusCode}")
                .Add("Error Code", "1205", error.Code)
                .Add("Error Message","ISBN supplied is not available in Books Collection!", error.Message);

            AllureReport.Assertions(assertions);

        }
    }
}
