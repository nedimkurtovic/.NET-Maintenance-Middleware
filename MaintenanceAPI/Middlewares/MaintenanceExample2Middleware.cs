using MaintenanceAPI.Exceptions;
using MaintenanceAPI.Models;
using MaintenanceAPI.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaintenanceAPI.Middlewares
{
    //this example of maintenance middleware will do everything as example 1 but with
    //added checks for disabled/affected user provided endpoints during the POST /maintenances action
    public class MaintenanceExample2Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<CustomEndpoint> _allowedEndpoints;
        public MaintenanceExample2Middleware(RequestDelegate next)
        {
            _next = next;
            _allowedEndpoints = new List<CustomEndpoint>
            {
                new CustomEndpoint { Action = "*", Controller = "Maintenances" }
            };
        }
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var endpoint = context.GetEndpoint();

            var lastMaintenance = await dbContext.Maintenances
                .Include(x => x.AffectedEndpoints)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            if(endpoint != null && (lastMaintenance?.Enabled ?? false))
            {
                var descriptor = endpoint.Metadata
                    .GetMetadata<ControllerActionDescriptor>();

                var action = descriptor.ActionName;
                var controller = descriptor.ControllerName;
                var routeTemplate = (endpoint as RouteEndpoint).RoutePattern.RawText.ToLower();

                //if maintenance disabled whole site and if current endpoint is not targeting
                //one of the allowed endpoints
                //1st condition: (if allowedEndpoint has all actions allowed and target endpoint matches controller, return true)
                //2nd condition: (if allowedEndpoint has specific action allowed and target endpoint matches that controller and action, return true)
                //we use ! operator to ask if theres no items that correspond to this conditions, in that case we throw an exception
                if (lastMaintenance.AllAffected)
                {
                    if(!_allowedEndpoints.Any(x => (x.AllActions && x.Controller == controller) || (!x.AllActions
                            && x.Controller == controller && x.Action == action)))
                    {
                        throw new MaintenanceException("Entire page is under the maintenance.");
                    }
                }
                //if the whole site is not affected, then check if the target action, controller, routepattern
                //exist under the affected endpoints.
                else
                {
                    if(lastMaintenance.AffectedEndpoints.Any(x => x.Controller == controller
                        && x.Action == x.Action && x.RouteTemplate == routeTemplate))
                    {
                        throw new MaintenanceException("The endpoint that you are trying to access is currently disabled.");
                    }
                }
            }
            await _next(context);
        }
    }
    public static class MaintenanceExample2MiddlewareExtensions {
        public static IApplicationBuilder UseMaintenanceExample2Middleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceExample2Middleware>();
        }
    }
}
