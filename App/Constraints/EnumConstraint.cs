namespace App.Constraints;

public class EnumConstraint<TEnum> : IRouteConstraint where TEnum : struct, Enum
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, 
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is null)
        {
            return false;
        }

        var valueString = value.ToString()!;
        return Enum.TryParse<TEnum>(valueString, ignoreCase: true, out _);
    }
}