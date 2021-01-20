using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class Tariff
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Max_enterprises { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Max_rooms { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Max_sensors { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        //public virtual ICollection<User> Users { get; set; }
    }
}
