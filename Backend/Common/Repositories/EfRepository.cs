using CampusServicesPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Common.Repositories;

public class EfRepository<TEntity>(ApplicationDbContext dbContext)
    : IRepository<TEntity>
    where TEntity : class
{
    protected ApplicationDbContext DbContext { get; } = dbContext;
    protected DbSet<TEntity> Entities { get; } = dbContext.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await Entities.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyCollection<TEntity>> ListAsync(
        CancellationToken cancellationToken = default) =>
        await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default) =>
        await Entities.AddAsync(entity, cancellationToken);

    public virtual void Update(TEntity entity) => Entities.Update(entity);
    public virtual void Remove(TEntity entity) => Entities.Remove(entity);

    public virtual Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default) =>
        DbContext.SaveChangesAsync(cancellationToken);
}
