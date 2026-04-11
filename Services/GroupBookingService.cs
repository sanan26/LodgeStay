using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class GroupBookingService
    {
        private readonly DatabaseContext _database;

        public GroupBookingService(DatabaseContext database)
        {
            _database = database;
        }

        public string GenerateMasterReference()
        {
            var reference = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            return "GRP-" + reference;
        }

        public async Task<bool> CreateGroupBookingAsync(List<int> roomIds, string guestemail, string guestname, string guestphone, DateTime checkin, DateTime checkout)
        {
            var masterReference = GenerateMasterReference();
            var discountpercent = Math.Min(roomIds.Count * 5, 30);
            foreach (var roomId in roomIds)
            {
                var room = await _database.GetRoomByIdAsync(roomId);
                if (room == null)
                {
                    return false;
                }
                var totalPrice = room.Price * (checkout - checkin).Days * (1 - discountpercent / 100.0);
                var reservation = new Models.Reservation
                { 
                    Room_ID = roomId,
                    GuestEmail = guestemail,
                    GuestName = guestname,
                    GuestPhone = guestphone,
                    CheckIn = checkin,
                    CheckOut = checkout,
                    TotalPrice = totalPrice,
                    Status = "Confirmed",
                    BookingReference = masterReference
                };
                await _database.SaveReservationAsync(reservation);
            }
            return true;
        }
    }
}
