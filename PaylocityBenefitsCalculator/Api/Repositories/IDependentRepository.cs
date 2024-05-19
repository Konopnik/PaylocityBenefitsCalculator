using Api.Models;

namespace Api.Repositories;

public interface IDependentRepository
{
    Task<IEnumerable<Dependent>> GetAll(CancellationToken cancellationToken);
    Task<Dependent?> Find(int id, CancellationToken cancellationToken);
}