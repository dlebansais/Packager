namespace Packager.Test;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NuGet.Configuration;

internal static partial class Launcher
{
    public static bool Launch(string demoAppName, string? arguments = null, string? workingDirectory = null)
    {
        string? OpenCoverBasePath = GetPackagePath("opencover");

        string TestDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if NETFRAMEWORK
        string AppDirectory = TestDirectory.Replace(@"\Test\", @"\").Replace(@".Test\", @"\");
#else
        string AppDirectory = TestDirectory.Replace(@"\Test\", @"\", StringComparison.InvariantCulture).Replace(@".Test\", @"\", StringComparison.InvariantCulture).Replace(@"net8.0-windows\", @"net8.0-windows7.0\", StringComparison.InvariantCulture);
#endif
        string AppName = Path.Combine(AppDirectory, "win-x64", $"{demoAppName}.exe");
        string ResultFileName = Environment.GetEnvironmentVariable("RESULTFILENAME") ?? "result.xml";
        string CoverageAppName = @$"{OpenCoverBasePath}\tools\OpenCover.Console.exe";
        string CoverageAppArgs = @$"-register:user -target:""{AppName}"" -targetargs:""{arguments}"" -output:""{TestDirectory}{ResultFileName}"" -mergeoutput -mergebyhash";

        string WorkingDirectory = workingDirectory is null
            ? string.Empty
            : Path.Combine(TestDirectory, $"..\\..\\..\\..\\{workingDirectory}");

        // Console.WriteLine($"{CoverageAppName}");
        // Console.WriteLine($"{CoverageAppArgs}");
        // Console.WriteLine($"{WorkingDirectory}");

        ProcessStartInfo StartInfo = new()
        {
            FileName = CoverageAppName,
            Arguments = CoverageAppArgs,
            WorkingDirectory = WorkingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        Thread.Sleep(TimeSpan.FromSeconds(1));

        using FileStream OutputStream = new("output.txt", FileMode.Append, FileAccess.Write);
        using StreamWriter OutputWriter = new(OutputStream);
        using Process TestProcess = new();

        TestProcess.StartInfo = StartInfo;
        TestProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (e is not null && e.Data is not null)
                OutputWriter.WriteLine(e.Data);
        });

        TestProcess.Start();
        TestProcess.BeginOutputReadLine();
        TestProcess.WaitForExit();

        OutputWriter.Flush();

        return true;
    }

    private static string? GetPackagePath(string packageName)
    {
        if (Settings.LoadDefaultSettings(null) is var NugetSettings && SettingsUtility.GetGlobalPackagesFolder(NugetSettings) is var NugetFolder)
        {
            string PackageBasePath = Path.Combine(NugetFolder, packageName);
            string[] Directories = Directory.GetDirectories(PackageBasePath);
            int[]? SelectedVersion = null;
            int SelectedVersionIndex = -1;

            for (int i = 0; i < Directories.Length; i++)
                if (GetVersion(Path.GetFileName(Directories[i])) is int[] Version && (SelectedVersion is null || CompareVersion(SelectedVersion, Version) > 0))
                {
                    SelectedVersion = Version;
                    SelectedVersionIndex = i;
                }

            if (SelectedVersionIndex >= 0)
            {
                string PackageVersionPath = Directories[SelectedVersionIndex];
                return PackageVersionPath;
            }
        }

        return null;
    }

    private static int[]? GetVersion(string formattedVersion)
    {
        string[] Versions = formattedVersion.Split('.');
        int[] Version = new int[Versions.Length];

        for (int i = 0; i < Versions.Length; i++)
            if (int.TryParse(Versions[i], out int Value))
                Version[i] = Value;
            else
                return null;

        return Version;
    }

    private static int CompareVersion(int[] version1, int[] version2)
    {
        for (int i = 0; i < version1.Length && i < version2.Length; i++)
            if (version1[i] < version2[i])
                return 1;
            else if (version1[i] > version2[i])
                return -1;

        return version2.Length - version1.Length;
    }
}
