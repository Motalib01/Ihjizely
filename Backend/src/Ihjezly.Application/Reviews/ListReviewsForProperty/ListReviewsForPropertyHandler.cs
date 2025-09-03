using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Reviews.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Reviews.ListReviewsForProperty;

internal sealed class ListReviewsForPropertyHandler : IQueryHandler<ListReviewsForPropertyQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _repository;

    public ListReviewsForPropertyHandler(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ReviewDto>>> Handle(ListReviewsForPropertyQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _repository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        var dtos = reviews.Select(r => new ReviewDto(
            r.Id,
            r.PropertyId,
            r.UserId,
            r.Rating,
            r.Comment,
            r.CreatedAt
        )).ToList();

        return dtos;
    }
}