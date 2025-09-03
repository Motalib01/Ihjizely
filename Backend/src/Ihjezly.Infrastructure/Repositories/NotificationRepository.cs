using Ihjezly.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(Notification notification) => _dbContext.Notifications.Add(notification);

    public void Update(Notification notification) => _dbContext.Notifications.Update(notification);

    public void Delete(Notification notification) => _dbContext.Notifications.Remove(notification);

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.Notifications.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
}