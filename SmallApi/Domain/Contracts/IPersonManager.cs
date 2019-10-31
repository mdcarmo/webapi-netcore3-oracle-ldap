using SmallApi.Domain.Entity;
using SmallApi.Domain.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmallApi.Domain.Contracts
{
    public interface IPersonManager : IRepositoryBase<Person>
    {
        Task<bool> ExistWithThisRegisterAsync(object register);

        Task<Result<IEnumerable<Person>>> GetAllPersonByProcedure();
    }
}
