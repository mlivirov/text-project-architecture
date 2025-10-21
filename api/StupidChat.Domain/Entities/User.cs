using BillingAgreementService.Domain.Common;

namespace StupidChat.Domain.Entities;

public class User : Entity
{
    public required string UserName { get; set; }
}