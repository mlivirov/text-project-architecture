using FluentValidation;

namespace StupidChat.Application.Chat.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("ChatId is required.");

        RuleFor(x => x.Question)
            .NotNull()
            .WithMessage("Question is required.");

        RuleFor(x => x.Question.Content)
            .NotEmpty()
            .WithMessage("Message content is required.")
            .MaximumLength(2000)
            .WithMessage("Message content cannot exceed 2000 characters.");
    }
}