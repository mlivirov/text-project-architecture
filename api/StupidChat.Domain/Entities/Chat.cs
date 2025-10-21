using BillingAgreementService.Domain.Common;

namespace StupidChat.Domain.Entities;

public class Chat : Entity
{
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
}