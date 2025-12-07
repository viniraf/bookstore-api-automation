using Bookstore.Api.Automation.Models.Catalog;
using Bookstore.Api.Automation.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task<BookListResponse?> GetAllBooksAsync()
        {
            RestRequest request = new RestRequest("/Books", Method.Get);

            RestResponse<BookListResponse> response = await _client.ExecuteAsync<BookListResponse>(request);

            if (!response.IsSuccessful)
                throw new InvalidOperationException(
                    $"Failed to fetch books. Status: {response.StatusCode}, Message: {response.ErrorMessage}");

            return response.Data;
        }

    }

}