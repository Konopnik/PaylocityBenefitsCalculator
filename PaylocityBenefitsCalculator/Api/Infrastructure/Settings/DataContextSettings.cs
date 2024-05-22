namespace Api.Infrastructure.Settings;

public class DataContextSettings
{
    public const string SECTION_NAME = "DataContextSettings"; 
    
    public string ConnectionString { get; set; } = null!;
    public DatabaseKind DatabaseKind { get; set; }
}