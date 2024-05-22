using Api.Core.Entities;
using Api.UseCases.Employees;
using Riok.Mapperly.Abstractions;

namespace Api.UseCases;

[Mapper]
public partial class UseCasesMapper()
{
    public partial Employee RequestToEmployee(CreateEmployeeCommand command);
    public partial Dependent DependentDtoToDependent(DependentDto command);
}