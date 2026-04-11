using System;
using System.Collections.Generic;
using LodgeStay.Data;
using LodgeStay.Models;
using System.Threading.Tasks;

namespace LodgeStay.Services
{
    public class GuestService
    {
        private readonly DatabaseContext _database;

        public GuestService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<bool> CreateGuestAsync(GuestProfile guest)
        {
            var existingGuest = await _database.GetGuestByEmailAsync(guest.Email);
            if (existingGuest != null)
            {
                return false;
            }
            else
            {
                await _database.InsertGuestAsync(guest);
                return true;
            }
        }

        public async Task<GuestProfile?> GetGuestByEmailAsync(string email)
        {
            return await _database.GetGuestByEmailAsync(email);
        }

        public async Task<int> UpdateGuestAsync(GuestProfile guest)
        {
            return await _database.UpdateGuestAsync(guest);
        }

        public async Task<List<GuestProfile>> SearchGuestAsync(string query)
        {
            return await _database.SearchGuestsAsync(query);
        }
    }
}
