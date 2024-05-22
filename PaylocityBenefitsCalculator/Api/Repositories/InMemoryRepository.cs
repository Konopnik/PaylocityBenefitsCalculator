using Api.Models;
using Api.Repositories.Errors;

namespace Api.Repositories;

/// <summary>
/// Class responsible for handling the data access of the Employee entity.
/// </summary>
//note: For now I will just use list to store the data, but in a real application this would be some kind of a database.
// TODO - Replace this with a real database.

//note 2: I decided to use this repository for both interfaces, because it stores both entities and it would be easier to manage for now.
// During the real implementation I would have few questions to do right decisions here and I would even question existence of API for dependents,
// because dependent is always part of some employee and API for employee already returns all dependents:  
// - Does any part of our application list all dependents without employees? if not "Get all dependent" endpoint is not needed.
// - Does any part of our application need to load Dependent without previously loading Employee? if not "Get dependent by id" endpoint is not needed, if yes -> How did this part gets the Dependent Id?
// So it could lead to remove DependentController and its repository - I can explain this more on the interview 
public class InMemoryRepository : IEmployeeRepository, IDependentRepository
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

    public Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_employeesStorage.AsEnumerable());
    }

    async Task<Result<Dependent, NotFoundError>> IDependentRepository.Find(int id, CancellationToken cancellationToken)
    {
        // simulating async call to a database
        var dependent = await Task.FromResult(_employeesStorage.SelectMany(e => e.Dependents).FirstOrDefault(d => d.Id == id));
        if (dependent == null)
        {
            return new NotFoundError();
        }
        return dependent;
    }

    Task<IEnumerable<Dependent>> IDependentRepository.GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_employeesStorage.SelectMany(e => e.Dependents));
    }

    public async Task<Result<Employee, NotFoundError>> Find(int id, CancellationToken cancellationToken = default)
    {
        // simulating async call to a database
        var employee = await Task.FromResult(_employeesStorage.FirstOrDefault(e => e.Id == id));
        if (employee == null)
        {
            return new NotFoundError();
        }
        return employee;
    }
}