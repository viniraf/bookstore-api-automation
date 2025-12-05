using Bookstore.Api.Automation.Clients;
using dotenv.net;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Bookstore.Api.Automation.Fixtures
{
    /// <summary>
    /// Shared fixture that initializes an authentication token once per test collection.
    /// This is equivalent to a pytest fixture with session scope.
    /// </summary>
    public class AuthFixture : IAsyncLifetime
    {
        private readonly string _baseUrlAccount;
        private readonly string _username;
        private readonly string _password;

        public string Token { get; private set; } = string.Empty;

        public AuthFixture() 
        {

            DotEnv.Load();

            _baseUrlAccount = Environment.GetEnvironmentVariable("BASE_URL_ACCOUNT") 
                ?? throw new InvalidOperationException("BASE_URL_ACCOUNT is not set in environment variables.");

            _username = Environment.GetEnvironmentVariable("BOOKSTORE_USERNAME")
                ?? throw new InvalidOperationException("BOOKSTORE_API_USERNAME is not set in environment variables.");

            _password = Environment.GetEnvironmentVariable("BOOKSTORE_PASSWORD")
                ?? throw new InvalidOperationException("BOOKSTORE_API_PASSWORD is not set in environment variables.");
        }

        /// <summary>
        /// Executed once before all tests in the collection.
        /// Generates the authentication token asynchronously.
        /// </summary>
        public async Task InitializeAsync()
        {
            var client = new AuthClient(_baseUrlAccount);
            var tokenResponse = await client.GenerateTokenAsync(_username, _password);

            Token = tokenResponse.Token;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
