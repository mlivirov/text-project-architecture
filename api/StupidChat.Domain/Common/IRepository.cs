namespace BillingAgreementService.Domain.Common;

public interface IRepository<T>
    where T : Entity
{
    IQueryable<T> AsQueryable();

    IQueryable<T> FromSqlInterpolated(FormattableString sql);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
}