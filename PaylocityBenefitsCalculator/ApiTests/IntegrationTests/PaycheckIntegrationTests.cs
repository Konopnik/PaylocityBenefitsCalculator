using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Api.Core.Enums;
using Api.Presentation.Dtos.Dependent;
using Api.Presentation.Dtos.Employee;
using Api.Presentation.Dtos.Paycheck;
using Xunit;

namespace ApiTests.IntegrationTests;

public class PaycheckIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForSpecificEmployeePaycheck_ShouldReturnPaycheckWithCorrectAmounts()
    {
        var response = await HttpClient.GetAsync("/api/v1/paycheck/2024/2/employee/1");
        var paycheck = new GetPaycheckDto()
        {
            Year = 2024,
            Employee = new GetEmployeeDto()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            Number = 2,
            // note: some rounding of the amounts would look better => I would ask if we need it or not during implementation - for this example I will not round it.
            GrossAmount = 2_900.8073076923076923076923077m,
            DeductionsAmount = 461.53846153846153846153846154m,
            NetAmount = 2439.2688461538461538461538462m,
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }    
    
    [Fact]
    public async Task WhenAskedForSpecificEmployeeWithDependentsPaycheck_ShouldReturnPaycheckWithCorrectAmounts()
    {
        var response = await HttpClient.GetAsync("/api/v1/paycheck/2024/2/employee/2");
        var paycheck = new GetPaycheckDto()
        {
            Year = 2024,
            Employee = new GetEmployeeDto()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
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
            Number = 2,
            GrossAmount = 3_552.5084615384615384615384615m,
            DeductionsAmount = 1_363.3578615384615384615384615m,
            NetAmount = 2189.1506000000000000000000000m,
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }    
    
    
    [Fact]
    public async Task WhenAskedForSpecificEmployeeWithDependentOlderThan50Yeard_ShouldReturnPaycheckWithCorrectAmounts()
    {
        var response = await HttpClient.GetAsync("/api/v1/paycheck/2024/2/employee/3");
        var paycheck = new GetPaycheckDto()
        {
            Year = 2024,
            Employee = new()
            {
            Id = 3,
            FirstName = "Michael",
            LastName = "Jordan",
            Salary = 143211.12m,
            DateOfBirth = new DateTime(1963, 2, 17),
            Dependents = new List<GetDependentDto>
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
        },
            Number = 2,
            GrossAmount = 5_508.12m,
            DeductionsAmount = 940.9316307692307692307692308m,
            NetAmount = 4_567.1883692307692307692307692m,
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }    
    
    [Fact]
    public async Task WhenAskedForPaycheckForNonExistingEmployee_ShouldReturnNotFound()
    {
        var response = await HttpClient.GetAsync($"/api/v1/paycheck/2024/2/employee/{int.MinValue}");
        await response.ShouldReturnErrorCode(HttpStatusCode.NotFound, "EMPLOYEE_NOT_FOUND");
    }
}