using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Solution] readonly Solution Solution;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly bool ShouldSkipTests = false;
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Restore => _ => _
        .Executes(() =>
        { 
            DotNetTasks.DotNetRestore(s=>s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Triggers(new []{Tests})
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s=>s.SetProjectFile(Solution));
        });

    Target Tests => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => !ShouldSkipTests)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s.SetProjectFile(Solution));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Compile)
        .Executes(() =>
        {
            var projectPath = RootDirectory / "Services" / "ServiceOne.FunctionApp";
            var settings = new DotNetPublishSettings()
                .SetProject(projectPath)
                .SetConfiguration(Configuration)
                .SetOutput(RootDirectory / "output");
            
            DotNetTasks.DotNetPublish(settings);
        });
}
