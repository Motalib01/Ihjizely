using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Notifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ihjezly.Infrastructure.BackgroundJobs;

public sealed class BookingCompletionService : BackgroundService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingCompletionService> _logger;

    public BookingCompletionService(
        IBookingRepository bookingRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookingCompletionService> logger)
    {
        _bookingRepository = bookingRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BookingCompletionService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CompleteExpiredBookingsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BookingCompletionService");
            }

            // Run every hour (you can change to 10 minutes if needed)
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CompleteExpiredBookingsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var expiredBookings = await _bookingRepository.GetExpiredConfirmedBookingsAsync(now, cancellationToken);
        if (!expiredBookings.Any())
        {
            _logger.LogInformation("No expired bookings found at {Time}", now);
            return;
        }

        foreach (var booking in expiredBookings)
        {
            try
            {
                booking.ChangeStatusTo(BookingStatus.Completed);

                var notification = Notification.Create(
                    booking.ClientId,
                    $"تم إكمال حجزك ل '{booking.Name}' تلقائيًا بعد انتهاء المدة."
                );

                _notificationRepository.Add(notification);
                _bookingRepository.Update(booking);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not complete booking {BookingId}", booking.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Marked {Count} bookings as completed.", expiredBookings.Count);
    }
}