using Api.Controllers;
using Api.Core.Repositories;
using Api.Presentation.Dtos.Employee;
using Api.Presentation.Models;
using Api.UseCases.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Presentation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly ISender _sender;
    private readonly ModelToDtosMapper _mapper;

    public EmployeesController(IEmployeeRepository repository, ISender sender,  ModelToDtosMapper mapper)
    {
        _repository = repository;
        _sender = sender;
        _mapper = mapper;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}", Name = "GetEmployee")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id, CancellationToken ct)
    {
        var employeeResult = await _repository.Find(id, ct);
        return employeeResult.ToResultWithApiResponse(_mapper.ToGetEmployeeDto, $"Employee {id} not found in the storage.", ErrorCodes.EmployeeNotFound);
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll(CancellationToken ct)
    {
        var employees = await _repository.GetAll(ct);

        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = _mapper.ToGetEmployeeDtoList(employees),
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Create employees")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Create(CreateEmployeeCommand createEmployeeCommand, CancellationToken ct)
    {
        var response = await _sender.Send(createEmployeeCommand, ct);
        return response.Match<ActionResult>(
            employee => CreatedAtAction(
                nameof(Get),
                new { id = employee.Id },
                new ApiResponse<GetEmployeeDto>
                {
                    Data = _mapper.ToGetEmployeeDto(employee),
                    Success = true
                }),
            error => new BadRequestObjectResult(
                ApiResponse<GetEmployeeDto>.CreateError(string.Join($"{Environment.NewLine}", error.Errors),
                    ErrorCodes.ValidationError)));
    }
}