using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;

namespace Ihjezly.Application.Booking.GetBookingById;

internal sealed class GetBookingByIdHandler : IQueryHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IBookingRepository _repository;

    public GetBookingByIdHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null) return Result.Failure<BookingDto>(BookingErrors.NotFound);

        var dto = new BookingDto(
            booking.Id,
            booking.ClientId,
            booking.Name,
            booking.PhoneNumber,
            booking.PropertyId,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice.Amount,
            booking.TotalPrice.Currency.Code,
            booking.Status.ToString(),
            booking.ReservedAt
        );

        return dto;
    }
}