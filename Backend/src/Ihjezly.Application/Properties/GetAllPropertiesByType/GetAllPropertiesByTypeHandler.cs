using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetAllPropertiesByType;

internal sealed class GetAllPropertiesByTypeHandler<TProperty, TDetails>
    : IQueryHandler<GetAllPropertiesByTypeQuery<TProperty, TDetails>, List<PropertyDto>>
    where TProperty : PropertyWithDetails<TDetails>
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly IUserRepository _userRepository;

    public GetAllPropertiesByTypeHandler(
        IPropertyRepository<TProperty> repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(
        GetAllPropertiesByTypeQuery<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.GetAllAsync(cancellationToken);
        var dtos = new List<PropertyDto>(properties.Count);

        foreach (var property in properties)
        {
            var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
            var ownerFirstName = owner?.FirstName ?? "Unknown";
            var ownerLastName = owner?.LastName ?? "Unknown";

            dtos.Add(property.ToDto(ownerFirstName, ownerLastName));
        }

        return Result.Success(dtos);
    }
}