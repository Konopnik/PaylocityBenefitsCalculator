using System;
using System.Net.Http;
using Xunit;
// note: configure the tests to run in sequence, because of database initialization problems - I do not want to spend more time here and i do not know xUnit and it's behaviour much
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass, DisableTestParallelization = true)]

namespace ApiTests;

[Collection("Integration tests")]
public class IntegrationTest : IDisposable
{
    // I decided to use test approach with WebApplicationFactory => this way I can run the tests without running API separately
    // I used custom implementation of it right away, because I wanted to have more control over the environment
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _httpClient;

    protected HttpClient HttpClient
    {
        get
        {
            if (_httpClient == default)
            {
                _httpClient = _factory.CreateClient();
                _httpClient.DefaultRequestHeaders.Add("accept", "text/plain");
            }

            return _httpClient;
        }
    }

    public IntegrationTest()
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
    
    public void Dispose()
    {
        HttpClient.Dispose();
        _factory.Dispose();
    }
}