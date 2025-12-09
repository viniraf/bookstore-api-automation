using Bookstore.Api.Automation.Models.Bookshelf;
using Bookstore.Api.Automation.Utils;
using RestSharp;

namespace Bookstore.Api.Automation.Clients
{
    public class BookshelfClient
    {
        private readonly RestClient _client;

        public BookshelfClient(string baseUrl, string token)
        {
            _client = RestClientFactory.Create(baseUrl, token);
        }

        public async Task<RestResponse> ClearUserBooksAsync(string userId)
        {
            var request = new RestRequest("/Books", Method.Delete);
            request.AddQueryParameter("UserId", userId);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> AddBookAsync(AddBookRequest request)
        {
            RestRequest restRequest = new RestRequest("/Books", Method.Post);

            restRequest.AddJsonBody(request);

            RestResponse restResponse = await _client.ExecuteAsync(restRequest);

            return restResponse;
        }
    }
}