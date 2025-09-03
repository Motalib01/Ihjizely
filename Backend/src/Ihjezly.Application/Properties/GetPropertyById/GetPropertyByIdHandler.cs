using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetPropertyById;

internal sealed class GetPropertyByIdHandler<TProperty, TDetails>
    : IQueryHandler<GetPropertyByIdQuery<TProperty, TDetails>, PropertyDto?>
    where TProperty : PropertyWithDetails<TDetails>
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly IUserRepository _userRepository;

    public GetPropertyByIdHandler(
        IPropertyRepository<TProperty> repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<PropertyDto?>> Handle(
        GetPropertyByIdQuery<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (property is null)
            return Result.Failure<PropertyDto?>(PropertyErrors.NotFound);

        // Fetch owner info
        var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
        var ownerFirstName = owner?.FirstName ?? "Unknown";
        var ownerLastName = owner?.LastName ?? "Unknown";

        return Result.Success<PropertyDto?>(property.ToDto(ownerFirstName, ownerLastName));
    }
}