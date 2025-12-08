using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Bookstore.Api.Automation.Models.Catalog
{
    /// <summary>
    /// Represents the response structure of GET /Books.
    /// </summary>
    public class BookListResponse
    {
        [JsonPropertyName("books")]
        public List<Book> Books { get; set; } = [];
    }
}
