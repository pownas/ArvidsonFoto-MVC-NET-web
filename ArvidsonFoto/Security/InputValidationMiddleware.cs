using System.Text.RegularExpressions;

namespace ArvidsonFoto.Security;

/// <summary>
/// Middleware to validate and sanitize all incoming HTTP requests
/// to prevent SQL injection and other malicious input attempts.
/// </summary>
public class InputValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InputValidationMiddleware> _logger;

    // Patterns that indicate potential SQL injection attempts
    private static readonly List<Regex> SqlInjectionPatterns = new()
    {
        new Regex(@"(\bunion\b.*\bselect\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bselect\b.*\bfrom\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\binsert\b.*\binto\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bupdate\b.*\bset\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bdelete\b.*\bfrom\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bdrop\b.*\btable\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bexec\b|\bexecute\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bxp_\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(\bsp_\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(;\s*drop)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(;\s*shutdown)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(--\s|;\s*--)", RegexOptions.Compiled), // SQL comments with whitespace/semicolon
        new Regex(@"(\/\*.*?\*\/)", RegexOptions.Compiled | RegexOptions.Singleline), // SQL block comments
        new Regex(@"(\bor\b\s+\d+\s*=\s*\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled), // or 1=1
        new Regex(@"(\band\b\s+\d+\s*=\s*\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled), // and 1=1
        new Regex(@"(\bor\b\s+['""]?\d+['""]?\s*=\s*['""]?\d+['""]?)", RegexOptions.IgnoreCase | RegexOptions.Compiled), // or '1'='1'
        new Regex(@"(\band\b\s+['""]?\d+['""]?\s*=\s*['""]?\d+['""]?)", RegexOptions.IgnoreCase | RegexOptions.Compiled), // and '1'='1'
        new Regex(@"(char\s*\(\d+\))", RegexOptions.IgnoreCase | RegexOptions.Compiled), // char(xxx)
        new Regex(@"(concat\s*\()", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(name_const\s*\()", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(unhex\s*\()", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(hex\s*\()", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(version\s*\(\))", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(database\s*\(\))", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(user\s*\(\))", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(convert\s*\(int)", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };

    public InputValidationMiddleware(RequestDelegate next, ILogger<InputValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check query string parameters
        if (context.Request.Query.Any())
        {
            foreach (var param in context.Request.Query)
            {
                // StringValues can contain multiple values, check each one
                foreach (var value in param.Value)
                {
                    if (ContainsSqlInjectionAttempt(value))
                    {
                        _logger.LogWarning(
                            "Potential SQL injection attempt detected in query parameter '{Key}' from IP {IpAddress}. Value: {Value}",
                            param.Key,
                            context.Connection.RemoteIpAddress,
                            value
                        );

                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }
                }
            }
        }

        // Check route values
        if (context.Request.RouteValues.Any())
        {
            foreach (var routeValue in context.Request.RouteValues)
            {
                if (routeValue.Value != null && ContainsSqlInjectionAttempt(routeValue.Value.ToString()!))
                {
                    _logger.LogWarning(
                        "Potential SQL injection attempt detected in route parameter '{Key}' from IP {IpAddress}. Value: {Value}",
                        routeValue.Key,
                        context.Connection.RemoteIpAddress,
                        routeValue.Value
                    );

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Checks if the input string contains patterns that indicate SQL injection attempts.
    /// </summary>
    private static bool ContainsSqlInjectionAttempt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        try
        {
            // Decode URL-encoded strings
            var decodedInput = Uri.UnescapeDataString(input);

            // Check against all SQL injection patterns
            return SqlInjectionPatterns.Any(pattern => pattern.IsMatch(decodedInput));
        }
        catch (UriFormatException)
        {
            // If URL decoding fails, check the original input
            return SqlInjectionPatterns.Any(pattern => pattern.IsMatch(input));
        }
    }
}
