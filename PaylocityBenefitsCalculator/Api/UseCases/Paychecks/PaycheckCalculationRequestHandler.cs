using Api.Core.Entities;
using Api.Core.Errors;
using Api.Core.Models;
using Api.Core.Repositories;
using Api.Core.Services;
using MediatR;

namespace Api.UseCases.Paychecks;

public class GetPaycheckCalculationQueryHandler : IRequestHandler<GetPaycheckCalculationQuery, Result<Paycheck, NotFoundError>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPaycheckCalculator _paycheckCalculator;

    public GetPaycheckCalculationQueryHandler(IEmployeeRepository employeeRepository, IPaycheckCalculator paycheckCalculator)
    {
        _employeeRepository = employeeRepository;
        _paycheckCalculator = paycheckCalculator;
    }

    public async Task<Result<Paycheck, NotFoundError>> Handle(GetPaycheckCalculationQuery query, CancellationToken ct)
    {
        var employeeResult = await _employeeRepository.Find(query.EmployeeId, ct);
        return employeeResult.Match<Result<Paycheck, NotFoundError>>(
            employee => _paycheckCalculator.Calculate(query.Year, query.PaycheckNumber, employee),
            error => error
        );
    }
}