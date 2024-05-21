using System.Collections;
using Api.Models;
using Api.Settings;

namespace Api.Services;

public class PaycheckCalculator : IPaycheckCalculator
{
    private readonly PaycheckCalculatorSettings _settings;

    public PaycheckCalculator(PaycheckCalculatorSettings settings)
    {
        _settings = settings;
    }

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

        foreach (var deduction in GetDeductions(employee))
        {
            paycheck.Deductions.Add(deduction);
        }
        
        return paycheck;
    }

    private IEnumerable<Deduction> GetDeductions(Employee employee)
    {
        yield return new Deduction
        {
            Amount = ConvertMonthlyAmountToAmountPerPaycheck(_settings.BaseEmployeeCostPerMonth),
            Type = DeductionType.Base
        };

        if (employee.Salary > _settings.HighSalaryThreshold)
        {
            yield return new Deduction
            {
                Amount = _settings.HighSalaryCostPercentagePerYear / 100m * employee.Salary / _settings.PaycheckCountPerYear,
                Type = DeductionType.HighSalary
            };
        }

        foreach (var dependent in employee.Dependents)
        {
            yield return new Deduction
            {
                Amount = ConvertMonthlyAmountToAmountPerPaycheck(_settings.DependentCostPerMonth),
                Type = DeductionType.Dependent
            };
        }
    }

    private decimal ConvertMonthlyAmountToAmountPerPaycheck(decimal monthlyAmount)
    {
        return monthlyAmount * 12m / _settings.PaycheckCountPerYear;
    }

    private decimal CalculateGrossAmount(decimal employeeSalary)
    {
        return employeeSalary / _settings.PaycheckCountPerYear;
    }
}