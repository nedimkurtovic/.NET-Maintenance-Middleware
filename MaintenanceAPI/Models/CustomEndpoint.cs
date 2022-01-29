using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Models
{
    public class CustomEndpoint
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool AllActions => Action == "*";
    }
}
