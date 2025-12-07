using RestSharp;

namespace Bookstore.Api.Automation.Utils
{
    public static class RestClientFactory
    {
        public static RestClient Create(string baseUrl, string token)
        {
            var options = new RestClientOptions(baseUrl)
            {
                Timeout = TimeSpan.FromSeconds(30),
                Authenticator = new JwtAuthenticator(token)
            };

            var client = new RestClient(options);

            return client;
        }
    }
}