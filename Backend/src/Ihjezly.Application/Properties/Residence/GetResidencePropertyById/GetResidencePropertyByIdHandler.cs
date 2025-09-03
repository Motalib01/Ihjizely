using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.GetPropertyByIdGeneric;

internal sealed class GetResidencePropertyByIdHandler<TProperty, TDetails>
    : IQueryHandler<GetResidencePropertyByIdQuery<TProperty, TDetails>, ResidencePropertyDto>
    where TProperty : ResidenceProperty<TDetails>
{
    private readonly IPropertyRepository<TProperty> _repository;

    public GetResidencePropertyByIdHandler(IPropertyRepository<TProperty> repository)
    {
        _repository = repository;
    }

    public async Task<Result<ResidencePropertyDto>> Handle(
        GetResidencePropertyByIdQuery<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure<ResidencePropertyDto>(PropertyErrors.NotFound);

        return Result.Success(property.ToResidenceDto());
    }
}