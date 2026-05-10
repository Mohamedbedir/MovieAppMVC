using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.DAL.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        [Required]
        [MaxLength(500)]
        public string Bio {  get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }
        public ICollection<ActorMovie> ActorMovies { get; set; } = new HashSet<ActorMovie>();
    }
}
