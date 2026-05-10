using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tracing.DAL.Entities;

namespace WebAppTracingMvc.ViewModels
{
    public class ProducerViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FullName is required !")]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Bio is required !")]
        [MaxLength(500)]
        public string Bio { get; set; }
        public string? ProfilePictureURL { get; set; }
        public IFormFile Image { get; set; }

        public List<Movie> Movies { get; set; }

    }
}
