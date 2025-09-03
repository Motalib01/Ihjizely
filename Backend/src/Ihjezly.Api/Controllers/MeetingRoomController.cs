using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/meeting-room")]
public class MeetingRoomController : HallPropertyControllerBase<MeetingRoom, MeetingRoomDetails>
{
    public MeetingRoomController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}