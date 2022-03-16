using System;
using System.Collections.Generic;

namespace BookstoreAPI.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
    }
}
