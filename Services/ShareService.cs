using System.Threading.Tasks;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class ShareService
    {
        public async Task ShareConfirmationAsync(Reservation reservation)
        {
            string message = $"🏨 LodgeStay Booking Confirmation\n\n" +
                 $"Booking Reference: {reservation.BookingReference}\n" +
                 $"Guest Name: {reservation.GuestName}\n" +
                 $"Guest Email: {reservation.GuestEmail}\n" +
                 $"Guest Phone: {reservation.GuestPhone}\n" +
                 $"Check-in: {reservation.CheckIn:MMMM dd, yyyy}\n" +
                 $"Check-out: {reservation.CheckOut:MMMM dd, yyyy}\n" +
                 $"Total Price: PKR {reservation.TotalPrice}\n" +
                 $"Status: {reservation.Status}\n\n" +
                 $"Thank you for choosing LodgeStay!";

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "LodgeStay Booking Confirmation",
                Text = message
            });
        }
    }
}