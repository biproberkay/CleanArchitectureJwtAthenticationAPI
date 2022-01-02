using System.ComponentModel.DataAnnotations;

namespace Shared.Communicate.Identity.Token
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}