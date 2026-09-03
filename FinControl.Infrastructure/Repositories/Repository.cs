using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly FinControlDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(FinControlDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);
}
