namespace StupidChat.Application.Common.Models;

public class Page<T>
{
    public long Total { get; set; }

    public required ICollection<T> Items { get; set; }
}