using Nuke.Common.Tooling;
using Serilog;

namespace Sample.Infrastructure.Base.Nuke.Models;

public static class PulumiLoggerExtensions
{
    public static Action<OutputType, string> CustomLogger => (outputType, message) =>
    {
        if (outputType == OutputType.Err)
        {
            Log.Error(message);
        }
        else
        {
            Log.Information(message);
        }
    };
}