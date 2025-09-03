using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reviews.DeleteReview;

public sealed record DeleteReviewCommand(Guid ReviewId) : ICommand;