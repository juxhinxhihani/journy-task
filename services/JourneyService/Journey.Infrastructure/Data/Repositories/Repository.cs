using Microsoft.EntityFrameworkCore;

namespace Journey.Infrastructure.Data.Repositories;

public abstract class Repository<T, TKey> where T : class
{
    protected readonly ApplicationDbContext _context;

    protected Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Add(entity);
    }
    public void AddRange(List<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }
    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<bool> Exists(TKey id)
    {
        var entity = await Get(id);
        return entity != null;
    }

    public async Task<T?> Get(TKey id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAll()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void UpdateRange(List<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }
}