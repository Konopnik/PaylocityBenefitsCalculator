using Api.Models;

namespace Api.Services;

public class PaycheckCalculator : IPaycheckCalculator
{
    public async Task<Paycheck> Calculate(int year, int number, Employee employee)
    {
        //note: starting with just simple implementation to return paycheck with 0 amounts to have working endpoint
        var paycheck = new Paycheck()
        {
            Year = year,
            Employee = employee,
            Number = number,
            GrossAmount = CalculateGrossAmount(employee.Salary),
            NetAmount = 0,
        };
        paycheck.Deductions.Add(new Deduction() { Amount = 1000, Type = DeductionType.Base});
        return paycheck;
    }

    private decimal CalculateGrossAmount(decimal employeeSalary)
    {
        return employeeSalary / 26m;
    }
}