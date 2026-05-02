using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class GuestService
    {
        private readonly DatabaseContext _db;

        public GuestService(DatabaseContext db)
        {
            _db = db;
        }

        public Task<List<GuestProfile>> GetAllGuestsAsync()
            => _db.GetAllGuestsAsync();

        public Task<GuestProfile?> GetGuestByEmailAsync(string email)
            => _db.GetGuestByEmailAsync(email);

        public Task<GuestProfile?> GetGuestByIdAsync(int id)
            => _db.GetGuestByIdAsync(id);

        public Task<List<GuestProfile>> SearchGuestsAsync(string query)
            => _db.SearchGuestsAsync(query);

        public async Task<bool> CreateGuestAsync(GuestProfile guest)
        {
            var existing = await _db.GetGuestByEmailAsync(guest.Email);
            if (existing != null) return false;

            guest.CreatedAt = DateTime.UtcNow;
            guest.LoyaltyTier = "Bronze";
            await _db.InsertGuestAsync(guest);
            return true;
        }

        public async Task<bool> UpdateGuestAsync(GuestProfile guest)
        {
            await _db.UpdateGuestAsync(guest);
            return true;
        }
    }
}
