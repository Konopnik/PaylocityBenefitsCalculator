using Api.Core.Entities;
using Api.Core.Errors;
using Api.Core.Models;
using Api.Core.Repositories;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

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

    public async Task<Employee> Add(Employee employee, CancellationToken cancellationToken)
    {
        dataContext.Employees.Add(employee);
        await dataContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    private IQueryable<Employee> GetEmployeesWithDependentsAsNoTracking()
    {
        return dataContext.Employees.Include(e=>e.Dependents).AsNoTracking();
    }
}