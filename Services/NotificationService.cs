using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class NotificationService
    {
        private readonly DatabaseContext _db;
        private const double LowOccupancyThreshold = 30.0;
        private int _notificationCounter = 0;

        public event Action<InAppNotification>? OnInAppNotification;

        public NotificationService(DatabaseContext db)
        {
            _db = db;
        }

        // ── New booking alert ─────────────────────────────────────────────────
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

        // ── Low occupancy alert (no argument) ────────────────────────────────
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

        // ── Low occupancy alert (with argument — called from dashboard) ───────
        public async Task<NotificationResult> SendOccupancyAlertAsync(double occupancyPercent)
        {
            if (occupancyPercent >= LowOccupancyThreshold)
                return new NotificationResult(true, $"Occupancy {occupancyPercent:F1}%. No alert needed.");

            string title = "Low Occupancy Alert";
            string message = $"Occupancy is at {occupancyPercent:F1}% — consider offering a discount.";
            ShowLocalNotification(title, message);
            return new NotificationResult(true, message);
        }

        // ── Morning checks ────────────────────────────────────────────────────
        public async Task RunMorningChecksAsync()
        {
            await SendCheckinReminderAsync();
            await SendCheckoutReminderAsync();
            await SendOccupancyAlertAsync();
        }

        // ── Preferences ───────────────────────────────────────────────────────
        public async Task<NotificationPreferences> GetPreferencesAsync(int userId)
        {
            return await Task.FromResult(new NotificationPreferences());
        }

        public async Task SavePreferencesAsync(int userId, NotificationPreferences prefs)
        {
            await Task.CompletedTask;
        }

        // ── Browser permission ────────────────────────────────────────────────
        public async Task<bool> GetBrowserPermissionStatusAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> RequestBrowserPermissionAsync()
        {
            return await Task.FromResult(true);
        }

        // ── Local notification display ────────────────────────────────────────
        private void ShowLocalNotification(string title, string message)
        {
            var notification = new InAppNotification
            {
                Id = ++_notificationCounter,
                Title = title,
                Message = message,
                IsVisible = true,
                IsExiting = false,
                Type = "info"
            };
            OnInAppNotification?.Invoke(notification);
            System.Diagnostics.Debug.WriteLine($"[NOTIFICATION] {title}: {message}");
        }

    } // ← NotificationService ends here

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

} // ← namespace ends here