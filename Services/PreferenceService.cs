using System;
using LodgeStay.Data;
using LodgeStay.Models;
using System.Threading.Tasks;

namespace LodgeStay.Services
{
    public class PreferenceService
    {
        private readonly DatabaseContext _database;

        public PreferenceService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<GuestProfile?> GetPreferencesAsync(string email)
        {
            return await _database.GetGuestByEmailAsync(email);
        }

        public async Task<bool> SavePreferencesAsync(string email, string roomType, string floor, string bedType, string dietary, string amenity)
        {
            var existingperference = await _database.GetGuestByEmailAsync(email);
            if (existingperference == null)
            {
                return false;
            }
            else
            {
                existingperference.PreferredBedType = bedType;
                existingperference.PreferredFloor = floor;
                existingperference.PreferredRoomType = roomType;
                existingperference.DietaryPreferences = dietary;
                existingperference.AmenityPreferences = amenity;
                await _database.UpdateGuestAsync(existingperference);
                return true;
            }
        }

    }
}
