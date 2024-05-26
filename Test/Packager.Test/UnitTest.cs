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
        Process TestedApp = Launcher.Launch(TestedAppName);

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestSampleSolution()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments:null, workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDebug()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestInvalidSolution()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-001");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestConditionalRelease()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestConditionalDebug()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingInfo()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-002");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMerge()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithError()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-003");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeValidName()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge:Helper", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeInvalidName()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge:Invalid", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithNoSolution()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-004");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeWithNoProjects()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-005");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedVersions()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-006");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedAuthor()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-007");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedCopyright()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-008");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedRepositoryUrk()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-009");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks1()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-1");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks2()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-2");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks3()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-3");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks4()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-4");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDescription()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeAndDescription()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge --description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersion()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-011");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionTwo()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-012");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple1()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-1");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple2()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-2");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple3()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-3");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAnalyzer()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestDebugAnalyzer()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--debug --analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestNuspecPrefix()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--prefix \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

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

        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAppIcon()
    {
        BackupPgSearchIcon();

        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.ico\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestAppPngIcon()
    {
        BackupPgSearchIcon();

        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.png\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingDetails()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments:null, workingDirectory: @"TestSolutions\Invalid-014");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestNoDependency()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-015");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestOtherFrameworks()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-016");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingIcon()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-017");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }

    [Test]
    public void TestMissingPng()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-018");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }
}
