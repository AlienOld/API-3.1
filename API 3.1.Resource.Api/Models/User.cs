using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Surname { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Mail { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Pass { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int TariffId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int RoleId { get; set; }

        //[Required]
        //public virtual Tariff Tariff { get; set; }

        //[Required]
        //public virtual Role Role { get; set; }

        //public virtual ICollection<Enterprise> Enterprises { get; set; }
    }
}
