namespace Packager.Test;

using System.Diagnostics;
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
    public void TestMergeMixedFrameworks()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-010");

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
    public void TestMergeMixedDependencyVersionMultiple()
    {
        Process TestedApp = Launcher.Launch(TestedAppName, arguments: "--merge", workingDirectory: @"TestSolutions\Invalid-012");

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }
}
