using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using Bookstore.Api.Automation.Utils;
using System.Net;
using System.Text.Json;

namespace Bookstore.Api.Automation.Tests.Catalog
{
    [Collection("Auth collection")]
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
        public async Task GetBookByIsbn_WhenIsbnIsValid_ShouldReturnBook()
        {
            string isbn = "9781449325862";

            AllureReport.Arrange("Valid ISBN request", new { ISBN = isbn });

            var response = await _catalogClient.GetBookByIsbnAsync(isbn);

            Assert.False(string.IsNullOrEmpty(response.Content));

            var book = JsonSerializer.Deserialize<Book>(response.Content);

            Assert.NotNull(book);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.OK, response.StatusCode)
                .Add("ISBN", isbn, book.Isbn);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "GET /Book?ISBN=invalid - When ISBN is invalid, should return bad request")]
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

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.BadRequest, response.StatusCode)
                .Add("Error Code", "1205", error.Code)
                .Add("Error Message", "ISBN supplied is not available in Books Collection!", error.Message);

            AllureReport.Assertions(assertions);
        }
    }
}
