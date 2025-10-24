using System.Text;

namespace Application.Service.OAuth;

public class OAuthUriBuilder
{
    private string scheme;
    private string host;
    private int? port;
    private readonly List<string> path;
    private readonly Dictionary<string, List<string>> queryParams;

    public OAuthUriBuilder(Uri uri)
    {
        scheme = uri.Scheme;
        host = uri.Host;
        port = uri.IsDefaultPort ? null : uri.Port;
        path = uri.AbsolutePath.Split('/').ToList();

        var queryHolder = new Dictionary<string, List<string>>();
        foreach (var queryParamItem in uri.Query.Split('&'))
        {
            if (string.IsNullOrEmpty(uri.Query))
            {
                break;
            }
            
            var queryParam = queryParamItem[0] == '?'
                ? queryParamItem.Remove(0, 1)
                : queryParamItem;
            
            var firstEqualSign = queryParam.IndexOf('=');
            if (firstEqualSign == -1)
            {
                throw new UriBuilderException($"Invalid query parameter '{queryParam}'");
            }
            
            var key = queryParam[..firstEqualSign];
            var value = queryParam[(firstEqualSign + 1)..];

            if (queryHolder.TryGetValue(key, out var list))
            {
                list.Add(value);
            }
            else
            {
                queryHolder.Add(key, [value]);
            }
        }

        queryParams = queryHolder;
    }

    public OAuthUriBuilder(string url) :  this(new Uri(url))
    {
    }
    
    public OAuthUriBuilder SetBaseAddress(string baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
        {
            throw new UriBuilderException("Base address cannot be null or empty");
        }

        var parsedUri = new Uri(baseAddress, UriKind.Absolute);
        scheme = parsedUri.Scheme;
        host = parsedUri.Host;
        port = parsedUri.IsDefaultPort ? null : parsedUri.Port;

        // Only update path if base address has one
        return parsedUri.AbsolutePath != "/"
            ? throw new UriBuilderException("Base address cannot have a path")
            : this;
    }
    
    public OAuthUriBuilder SetPath(string newPath)
    {
        path.Clear();
        path.AddRange(newPath.Split('/').Where(r => !string.IsNullOrWhiteSpace(r)));
        return this;
    }
    
    public OAuthUriBuilder AddQueryParam(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new UriBuilderException($"Query parameter key cannot be null or empty {nameof(key)}");
        }

        if (queryParams.TryGetValue(key, out var list))
        {
            list.Add(value ?? string.Empty);
        }
        else
        {
            queryParams[key] = [value ?? string.Empty];
        }
        
        return this;
    }
    
    public OAuthUriBuilder AddQueryParams(IDictionary<string, List<string>> parameters)
    {
        foreach (var kvp in parameters)
        {
            kvp.Value.ForEach(v => AddQueryParam(kvp.Key, v));
        }

        return this;
    }
    
    public OAuthUriBuilder RemoveQueryParam(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new UriBuilderException($"Query parameter key cannot be null or empty {nameof(key)}");
        }
        
        queryParams.Remove(key);
        return this;
    }
    
    public OAuthUriBuilder ClearQueryParams()
    {
        queryParams.Clear();
        return this;
    }
    
    public Uri Build()
    {
        var builder = new StringBuilder();
        
        // Scheme and host
        builder.Append(scheme);
        builder.Append("://");
        builder.Append(host);

        // Port (only if not default)
        if (port.HasValue)
        {
            builder.Append(':');
            builder.Append(port.Value);
        }

        // Path
        if (path.Count > 0)
        {
            builder.Append('/');
            builder.Append(string.Join('/', path.Where(p => !string.IsNullOrWhiteSpace(p))));
        }
        

        // Query string
        if (queryParams.Count == 0)
        {
            return new Uri(builder.ToString());
        }
        
        builder.Append('?');
        var queryParameterString = string.Join("&", queryParams
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Key))
            .SelectMany(kv => kv.Value.Select(value => new KeyValuePair<string, string>(kv.Key, value)))
            .OrderBy(x => Random.Shared.Next())
            .Select(p => $"{p.Key.Trim()}={p.Value.Trim()}"));
        
        builder.Append(queryParameterString);
        return new Uri(builder.ToString());
    }

    public class UriBuilderException(string message) : Exception(message);
}