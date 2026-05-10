using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tracing.DAL.Entities;

namespace WebAppTracingMvc.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required !")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required !")]
        [MaxLength(500)]
        public string Description { get; set; }
        public IFormFile Image { get; set; }

        public string? ImageURL { get; set; }
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string Catigory { get; set; }

        [ForeignKey(nameof(Producer))]
        public int? ProducerId { get; set; }
        [InverseProperty("Movies")]
        public Producer? Producer { get; set; }
        [ForeignKey(nameof(Cinema))]
        public int? CinemaId { get; set; }
        [InverseProperty("Movies")]
        public Cinema? Cinema { get; set; }

        public List<ActorViewModel> Actors { get; set; }

        public List<int> SelectedActorsIds { get; set; } = new List<int>();

        public List<ActorViewModel> AllActors { get; set; }


    }
}
