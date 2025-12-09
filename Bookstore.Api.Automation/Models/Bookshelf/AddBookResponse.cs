using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Api.Automation.Models.Bookshelf
{
    public class AddBookResponse
    {
        public List<BookEntry>? Books { get; set; }
    }

    public class BookEntry
    {
        public string Isbn { get; set; } = string.Empty;
    }
}
