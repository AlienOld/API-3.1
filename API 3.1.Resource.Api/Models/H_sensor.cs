using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class H_sensor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Coordinates { get; set; }

        [Required]
        [Column(TypeName = "real")]
        public float Min_value { get; set; }

        [Required]
        [Column(TypeName = "real")]
        public float Max_value { get; set; }

        [Required]
        [Column(TypeName = "real")]
        public float Value { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool Is_working { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int RoomId { get; set; }

        //[Required]
        //public virtual Room Room { get; set; }
    }
}
