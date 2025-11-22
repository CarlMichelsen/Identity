using Application.Service.OAuth;
using Shouldly;

namespace Test.Unit.UriBuilder;

public class OAuthUriBuilderTests
{
    [Theory]
    [InlineData("https://www.youtube.com/watch?v=4Zw6xamarcE", "https://www.youtube.com/watch?v=4Zw6xamarcE")]
    [InlineData("https://www.youtube.com", "https://www.youtube.com/")]
    [InlineData("https://www.youtube.com/test", "https://www.youtube.com/test")]
    public void QueryParameterTest(string url, string builtUrl)
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder(url);
        
        // Act
        var uri = uriBuilder.Build();

        // Assert
        uri.AbsoluteUri.ShouldBe(builtUrl);
    }

    [Fact]
    public void SetPathTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://www.example.com/oldpath");
    
        // Act
        var uri = uriBuilder.SetPath("/new/path/here").Build();
    
        // Assert
        uri.AbsoluteUri.ShouldBe("https://www.example.com/new/path/here");
    }

    [Fact]
    public void AddQueryParameterTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://www.example.com/path");
    
        // Act
        var uri = uriBuilder.AddQueryParam("client_id", "abc123")
            .AddQueryParam("redirect_uri", "https://callback.com")
            .Build();
    
        // Assert
        uri.Query.ShouldContain("client_id=abc123");
        uri.Query.ShouldContain("redirect_uri=https://callback.com");
    }

    [Fact]
    public void RemoveQueryParameterTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://www.example.com/path?client_id=abc123&state=xyz&scope=read");
    
        // Act
        var uri = uriBuilder.RemoveQueryParam("state").Build();
    
        // Assert
        uri.Query.ShouldContain("client_id=abc123");
        uri.Query.ShouldContain("scope=read");
        uri.Query.ShouldNotContain("state");
    }

    [Fact]
    public void AddMultipleQueryParametersWithSameKeyTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://www.example.com/path");
    
        // Act
        var uri = uriBuilder.AddQueryParam("scope", "read")
            .AddQueryParam("scope", "write")
            .AddQueryParam("scope", "delete")
            .Build();
    
        // Assert
        var scopeParams = uri.Query.Split('&')
            .Where(p => p.Contains("scope="))
            .ToList();
    
        scopeParams.Count.ShouldBe(3);
        uri.Query.ShouldContain("scope=read");
        uri.Query.ShouldContain("scope=write");
        uri.Query.ShouldContain("scope=delete");
    }
    
    [Fact]
    public void SetBaseAddressTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://old-domain.com/path?param=value");
        
        // Act
        var uri = uriBuilder.SetBaseAddress("https://new-domain.com").Build();
        
        // Assert
        uri.Host.ShouldBe("new-domain.com");
        uri.Scheme.ShouldBe("https");
        uri.AbsolutePath.ShouldContain("path");
        uri.Query.ShouldContain("param=value");
    }

    [Fact]
    public void SetBaseAddressWithCustomPortTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/path");
        
        // Act
        var uri = uriBuilder.SetBaseAddress("https://example.com:8080").Build();
        
        // Assert
        uri.Port.ShouldBe(8080);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetBaseAddress_ThrowsException_WhenNullOrEmpty(string? baseAddress)
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com");
        
        // Act & Assert
        Should.Throw<OAuthUriBuilder.UriBuilderException>(() => 
            uriBuilder.SetBaseAddress(baseAddress!));
    }

    [Fact]
    public void SetBaseAddress_ThrowsException_WhenContainsPath()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com");
        
        // Act & Assert
        Should.Throw<OAuthUriBuilder.UriBuilderException>(() => 
            uriBuilder.SetBaseAddress("https://example.com/path"));
    }

    [Fact]
    public void ClearQueryParamsTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/path?client_id=abc&state=xyz&scope=read");
        
        // Act
        var uri = uriBuilder.ClearQueryParams().Build();
        
        // Assert
        uri.Query.ShouldBeEmpty();
        uri.AbsoluteUri.ShouldBe("https://example.com/path");
    }

    [Fact]
    public void AddQueryParamsTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/path");
        var parameters = new Dictionary<string, List<string>>
        {
            { "client_id", ["abc123"] },
            { "scope", ["read", "write"] },
            { "state", ["random_state"] }
        };
        
        // Act
        var uri = uriBuilder.AddQueryParams(parameters).Build();
        
        // Assert
        uri.Query.ShouldContain("client_id=abc123");
        uri.Query.ShouldContain("scope=read");
        uri.Query.ShouldContain("scope=write");
        uri.Query.ShouldContain("state=random_state");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AddQueryParam_ThrowsException_WhenKeyIsNullOrEmpty(string? key)
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com");
        
        // Act & Assert
        Should.Throw<OAuthUriBuilder.UriBuilderException>(() => 
            uriBuilder.AddQueryParam(key!, "value"));
    }

    [Fact]
    public void AddQueryParam_AllowsNullValue()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/path");
        
        // Act
        var uri = uriBuilder.AddQueryParam("param", null).Build();
        
        // Assert
        uri.Query.ShouldContain("param=");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void RemoveQueryParam_ThrowsException_WhenKeyIsNullOrEmpty(string? key)
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com?test=value");
        
        // Act & Assert
        Should.Throw<OAuthUriBuilder.UriBuilderException>(() => 
            uriBuilder.RemoveQueryParam(key!));
    }

    [Fact]
    public void Constructor_ParsesMultipleQueryParametersCorrectly()
    {
        // Arrange & Act
        var uriBuilder = new OAuthUriBuilder("https://example.com/oauth?client_id=abc&redirect_uri=https://callback.com&scope=read&scope=write");
        var uri = uriBuilder.Build();
        
        // Assert
        uri.Query.ShouldContain("client_id=abc");
        uri.Query.ShouldContain("redirect_uri=https://callback.com");
        uri.Query.ShouldContain("scope=read");
        uri.Query.ShouldContain("scope=write");
    }

    [Fact]
    public void Constructor_HandlesQueryParameterWithEqualsSignInValue()
    {
        // Arrange & Act
        var uriBuilder = new OAuthUriBuilder("https://example.com/path?token=abc=123=xyz");
        var uri = uriBuilder.Build();
        
        // Assert
        uri.Query.ShouldContain("token=abc=123=xyz");
    }

    [Fact]
    public void Constructor_ThrowsException_WhenQueryParameterHasNoValue()
    {
        // Arrange & Act & Assert
        Should.Throw<OAuthUriBuilder.UriBuilderException>(() => 
            new OAuthUriBuilder("https://example.com/path?invalidparam"));
    }

    [Fact]
    public void SetPath_ClearsExistingPath()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/old/path/here");
        
        // Act
        var uri = uriBuilder.SetPath("/completely/new").Build();
        
        // Assert
        uri.AbsolutePath.ShouldBe("/completely/new");
    }

    [Fact]
    public void SetPath_HandlesPathWithoutLeadingSlash()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://example.com/");
        
        // Act
        var uri = uriBuilder.SetPath("oauth/authorize").Build();
        
        // Assert
        uri.AbsolutePath.ShouldBe("/oauth/authorize");
    }

    [Fact]
    public void Build_PreservesHttpsScheme()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://secure.example.com/path");
        
        // Act
        var uri = uriBuilder.Build();
        
        // Assert
        uri.Scheme.ShouldBe("https");
    }

    [Fact]
    public void Build_PreservesHttpScheme()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("http://example.com/path");
        
        // Act
        var uri = uriBuilder.Build();
        
        // Assert
        uri.Scheme.ShouldBe("http");
    }

    [Fact]
    public void ChainMultipleOperationsTest()
    {
        // Arrange
        var uriBuilder = new OAuthUriBuilder("https://old.com/oldpath?old=param");
        
        // Act
        var uri = uriBuilder
            .SetBaseAddress("https://auth.example.com")
            .SetPath("/oauth/v2/authorize")
            .ClearQueryParams()
            .AddQueryParam("client_id", "my_client_id")
            .AddQueryParam("response_type", "code")
            .AddQueryParam("redirect_uri", "https://myapp.com/callback")
            .AddQueryParam("scope", "read")
            .AddQueryParam("scope", "write")
            .AddQueryParam("state", "random_state_123")
            .Build();
        
        // Assert
        uri.Host.ShouldBe("auth.example.com");
        uri.AbsolutePath.ShouldBe("/oauth/v2/authorize");
        uri.Query.ShouldNotContain("old=param");
        uri.Query.ShouldContain("client_id=my_client_id");
        uri.Query.ShouldContain("response_type=code");
        uri.Query.ShouldContain("redirect_uri=https://myapp.com/callback");
        uri.Query.ShouldContain("scope=read");
        uri.Query.ShouldContain("scope=write");
        uri.Query.ShouldContain("state=random_state_123");
    }
}