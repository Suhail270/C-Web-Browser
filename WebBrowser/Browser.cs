using Gtk;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;

public class WebBrowserApp : Window
{
    private Entry addressEntry;
    private Button navigateButton;
    private Button homeButton;
    private TextView contentTextView;
    private Label titleLabel;
    private ScrolledWindow scrolledWindow;
    private string displayUrl;
    private string currentUrl;
    private List<string> prevUrl = new List<string>();
    private string prevDisplay;
    private List<string> nextUrl = new List<string>();
    private string nextDisplay;
    private string homePath = "homeURL.txt";
    private string homePageUrl;
    private string favouritesPath = "favourites.txt";
    private string historyPath = "history.txt";
    private bool startUp = true;
    private Button historyButton;
    private Button backButton;
    private Button forwardButton;
    private Button favouritesButton;
    private string localHistoryBack = "back.txt";
    private string localHistoryForward = "forward.txt";
    TextIter startIter;
    TextIter endIter;
    public WebBrowserApp() : base("F20SC - CW1")
    {
        SetDefaultSize(900, 700);
        SetPosition(WindowPosition.Center);

        var mainVBox = new VBox();
        var mainHBox = new HBox();
        var titleHBox = new HBox();

        addressEntry = new Entry();
        addressEntry.WidthChars = 50;
        navigateButton = new Button("Go");
        homeButton = new Button("Home");
        backButton = new Button("Back");
        forwardButton = new Button("Forward");
        favouritesButton = new Button("Favourites");
        contentTextView = new TextView();
        scrolledWindow = new ScrolledWindow();
        titleLabel = new Label();
        historyButton = new Button("History"); // Create the history button
        
        historyButton.Clicked += HistoryButton_ClickedAsync; // Attach an event handler
        navigateButton.Clicked += NavigateButton_Clicked;
        homeButton.Clicked += HomeButton_Clicked;
        backButton.Clicked += BackButton_Clicked;
        forwardButton.Clicked += ForwardButton_Clicked;
        favouritesButton.Clicked += FavouritesButton_Clicked;

        mainVBox.PackStart(mainHBox, false, false, 0);
        mainVBox.PackStart(titleHBox, false, false, 0);

        mainHBox.PackStart(backButton, false, false, 0);
        mainHBox.PackStart(forwardButton, false, false, 10);
        mainHBox.PackStart(addressEntry, false, false, 10);
        mainHBox.PackStart(navigateButton, false, false, 10);
        mainHBox.PackStart(homeButton, false, false, 10);
        mainHBox.PackStart(historyButton, false, false, 10);
        mainHBox.PackStart(favouritesButton, false, false, 10);

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

    private async void FavouritesButton_Clicked(object sender, EventArgs e)
{
    try
    {
        string[] favourites = await ReadFavouritesAsync();

        if (favourites.Length > 0)
        {
            string favouritesText = string.Join("\n", favourites);
            contentTextView.Buffer.Text = favouritesText;
            titleLabel.Text = "Favourites";

            var hyperlinkTag = contentTextView.Buffer.TagTable.Lookup("hyperlink");

            // If it doesn't exist, create a new "hyperlink" tag
            if (hyperlinkTag == null)
            {
                hyperlinkTag = new TextTag("hyperlink");
                hyperlinkTag.Underline = Pango.Underline.Single;
                contentTextView.Buffer.TagTable.Add(hyperlinkTag);
            }


            hyperlinkTag.Underline = Pango.Underline.Single;
            contentTextView.Buffer.TagTable.Add(hyperlinkTag);

            // Apply the hyperlink tag to specific text portions
            var buffer = contentTextView.Buffer;
            int startOffset = 0;
            foreach (var line in favourites)
            {
                int endOffset = startOffset + line.Length;
                buffer.ApplyTag(hyperlinkTag, buffer.GetIterAtOffset(startOffset), buffer.GetIterAtOffset(endOffset));
                startOffset = endOffset + 1;
            }

            // Handle button press events
            contentTextView.ButtonPressEvent += (s, args) =>
            {
                if (args.Event.Type == Gdk.EventType.ButtonPress && args.Event.Button == 1)
                {
                    TextIter iter;
                    if (contentTextView.GetIterAtLocation(out iter, (int)args.Event.X, (int)args.Event.Y))
                    {
                        TextTag[] tags = iter.Tags;
                        foreach (TextTag tag in tags)
                        {
                            if (tag.Name == "hyperlink")
                            {
                                // Handle the hyperlink click
                                int start = iter.Offset;
                                int end = iter.Offset;

                                // Find the start and end of the clicked link
                                while (start >= 0 && buffer.GetIterAtOffset(start).HasTag(tag))
                                {
                                    start--;
                                }

                                while (end < buffer.Text.Length && buffer.GetIterAtOffset(end).HasTag(tag))
                                {
                                    end++;
                                }

                                // Extract the URL from the clicked link
                                string hyperlinkText = buffer.GetText(buffer.GetIterAtOffset(start + 1), buffer.GetIterAtOffset(end), false);
                                DisplayWebContent(hyperlinkText, "nav");
                            }
                        }
                    }
                }
            };
        }
        else
        {
            contentTextView.Buffer.Text = "Favourites is empty.";
            titleLabel.Text = string.Empty;
        }
    }
    catch (Exception ex)
    {
        contentTextView.Buffer.Text = $"Error: {ex.Message}";
        titleLabel.Text = string.Empty;
    }
}


    private async Task<string[]> ReadFavouritesAsync()
    {
        if (File.Exists(favouritesPath))
        {
            return await File.ReadAllLinesAsync(favouritesPath);
        }

        return new string[0];
    }
    
     private void BackButton_Clicked(object sender, EventArgs e) //Need to update previous and next lists in the displaywebcontent function
     {
        string[] localSession = File.ReadAllLines(localHistoryBack);

        if (localSession.Length==0){
            ShowMessage("No previous URL.");
        }
        else{
            string prevURL = localSession.Last();
            string[] updatedSession = localSession.Take(localSession.Length - 1).ToArray();
            File.WriteAllLines(localHistoryBack, updatedSession);
            DisplayWebContent(prevURL, "back");
        }
    }

    private void ForwardButton_Clicked(object sender, EventArgs e)
     {

        string[] localSessionForward = File.ReadAllLines(localHistoryForward);

        if (localSessionForward.Length==0){
            ShowMessage("No next URL.");
        }
        else{
            string nextURL = localSessionForward.Last();
            string[] updatedSessionForward = localSessionForward.Take(localSessionForward.Length - 1).ToArray();
            File.WriteAllLines(localHistoryForward, updatedSessionForward);
            DisplayWebContent(nextURL, "next");
        }
    }

    private async void HistoryButton_ClickedAsync(object sender, EventArgs e)
    {
        try
        {
            string[] history = await ReadHistoryAsync();

            if (history.Length > 0)
            {
                string historyText = string.Join("\n", history);
                contentTextView.Buffer.Text = historyText;
                titleLabel.Text = "History";
            }
            else
            {
                contentTextView.Buffer.Text = "History is empty.";
                titleLabel.Text = string.Empty;
            }
        }
        catch (Exception ex)
        {
            contentTextView.Buffer.Text = $"Error: {ex.Message}";
            titleLabel.Text = string.Empty;
        }
    }

    private async Task<string[]> ReadHistoryAsync()
    {
        if (File.Exists(historyPath))
        {
            return await File.ReadAllLinesAsync(historyPath);
        }

        return new string[0];
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
    private async void DisplayWebContent(string url, string action){

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            ShowMessage("Invalid URL.");
            return;
        }

        try
        {
            string content = await FetchWebContentAsync(url, action);
            addressEntry.Text = url;

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

        }
        catch (Exception ex)
        {
            contentTextView.Buffer.Text = $"Error: {ex.Message}";
            titleLabel.Text = string.Empty;
        }

    }

    private async void NavigateButton_Clicked(object sender, EventArgs e)
    {

        displayUrl = addressEntry.Text;
        DisplayWebContent(displayUrl, "nav");
    }

    private async Task<string> FetchWebContentAsync(string url, string action)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {   
                    if (File.Exists(localHistoryBack) && (action=="next" || action=="nav"))
                    {
                        File.AppendAllText(localHistoryBack, currentUrl+"\n");
                    }

                    else if (File.Exists(localHistoryForward) && action=="back"){
                        File.AppendAllText(localHistoryForward, currentUrl+"\n");
                    }

                    else if (action=="home"){
                        if(!startUp){
                            File.AppendAllText(localHistoryBack, currentUrl+"\n");
                        }
                    }

                    if(!startUp){
                        currentUrl = url;
                        await UpdateHistoryAsync();
                    }
                    else{
                        startUp = false;
                        currentUrl = url;
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

            DisplayWebContent(homePageUrl, "home");
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
        File.WriteAllText(app.localHistoryBack, "");
        File.WriteAllText(app.localHistoryForward, "");
        app.LoadHomePage();
        app.ShowAll();
        Application.Run();
    }
}