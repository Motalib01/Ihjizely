using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using MediatR;

namespace Ihjezly.Application.Properties.GetPropertyTypeById;

public record GetPropertyTypeByIdQuery(Guid PropertyId) : IRequest<Result<string>>;

internal sealed class GetPropertyTypeByIdQueryHandler : IRequestHandler<GetPropertyTypeByIdQuery, Result<string>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyTypeByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<string>> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure<string>(PropertyErrors.NotFound);

        // Return only the type name
        return Result.Success(property.GetType().Name);
    }
}