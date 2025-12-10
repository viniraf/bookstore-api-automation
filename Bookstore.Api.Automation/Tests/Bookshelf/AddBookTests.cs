using Bookstore.Api.Automation.Clients;
using Bookstore.Api.Automation.Fixtures;
using Bookstore.Api.Automation.Models.Bookshelf;
using Bookstore.Api.Automation.Tests.Builders;
using System;
using System.Net;
using System.Text;
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
            await _bookshelfClient.ClearUserBooksAsync(_userId);
            Console.WriteLine("\n[SETUP] Clearing user's bookshelf before test execution");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact(DisplayName = "POST /book - Should add book successfully")]
        public async Task ShouldAddBookSuccessfully()
        {
            Console.WriteLine("\n---------------------------------------------------");
            // Arrange
            string isbn = "9781449325862";

            Console.WriteLine("\n[STEP] Building request body");
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(isbn)
                .Build();

            // Act
            Console.WriteLine("\n[STEP] Calling POST /Book endpoint");
            var response = await _bookshelfClient.AddBookAsync(requestBody);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine($"\n[ASSERT] Expected Status: {HttpStatusCode.Created}");
            Console.WriteLine($"[ASSERT] Actual Status: {response.StatusCode}");

            Console.WriteLine("\n[STEP] Deserializing response");
            var responseData = JsonSerializer.Deserialize<AddBookResponse>(
                response.Content!,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Books);
            Assert.NotEmpty(responseData.Books);
            Console.WriteLine("\n[ASSERT] Checking response object is not null");

            var books = responseData.Books;
            Assert.Equal(isbn, books.First().Isbn);

            Console.WriteLine($"\n[ASSERT] Checking ISBN in response");
            Console.WriteLine($"[ASSERT] Expected ISBN: {isbn}");
            Console.WriteLine($"[ASSERT] Actual ISBN: {responseData.Books.First().Isbn}");

            Console.WriteLine("\n[INFO] Test finished successfully");
            Console.WriteLine("---------------------------------------------------\n");
        }

        [Fact(DisplayName = "POST /Book - Should return error when adding duplicated ISBN")]
        public async Task ShouldReturnErrorWhenAddingDuplicatedIsbn()
        {
            Console.WriteLine("\n---------------------------------------------------");

            string isbn = "9781449325862";

            // Arrange
            Console.WriteLine("[STEP] Building request body for initial insert");
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(isbn)
                .Build();

            // Act 1
            Console.WriteLine("[STEP] Calling POST /Book for first insert");
            var firstResponse = await _bookshelfClient.AddBookAsync(requestBody);

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
            Console.WriteLine($"[ASSERT] First insert status: {firstResponse.StatusCode}");

            // Act 2
            Console.WriteLine("\n[STEP] Calling POST /Book for duplicated insert");
            var duplicatedResponse = await _bookshelfClient.AddBookAsync(requestBody);

            Assert.Equal(HttpStatusCode.BadRequest, duplicatedResponse.StatusCode);
            Console.WriteLine($"[ASSERT] Expected Status: {HttpStatusCode.BadRequest}");
            Console.WriteLine($"[ASSERT] Actual Status: {duplicatedResponse.StatusCode}");

            // Deserialize error object
            Console.WriteLine("[STEP] Deserializing error response");
            var errorData = JsonSerializer.Deserialize<ErrorResponse>(
                duplicatedResponse.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(errorData);
            Console.WriteLine("[ASSERT] Validating error object");

            Assert.Contains("ISBN already present", errorData.Message, StringComparison.OrdinalIgnoreCase);
            Console.WriteLine($"[ASSERT] API returned message: {errorData!.Message}");

            Console.WriteLine("[INFO] Duplicated insert test finished");
            Console.WriteLine("---------------------------------------------------\n");
        }

        [Fact(DisplayName = "POST /Book - Should return error when ISBN is invalid")]
        public async Task ShouldReturnErrorWhenIsbnInvalid()
        {
            Console.WriteLine("\n---------------------------------------------------");

            string invalidIsbn = "invalid-isbn";

            // Arrange
            Console.WriteLine("[STEP] Building request body with invalid ISBN");
            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbn(invalidIsbn)
                .Build();

            // Act
            Console.WriteLine("[STEP] Calling POST /Book with invalid ISBN");
            var response = await _bookshelfClient.AddBookAsync(requestBody);


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Console.WriteLine($"[ASSERT] Expected Status: {HttpStatusCode.BadRequest}");
            Console.WriteLine($"[ASSERT] Actual Status: {response.StatusCode}");

            // Deserialize error object
            Console.WriteLine("[STEP] Deserializing error response");
            var errorData = JsonSerializer.Deserialize<ErrorResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            
            Assert.NotNull(errorData);
            Console.WriteLine("[ASSERT] Validating error object");

            Console.WriteLine($"[ASSERT] API error message: {errorData!.Message}");
            Assert.Contains("ISBN supplied is not available", errorData.Message, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine("[INFO] Invalid ISBN test finished");
            Console.WriteLine("---------------------------------------------------\n");
        }

        [Fact(DisplayName = "POST /Book - Should add multiple books successfully")]
        public async Task ShouldAddMultipleBooksSuccessfully()
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("[STEP] Preparing request body with multiple ISBNs");

            // Arrange
            string isbn1 = "9781449325862";
            string isbn2 = "9781449331818";

            var requestBody = new AddBookRequestBuilder()
                .WithUserId(_userId)
                .WithIsbns(isbn1, isbn2)
                .Build();

            Console.WriteLine("[STEP] Calling POST /Book endpoint to add multiple books");
            var response = await _bookshelfClient.AddBookAsync(requestBody);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine($"\n[ASSERT] Expected Status: {HttpStatusCode.Created}");
            Console.WriteLine($"[ASSERT] Actual Status: {response.StatusCode}");

            Console.WriteLine("[STEP] Deserializing response body");
            var responseData = JsonSerializer.Deserialize<AddBookResponse>(
                response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("[ASSERT] Validating response content");
            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Books);
            Assert.NotEmpty(responseData.Books);

            var books = responseData.Books;
            Assert.True(books.Count == 2, $"Expected 2 books in response instead of {books.Count}");

            var isbnsReturned = books.Select(b => b.Isbn).ToList();
            Assert.Contains(isbn1, isbnsReturned);
            Assert.Contains(isbn2, isbnsReturned);

            Console.WriteLine($"[ASSERT] Expected ISBNs: {isbn1}, {isbn2}");
            Console.WriteLine($"[ASSERT] Actual ISBNs returned: {string.Join(", ", isbnsReturned)}");

            Console.WriteLine("[INFO] add multiple books test finished");
            Console.WriteLine("---------------------------------------------------\n");
        }
    }
}
