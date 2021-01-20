using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mail { get; set; }
        public string Tariff { get; set; }
        public string Role { get; set; }
    }
}
