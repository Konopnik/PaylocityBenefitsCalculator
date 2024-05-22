using Api.Models;
using Api.Repositories.Errors;

namespace Api.Repositories;

public interface IDependentRepository
{
    Task<IEnumerable<Dependent>> GetAll(CancellationToken cancellationToken);
    Task<Result<Dependent, NotFoundError>> Find(int id, CancellationToken cancellationToken);
}