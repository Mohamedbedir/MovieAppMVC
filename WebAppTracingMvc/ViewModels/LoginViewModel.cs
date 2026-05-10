using System.ComponentModel.DataAnnotations;

namespace WebAppTracingMvc.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email Is required")]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
