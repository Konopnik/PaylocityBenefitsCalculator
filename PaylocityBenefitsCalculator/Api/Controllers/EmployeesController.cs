using Api.Dtos;
using Api.Dtos.Employee;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly ModelToDtosMapper _mapper;

    public EmployeesController(IEmployeeRepository repository, ModelToDtosMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id, CancellationToken ct)
    {
        var employeeResult = await _repository.Find(id, ct);
        return employeeResult.Match<ActionResult<ApiResponse<GetEmployeeDto>>>(
            e => new ApiResponse<GetEmployeeDto>
            {
                Data = _mapper.EmployeeToGetEmployeeDto(e),
                Success = true
            },
            //node: return not found with error code and message which can help clint to understand what happened 
            // we can move strings to resources file if we would like to show it on the client side UI and we would like this service to be responsible for the localized text.
            // I decided not to do it for this example.
            error => NotFound(
                ApiResponse<GetEmployeeDto>.CreateError(
                    $"Employee {id} not found in the storage.",
                    ErrorCodes.EmployeeNotFound))
        );
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll(CancellationToken ct)
    {
        var employees = await _repository.GetAll(ct);

        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = _mapper.EmployeesToGetEmployeeDtosList(employees),
            Success = true
        };

        return result;
    }
}