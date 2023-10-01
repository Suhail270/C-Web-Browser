using Gtk;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class WebBrowserApp : Window
{
    private Entry addressEntry;
    private Button navigateButton;
    private Button homeButton;
    private TextView contentTextView;
    private Label titleLabel;
    private ScrolledWindow scrolledWindow;
    private string currentUrl;
    private string homePath = "homeURL.txt";
    private string homePageUrl;
    private string favouritesPath = "favourites.txt";
    private string historyPath = "history.txt";
    private bool startUp = true;
    private Button historyButton; //TODO

    public WebBrowserApp() : base("F20SC - CW1")
    {
        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);

        var mainVBox = new VBox();
        var mainHBox = new HBox();
        var titleHBox = new HBox();

        addressEntry = new Entry();
        addressEntry.WidthChars = 50;
        navigateButton = new Button("Go");
        homeButton = new Button("Home");
        contentTextView = new TextView();
        scrolledWindow = new ScrolledWindow();
        titleLabel = new Label();

        navigateButton.Clicked += NavigateButton_Clicked;
        homeButton.Clicked += HomeButton_Clicked;

        mainVBox.PackStart(mainHBox, false, false, 0);
        mainVBox.PackStart(titleHBox, false, false, 0);

        mainHBox.PackStart(addressEntry, false, false, 0);
        mainHBox.PackStart(navigateButton, false, false, 10);
        mainHBox.PackStart(homeButton, false, false, 10);

        titleHBox.PackStart(titleLabel, false, false, 10);

        var textBuffer = new TextBuffer(null);
        contentTextView.Buffer = textBuffer;
        contentTextView.Editable = false;
        contentTextView.WrapMode = WrapMode.WordChar;

        scrolledWindow.Add(contentTextView);
        mainVBox.PackStart(scrolledWindow, true, true, 5);

        Add(mainVBox);

        DeleteEvent += (sender, args) => Application.Quit();

        contentTextView.Buffer.Text = "Enter a URL and click 'Go' to view HTML code.";
    }

    private void ShowMessage(string message)
{
    GLib.Idle.Add(() =>
    {
        var messageDialog = new MessageDialog(
            this,
            DialogFlags.Modal,
            MessageType.Info,
            ButtonsType.Ok,
            message
        );

        messageDialog.Run();
        messageDialog.Destroy();
        return false;
    });
}

    private void HomeButton_Clicked(object sender, EventArgs e)
    {
        LoadHomePage();
    }

    private async void NavigateButton_Clicked(object sender, EventArgs e)
    {
        string url = addressEntry.Text;
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            ShowMessage("Invalid URL.");
            return;
        }

        try
        {
            string content = await FetchWebContentAsync(url);

            // Display the HTML code in the TextView
            contentTextView.Buffer.Text = content;
            // Display the HTTP response status code
            string statusCode = $"HTTP Status Code: {currentUrl}";
            titleLabel.Text = statusCode;

            // Parse the title of the web page and display it
            string title = GetWebPageTitle(content);
            if (!string.IsNullOrWhiteSpace(title))
            {
                titleLabel.Text += $" - Title: {title}";
            }

            // Store the current URL
            currentUrl = url;
        }
        catch (Exception ex)
        {
            contentTextView.Buffer.Text = $"Error: {ex.Message}";
            titleLabel.Text = string.Empty;
        }
    }

    private async Task<string> FetchWebContentAsync(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {   
                    currentUrl = url;

                    if(!startUp){
                        await UpdateHistoryAsync();
                    }
                    else{
                        startUp = false;
                    }
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Request Error: {ex.Message}");
        }
    }

    private async void LoadHomePage()
    {
        try
        {
            homePageUrl = await ReadHomePageAsync();

            if (string.IsNullOrWhiteSpace(homePageUrl))
            {
                ShowMessage("Home page URL not set.");
                return;
            }

            string content = await FetchWebContentAsync(homePageUrl);

            // Display the HTML code in the TextView
            contentTextView.Buffer.Text = content;

            // Display the HTTP response status code
            string statusCode = $"HTTP Status Code: {currentUrl}";
            titleLabel.Text = statusCode;

            // Parse the title of the web page and display it
            string title = GetWebPageTitle(content);
            if (!string.IsNullOrWhiteSpace(title))
            {
                titleLabel.Text += $" - Title: {title}";
            }

            // Store the current URL
            currentUrl = homePageUrl;
        }
        catch (Exception ex)
        {
            contentTextView.Buffer.Text = $"Error: {ex.Message}";
            titleLabel.Text = string.Empty;
        }
    }

    private async Task<string> ReadHomePageAsync()
    {
        if (File.Exists(homePath))
        {
            return await File.ReadAllTextAsync(homePath);
        }

        return string.Empty;
    }

    private async Task<string> UpdateHistoryAsync()
    {
        if (File.Exists(historyPath))
        {
            File.AppendAllText(historyPath, currentUrl+"\n");
        }

        return string.Empty;
    }

    private string GetWebPageTitle(string htmlContent)
    {
        try
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);
            var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");
            return titleNode?.InnerText;
        }
        catch
        {
            return null;
        }
    }

    public static void Main(string[] args)
    {
        Application.Init();
        var app = new WebBrowserApp();
        app.LoadHomePage();
        app.ShowAll();
        Application.Run();
    }
}