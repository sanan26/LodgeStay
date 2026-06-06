using LodgeStay.Models;
using LodgeStay.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LodgeStay.Services
{
    public class LoyaltyService
    {
        private readonly DatabaseContext _db;

        public LoyaltyService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<bool> EarnPointsAsync(int guestId, int nights)
        {
            var guest = await _db.GetGuestByIdAsync(guestId);
            if (guest == null) return false;

            guest.LoyaltyPoints += nights * 10;
            guest.Tier = CalculateTier(guest.LoyaltyPoints);
            await _db.UpdateGuestAsync(guest);

            await _db.InsertLoyaltyHistoryAsync(new LoyaltyHistory
            {
                GuestId = guestId,
                Description = $"Stay completed — {nights} nights",
                Points = nights * 10,
                IsEarn = true
            });

            return true;
        }

        public async Task<(bool success, int discountAmount)> RedeemPointsAsync(int guestId, int points)
        {
            if (points < 100) return (false, 0);

            var guest = await _db.GetGuestByIdAsync(guestId);
            if (guest == null || guest.LoyaltyPoints < points) return (false, 0);

            guest.LoyaltyPoints -= points;
            guest.Tier = CalculateTier(guest.LoyaltyPoints);
            await _db.UpdateGuestAsync(guest);

            await _db.InsertLoyaltyHistoryAsync(new LoyaltyHistory
            {
                GuestId = guestId,
                Description = $"Redeemed — Rs. {points} discount",
                Points = points,
                IsEarn = false
            });

            return (true, points);
        }

        public async Task<List<LoyaltyHistory>> GetHistoryAsync(int guestId)
        {
            return await _db.GetLoyaltyHistoryByGuestAsync(guestId);
        }

        public string CalculateTier(int points) =>
            points >= 5000 ? "Gold" : points >= 1000 ? "Silver" : "Bronze";
    }
}
