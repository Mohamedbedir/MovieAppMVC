using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppTracingMvc.ViewModels
{
    public class ActorViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        public IFormFile Image { get; set; }
        public string? ProfilePictureUrl { get; set; }
        [Required]
        [MaxLength(500)]
        public string Bio { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }
        public List<MovieViewModel> Movies { get; set; }

    }
}
