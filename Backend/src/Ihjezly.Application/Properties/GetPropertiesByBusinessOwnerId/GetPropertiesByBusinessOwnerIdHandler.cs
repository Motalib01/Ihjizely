using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetPropertiesByBusinessOwnerId;

internal sealed class GetPropertiesByBusinessOwnerIdHandler
    : IQueryHandler<GetPropertiesByBusinessOwnerIdQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _repository;
    private readonly IUserRepository _userRepository;

    public GetPropertiesByBusinessOwnerIdHandler(
        IPropertyRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(
        GetPropertiesByBusinessOwnerIdQuery query,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.GetByBusinessOwnerIdAsync(query.BusinessOwnerId, cancellationToken);

        // Fetch the owner once since all properties belong to them
        var owner = await _userRepository.GetByIdAsync(query.BusinessOwnerId, cancellationToken);
        var ownerFirstName = owner?.FirstName ?? "Unknown";
        var ownerLastName = owner?.LastName ?? "Unknown";

        var dtos = properties
            .Select(p => p.ToDto(ownerFirstName, ownerLastName))
            .ToList();

        return Result.Success(dtos);
    }
}