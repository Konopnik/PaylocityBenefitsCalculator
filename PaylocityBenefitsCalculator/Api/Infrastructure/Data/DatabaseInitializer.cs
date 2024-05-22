using Api.Core.Entities;
using Api.Core.Enums;

namespace Api.Infrastructure.Data;

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

// note3: since I introduced repositories which uses EF, I will rename this class and use it for database seeding only 
public class DatabaseInitializer
{
    private readonly DataContext _context;

    private readonly List<Employee> _initialEmployees = new List<Employee>
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

    public DatabaseInitializer(DataContext context)
    {
        _context = context;
    }

    public async Task Initialize(CancellationToken ct = default)
    {
        if (!_context.Employees.Any())
        {
            _context.Employees.AddRange(_initialEmployees);
            await _context.SaveChangesAsync(ct);
        }
    }
}