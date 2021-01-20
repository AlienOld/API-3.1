using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Classes
{
    public class Backups
    {
        private List<string> list { get; set; }

        public Backups()
        {
            list = new List<string>();
        }

        public List<string> List
        {
            get
            {
                return list;
            }
        }
    }
}
