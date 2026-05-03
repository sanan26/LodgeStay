namespace LodgeStay.Models
{
    public class DashboardSummary
    {
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public double OccupancyPercent { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public double MonthRevenue { get; set; }
    }
}