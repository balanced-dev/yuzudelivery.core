using System;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{


    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    [NerdbankGitVersioning]
    readonly NerdbankGitVersioning NerdbankVersioning;

    readonly DateTime BuildDate;


    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath PackagesDirectory => OutputDirectory / "packages";
    AbsolutePath TestResultsDirectory => OutputDirectory / "test_results";

    public Build()
    {
        BuildDate = DateTime.Now;
    }


    public static int Main () => Execute<Build>(x => x.Report);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = Solution.GetProjects("*.Tests");

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetResultsDirectory(TestResultsDirectory)
                .EnableNoBuild()
                .CombineWith(testProjects, (_, project) => _
                    .SetProjectFile(project)
                    .SetLoggers($"trx;LogFileName={project.Name}_{BuildDate:ddMMyy_hhmmss}.trx")));
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetOutputDirectory(PackagesDirectory)
                .SetConfiguration(Configuration));
        });

    Target Report => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            Log.Information("NuGetPackageVersion:          {Value}", NerdbankVersioning.NuGetPackageVersion);
            Log.Information("AssemblyInformationalVersion: {Value}", NerdbankVersioning.AssemblyInformationalVersion);
        });

}
