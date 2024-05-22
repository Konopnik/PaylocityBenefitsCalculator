using Api.Infrastructure;
using Api.Models;
using Api.Repositories.Errors;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class EmployeeRepository(DataContext dataContext) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAll(CancellationToken ct)
    {
        return await GetEmployeesWithDependentsAsNoTracking().ToListAsync(ct);
    }

    public async Task<Result<Employee, NotFoundError>> Find(int id, CancellationToken ct = default)
    {
        var employee = await GetEmployeesWithDependentsAsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee == null)
        {
            return new NotFoundError();
        }

        return employee;
    }

    private IQueryable<Employee> GetEmployeesWithDependentsAsNoTracking()
    {
        return dataContext.Employees.Include(e=>e.Dependents).AsNoTracking();
    }
}