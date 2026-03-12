using System;
using SQLite;

namespace LodgeStay.Models
{
    [Table("Rooms")]
    public class Room
    {
        [PrimaryKey, AutoIncrement]
        public int Room_ID { get; set; }

        [NotNull, Unique]
        public string RoomNo { get; set; } = string.Empty;

        [NotNull]
        public string Room_Type { get; set; } = string.Empty;

        [NotNull]
        public int Capacity { get; set; } = 1;

        [NotNull]
        public double Price { get; set; } = 0.0;

        [NotNull]
        public bool IsEcoCertified { get; set; } = false;

        [NotNull]
        public string Status { get; set; } = "Available";
    }
}
