namespace Api.Core.Errors;

// note: creating just simple validation error - in real world it would contain more structured errors
public record ValidationError(IReadOnlyCollection<string> Errors);