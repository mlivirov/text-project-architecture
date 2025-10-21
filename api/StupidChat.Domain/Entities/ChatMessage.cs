using BillingAgreementService.Domain.Common;

namespace StupidChat.Domain.Entities;

public class ChatMessage : Entity
{
    public DateTime CreatedAt { get; set; }

    public Guid ChatId { get; set; }

    public required MessageContent Question { get; set; }

    public required MessageContent Answer { get; set; }
}