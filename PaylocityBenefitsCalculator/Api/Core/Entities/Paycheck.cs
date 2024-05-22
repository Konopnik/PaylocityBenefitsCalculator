namespace Api.Core.Entities;

public class Paycheck
{
    public int Year { get; set; }
    public int Number { get; set; }
    public Employee Employee { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }

    public ICollection<Deduction> Deductions { get; set; } = new List<Deduction>();
}