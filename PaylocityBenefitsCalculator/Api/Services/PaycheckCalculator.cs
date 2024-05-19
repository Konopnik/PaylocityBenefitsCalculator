using Api.Models;

namespace Api.Services;

public class PaycheckCalculator : IPaycheckCalculator
{
    public async Task<Paycheck> Calculate(int year, int number, Employee employee)
    {
        //note: starting with just simple implementation to return paycheck with 0 amounts to have working endpoint
        return new Paycheck()
        {
            Year = year,
            Employee = employee,
            Number = number,
            GrossAmount = CalculateGrossAmount(employee.Salary),
            DeductionsAmount = 0,
            NetAmount = 0,
        };
    }

    private decimal CalculateGrossAmount(decimal employeeSalary)
    {
        return employeeSalary / 26m;
    }
}