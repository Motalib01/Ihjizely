using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetAllPropertiesByType;

internal sealed class GetAllPropertiesByStatusHandler
    : IQueryHandler<GetAllPropertiesByStatusQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository<Property> _repository;
    private readonly IUserRepository _userRepository;

    public GetAllPropertiesByStatusHandler(
        IPropertyRepository<Property> repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(
        GetAllPropertiesByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.GetByStatusAsync(request.Status, cancellationToken);

        var dtos = new List<PropertyDto>(properties.Count);

        // NOTE: sequential lookups to avoid parallel access to the same DbContext
        foreach (var p in properties)
        {
            var owner = await _userRepository.GetByIdAsync(p.BusinessOwnerId, cancellationToken);
            var firstName = owner?.FullName ?? "Unknown";

            dtos.Add(p.ToDto(firstName));
        }

        return Result.Success(dtos);
    }
}