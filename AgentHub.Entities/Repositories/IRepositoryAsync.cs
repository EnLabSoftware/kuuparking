using System.Threading;
using System.Threading.Tasks;
using AgentHub.Entities.DataContext;
using AgentHub.Entities.Infrastructure;

namespace AgentHub.Entities.Repositories
{
    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : class, IObjectState
    {
        IDataContextAsync Context { get; }

        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}