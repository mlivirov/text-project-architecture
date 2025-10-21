using BillingAgreementService.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace StupidChat.Infrastructure.Persistence;

public class Repository<T>(ApplicationDbContext dbContext) : IRepository<T>
    where T : Entity
{
    public IQueryable<T> AsQueryable()
    {
        return dbContext.Set<T>();
    }

    public IQueryable<T> FromSqlInterpolated(FormattableString sql)
    {
        return dbContext.Set<T>().FromSqlInterpolated(sql);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }
}