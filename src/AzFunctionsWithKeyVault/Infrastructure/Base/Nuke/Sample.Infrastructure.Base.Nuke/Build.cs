using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;
using Sample.Infrastructure.Base.Nuke.Models;
using Serilog;

namespace Sample.Infrastructure.Base.Nuke;

public class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Preview);

    private AbsolutePath CurrentPulumiPath = RootDirectory / "Infrastructure" / "Base" / "Pulumi";

    [Parameter] private readonly string PULUMI_STATE_STORAGE_ACCOUNT_NAME = default!;
    [Parameter] private readonly string PULUMI_STATE_STORAGE_KEY = default!;
    [Parameter] private readonly string PULUMI_PASSPHRASE = default!;
    [Parameter] private readonly string STAGE = default!;

    private string PulumiStackName => $"base-{STAGE}";

    Target SetupPulumiVars => _ => _
        .Triggers(PulumiLogin)
        .Requires(() => PULUMI_STATE_STORAGE_ACCOUNT_NAME)
        .Requires(() => PULUMI_STATE_STORAGE_KEY)
        .Requires(() => PULUMI_PASSPHRASE)
        .Executes(() =>
        {
            Environment.SetEnvironmentVariable("AZURE_STORAGE_ACCOUNT", PULUMI_STATE_STORAGE_ACCOUNT_NAME);
            Environment.SetEnvironmentVariable("AZURE_STORAGE_KEY", PULUMI_STATE_STORAGE_KEY);
            Environment.SetEnvironmentVariable("PULUMI_CONFIG_PASSPHRASE", PULUMI_PASSPHRASE);
        });

    Target PulumiLogin => _ => _
        .DependsOn(SetupPulumiVars)
        .Executes(() => { PulumiTasks.Pulumi("login azblob://state"); });

    Target InitStack => _ => _
        .DependsOn(PulumiLogin)
        .OnlyWhenDynamic(() => !VerifyIfStackNameExist())
        .Executes(() => { PulumiTasks.Pulumi($"stack init {PulumiStackName}", CurrentPulumiPath); });

    Target SelectStack => _ => _
        .DependsOn(InitStack)
        .Requires(() => STAGE)
        .Executes(() => { PulumiTasks.Pulumi($"stack select {PulumiStackName}", CurrentPulumiPath); });

    Target Preview => _ => _
        .DependsOn(SelectStack)
        .Executes(() =>
        {
            PulumiTasks.Pulumi("preview", CurrentPulumiPath, customLogger: PulumiLoggerExtensions.CustomLogger);
        });

    Target Up => _ => _
        .DependsOn(Preview)
        .Executes(() =>
        {
            PulumiTasks.Pulumi("up -y", CurrentPulumiPath, customLogger: PulumiLoggerExtensions.CustomLogger);
        });

    Target Destroy => _ => _
        .DependsOn(Preview)
        .Executes(() =>
        {
            PulumiTasks.Pulumi("destroy -y", CurrentPulumiPath, customLogger: PulumiLoggerExtensions.CustomLogger);
        });

    private bool VerifyIfStackNameExist()
    {
        var result = PulumiTasks.Pulumi("stack ls", CurrentPulumiPath);
        return result.StdToText().Contains(PulumiStackName);
    }
}