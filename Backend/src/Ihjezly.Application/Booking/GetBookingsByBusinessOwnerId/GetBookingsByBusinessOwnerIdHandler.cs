using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking.Repositories;

namespace Ihjezly.Application.Booking.GetBookingsByBusinessOwnerId;

internal sealed class GetBookingsByBusinessOwnerIdHandler
    : IQueryHandler<GetBookingsByBusinessOwnerIdQuery, List<BookingDto>>
{
    private readonly IBookingRepository _repository;

    public GetBookingsByBusinessOwnerIdHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BookingDto>>> Handle(GetBookingsByBusinessOwnerIdQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _repository.GetByBusinessOwnerIdAsync(query.BusinessOwnerId, cancellationToken);

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