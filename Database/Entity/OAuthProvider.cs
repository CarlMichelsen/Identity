namespace Database.Entity;

public enum OAuthProvider
{
    /// <summary>
    /// Development login.
    /// </summary>
    Development = 1,
    
    /// <summary>
    /// Guest login.
    /// </summary>
    Guest = 2,
    
    /// <summary>
    /// Discord login.
    /// </summary>
    Discord = 3,
    
    /// <summary>
    /// GitHub login.
    /// </summary>
    Github = 4,
}