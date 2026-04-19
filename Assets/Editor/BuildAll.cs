using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class BuildAll
{
    private static readonly string[] scenes = { "Assets/Scenes/SampleScene.unity" };

    [MenuItem("Build/Build All Platforms")]
    public static void BuildAllPlatforms()
    {
        BuildWindows();
        BuildMac();
        BuildLinux();
        Debug.Log("All platform builds complete.");
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        string path = "Builds/Windows/Communion.exe";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        Run(new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = path,
            target           = BuildTarget.StandaloneWindows64,
            options          = BuildOptions.None,
        }, "Windows");
    }

    [MenuItem("Build/Build Mac")]
    public static void BuildMac()
    {
        string path = "Builds/Mac.app";
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "Builds");
        Run(new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = path,
            target           = BuildTarget.StandaloneOSX,
            options          = BuildOptions.None,
        }, "Mac");
    }

    [MenuItem("Build/Build Linux")]
    public static void BuildLinux()
    {
        string path = "Builds/Linux/Communion.x86_64";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        Run(new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = path,
            target           = BuildTarget.StandaloneLinux64,
            options          = BuildOptions.None,
        }, "Linux");
    }

    static void Run(BuildPlayerOptions opts, string label)
    {
        BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"{label} build succeeded — {report.summary.totalSize / 1048576f:F1} MB");
        else
            Debug.LogError($"{label} build FAILED: {report.summary.result}");
    }
}
