using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.PropertySearch;

public sealed record SearchPropertiesQuery(PropertySearchRequest Request) : IQuery<List<PropertyDto>>;


internal sealed class SearchPropertiesHandler : IQueryHandler<SearchPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository; 

    public SearchPropertiesHandler(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.SearchAsync(request.Request, cancellationToken);

        var dtos = new List<PropertyDto>();

        foreach (var property in properties)
        {
            // Get business owner info
            var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
            var firstName = owner?.FirstName ?? string.Empty;
            var lastName = owner?.LastName ?? string.Empty;

            dtos.Add(property.ToDto(firstName, lastName));
        }

        return Result.Success(dtos);
    }
}
