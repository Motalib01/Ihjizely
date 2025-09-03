using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reposrts.Events;

namespace Ihjezly.Domain.NewFolder;

public sealed class Report : Entity
{
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Report() : base(Guid.NewGuid()) { }

    private Report( Guid userId, string content, string reason) : this()
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Report reason cannot be empty.");

        UserId = userId;
        Reason = reason;
        Content = content;
        CreatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReportCreatedDomainEvent(Id , UserId,Content, Reason, CreatedAt));
    }

    public static Report Create( Guid userId, string reason, string content)
        => new( userId, content, reason);

    public void Update(string reason,string content)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Report reason cannot be empty.");

        Reason = reason;
        RaiseDomainEvent(new ReportUpdatedDomainEvent(Id, UserId, Reason, Content));
    }

    public void Delete()
    {
        RaiseDomainEvent(new ReportDeletedDomainEvent(Id, UserId));
    }
}