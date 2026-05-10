using System.ComponentModel.DataAnnotations;

namespace WebAppTracingMvc.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="Email Is Required")]
        [EmailAddress(ErrorMessage ="InValid Email")]
        public string Email { get; set; }
    }
}
