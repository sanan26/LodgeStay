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

        public async Task<GuestEcoStats> GetGuestEcoStatsAsync(int guestId)
        {
            var actions = await _db.GetEcoActionsByGuestAsync(guestId);
            int towelCount = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse);

            return new GuestEcoStats
            {
                TotalEcoPoints = actions.Sum(a => a.PointsAwarded),
                TowelReuseCount = towelCount,
                SkipHousekeepingCount = actions.Count(a => a.ActionType == EcoActionTypes.SkipHousekeeping),
                DigitalReceiptCount = actions.Count(a => a.ActionType == EcoActionTypes.DigitalReceipt),
                TotalActionsCount = actions.Count,
                ActionsCompleted = actions.Count,
                WaterSavedLitres = towelCount * 15,
                CO2SavedKg = Math.Round(towelCount * 0.5, 2),
                Actions = actions
            };
        }

        public async Task<PropertyEcoAggregate> GetPropertyEcoAggregateAsync()
        {
            var actions = await _db.GetAllEcoActionsAsync();
            int towelCount = actions.Count(a => a.ActionType == EcoActionTypes.TowelReuse);

            return new PropertyEcoAggregate
            {
                TotalParticipants = actions.Select(a => a.GuestId).Distinct().Count(),
                TotalEcoPoints = actions.Sum(a => a.PointsAwarded),
                TotalActionsRecorded = actions.Count,
                TotalWaterSaved = towelCount * 15,
                UniqueGuestsParticipated = actions.Select(a => a.GuestId).Distinct().Count(),
                ActionBreakdown = new Dictionary<string, int>
        {
            { EcoActionTypes.TowelReuse,       towelCount },
            { EcoActionTypes.SkipHousekeeping, actions.Count(a => a.ActionType == EcoActionTypes.SkipHousekeeping) },
            { EcoActionTypes.DigitalReceipt,   actions.Count(a => a.ActionType == EcoActionTypes.DigitalReceipt) }
        }
            };
        }

        public async Task<List<EcoAction>> GetReservationEcoActionsAsync(int reservationId)
        {
            return await _db.GetEcoActionsByReservationAsync(reservationId);
        }

        public async Task<List<EcoAction>> GetRecordedActionsAsync(int guestId, int reservationId)
        {
            var all = await _db.GetEcoActionsByReservationAsync(reservationId);
            return all.Where(a => a.GuestId == guestId).ToList();
        }
    }

    public class GuestEcoStats
    {
        public int TotalEcoPoints { get; set; }
        public int TotalActionsCount { get; set; }
        public int ActionsCompleted { get; set; } // alias for TotalActionsCount
        public int TowelReuseCount { get; set; }
        public int SkipHousekeepingCount { get; set; }
        public int DigitalReceiptCount { get; set; }
        public int WaterSavedLitres { get; set; }
        public double CO2SavedKg { get; set; }
        public List<EcoAction> Actions { get; set; } = new();
    }

    public class PropertyEcoAggregate
    {
        public int TotalParticipants { get; set; }
        public int TotalEcoPoints { get; set; }
        public int TotalActionsRecorded { get; set; }
        public int TotalWaterSaved { get; set; }
        public int UniqueGuestsParticipated { get; set; }
        public Dictionary<string, int> ActionBreakdown { get; set; } = new();
    }
}