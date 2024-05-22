using Api.Dtos;
using Api.Dtos.Employee;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

//note: I would start with discussion about the paycheck API structure => to get use cases of it and to understand the requirements 
// I was thinking about two options:
// - have a call for getting employee's paycheck as a responsibility of Employees Controller (endpoint "/api/v1/employees/{employeeId}/paycheck/{year}/{paycheckNumber}")
// - have a separate Paycheck Controller
// => I decided to have separated controller, because in my opinion there could be a requirement to have more use cases connected with paychecks. For example list all paychecks for employees per year / month etc... 
// I decided to have a simple endpoint to get a paycheck by year, paycheck number and employee id
// Endpoint: "/api/v1/paycheck/{year}/{paycheckNumber}/employee/{employeeId}" - I decided to have a path like this because it is easy to understand and it is clear what we are asking for.
// and if we will need to load paychecks for all employees for some year and paycheck number we can just implement endpoint "/api/v1/paycheck/{year}/{paycheckNumber}" and follow the same API structure 
[ApiController]
[Route("api/v1/[controller]")]
public class PaycheckController : ControllerBase
{
    private readonly IPaycheckCalculator _paycheckCalculator;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ModelToDtosMapper _mapper;

    public PaycheckController(IPaycheckCalculator paycheckCalculator, IEmployeeRepository employeeRepository, ModelToDtosMapper mapper)
    {
        _paycheckCalculator = paycheckCalculator;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{year}/{paycheckNumber}/employee/{employeeId}")]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> Get(
        int year,
        int paycheckNumber,
        int employeeId,
        CancellationToken ct)
    {
        var employee = await _employeeRepository.Find(employeeId, ct);
        if (employee == null)
        {
            return NotFound(
                ApiResponse<GetEmployeeDto>.CreateError(
                        $"Employee {employeeId} not found => paycheck cannot be calculated.", 
                        ErrorCodes.EmployeeNotFound));
        }

        var paycheck = _paycheckCalculator.Calculate(year, paycheckNumber, employee);
        
        var result = new ApiResponse<GetPaycheckDto>
        {
            Data =  _mapper.PaycheckToGetPaycheckDto(paycheck),
            Success = true
        };

        return await Task.FromResult(result);
    }
}