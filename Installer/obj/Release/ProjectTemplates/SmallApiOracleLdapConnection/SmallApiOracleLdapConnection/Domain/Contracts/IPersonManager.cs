using $safeprojectname$.Domain.Entity;
using $safeprojectname$.Domain.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace $safeprojectname$.Domain.Contracts
{
    public interface IPersonManager : IRepositoryBase<Person>
    {
        Task<bool> ExistWithThisRegisterAsync(object register);

        Task<Result<IEnumerable<Person>>> GetAllPersonByProcedure();
    }
}
