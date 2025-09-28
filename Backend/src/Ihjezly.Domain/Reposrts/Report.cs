using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reposrts.Events;
using Ihjezly.Domain.Users;

namespace Ihjezly.Domain.NewFolder;

public sealed class Report : Entity
{
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? Replay { get; set; }=string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; set; }

    private Report() : base(Guid.NewGuid()) { }

    private Report( Guid userId, string content, string reason, string replay, bool isRead) : this()
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Report reason cannot be empty.");

        UserId = userId;
        Reason = reason;
        Content = content;
        Reason = reason;
        IsRead = isRead;
        CreatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReportCreatedDomainEvent(Id , UserId,Content, Reason, IsRead, CreatedAt));
    }

    public static Report Create( Guid userId, string reason, string content, string replay, bool isRead)
        => new( userId, content,  reason, replay, isRead);

    public void Update(string reason,string content, bool isRead)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Report reason cannot be empty.");

        Reason = reason;
        RaiseDomainEvent(new ReportUpdatedDomainEvent(Id, UserId, Reason, Content, IsRead));
    }

    public void AddReplay(string replay)
    {
        if (string.IsNullOrWhiteSpace(replay))
            throw new ArgumentException("Replay cannot be empty.");

        Replay = replay;
        var repliedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReportRepliedDomainEvent(Id, UserId, Replay!, repliedAt));
    }

    public void Delete()
    {
        RaiseDomainEvent(new ReportDeletedDomainEvent(Id, UserId));
    }
}