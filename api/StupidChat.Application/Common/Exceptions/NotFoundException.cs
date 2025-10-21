namespace StupidChat.Application.Common.Exceptions;

public class NotFoundException(Type type, object id)
    : ApplicationException($"Entity of type {type.Name} with id {id} not found.")
{
    public Type Type { get; } = type;

    public object Id { get; } = id;
}