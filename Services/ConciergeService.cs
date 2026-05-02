using LodgeStay.Data;
using LodgeStay.Models;

namespace LodgeStay.Services
{
    public class ConciergeService
    {
        private readonly DatabaseContext _db;

        public ConciergeService(DatabaseContext db)
        {
            _db = db;
        }

        // ── Keyword search — main staff-facing feature ────────────────────────
        public async Task<ConciergeResult> SearchByKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new ConciergeResult(false, "Please enter a keyword to search.", null);

            var allRules = await _db.GetAllConciergeRulesAsync();
            string search = keyword.Trim().ToLower();

            // Match against any keyword in the comma-separated Keywords field
            var matched = allRules.Where(r =>
                r.Keywords.Split(',')
                          .Any(k => k.Trim().ToLower().Contains(search) ||
                                    search.Contains(k.Trim().ToLower())))
                .ToList();

            if (matched.Count == 0)
                return new ConciergeResult(false, "No results found. Try a different keyword.", null);

            // Return the best match (first matched rule)
            return new ConciergeResult(true, "Match found.", matched.First());
        }

        // ── Browse by category ────────────────────────────────────────────────
        public async Task<List<ConciergeRule>> GetByCategoryAsync(string category)
        {
            return await _db.GetConciergeRulesByCategoryAsync(category);
        }

        // ── Get all active rules ──────────────────────────────────────────────
        public async Task<List<ConciergeRule>> GetAllRulesAsync()
        {
            return await _db.GetAllConciergeRulesAsync();
        }

        // ── Get all available categories ──────────────────────────────────────
        public List<string> GetAllCategories()
        {
            return new List<string>
            {
                ConciergeCategories.CheckIn,
                ConciergeCategories.CheckOut,
                ConciergeCategories.Cancellation,
                ConciergeCategories.RoomTypes,
                ConciergeCategories.Breakfast,
                ConciergeCategories.WiFi,
                ConciergeCategories.Pricing,
                ConciergeCategories.General
            };
        }

        // ── Add a new rule — manager only ─────────────────────────────────────
        public async Task<(bool Success, string Message)> AddRuleAsync(
            string keywords, string category, string responseText, string responseTemplate)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return (false, "Keywords are required.");

            if (string.IsNullOrWhiteSpace(category))
                return (false, "Category is required.");

            if (string.IsNullOrWhiteSpace(responseText))
                return (false, "Response text is required.");

            var rule = new ConciergeRule
            {
                Keywords = keywords.Trim().ToLower(),
                Category = category.Trim(),
                ResponseText = responseText.Trim(),
                ResponseTemplate = responseTemplate?.Trim() ?? string.Empty,
                IsActive = true
            };

            await _db.InsertConciergeRuleAsync(rule);
            return (true, "Concierge rule added successfully.");
        }

        // ── Update existing rule — manager only ───────────────────────────────
        public async Task<(bool Success, string Message)> UpdateRuleAsync(
            int ruleId, string keywords, string category,
            string responseText, string responseTemplate)
        {
            var rule = await _db.GetConciergeRuleByIdAsync(ruleId);
            if (rule == null)
                return (false, "Rule not found.");

            if (string.IsNullOrWhiteSpace(keywords))
                return (false, "Keywords are required.");

            if (string.IsNullOrWhiteSpace(responseText))
                return (false, "Response text is required.");

            rule.Keywords = keywords.Trim().ToLower();
            rule.Category = category.Trim();
            rule.ResponseText = responseText.Trim();
            rule.ResponseTemplate = responseTemplate?.Trim() ?? string.Empty;

            await _db.UpdateConciergeRuleAsync(rule);
            return (true, "Concierge rule updated successfully.");
        }

        // ── Deactivate a rule — manager only ──────────────────────────────────
        public async Task<(bool Success, string Message)> DeactivateRuleAsync(int ruleId)
        {
            var rule = await _db.GetConciergeRuleByIdAsync(ruleId);
            if (rule == null)
                return (false, "Rule not found.");

            rule.IsActive = false;
            await _db.UpdateConciergeRuleAsync(rule);
            return (true, "Rule deactivated successfully.");
        }
    }

    // ── Result DTO ────────────────────────────────────────────────────────────
    public class ConciergeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ConciergeRule? Rule { get; set; }

        public ConciergeResult(bool success, string message, ConciergeRule? rule)
        {
            Success = success;
            Message = message;
            Rule = rule;
        }
    }
}