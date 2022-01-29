using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Models.DTOs
{
    public class GetMaintenanceDTO
    {
        public int Id { get; set; }
        public bool AllAffected { get; set; }
        public bool Enabled { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<EndpointDto> AffectedEndpoints { get; set; }
    }
    public class EndpointDto
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RouteTemplate { get; set; }
    }
}
