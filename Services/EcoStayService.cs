using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class EcoStayService
    {
        private readonly DatabaseContext _db;

        public EcoStayService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<(bool Success, string Message, int PointsAwarded)> RecordActionAsync(
            int guestId, int reservationId, string actionType)
        {
            int points = EcoActionPoints.GetPoints(actionType);
            if (points == 0)
                return (false, "Invalid eco-action type.", 0);

            var existing = await _db.GetEcoActionAsync(guestId, reservationId, actionType);
            if (existing != null)
                return (false, "This eco-action is already recorded for this stay.", 0);

            var guest = await _db.GetGuestByIdAsync(guestId);
            if (guest == null)
                return (false, "Guest profile not found.", 0);

            var action = new EcoAction
            {
                GuestId = guestId,
                ReservationId = reservationId,
                ActionType = actionType,
                PointsAwarded = points
            };
            await _db.InsertEcoActionAsync(action);

            guest.EcoPoints += points;
            await _db.UpdateGuestAsync(guest);

            return (true, $"Eco-action recorded! +{points} eco-points awarded.", points);
        }

        public async Task<EcoGuestStats> GetGuestEcoStatsAsync(int guestId)
        {
            var actions = await _db.GetEcoActionsByGuestAsync(guestId);

            return new EcoGuestStats
            {
                TotalEcoPoints = actions.Sum(a => a.PointsAwarded),
                TowelReuseCount = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse),
                SkipHousekeepingCount = actions.Count(a => a.ActionType == EcoActionTypes.SkipHousekeeping),
                DigitalReceiptCount = actions.Count(a => a.ActionType == EcoActionTypes.DigitalReceipt),
                TotalActionsCount = actions.Count,
                WaterSavedLitres = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse) * 15
            };
        }

        public async Task<EcoPropertyStats> GetPropertyEcoAggregateAsync()
        {
            var actions = await _db.GetAllEcoActionsAsync();

            return new EcoPropertyStats
            {
                TotalParticipations = actions.Count,
                TotalEcoPointsAwarded = actions.Sum(a => a.PointsAwarded),
                TowelReuseCount = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse),
                SkipHousekeepingCount = actions.Count(a => a.ActionType == EcoActionTypes.SkipHousekeeping),
                DigitalReceiptCount = actions.Count(a => a.ActionType == EcoActionTypes.DigitalReceipt),
                WaterSavedLitres = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse) * 15,
                UniqueGuestsParticipated = actions.Select(a => a.GuestId).Distinct().Count()
            };
        }

        public async Task<List<EcoAction>> GetReservationEcoActionsAsync(int reservationId)
        {
            return await _db.GetEcoActionsByReservationAsync(reservationId);
        }
    }

    public class EcoGuestStats
    {
        public int TotalEcoPoints { get; set; }
        public int TotalActionsCount { get; set; }
        public int TowelReuseCount { get; set; }
        public int SkipHousekeepingCount { get; set; }
        public int DigitalReceiptCount { get; set; }
        public int WaterSavedLitres { get; set; }
    }

    public class EcoPropertyStats
    {
        public int TotalParticipations { get; set; }
        public int TotalEcoPointsAwarded { get; set; }
        public int TowelReuseCount { get; set; }
        public int SkipHousekeepingCount { get; set; }
        public int DigitalReceiptCount { get; set; }
        public int WaterSavedLitres { get; set; }
        public int UniqueGuestsParticipated { get; set; }
    }
}