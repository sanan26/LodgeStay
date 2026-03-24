using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using LodgeStay.Data;
using LodgeStay.Models;
using System.Linq;

namespace LodgeStay.Services
{
    public class CalendarExportService
    {
        private readonly DatabaseContext _database;

        public CalendarExportService(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<string> GenerateIcsAsync(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow;
            var end = endDate ?? DateTime.UtcNow.AddDays(30);

            var all = await _database.GetAllReservationsAsync();

            var filtered = all.Where(r => r.CheckIn >= start && r.CheckOut <= end && r.Status == "Confirmed").ToList();
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//LodgeStay//EN");

            foreach (var r in filtered)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{r.BookingReference}");
                sb.AppendLine($"SUMMARY:{r.GuestName}");
                sb.AppendLine($"LOCATION:Room {r.Room_ID}");
                sb.AppendLine($"DTSTART:{r.CheckIn.ToString("yyyyMMdd")}");
                sb.AppendLine($"DTEND:{r.CheckOut.ToString("yyyyMMdd")}");
                sb.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
    }
}