using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Presentation.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests;

internal static class ShouldExtensions
{
    public static Task ShouldReturn(this HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        AssertCommonResponseParts(response, expectedStatusCode);
        return Task.CompletedTask;
    }
    
    public static async Task ShouldReturnErrorCode(this HttpResponseMessage response, HttpStatusCode expectedStatusCode, string expectedErrorCode)
    {
        AssertCommonResponseParts(response, expectedStatusCode);
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(await response.Content.ReadAsStringAsync());
        apiResponse.Success.Should().BeFalse();
        apiResponse.Error.Should().Be(expectedErrorCode);
    }
    
    public static async Task ShouldReturn<T>(this HttpResponseMessage response, HttpStatusCode expectedStatusCode, T expectedContent)
    {
        await response.ShouldReturn(expectedStatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(await response.Content.ReadAsStringAsync());
        Assert.True(apiResponse.Success);
        Assert.Equal(JsonConvert.SerializeObject(expectedContent), JsonConvert.SerializeObject(apiResponse.Data));
    }

    private static void AssertCommonResponseParts(this HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }
}

