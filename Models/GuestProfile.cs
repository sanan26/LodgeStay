using SQLite;
using System;

namespace LodgeStay.Models
{
    [Table("GuestProfiles")]
    public class GuestProfile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull, Unique]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        public string PhoneNo { get; set; } = string.Empty;

        public string PreferredRoomType { get; set; } = string.Empty;
        public string PreferredBedType { get; set; } = string.Empty;
        public string PreferredFloor { get; set; } = string.Empty;
        public string DietaryNotes { get; set; } = string.Empty;
        public string AmenityNotes { get; set; } = string.Empty;

        public int LoyaltyPoints { get; set; } = 0;
        public string LoyaltyTier { get; set; } = "Bronze";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int EcoPoints { get; set; } = 0;
    }
}
