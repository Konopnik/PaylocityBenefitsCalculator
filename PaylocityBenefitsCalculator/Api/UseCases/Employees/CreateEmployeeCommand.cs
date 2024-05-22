using Api.Core.Entities;
using Api.Core.Enums;
using Api.Core.Errors;
using Api.Core.Models;
using MediatR;

namespace Api.UseCases.Employees;

public record CreateEmployeeCommand(string? FirstName, string? LastName, decimal Salary, DateTime DateOfBirth, ICollection<DependentDto> Dependents) 
    : IRequest<Result<Employee, ValidationError>>;

public record DependentDto(string? FirstName, string? LastName, Relationship Relationship);