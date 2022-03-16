using System;
using System.Collections.Generic;

namespace BookstoreClient.Models
{
    public partial class Book
    {
        public int Id { get; set; }
        public string Titles { get; set; } = null!;
        public string Authors { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string PublishedDate { get; set; } = null!;
        public string SmallThumbnail { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public string InfoLink { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
