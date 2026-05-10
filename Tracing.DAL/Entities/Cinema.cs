using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.DAL.Entities
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public string? Logo { get; set; }
        //[Required]
        public string? Address { get; set; }

        [InverseProperty("Cinema")]
        public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}
