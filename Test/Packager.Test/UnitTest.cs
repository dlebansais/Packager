namespace Packager.Test;

using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class UnitTest
{
    private const string TestedAppName = "Packager";

    [Test]
    public void TestDefault1()
    {
        Process TestedApp = Launcher.Launch(TestedAppName);

        Assert.That(TestedApp, Is.Not.Null);
        Assert.That(TestedApp.HasExited, Is.True);
    }
}
