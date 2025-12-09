using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Api.Automation.Models.Bookshelf
{
    public class ErrorResponse
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
