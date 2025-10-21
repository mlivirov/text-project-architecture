using FluentValidation;

namespace StupidChat.Application.Chat.GetMessages;

public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesQueryValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("ChatId is required.");

        RuleFor(x => x.Pagination.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Take must be between 1 and 100.");

        RuleFor(x => x.Pagination.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be 0 or greater.");
    }
}