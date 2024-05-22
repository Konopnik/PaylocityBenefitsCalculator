namespace Api.Core.Services;

public static class AgeCalculator
{
    public static int Calculate(DateTime birthDate, DateTime referenceDate)
    {
        return referenceDate.AddTicks(birthDate.Ticks * -1).Year - 1;
    }
}