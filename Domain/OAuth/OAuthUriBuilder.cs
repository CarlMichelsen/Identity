using System.Web;

namespace Domain.OAuth;

public class OAuthUriBuilder
{
    private readonly UriBuilder uriBuilder;
    private readonly System.Collections.Specialized.NameValueCollection query;

    public OAuthUriBuilder(Uri initialUri, bool allowPort = false)
    {
        this.uriBuilder = new UriBuilder(initialUri);
        if (!allowPort)
        {
            this.uriBuilder.Port = -1;
        }
        
        this.query = HttpUtility.ParseQueryString(this.uriBuilder.Query);
    }
    
    public OAuthUriBuilder SetQueryParameter(string name, string value)
    {
        this.query[HttpUtility.UrlEncode(name)] = value;
        return this;
    }
    
    public Uri GetUrl()
    {
        this.uriBuilder.Query = this.query.ToString();
        return this.uriBuilder.Uri;
    }
}