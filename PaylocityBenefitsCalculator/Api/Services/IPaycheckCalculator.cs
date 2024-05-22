using Api.Models;

namespace Api.Services;

public interface IPaycheckCalculator
{
    // note: at the beginning I create this as async, but now I realized that it is not needed => removing Task here
    // as we are inside scope of micro service we can do such changes easily, if this would be part of some library - we should discuss it with clients of this library.
    public Paycheck Calculate(int year, int number, Employee employee);
}