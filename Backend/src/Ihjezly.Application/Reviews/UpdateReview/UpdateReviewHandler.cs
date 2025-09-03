using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reviews;

namespace Ihjezly.Application.Reviews.UpdateReview;

internal sealed class UpdateReviewHandler : ICommandHandler<UpdateReviewCommand>
{
    private readonly IReviewRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewHandler(IReviewRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null) return Result.Failure(ReviewErrors.NotFound);

        review.Update(request.Rating, request.Comment);
        _repository.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}