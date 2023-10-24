namespace nUnitTesting;
using MainApp;
using Gtk;

public class Tests
{
    private WebBrowserApp app;

    [SetUp]
    public void Setup()
    {
        Application.Init();

        // Create your GTK# application window (replace with your window creation code).
        app = new WebBrowserApp();

        // Load the home page or set up any initial state.
        app.LoadHomePage();
        app.ShowAll();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after the test.
        app.Destroy();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}