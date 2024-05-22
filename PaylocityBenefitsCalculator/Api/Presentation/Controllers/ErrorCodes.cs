namespace Api.Presentation.Controllers;

internal static class ErrorCodes
{
    //note: I decided to use error codes to have a clear way to identify what happened on the server side
    // this could be used on multiple places e.g. get employee / calculate paycheck when employee is not found etc...
    public static readonly string EmployeeNotFound = "EMPLOYEE_NOT_FOUND"; 
    
    public static readonly string DependentNotFound = "DEPENDENT_NOT_FOUND"; 
    
    //note: would be returned if paycheck number is less than 1 or greater than 26 if we would sanitize input on the server side
    public static readonly string InvalidPaycheckNumber = "INVALID_PAYCHECK_NUMBER"; 

    public static readonly string ValidationError = "VALIDATION_ERROR"; 
}