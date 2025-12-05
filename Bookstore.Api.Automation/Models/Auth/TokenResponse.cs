using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Bookstore.Api.Automation.Models.Auth
{
    /// <summary>
    /// Represents the response returned by the token generation endpoint.
    /// </summary>
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expires")]
        public string? Expires { get; set; }
    }
}
