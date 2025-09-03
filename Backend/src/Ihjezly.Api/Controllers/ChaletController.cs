using Ihjezly.Api.Controllers.Base;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[Route("api/properties/chalet")]
public class ChaletController : HallPropertyControllerBase<Chalet, ChaletDetails>
{
    public ChaletController(ISender mediator, IWebHostEnvironment environment)
        : base(mediator, environment) { }
}