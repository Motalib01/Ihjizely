using Ihjezly.Domain.Notifications;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(Notification notification);
    void Update(Notification notification);
    void Delete(Notification notification);
}