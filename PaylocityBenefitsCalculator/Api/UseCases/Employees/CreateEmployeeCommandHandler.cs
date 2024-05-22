using Api.Core.Entities;
using Api.Core.Errors;
using Api.Core.Models;
using Api.Core.Repositories;
using FluentValidation;
using MediatR;

namespace Api.UseCases.Employees;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Employee, ValidationError>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<Employee> _employeeValidator;
    private readonly UseCases.UseCasesMapper _mapper;

    public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IValidator<Employee> employeeValidator, UseCases.UseCasesMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _employeeValidator = employeeValidator;
        _mapper = mapper;
    }

    public async Task<Result<Employee, ValidationError>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = _mapper.RequestToEmployee(request);
        var validationResult = await _employeeValidator.ValidateAsync(employee, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.Errors.Select(e=>e.ErrorMessage).ToList());
        }

        return await _employeeRepository.Add(employee, cancellationToken);
    }
}