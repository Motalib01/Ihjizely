using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories; // to get IUserRepository

namespace Ihjezly.Application.Properties.GetAllPropertiesNonGeneric;

internal sealed class GetAllPropertiesNonGenericHandler
    : IQueryHandler<GetAllPropertiesNonGenericQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;

    public GetAllPropertiesNonGenericHandler(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(
        GetAllPropertiesNonGenericQuery request,
        CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetAllNonGenericAsync(cancellationToken);

        var dtos = new List<PropertyDto>();

        foreach (var property in properties)
        {
            var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
            var ownerFullName = owner?.FullName ?? "Unknown";


            dtos.Add(property.ToDto(ownerFullName));
        }

        return Result.Success(dtos);
    }
}