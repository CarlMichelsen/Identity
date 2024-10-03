namespace Domain.Abstraction;

public enum ResultErrorType
{
    /// <summary>
    /// An exception was thrown and captured in this error.
    /// </summary>
    Exception,
    
    /// <summary>
    /// An error occured during mapping.
    /// </summary>
    MapError,
    
    /// <summary>
    /// Invalid http response status.
    /// </summary>
    HttpStatus,
    
    /// <summary>
    /// An error related to json parsing.
    /// </summary>
    JsonParse,
    
    /// <summary>
    /// Action not allowed.
    /// </summary>
    Unauthorized,
    
    /// <summary>
    /// Unable to find something.
    /// </summary>
    NotFound,
}