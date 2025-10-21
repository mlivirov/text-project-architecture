using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StupidChat.Application.Chat.CreateChat;
using StupidChat.Application.Chat.GetMessages;
using StupidChat.Application.Chat.GetMessages.Models;
using StupidChat.Application.Chat.SendMessage;
using StupidChat.Application.Chat.SendMessage.Models;
using StupidChat.Application.Common.Models;
using StupidChat.Domain.Entities;

namespace StupidChat.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<Guid> CreateChat(CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateChatCommand(), cancellationToken);
    }

    [HttpPost("{chatId}")]
    public async Task<ResponseModel> AskQuestion(Guid chatId, [FromBody] MessageContent content, CancellationToken cancellationToken)
    {
        return await sender.Send(new SendMessageCommand()
        {
            Question = content,
            ChatId = chatId
        }, cancellationToken);
    }

    [HttpGet("{chatId}")]
    public async Task<Page<MessageModel>> GetMessages(Guid chatId, int take, int skip, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetMessagesQuery()
        {
            ChatId = chatId,
            Pagination = new Pagination()
            {
                Take = take,
                Skip = skip
            }
        }, cancellationToken);
    }
}