using Api.Models;

namespace Api.Repositories;

/// <summary>
/// Class responsible for handling the data access of the Employee entity.
/// </summary>
//note: For now I will just use list to store the data, but in a real application this would be some kind of a database.
// TODO - Replace this with a real database.
public class EmployeeRepositoryInMemory : IEmployeeRepository
{
    private readonly List<Employee> _employeesStorage = new List<Employee>
    {
        new()
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        },
        new()
        {
            Id = 2,
            FirstName = "Ja",
            LastName = "Morant",
            Salary = 92365.22m,
            DateOfBirth = new DateTime(1999, 8, 10),
            Dependents = new List<Dependent>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Spouse",
                    LastName = "Morant",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1998, 3, 3)
                },
                new()
                {
                    Id = 2,
                    FirstName = "Child1",
                    LastName = "Morant",
                    Relationship = Relationship.Child,
                    DateOfBirth = new DateTime(2020, 6, 23)
                },
                new()
                {
                    Id = 3,
                    FirstName = "Child2",
                    LastName = "Morant",
                    Relationship = Relationship.Child,
                    DateOfBirth = new DateTime(2021, 5, 18)
                }
            }
        },
        new()
        {
            Id = 3,
            FirstName = "Michael",
            LastName = "Jordan",
            Salary = 143211.12m,
            DateOfBirth = new DateTime(1963, 2, 17),
            Dependents = new List<Dependent>
            {
                new()
                {
                    Id = 4,
                    FirstName = "DP",
                    LastName = "Jordan",
                    Relationship = Relationship.DomesticPartner,
                    DateOfBirth = new DateTime(1974, 1, 2)
                }
            }
        }
    };
    
    public async Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_employeesStorage.AsEnumerable());
    }

    public Task<Employee?> Find(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_employeesStorage.FirstOrDefault(e => e.Id == id));
    }
}