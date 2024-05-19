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
        var employee = await _repository.Find(id, ct);
        if (employee == null)
        {
            return NotFound();
        }
        
        var result = new ApiResponse<GetEmployeeDto>
        {
            Data = _mapper.EmployeeToGetEmployeeDto(employee),
            Success = true
        };

        return result;
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
