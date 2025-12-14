using ArvidsonFoto.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ArvidsonFoto.Tests.Unit.Security;

public class InputValidationMiddlewareTests
{
    private readonly Mock<ILogger<InputValidationMiddleware>> _mockLogger;
    private readonly Mock<RequestDelegate> _mockNext;

    public InputValidationMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<InputValidationMiddleware>>();
        _mockNext = new Mock<RequestDelegate>();
    }

    [Theory]
    [InlineData("normal text")]
    [InlineData("category-name")]
    [InlineData("Fåglar")]
    [InlineData("123")]
    [InlineData("test@example.com")]
    public async Task InvokeAsync_WithSafeInput_ShouldContinuePipeline(string safeInput)
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.QueryString = new QueryString($"?param={safeInput}");

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("69 union select version()")]
    [InlineData("1' or '1'='1")]
    [InlineData("1 or 1=1")]
    [InlineData("'; drop table users--")]
    [InlineData("1; exec xp_cmdshell")]
    [InlineData("convert(int,char(65))")]
    [InlineData("name_const(CHAR(111))")]
    [InlineData("unhex(hex(version()))")]
    public async Task InvokeAsync_WithSqlInjectionAttempt_ShouldBlockRequest(string maliciousInput)
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.QueryString = new QueryString($"?param={Uri.EscapeDataString(maliciousInput)}");

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithSqlInjectionAttempt_ShouldBlockRequestAndReturnBadRequest()
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        var maliciousInput = "1 or 1=1";
        context.Request.QueryString = new QueryString($"?id={Uri.EscapeDataString(maliciousInput)}");

        // Act
        await middleware.InvokeAsync(context);

        // Assert - request should be blocked
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("/latest.asp?page=69%22%20or%20(1,2)=(select*from(select%20name_const(CHAR(111)))")]
    [InlineData("/latest.asp?page=6999999%22%20union%20select%20unhex(hex(version()))")]
    [InlineData("/latest.asp?page=69%20or%20(1,2)=(select*from(select%20name_const(CHAR(111))))")]
    public async Task InvokeAsync_WithRealWorldAttackExamples_ShouldBlockRequest(string attackQuery)
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        
        // Extract query string from the attack URL
        var queryIndex = attackQuery.IndexOf('?');
        if (queryIndex >= 0)
        {
            context.Request.QueryString = new QueryString(attackQuery.Substring(queryIndex));
        }

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyQueryString_ShouldContinuePipeline()
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.QueryString = QueryString.Empty;

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithMultipleParameters_OneMalicious_ShouldBlockRequest()
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.QueryString = new QueryString("?category=birds&page=1%20or%201=1&sort=desc");

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("select * from users")]
    [InlineData("insert into table values")]
    [InlineData("update table set field")]
    [InlineData("delete from table")]
    [InlineData("drop table users")]
    public async Task InvokeAsync_WithBasicSqlKeywords_ShouldBlockRequest(string sqlInput)
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.QueryString = new QueryString($"?query={Uri.EscapeDataString(sqlInput)}");

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithSqlInjectionInRouteValue_ShouldBlockRequest()
    {
        // Arrange
        var middleware = new InputValidationMiddleware(_mockNext.Object, _mockLogger.Object);
        var context = CreateHttpContext();
        context.Request.RouteValues["id"] = "1 or 1=1";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }
}
