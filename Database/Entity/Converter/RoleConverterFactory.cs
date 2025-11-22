using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Entity.Converter;

public static class RoleConverterFactory
{
    private const string DefaultRole = "default";
    
    private const char RoleDelimiter = ',';
    
    public static ValueConverter<List<string>, string> CreateConverter()
    {
        return new ValueConverter<List<string>, string>(
            roleList => ValidateRolesAndConvert(roleList),
            roleString => roleString.Split(RoleDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList());
    }
    
    public static ValueComparer<List<string>> CreateComparer()
    {
        return new ValueComparer<List<string>>(
            (c1, c2) => ReduceList(c1).SequenceEqual(ReduceList(c2)),
            c => ReduceList(c).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => ReduceList(c));
    }

    private static List<string> ReduceList(List<string>? roleList)
    {
        if (roleList is null)
        {
            return [];
        }
        
        var dict = roleList
            .Select(r => r.ToLower().Trim())
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Where(r => !r.Contains(RoleDelimiter))
            .ToDictionary(r => r, r => true);
        
        // Make sure the user has the "default" role - all users should have this.
        dict.TryAdd(DefaultRole, true);
        
        return dict
            .Select(kv => kv.Key)
            .OrderBy(r => r)
            .ToList();
    }

    private static string ValidateRolesAndConvert(List<string> roles)
        => string.Join(RoleDelimiter, ReduceList(roles));
}