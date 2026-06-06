using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class ReportService
    {
        private readonly DatabaseContext _db;

        public ReportService(DatabaseContext db)
        {
            _db = db;
        }

        // ── Occupancy Report ──────────────────────────────────────────────────
        public async Task<OccupancyReport> GetOccupancyReportAsync(DateTime startDate, DateTime endDate)
        {
            var reservations = await _db.GetAllReservationsAsync();
            var rooms = await _db.GetAllRoomsAsync();

            var filtered = reservations.Where(r =>
                r.Status == "Confirmed" &&
                r.CheckIn.Date >= startDate.Date &&
                r.CheckOut.Date <= endDate.Date).ToList();

            return new OccupancyReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRooms = rooms.Count,
                TotalReservations = filtered.Count,
                OccupancyPercent = rooms.Count == 0 ? 0 :
                                    Math.Round((double)filtered.Count / rooms.Count * 100, 1),
                Reservations = filtered
            };
        }

        // ── Revenue Report ────────────────────────────────────────────────────
        public async Task<RevenueReport> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var reservations = await _db.GetAllReservationsAsync();
            var rooms = await _db.GetAllRoomsAsync();

            var filtered = reservations.Where(r =>
                r.Status == "Confirmed" &&
                r.CheckIn.Date >= startDate.Date &&
                r.CheckOut.Date <= endDate.Date).ToList();

            var breakdown = filtered.Select(r => new RevenueLineItem
            {
                BookingReference = r.BookingReference,
                GuestName = r.GuestName,
                RoomNo = rooms.FirstOrDefault(rm => rm.Room_ID == r.Room_ID)?.RoomNo ?? "N/A",
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                TotalPrice = r.TotalPrice
            }).ToList();

            return new RevenueReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = filtered.Sum(r => r.TotalPrice),
                TotalBookings = filtered.Count,
                LineItems = breakdown
            };
        }

        // ── Loyalty Report ────────────────────────────────────────────────────
        public async Task<LoyaltyReport> GetLoyaltyReportAsync(DateTime startDate, DateTime endDate)
        {
            var guests = await _db.GetAllGuestsAsync();
            var history = new List<LoyaltyHistory>();

            foreach (var guest in guests)
            {
                var guestHistory = await _db.GetLoyaltyHistoryByGuestAsync(guest.Id);
                var filtered = guestHistory.Where(h =>
                    h.CreatedAt.Date >= startDate.Date &&
                    h.CreatedAt.Date <= endDate.Date).ToList();
                history.AddRange(filtered);
            }

            return new LoyaltyReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalPointsEarned = history.Where(h => h.Points > 0).Sum(h => h.Points),
                TotalPointsRedeemed = Math.Abs(history.Where(h => h.Points < 0).Sum(h => h.Points)),
                TotalTransactions = history.Count,
                TopGuests = guests.OrderByDescending(g => g.LoyaltyPoints)
                                            .Take(5)
                                            .ToList()
            };
        }

        // ── Eco Report ────────────────────────────────────────────────────────
        public async Task<EcoReport> GetEcoReportAsync(DateTime startDate, DateTime endDate)
        {
            var actions = await _db.GetAllEcoActionsAsync();

            var filtered = actions.Where(a =>
                a.RecordedAt.Date >= startDate.Date &&
                a.RecordedAt.Date <= endDate.Date).ToList();

            return new EcoReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalActions = filtered.Count,
                TotalEcoPointsAwarded = filtered.Sum(a => a.PointsAwarded),
                TowelReuseCount = filtered.Count(a => a.ActionType == EcoActionTypes.TowelReuse),
                SkipHousekeepingCount = filtered.Count(a => a.ActionType == EcoActionTypes.SkipHousekeeping),
                DigitalReceiptCount = filtered.Count(a => a.ActionType == EcoActionTypes.DigitalReceipt),
                WaterSavedLitres = filtered.Count(a => a.ActionType == EcoActionTypes.TowelReuse) * 15,
                UniqueGuestsParticipated = filtered.Select(a => a.GuestId).Distinct().Count()
            };
        }

        // ── Generate Report as bytes (CSV) ────────────────────────────────────
        public async Task<byte[]> GenerateReportAsync(string reportType, DateTime startDate, DateTime endDate)
        {
            string content = reportType switch
            {
                "Occupancy" => await GenerateOccupancyCsvAsync(startDate, endDate),
                "Revenue" => await GenerateRevenueCsvAsync(startDate, endDate),
                "Loyalty" => await GenerateLoyaltyCsvAsync(startDate, endDate),
                "Eco" => await GenerateEcoCsvAsync(startDate, endDate),
                _ => "Invalid report type"
            };
            return System.Text.Encoding.UTF8.GetBytes(content);
        }

        // ── Private CSV helpers ───────────────────────────────────────────────
        private async Task<string> GenerateOccupancyCsvAsync(DateTime start, DateTime end)
        {
            var report = await GetOccupancyReportAsync(start, end);
            var lines = new List<string> { "BookingReference,GuestName,CheckIn,CheckOut,Status" };
            lines.AddRange(report.Reservations.Select(r =>
                $"{r.BookingReference},{r.GuestName},{r.CheckIn:dd-MM-yyyy},{r.CheckOut:dd-MM-yyyy},{r.Status}"));
            return string.Join("\n", lines);
        }

        private async Task<string> GenerateRevenueCsvAsync(DateTime start, DateTime end)
        {
            var report = await GetRevenueReportAsync(start, end);
            var lines = new List<string> { "BookingReference,GuestName,Room,CheckIn,CheckOut,TotalPrice" };
            lines.AddRange(report.LineItems.Select(r =>
                $"{r.BookingReference},{r.GuestName},{r.RoomNo},{r.CheckIn:dd-MM-yyyy},{r.CheckOut:dd-MM-yyyy},{r.TotalPrice}"));
            return string.Join("\n", lines);
        }

        private async Task<string> GenerateLoyaltyCsvAsync(DateTime start, DateTime end)
        {
            var report = await GetLoyaltyReportAsync(start, end);
            var lines = new List<string> { "GuestName,Email,LoyaltyPoints,Tier" };
            lines.AddRange(report.TopGuests.Select(g =>
                $"{g.Name},{g.Email},{g.LoyaltyPoints},{g.Tier}"));
            return string.Join("\n", lines);
        }

        private async Task<string> GenerateEcoCsvAsync(DateTime start, DateTime end)
        {
            var report = await GetEcoReportAsync(start, end);
            return "TotalActions,TotalEcoPoints,TowelReuse,SkipHousekeeping,DigitalReceipt,WaterSavedLitres\n" +
                   $"{report.TotalActions},{report.TotalEcoPointsAwarded},{report.TowelReuseCount}," +
                   $"{report.SkipHousekeepingCount},{report.DigitalReceiptCount},{report.WaterSavedLitres}";
        }

    } // ← ReportService ends here

    // ── DTOs ──────────────────────────────────────────────────────────────────

    public class OccupancyReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRooms { get; set; }
        public int TotalReservations { get; set; }
        public double OccupancyPercent { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }

    public class RevenueReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public List<RevenueLineItem> LineItems { get; set; } = new();
    }

    public class RevenueLineItem
    {
        public string BookingReference { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public double TotalPrice { get; set; }
    }

    public class LoyaltyReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPointsEarned { get; set; }
        public int TotalPointsRedeemed { get; set; }
        public int TotalTransactions { get; set; }
        public List<GuestProfile> TopGuests { get; set; } = new();
    }

    public class EcoReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalActions { get; set; }
        public int TotalEcoPointsAwarded { get; set; }
        public int TowelReuseCount { get; set; }
        public int SkipHousekeepingCount { get; set; }
        public int DigitalReceiptCount { get; set; }
        public int WaterSavedLitres { get; set; }
        public int UniqueGuestsParticipated { get; set; }
    }

} // ← namespace ends here