using SQLite;

namespace LodgeStay.Models
{
    [Table("EcoActions")]
    public class EcoAction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int GuestId { get; set; }

        [NotNull]
        public int ReservationId { get; set; }

        [NotNull, MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        public int PointsAwarded { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }

    public static class EcoActionTypes
    {
        public const string TowelReuse = "TowelReuse";
        public const string SkipHousekeeping = "SkipHousekeeping";
        public const string DigitalReceipt = "DigitalReceipt";
    }

    public static class EcoActionPoints
    {
        public const int TowelReuse = 10;
        public const int SkipHousekeeping = 20;
        public const int DigitalReceipt = 5;

        public static int GetPoints(string actionType) => actionType switch
        {
            EcoActionTypes.TowelReuse => TowelReuse,
            EcoActionTypes.SkipHousekeeping => SkipHousekeeping,
            EcoActionTypes.DigitalReceipt => DigitalReceipt,
            _ => 0
        };
    }
}