namespace Api.Models;

public class Paycheck
{
    public int Year { get; set; }
    public int Number { get; set; }
    public Employee Employee { get; set; }
    public decimal GrossAmount { get; set; }
    public int DeductionsAmount { get; set; }
    public decimal NetAmount { get; set; }
}