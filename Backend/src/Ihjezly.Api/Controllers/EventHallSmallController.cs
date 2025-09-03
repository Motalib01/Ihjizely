using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/event-hall-small")]
public class EventHallSmallController : HallPropertyControllerBase<EventHallSmall, EventHallSmallDetails>
{
    public EventHallSmallController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}