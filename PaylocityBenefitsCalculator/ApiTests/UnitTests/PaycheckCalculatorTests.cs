using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;
using Api.Services;
using FluentAssertions;
using Xunit;

namespace ApiTests.UnitTests;

public class PaycheckCalculatorTests
{
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnSomePaycheck()
    {
        var paycheckCalculator = new PaycheckCalculator();
        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee());

        paycheck.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(GrossAmountTestCases))]

    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithCorrectGrossAmount(
        decimal salary, decimal expectedGrossAmount)
    {
        var paycheckCalculator = new PaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = salary });

        paycheck.Should().NotBeNull();
        paycheck.GrossAmount.Should().BeApproximately(expectedGrossAmount, 0.01m);
    }    
    
    [Fact]
    public async Task WhenAskedForCalculationOfPaycheck_ShouldReturnPaycheckWithBaseEmployeeDeduction()
    {
        var paycheckCalculator = new PaycheckCalculator();

        var paycheck = await paycheckCalculator.Calculate(2024, 1, new Employee() { Salary = 1234567 });

        paycheck.Should().NotBeNull();
        paycheck.Deductions.Should().Contain(d => d.Amount == 1_000m && d.Type == DeductionType.Base);
    }

    public static TheoryData<decimal, decimal> GrossAmountTestCases = new()
    {
        { 26_000m, 1_000m },
        { 10_000m, 384.62m },
        { 0m, 0m },
    };

}