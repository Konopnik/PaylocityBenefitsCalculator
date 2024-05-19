using Api.Dtos;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly IDependentRepository _repository;
    private readonly ModelToDtosMapper _mapper;

    public DependentsController(IDependentRepository repository, ModelToDtosMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    
    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id, CancellationToken ct)
    {
        var dependent = await _repository.Find(id, ct);
        if (dependent == null)
        {
            return NotFound();
        }
        
        var result = new ApiResponse<GetDependentDto>
        {
            Data = _mapper.DependentToGetDependentDto(dependent),
            Success = true
        };

        return result;    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll(CancellationToken ct)
    {
        var dependents = await _repository.GetAll(ct);
        
        var result = new ApiResponse<List<GetDependentDto>>
        {
            Data = _mapper.DependentToGetDependentDtosList(dependents),
            Success = true
        };

        return result;    }
}
