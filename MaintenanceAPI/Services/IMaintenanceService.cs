using MaintenanceAPI.Models.DTOs;
using MaintenanceAPI.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Services
{
    public interface IMaintenanceService
    {
        Task<GetMaintenanceDTO> GetLastMaintenance();
        Task DisableLastMaintenance();
        Task CreateMaintenance(CreateMaintenanceRequest request);
        Task<IEnumerable<EndpointDto>> GetAvailableEndpoints();
    }
}
