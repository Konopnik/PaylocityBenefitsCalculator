using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.Core.Enums;
using Api.Presentation.Dtos.Dependent;
using Api.Presentation.Dtos.Employee;
using Api.UseCases.Employees;
using FluentAssertions;
using Xunit;

namespace ApiTests.IntegrationTests;

public class EmployeeIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees");
        var employees = new List<GetEmployeeDto>
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
            new()
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
            }
        };
        await response.ShouldReturn(HttpStatusCode.OK, employees);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/1");
        var employee = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        };
        await response.ShouldReturn(HttpStatusCode.OK, employee);
    }
    
    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/employees/{int.MinValue}");
        await response.ShouldReturnErrorCode(HttpStatusCode.NotFound, "EMPLOYEE_NOT_FOUND");
    }

    [Fact]
    public async Task WhenAskedForCreateNewEmployee_EmployeeShouldBeCreated()
    {
        var newEmployee = new CreateEmployeeCommand("TestFN", "TestLN", 80000, new DateTime(2020, 1, 1),
            new List<DependentDto>()
            {
                new DependentDto("ChildFN", "ChildLN", Relationship.Child),
                new DependentDto("PartnerFN", "PartnerLN", Relationship.DomesticPartner)
            });
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/employees/", newEmployee);
        
       var createdEmployee = await response.ShouldReturn<GetEmployeeDto>(HttpStatusCode.Created);
       createdEmployee.Data.Should().NotBeNull();
       createdEmployee.Data.Should().BeEquivalentTo(newEmployee);
       
       response = await HttpClient.GetAsync($"/api/v1/employees/{createdEmployee.Data!.Id}");

       await response.ShouldReturn(HttpStatusCode.OK, createdEmployee.Data);
    }    
    
    [Theory]
    [InlineData(Relationship.DomesticPartner, Relationship.DomesticPartner)]
    [InlineData(Relationship.Spouse, Relationship.DomesticPartner)]
    [InlineData(Relationship.Spouse, Relationship.Spouse)]
    public async Task WhenAskedForCreateNewEmployeeWithTwoPartnerDependents_ItShouldReturnValidationError(Relationship firstPartnerRelationship, Relationship secondPartnerRelationship)
    {
        var newEmployee = new CreateEmployeeCommand("TestFN", "TestLN", 80000, new DateTime(2020, 1, 1),
            new List<DependentDto>()
            {
                new DependentDto("FirstPartnerFN", "FirstPartnerLN", firstPartnerRelationship),
                new DependentDto("SecondPartnerFN", "SecondPartnerLN", secondPartnerRelationship)
            });
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/employees/", newEmployee);
        
       await response.ShouldReturnErrorCode(HttpStatusCode.BadRequest, "VALIDATION_ERROR");
    }
}

