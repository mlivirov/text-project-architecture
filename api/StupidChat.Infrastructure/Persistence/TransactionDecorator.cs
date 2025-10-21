using Microsoft.EntityFrameworkCore.Storage;
using StupidChat.Application.Common;

namespace StupidChat.Infrastructure.Persistence;

public class TransactionDecorator(IDbContextTransaction dbContextTransaction) : ITransaction
{
    public async ValueTask DisposeAsync()
    {
        await dbContextTransaction.DisposeAsync();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContextTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await dbContextTransaction.RollbackAsync(cancellationToken);
    }
}