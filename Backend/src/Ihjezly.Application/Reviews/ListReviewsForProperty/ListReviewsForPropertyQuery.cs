using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Reviews.DTO;

namespace Ihjezly.Application.Reviews.ListReviewsForProperty;

public sealed record ListReviewsForPropertyQuery(Guid PropertyId) : IQuery<List<ReviewDto>>;