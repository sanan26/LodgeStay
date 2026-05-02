using SQLite;

namespace LodgeStay.Models
{
    [Table("LocalDeals")]
    public class LocalDeal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string PartnerName { get; set; } = string.Empty;

        [NotNull, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public double Price { get; set; } = 0.0;

        // true = visible to staff during booking, false = hidden
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}