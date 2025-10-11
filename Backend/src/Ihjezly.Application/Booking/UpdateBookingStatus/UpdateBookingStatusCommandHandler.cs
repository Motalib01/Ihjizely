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
using Ihjezly.Domain.Users.Repositories;

public sealed class UpdateBookingStatusCommandHandler : ICommandHandler<UpdateBookingStatusCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UpdateBookingStatusCommandHandler(
        IBookingRepository bookingRepository,
        IWalletRepository walletRepository,
        INotificationRepository notificationRepository,
        ITransactionRepository transactionRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _walletRepository = walletRepository;
        _notificationRepository = notificationRepository;
        _transactionRepository = transactionRepository;
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(BookingErrors.NotFound);

        var property = await _propertyRepository.GetByIdNonGeniricAsync(booking.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(new Error("Property.NotFound", "Property not found."));

        var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
        if (owner is null)
            return Result.Failure(new Error("Owner.NotFound", "Business owner not found."));

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
            // Refund the 20 LYD
            var wallet = await _walletRepository.GetByUserIdAsync(clientId, cancellationToken);
            if (wallet is null)
                return Result.Failure(WalletErrors.NotFound);

        }
        else if (request.NewStatus == BookingStatus.Confirmed)
        {
            // 1. Deduct 20 LYD from client's wallet
            var wallet = await _walletRepository.GetByUserIdAsync(clientId, cancellationToken);
            if (wallet is null)
                return Result.Failure(WalletErrors.NotFound);

            var bookingFee = new Money(20, Currency.FromCode("LYD"));

            wallet.DeductFunds(bookingFee);
            _walletRepository.Update(wallet);

            var paymentTransaction = Transaction.Create(wallet.Id, bookingFee, "Booking confirmation fee");
            _transactionRepository.Add(paymentTransaction);

            // 2. Mark property dates as unavailable
            property.AddUnavailableRange(booking.StartDate, booking.EndDate);
            _propertyRepository.Update(property);

            // 3. Notification with optional owner phone
            notificationMessage = $"تم قبول حجزك ل '{booking.Name}'. تم خصم مبلغ {bookingFee.Amount} {bookingFee.Currency.Code} من محفظتك.";
            if (property is RestHouse || property is Apartment || property is Chalet)
            {
                notificationMessage += $" للتواصل مع صاحب العقار: {owner.PhoneNumber}";
            }

            // 4. Refuse all overlapping bookings
            var overlappingBookings = await _bookingRepository.GetOverlappingBookingsAsync(
                booking.PropertyId,
                booking.StartDate,
                booking.EndDate,
                cancellationToken
            );

            foreach (var otherBooking in overlappingBookings)
            {
                if (otherBooking.Id == booking.Id)
                    continue;

                if (otherBooking.Status != BookingStatus.Pending)
                    continue;

                otherBooking.ChangeStatusTo(BookingStatus.Rejected);

                var otherNotification = Notification.Create(
                    otherBooking.ClientId,
                    $"تم رفض حجزك ل '{otherBooking.Name}' بسبب تعارض مع حجز آخر مؤكد."
                );
                _notificationRepository.Add(otherNotification);

                _bookingRepository.Update(otherBooking);
            }
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
