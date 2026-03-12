using System;
using SQLite;

namespace LodgeStay.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int User_ID { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull, Unique]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        public string PhoneNo { get; set; } = string.Empty;

        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        [NotNull]
        public string Role { get; set; } = "User";

        [NotNull]
        public bool IsVerified { get; set; } = false;

        [NotNull]
        public DateTime Created_at { get; set; } = DateTime.UtcNow;

    }
}
