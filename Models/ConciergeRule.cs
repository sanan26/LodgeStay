using SQLite;

namespace LodgeStay.Models
{
    [Table("ConciergeRules")]
    public class ConciergeRule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Comma-separated keywords e.g. "checkin,check in,arrival time"
        [NotNull, MaxLength(500)]
        public string Keywords { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [NotNull, MaxLength(1000)]
        public string ResponseText { get; set; } = string.Empty;

        // Pre-written template staff can copy to WhatsApp/email
        [MaxLength(1000)]
        public string ResponseTemplate { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // All available categories
    public static class ConciergeCategories
    {
        public const string CheckIn = "Check-In";
        public const string CheckOut = "Check-Out";
        public const string Cancellation = "Cancellation";
        public const string RoomTypes = "Room Types";
        public const string Breakfast = "Breakfast";
        public const string WiFi = "Wi-Fi";
        public const string Pricing = "Pricing";
        public const string General = "General";
    }
}