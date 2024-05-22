using Api.Core.Enums;

namespace Api.Core.Entities;

public class Deduction
{
    public DeductionType Type { get; set; }
    public decimal Amount { get; set; }
}