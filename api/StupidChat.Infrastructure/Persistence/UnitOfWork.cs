using System.Data;
using Microsoft.EntityFrameworkCore;
using StupidChat.Application.Common;

namespace StupidChat.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        var dbContextTransaction = await dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return new TransactionDecorator(dbContextTransaction);
    }
}