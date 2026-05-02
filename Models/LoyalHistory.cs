using SQLite;
using System;

namespace LodgeStay.Models
{
    [Table("LoyaltyHistory")]
    public class LoyaltyHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int GuestId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public bool IsEarn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}