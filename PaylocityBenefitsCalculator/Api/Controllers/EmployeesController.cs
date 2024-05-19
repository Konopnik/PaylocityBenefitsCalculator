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
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        throw new NotImplementedException();
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll(CancellationToken ct)
    {
        var employees = await _repository.GetAll(ct);
        
        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = _mapper.EmployeesToGetEmployeeDtos(employees).ToList(),
            Success = true
        };

        return result;
    }
}
