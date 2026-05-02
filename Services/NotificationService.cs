using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class NotificationService
    {
        private readonly DatabaseContext _db;

        // Occupancy threshold — alert fires when below this percentage
        private const double LowOccupancyThreshold = 30.0;

        public NotificationService(DatabaseContext db)
        {
            _db = db;
        }

        // ── New booking alert ─────────────────────────────────────────────────
        // Call this immediately after a reservation is created
        public async Task<NotificationResult> SendNewBookingAlertAsync(int reservationId)
        {
            var reservation = await _db.GetReservationByIdAsync(reservationId);
            if (reservation == null)
                return new NotificationResult(false, "Reservation not found.");

            string title = "New Booking Confirmed";
            string message = $"{reservation.GuestName} booked room for " +
                             $"{reservation.CheckIn:dd MMM} – {reservation.CheckOut:dd MMM}. " +
                             $"Ref: {reservation.BookingReference}";

            ShowLocalNotification(title, message);

            return new NotificationResult(true, message);
        }

        // ── Check-in reminder ─────────────────────────────────────────────────
        // Call this on app startup each morning to notify about today's check-ins
        public async Task<NotificationResult> SendCheckinReminderAsync()
        {
            var reservations = await _db.GetAllReservationsAsync();

            var todayCheckins = reservations.Where(r =>
                r.Status == "Confirmed" &&
                r.CheckIn.Date == DateTime.Today).ToList();

            if (todayCheckins.Count == 0)
                return new NotificationResult(true, "No check-ins today.");

            string title = $"Today's Check-ins ({todayCheckins.Count})";
            string message = string.Join(", ", todayCheckins.Select(r => r.GuestName));

            ShowLocalNotification(title, message);

            return new NotificationResult(true, $"{todayCheckins.Count} check-in(s) today: {message}");
        }

        // ── Check-out reminder ────────────────────────────────────────────────
        public async Task<NotificationResult> SendCheckoutReminderAsync()
        {
            var reservations = await _db.GetAllReservationsAsync();

            var todayCheckouts = reservations.Where(r =>
                r.Status == "Confirmed" &&
                r.CheckOut.Date == DateTime.Today).ToList();

            if (todayCheckouts.Count == 0)
                return new NotificationResult(true, "No check-outs today.");

            string title = $"Today's Check-outs ({todayCheckouts.Count})";
            string message = string.Join(", ", todayCheckouts.Select(r => r.GuestName));

            ShowLocalNotification(title, message);

            return new NotificationResult(true, $"{todayCheckouts.Count} check-out(s) today: {message}");
        }

        // ── Low occupancy alert ───────────────────────────────────────────────
        // Call this from the dashboard after calculating occupancy
        public async Task<NotificationResult> SendOccupancyAlertAsync()
        {
            var rooms = await _db.GetAllRoomsAsync();
            var reservations = await _db.GetAllReservationsAsync();

            if (rooms.Count == 0)
                return new NotificationResult(false, "No rooms configured.");

            var occupiedToday = reservations.Where(r =>
                r.Status == "Confirmed" &&
                r.CheckIn.Date <= DateTime.Today &&
                r.CheckOut.Date > DateTime.Today).ToList();

            double occupancyPercent = (double)occupiedToday.Count / rooms.Count * 100;

            if (occupancyPercent >= LowOccupancyThreshold)
                return new NotificationResult(true, $"Occupancy is {occupancyPercent:F1}%. No alert needed.");

            string title = "Low Occupancy Alert";
            string message = $"Occupancy is at {occupancyPercent:F1}% — " +
                             $"{rooms.Count - occupiedToday.Count} rooms available. Consider offering a discount.";

            ShowLocalNotification(title, message);

            return new NotificationResult(true, message);
        }

        // ── Check if today has any events worth notifying ─────────────────────
        // Call this on app startup
        public async Task RunMorningChecksAsync()
        {
            await SendCheckinReminderAsync();
            await SendCheckoutReminderAsync();
            await SendOccupancyAlertAsync();
        }

        // ── Local notification display ────────────────────────────────────────
        // Uses built-in MAUI local notification approach
        // Replace with a plugin like Plugin.LocalNotification if needed
        private void ShowLocalNotification(string title, string message)
        {
#if ANDROID || IOS
            // For full push notifications install Plugin.LocalNotification:
            // dotnet add package Plugin.LocalNotification
            // Then replace this with:
            // LocalNotificationCenter.Current.Show(title, message, notificationId++);
            System.Diagnostics.Debug.WriteLine($"[NOTIFICATION] {title}: {message}");
#else
            // PWA / Windows — log to debug console
            System.Diagnostics.Debug.WriteLine($"[NOTIFICATION] {title}: {message}");
#endif
        }
    }

    // ── Result DTO ────────────────────────────────────────────────────────────
    public class NotificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public NotificationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}