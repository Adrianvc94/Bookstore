namespace BookstoreClient.Models
{
    public class BookDto
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
