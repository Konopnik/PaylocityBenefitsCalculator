using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>(); 
    public DbSet<Dependent> Dependents => Set<Dependent>(); 
}