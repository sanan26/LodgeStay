using LodgeStay.Data;
using LodgeStay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LodgeStay.Services
{
    public class RoomService
    {
        private readonly DatabaseContext _database;

        public RoomService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<List<Room>> GetAvailableRoomsAsync (DateTime checkin, DateTime checkout, int guests)
        {
            if (guests >= 1)
            {
                var allRooms = await GetAllRoomsAsync();
                var overlapping = await _database.GetOverlappingReservationsAsync(checkin, checkout);
                var bookedRoomids = overlapping
                    .Select(r => r.Room_ID)
                    .ToList();
                return allRooms
                    .Where(r => r.Capacity >= guests)
                    .Where(r => !bookedRoomids.Contains(r.Room_ID))
                    .ToList();
            }
            else
            {
                return new List<Room>();
            }
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _database.GetAllRoomsAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int roomid)
        {
            return await _database.GetRoomByIdAsync(roomid);
        }
    }
}
