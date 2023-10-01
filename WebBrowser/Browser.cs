using Gtk;
using System;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;

public class WebBrowserApp : Window
{
    private Entry addressEntry;
    private Button navigateButton;
    private Button homeButton;
    private TextView contentTextView;
    private Label titleLabel; // Add a Label for displaying the title
    private ScrolledWindow scrolledWindow;
    private string currentUrl;
    private string homePath = "homeURL.txt";
    private string homePageUrl;

    private Dialog editHomeDialog;
    private Entry editHomeEntry;
    private Button editHomeButton;

    
    public WebBrowserApp() : base("F20SC - CW1")
    {
        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);

        // Create UI elements
        var mainVBox = new VBox();
        var mainHBox = new HBox();
        var titleHBox = new HBox(); // Add a new HBox for title and reload button

        addressEntry = new Entry();
        addressEntry.WidthChars = 50;
        navigateButton = new Button("Go");
        homeButton = new Button("Home");
        editHomeButton = new Button("Edit Home");
        contentTextView = new TextView();
        scrolledWindow = new ScrolledWindow();
        titleLabel = new Label(); // Initialize the Label

        navigateButton.Clicked += NavigateButton_Clicked;
        homeButton.Clicked += HomeButton_Clicked;
        editHomeButton.Clicked += EditHomeButton_Clicked;

        mainVBox.PackStart(mainHBox, false, false, 0);
        mainVBox.PackStart(titleHBox, false, false, 0); // Add the titleHBox to mainVBox

        mainHBox.PackStart(addressEntry, false, false, 0);
        mainHBox.PackStart(navigateButton, false, false, 10);
        mainHBox.PackStart(homeButton, false, false, 10);
        mainHBox.PackStart(editHomeButton, false, false, 10); // Add the editHomeButton


        titleHBox.PackStart(titleLabel, false, false, 10); // Add the titleLabel to titleHBox

        var textBuffer = new TextBuffer(null);
        contentTextView.Buffer = textBuffer;
        contentTextView.Editable = false;
        contentTextView.WrapMode = WrapMode.WordChar;

        scrolledWindow.Add(contentTextView);
        mainVBox.PackStart(scrolledWindow, true, true, 5);

        // Inside the WebBrowserApp constructor
        editHomeDialog = new Dialog("Edit Home Page URL", this, DialogFlags.Modal);

        editHomeEntry = new Entry();
        editHomeEntry.Text = homePageUrl; // Set the current home page URL as the default text
        editHomeEntry.WidthChars = 50;

        editHomeDialog.ContentArea.PackStart(editHomeEntry, true, true, 0);
        editHomeDialog.AddButton("Cancel", ResponseType.Cancel);
        editHomeDialog.AddButton("OK", ResponseType.Ok);

        // Handle the response from the dialog when the "OK" button is clicked
        editHomeDialog.Response += (o, args) =>
        {
            if (args.ResponseId == ResponseType.Ok)
            {
                // Update the homePageUrl with the edited URL
                string url = editHomeEntry.Text;
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    ShowMessage("Invalid URL.");
                    return;
                }

                else{
                    homePageUrl = editHomeEntry.Text;

                    // Save the updated home page URL to the "homeURL.txt" file
                    File.WriteAllText(homePath, homePageUrl);

                    editHomeDialog.Hide();

                    ShowMessage("Home page updated successfully.");
                    LoadHomePage();
                }
                
            }
            // Close the dialog
            editHomeDialog.Hide();
        };

        Add(mainVBox);

        DeleteEvent += (sender, args) => Application.Quit();

        contentTextView.Buffer.Text = "Enter a URL and click 'Go' to view HTML code.";
    }

    // Define the ShowMessage method within the WebBrowserApp class
    private void ShowMessage(string message)
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
    }

    private void EditHomeButton_Clicked(object sender, EventArgs e)
    {
        // Show the custom edit home dialog
        editHomeDialog.ShowAll();

    }

    private async void NavigateButton_Clicked(object sender, EventArgs e)
    {
        string url = addressEntry.Text;
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            contentTextView.Buffer.Text = "Invalid URL";
            return;
        }

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Display the HTML code in the TextView
                    contentTextView.Buffer.Text = content;

                    // Display the HTTP response status code
                    string statusCode = $"HTTP Status Code: {(int)response.StatusCode} {response.ReasonPhrase}";
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
                else
                {
                    contentTextView.Buffer.Text = $"HTTP Error {response.StatusCode}: {content}";
                    titleLabel.Text = string.Empty; // Clear the title if there's an error
                }
            }
        }
        catch (HttpRequestException ex)
        {
            contentTextView.Buffer.Text = $"Request Error: {ex.Message}";
            titleLabel.Text = string.Empty; // Clear the title if there's an error
        }
    }

    private void HomeButton_Clicked(object sender, EventArgs e)
    {
        LoadHomePage();
    }
    private void ReloadCurrentPage()
    {
        if (!string.IsNullOrWhiteSpace(currentUrl))
        {
            addressEntry.Text = currentUrl; // Set the address entry to the current URL
            NavigateButton_Clicked(this, EventArgs.Empty); // Trigger a navigation to the current URL
        }
    }

    // Helper method to load the home page URL
    private async void LoadHomePage()
    {
        try
        {
            homePageUrl = File.ReadLines(homePath).FirstOrDefault();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(homePageUrl);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Display the HTML code in the TextView
                    contentTextView.Buffer.Text = content;

                    // Display the HTTP response status code
                    string statusCode = $"HTTP Status Code: {(int)response.StatusCode} {response.ReasonPhrase}";
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
                else
                {
                    contentTextView.Buffer.Text = $"HTTP Error {response.StatusCode}: {content}";
                    titleLabel.Text = string.Empty; // Clear the title if there's an error
                }
            }
        }
        catch (HttpRequestException ex)
        {
            contentTextView.Buffer.Text = $"Request Error: {ex.Message}";
            titleLabel.Text = string.Empty; // Clear the title if there's an error
        }
    }

    // Helper method to parse the title of a web page
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
        if (File.ReadLines(app.homePath).FirstOrDefault()!=null){
            app.LoadHomePage();
        }
        app.ShowAll();
        Application.Run();
    }
}
