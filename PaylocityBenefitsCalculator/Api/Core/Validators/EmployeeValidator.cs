using Api.Core.Entities;
using Api.Core.Enums;
using FluentValidation;

namespace Api.Core.Validators;

// note: I am using fluent validation because it is easy to use and with it I can separate validation and the entity itself. 
public class EmployeeValidator :  AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(e => e.Dependents)
            .Must(d=>
                d.Count(a=> a.Relationship is Relationship.Spouse or Relationship.DomesticPartner) <= 1)
            .WithMessage("Employee can have only one spouse or domestic partner");
    }
}