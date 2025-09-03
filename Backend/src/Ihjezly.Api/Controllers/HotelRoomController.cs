using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/hotel-room")]
public class HotelRoomController : ResidencePropertyControllerBase<HotelRoom, HotelRoomDetails>
{
    public HotelRoomController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}