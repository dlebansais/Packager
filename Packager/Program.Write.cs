namespace Packager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using Contracts;
using SlnExplorer;

/// <summary>
/// Generates a .nuspec file based on project .csproj content.
/// </summary>
internal partial class Program
{
    private static void WriteNuspec(Nuspec nuspec, bool isDebug, bool isAnalyzer, string nuspecIcon, string nuspecPrefix)
    {
        if (nuspec.RelativePath.Length > 0)
            ConsoleDebug.Write($"  Processing: {nuspec.RelativePath}");
        else
            ConsoleDebug.Write($"  Processing...");

        InitializeFile(nuspec, isDebug, nuspecPrefix, out string NuspecPath);

        using FileStream Stream = new(NuspecPath, FileMode.Append, FileAccess.Write);
        using StreamWriter Writer = new(Stream, Encoding.UTF8);

        Writer.WriteLine("<package xmlns=\"http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd\">");
        Writer.WriteLine("  <metadata>");

        string ApplicationIcon = nuspecIcon.Length > 0 ? nuspecIcon : nuspec.ApplicationIcon;

        WriteMiscellaneousInfo(Writer, nuspec, isDebug, NuspecPath, ApplicationIcon, nuspecPrefix);
        WriteDependencies(Writer, nuspec, isAnalyzer);
        WriteExtraContentFiles(Writer, isDebug, isAnalyzer);

        Writer.WriteLine("  </metadata>");
        Writer.Write("</package>");
    }

    private static void InitializeFile(Nuspec nuspec, bool isDebug, string nuspecPrefix, out string nuspecPath)
    {
        string DebugSuffix = GetDebugSuffix(isDebug);
        string NugetDirectory = isDebug ? "nuget-debug" : "nuget";
        string Prefix = nuspecPrefix == string.Empty ? string.Empty : $"{nuspecPrefix}.";
        string NuspecFileName = $"{Prefix}{nuspec.Name}{DebugSuffix}.nuspec";
        nuspecPath = Path.Combine(NugetDirectory, NuspecFileName);

        using FileStream FirstStream = new(nuspecPath, FileMode.Create, FileAccess.Write);
        using StreamWriter FirstWriter = new(FirstStream, Encoding.ASCII);
        FirstWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

        ConsoleDebug.Write($"     Created: {nuspecPath}");
    }

    private static void WriteMiscellaneousInfo(StreamWriter writer, Nuspec nuspec, bool isDebug, string nuspecPath, string nuspecIcon, string nuspecPrefix)
    {
        string Prefix = nuspecPrefix == string.Empty ? string.Empty : $"{nuspecPrefix}.";
        string DebugSuffix = GetDebugSuffix(isDebug);
        string DebugTitle = GetDebugTitle(isDebug);

        writer.WriteLine($"    <id>{Prefix}{nuspec.Name}{DebugSuffix}</id>");
        writer.WriteLine($"    <version>{nuspec.Version}</version>");
        writer.WriteLine($"    <title>{nuspec.Name}{DebugTitle}</title>");

        if (nuspec.Author.Length > 0)
            writer.WriteLine($"    <authors>{HtmlEncoded(nuspec.Author)}</authors>");

        if (nuspec.Description.Length > 0)
            writer.WriteLine($"    <description>{HtmlEncoded(nuspec.Description)}</description>");

        if (nuspec.Copyright.Length > 0)
            writer.WriteLine($"    <copyright>{HtmlEncoded(nuspec.Copyright)}</copyright>");

        if (nuspecIcon.Length > 0)
        {
            ConvertIcoToPng(nuspecPath, ref nuspecIcon);
            writer.WriteLine($"    <icon>{nuspecIcon}</icon>");
        }
        else if (nuspec.PackageIcon.Length > 0)
        {
            writer.WriteLine($"    <icon>{nuspec.PackageIcon}</icon>");
        }

        if (nuspec.PackageLicenseExpression.Length > 0)
        {
            writer.WriteLine($"    <license type=\"expression\">{nuspec.PackageLicenseExpression}</license>");
        }

        if (nuspec.PackageReadmeFile.Length > 0)
        {
            writer.WriteLine($"    <readme>{nuspec.PackageReadmeFile}</readme>");
        }

        writer.WriteLine($"    <projectUrl>{nuspec.RepositoryUrl}</projectUrl>");
        writer.WriteLine($"    <repository type=\"git\" url=\"{nuspec.RepositoryUrl}\"/>");
    }

    private static void WriteDependencies(StreamWriter writer, Nuspec nuspec, bool isAnalyzer)
    {
        writer.WriteLine("    <dependencies>");

        foreach (Framework Framework in nuspec.FrameworkList)
            if (!isAnalyzer || Framework.Name == "netstandard2.0")
                WriteFrameworkDependency(writer, nuspec, Framework);

        writer.WriteLine("    </dependencies>");
    }

    private static void WriteFrameworkDependency(StreamWriter writer, Nuspec nuspec, Framework framework)
    {
        string FrameworkName = FrameworkTypeToName(framework);
        string MonikerName = FrameworkMonikerToName(framework);
        string VersionString = FrameworkToVersion(framework);
        string TargetFrameworkName = $"{FrameworkName}{VersionString}{MonikerName}";

        Contract.Assert(nuspec.FrameworkList.Count > 0);

        PackageReferenceList FilteredDependencies = [];
        foreach (KeyValuePair<Framework, PackageReferenceList> Entry in nuspec.PackageDependencies)
        {
            Framework DependencyFramework = Entry.Key;
            if (DependencyFramework == Nuspec.AnyFramework || DependencyFramework == framework)
                FilteredDependencies.AddRange(Entry.Value);
        }

        if (FilteredDependencies.Count > 0)
        {
            writer.WriteLine($"      <group targetFramework=\"{TargetFrameworkName}\">");

            foreach (PackageReference Item in FilteredDependencies)
                writer.WriteLine($"        <dependency id=\"{Item.Name}\" version=\"{Item.Version}\"/>");

            writer.WriteLine($"      </group>");
        }
        else
        {
            writer.WriteLine($"      <group targetFramework=\"{TargetFrameworkName}\"/>");
        }
    }

    private static string FrameworkTypeToName(Framework framework)
    {
        Dictionary<FrameworkType, Func<string>> Table = new()
        {
            { FrameworkType.NetStandard, () => ".NETStandard" },
            { FrameworkType.NetCore, () => ".NETCoreApp" },
            { FrameworkType.NetFramework, () => (framework.Major < 5) ? ".NETFramework" : "net" },
        };

        bool IsKnown = Table.TryGetValue(framework.Type, out Func<string>? Handler);
        Contract.Assert(IsKnown);

        return Contract.AssertNotNull(Handler)();
    }

    private static string FrameworkMonikerToName(Framework framework)
    {
        Dictionary<FrameworkMoniker, Func<string>> Table = new()
        {
            { FrameworkMoniker.none, () => string.Empty },
            { FrameworkMoniker.android, () => $"-{framework.Moniker}" },
            { FrameworkMoniker.ios, () => $"-{framework.Moniker}" },
            { FrameworkMoniker.macos, () => $"-{framework.Moniker}" },
            { FrameworkMoniker.tvos, () => $"-{framework.Moniker}" },
            { FrameworkMoniker.watchos, () => $"-{framework.Moniker}" },
            { FrameworkMoniker.windows, () => (framework.MonikerMajor >= 0 && framework.MonikerMinor >= 0) ? $"-{framework.Moniker}{framework.MonikerMajor}.{framework.MonikerMinor}" : $"-{framework.Moniker}" },
        };

        bool IsKnown = Table.TryGetValue(framework.Moniker, out Func<string>? Handler);
        Contract.Assert(IsKnown);

        return Contract.AssertNotNull(Handler)();
    }

    private static string FrameworkToVersion(Framework framework)
    {
        Dictionary<string, string> ThreeVersionFrameworks = new()
        {
            { "net403", "4.0.3" },
            { "net451", "4.5.1" },
            { "net452", "4.5.2" },
            { "net461", "4.6.1" },
            { "net462", "4.6.2" },
            { "net471", "4.7.1" },
            { "net472", "4.7.2" },
            { "net481", "4.8.1" },
        };

        return ThreeVersionFrameworks.TryGetValue(framework.Name, out string? Value)
            ? Value
            : $"{framework.Major}.{framework.Minor}";
    }

    private static void WriteExtraContentFiles(StreamWriter writer, bool isDebug, bool isAnalyzer)
    {
        if (isDebug)
        {
            writer.WriteLine("    <contentFiles>");

            if (isAnalyzer)
                writer.WriteLine("      <files include=\"analyzers/**/*.pdb\"/>");
            else
                writer.WriteLine("      <files include=\"lib/**/*.pdb\"/>");

            writer.WriteLine("    </contentFiles>");
        }
    }

    private static string GetDebugSuffix(bool isDebug) => isDebug ? "-Debug" : string.Empty;

    private static string GetDebugTitle(bool isDebug) => isDebug ? " (Debug)" : string.Empty;

    private static string HtmlEncoded(string s)
    {
        Replace(ref s, "\"", "&quot;");
        Replace(ref s, "?", "&amp;");
        Replace(ref s, "<", "&lt;");
        Replace(ref s, ">", "&gt;");

        return s;
    }

#if NET5_0_OR_GREATER
    private static void Replace(ref string s, string oldValue, string newValue) => s = s.Replace(oldValue, newValue, StringComparison.InvariantCulture);
#else
    private static void Replace(ref string s, string oldValue, string newValue) => s = s.Replace(oldValue, newValue);
#endif

    private static void ConvertIcoToPng(string nuspecPath, ref string fileName)
    {
        if (Path.GetExtension(fileName) != ".ico")
            return;

        string NuspecFolder = Contract.AssertNotNull(Path.GetDirectoryName(nuspecPath));
        string IconFileName = Path.Combine(NuspecFolder, Path.GetFileName(fileName));
        string PngFileName = Path.ChangeExtension(IconFileName, ".png");

        if (!File.Exists(IconFileName))
        {
            if (File.Exists(PngFileName))
                fileName = Path.ChangeExtension(fileName, ".png");

            return;
        }

        ConvertIcoToPng(IconFileName, PngFileName);

        File.Delete(IconFileName);

        fileName = Path.ChangeExtension(fileName, ".png");
    }

    private static void ConvertIcoToPng(string iconFileName, string pngFileName)
    {
        using FileStream IconStream = new(iconFileName, FileMode.Open, FileAccess.Read);
        using FileStream PngStream = new(pngFileName, FileMode.Create, FileAccess.Write);

        IconBitmapDecoder Decoder = new(IconStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
        PngBitmapEncoder Encoder = new();
        Encoder.Frames.Add(Decoder.Frames[0]);
        Encoder.Save(PngStream);
    }
}
