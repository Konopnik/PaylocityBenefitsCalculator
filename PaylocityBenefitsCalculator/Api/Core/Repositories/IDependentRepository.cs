using Api.Core.Entities;
using Api.Core.Errors;
using Api.Core.Models;

namespace Api.Core.Repositories;

public interface IDependentRepository
{
    Task<IEnumerable<Dependent>> GetAll(CancellationToken cancellationToken);
    Task<Result<Dependent, NotFoundError>> Find(int id, CancellationToken cancellationToken);
}