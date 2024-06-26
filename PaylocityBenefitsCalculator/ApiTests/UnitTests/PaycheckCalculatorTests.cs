using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Entities;
using Api.Core.Enums;
using Api.Core.Services;
using Api.Core.Settings;
using FluentAssertions;
using Xunit;

namespace ApiTests.UnitTests;

// note: I used TDD approach for implementation of PaycheckCalculator - see commits history
public class PaycheckCalculatorTests
{
    private const decimal ExpectedDependentCostWithAdditionalCost = 369.23m;
    private const decimal ExpectedBaseDependentCost = 276.92m;
    private const decimal Precision = 0.01m;

    private static PaycheckCalculator GetPaycheckCalculator(PaycheckCalculatorSettings settings)
    {
        return new PaycheckCalculator(settings);
    }

    private static PaycheckCalculator GetPaycheckCalculator()
    {
        return GetPaycheckCalculator(new PaycheckCalculatorSettings());
    }

    [Fact]
    public void WhenAskedForCalculationOfPaycheck_ShouldReturnSomePaycheck()
    {
        var paycheckCalculator = GetPaycheckCalculator();
        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee());

        paycheck.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(GrossAmountTestCases))]
    public void WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithCorrectGrossAmount(
        decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.GrossAmount.Should().BeApproximately(expectedGrossAmount, Precision);
    }

    public static TheoryData<decimal, decimal> GrossAmountTestCases = new()
    {
        { 26_000m, 1_000m },
        { 10_000m, 384.62m },
        { 0m, 0m },
    };

    [Theory]
    [MemberData(nameof(GrossAmountWithConfiguredPaycheckCountTestCases))]
    public void WhenAskedForCalculationOfPaycheckWithChangedPaycheckCount_ShouldReturnPaycheckWithCorrectGrossAmount(
        int paycheckCountPerYear, decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator(new PaycheckCalculatorSettings()
            { PaycheckCountPerYear = paycheckCountPerYear });

        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.GrossAmount.Should().BeApproximately(expectedGrossAmount, Precision);
    }

    public static TheoryData<int, decimal, decimal> GrossAmountWithConfiguredPaycheckCountTestCases = new()
    {
        { 26, 26_000m, 1_000m },
        { 12, 12_000m, 1_000m },
    };

    [Fact]
    public void WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithBaseEmployeeDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 70_000 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(1);
        var deduction = paycheck.Deductions.First();
        deduction.Amount.Should().BeApproximately(461.53m, 0.1m);
        deduction.Type.Should().Be(DeductionType.Base);
    }

    [Fact]
    public void
        WhenAskedForCalculationOfPaycheckForEmployeeWithoutAnyDependants_ShouldReturnPaycheckWithoutAnyDependantDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 70_000 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(d => d.Type == DeductionType.Dependent);
    }

    [Fact]
    public void WhenAskedForCalculationOfPaycheckForEmployeeWithDependants_ShouldReturnPaycheckWithDependantDeductions()
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

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(4);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(ExpectedBaseDependentCost, Precision));
    }

    [Fact]
    public void
        WhenAskedForCalculationOfPaycheckForEmployeeWithDependantOlderThanThreshold_ShouldReturnPaycheckWithAdditionalDependantDeduction()
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

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(ExpectedDependentCostWithAdditionalCost, Precision));
    }

    [Theory]
    [MemberData(nameof(A))]
    public void
        WhenAskedForCalculationOfPaycheckForEmployeeWithDependantsWithVariousDateOfBirth_ShouldReturnPaycheckWithCorrectDependentDeduction(
            DateTime dateOfBirth, int year, int paycheckNumber, decimal expectedDeductionAmount)
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

        var paycheck = paycheckCalculator.Calculate(year, paycheckNumber, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        paycheck.Deductions.Where(a => a.Type == DeductionType.Dependent)
            .Should()
            .HaveSameCount(employee.Dependents)
            .And
            .AllSatisfy(a => a.Amount.Should().BeApproximately(expectedDeductionAmount, Precision));
    }

    public static TheoryData<DateTime, int, int, decimal> A = new()
    {
        { new DateTime(1974, 01, 01), 2024, 1, ExpectedDependentCostWithAdditionalCost },
        { new DateTime(1974, 02, 01), 2024, 1, ExpectedBaseDependentCost },
        { new DateTime(1974, 02, 01), 2024, 2, ExpectedBaseDependentCost },
        { new DateTime(1974, 02, 01), 2024, 3, ExpectedDependentCostWithAdditionalCost },
        { new DateTime(1974, 05, 15), 2024, 9, ExpectedBaseDependentCost },
        { new DateTime(1974, 05, 15), 2024, 10, ExpectedDependentCostWithAdditionalCost },
        { new DateTime(1974, 02, 01), 2024, 26, ExpectedDependentCostWithAdditionalCost },
        { new DateTime(1975, 01, 01), 2024, 26, ExpectedBaseDependentCost },
    };

    [Fact]
    public void WhenAskedForCalculationOfPaycheckForEmployeeWithHighSalary_ShouldReturnPaycheckWithHighSalaryDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 100_000
        };

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(2);
        var deduction = paycheck.Deductions.Single(a => a.Type == DeductionType.HighSalary);
        deduction.Amount.Should().BeApproximately(76.92m, Precision);
    }

    [Fact]
    public void
        WhenAskedForCalculationOfPaycheckWithSalaryLowerOrEqualToHighSalaryThreshold_ShouldReturnPaycheckWithoutHighSalaryDeduction()
    {
        var paycheckCalculatorSettings = new PaycheckCalculatorSettings() { HighSalaryThreshold = 10000 };
        var paycheckCalculator = GetPaycheckCalculator(paycheckCalculatorSettings);

        var employee = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold
        };

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(a => a.Type == DeductionType.HighSalary);

        var employee2 = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold - 1
        };

        paycheck = paycheckCalculator.Calculate(2024, 1, employee2);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(a => a.Type == DeductionType.HighSalary);
    }

    [Fact]
    public void
        WhenAskedForCalculationOfPaycheckWithSalaryHigherThanHighSalaryThreshold_ShouldReturnPaycheckWithoutHighSalaryDeduction()
    {
        var paycheckCalculatorSettings = new PaycheckCalculatorSettings() { HighSalaryThreshold = 10000 };
        var paycheckCalculator = GetPaycheckCalculator(paycheckCalculatorSettings);

        var employee = new Employee()
        {
            Salary = paycheckCalculatorSettings.HighSalaryThreshold + 1
        };

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().Contain(a => a.Type == DeductionType.HighSalary);
    }


    [Theory]
    [MemberData(nameof(NetAmountTestCases))]
    public void WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithCorrectNetAmount(
        decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.NetAmount.Should().BeApproximately(expectedGrossAmount, Precision);
    }

    public static TheoryData<decimal, decimal> NetAmountTestCases = new()
    {
        { 100_000m, 3307.69m },
        { 80_000m, 2615.38m },
        { 50_000m, 1461.54m },
    };


    [Fact]
    public void WhenAskedForCalculationOfPaycheckForEmployeeWithDependants_ShouldReturnPaycheckWithCorrectNetAmount()
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

        var paycheck = paycheckCalculator.Calculate(2024, 1, employee);

        paycheck.Should().NotBeNull();
        paycheck.NetAmount.Should().BeApproximately(1400, Precision);
    }
}