using Api.Models;
using Api.Settings;

namespace Api.Services;

// note: I would guess that there should be some Tax deduction as well, but it is not part of the requirements => I am not implementing it now
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
        };

        foreach (var deduction in GetDeductions(year, number, employee))
        {
            paycheck.Deductions.Add(deduction);
        }
        
        paycheck.NetAmount = paycheck.GrossAmount - paycheck.Deductions.Sum(d => d.Amount);
        return paycheck;
    }

    private IEnumerable<Deduction> GetDeductions(int year, int number, Employee employee)
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
            yield return GetDependentDeduction(year, number, dependent);
        }
    }

    private Deduction GetDependentDeduction(int year, int number, Dependent dependent)
    {
        var dependentCostPerMonth = _settings.DependentCostPerMonth + GetDependentAdditionalCost(year, number, dependent);

        return new Deduction
        {
            Amount = ConvertMonthlyAmountToAmountPerPaycheck(dependentCostPerMonth),
            Type = DeductionType.Dependent
        };
    }

    private decimal GetDependentAdditionalCost(int year, int number, Dependent dependent)
    {
        var age = CalculateAge(dependent, year, number);

        var additionalCost = age >= _settings.OlderDependentAgeThreshold ? _settings.OlderDependentAdditionalCost : 0;
        return additionalCost;
    }

    private int CalculateAge(Dependent dependent, int year, int number)
    {
        //note: I am not sure when the correct  paycheck date is => just creating simple implementation - because I do not want to spend much time on this...
        //note2: I am even not sure if this is required for age calculation. I can see two options how it could be handled:
        // - calculate age according to paycheck date
        // - calculate age according to year of the paycheck
        // => I decided to calculate age according to paycheck date...
        // During real implementation I would ask for clarification, which option is correct and how to calculate the paycheck date correctly - here I am just counting days per paycheck in the year and adding it to the 1st of January of the year
        var daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
        var daysPerPayCheck = daysInYear / _settings.PaycheckCountPerYear;
        var payCheckDate =new DateTime(year, 1, 1).AddDays(number * daysPerPayCheck);
        return AgeCalculator.Calculate(dependent.DateOfBirth, payCheckDate);
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
