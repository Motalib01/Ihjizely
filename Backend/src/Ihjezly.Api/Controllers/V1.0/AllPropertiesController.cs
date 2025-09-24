using Asp.Versioning;
using Ihjezly.Application.Properties.ChangePropertyIsAd;
using Ihjezly.Application.Properties.ChangePropertyStatusNonGeneric;
using Ihjezly.Application.Properties.DeleteProperty;
using Ihjezly.Application.Properties.GetAllPropertiesByType;
using Ihjezly.Application.Properties.GetAllPropertiesNonGeneric;
using Ihjezly.Application.Properties.GetPropertiesByBusinessOwnerId;
using Ihjezly.Application.Properties.GetPropertyByIdNonGeneric;
using Ihjezly.Application.Properties.GetPropertyTypeById;
using Ihjezly.Application.Properties.PropertySearch;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ihjezly.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AllPropertiesController : ControllerBase
{
    private readonly ISender _mediator;

    public AllPropertiesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProperties(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllPropertiesNonGenericQuery(), cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // GET: api/allproperties/by-owner/{businessOwnerId}
    [HttpGet("by-owner/{businessOwnerId}")]
    public async Task<IActionResult> GetByBusinessOwnerId(Guid businessOwnerId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPropertiesByBusinessOwnerIdQuery(businessOwnerId), cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("all/status/{status}")]
    public async Task<IActionResult> GetAllPropertiesByStatus(string status)
    {
        if (!Enum.TryParse<PropertyStatus>(status, true, out var parsedStatus))
            return BadRequest("Invalid status");

        var result = await _mediator.Send(new GetAllPropertiesByStatusQuery(parsedStatus));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] PropertyStatus newStatus, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ChangePropertyStatusNonGenericCommand(id, newStatus), cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePropertyNonGenericCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPropertyByIdNonGenericQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetByType(string type, CancellationToken cancellationToken)
    {
        var propertyDtoResult = type.ToLower() switch
        {
            "apartment" => await _mediator.Send(new GetAllPropertiesByTypeQuery<Apartment, ApartmentDetails>(), cancellationToken),
            "chalet" => await _mediator.Send(new GetAllPropertiesByTypeQuery<Chalet, ChaletDetails>(), cancellationToken),
            "hotelroom" => await _mediator.Send(new GetAllPropertiesByTypeQuery<HotelRoom, HotelRoomDetails>(), cancellationToken),
            "hotelapartment" => await _mediator.Send(new GetAllPropertiesByTypeQuery<HotelApartment, HotelApartmentDetails>(), cancellationToken),
            "resort" => await _mediator.Send(new GetAllPropertiesByTypeQuery<Resort, ResortDetails>(), cancellationToken),
            "resthouse" => await _mediator.Send(new GetAllPropertiesByTypeQuery<RestHouse, RestHouseDetails>(), cancellationToken),
            "eventhallsmall" => await _mediator.Send(new GetAllPropertiesByTypeQuery<EventHallSmall, EventHallSmallDetails>(), cancellationToken),
            "eventhalllarge" => await _mediator.Send(new GetAllPropertiesByTypeQuery<EventHallLarge, EventHallLargeDetails>(), cancellationToken),
            "meetingroom" => await _mediator.Send(new GetAllPropertiesByTypeQuery<MeetingRoom, MeetingRoomDetails>(), cancellationToken),
            "villaevent" => await _mediator.Send(new GetAllPropertiesByTypeQuery<VillaEvent, VillaEventDetails>(), cancellationToken),
            _ => null
        };

        if (propertyDtoResult == null)
            return BadRequest("Invalid property type");

        return propertyDtoResult.IsSuccess ? Ok(propertyDtoResult.Value) : BadRequest(propertyDtoResult.Error);
    }


    [HttpPatch("{id}/is-ad")]
    public async Task<IActionResult> ChangeIsAd(Guid id, [FromBody] bool isAd, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ChangePropertyIsAdCommand(id, isAd), cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PropertySearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPropertiesQuery(request), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id}/type")]
    public async Task<IActionResult> GetTypeByPropertyId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPropertyTypeByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

}