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
            await _database.CreateTableAsync<EcoAction>();
            await _database.CreateTableAsync<LocalDeal>();
            await _database.CreateTableAsync<ConciergeRule>();

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

            var deals = await _database.Table<LocalDeal>().ToListAsync();
            if (deals.Count == 0)
            {
                await _database.InsertAllAsync(new List<LocalDeal>
                {
                    new LocalDeal { PartnerName = "Lahore Food Street",  Description = "10% off dinner for lodge guests", Price = 0,    IsActive = true },
                    new LocalDeal { PartnerName = "City Tour Services",  Description = "Half-day city tour PKR 1500/person", Price = 1500, IsActive = true },
                    new LocalDeal { PartnerName = "Badshahi Mosque Tour",Description = "Guided heritage tour PKR 500/person", Price = 500,  IsActive = true },
                    new LocalDeal { PartnerName = "Airport Transfers",   Description = "One-way airport drop PKR 800",        Price = 800,  IsActive = true },
                    new LocalDeal { PartnerName = "Spa & Wellness",      Description = "60-min massage PKR 2000/person",      Price = 2000, IsActive = true },
                });
            }

            var rules = await _database.Table<ConciergeRule>().ToListAsync();
            if (rules.Count == 0)
            {
                await _database.InsertAllAsync(new List<ConciergeRule>
                {
                    new ConciergeRule
                    {
                        Keywords         = "checkin,check in,arrival,arrive",
                        Category         = ConciergeCategories.CheckIn,
                        ResponseText     = "Check-in time is 2:00 PM. Early check-in available on request subject to availability.",
                        ResponseTemplate = "Dear Guest, your check-in time is 2:00 PM. Please contact us if you require early check-in.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "checkout,check out,departure,leave",
                        Category         = ConciergeCategories.CheckOut,
                        ResponseText     = "Check-out time is 11:00 AM. Late check-out until 2:00 PM available for PKR 500.",
                        ResponseTemplate = "Dear Guest, check-out is at 11:00 AM. Late check-out until 2:00 PM is available for PKR 500.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "cancel,cancellation,refund",
                        Category         = ConciergeCategories.Cancellation,
                        ResponseText     = "Free cancellation up to 24 hours before check-in. After that a one-night charge applies.",
                        ResponseTemplate = "Dear Guest, cancellations made 24+ hours before check-in are fully refunded. Late cancellations incur a one-night charge.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "room,single,double,suite,family,types",
                        Category         = ConciergeCategories.RoomTypes,
                        ResponseText     = "We offer Single (PKR 3000), Double (PKR 5000), Suite (PKR 9000), and Family rooms (PKR 12000).",
                        ResponseTemplate = "Dear Guest, our room options are: Single PKR 3000, Double PKR 5000, Suite PKR 9000, Family PKR 12000 per night.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "breakfast,food,meal,dining",
                        Category         = ConciergeCategories.Breakfast,
                        ResponseText     = "Breakfast is served from 7:00 AM to 10:00 AM in the dining area. Continental and Pakistani options available.",
                        ResponseTemplate = "Dear Guest, breakfast is served 7:00–10:00 AM with continental and Pakistani options.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "wifi,wi-fi,internet,password,network",
                        Category         = ConciergeCategories.WiFi,
                        ResponseText     = "Free Wi-Fi is available throughout the property. Network: LodgeStay_Guest. Ask reception for the password.",
                        ResponseTemplate = "Dear Guest, free Wi-Fi is available lodge-wide. Network: LodgeStay_Guest. Please ask reception for the password.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "price,cost,rate,how much,charges",
                        Category         = ConciergeCategories.Pricing,
                        ResponseText     = "Room rates start from PKR 3000/night. Group discounts and loyalty redemptions available.",
                        ResponseTemplate = "Dear Guest, our rates start from PKR 3000/night. Group discounts and loyalty points redemption are available.",
                        IsActive         = true
                    },
                    new ConciergeRule
                    {
                        Keywords         = "parking,car,vehicle",
                        Category         = ConciergeCategories.General,
                        ResponseText     = "Free parking is available on premises for all guests.",
                        ResponseTemplate = "Dear Guest, free parking is available on premises.",
                        IsActive         = true
                    },
    });
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

        public async Task<int> InsertEcoActionAsync(EcoAction action)
        {
            action.RecordedAt = DateTime.Now;
            return await _database.InsertAsync(action);
        }

        public async Task<EcoAction?> GetEcoActionAsync(int guestId, int reservationId, string actionType)
        {
            return await _database.Table<EcoAction>()
                            .Where(e => e.GuestId == guestId
                                     && e.ReservationId == reservationId
                                     && e.ActionType == actionType)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<EcoAction>> GetEcoActionsByGuestAsync(int guestId)
        {
            return await _database.Table<EcoAction>()
                            .Where(e => e.GuestId == guestId)
                            .OrderByDescending(e => e.RecordedAt)
                            .ToListAsync();
        }

        public async Task<List<EcoAction>> GetAllEcoActionsAsync()
        {
            return await _database.Table<EcoAction>()
                            .OrderByDescending(e => e.RecordedAt)
                            .ToListAsync();
        }

        public async Task<List<EcoAction>> GetEcoActionsByReservationAsync(int reservationId)
        {
            return await _database.Table<EcoAction>()
                            .Where(e => e.ReservationId == reservationId)
                            .ToListAsync();
        }

        public async Task<List<LocalDeal>> GetActiveDealsAsync()
        {
            return await _database.Table<LocalDeal>()
                            .Where(d => d.IsActive == true)
                            .ToListAsync();
        }

        public async Task<List<LocalDeal>> GetAllDealsAsync()
        {
            return await _database.Table<LocalDeal>().ToListAsync();
        }

        public async Task<LocalDeal?> GetDealByIdAsync(int id)
        {
            return await _database.Table<LocalDeal>()
                            .Where(d => d.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> InsertDealAsync(LocalDeal deal)
        {
            return await _database.InsertAsync(deal);
        }

        public async Task<int> UpdateDealAsync(LocalDeal deal)
        {
            return await _database.UpdateAsync(deal);
        }

        public async Task<List<ConciergeRule>> GetAllConciergeRulesAsync()
        {
            return await _database.Table<ConciergeRule>()
                            .Where(r => r.IsActive == true)
                            .ToListAsync();
        }

        public async Task<List<ConciergeRule>> GetConciergeRulesByCategoryAsync(string category)
        {
            return await _database.Table<ConciergeRule>()
                            .Where(r => r.Category == category && r.IsActive == true)
                            .ToListAsync();
        }

        public async Task<int> InsertConciergeRuleAsync(ConciergeRule rule)
        {
            return await _database.InsertAsync(rule);
        }

        public async Task<int> UpdateConciergeRuleAsync(ConciergeRule rule)
        {
            return await _database.UpdateAsync(rule);
        }

        public async Task<ConciergeRule?> GetConciergeRuleByIdAsync(int id)
        {
            return await _database.Table<ConciergeRule>()
                            .Where(r => r.Id == id)
                            .FirstOrDefaultAsync();
        }
    }
}
