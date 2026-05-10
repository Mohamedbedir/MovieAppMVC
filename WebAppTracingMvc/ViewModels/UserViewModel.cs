using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebAppTracingMvc.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string? ProfilePicture { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? RoleName { get; set; }
        public List<string>? Roles { get; set; }

    }
}
