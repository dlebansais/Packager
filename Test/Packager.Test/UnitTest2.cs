namespace Packager.Test;

using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
internal class UnitTest2
{
    private const string TestedAppName = "Packager";

    [Test]
    public void TestNoParameter()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName);

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestSampleSolution()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestDebug()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestInvalidSolution()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-001");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestConditionalRelease()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestConditionalDebug()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--debug", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMissingInfo()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-002");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMerge()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeWithError()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-003");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeValidName()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge:Helper", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeInvalidName()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge:Invalid", workingDirectory: @"TestSolutions\Method.Contracts.Analyzers");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeWithNoSolution()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-004");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeWithNoProjects()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-005");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedVersions()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-006");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedAuthor()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-007");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedCopyright()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-008");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedRepositoryUrk()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-009");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks1()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-1");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks2()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-2");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks3()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-3");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedFrameworks4()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010-4");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestDescription()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeAndDescription()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge --description \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersion()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-011");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionTwo()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-012");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple1()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-1");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple2()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-2");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMergeMixedDependencyVersionMultiple3()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-013-3");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestAnalyzer()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestDebugAnalyzer()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--debug --analyzer", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestNuspecPrefix()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--prefix \"test\"", workingDirectory: @"TestSolutions\Method.Contracts");

        Assert.That(IsSuccessful, Is.True);
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

        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestAppIcon()
    {
        BackupPgSearchIcon();

        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.ico\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestAppPngIcon()
    {
        BackupPgSearchIcon();

        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: "--icon \"Resources\\main.png\"", workingDirectory: @"TestSolutions\PgSearch");

        RestorePgSearchIcon();

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMissingDetails()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-014");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestNoDependency()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-015");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestOtherFrameworks()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-016");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMissingIcon()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-017");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestMissingPng()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-018");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestNotPackable()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-019");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestIsTestProject()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-020");

        Assert.That(IsSuccessful, Is.True);
    }

    [Test]
    public void TestNoRepository()
    {
        bool IsSuccessful = Launcher.Launch(TestedAppName, arguments: null, workingDirectory: @"TestSolutions\Invalid-021");

        Assert.That(IsSuccessful, Is.True);
    }
}
