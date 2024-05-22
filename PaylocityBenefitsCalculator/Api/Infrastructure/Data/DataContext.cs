using Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>(); 
    public DbSet<Dependent> Dependents => Set<Dependent>(); 
}