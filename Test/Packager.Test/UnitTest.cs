namespace Packager.Test;

using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class UnitTest
{
    private const string TestedAppName = "Packager";

    [Test]
    public void TestNoParameter()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName);

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestSampleSolution()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments:null, workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDebug()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestInvalidSolution()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-001");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestConditionalRelease()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestConditionalDebug()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingInfo()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-002");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMerge()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithError()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-003");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeValidName()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge:Helper", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeInvalidName()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge:Invalid", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithNoSolution()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-004");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithNoProjects()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-005");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedVersions()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-006");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedAuthor()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-007");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedCopyright()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-008");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedRepositoryUrk()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-009");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks1()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-1");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks2()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-2");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks3()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-3");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks4()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-4");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDescription()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeAndDescription()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge --description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersion()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-011");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionTwo()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-012");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple1()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-1");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple2()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-2");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple3()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-3");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAnalyzer()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDebugAnalyzer()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug --analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestNuspecPrefix()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--prefix \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    private static void BackupPgSearchIcon()
    {
        string TestDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string WorkingDirectory = Path.Combine(TestDirectory, $"..\\..\\..\\..\\TestSolutions\\PgSearch\\nuget");
        File.Copy($"{WorkingDirectory}\\main.ico", $"{WorkingDirectory}\\copy.ico", overwrite: true);
    }

    private static void RestorePgSearchIcon()
    {
        string TestDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string WorkingDirectory = Path.Combine(TestDirectory, $"..\\..\\..\\..\\TestSolutions\\PgSearch\\nuget");
        File.Copy($"{WorkingDirectory}\\copy.ico", $"{WorkingDirectory}\\main.ico", overwrite: true);
        File.Delete($"{WorkingDirectory}\\copy.ico");
    }

    [Test]
    public void TestDefaultAppIcon()
    {
        BackupPgSearchIcon();

        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAppIcon()
    {
        BackupPgSearchIcon();

        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.ico\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAppPngIcon()
    {
        BackupPgSearchIcon();

        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.png\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingDetails()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments:null, workingDirectory: @"TestSolutions\Invalid-014");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestNoDependency()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-015");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestOtherFrameworks()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-016");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingIcon()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-017");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingPng()
    {
        using Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-018");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }
}
