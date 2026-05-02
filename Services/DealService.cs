using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class DealService
    {
        private readonly DatabaseContext _db;

        public DealService(DatabaseContext db)
        {
            _db = db;
        }

        // ── Get all active deals — shown to staff during booking ──────────────
        public async Task<List<LocalDeal>> GetActiveDealsAsync()
        {
            return await _db.GetActiveDealsAsync();
        }

        // ── Get all deals — shown to manager in admin panel ───────────────────
        public async Task<List<LocalDeal>> GetAllDealsAsync()
        {
            return await _db.GetAllDealsAsync();
        }

        // ── Add a new deal — manager only ─────────────────────────────────────
        public async Task<(bool Success, string Message)> AddDealAsync(
            string partnerName, string description, double price)
        {
            if (string.IsNullOrWhiteSpace(partnerName))
                return (false, "Partner name is required.");

            if (string.IsNullOrWhiteSpace(description))
                return (false, "Description is required.");

            if (price < 0)
                return (false, "Price cannot be negative.");

            var deal = new LocalDeal
            {
                PartnerName = partnerName.Trim(),
                Description = description.Trim(),
                Price = price,
                IsActive = true
            };

            await _db.InsertDealAsync(deal);
            return (true, "Deal added successfully.");
        }

        // ── Toggle deal active/inactive — manager only ────────────────────────
        public async Task<(bool Success, string Message)> ToggleDealAsync(int dealId)
        {
            var deal = await _db.GetDealByIdAsync(dealId);
            if (deal == null)
                return (false, "Deal not found.");

            deal.IsActive = !deal.IsActive;
            await _db.UpdateDealAsync(deal);

            string status = deal.IsActive ? "activated" : "deactivated";
            return (true, $"Deal {status} successfully.");
        }

        // ── Update deal details — manager only ────────────────────────────────
        public async Task<(bool Success, string Message)> UpdateDealAsync(
            int dealId, string partnerName, string description, double price)
        {
            var deal = await _db.GetDealByIdAsync(dealId);
            if (deal == null)
                return (false, "Deal not found.");

            if (string.IsNullOrWhiteSpace(partnerName))
                return (false, "Partner name is required.");

            if (string.IsNullOrWhiteSpace(description))
                return (false, "Description is required.");

            if (price < 0)
                return (false, "Price cannot be negative.");

            deal.PartnerName = partnerName.Trim();
            deal.Description = description.Trim();
            deal.Price = price;

            await _db.UpdateDealAsync(deal);
            return (true, "Deal updated successfully.");
        }
    }
}