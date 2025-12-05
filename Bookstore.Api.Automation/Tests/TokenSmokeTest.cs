using Xunit;
using Bookstore.Api.Automation.Fixtures;

namespace Bookstore.Api.Automation.Tests.Auth
{
    [Collection("Auth collection")]
    public class TokenSmokeTest
    {
        private readonly AuthFixture _fixture;

        public TokenSmokeTest(AuthFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Generate token successfully")]
        public void Should_Generate_Token()
        {
            Assert.False(string.IsNullOrEmpty(_fixture.Token));

            Console.WriteLine("\n[Test] Token received from fixture:");
            Console.WriteLine(_fixture.Token);
        }
    }
}
