using Api.Infrastructure;
using Api.Models;
using Api.Repositories.Errors;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class DependentRepository(DataContext dataContext) : IDependentRepository
{
    public async Task<IEnumerable<Dependent>> GetAll(CancellationToken ct)
    {
        return await dataContext.Dependents.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Result<Dependent, NotFoundError>> Find(int id, CancellationToken ct = default)
    {
        var dependent = await dataContext.Dependents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (dependent == null)
        {
            return new NotFoundError();
        }

        return dependent;
    }
}