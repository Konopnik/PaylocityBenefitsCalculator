using Api.Models;

namespace Api.Repositories;

public interface IEmployeeRepository
{

    //note: making this async ready right away, so it can be easily replaced with a real async call to a database.
    Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken = default);
}