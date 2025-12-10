using Bookstore.Api.Automation.Models.Bookshelf;

namespace Bookstore.Api.Automation.Tests.Builders
{
    public class AddBookRequestBuilder
    {
        private readonly AddBookRequest _request;

        public AddBookRequestBuilder()
        {
            _request = new AddBookRequest();
        }

        public AddBookRequestBuilder WithUserId(string userId)
        {
            _request.UserId = userId;
            return this;
        }

        public AddBookRequestBuilder WithIsbn(string isbn)
        {
            _request.CollectionOfIsbns = new List<IsbnEntry>
            {
                new IsbnEntry { Isbn = isbn }
            };
            return this;
        }

        public AddBookRequestBuilder WithIsbns(params string[] isbns)
        {
            _request.CollectionOfIsbns = isbns
                .Select(i => new IsbnEntry { Isbn = i })
                .ToList();
            return this;
        }

        public AddBookRequest Build()
        {
            return _request;
        }
    }
}