#tool nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0012
#tool "JetBrains.ReSharper.CommandLineTools"
#tool nuget:?package=vswhere
#tool ReSharperReports
#addin Cake.ReSharperReports

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target                  = Argument("target", "Default");
var configuration           = Argument("configuration", "Release");
var treatWarningsAsErrors   = Argument("treatWarningsAsErrors", "false");
var solutionPath            = MakeAbsolute(File(Argument("solutionPath", "./ArchitectNow.Framework.sln")));
var includeSymbols          = Argument("includeSymbols", "false");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var testAssemblies          = "./tests/**/bin/" +configuration +"/*.Tests.dll";

var artifacts               = MakeAbsolute(Directory(Argument("artifactPath", "./artifacts")));
var versionAssemblyInfo     = MakeAbsolute(File(Argument("versionAssemblyInfo", "VersionAssemblyInfo.cs")));
var analysisReports        = MakeAbsolute(Directory(Argument("analysisReports", "./analysis")));

IEnumerable<FilePath> nugetProjectPaths     = null;
SolutionParserResult solution               = null;
GitVersion versionInfo                      = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(ctx => {
    if(!FileExists(solutionPath)) throw new Exception(string.Format("Solution file not found - {0}", solutionPath.ToString()));
    solution = ParseSolution(solutionPath.ToString());

    Information("[Setup] Using Solution '{0}'", solutionPath.ToString());

    if(DirectoryExists(artifacts)) 
    {
        DeleteDirectory(artifacts, true);
    }
    
    EnsureDirectoryExists(artifacts);

    if(DirectoryExists(analysisReports)) 
    {
        DeleteDirectory(analysisReports, true);
    }

    EnsureDirectoryExists(analysisReports);
    
    var binDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\bin");
    var objDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\obj");
    DeleteDirectories(binDirs, true);
    DeleteDirectories(objDirs, true);
});

Task("Update-Version-Info")
    .IsDependentOn("Create-Version-Info")
    .Does(() => 
{
        versionInfo = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            UpdateAssemblyInfoFilePath = versionAssemblyInfo
        });

    if(versionInfo != null) {
        Information("Version: {0}", versionInfo.FullSemVer);
    } else {
        throw new Exception("Unable to determine version");
    }
});

Task("Create-Version-Info")
    .WithCriteria(() => !FileExists(versionAssemblyInfo))
    .Does(() =>
{
    Information("Creating version assembly info");
    CreateAssemblyInfo(versionAssemblyInfo, new AssemblyInfoSettings {
        Version = "0.0.0.0",
        FileVersion = "0.0.0.0",
        InformationalVersion = "",
    });
});

Task("DotNet-MsBuild-Restore")
    .IsDependentOn("Update-Version-Info")
    .Does(() => {

        MSBuild(solutionPath, c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithTarget("Restore")
        );
});

Task("Resharper-Analysis")
    .IsDependentOn("Build")
    .Does(() => {

        var msBuildProperties = new Dictionary<string, string>();
        msBuildProperties.Add("configuration", configuration);
        msBuildProperties.Add("platform", "AnyCPU");

        InspectCode(solutionPath, new InspectCodeSettings {
            SolutionWideAnalysis = true,
            //Profile = "./MySolution.sln.DotSettings",
            MsBuildProperties = msBuildProperties,
            OutputFile = analysisReports + "/" + File("inspectcode-output.xml"),
            ThrowExceptionOnFindingViolations = false
        });
});

Task("Resharper-Report")
    .IsDependentOn("Resharper-Analysis")
    .Does(() => {

        var input = analysisReports + "/" + File("inspectcode-output.xml");
        var output = analysisReports + "/" + File("inspectcode-output.html");

        ReSharperReports(input, output);
});
Task("DotNet-MsBuild")
    .IsDependentOn("Restore")
    .Does(() => {
        //Must be built in correct order

        //Base
        MSBuild("src/ArchitectNow.Models/ArchitectNow.Models.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithProperty("AssemblyVersion")
            .WithTarget("Build")
        );

        MSBuild("src/ArchitectNow.Services/ArchitectNow.Services.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithTarget("Build")
        );
        
        MSBuild("src/ArchitectNow.Web/ArchitectNow.Web.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithTarget("Build")
        );

        //Mongo
        MSBuild("src/ArchitectNow.Mongo/ArchitectNow.Mongo.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithTarget("Build")
        );

        MSBuild("src/ArchitectNow.Web.Mongo/ArchitectNow.Web.Mongo.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithTarget("Build")
        );

        //SQL

        /*
        MSBuild("src/ArchitectNow.Models/ArchitectNow.Models.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors)
            .WithTarget("Build")
        );
        */

});

Task("DotNet-MsBuild-Pack")
    .IsDependentOn("Build")
    .Does(() => {
       
       //Base
       MSBuild("src/ArchitectNow.Models/ArchitectNow.Models.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithProperty("IncludeSymbols", includeSymbols)
            .WithTarget("Pack"));

        MSBuild("src/ArchitectNow.Web/ArchitectNow.Web.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithProperty("IncludeSymbols", includeSymbols)
            .WithTarget("Pack"));

        MSBuild("src/ArchitectNow.Services/ArchitectNow.Services.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithProperty("IncludeSymbols", includeSymbols)
            .WithTarget("Pack"));

        //SQL
        MSBuild("src/ArchitectNow.Mongo/ArchitectNow.Mongo.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithProperty("IncludeSymbols", includeSymbols)
            .WithTarget("Pack"));

        MSBuild("src/ArchitectNow.Web.Mongo/ArchitectNow.Web.Mongo.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithProperty("IncludeSymbols", includeSymbols)
            .WithTarget("Pack"));
});

Task("DotNet-MsBuild-CopyToArtifacts")
    .IsDependentOn("DotNet-MsBuild-Pack")
    .Does(() => {
        
        EnsureDirectoryExists(artifacts);
        CopyFiles("src/ArchitectNow.Web/bin/" +configuration +"/*.nupkg", artifacts);
        CopyFiles("src/ArchitectNow.Models/bin/" +configuration +"/*.nupkg", artifacts);
        CopyFiles("src/ArchitectNow.Services/bin/" +configuration +"/*.nupkg", artifacts);
        CopyFiles("src/ArchitectNow.Mongo/bin/" +configuration +"/*.nupkg", artifacts);
        CopyFiles("src/ArchitectNow.Web.Mongo/bin/" +configuration +"/*.nupkg", artifacts);
});

Task("DotNet-Test")
    .IsDependentOn("Build")
    .Does(() => {

    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true
    };

    //DotNetCoreTest("tests/ArchitectNow.Web.Tests/ArchitectNow.Web.Tests.csproj", settings);
});

Task("MyGet-Upload-Artifacts")
    .IsDependentOn("Package")    
    .Does(() =>
{
    var nugetFeed = "https://www.myget.org/F/architectnow/api/v2/package";
    
    foreach(var nupkg in GetFiles(artifacts + "/*.nupkg")) {
        Information("Pushing: " + nupkg);
        NuGetPush(nupkg, new NuGetPushSettings {
             Source = nugetFeed
        });
    }
});

Task("MyGet-Upload-Symbols")
    .IsDependentOn("MyGet-Upload-Artifacts")    
    .Does(() =>
{
    var nugetFeed = "https://www.myget.org/F/architectnow/symbols/api/v2/package";
    
    foreach(var nupkg in GetFiles(artifacts +"/*.symbols.nupkg")) {
        Information("Pushing: " + nupkg);
        /*
        NuGetPush(nupkg, new NuGetPushSettings {
             Source = nugetFeed
        });
        */
    }
});

Task("Nuget-Upload-Artifacts")
    .IsDependentOn("Package")    
    .Does(() =>
{
    var nugetFeed = "https://www.nuget.org/api/v2/package";

    foreach(var nupkg in GetFiles(artifacts +"/*.nupkg")) {
        Information("Pushing: " + nupkg);
        /*NuGetPush(nupkg, new NuGetPushSettings {
            Source = nugetFeed
        });*/
    }
});

Task("MyGet")        
    // .IsDependentOn("Nuget-Upload-Artifacts")
    .IsDependentOn("MyGet-Upload-Artifacts")
    .IsDependentOn("MyGet-Upload-Symbols");

// ************************** //

Task("Restore")
    .IsDependentOn("DotNet-MsBuild-Restore");

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("DotNet-MsBuild");

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("DotNet-Test");

Task("Analysis")    
    .IsDependentOn("Resharper-Analysis")
    .IsDependentOn("Resharper-Report");

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("DotNet-MsBuild-CopyToArtifacts")
    .IsDependentOn("DotNet-MsBuild-Pack");

Task("CI")
    .IsDependentOn("MyGet")
    .IsDependentOn("Default");

Task("TestGlob")
    .Does(()=>{
        foreach(var nupkg in GetFiles(artifacts +"/*symbols.nupkg")) {
                Information("Pushing: " + nupkg);
        }

                Information("--------------------------");
        
        // Func<IFileSystemInfo, bool> exclude_symbols =
        //      fileSystemInfo => {
        //          Information(fileSystemInfo.Name);
        //           return !fileSystemInfo.Path.FullPath.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase);
        //      };

        // foreach(var nupkg in GetFiles(artifacts +"/*nupkg", exclude_symbols)) {
        //         Information("Pushing: " + nupkg);
        // }
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
