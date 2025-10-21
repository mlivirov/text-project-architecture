using BillingAgreementService.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StupidChat.Application.Chat.GetMessages.Models;
using StupidChat.Application.Common;
using StupidChat.Application.Common.Exceptions;
using StupidChat.Application.Common.Extensions;
using StupidChat.Application.Common.Models;
using StupidChat.Domain.Entities;
using AccessViolationException = System.AccessViolationException;

namespace StupidChat.Application.Chat.GetMessages;

public class GetMessagesQueryHandler(
    IRepository<ChatMessage> chatMessagesRepository,
    IRepository<Domain.Entities.Chat> chatRepository,
    ICurrentUserAccessor currentUserAccessor,
    IDistributedCache cache
) : IRequestHandler<GetMessagesQuery, Page<MessageModel>>
{
    public async Task<Page<MessageModel>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var chat = await cache.GetObjectAsync<Domain.Entities.Chat>(request.ChatId.ToString(), cancellationToken)
                   ?? await chatRepository.AsQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ChatId, cancellationToken);

        if (chat is null)
        {
            throw new NotFoundException(typeof(Domain.Entities.Chat), request.ChatId);
        }

        if (chat.UserId != currentUserAccessor.UserId)
        {
            throw new AccessViolationException("Chat doesn't belong to the user");
        }

        var messages = chatMessagesRepository.AsQueryable().AsNoTracking()
            .Where(x => x.ChatId == request.ChatId);

        var total = await messages.CountAsync(cancellationToken);
        var page = await messages
            .OrderBy(x => x.CreatedAt)
            .Skip(request.Pagination.Skip)
            .Take(request.Pagination.Take)
            .Select(t => new MessageModel()
            {
                Id = t.Id,
                Answer = t.Answer,
                Question = t.Question,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new Page<MessageModel>
        {
            Total = total,
            Items = page
        };
    }
}