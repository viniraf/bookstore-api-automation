using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Catalog;
using System.Text.Json;
using System.Net;

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
            Console.WriteLine("\n\n[STEP] Calling GET /Books?ISBN=9781449325862 endpoint");
            string isbn = "9781449325862";

            var response = await _catalogClient.GetBookByIsbnAsync(isbn);

            Console.WriteLine($"\n[ASSERT] Expected Status Code: {HttpStatusCode.OK}");
            Console.WriteLine($"\n[ASSERT] Actual Status Code: {response.StatusCode}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(response.Content));

            var book = JsonSerializer.Deserialize<Book>(response.Content);

            Assert.NotNull(book);
            Console.WriteLine($"\n[ASSERT] Expected ISBN from book: {isbn}");
            Console.WriteLine($"\n[ASSERT] Actual ISBN from book: {book.Isbn}");
            Assert.Equal(isbn, book.Isbn);
        }

        [Fact(DisplayName = "GET /Book?ISBN=invalid - When ISBN is invalid, should return bad request")]
        public async Task GetBookByIsbn_WhenIsbnIsInvalid_ShouldReturnBadRequest()
        {
            Console.WriteLine("\n\n[STEP] Calling GET /Books?ISBN=invalid endpoint");
            string invalidIsbn = "invalid";
            var response = await _catalogClient.GetBookByIsbnAsync(invalidIsbn);

            Console.WriteLine($"\n[ASSERT] Expected Status Code: {HttpStatusCode.BadRequest}");
            Console.WriteLine($"\n[ASSERT] Actual Status Code: {response.StatusCode}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // TODO - Testar
            var error = JsonSerializer.Deserialize<ErrorResponse>(
                response.Content!,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            Assert.NotNull(error);

            Console.WriteLine($"\n[ASSERT] Expected error code: {1205}");
            Console.WriteLine($"\n[ASSERT] Actual error code: {error.Code}");
            Assert.Equal("1205", error.Code);

            Console.WriteLine($"\n[ASSERT] Expected error message: ISBN supplied is not available in Books Collection!");
            Console.WriteLine($"\n[ASSERT] Actual error message: {error.Message}");
            Assert.Equal("ISBN supplied is not available in Books Collection!", error.Message);

        }



    }
}
