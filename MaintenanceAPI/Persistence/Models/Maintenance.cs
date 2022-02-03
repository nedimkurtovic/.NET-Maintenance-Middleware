using MaintenanceAPI.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Persistence.Models
{
    public class Maintenance
    {
        
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public bool AllAffected { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<MaintenanceEndpoint> AffectedEndpoints { get; set; }
        public Maintenance(bool allAffected, IEnumerable<AffectedEndpointRequest> affectedEndpoints)
        {
            AllAffected = allAffected;
            Enabled = true;
            Created = DateTime.UtcNow;
            AffectedEndpoints = affectedEndpoints != null ? affectedEndpoints.Select(x => new MaintenanceEndpoint(x)).ToList() : new List<MaintenanceEndpoint>();
        }
        public Maintenance()
        {

        }
    }
}
