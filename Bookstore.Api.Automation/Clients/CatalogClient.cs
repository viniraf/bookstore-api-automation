using Bookstore.Api.Automation.Models.Catalog;
using Bookstore.Api.Automation.Utils;
using RestSharp;

namespace Bookstore.Api.Automation.Clients
{
    /// <summary>
    /// Client responsible for interacting with the Bookstore catalog endpoints.
    /// Handles operations such as listing all books or retrieving a book by ISBN.
    /// </summary>
    public class CatalogClient
    {
        private readonly RestClient _client;

        public CatalogClient(string baseUrl, string token)
        {
            _client = RestClientFactory.Create(baseUrl, token);
        }

        public async Task<RestResponse<BookListResponse>> GetAllBooksAsync()
        {
            RestRequest request = new RestRequest("/Books", Method.Get);

            RestResponse<BookListResponse> response = await _client.ExecuteAsync<BookListResponse>(request);

            AllureReport.AttachApiCall(null, response, "GET /Books");

            return response;
        }

        public async Task<RestResponse> GetBookByIsbnAsync(string isbn)
        { 
            RestRequest request = new RestRequest($"/Book", Method.Get);
            request.AddQueryParameter("ISBN", isbn);

            RestResponse response = await _client.ExecuteAsync(request);

            AllureReport.AttachApiCall(new { ISBN = isbn }, response, "GET /Book");

            return response;
        }
    }
}