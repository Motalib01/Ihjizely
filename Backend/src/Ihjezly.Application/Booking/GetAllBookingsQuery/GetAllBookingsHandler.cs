using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking.Repositories;

namespace Ihjezly.Application.Booking.GetAllBookingsQuery;

internal sealed class GetAllBookingsHandler
    : IQueryHandler<GetAllBookingsQuery, List<BookingDto>>
{
    private readonly IBookingRepository _repository;

    public GetAllBookingsHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BookingDto>>> Handle(GetAllBookingsQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _repository.GetAllAsync(cancellationToken);

        var dtos = bookings.Select(b => new BookingDto(
            b.Id,
            b.ClientId,
            b.Name,
            b.PhoneNumber,
            b.PropertyId,
            b.StartDate,
            b.EndDate,
            b.TotalPrice.Amount,
            b.TotalPrice.CurrencyCode,
            b.Status.ToString(),
            b.ReservedAt
        )).ToList();

        return Result.Success(dtos);
    }
}