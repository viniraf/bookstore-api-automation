namespace Bookstore.Api.Automation.Models.Bookshelf
{
    public class AddBookRequest
    {
        public string UserId { get; set; } = "";
        public List<IsbnEntry> CollectionOfIsbns { get; set; } = new();

        public AddBookRequest() { }
    }

    public class IsbnEntry
    {
        public string Isbn { get; set; } = "";

        public IsbnEntry() { }
    }
}