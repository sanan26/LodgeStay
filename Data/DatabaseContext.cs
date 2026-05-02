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
            await _database.CreateTableAsync<GuestProfile>();
            await _database.CreateTableAsync<LoyaltyHistory>();

            // Seed rooms if none exist
            var rooms = await _database.Table<Room>().ToListAsync();
            if (rooms.Count == 0)
            {
                await _database.InsertAllAsync(new List<Room>
        {
            new Room { RoomNo = "101", Room_Type = "Single", Capacity = 1, Price = 3000, Status = "Available", IsEcoCertified = true },
            new Room { RoomNo = "102", Room_Type = "Double", Capacity = 2, Price = 5000, Status = "Available", IsEcoCertified = false },
            new Room { RoomNo = "103", Room_Type = "Suite", Capacity = 3, Price = 9000, Status = "Available", IsEcoCertified = true },
            new Room { RoomNo = "104", Room_Type = "Family", Capacity = 4, Price = 12000, Status = "Available", IsEcoCertified = false },
            new Room { RoomNo = "105", Room_Type = "Double", Capacity = 2, Price = 5500, Status = "Available", IsEcoCertified = true },
            new Room { RoomNo = "106", Room_Type = "Single", Capacity = 1, Price = 3500, Status = "Available", IsEcoCertified = false },
        });
            }
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

        public async Task<List<Reservation>> GetOverlappingReservationsAsync(DateTime checkin, DateTime checkout)
        {
            var all = await _database.Table<Reservation>()
                .Where(r => r.Status == "Confirmed")
                .ToListAsync();

            return all.Where(r =>
                r.CheckIn.Date < checkout.Date &&
                r.CheckOut.Date > checkin.Date
            ).ToList();
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

        public async Task<GuestProfile?> GetGuestByEmailAsync(string email)
        {
            return await _database.Table<GuestProfile>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<GuestProfile?> GetGuestByIdAsync(int id)
        {
            return await _database.Table<GuestProfile>()
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> InsertGuestAsync(GuestProfile guest)
        {
            return await _database.InsertAsync(guest);
        }

        public async Task<int> UpdateGuestAsync(GuestProfile guest)
        {
            return await _database.UpdateAsync(guest);
        }
        public async Task<List<GuestProfile>> SearchGuestsAsync(string query)
        {
            var all = await _database.Table<GuestProfile>().ToListAsync();
            return all.Where(g => g.Name.Contains(query) || g.Email.Contains(query))
                      .ToList();
        }
        public async Task<Reservation?> GetReservationByReferenceAsync(string reference)
        {
            return await _database.Table<Reservation>()
                .Where(r => r.BookingReference == reference)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GuestProfile>> GetAllGuestsAsync()
        {
            return await _database.Table<GuestProfile>().ToListAsync();
        }

        public async Task<int> InsertLoyaltyHistoryAsync(LoyaltyHistory entry)
        {
            return await _database.InsertAsync(entry);
        }

        public async Task<List<LoyaltyHistory>> GetLoyaltyHistoryByGuestAsync(int guestId)
        {
            return await _database.Table<LoyaltyHistory>()
                .Where(h => h.GuestId == guestId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}
