using Application.Service.OAuth;

namespace Test.Unit.Mapper;

public class CookieMapperTests
{
    [Fact]
    public void GetCookieDomain_WhenUriIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Uri uri = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CookieMapper.GetCookieDomain(uri));
    }

    [Theory]
    [InlineData("https://example.com", ".example.com")]
    [InlineData("https://www.example.com", ".example.com")]
    [InlineData("https://api.example.com", ".example.com")]
    [InlineData("https://subdomain.example.com", ".example.com")]
    [InlineData("https://deep.subdomain.example.com", ".example.com")]
    [InlineData("https://very.deep.subdomain.example.com", ".example.com")]
    public void GetCookieDomain_WithStandardTld_ReturnsRootDomainWithLeadingDot(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.uk", ".example.co.uk")]
    [InlineData("https://www.example.co.uk", ".example.co.uk")]
    [InlineData("https://api.example.co.uk", ".example.co.uk")]
    [InlineData("https://subdomain.example.co.uk", ".example.co.uk")]
    [InlineData("https://deep.subdomain.example.co.uk", ".example.co.uk")]
    public void GetCookieDomain_WithUkDomain_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.gov.uk", ".example.gov.uk")]
    [InlineData("https://subdomain.example.gov.uk", ".example.gov.uk")]
    [InlineData("https://example.ac.uk", ".example.ac.uk")]
    [InlineData("https://example.org.uk", ".example.org.uk")]
    [InlineData("https://example.me.uk", ".example.me.uk")]
    [InlineData("https://example.net.uk", ".example.net.uk")]
    public void GetCookieDomain_WithVariousUkTlds_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com.au", ".example.com.au")]
    [InlineData("https://www.example.com.au", ".example.com.au")]
    [InlineData("https://example.net.au", ".example.net.au")]
    [InlineData("https://example.org.au", ".example.org.au")]
    [InlineData("https://example.edu.au", ".example.edu.au")]
    [InlineData("https://example.gov.au", ".example.gov.au")]
    public void GetCookieDomain_WithAustralianDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.nz", ".example.co.nz")]
    [InlineData("https://example.net.nz", ".example.net.nz")]
    [InlineData("https://example.org.nz", ".example.org.nz")]
    [InlineData("https://example.ac.nz", ".example.ac.nz")]
    [InlineData("https://example.govt.nz", ".example.govt.nz")]
    public void GetCookieDomain_WithNewZealandDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.za", ".example.co.za")]
    [InlineData("https://example.gov.za", ".example.gov.za")]
    [InlineData("https://example.ac.za", ".example.ac.za")]
    [InlineData("https://example.org.za", ".example.org.za")]
    [InlineData("https://example.net.za", ".example.net.za")]
    public void GetCookieDomain_WithSouthAfricanDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com.br", ".example.com.br")]
    [InlineData("https://example.net.br", ".example.net.br")]
    [InlineData("https://example.org.br", ".example.org.br")]
    [InlineData("https://example.gov.br", ".example.gov.br")]
    [InlineData("https://example.edu.br", ".example.edu.br")]
    public void GetCookieDomain_WithBrazilianDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.jp", ".example.co.jp")]
    [InlineData("https://example.ne.jp", ".example.ne.jp")]
    [InlineData("https://example.or.jp", ".example.or.jp")]
    [InlineData("https://example.go.jp", ".example.go.jp")]
    [InlineData("https://example.ac.jp", ".example.ac.jp")]
    public void GetCookieDomain_WithJapaneseDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com.cn", ".example.com.cn")]
    [InlineData("https://example.net.cn", ".example.net.cn")]
    [InlineData("https://example.org.cn", ".example.org.cn")]
    [InlineData("https://example.gov.cn", ".example.gov.cn")]
    [InlineData("https://example.edu.cn", ".example.edu.cn")]
    public void GetCookieDomain_WithChineseDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.in", ".example.co.in")]
    [InlineData("https://example.net.in", ".example.net.in")]
    [InlineData("https://example.org.in", ".example.org.in")]
    [InlineData("https://example.gov.in", ".example.gov.in")]
    [InlineData("https://example.ac.in", ".example.ac.in")]
    public void GetCookieDomain_WithIndianDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com.mx", ".example.com.mx")]
    [InlineData("https://example.net.mx", ".example.net.mx")]
    [InlineData("https://example.org.mx", ".example.org.mx")]
    [InlineData("https://example.gob.mx", ".example.gob.mx")]
    [InlineData("https://example.edu.mx", ".example.edu.mx")]
    public void GetCookieDomain_WithMexicanDomains_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://192.168.1.1", "192.168.1.1")]
    [InlineData("http://10.0.0.1", "10.0.0.1")]
    [InlineData("http://172.16.0.1", "172.16.0.1")]
    [InlineData("https://127.0.0.1", "127.0.0.1")]
    [InlineData("http://192.168.1.100:8080", "192.168.1.100")]
    public void GetCookieDomain_WithIPv4Address_ReturnsIpWithoutLeadingDot(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://[::1]", "::1")]
    [InlineData("http://[2001:db8::1]", "2001:db8::1")]
    [InlineData("http://[fe80::1]", "fe80::1")]
    [InlineData("http://[2001:0db8:85a3:0000:0000:8a2e:0370:7334]", "2001:db8:85a3::8a2e:370:7334")]
    public void GetCookieDomain_WithIPv6Address_ReturnsIpWithoutLeadingDot(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://localhost", "localhost")]
    [InlineData("http://localhost:5000", "localhost")]
    [InlineData("https://localhost", "localhost")]
    [InlineData("http://LOCALHOST", "localhost")]
    [InlineData("http://LocalHost", "localhost")]
    public void GetCookieDomain_WithLocalhost_ReturnsLocalhostWithoutLeadingDot(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.net", ".example.net")]
    [InlineData("https://example.org", ".example.org")]
    [InlineData("https://example.io", ".example.io")]
    [InlineData("https://example.dev", ".example.dev")]
    [InlineData("https://example.app", ".example.app")]
    [InlineData("https://example.ai", ".example.ai")]
    public void GetCookieDomain_WithVariousStandardTlds_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.co.uk/path/to/resource", ".example.co.uk")]
    [InlineData("https://example.com/path?query=value", ".example.com")]
    [InlineData("https://subdomain.example.com/path?query=value#fragment", ".example.com")]
    [InlineData("https://api.example.co.uk:8080/api/v1/users", ".example.co.uk")]
    public void GetCookieDomain_WithPathQueryAndFragment_IgnoresThemAndReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://EXAMPLE.COM", ".example.com")]
    [InlineData("https://WWW.EXAMPLE.COM", ".example.com")]
    [InlineData("https://Example.Co.Uk", ".example.co.uk")]
    [InlineData("https://API.EXAMPLE.CO.UK", ".example.co.uk")]
    public void GetCookieDomain_WithMixedCaseHostnames_ReturnsDomainInLowerCase(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://example.com", ".example.com")]
    [InlineData("https://example.com", ".example.com")]
    [InlineData("ftp://example.com", ".example.com")]
    public void GetCookieDomain_WithDifferentSchemes_ReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com:443", ".example.com")]
    [InlineData("http://example.com:80", ".example.com")]
    [InlineData("https://api.example.com:8443", ".example.com")]
    [InlineData("http://subdomain.example.co.uk:3000", ".example.co.uk")]
    public void GetCookieDomain_WithExplicitPort_IgnoresPortAndReturnsCorrectDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://a.b.c.d.example.com", ".example.com")]
    [InlineData("https://x.y.z.example.co.uk", ".example.co.uk")]
    [InlineData("https://level1.level2.level3.level4.level5.example.com", ".example.com")]
    public void GetCookieDomain_WithDeeplyNestedSubdomains_ReturnsRootDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetCookieDomain_WithSinglePartDomain_ReturnsDomainAsIs()
    {
        // Arrange
        var uri = new Uri("http://localhost");

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal("localhost", result);
    }

    [Theory]
    [InlineData("https://example.CO.UK", ".example.co.uk")]
    [InlineData("https://example.Com.Au", ".example.com.au")]
    [InlineData("https://example.GOV.UK", ".example.gov.uk")]
    public void GetCookieDomain_WithMixedCaseMultipartTld_HandlesCaseInsensitively(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://sub1.sub2.example.com", ".example.com")]
    [InlineData("https://api.v2.staging.example.com", ".example.com")]
    [InlineData("https://www.blog.subdomain.example.co.uk", ".example.co.uk")]
    public void GetCookieDomain_WithMultipleSubdomainLevels_ReturnsRootDomain(string uriString, string expected)
    {
        // Arrange
        var uri = new Uri(uriString);

        // Act
        var result = CookieMapper.GetCookieDomain(uri);

        // Assert
        Assert.Equal(expected, result);
    }
}