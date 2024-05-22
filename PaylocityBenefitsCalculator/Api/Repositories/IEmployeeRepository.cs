using Api.Models;
using Api.Repositories.Errors;

namespace Api.Repositories;

public interface IEmployeeRepository
{
    //note: making this async ready right away, so it can be easily replaced with a real async call to a database.
    Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken);
    Task<Result<Employee, NotFoundError>> Find(int id, CancellationToken cancellationToken);
}