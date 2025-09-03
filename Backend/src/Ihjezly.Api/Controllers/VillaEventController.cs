using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/villa-event")]
public class VillaEventController : HallPropertyControllerBase<VillaEvent, VillaEventDetails>
{
    public VillaEventController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}