using Microsoft.EntityFrameworkCore;
using VIviendaYa.API.Shared.Domain.Repositories;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Base generic repository and unit of work implementation for Entity Framework Core.
/// </summary>
public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext Context;

    public BaseRepository(AppDbContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Adds an entity asynchronously to the database context.
    /// </summary>
    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
    }

    /// <summary>
    /// Finds an entity by integer ID (not recommended if TId is not int).
    /// </summary>
    public Task<TEntity?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Finds an entity by its ID (supports custom ID types).
    /// </summary>
    public async Task<TEntity?> FindByIdAsync(TId id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    /// <summary>
    /// Removes an entity from the context.
    /// </summary>
    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    /// <summary>
    /// Returns a list of all entities.
    /// </summary>
    public async Task<IEnumerable<TEntity>> ListAsync()
    {
        return await Context.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    /// Saves all pending changes in the current unit of work.
    /// </summary>
    public async Task CompleteAsync()
    {
        await Context.SaveChangesAsync();
    }
}
