using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/rest-house")]
public class RestHouseController : ResidencePropertyControllerBase<RestHouse, RestHouseDetails>
{
    public RestHouseController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}