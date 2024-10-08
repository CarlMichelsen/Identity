using System.Diagnostics.CodeAnalysis;
using Domain.Abstraction;

namespace Domain.OAuth;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "It makes sense to have these close to each other.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "It makes sense to have these close to each other.")]
public static class OAuthProviderExtensions
{
    private static readonly Dictionary<string, OAuthProvider> StringProviderMap =
        Enum.GetValues<OAuthProvider>()
            .ToDictionary(op => SafeString(Enum.GetName(op)!), op => op); 
    
    public static Result<string> MapToProviderString(this OAuthProvider provider)
    {
        var str = Enum.GetName(provider);
        if (string.IsNullOrWhiteSpace(str))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Enum to string conversion resulted in an empty or null string");
        }

        return SafeString(str);
    }
    
    public static Result<OAuthProvider> MapToProvider(string providerString)
    {
        if (string.IsNullOrWhiteSpace(providerString))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "String to enum conversion resulted in an empty or null string");
        }
        
        var ps = SafeString(providerString);
        if (!StringProviderMap.TryGetValue(ps, out var provider))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Enum string conversion resulted in an empty or null string");
        }

        return provider;
    }

    private static string SafeString(string providerString)
    {
        return providerString.ToLower().Trim();
    }
}