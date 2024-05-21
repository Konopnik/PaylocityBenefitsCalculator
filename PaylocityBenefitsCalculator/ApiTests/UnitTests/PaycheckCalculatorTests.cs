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

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 1_234_567 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().HaveCount(1);
        var dedustion = paycheck.Deductions.First();
        dedustion.Amount.Should().BeApproximately(461.53m, 0.1m);
        dedustion.Type.Should().Be(DeductionType.Base);
    }

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithoutAnyDependants_ShouldReturnPaycheckWithoutAnyDependantDeduction()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 1_234_567 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().NotContain(d => d.Type == DeductionType.Dependent);
    }
    

    [Fact]
    public async Task WhenAskedForCalculationOfPaycheckForEmployeeWithDependants_ShouldReturnPaycheckWithDependantDeductions()
    {
        var paycheckCalculator = GetPaycheckCalculator();

        var employee = new Employee()
        {
            Salary = 1_234_567,
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
}