using System.ComponentModel.DataAnnotations;

namespace SpotifyDeviceFlow.Models
{
    public class CodeRequest
    {
        [Required]
        public string Code { get; set; } = "";
    }
}
