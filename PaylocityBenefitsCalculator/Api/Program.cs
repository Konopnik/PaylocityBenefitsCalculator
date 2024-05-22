using System.Data.Common;
using Api.Dtos;
using Api.Infrastructure;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Api.Settings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ModelToDtosMapper>();

builder.Services.AddSettings();
builder.Services.AddRepositories();
builder.Services.AddServices();

// note: using sql lite just for example - in real world scenario, we would use a real database (e.g. SQL Server, Postgres, etc.)
// switch with only one value in enum is here just for demonstration purpose - this is a way how we can switch between in-memory and real database using the appsettings / env values ...
// with this approach we can have for example sqlite database for local development + automatic tests and real database for production
var dataContextSettings = builder.Configuration.GetSection(DataContextSettings.SECTION_NAME).Get<DataContextSettings>()!;
switch (dataContextSettings.DatabaseKind)
{
    case DatabaseKind.InMemory:
        builder.Services.AddDbContext<DataContext>((Action<DbContextOptionsBuilder>) (o =>
        {
            var sqliteConnection = new SqliteConnection(dataContextSettings.ConnectionString);
            sqliteConnection.Open();
            o.UseSqlite(sqliteConnection);
        }));
        break;
    default:
        throw new ArgumentOutOfRangeException();
}


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee Benefit Cost Calculation Api",
        Description = "Api to support employee benefit cost calculations"
    });
});

var allowLocalhost = "allow localhost";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalhost,
        policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowLocalhost);

app.UseHttpsRedirection();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    // note: here we can check if database is created / all migrations applied etc... I will just initialize DB with original data here.
    // when we will use real database we can move the test data initialization to the test project 
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    if (dataContextSettings.DatabaseKind == DatabaseKind.InMemory)
    {
        await dataContext.Database.EnsureCreatedAsync();

        var databaseInitializer = new DatabaseInitializer(dataContext);
        await databaseInitializer.Initialize();
    }
}

app.MapControllers();

app.Run();

namespace Api
{
    public partial class Program {} 
}
