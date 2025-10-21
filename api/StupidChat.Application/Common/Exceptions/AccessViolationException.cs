namespace StupidChat.Application.Common.Exceptions;

public class AccessViolationException(string message) : System.ApplicationException(message)
{
}