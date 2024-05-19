using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Riok.Mapperly.Abstractions;

namespace Api.Dtos;

//note: I choose to use Mapperly mapper because it is using code generators => it is fast and memory efficient
// it is under Apache 2.0 license => it is free to use
// I do not have much experience with it, because we are not using it, but I read some article about different mappers it while ago and it seemed like a good choice 
[Mapper]
public partial class ModelToDtosMapper
{
    public partial GetEmployeeDto EmployeeToGetEmployeeDto(Models.Employee employee);
    
    public partial IEnumerable<GetEmployeeDto> EmployeesToGetEmployeeDtos(IEnumerable<Models.Employee> employee);
    public partial GetDependentDto DependentToGetDependentDto(Models.Dependent dependent);
}