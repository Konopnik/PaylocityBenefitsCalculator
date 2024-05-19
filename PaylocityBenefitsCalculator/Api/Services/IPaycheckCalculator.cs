using Api.Models;

namespace Api.Services;

public interface IPaycheckCalculator
{
    public Task<Paycheck> Calculate(int year, int number, Employee employee);
}