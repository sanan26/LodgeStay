namespace LodgeStay.Models
{
    public class NotificationPreferences
    {
        public bool NewBooking { get; set; } = true;
        public bool CheckIn { get; set; } = true;
        public bool CheckOut { get; set; } = true;
        public bool LowOccupancy { get; set; } = true;

        public bool CheckInReminder { get; set; } = true;
        public bool CheckOutReminder { get; set; } = true;
    }
}