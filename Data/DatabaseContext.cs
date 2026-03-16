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

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _database.Table<User>()
                .Where(u => u.User_ID == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _database.Table<Room>()
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int roomid)
        {
            return await _database.Table<Room>()
                .Where(r => r.Room_ID == roomid)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Reservation>> GetOverlappingReservationsAsync(DateTime checkin, DateTime checkout){
            return await _database.Table<Reservation>()
                .Where(r => r.CheckIn < checkout && r.CheckOut > checkin && r.Status == "Confirmed")
                .ToListAsync();
        }  

        public async Task<int> SaveReservationAsync(Reservation reservation)
        {
            if (reservation.Reservation_ID != 0)
            {
                return await _database.UpdateAsync(reservation);
            }
            else
            {
                return await _database.InsertAsync(reservation);
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _database.Table<Reservation>().ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _database.Table<Reservation>()
                .Where(r => r.Reservation_ID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Reservation>> GetReservationsByGuestEmailAsync(string email)
        {
            return await _database.Table<Reservation>()
                .Where(r => r.GuestEmail == email)
                .ToListAsync();
        }

        public async Task<int> SaveRoomAsync(Room room)
        {
            return await _database.UpdateAsync(room);
        }
    }
}
