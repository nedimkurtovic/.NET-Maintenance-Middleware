<img src="https://user-images.githubusercontent.com/33371344/152246024-4be97e18-a2ef-414f-a935-bfaffb7bc521.jpg" height=300 width=1000>
<h2 align="center">Maintenance Middleware</h3>
<h3 align="center">
  <a href="#intro">Intro</a>
  <span> · </span>
  <a href="#database-model">Database Model</a>
  <span> · </span>
  <a href="#middleware">Middleware</a>
  <span> · </span>
  <a href="#controllers-and-examples">Controllers and examples</a>
  <span> · </span>
  <a href="#extras">Extras</a>
</h3>

## Intro

This example will demonstrate how you can implement very basic and minimal Maintenance middleware that disables or renders certain endpoints on your Web API as unavailable. I will use .NET 5.0 with Entity Framework Core for persistence to implement this project. I would be glad to receive your feedback and thoughts about this idea, thanks for reading.

## Database Model

Main idea for database models is that we have two tables, one for the Maintenances, which the user will be required to create and disable in order for middleware to work, and the other one for Maintenance_Endpoints which will connect Maintenance to affected points in zero to many relationship. Although for the most basic usage, second table is not necessary but it really comes in handy when we want to disable specific endpoints.

_Note_:
_When I was thinking and designing solution for this problem, an option to use the 3rd table called Endpoints for many to many relationship to Maintenances was also coming to my mind but I decided to denormalize data of that table to the Maintenance_Endpoints table for the following reasons_:

APIs in the development lifecycle are susceptible to changes which could alter controller and action names, route templates or they could break controller into smaller controllers if it grows large, etc.. So if we would define current Endpoints and add them to the database, we would run into a problem of updating or adding new entries in the future whenever we introduce breaking changes to the API. This is prone to errors and for those reasons I think that it would be better to place properties/columns of the Endpoint table to the Maintenance_Endpoint because we would always keep the latest data in the Maintenance_Endpoints table without the need of updating or adding new endpoints.

**Maintenance model**:

```cs
public class Maintenance
{
    public int Id { get; set; }
    public bool Enabled { get; set; }
    public bool AllAffected { get; set; }
    public DateTime Created { get; set; }
    public IEnumerable<MaintenanceEndpoint> AffectedEndpoints { get; set; }
}
```

**MaintenanceEndpoint model**:

```cs
public class MaintenanceEndpoint
{
    public int Id { get; set; }
    public string Action { get; set; }
    public string Controller { get; set; }
    public string RouteTemplate { get; set; }
    public int MaintenanceId { get; set; }
    public Maintenance Maintenance { get; set; }
}
```

We will use the Action, Controller, RouteTemplate to uniquely identify affected endpoint and maintenanceId to actually link affected endpoint to the maintenance. There is one small, to say a potential problem, picture this hypothetical situation:
User created maintenance on the page and selected desired endpoints to be disabled. If user doesn't disable the maintenance and the application shuts down, and while the application was not shut down, there were certain breaking changes (Controller/Action name or route template of the affected endpoint changed) then upon the next start of the application, those changed endpoints will not be disabled anymore. The user will have to disable and create new maintenance for the page with the latest currently available endpoints list. This is not impossible to happen, it's just the limit of this implementation.

## Middleware

I listed two sample cases with the names `MaintenanceExample1Middleware.cs` and `MaintenanceExample2Middleware.cs`, although this is customizable to your business logic or needs.
First sample case shows the simplest way, either the page is working with some preinitialized endpoints or its not working at all. I should've probably made two versions of POST /maintenances, one for the Example 1 where you can only specify whether the page is under the maintenance or not and it totally skips the Maintenance Endpoints. The other one (current implementation of POST /maintenances) where we actually use the full potential of the other table (MaintenanceEndpoints) allowing us to shut down only certain endpoints.

To create your own middleware in .NET core check this out: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-6.0

So the structure is fairly simple:

- Initialized list of allowed endpoints – These endpoints will work no matter what (we need these for Maintenance controller endpoints)
- InvokeAsync which is the method that must return Task and accept first parameter HttpContext, you can inject additional parameters with DI in this method (like I did with the ApplicationDbContext)

This Invoke async method will first use the _HttpContext_ method _GetEndpoint()_ to retrieve the target endpoint and also get the latest maintenance in the database. Few nullchecks and status enabled later and we arrive to the body:

- Extract the `ControllerActionDescriptor` using _.GetMetadata<T>()_ method from the endpoint metadata
- Then we assign action, controller and route template variables for easier usage in conditions
- Afterwards, we do the conditions which check whether the whole page or only portion of it is under maintenance and then we do the inner conditions which essentialy check whether the target endpoint is in the list of allowed endpoints or if its in the affected endpoints list. Based on results we throw the MaintenanceException.

**_Tada_**

## Controllers and examples

We will need some endpoints for creating, retrieving and disabling maintenance and also for getting actual list of endpoints which can be used to populate dropdown list on the frontend. Service class provides only the basic implementations that fulfill action requirements, obviously you would customize this to your needs and validate it more thorougly (for example validate whether the user specified valid endpoints using the injected `IEnumerable<EndpointDataSource>`).

So the actions are:

1. **POST** /maintenances -> _CreateMaintenance()_ (CreateMaintenanceRequest request) – used to create new maintenance on the page, it's set to be enabled by default.
2. **GET** /maintenances/endpoints -> _GetAvailableEndpoints()_ – used to populate dropdown list on the frontend so user can specify certain endpoints to be affected. We could probably filter out the maintenance endpoints and not display them to user, or find a way to share `_allowedEndpoints` list source and filter out all endpoints that belong to that list.
3. **GET** /maintenances/last -> _Get()_ -> used to retrieve the last maintenance in the database, we will always work with the latest row in db.
4. **PATCH** /maintenances/last/disable -> _DisableLastMaintenance()_ -> used to disable the last maintenance in the database. When you do this, affected endpoints will no longer be disabled and you will be able to use them as usual.

## Extras

**Exceptions and exception middleware** - Its the basic implementation of exception handling in form of middleware on web API. I added two exceptions, one for the validation and one for the maintenance in case user tries to access disabled endpoint.

**Additional controllers** - Used to demonstrate and test out the middleware.
