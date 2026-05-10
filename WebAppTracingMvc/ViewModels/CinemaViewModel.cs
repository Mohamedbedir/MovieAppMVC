using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tracing.DAL.Entities;

namespace WebAppTracingMvc.ViewModels
{
    public class CinemaViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required !")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required !")]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Address is required !")]
        [MaxLength(150)]
        public string Address { get; set; }

        public string? Logo { get; set; }
        public IFormFile Image { get; set; }

        public List<Movie> Movies { get; set; }


    }
}
