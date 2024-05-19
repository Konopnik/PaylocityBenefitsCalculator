using Api.Dtos.Employee;

namespace Api.Dtos.Paycheck;

public class GetPaycheckDto
{
    public int Year { get; set; }
    public int Number { get; set; }
    public GetEmployeeDto Employee { get; set; }
    public decimal GrossAmount { get; set; }
    public int DeductionsAmount { get; set; }
    public decimal NetAmount { get; set; }
}