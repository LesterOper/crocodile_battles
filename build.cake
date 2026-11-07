#addin nuget:?package=Cake.Unity&version=0.8.0

var target = Argument("target", "Build-Android");

Task("Clean-Artifacts")
    .Does(() =>
{
    CleanDirectory($"./artifact");
});


Task("Build-Android")
    .IsDependentOn("Clean-Artifacts")
    .Does(() =>
{
    
UnityEditor(2020,3,
    new UnityEditorArguments
    {
     BatchMode = true,
     Quit = true,
     ExecuteMethod = "Builder.BuildAndroid",
     BuildTarget = BuildTarget.Android,
     LogFile = "./artifacts/unity.log",

    },
    new UnityEditorSettings{
        RealTimeLog =true,
    }
    );

});

RunTarget(target);
