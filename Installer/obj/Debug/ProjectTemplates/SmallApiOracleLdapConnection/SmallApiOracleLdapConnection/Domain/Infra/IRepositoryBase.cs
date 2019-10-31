using System.Collections.Generic;
using System.Threading.Tasks;

namespace $safeprojectname$.Domain.Infra
{
    public interface IRepositoryBase<T>
    {
        Task<Result<IEnumerable<T>>> GetAllAsync();
        Task<Result<T>> GetByIdAsync(object id);
        Task<Result<int>> CreateAsync(T entity);
        Task<Result<bool>> UpdateAsync(T entity);
        Task<Result<bool>> DeleteAsync(object id);
        Task<bool> ExistAsync(object id);
    }
}
