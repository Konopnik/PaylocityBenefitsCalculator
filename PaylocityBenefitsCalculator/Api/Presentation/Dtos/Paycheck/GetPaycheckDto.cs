using Api.Presentation.Dtos.Employee;

namespace Api.Presentation.Dtos.Paycheck;

public class GetPaycheckDto
{
    public int Year { get; set; }
    public int Number { get; set; }
    public GetEmployeeDto Employee { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DeductionsAmount { get; set; }
    public decimal NetAmount { get; set; }
}