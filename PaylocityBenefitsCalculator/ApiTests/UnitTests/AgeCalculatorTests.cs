using System;
using Api.Core.Services;
using FluentAssertions;
using Xunit;

namespace ApiTests.UnitTests;

public class AgeCalculatorTests
{
    [Theory]
    [InlineData("2000-01-01", "2024-01-01", 24)]
    [InlineData("2000-01-02", "2024-01-01", 23)]
    public void WhenAskedForCalculationOfAge_ShouldReturnCorrectAge(DateTime dateOfBirth, DateTime referenceDate, int expectedAge)
    {
        var age = AgeCalculator.Calculate(dateOfBirth, referenceDate);

        age.Should().Be(expectedAge);
    }
}