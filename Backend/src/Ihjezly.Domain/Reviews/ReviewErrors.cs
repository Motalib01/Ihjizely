using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotFound = new(
        "Review.NotFound",
        "The specified review could not be found."
    );

    public static Error CantReview = new(
        "Review.CantReview",
        "Cant Review for this property."
        );
}