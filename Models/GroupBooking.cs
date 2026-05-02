using LodgeStay.Models;

namespace LodgeStay.Models
{
    public class GroupBooking
    {
        public string MasterReference { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string OrganizerEmail { get; set; } = "";
        public List<Reservation> Reservations { get; set; } = new();
        public int DiscountPercent { get; set; }
        public double TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}