
using UrlShortener.Api.Data;

public abstract class RepositoryBase : IRepository
{
    protected readonly UrlShortenerDbContext _context;

    public RepositoryBase(UrlShortenerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}