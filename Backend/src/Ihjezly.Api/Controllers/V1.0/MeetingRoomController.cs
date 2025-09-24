using Asp.Versioning;
using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MeetingRoomController : HallPropertyControllerBase<MeetingRoom, MeetingRoomDetails>
{
    public MeetingRoomController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}