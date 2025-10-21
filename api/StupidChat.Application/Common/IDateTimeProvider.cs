namespace StupidChat.Application.Common;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}