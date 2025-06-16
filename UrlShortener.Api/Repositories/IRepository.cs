public interface IRepository
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}