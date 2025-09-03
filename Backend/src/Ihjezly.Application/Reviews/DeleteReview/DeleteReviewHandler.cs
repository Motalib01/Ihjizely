using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reviews;

namespace Ihjezly.Application.Reviews.DeleteReview;

internal sealed class DeleteReviewHandler : ICommandHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewHandler(IReviewRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null) return Result.Failure(ReviewErrors.NotFound);

        review.Delete();
        _repository.Delete(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}