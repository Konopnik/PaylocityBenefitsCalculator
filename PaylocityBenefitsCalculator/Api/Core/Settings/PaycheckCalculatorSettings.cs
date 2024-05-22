namespace Api.Core.Settings;

public class PaycheckCalculatorSettings
{
    public const string SECTION_NAME = "PaycheckCalculatorSettings"; 
    
    public int PaycheckCountPerYear { get; set; } = 26;
    public decimal BaseEmployeeCostPerMonth { get; set; } = 1_000;
    public decimal DependentCostPerMonth { get; set; } = 600;
    public decimal OlderDependentAgeThreshold { get; set; } = 50;
    public decimal OlderDependentAdditionalCost { get; set; } = 200;
    public decimal HighSalaryThreshold { get; set; } = 80_000;
    public decimal HighSalaryCostPercentagePerYear { get; set; } = 2;
}