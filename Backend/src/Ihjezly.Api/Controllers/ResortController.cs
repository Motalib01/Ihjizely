using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/resort")]
public class ResortController : ResidencePropertyControllerBase<Resort, ResortDetails>
{
    public ResortController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}