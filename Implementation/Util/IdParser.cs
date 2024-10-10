using Domain.Abstraction;

namespace Implementation.Util;

public static class IdParser
{
    public static Result<List<long>> Parse(string commaSeparatedIds)
    {
        try
        {
            return commaSeparatedIds
                .Trim()
                .Split(',')
                .Where(s => long.TryParse(s, out _))
                .Select(long.Parse)
                .ToList();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Failed to parse ids",
                e);
        }
    }
}