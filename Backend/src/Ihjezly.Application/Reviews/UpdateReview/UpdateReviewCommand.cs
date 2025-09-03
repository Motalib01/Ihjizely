using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reviews.UpdateReview;

public sealed record UpdateReviewCommand(
    Guid ReviewId,
    int Rating,
    string Comment
) : ICommand;