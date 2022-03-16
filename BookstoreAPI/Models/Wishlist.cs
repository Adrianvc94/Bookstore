using System;
using System.Collections.Generic;

namespace BookstoreAPI.Models
{
    public partial class Wishlist
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdBook { get; set; }
    }
}
