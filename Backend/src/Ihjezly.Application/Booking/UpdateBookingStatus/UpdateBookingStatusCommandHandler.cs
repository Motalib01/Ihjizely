using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Transactions;

public sealed class UpdateBookingStatusCommandHandler : ICommandHandler<UpdateBookingStatusCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookingStatusCommandHandler(
        IBookingRepository bookingRepository,
        IWalletRepository walletRepository,
        INotificationRepository notificationRepository,
        ITransactionRepository transactionRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _walletRepository = walletRepository;
        _notificationRepository = notificationRepository;
        _transactionRepository = transactionRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(BookingErrors.NotFound);

        var clientId = booking.ClientId;
        string notificationMessage = "";

        try
        {
            booking.ChangeStatusTo(request.NewStatus);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("Booking.StatusChangeError", ex.Message));
        }

        if (request.NewStatus == BookingStatus.Rejected)
        {
            notificationMessage = $"تم رفض حجزك ل '{booking.Name}'. لم يتم خصم أي مبلغ.";
        }
        else if (request.NewStatus == BookingStatus.Confirmed)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(clientId, cancellationToken);
            if (wallet is null)
                return Result.Failure(WalletErrors.NotFound);

            var fee = new Money(20, Currency.FromCode("LYD"));

            try
            {
                wallet.DeductFunds(fee);
            }
            catch (InvalidOperationException)
            {
                return Result.Failure<BookingDto>(WalletErrors.InsufficientBalance);
            }

            _walletRepository.Update(wallet);

            var transaction = Transaction.Create(wallet.Id, fee, "Booking creation fee");
            _transactionRepository.Add(transaction);

            // Add booking dates to property's unavailable list
            var property = await _propertyRepository.GetByIdNonGeniricAsync(booking.PropertyId, cancellationToken);
            if (property is null)
                return Result.Failure(new Error("Property.NotFound", "Property not found."));

            property.AddUnavailableRange(booking.StartDate, booking.EndDate);

            _propertyRepository.Update(property);

            notificationMessage = $"تم قبول حجزك ل '{booking.Name}'. تم خصم رسوم الحجز بقيمة {fee.Amount} {fee.Currency.Code}.";
        }

        else if (request.NewStatus == BookingStatus.Completed)
        {
            notificationMessage = $"تم إكمال حجزك ل '{booking.Name}'.";
        }

        _bookingRepository.Update(booking);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(notificationMessage))
        {
            var notification = Notification.Create(clientId, notificationMessage);
            _notificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
