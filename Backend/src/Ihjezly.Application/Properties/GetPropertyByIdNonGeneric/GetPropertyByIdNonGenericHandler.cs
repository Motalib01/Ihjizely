using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetPropertyByIdNonGeneric;

internal sealed class GetPropertyByIdNonGenericHandler
    : IQueryHandler<GetPropertyByIdNonGenericQuery, PropertyDto>
{
    private readonly IPropertyRepository _repository;
    private readonly IUserRepository _userRepository;

    public GetPropertyByIdNonGenericHandler(
        IPropertyRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<PropertyDto>> Handle(
        GetPropertyByIdNonGenericQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdNonGeniricAsync(request.Id, cancellationToken);

        if (property is null)
            return Result.Failure<PropertyDto>(PropertyErrors.NotFound);

        // Fetch owner info
        var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
        var ownerFullName = owner?.FullName ?? "Unknown";

        var dto = property.ToDto(ownerFullName);
        return Result.Success(dto);
    }
}