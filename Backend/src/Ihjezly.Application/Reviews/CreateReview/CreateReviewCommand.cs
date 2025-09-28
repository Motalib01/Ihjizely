using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reviews.CreateReview;

public sealed record CreateReviewCommand(
    Guid PropertyId,
    Guid UserId,
    Guid BookingId,
    int Rating,
    string Comment
) : ICommand<Guid>;