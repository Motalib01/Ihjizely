using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Halls.GetHallPropertyById;

internal sealed class GetHallPropertyByIdHandler<TProperty, TDetails>
    : IQueryHandler<GetHallPropertyByIdQuery, HallPropertyDto>
    where TProperty : HallProperty<TDetails>
{
    private readonly IPropertyRepository<TProperty> _repository;

    public GetHallPropertyByIdHandler(IPropertyRepository<TProperty> repository)
    {
        _repository = repository;
    }

    public async Task<Result<HallPropertyDto>> Handle(
        GetHallPropertyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure<HallPropertyDto>(PropertyErrors.NotFound);

        return Result.Success(property.ToHallDto());
    }
}
