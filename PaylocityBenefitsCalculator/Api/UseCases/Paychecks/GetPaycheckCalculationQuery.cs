using Api.Core.Entities;
using Api.Core.Errors;
using Api.Core.Models;
using MediatR;

namespace Api.UseCases.Paychecks;

public record GetPaycheckCalculationQuery(int Year, int PaycheckNumber, int EmployeeId) : IRequest<Result<Paycheck, NotFoundError>>;