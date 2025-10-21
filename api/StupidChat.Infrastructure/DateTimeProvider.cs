using StupidChat.Application.Common;

namespace StupidChat.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}