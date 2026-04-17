namespace Repositories;

using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
public class Repository<T> : IRepository<T>
    where T : class
{
    private readonly DbSet<T> dbSet;

    public Repository(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.dbSet = context.Set<T>();
    }

    protected DbSet<T> DbSet => this.dbSet;

    public async Task<T?> GetByIdAsync(int id)
    {
        return await this.dbSet.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await this.dbSet.ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return await this.dbSet.Where(predicate).ToListAsync().ConfigureAwait(false);
    }

    public async Task AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await this.dbSet.AddAsync(entity).ConfigureAwait(false);
    }

    public Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return await this.dbSet.AnyAsync(predicate).ConfigureAwait(false);
    }
}
#nullable restore