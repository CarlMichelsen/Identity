namespace Application.Service.OAuth;

public static class CookieMapper
{
    public static string GetCookieDomain(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var host = uri.DnsSafeHost;  // Changed from uri.Host

        // Handle IP addresses - return as-is without leading dot
        if (Uri.CheckHostName(host) == UriHostNameType.IPv4 ||
            Uri.CheckHostName(host) == UriHostNameType.IPv6)
        {
            return host;
        }

        // Handle localhost
        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return $".{host}";
        }

        var parts = host.Split('.');

        switch (parts.Length)
        {
            // If there are less than 2 parts, return as-is (shouldn't happen with valid domains)
            case < 2:
                return host;
            // Check for known multipart TLDs (e.g., .co.uk, .com.au, .gov.uk)
            case >= 3:
            {
                var lastTwo = $"{parts[^2]}.{parts[^1]}";
        
                // Common two-part TLDs
                string[] twoPartTlds =
                [
                    "co.uk", "gov.uk", "ac.uk", "org.uk", "me.uk", "net.uk",
                    "com.au", "net.au", "org.au", "edu.au", "gov.au",
                    "co.nz", "net.nz", "org.nz", "ac.nz", "govt.nz",
                    "co.za", "gov.za", "ac.za", "org.za", "net.za",
                    "com.br", "net.br", "org.br", "gov.br", "edu.br",
                    "co.jp", "ne.jp", "or.jp", "go.jp", "ac.jp",
                    "com.cn", "net.cn", "org.cn", "gov.cn", "edu.cn",
                    "co.in", "net.in", "org.in", "gov.in", "ac.in",
                    "com.mx", "net.mx", "org.mx", "gob.mx", "edu.mx"
                ];

                if (twoPartTlds.Contains(lastTwo, StringComparer.OrdinalIgnoreCase))
                {
                    // For multipart TLDs, we need domain + TLD (3 parts total)
                    // e.g., subdomain.example.co.uk -> .example.co.uk
                    return $".{parts[^3]}.{parts[^2]}.{parts[^1]}";
                }

                break;
            }
        }

        // Standard TLD (e.g., .com, .net, .org)
        // Take the last two parts: domain + TLD
        // e.g., subdomain.example.com -> .example.com
        return $".{parts[^2]}.{parts[^1]}";
    }
}