namespace SpotifyDeviceFlow.Models
{
    public class Code
    {
        public string CodeValue { get; set; } = "";
        public string Token { get; set; } = "";
        public DateTime ExpiryTime { get; } = DateTime.UtcNow.AddMinutes(5);
    }
}
