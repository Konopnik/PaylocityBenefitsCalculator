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
}