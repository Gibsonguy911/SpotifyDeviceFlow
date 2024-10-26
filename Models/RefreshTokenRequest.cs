using System.ComponentModel.DataAnnotations;

namespace SpotifyDeviceFlow.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }
}
