using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Pulumi;
using Serilog;

namespace Sample.Infrastructure.Base.Nuke;

public class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Preview);

    private AbsolutePath CurrentPulumiPath = RootDirectory / "Infrastructure" / "Base" / "Pulumi";
        
    [Parameter] private readonly string PULUMI_STATE_STORAGE_ACCOUNT_NAME = default!;
    [Parameter] private readonly string PULUMI_STATE_STORAGE_KEY = default!;
    [Parameter] private readonly string STAGE = default!;

    private string PulumiStackName => $"base-{STAGE}";
    Target SetupPulumiVars => _ => _
        .Triggers(PulumiLogin)
        .Requires(() => PULUMI_STATE_STORAGE_ACCOUNT_NAME)
        .Requires(() => PULUMI_STATE_STORAGE_KEY)
        .Executes(() =>
        {
            
            Log.Information($"Branch {CurrentPulumiPath}");
            Environment.SetEnvironmentVariable("AZURE_STORAGE_ACCOUNT", PULUMI_STATE_STORAGE_ACCOUNT_NAME);//"stpulumistatedmr");
            Environment.SetEnvironmentVariable("AZURE_STORAGE_KEY", PULUMI_STATE_STORAGE_KEY);
            Environment.SetEnvironmentVariable("PULUMI_CONFIG_PASSPHRASE", "passphrase!");
        });

    Target PulumiLogin => _ => _
        .DependsOn(SetupPulumiVars)
        .Triggers(InitStack)
        .Executes(() => { PulumiTasks.Pulumi("login azblob://state"); });


    bool VerifyIfStackNameExist()
    {
        var result = PulumiTasks.Pulumi("stack ls", CurrentPulumiPath);
        
        return result.StdToText().Contains(PulumiStackName);
    }

    Target InitStack => _ => _
        .DependsOn(PulumiLogin)
        .OnlyWhenDynamic(() => !VerifyIfStackNameExist())
        .Triggers(SelectStack)
        .Executes(() => { PulumiTasks.Pulumi($"stack init {PulumiStackName}", CurrentPulumiPath); });
    
    Target SelectStack => _ => _
        .DependsOn(InitStack)
        .Triggers(Preview)
        .Requires(()=> STAGE)
        .Executes(() => { PulumiTasks.Pulumi($"stack select {PulumiStackName}", CurrentPulumiPath); });

    Target Preview => _ => _
        .DependsOn(SelectStack)
        .Triggers(Up)
        .Executes(() =>
        {
            PulumiTasks.Pulumi("preview", CurrentPulumiPath, customLogger: (outputType, message) =>
            {
                if (outputType == OutputType.Err)
                {
                    Log.Error(message);
                }
                else
                {
                    Log.Information(message);
                }
            });
        });
    
    Target Up => _ => _
        .DependsOn(Preview)
        .Executes(() =>
        {
            PulumiTasks.Pulumi("preview", CurrentPulumiPath, customLogger: (outputType, message) =>
            {
                if (outputType == OutputType.Err)
                {
                    Log.Error(message);
                }
                else
                {
                    Log.Information(message);
                }
            });
        });

    Target Destroy => _ => _
        .Executes(() =>
        {
            Log.Warning("TODO destroy :)");
        });

}