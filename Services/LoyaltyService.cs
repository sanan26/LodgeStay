using System;
using System.Threading.Tasks;
using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class LoyaltyService
    {
        private readonly DatabaseContext _database;

        public LoyaltyService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<bool> EarnPointsAsync(string email, int nights)
        {
            var guest = await _database.GetGuestByEmailAsync(email);
            if (guest == null)
            {
                return false;
            }
            else
            {
                guest.LoyaltyPoints += nights * 10; // Earn 10 points per night
                await _database.UpdateGuestAsync(guest);
                return true;
            }
        }

        public async Task<double> RedeemPointsAsync(string email, int pointsToRedeem)
        {
            var guest = await _database.GetGuestByEmailAsync(email);
            if (guest == null)
            {
                return 0.0;
            }
            if (guest.LoyaltyPoints >= pointsToRedeem && pointsToRedeem >= 100)
            {
                guest.LoyaltyPoints -= pointsToRedeem;
                await _database.UpdateGuestAsync(guest);
                return (double)pointsToRedeem; // Redeem 100 points for pkr100 discount
            }
            return 0.0;
        }

        public async Task<string> GetTierAsync(string email)
        {
            var guest = await _database.GetGuestByEmailAsync(email);
            if (guest == null)
            {
                return "No Tier";
            }
            if (guest.LoyaltyPoints >= 5000)
            {
                return "Gold";
            }
            else if (guest.LoyaltyPoints >= 1000 && guest.LoyaltyPoints < 5000)
            {
                return "Silver";
            }
            else if (guest.LoyaltyPoints >= 0 && guest.LoyaltyPoints < 1000)
            {
                return "Bronze";
            }
            return "No Tier";
        }
    }
}
