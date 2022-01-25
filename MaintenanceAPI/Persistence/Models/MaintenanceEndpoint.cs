using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Persistence.Models
{
    public class MaintenanceEndpoint
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RouteTemplate { get; set; }
        public int MaintenanceId { get; set; }
        public Maintenance Maintenance { get; set; }
    }
}
