using Api.Core.Errors;
using Api.Core.Models;
using Api.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public static class Extensions
{
    public static ActionResult<ApiResponse<TOutput>> ToResultWithApiResponse<TInput, TOutput>(
        this Result<TInput, NotFoundError> result, 
        Func<TInput, TOutput> mapFunction,
        string message, 
        string errorCode)
    {
        return result.Match<ActionResult<ApiResponse<TOutput>>>(e => new OkObjectResult(new ApiResponse<TOutput>
            {
                Data = mapFunction.Invoke(e),
                Success = true
            }),
            error => new NotFoundObjectResult(ApiResponse<TOutput>.CreateError(message, errorCode)));
    }        
}