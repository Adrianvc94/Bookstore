using System;
using System.Collections.Generic;

namespace BookstoreClient.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
