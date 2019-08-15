using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Your password must be between 5 to 10 characters ")]
        public string Password { get; set; }
    }
}