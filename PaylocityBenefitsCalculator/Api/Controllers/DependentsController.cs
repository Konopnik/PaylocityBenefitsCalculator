﻿using Api.Dtos;
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
        var findResult = await _repository.Find(id, ct);
        return findResult.Match<ActionResult<ApiResponse<GetDependentDto>>>(
            d => new ApiResponse<GetDependentDto>
            {
                Data = _mapper.DependentToGetDependentDto(d),
                Success = true
            },
            error => NotFound(
                ApiResponse<GetDependentDto>.CreateError(
                    $"Dependent {id} not found in the storage.",
                    ErrorCodes.DependentNotFound))
        );
    }

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
