using MaintenanceAPI.Exceptions;
using MaintenanceAPI.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaintenanceAPI.Middlewares
{
    public class Endpoint
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool AllActions => Action == "*";
    }
    //this middleware will query the database for the last maintenance and check whether its enabled
    //if enabled it will check whether the whole site is affected and if true, will check whether the
    //target endpoint is contained in the allowed endpoints
    public class MaintenanceExample1Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<Endpoint> _allowedEndpoints;
        public MaintenanceExample1Middleware(RequestDelegate next)
        {
            _next = next;
            _allowedEndpoints = new List<Endpoint>
            {
                new Endpoint { Action = "*", Controller = "WeatherForecast"},
                new Endpoint { Action = "*", Controller = "AllAllowed" },
                new Endpoint { Action = "One", Controller = "PartiallyAllowed" },
                new Endpoint { Action = "*", Controller = "Maintenances" },
            };
        }
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var endpoint = context.GetEndpoint();
            var lastMaintenance = dbContext.Maintenances.LastOrDefault();
            if(endpoint != null && (lastMaintenance?.Enabled ?? false))
            {
                var descriptor = endpoint.Metadata
                    .GetMetadata<ControllerActionDescriptor>();

                var action = descriptor.ActionName;
                var controller = descriptor.ControllerName;

                //if maintenance disabled whole site and if current endpoint is not targeting
                //one of the allowed endpoints
                //1st condition: (if allowedEndpoint has all actions allowed and target endpoint matches controller, return true)
                //2nd condition: (if allowedEndpoint has specific action allowed and target endpoint matches that controller and action, return true)
                //we use ! operator to ask if theres no items that correspond to this conditions, in that case we throw an exception
                if(lastMaintenance.AllAffected && !_allowedEndpoints.Any(x => (x.AllActions && x.Controller == controller) || (!x.AllActions
                && x.Controller == controller && x.Action == action)))
                {
                    throw new MaintenanceException("Entire page is under the maintenance.");
                }
            }
            await _next(context);
        }
    }
    public static class MaintenanceExample1MiddlewareExtensions {
        public static IApplicationBuilder UseMaintenanceExample1Middleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceExample1Middleware>();
        }
    }
}
