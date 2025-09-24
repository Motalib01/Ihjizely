using Asp.Versioning;
using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VillaEventController : HallPropertyControllerBase<VillaEvent, VillaEventDetails>
{
    public VillaEventController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}