using System.Diagnostics;

namespace ArvidsonFoto.Core.Diagnostics;

internal static class AppTelemetry
{
    internal const string ActivitySourceName = "ArvidsonFoto";

    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    internal static void RecordError(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error);
        activity?.SetTag("error.type", exception.GetType().FullName);
    }
}
