using LodgeStay.Data;
using LodgeStay.Models;
using System;

namespace LodgeStay.Services
{
    public class OtpService
    {
        private readonly DatabaseContext _database;

        public OtpService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<string> GenerateOtpAsync(int userId)
        {
            var random = new Random();
            string code = random.Next(100000, 999999).ToString();
            OtpVerification obj = new OtpVerification();
            obj.Code = code;
            obj.User_Id = userId;
            await _database.SaveOtpAsync(obj);
            return code;
        }

        public async Task<bool> VerifyOtpAsync(int userId, string code)
        {
            var otp = await _database.GetOtpByUserIdAsync(userId);
            if (otp == null)
            {
                return false; // No OTP found for user
            }
            else if (otp.Code == code && otp.ExpiresAt > DateTime.UtcNow && !otp.IsUsed)
            {
                otp.IsUsed = true;
                await _database.SaveOtpAsync(otp);
                var user = await _database.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return false; // User not found
                }
                user.IsVerified = true;
                await _database.SaveUserAsync(user);
                return true; // OTP is valid
            }
            return false; // OTP is invalid or expired
        }
    }
}
