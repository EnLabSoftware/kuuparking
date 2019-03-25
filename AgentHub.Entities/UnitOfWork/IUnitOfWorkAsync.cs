using System.Threading;
using System.Threading.Tasks;
using AgentHub.Entities.Infrastructure;
using AgentHub.Entities.Repositories;

namespace AgentHub.Entities.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IObjectState;
    }
}