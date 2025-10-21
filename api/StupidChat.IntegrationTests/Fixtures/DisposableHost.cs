namespace StupidChat.IntegrationTests.Fixtures;

public class DisposableHost<T>(TestWebApplicationFactory factory, T client) : IAsyncDisposable
{
    public TestWebApplicationFactory Factory { get; } = factory;

    public T Client { get; } = client;

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}