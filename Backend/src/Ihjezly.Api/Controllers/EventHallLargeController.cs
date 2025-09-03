using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/event-hall-large")]
public class EventHallLargeController : HallPropertyControllerBase<EventHallLarge, EventHallLargeDetails>
{
    public EventHallLargeController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}