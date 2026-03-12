using System;
using SQLite;

namespace LodgeStay.Models
{
    [Table("Reservations")]
    public class Reservation
    {
        [PrimaryKey, AutoIncrement]
        public int Reservation_ID { get; set; }

        [NotNull]
        public string GuestName { get; set; } = string.Empty;

        [NotNull]
        public string GuestEmail { get; set; } = string.Empty;

        [NotNull]
        public string GuestPhone { get; set; } = string.Empty;

        [NotNull]
        public int Room_ID { get; set; }

        [NotNull]
        public int User_ID { get; set; }

        [NotNull]
        public DateTime CheckIn { get; set; } = DateTime.UtcNow;

        [NotNull]
        public DateTime CheckOut { get; set; } = DateTime.UtcNow.AddDays(1);

        [NotNull]
        public int NumberOfGuests { get; set; } = 1;

        [NotNull]
        public double TotalPrice { get; set; } = 0.0;

        [NotNull]
        public string Status { get; set; } = "Confirmed";

        [NotNull, Unique]
        public string BookingReference { get; set; } = "LS-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        [NotNull]
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
    }
}
