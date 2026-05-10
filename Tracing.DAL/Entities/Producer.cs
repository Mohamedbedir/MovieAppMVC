using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.DAL.Entities
{
    public class Producer
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(500)]
        public string Bio { get; set; }
        public string? ProfilePictureURL { get; set; }

        [InverseProperty("Producer")]
        public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}
