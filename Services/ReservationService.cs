using LodgeStay.Data;
using LodgeStay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LodgeStay.Services
{
    public class ReservationService
    {
        private readonly DatabaseContext _database;
        private readonly RoomService _roomService;

        public ReservationService(DatabaseContext database, RoomService roomService)
        {
            _database = database;
            _roomService = roomService;
        }

        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            if (reservation.CheckOut > reservation.CheckIn)
            {
                bool isavaiable = await _roomService.IsRoomAvailableAsync(reservation.Room_ID, reservation.CheckIn, reservation.CheckOut);
                if (isavaiable)
                {
                    var room = await _database.GetRoomByIdAsync(reservation.Room_ID);
                    if (room != null)
                    {
                        int nights = (reservation.CheckOut - reservation.CheckIn).Days;
                        reservation.TotalPrice = nights * room.Price;
                        await _database.SaveReservationAsync(reservation);
                        room.Status = "Reserved";
                        await _database.SaveRoomAsync(room);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationid)
        {
            var reservation = await _database.GetReservationByIdAsync(reservationid);
            if (reservation != null)
            {
                reservation.Status = "Cancelled";
                var room = await _database.GetRoomByIdAsync(reservation.Room_ID);
                if (room == null)
                {
                    return false;
                }
                room.Status = "Available";
                await _database.SaveReservationAsync(reservation);
                await _database.SaveRoomAsync(room);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _database.GetAllReservationsAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int reservationid)
        {
            return await _database.GetReservationByIdAsync(reservationid);
        }

        public async Task<List<Reservation>> GetReservationsByGuestEmailAsync(string email)
        {
            return await _database.GetReservationsByGuestEmailAsync(email);

        }

        public async Task<List<Reservation>> GetReservationsAsync()
        {
            return await GetAllReservationsAsync();
        }

        public async Task<bool> AddReservationAsync(Reservation reservation)
        {
            return await CreateReservationAsync(reservation);
        }

        public async Task<List<Reservation>> GetReservationByReferenceAsync(string reference)
        {
            var all = await _database.GetAllReservationsAsync();
            return all.Where(r => r.BookingReference == reference).ToList();
        }
    }
}
