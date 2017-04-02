/* ****************************************
   Publishing workflow
   -------------------

 - Update CHANGELOG.md
 - Run a normal build with Cake
 - Push to devel and FF merge to master
 - Switch to master
 - Run a Publish build with Cake
 - Switch back to devel branch
   **************************************** */

#tool "nuget:?package=xunit.runner.console"
#addin "Cake.FileHelpers"
#addin "Octokit"
using Octokit;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var isLocal = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var buildNumber = AppVeyor.Environment.Build.Number;
var releaseNotes = ParseReleaseNotes("./CHANGELOG.md");
var version = releaseNotes.Version.ToString();
var buildDir = Directory("./src/AdultEmby.Plugins/bin") + Directory(configuration);
var buildResultDir = Directory("./bin") + Directory(version);

// Initialization
// ----------------------------------------

Setup(_ =>
{
    Information("Building version {0} of AdultEmby.Plugins.", version);
    Information("For the publish target the following environment variables need to be set:");
    Information(" GITHUB_API_TOKEN");
});

// Tasks
// ----------------------------------------

Task("Clean")
    .Does(() =>
    {
		CleanDirectories("./src/**/obj");
		CleanDirectories("./src/**/bin");
    });

Task("Restore-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore("./src/AdultEmby.Plugins.sln");
    });

Task("UpdateAssemblyVersion")
	.Does(() =>
{
	CreateAssemblyInfo("src\\SolutionInfo.cs", new AssemblyInfoSettings
	{
		Version = version,
		FileVersion = version,
		Company = "AdultEmby.Plugins"
	});
});

Task("Build")
    .IsDependentOn("Restore-Packages")
	.IsDependentOn("UpdateAssemblyVersion")
    .Does(() =>
    {
		MSBuild("./src/AdultEmby.Plugins.sln", new MSBuildSettings()
			.SetConfiguration(configuration)
			.UseToolVersion(MSBuildToolVersion.VS2015)
			.SetPlatformTarget(PlatformTarget.MSIL)
			.SetMSBuildPlatform(MSBuildPlatform.x86)
			.SetVerbosity(Verbosity.Minimal)
		);
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        XUnit2("./src/**/bin/" + configuration + "/*.Test.dll");
    });

Task("Publish-Release")
    .IsDependentOn("Run-Unit-Tests")
    .WithCriteria(() => isLocal)
    .Does(() =>
    {
//        var githubToken = EnvironmentVariable("GITHUB_API_TOKEN");

//        if (String.IsNullOrEmpty(githubToken))
//        {
//            throw new InvalidOperationException("Could not resolve AdultEmby.Plugins GitHub token.");
//        }

//        var github = new GitHubClient(new ProductHeaderValue("AdultEmby.PluginsCakeBuild"))
//        {
//            Credentials = new Credentials(githubToken)
//        };

//        var newRelease = github.Repository.Release;
//        newRelease.Create("AdultEmby.Plugins", "AdultEmby.Plugins", new NewRelease("v" + version)
//        {
//            Name = version,
//            Body = String.Join(Environment.NewLine, releaseNotes.Notes),
//            Prerelease = false,
//            TargetCommitish = "master"
//        }).Wait();
    });

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
    {
        AppVeyor.UpdateBuildVersion(version);
    });

// Targets
// ----------------------------------------

Task("Default")
    .IsDependentOn("Build")
	.IsDependentOn("Run-Unit-Tests");

Task("Publish")
    .IsDependentOn("Publish-Release");

Task("AppVeyor")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Update-AppVeyor-Build-Number");

// Execution
// ----------------------------------------

RunTarget(target);