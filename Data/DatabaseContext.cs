using System;
using SQLite;
using LodgeStay.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LodgeStay.Data
{
    public class DatabaseContext
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseContext(string dbpath) 
        {
            _database = new SQLiteAsyncConnection(dbpath);
        }

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Room>();
            await _database.CreateTableAsync<Reservation>();
            await _database.CreateTableAsync<OtpVerification>();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<int> InsertUserAsync(User user)
        {
            return await _database.InsertAsync(user);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _database.Table<User>()
                .ToListAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            if (user.User_ID != 0)
            {
                return await _database.UpdateAsync(user);
            }
            else
            {
                return await _database.InsertAsync(user);
            }
        }

        public async Task<int> SaveOtpAsync(OtpVerification otp)
        {
            if (otp.Id != 0)
            {
                return await _database.UpdateAsync(otp);
            }
            else
            {
                return await _database.InsertAsync(otp);
            }
        }

        public async Task<OtpVerification?> GetOtpByUserIdAsync(int userid)
        {
            return await _database.Table<OtpVerification>()
                .Where(o => o.User_Id == userid)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        } 
    }
}
