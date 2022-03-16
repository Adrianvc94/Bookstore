using System;
using System.Collections.Generic;

namespace BookstoreAPI.Models
{
    public partial class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int IdUser { get; set; }
        public int IdBook { get; set; }
    }
}
