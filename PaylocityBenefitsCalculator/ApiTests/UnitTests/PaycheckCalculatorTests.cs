using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Services;
using Api.Settings;
using FluentAssertions;
using Xunit;

namespace ApiTests.UnitTests;

// note: I used TDD approach for implementation of PaycheckCalculator - see commits history
public class PaycheckCalculatorTests
{
    private static PaycheckCalculator GetPaycheckCalculator(PaycheckCalculatorSettings settings)
    {
        return new PaycheckCalculator(settings);
    }

    private static PaycheckCalculator GetPaycheckCalculator()
    {
        return GetPaycheckCalculator(new PaycheckCalculatorSettings());
    }

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnSomePaycheck()
    {
        var paycheckCalculator = GetPaycheckCalculator();
        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee());

        paycheck.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(GrossAmountTestCases))]
    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithCorrectGrossAmount(
        decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.GrossAmount.Should().BeApproximately(expectedGrossAmount, 0.01m);
    }

    public static TheoryData<decimal, decimal> GrossAmountTestCases = new()
    {
        { 26_000m, 1_000m },
        { 10_000m, 384.62m },
        { 0m, 0m },
    };

    [Theory]
    [MemberData(nameof(GrossAmountWithConfiguredPaycheckCountTestCases))]
    public async Task WhenAskedForCalculationOfPaycheckWithChangedPaycheckCount_ShouldReturnPaycheckWithCorrectGrossAmount(int paycheckCountPerYear, decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator(new PaycheckCalculatorSettings() { PaycheckCountPerYear = paycheckCountPerYear });

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.GrossAmount.Should().BeApproximately(expectedGrossAmount, 0.01m);
    }

    public static TheoryData<int, decimal, decimal> GrossAmountWithConfiguredPaycheckCountTestCases = new()
    {
        { 26, 26_000m, 1_000m },
        { 12, 12_000m, 1_000m },
    };

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithBaseEmployeeDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 70_000 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(1);
        var deduction = paycheck.Deductions.First();
        deduction.Amount.Should().BeApproximately(461.53m, 0.1m);
        deduction.Type.Should().Be(DeductionType.Base);
    }

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithoutAnyDependants_ShouldReturnPaycheckWithoutAnyDependantDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 70_000 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(d => d.Type == DeductionType.Dependent);
    }
    
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithDependants_ShouldReturnPaycheckWithDependantDeductions()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 70_000,
            Dependents = new List<Dependent>
            {
                new()
                {
                    Relationship = Relationship.Child,
                    DateOfBirth = DateTime.Now.AddYears(-5)
                },
                new()
                {
                    Relationship = Relationship.Child,
                    DateOfBirth = DateTime.Now.AddYears(-8)
                },
                new()
                {
                    Relationship = Relationship.Spouse,
                    DateOfBirth = DateTime.Now.AddYears(-35)
                }
            }
        };
        
        var paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(4);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(276.92m, 0.01m));
    }    
    
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithDependantOlderThanThreshold_ShouldReturnPaycheckWithAdditionalDependantDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 70_000,
            Dependents = new List<Dependent>
            {
                new()
                {
                    Relationship = Relationship.Child,
                    DateOfBirth = DateTime.Now.AddYears(-55)
                }
            }
        };
        
        var paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(369.23m, 0.01m));
    }        
    
    [Theory]
    [MemberData(nameof(A))]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithDependantsWithVariousDateOfBirth_ShouldReturnPaycheckWithCorrectDependentDeduction(DateTime dateOfBirth, int year, int paycheckNumber, decimal expectedDeductionAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 70_000,
            Dependents = new List<Dependent>
            {
                new()
                {
                    Relationship = Relationship.Child,
                    DateOfBirth = dateOfBirth
                }
            }
        };
        
        var paycheck = await paycheckCalculator.Calculate(year, paycheckNumber, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(expectedDeductionAmount, 0.01m));
    }    
    
    public static TheoryData<string, int, int, decimal> A = new()
    {
        { "1974-01-01", 2024, 1, 369.23m },
        { "1974-02-01", 2024, 1, 276.92m },
        { "1974-02-01", 2024, 2, 276.92m },
        { "1974-02-01", 2024, 3,  369.23m },
        { "1974-05-15", 2024, 9,  276.92m  },
        { "1974-05-15", 2024, 10,  369.23m },
        { "1974-02-01", 2024, 26,  369.23m },
        { "1975-01-01", 2024, 26,  276.92m },
    };
    
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithHighSalary_ShouldReturnPaycheckWithHighSalaryDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 100_000
        };
        
        var paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        var deduction = paycheck.Deductions.Single(a => a.Type == DeductionType.HighSalary);
        deduction.Amount.Should().BeApproximately(76.92m, 0.01m);
    }
    
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckWithSalaryLowerOrEqualToHighSalaryThreshold_ShouldReturnPaycheckWithoutHighSalaryDeduction()
    {
        var paycheckCalculatorSettings = new PaycheckCalculatorSettings() { HighSalaryThreshold = 10000};
        var paycheckCalculator = GetPaycheckCalculator( paycheckCalculatorSettings);

        var employee = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold
        };
        
        var paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(a => a.Type == DeductionType.HighSalary);

        var employee2 = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold - 1
        };
        
        paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(a => a.Type == DeductionType.HighSalary);
    }

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckWithSalaryHigherThanHighSalaryThreshold_ShouldReturnPaycheckWithoutHighSalaryDeduction()
    {
        var paycheckCalculatorSettings = new PaycheckCalculatorSettings() { HighSalaryThreshold = 10000};
        var paycheckCalculator = GetPaycheckCalculator( paycheckCalculatorSettings);

        var employee = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold + 1
        };
        
        var paycheck = await paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().Contain(a => a.Type == DeductionType.HighSalary);
    }
}