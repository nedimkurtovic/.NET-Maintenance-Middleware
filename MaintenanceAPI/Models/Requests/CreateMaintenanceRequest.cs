using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Models.Requests
{
    public class CreateMaintenanceRequest
    {
        public bool AllAffected { get; set; }
        public IEnumerable<AffectedEndpointRequest> AffectedEndpoints { get; set; }
    }
    public class AffectedEndpointRequest
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RouteTemplate { get; set; }
    }
}
