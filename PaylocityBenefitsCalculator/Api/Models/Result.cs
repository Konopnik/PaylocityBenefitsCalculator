namespace Api.Models;

//note: I do not like to propagate null values through various layers of application => introducing simple Result type 
public class Result<TValue, TError>
{
     private readonly TValue? _value;
     private readonly TError? _error;
     
     public bool IsError { get; }

     private Result(TValue value)
     {
          IsError = false;
          _value = value;
          _error = default;
     }

     private Result(TError error)
     {
          IsError = true;
          _value = default;
          _error = error;
     }

     public static implicit operator Result<TValue, TError>(TValue value) => new(value);
     public static implicit operator Result<TValue, TError>(TError error) => new(error);

     public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure)
     {
          return !IsError ? success(_value!) : failure(_error!);
     }
}