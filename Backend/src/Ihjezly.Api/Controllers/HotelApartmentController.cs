using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/hotel-apartment")]
public class HotelApartmentController : ResidencePropertyControllerBase<HotelApartment, HotelApartmentDetails>
{
    public HotelApartmentController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}