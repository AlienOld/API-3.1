using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class Enterprise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int UserId { get; set; }

        //[Required]
        //public virtual User User { get; set; }
        //public virtual ICollection<Room> Rooms { get; set; }
    }
}
