using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int EnterpriseId { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool Alarm { get; set; }

        //[Required]
        //public virtual Enterprise Enterprise { get; set; }

        //public virtual ICollection<T_sensor> T_Sensors { get; set; }
        //public virtual ICollection<H_sensor> H_Sensors { get; set; }
    }
}
