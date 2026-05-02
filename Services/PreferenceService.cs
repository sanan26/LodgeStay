using LodgeStay.Models;
using LodgeStay.Data;
using System.Threading.Tasks;

namespace LodgeStay.Services
{
    public class PreferenceService
    {
        private readonly DatabaseContext _db;

        public PreferenceService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<bool> SavePreferencesAsync(
            int guestId,
            string preferredRoomType,
            string preferredBedType,
            string preferredFloor,
            string dietaryNotes,
            string amenityNotes)
        {
            var guest = await _db.GetGuestByIdAsync(guestId);
            if (guest == null) return false;

            guest.PreferredRoomType = preferredRoomType;
            guest.PreferredBedType = preferredBedType;
            guest.PreferredFloor = preferredFloor;
            guest.DietaryNotes = dietaryNotes;
            guest.AmenityNotes = amenityNotes;

            await _db.UpdateGuestAsync(guest);
            return true;
        }

        public async Task<GuestProfile?> GetPreferencesAsync(string email)
        {
            return await _db.GetGuestByEmailAsync(email);
        }
    }
}
