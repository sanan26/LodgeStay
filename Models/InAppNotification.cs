namespace LodgeStay.Models
{
    public class InAppNotification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = false;
        public bool IsExiting { get; set; } = false;
        public string Type { get; set; } = "info";
    }
}