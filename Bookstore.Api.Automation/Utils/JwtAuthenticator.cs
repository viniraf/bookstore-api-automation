using RestSharp;
using RestSharp.Authenticators;

namespace Bookstore.Api.Automation.Utils
{
    public class JwtAuthenticator : IAuthenticator
    {
        private readonly string _token;

        public JwtAuthenticator(string token)
        {
            _token = token;
        }

        public ValueTask Authenticate(IRestClient client, RestRequest request)
        {
            request.AddOrUpdateHeader("Authorization", $"Bearer {_token}");
            return ValueTask.CompletedTask;
        }
    }
}
