using BillingAgreementService.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StupidChat.Application.Chat.SendMessage.Models;
using StupidChat.Application.Common;
using StupidChat.Application.Common.Exceptions;
using StupidChat.Application.Common.Extensions;
using StupidChat.Domain.Entities;
using AccessViolationException = System.AccessViolationException;

namespace StupidChat.Application.Chat.SendMessage;

public class SendMessageCommandHandler(
    IRepository<Domain.Entities.Chat> chatRepository,
    IRepository<ChatMessage> chatMessageRepository,
    ICurrentUserAccessor currentUserAccessor,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    IDistributedCache cache
) : IRequestHandler<SendMessageCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(SendMessageCommand request, CancellationToken cancellationToken)
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

        var newMessage = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            CreatedAt = dateTimeProvider.Now,
            ChatId = request.ChatId,
            Question = request.Question,
            Answer = new MessageContent()
            {
                Content = "I don't know"
            }
        };

        await chatMessageRepository.AddAsync(newMessage, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResponseModel
        {
            Answer = newMessage.Answer,
            CreatedAt = newMessage.CreatedAt,
            Id = newMessage.Id,
        };
    }
}