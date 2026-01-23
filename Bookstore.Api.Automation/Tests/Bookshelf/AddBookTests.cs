using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Bookshelf;
using Bookstore.Api.Automation.Tests.Builders;
using Bookstore.Api.Automation.Utils;
using System.Net;
using System.Text.Json;

namespace Bookstore.Api.Automation.Tests.Bookshelf
{
    [Collection("Auth collection")]
    public class AddBookTests : IAsyncLifetime
    {
        private readonly AuthFixture _authFixture;
        private readonly BookshelfClient _bookshelfClient;
        private readonly string _userId;

        public AddBookTests(AuthFixture authFixture)
        {
            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL_BOOKSTORE")
                          ?? throw new InvalidOperationException("BASE_URL_BOOKSTORE not set");

            _userId = Environment.GetEnvironmentVariable("BOOKSTORE_USER_ID")
              ?? throw new InvalidOperationException("USER_ID_BOOKSTORE not set");

            _authFixture = authFixture;
            _bookshelfClient = new BookshelfClient(baseUrl, _authFixture.Token);
        }

        public async Task InitializeAsync()
        {
            await _bookshelfClient.DeleteUserBooksAsync(_userId);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact(DisplayName = "POST /Book - When book is valid, should add book successfully")]
        public async Task AddBook_WhenBookIsValid_ShouldAddBookSuccessfully()
        {
            string isbn = "9781449325862";
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(isbn)
                .Build();

            AllureReport.Arrange("Valid book request", requestBody);

            var response = await _bookshelfClient.AddBookAsync(requestBody);

            var responseData = JsonSerializer.Deserialize<AddBookResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Books);
            Assert.NotEmpty(responseData.Books);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.Created, response.StatusCode)
                .Add("ISBN", isbn, responseData.Books.First().Isbn);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "POST /Book - When ISBN already exists, should return error")]
        public async Task AddBook_WhenIsbnAlreadyExists_ShouldReturnError()
        {
            string isbn = "9781449325862";
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(isbn)
                .Build();

            AllureReport.Arrange("Valid book request for duplicate test", requestBody);

            var firstResponse = await _bookshelfClient.AddBookAsync(requestBody);
            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var duplicatedResponse = await _bookshelfClient.AddBookAsync(requestBody);

            var errorData = JsonSerializer.Deserialize<ErrorResponse>(
                duplicatedResponse.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(errorData);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.BadRequest, duplicatedResponse.StatusCode)
                .Add("Error Message", "ISBN already present", errorData.Message);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "POST /Book - When ISBN is invalid, should return error")]
        public async Task AddBook_WhenIsbnIsInvalid_ShouldReturnError()
        {
            string invalidIsbn = "invalid-isbn";
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(invalidIsbn)
                .Build();

            AllureReport.Arrange("Invalid ISBN request", requestBody);

            var response = await _bookshelfClient.AddBookAsync(requestBody);

            var errorData = JsonSerializer.Deserialize<ErrorResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(errorData);

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.BadRequest, response.StatusCode)
                .Add("Error Message", "ISBN supplied is not available", errorData.Message);

            AllureReport.Assertions(assertions);
        }

        [Fact(DisplayName = "POST /Book - When multiple books are provided, should add all successfully")]
        public async Task AddBook_WhenMultipleBooksProvided_ShouldAddAllSuccessfully()
        {
            string isbn1 = "9781449325862";
            string isbn2 = "9781449331818";

            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbns(isbn1, isbn2)
                .Build();

            AllureReport.Arrange("Multiple books request", requestBody);

            var response = await _bookshelfClient.AddBookAsync(requestBody);

            var responseData = JsonSerializer.Deserialize<AddBookResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Books);
            Assert.NotEmpty(responseData.Books);

            var books = responseData.Books;
            var isbnsReturned = books.Select(b => b.Isbn).ToList();

            var assertions = new AllureReport.AssertionBuilder()
                .Add("StatusCode", HttpStatusCode.Created, response.StatusCode)
                .Add("Books Count", 2, books.Count)
                .Add("ISBN 1", isbn1, isbnsReturned.Contains(isbn1) ? isbn1 : "NOT FOUND")
                .Add("ISBN 2", isbn2, isbnsReturned.Contains(isbn2) ? isbn2 : "NOT FOUND");

            AllureReport.Assertions(assertions);
        }
    }
}
