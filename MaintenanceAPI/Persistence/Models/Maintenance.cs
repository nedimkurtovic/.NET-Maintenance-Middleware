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
    }
}
