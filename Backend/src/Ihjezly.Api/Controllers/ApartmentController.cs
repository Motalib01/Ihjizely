using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/apartment")]
public class ApartmentController : ResidencePropertyControllerBase<Apartment, ApartmentDetails>
{
    public ApartmentController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}