using MaintenanceAPI.Exceptions;
using MaintenanceAPI.Models.DTOs;
using MaintenanceAPI.Models.Requests;
using MaintenanceAPI.Persistence;
using MaintenanceAPI.Persistence.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaintenanceAPI.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEnumerable<EndpointDataSource> _availableEndpoints;

        public MaintenanceService(ApplicationDbContext dbContext, IEnumerable<EndpointDataSource> availableEndpoints)
        {
            _dbContext = dbContext;
            _availableEndpoints = availableEndpoints;
        }
        public async Task CreateMaintenance(CreateMaintenanceRequest request)
        {
            await ValidateCreateMaintenance(request.AllAffected, request.AffectedEndpoints);

            var maintenance = new Maintenance(request.AllAffected, request.AffectedEndpoints);

            _dbContext.Maintenances.Add(maintenance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DisableLastMaintenance()
        {
            //helper method not used because of AsNoTracking()
            var lastMaintenance = await _dbContext.Maintenances
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            lastMaintenance.Enabled = false;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EndpointDto>> GetAvailableEndpoints()
        {
            var endpoints = _availableEndpoints.SelectMany(x => x.Endpoints).OfType<RouteEndpoint>();
            var mappedEndpoints = endpoints.Select(x =>
            {
                var descriptor = x.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                if (descriptor == null)
                    return null;

                return new EndpointDto
                {
                    Action = descriptor.ActionName,
                    Controller = descriptor.ControllerName,
                    RouteTemplate = x.RoutePattern.RawText.ToLower()
                };
            })
                .Where(x => x != null)
                .ToList();

            return await Task.FromResult(mappedEndpoints);
        }

        public async Task<GetMaintenanceDTO> GetLastMaintenance()
        {
            var lastMaintenance = await GetLastMaintenanceDb();

            return new GetMaintenanceDTO
            {
                Id = lastMaintenance.Id,
                Created = lastMaintenance.Created,
                Enabled = lastMaintenance.Enabled,
                AllAffected = lastMaintenance.AllAffected,
                AffectedEndpoints = lastMaintenance.AffectedEndpoints
                                    .Select(a => new EndpointDto
                                    {
                                        Action = a.Action,
                                        Controller = a.Controller,
                                        RouteTemplate = a.RouteTemplate
                                    }).ToList()
            };
        }
        private async Task<Maintenance> GetLastMaintenanceDb()
        {
            var lastMaintenance = await _dbContext.Maintenances
                .AsNoTracking()
                .Include(x => x.AffectedEndpoints)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            if (lastMaintenance == null)
            {
                throw new Exception("There are currently no maintenances present in the database");
            }

            return lastMaintenance;
        }
        private async Task ValidateCreateMaintenance(bool allAfected, IEnumerable<AffectedEndpointRequest> affectedEndpoints)
        {
            var lastMaintenance = await GetLastMaintenanceDb();
            if (lastMaintenance != null && lastMaintenance.Enabled)
            {
                throw new RequestValidationException("You cannot create new maintenance if previous one is still enabled");
            }
            //if you want you could validate or just skip assigning affectedEndpoints to the db object
            //but its probably better to let the user know that its not intended to provide allAffected as true,
            //which will disable whole portal (except specific allowed endpoints which are defined in middleware),
            //and additionally specify affectedEndpoints. Customize validation to your needs and business logic.
            if (allAfected && affectedEndpoints.Count() > 0)
            {
                throw new RequestValidationException("You cannot specify affected endpoints if you selected whole site to be affected");
            }
        }
    }
}
