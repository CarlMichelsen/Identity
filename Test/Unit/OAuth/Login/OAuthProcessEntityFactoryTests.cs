using Application.Service.OAuth.Login;
using Moq;
using Presentation;
using Presentation.Service.OAuth.Login;
using Shouldly;

namespace Test.Unit.OAuth.Login;

public class OAuthProcessEntityFactoryTests
{
    private readonly Mock<TimeProvider> mockTimeProvider;
    private readonly Mock<IRedirectUriFactory> mockRedirectUriFactory;
    private readonly OAuthProcessEntityFactory sut;
    private readonly DateTime fixedDateTime;

    public OAuthProcessEntityFactoryTests()
    {
        mockTimeProvider = new Mock<TimeProvider>();
        mockRedirectUriFactory = new Mock<IRedirectUriFactory>();
        sut = new OAuthProcessEntityFactory(mockTimeProvider.Object, mockRedirectUriFactory.Object);
        
        fixedDateTime = new DateTime(2025, 10, 25, 12, 0, 0, DateTimeKind.Utc);
        mockTimeProvider.Setup(x => x.GetUtcNow())
            .Returns(new DateTimeOffset(fixedDateTime));
    }

    [Fact]
    public void CreateProcess_ShouldCreateValidOAuthProcessEntity()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var expectedRedirectUri = new Uri("https://discord.com/oauth2/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(expectedRedirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNull();
        result.State.ShouldNotBeNullOrWhiteSpace();
        result.LoginRedirectUri.ShouldBe(expectedRedirectUri);
        result.SuccessRedirectUri.ShouldBe(successUrl);
        result.ErrorRedirectUri.ShouldBe(errorUrl);
        result.CreatedAt.ShouldBe(fixedDateTime);
    }

    [Fact]
    public void CreateProcess_ShouldGenerateUniqueStateForEachCall()
    {
        // Arrange
        const AuthenticationProvider  authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://discord.com/oauth2/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result1 = sut.CreateProcess(authProvider, successUrl, errorUrl);
        var result2 = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        result1.State.ShouldNotBe(result2.State);
    }

    [Fact]
    public void CreateProcess_ShouldGenerateUniqueIdForEachCall()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://discord.com/oauth2/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result1 = sut.CreateProcess(authProvider, successUrl, errorUrl);
        var result2 = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        result1.Id.ShouldNotBe(result2.Id);
    }

    [Fact]
    public void CreateProcess_ShouldCallRedirectUriFactoryWithCorrectParameters()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.GitHub;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://github.com/login/oauth/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var _ = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        mockRedirectUriFactory.Verify(
            x => x.CreateRedirectUri(authProvider, It.Is<string>(s => !string.IsNullOrWhiteSpace(s))),
            Times.Once);
    }

    [Fact]
    public void CreateProcess_ShouldUseTimeProviderForCreatedAt()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Test;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://test.com/oauth");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        mockTimeProvider.Verify(x => x.GetUtcNow(), Times.Once);
        result.CreatedAt.ShouldBe(fixedDateTime);
        result.CreatedAt.Kind.ShouldBe(DateTimeKind.Utc);
    }

    [Theory]
    [InlineData(AuthenticationProvider.Discord)]
    [InlineData(AuthenticationProvider.GitHub)]
    [InlineData(AuthenticationProvider.Test)]
    public void CreateProcess_ShouldWorkWithAllAuthenticationProviders(AuthenticationProvider provider)
    {
        // Arrange
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://provider.com/oauth");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(provider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(provider, successUrl, errorUrl);

        // Assert
        result.ShouldNotBeNull();
        result.LoginRedirectUri.ShouldBe(redirectUri);
    }

    [Fact]
    public void CreateProcess_ShouldGenerateStateAsValidGuid()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://discord.com/oauth2/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        Guid.TryParse(result.State, out var parsedGuid).ShouldBeTrue();
        parsedGuid.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void CreateProcess_ShouldPassGeneratedStateToRedirectUriFactory()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://discord.com/oauth2/authorize");
        string capturedState = null!;
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Callback<AuthenticationProvider, string>((_, state) => capturedState = state)
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        capturedState.ShouldNotBeNullOrWhiteSpace();
        result.State.ShouldBe(capturedState);
    }

    [Fact]
    public void CreateProcess_ShouldCreateEntityWithNullOptionalProperties()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.Discord;
        var successUrl = new Uri("https://example.com/success");
        var errorUrl = new Uri("https://example.com/error");
        var redirectUri = new Uri("https://discord.com/oauth2/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        result.UserId.ShouldBeNull();
        result.User.ShouldBeNull();
        result.LoginId.ShouldBeNull();
        result.Login.ShouldBeNull();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void CreateProcess_WithDifferentUrls_ShouldPreserveUrls()
    {
        // Arrange
        const AuthenticationProvider authProvider = AuthenticationProvider.GitHub;
        var successUrl = new Uri("https://example.com/custom/success?param=value");
        var errorUrl = new Uri("https://example.com/custom/error?param=error");
        var redirectUri = new Uri("https://github.com/login/oauth/authorize");
        
        mockRedirectUriFactory
            .Setup(x => x.CreateRedirectUri(authProvider, It.IsAny<string>()))
            .Returns(redirectUri);

        // Act
        var result = sut.CreateProcess(authProvider, successUrl, errorUrl);

        // Assert
        result.SuccessRedirectUri.ShouldBe(successUrl);
        result.SuccessRedirectUri.AbsoluteUri.ShouldBe("https://example.com/custom/success?param=value");
        result.ErrorRedirectUri.ShouldBe(errorUrl);
        result.ErrorRedirectUri.AbsoluteUri.ShouldBe("https://example.com/custom/error?param=error");
    }
}