using SQLite;
using System;

namespace LodgeStay.Models
{
    [Table ("OtpVerifications")]
    public class OtpVerification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int User_Id { get; set; }

        [NotNull]
        public string Code { get; set; } = string.Empty;

        [NotNull]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(5);

        [NotNull]
        public bool IsUsed { get; set; } = false;

        [NotNull]
        public DateTime CreatedAt { get; set;} = DateTime.UtcNow; 
    }
}
