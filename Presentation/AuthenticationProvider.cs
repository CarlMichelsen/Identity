namespace Presentation;

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