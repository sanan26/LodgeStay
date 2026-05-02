using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class ReservationService
    {
        private readonly DatabaseContext _db;

        public ReservationService(DatabaseContext db)
        {
            _db = db;
        }

        public Task<List<Reservation>> GetReservationsAsync()
            => _db.GetAllReservationsAsync();

        public Task<Reservation?> GetReservationByReferenceAsync(string bookingReference)
            => _db.GetReservationByReferenceAsync(bookingReference);

        public Task UpdateReservationAsync(Reservation reservation)
            => _db.SaveReservationAsync(reservation);

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var ci = checkIn.Date;
            var co = checkOut.Date;

            var overlapping = await _db.GetOverlappingReservationsAsync(ci, co);
            var conflict = overlapping.Any(r => r.Room_ID == roomId);
            return !conflict;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            reservation.Created_at = DateTime.UtcNow;
            await _db.SaveReservationAsync(reservation);

            
        }

        public async Task CancelReservationAsync(string bookingReference)
        {
            var reservation = await _db.GetReservationByReferenceAsync(bookingReference);
            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                await _db.SaveReservationAsync(reservation);

                
            }
        }
    }
}
