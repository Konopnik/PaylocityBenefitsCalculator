using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Dtos.Paycheck;
using Riok.Mapperly.Abstractions;

namespace Api.Dtos;

//note: I choose to use Mapperly mapper because it is using code generators => it is fast and memory efficient
// it is under Apache 2.0 license => it is free to use
// I do not have much experience with it, because we are not using it, but I read some article about different mappers it while ago and it seemed like a good choice 
[Mapper]
public partial class ModelToDtosMapper
{
    public partial GetEmployeeDto EmployeeToGetEmployeeDto(Models.Employee employee);
    
    [MapProperty(nameof(Models.Paycheck.Deductions), nameof(GetPaycheckDto.DeductionsAmount), Use = nameof(MapDeductionsAmount))]
    public partial GetPaycheckDto PaycheckToGetPaycheckDto(Models.Paycheck paycheck);
    
    [UserMapping(Default = false)]
    public decimal MapDeductionsAmount(ICollection<Models.Deduction> deductions)
    {
        return deductions.Sum(d => d.Amount);
    }
    
    public partial List<GetEmployeeDto> EmployeesToGetEmployeeDtosList(IEnumerable<Models.Employee> employee);
    public partial GetDependentDto DependentToGetDependentDto(Models.Dependent dependent);
    public partial List<GetDependentDto> DependentToGetDependentDtosList(IEnumerable<Models.Dependent> dependents);
}