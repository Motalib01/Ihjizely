using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.SavedProperties.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.SavedProperties.GetSavedPropertiesForUser;

internal sealed class GetSavedPropertiesForUserHandler
    : IQueryHandler<GetSavedPropertiesForUserQuery, List<SavedPropertyDto>>
{
    private readonly ISavedPropertyRepository _repository;

    public GetSavedPropertiesForUserHandler(ISavedPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SavedPropertyDto>>> Handle(GetSavedPropertiesForUserQuery request, CancellationToken cancellationToken)
    {
        var savedProperties = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = savedProperties
            .Select(p => new SavedPropertyDto(p.Id, p.UserId, p.PropertyId, p.SavedAt))
            .ToList();

        return dtos;
    }
}