[TestFixture]
public class WebBrowserAppTests
{
    private WebBrowserApp _webBrowserApp;

    [SetUp]
    public void Setup()
    {
        _webBrowserApp = new WebBrowserApp();
    }

    [Test]
    public void TestAddressEntry()
    {
        Assert.IsNotNull(_webBrowserApp.addressEntry);
    }

    [Test]
    public void TestNavigateButton()
    {
        Assert.IsNotNull(_webBrowserApp.navigateButton);
    }

    [Test]
    public void TestHomeButton()
    {
        Assert.IsNotNull(_webBrowserApp.homeButton);
    }

    [Test]
    public void TestContentTextView()
    {
        Assert.IsNotNull(_webBrowserApp.contentTextView);
    }

    [Test]
    public void TestTitleLabel()
    {
        Assert.IsNotNull(_webBrowserApp.titleLabel);
    }

    [Test]
    public void TestScrolledWindow()
    {
        Assert.IsNotNull(_webBrowserApp.scrolledWindow);
    }

    [Test]
    public void TestHistoryButton()
    {
        Assert.IsNotNull(_webBrowserApp.historyButton);
    }

    [Test]
    public void TestBackButton()
    {
        Assert.IsNotNull(_webBrowserApp.backButton);
    }

    [Test]
    public void TestForwardButton()
    {
        Assert.IsNotNull(_webBrowserApp.forwardButton);
    }

    [Test]
    public void TestFavouritesButton()
    {
        Assert.IsNotNull(_webBrowserApp.favouritesButton);
    }

    [Test]
    public void TestEditHomeButton()
    {
        Assert.IsNotNull(_webBrowserApp.editHomeButton);
    }

    [Test]
    public void TestClearHistoryButton()
    {
        Assert.IsNotNull(_webBrowserApp.clearHistoryButton);
    }

    [Test]
    public void TestReloadButton()
    {
        Assert.IsNotNull(_webBrowserApp.reloadButton);
    }

    [Test]
    public void TestBulkDownloadButton()
    {
        Assert.IsNotNull(_webBrowserApp.bulkDownloadButton);
    }

    [Test]
    public void TestProfileButton()
    {
        Assert.IsNotNull(_webBrowserApp.profileButton);
    }

    [Test]
    public void TestComboBox()
    {
        Assert.IsNotNull(_webBrowserApp.comboBox);
    }

    [Test]
    public void TestListStore()
    {
        Assert.IsNotNull(_webBrowserApp.listStore);
    }

    [Test]
    public void TestCellRendererText()
    {
        Assert.IsNotNull(_webBrowserApp.cellRendererText);
    }
}