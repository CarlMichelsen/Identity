namespace Presentation;

/// <summary>
/// The names of this enum are converted to a string and back for the database - they can't change now.
/// </summary>
public enum AuthenticationProvider
{
    /// <summary>
    /// Test authentication provider used for development.
    /// </summary>
    Test = 1,
    
    /// <summary>
    /// Discord authentication provider.
    /// </summary>
    Discord = 2,
    
    /// <summary>
    /// GitHub authentication provider.
    /// </summary>
    GitHub = 3,
}