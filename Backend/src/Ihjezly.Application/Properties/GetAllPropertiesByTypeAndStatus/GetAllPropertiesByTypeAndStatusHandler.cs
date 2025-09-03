using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetPropertiesByStatus;

internal sealed class GetAllPropertiesByTypeAndStatusHandler<TProperty, TDetails>
    : IQueryHandler<GetAllPropertiesByTypeAndStatusQuery<TProperty, TDetails>, List<PropertyDto>>
    where TProperty : PropertyWithDetails<TDetails>
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly IUserRepository _userRepository;

    public GetAllPropertiesByTypeAndStatusHandler(
        IPropertyRepository<TProperty> repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(
        GetAllPropertiesByTypeAndStatusQuery<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.GetByStatusAsync(request.Status, cancellationToken);

        var dtos = new List<PropertyDto>();

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