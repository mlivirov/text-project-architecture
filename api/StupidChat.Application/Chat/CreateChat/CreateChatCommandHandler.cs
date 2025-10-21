using BillingAgreementService.Domain.Common;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using StupidChat.Application.Common;
using StupidChat.Application.Common.Extensions;

namespace StupidChat.Application.Chat.CreateChat;

public class CreateChatCommandHandler(
    ICurrentUserAccessor currentUserAccessor, 
    IUnitOfWork unitOfWork, 
    IRepository<Domain.Entities.Chat> chatRepository,
    IDateTimeProvider dateTimeProvider,
    IDistributedCache cache
) : IRequestHandler<CreateChatCommand, Guid>
{
    public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var newChat = new Domain.Entities.Chat()
        {
            CreatedAt = dateTimeProvider.Now,
            Id = Guid.NewGuid(),
            UserId = currentUserAccessor.UserId
        };

        await chatRepository.AddAsync(newChat, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.SetObjectAsync(newChat.Id.ToString(), newChat, new DistributedCacheEntryOptions(), cancellationToken);

        return newChat.Id;
    }
}