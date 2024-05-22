using Api.Core.Entities;
using Api.Presentation.Dtos.Dependent;
using Api.Presentation.Dtos.Employee;
using Api.Presentation.Dtos.Paycheck;
using Riok.Mapperly.Abstractions;

namespace Api.Presentation;

//note: I choose to use Mapperly mapper because it is using code generators => it is fast and memory efficient
// it is under Apache 2.0 license => it is free to use
// I do not have much experience with it, because we are not using it, but I read some article about different mappers it while ago and it seemed like a good choice 
[Mapper]
public partial class ModelToDtosMapper
{
    public partial GetEmployeeDto EmployeeToGetEmployeeDto(Employee employee);
    
    [MapProperty(nameof(Paycheck.Deductions), nameof(GetPaycheckDto.DeductionsAmount), Use = nameof(MapDeductionsAmount))]
    public partial GetPaycheckDto PaycheckToGetPaycheckDto(Paycheck paycheck);
    
    [UserMapping(Default = false)]
    public decimal MapDeductionsAmount(ICollection<Deduction> deductions)
    {
        return deductions.Sum(d => d.Amount);
    }
    
    public partial List<GetEmployeeDto> EmployeesToGetEmployeeDtosList(IEnumerable<Employee> employee);
    public partial GetDependentDto DependentToGetDependentDto(Dependent dependent);
    public partial List<GetDependentDto> DependentToGetDependentDtosList(IEnumerable<Dependent> dependents);
}