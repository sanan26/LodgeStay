using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class GroupBookingService
    {
        private readonly ReservationService _reservationService;

        public GroupBookingService(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public int CalculateDiscount(int roomCount) =>
            roomCount >= 5 ? 20 : roomCount >= 3 ? 15 : roomCount >= 2 ? 10 : 0;

        public async Task<GroupBooking> CreateGroupBookingAsync(
            string groupName,
            string organizerEmail,
            List<int> roomIds,
            DateTime checkIn,
            DateTime checkOut,
            List<Room> availableRooms)
        {
            var masterRef = "GRP-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            int discount = CalculateDiscount(roomIds.Count);
            int nights = (checkOut - checkIn).Days;
            var reservations = new List<Reservation>();

            foreach (var roomId in roomIds)
            {
                var room = availableRooms.FirstOrDefault(r => r.Room_ID == roomId);
                if (room == null) continue;

                double price = room.Price * nights * (100 - discount) / 100.0;
                var reservation = new Reservation
                {
                    BookingReference = $"{masterRef}-R{roomId}",
                    GuestName = groupName,
                    GuestEmail = organizerEmail,
                    Room_ID = roomId,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    NumberOfGuests = room.Capacity,
                    TotalPrice = price,
                    Status = "Confirmed",
                    Created_at = DateTime.UtcNow
                };
                await _reservationService.AddReservationAsync(reservation);
                reservations.Add(reservation);
            }

            return new GroupBooking
            {
                MasterReference = masterRef,
                GroupName = groupName,
                OrganizerEmail = organizerEmail,
                Reservations = reservations,
                DiscountPercent = discount,
                TotalAmount = reservations.Sum(r => r.TotalPrice)
            };
        }

        public async Task<List<GroupBooking>> GetAllGroupBookingsAsync()
        {
            // Load all reservations with GRP- prefix and group them
            var all = await _reservationService.GetReservationsAsync();
            return all
                .Where(r => r.BookingReference.StartsWith("GRP-"))
                .GroupBy(r => r.BookingReference.Substring(0, 18)) // GRP-yyyyMMddHHmmss
                .Select(g => new GroupBooking
                {
                    MasterReference = g.Key,
                    GroupName = g.First().GuestName,
                    OrganizerEmail = g.First().GuestEmail,
                    Reservations = g.ToList(),
                    TotalAmount = g.Sum(r => r.TotalPrice)
                })
                .ToList();
        }
    }
}
