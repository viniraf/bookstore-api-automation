using Bookstore.Api.Automation.Models.Auth;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Api.Automation.Clients
{
    /// <summary>
    /// Client responsible for authentication-related API calls.
    /// Handles token generation for the Bookstore API.
    /// </summary>
    public class AuthClient
    {
        private readonly RestClient _client;

        public AuthClient(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        /// <summary>
        /// Sends a request to generate a JWT authentication token.
        /// </summary>
        /// <param name="username">Bookstore API username.</param>
        /// <param name="password">Bookstore API password.</param>
        /// <returns>The parsed authentication response containing the token.</returns>
        public async Task<TokenResponse> GenerateTokenAsync(string username, string password)
        {
            var request = new RestRequest("/GenerateToken", Method.Post);

            request.AddJsonBody(new
            {
                userName = username,
                password = password
            });

            var response = await _client.ExecuteAsync<TokenResponse>(request);

            if (!response.IsSuccessful || response.Data?.Token == null)
            {
                throw new InvalidOperationException($"Failed to generate token: {response.StatusCode} - {response.Content}");
            }

            return response.Data;
        }
    }
}
