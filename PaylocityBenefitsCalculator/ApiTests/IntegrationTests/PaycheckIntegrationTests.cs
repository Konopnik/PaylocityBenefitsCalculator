using System;
using System.Net;
using System.Threading.Tasks;
using Api.Dtos.Employee;
using Api.Dtos.Paycheck;
using Xunit;

namespace ApiTests.IntegrationTests;

public class PaycheckIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForSpecificEmployeePaycheck_ShouldReturnPaycheck()
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
            GrossAmount = 2_900.8073076923076923076923077m,
            DeductionsAmount = 461.53846153846153846153846154m,
            NetAmount = 2439.2688461538461538461538462m,
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