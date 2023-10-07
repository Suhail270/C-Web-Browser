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
    private LinkedList<string> localHistoryBack;
    private LinkedList<string> localHistoryForward;
    TextIter startIter;
    TextIter endIter;
    private Grid buttonsGrid;
    private Button addButton;
    private Button editButton;
    private Button deleteButton;
    private bool isFavoritesPage;
    private Entry favouritesEntry;
    private Entry favouritesTitleEntry;
    private Button addOkButton;
    private Button editOkButton;
    private Button deleteOkButton;
    private string[] favourites;
    private string selectedText;
    private string navigateText;
    private string titleText;
    private bool editMode;
    private int favouritesIndex;
    private bool valid;
    private Button confirmButton;
    private Button cancelButton;
    private List<string> deleteFavouritesList;
    private bool dialogDisplayed;
    private Button editHomeButton;
    private Entry homeEntry;
    private Button editHomeOkButton;
    private List<string> newHomePage;
    private string[] historyText;
    private Button clearHistoryButton;

    private Dictionary<TextTag, string> tagToUrlMap;

    
    public WebBrowserApp() : base("F20SC - CW1")
    {
        SetDefaultSize(900, 700);
        SetPosition(WindowPosition.Center);

        var mainVBox = new VBox();
        var mainHBox = new HBox();
        var titleHBox = new HBox();

        addressEntry = new Entry();
        addressEntry.WidthChars = 50;

        homeEntry = new Entry();
        homeEntry.WidthChars = 50;

        localHistoryBack = new LinkedList<string>();
        localHistoryForward = new LinkedList<string>();

        navigateButton = new Button("Go");
        homeButton = new Button("Home");
        backButton = new Button("<");
        forwardButton = new Button(">");
        favouritesButton = new Button("Favourites");
        contentTextView = new TextView();
        scrolledWindow = new ScrolledWindow();
        titleLabel = new Label();
        historyButton = new Button("History"); // Create the history button
        editHomeButton = new Button("Edit Home");
        clearHistoryButton = new Button("Clear History");
        
        historyButton.Clicked += HistoryButton_ClickedAsync; // Attach an event handler
        navigateButton.Clicked += NavigateButton_Clicked;
        homeButton.Clicked += HomeButton_Clicked;
        backButton.Clicked += BackButton_Clicked;
        forwardButton.Clicked += ForwardButton_Clicked;
        favouritesButton.Clicked += FavouritesButton_Clicked;
        editHomeButton.Clicked += EditHomeButton_Clicked;
        clearHistoryButton.Clicked += ClearHistoryButton_Clicked;

        mainVBox.PackStart(mainHBox, false, false, 0);
        mainVBox.PackStart(titleHBox, false, false, 0);
        // mainVBox.PackStart(clearHistoryButton, false, false, 0);
        
        favouritesTitleEntry = new Entry();
        favouritesTitleEntry.PlaceholderText = "Enter title";
        favouritesTitleEntry.Visible = false;
        favouritesTitleEntry.WidthChars = 50;
        mainVBox.PackStart(favouritesTitleEntry, false, false, 10);

        favouritesEntry = new Entry();
        favouritesEntry.PlaceholderText = "Enter URL";
        favouritesEntry.Visible = false;
        favouritesEntry.WidthChars = 50;
        mainVBox.PackStart(favouritesEntry, false, false, 10);
        
        addOkButton = new Button("OK");
        mainVBox.PackStart(addOkButton, false, false, 10);

        editOkButton = new Button("OK");
        mainVBox.PackStart(editOkButton, false, false, 10);

        deleteOkButton = new Button("OK");
        mainVBox.PackStart(deleteOkButton, false, false, 10);

        editHomeOkButton = new Button("OK");
        mainVBox.PackStart(editHomeOkButton, false, false, 10);

        mainHBox.PackStart(backButton, false, false, 0);
        mainHBox.PackStart(forwardButton, false, false, 10);
        mainHBox.PackStart(addressEntry, false, false, 10);
        mainHBox.PackStart(navigateButton, false, false, 10);
        mainHBox.PackStart(homeButton, false, false, 10);
        mainHBox.PackStart(editHomeButton, false, false, 10);
        mainHBox.PackStart(historyButton, false, false, 10);
        mainHBox.PackStart(favouritesButton, false, false, 10);

        titleHBox.PackStart(titleLabel, false, false, 10);

        titleHBox.PackEnd(clearHistoryButton, false, false, 10);

        var textBuffer = new TextBuffer(null);
        contentTextView.Buffer = textBuffer;
        contentTextView.Editable = false;
        contentTextView.WrapMode = WrapMode.WordChar;
        scrolledWindow.Add(contentTextView);
        mainVBox.PackStart(homeEntry, false, false, 10);
        mainVBox.PackStart(scrolledWindow, true, true, 0);

        // Create a grid for the buttons
        buttonsGrid = new Grid();
        buttonsGrid.ColumnSpacing = 5;
        buttonsGrid.RowSpacing = 5;

        // Create buttons for add, edit, and delete
        addButton = new Button("Add Favorite");
        editButton = new Button("Edit Favorite");
        deleteButton = new Button("Delete Favorite");

        // Attach buttons to the grid
        buttonsGrid.Attach(addButton, 0, 0, 1, 1);
        buttonsGrid.Attach(editButton, 1, 0, 1, 1);
        buttonsGrid.Attach(deleteButton, 2, 0, 1, 1);

        // Set their visibility to false initially
        addButton.Visible = false;
        editButton.Visible = false;
        deleteButton.Visible = false;

        // favouritesEntry.Visible = false;
        addOkButton.Visible = false;
        editOkButton.Visible = false;
        deleteOkButton.Visible = false;

        var alignment = new Alignment(0.5f, 0.0f, 0.0f, 0.0f);
        alignment.Add(buttonsGrid);
        mainVBox.PackStart(alignment, false, false, 0);
        // Handle button click events (add your logic)
        addButton.Clicked += AddButton_Clicked;
        // editButton.Clicked += EditButton_Clicked;
        // deleteButton.Clicked += DeleteButton_Clicked;

        // Add the buttons grid to the bottom right corner
        scrolledWindow.AddWithViewport(contentTextView);
        
        mainVBox.PackStart(buttonsGrid, false, false, 0);


        Add(mainVBox);

        DeleteEvent += (sender, args) => Application.Quit();

        contentTextView.Buffer.Text = "Enter a URL and click 'Go' to view HTML code.";
    }

    private async void HideButtons(){
        isFavoritesPage = false;
        clearHistoryButton.Visible = false;
        addButton.Visible = false;
        editButton.Visible = false;
        deleteButton.Visible = false;
        favouritesEntry.Visible = false;
        favouritesTitleEntry.Visible = false;
        addOkButton.Visible = false;
        editOkButton.Visible = false;
        deleteOkButton.Visible = false;
        homeEntry.Visible = false;
        editHomeOkButton.Visible = false;
    }

    private async void ClearHistoryButton_Clicked(object sender, EventArgs e){
        string[] history = await ReadHistoryAsync();

        if (history.Length==0){
            ShowMessage("History is already empty.");
        }

        else{

            if(!dialogDisplayed){
                        using (var dialog = new MessageDialog(
                        this,
                        DialogFlags.Modal,
                        MessageType.Question,
                        ButtonsType.YesNo,
                        $"Are you sure you want to clear your history? This is a non-reversible action.")
                    )
                    {
                        // Configure the dialog
                        dialog.Title = "Clear History";
                        dialog.WindowPosition = WindowPosition.Center;

                        dialogDisplayed = true;

                        // Console.Write("\nBefore deleting: ");
                        // Console.Write(favourites.Length);

                        // Show the dialog and get the response
                        ResponseType response = (ResponseType)dialog.Run();

                        // Handle the dialog response
                        if (response == ResponseType.Yes)
                        {
                            File.WriteAllText(historyPath, string.Empty);
                            dialog.Hide();
                            dialogDisplayed = false;      
                        }
                        else{
                            dialog.Hide();
                            dialogDisplayed = false;
                            return;
                        }  
                        }
                    contentTextView.Buffer.Text = "History is empty.";
                    ShowMessage("History cleared sucessfully.");
                };

        }


        }

    private async void AddButton_Clicked(object sender, EventArgs e){
        favourites = await File.ReadAllLinesAsync(favouritesPath);

        if (!string.IsNullOrWhiteSpace(favouritesEntry.Text) && Uri.IsWellFormedUriString(favouritesEntry.Text, UriKind.Absolute)){
            
            if (favourites.Any(f => f == favouritesEntry.Text))
            {
                ShowMessage("This URL already exists in favorites.");
                return;
            }
        
        File.AppendAllText(favouritesPath, favouritesEntry.Text+"\n");
        }

        }

    private async Task<bool> Validation(string url){
        try
        {
            if(Uri.IsWellFormedUriString(url, UriKind.Absolute)){
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    return true;
                }
            }
            return false;
        }
        catch (HttpRequestException ex)
        {
            return false;
        }
    }

//    private async void ApplyingHyperlinkTags()
// {
//     titleLabel.Text = "Favourites";
//     favourites = await ReadFavouritesAsync();
//     string favouritesText = string.Join("\n", favourites);

//     if (favouritesText.Length == 0)
//     {
//         contentTextView.Buffer.Text = "No favourites added.";
//         return;
//     }

//     TextBuffer buffer = new TextBuffer(null);
//     contentTextView.Buffer = buffer; // Set the new TextBuffer to contentTextView

//     int startOffset = 0;

//     foreach (var line in favourites)
//     {
//         int endOffset = startOffset + line.Length;
//         TextIter iter = buffer.GetIterAtOffset(startOffset);

//         // Create a new TextTag for each URL
//         TextTag hyperlinkTag = new TextTag("hyperlink");
//         hyperlinkTag.Underline = Pango.Underline.Single;

//         // Store the URL as user data with the tag
//         hyperlinkTag.Data = line.Split(',')[1].Trim(); // Assuming URLs are comma-separated

//         // Apply the "hyperlink" tag to the text portion
//         buffer.ApplyTag(hyperlinkTag, iter, buffer.GetIterAtOffset(endOffset));

//         startOffset = endOffset + 1;
//     }
// }

private async void ApplyingHyperlinkTags(string page){
        
        if(page=="favourites"){
            titleLabel.Text = "Favourites";
            favourites = await ReadFavouritesAsync();
        }
        else{
            titleLabel.Text = "History";
            favourites = await ReadHistoryAsync();
        }
        string favouritesText = string.Join("\n", favourites);

        if (favouritesText.Length==0){
            contentTextView.Buffer.Text = "No favourites added.";
            return;
        }
            contentTextView.Buffer.Text = favouritesText;

            var hyperlinkTag = contentTextView.Buffer.TagTable.Lookup("hyperlink");

            // If it doesn't exist, create a new "hyperlink" tag
            if (hyperlinkTag == null)
            {
                hyperlinkTag = new TextTag("hyperlink");
            }

            hyperlinkTag.Underline = Pango.Underline.Single;
            contentTextView.Buffer.TagTable.Add(hyperlinkTag);

            // Apply the hyperlink tag to specific text portions
            // var buffer = contentTextView.Buffer;
            int startOffset = 0;
            foreach (var line in favourites)
            {
                int endOffset = startOffset + line.Length;
                contentTextView.Buffer.ApplyTag(hyperlinkTag, contentTextView.Buffer.GetIterAtOffset(startOffset), contentTextView.Buffer.GetIterAtOffset(endOffset));
                startOffset = endOffset + 1;
            }

    }

    private async void FavouritesButton_Clicked(object sender, EventArgs e)
{   
    HideButtons();
    favouritesEntry.Text = string.Empty;
    favouritesTitleEntry.Text = string.Empty;
    try
    {
        isFavoritesPage = true; // Set the flag to true when on the favorites page

        // Show the add/edit/delete buttons
        addButton.Visible = true;
        editButton.Visible = true;
        deleteButton.Visible = true;
        

        // if (favourites.Length > 0)
        // {
        ApplyingHyperlinkTags("favourites");

            addButton.Clicked += (sender, args) =>
        {
            
            // Show the Entry widget to take user input
            favouritesEntry.Visible = true;
            favouritesTitleEntry.Visible = true;
            addOkButton.Visible = true;
            titleLabel.Text = "Add Favourites";

            // favourites = File.ReadAllLines(favouritesPath);

            addOkButton.Clicked += async (okSender, okArgs) =>
            {

                if (string.IsNullOrWhiteSpace(favouritesEntry.Text)){
                    ShowMessage("Please enter the URL.");
                }

                else if (string.IsNullOrWhiteSpace(favouritesTitleEntry.Text)){
                    ShowMessage("Please enter the title for this URL.");
                }

                else if (!Uri.IsWellFormedUriString(favouritesEntry.Text, UriKind.Absolute)){
                    
                    ShowMessage("Invalid URL.");
                    favouritesEntry.Text = string.Empty;
                    favouritesTitleEntry.Text = string.Empty;
                }
                
                else{
                    if (favourites.Any(f => f == favouritesEntry.Text))
                    {
                        ShowMessage("This URL already exists in favorites.");
                        return;
                    }
                
                    File.AppendAllText(favouritesPath, favouritesTitleEntry.Text + ": " + favouritesEntry.Text+"\n");
                    favouritesEntry.Visible = false;
                    favouritesTitleEntry.Visible = false;
                    addOkButton.Visible = false;
                    // favourites = await ReadFavouritesAsync();
                    favouritesEntry.Text = string.Empty;
                    favouritesTitleEntry.Text = string.Empty;
                    ApplyingHyperlinkTags("favourites");
                }

                
                };
                
        };

        editButton.Clicked += async (s, args) =>
        {
            editMode = true;

            favouritesEntry.Text = string.Empty;
            favouritesEntry.Visible = true;

            favouritesTitleEntry.Text = string.Empty;
            favouritesTitleEntry.Visible = true;

            editOkButton.Visible = true;

            titleLabel.Text = "Edit Favourites";
            // favourites = File.ReadAllLines(favouritesPath);
            ShowMessage("Please select the URL to edit.");
            
            // if (string.IsNullOrWhiteSpace(selectedText))
            // {
            //     // ShowMessage("Please select a URL to edit.");
            // }
            // else
            // {
                // Console.WriteLine("TEXT: ", selectedText);
                navigateText = "";
                titleText = "";
                // favouritesEntry.Text = selectedText; // Populate the Entry with the selected hyperlink
                editOkButton.Clicked += async (okSender, okArgs) =>
                {
                    if (!string.IsNullOrWhiteSpace(favouritesEntry.Text))
                    {
                        // ShowMessage("Entry bar is empty.");
                        valid = await Validation(favouritesEntry.Text);
                    
                        if (!valid)
                        {
                            ShowMessage("Invalid URL.");
                            favouritesEntry.Text = string.Empty;
                            favouritesTitleEntry.Text = string.Empty;
                        }
                        else
                        {
                            // Find and update the selected hyperlink in the 'favourites' array
                            
                            // if (selectedIndex >= 0)
                            // {
                                favourites[favouritesIndex] = favouritesTitleEntry.Text + ": " + favouritesEntry.Text;
                                File.WriteAllLines(favouritesPath, favourites);
                                // favouritesEntry.Text = string.Empty;
                                favouritesEntry.Visible = false;
                                favouritesTitleEntry.Visible = false;
                                editOkButton.Visible = false;
                                editMode = false;
                                // favourites = await ReadFavouritesAsync();
                                ApplyingHyperlinkTags("favourites");
                            // }
                        }
                    }
                    
                };
            // }
            favouritesEntry.Text = string.Empty;
        };

        // editButton.Clicked += async (s, args) =>
        // {
        //     editMode = true;
        //     favouritesEntry.Text = string.Empty;
        //     favouritesTitleEntry.Text = string.Empty;
        //     favouritesEntry.Visible = true;
        //     favouritesTitleEntry.Visible = true;
        //     editOkButton.Visible = true;
        //     titleLabel.Text = "Edit Favourites";
        //     ShowMessage("Please select the URL to edit.");
            
        //         // navigateText = "";
        //         // titleText = "";

        //         editOkButton.Clicked += async (okSender, okArgs) =>
        //         {
        //             if (!string.IsNullOrWhiteSpace(favouritesEntry.Text))
        //             {
        //                 valid = await Validation(favouritesEntry.Text);
                    
        //                 if (!valid)
        //                 {
        //                     ShowMessage("Invalid URL.");
        //                     favouritesEntry.Text = string.Empty;
        //                     favouritesTitleEntry.Text = string.Empty;

        //                 }
        //                 else
        //                 {       
        //                     try{
        //                         favourites[favouritesIndex] = favouritesTitleEntry.Text + ": " + favouritesEntry.Text;
        //                         Console.WriteLine("TITLE: " + favouritesTitleEntry.Text);
        //                         Console.WriteLine("ENTRY: " + favouritesEntry.Text);
        //                         File.WriteAllLines(favouritesPath, favourites);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         throw new Exception($"Exception: {ex.Message}");
        //                     }
        //                         favouritesEntry.Visible = false;
        //                         editOkButton.Visible = false;
        //                         editMode = false;
        //                         favouritesEntry.Text = string.Empty;
        //                         favouritesTitleEntry.Text = string.Empty;
        //                         ApplyingHyperlinkTags("favourites");
        //                 }
        //             }
                    
        //         };
            
        // };

        deleteButton.Clicked += (s, args) =>
        {
            editMode = true;
            favouritesEntry.Text = string.Empty;
            favouritesTitleEntry.Text = string.Empty;
            favouritesEntry.Visible = true;
            deleteOkButton.Visible = true;
            titleLabel.Text = "Delete Favourites";
            dialogDisplayed = false;
            // favourites = File.ReadAllLines(favouritesPath);
            ShowMessage("Please select the URL to delete.");
            
            // if (string.IsNullOrWhiteSpace(selectedText))
            // {
            //     // ShowMessage("Please select a URL to edit.");
            // }
            // else
            // {
                // Console.WriteLine("TEXT: ", selectedText);
                // favouritesEntry.Text = selectedText; // Populate the Entry with the selected hyperlink

                deleteOkButton.Clicked += async (okSender, okArgs) =>
                {
                    if(!dialogDisplayed){
                        using (var dialog = new MessageDialog(
                        this,
                        DialogFlags.Modal,
                        MessageType.Question,
                        ButtonsType.YesNo,
                        $"Are you sure you want to delete {selectedText} from your favourites list?")
                    )
                    {
                        // Configure the dialog
                        dialog.Title = "Delete URL";
                        dialog.WindowPosition = WindowPosition.Center;

                        dialogDisplayed = true;

                        // Console.Write("\nBefore deleting: ");
                        // Console.Write(favourites.Length);

                        // Show the dialog and get the response
                        ResponseType response = (ResponseType)dialog.Run();

                        // Handle the dialog response
                        if (response == ResponseType.Yes)
                        {
                            // User clicked "OK," implement your edit logic here
                            // Console.WriteLine("OK clicked");
                            // favourites.RemoveAt(favouritesIndex);
                            // Console.Write("\n\nINDEX: ");
                            // Console.Write(favouritesIndex);

                            deleteFavouritesList = new List<string>(favourites);
                            deleteFavouritesList.RemoveAt(favouritesIndex);

                            favourites = deleteFavouritesList.ToArray();
                            // Console.Write("\nAfter deleting: ");
                            // Console.Write(favourites.Length);
                            // Console.WriteLine("\nELEMENTS:");
                            // foreach(var month in favourites)
                            // {
                            //     Console.WriteLine(month);
                            // }
                            File.WriteAllLines(favouritesPath, favourites);
                            // favouritesEntry.Text = string.Empty;
                            favouritesEntry.Visible = false;
                            favouritesTitleEntry.Visible = false;
                            deleteOkButton.Visible = false;
                            editMode = false;
                            dialog.Hide();
                            // favourites = await ReadFavouritesAsync();
                            ApplyingHyperlinkTags("favourites");
                        }
                        else{
                            dialog.Hide();
                            return;
                        }  
                        }
                };
                favouritesEntry.Text = string.Empty;
                favouritesTitleEntry.Text = string.Empty;
            };
        };

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
                                while (start >= 0 && contentTextView.Buffer.GetIterAtOffset(start).HasTag(tag))
                                {
                                    start--;
                                }

                                while (end < contentTextView.Buffer.Text.Length && contentTextView.Buffer.GetIterAtOffset(end).HasTag(tag))
                                {
                                    end++;
                                }

                                // Extract the URL from the clicked link
                                selectedText = contentTextView.Buffer.GetText(contentTextView.Buffer.GetIterAtOffset(start + 1), contentTextView.Buffer.GetIterAtOffset(end), false);
                                navigateText = "";
                                
                                for(int i = 0; i<selectedText.Length; i++){
                                    if(selectedText[i]==':'){
                                        navigateText = selectedText.Substring(i+2, selectedText.Length - (i + 2));
                                        // Console.Write(selectedText[i]);
                                        if (editMode==true){
                                            favouritesTitleEntry.Text = selectedText.Substring(0, i);
                                            favouritesEntry.Text = selectedText.Substring(i+2, selectedText.Length - (i + 2));
                                            favouritesIndex = Array.IndexOf(favourites, selectedText);
                                        }
                                        break;
                                    }
                                } 
                                
                                if(editMode==false){
                                    Console.Write(navigateText + "test");
                                DisplayWebContent(navigateText, "nav");
                                }
                            }
                        }
                    }
                }
            };
        // }
        // else
        // {
        //     contentTextView.Buffer.Text = "Favourites is empty.";
        //     titleLabel.Text = string.Empty;
        // }
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
        HideButtons();
        if (localHistoryBack.Count==0){
            ShowMessage("No next URL.");
        }
        else{
            DisplayWebContent(localHistoryBack.Last.Value, "back");
        }
    }

    private async void ForwardButton_Clicked(object sender, EventArgs e)
     {
        HideButtons();

        if (localHistoryForward.Count==0){
            ShowMessage("No next URL.");
        }
        else{
            DisplayWebContent(localHistoryForward.Last.Value, "next");
        }
    }

    private async void HistoryButton_ClickedAsync(object sender, EventArgs e)
    {
        try
        {
            HideButtons();
            clearHistoryButton.Visible = true;
            string[] history = await ReadHistoryAsync();

            if (history.Length > 0)
            {
                string historyText = string.Join("\n", history);
                ApplyingHyperlinkTags("history");
                
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
                                while (start >= 0 && contentTextView.Buffer.GetIterAtOffset(start).HasTag(tag))
                                {
                                    start--;
                                }

                                while (end < contentTextView.Buffer.Text.Length && contentTextView.Buffer.GetIterAtOffset(end).HasTag(tag))
                                {
                                    end++;
                                }

                                // Extract the URL from the clicked link
                                selectedText = contentTextView.Buffer.GetText(contentTextView.Buffer.GetIterAtOffset(start + 1), contentTextView.Buffer.GetIterAtOffset(end), false);
                                // navigateText = "";
                                
                                // Console.Write(selectedText);
                                
                                DisplayWebContent(selectedText, "nav");
                                
                            }
                        }
                    }
                }
            };
                
            }
            else
            {
                titleLabel.Text = "History";
                contentTextView.Buffer.Text = "History is empty.";
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
            historyText = await File.ReadAllLinesAsync(historyPath);
            Array.Reverse(historyText);
        }

        else{
            File.Create(historyPath).Close();
        }

        return historyText;
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
        HideButtons();
        LoadHomePage();
    }

    private void EditHomeButton_Clicked(object sender, EventArgs e){
        
        HideButtons();
        favouritesEntry.Text = string.Empty;
        favouritesTitleEntry.Text = string.Empty;
        favouritesEntry.Visible = true;
        editHomeOkButton.Visible = true;
        contentTextView.Buffer.Text = "Enter the new home URL in the entry bar above.";
        editHomeOkButton.Clicked += async (okSender, okArgs) =>
        {
            if (!string.IsNullOrWhiteSpace(favouritesEntry.Text))
                    {
                        // ShowMessage("Entry bar is empty.");
                        valid = await Validation(favouritesEntry.Text);
                    
                        if (!valid)
                        {
                            ShowMessage("Invalid URL.");
                            favouritesEntry.Text = string.Empty;
                            favouritesTitleEntry.Text = string.Empty;
                            // favouritesEntry.Text = string.Empty;
                        }
                        else
                        {
                            // Find and update the selected hyperlink in the 'favourites' array
                            
                            // if (selectedIndex >= 0)
                            // {
                                // favourites[favouritesIndex] = favouritesEntry.Text;
                                // File.WriteAllLines(homePath, favouritesEntry.Text);
                                // favouritesEntry.Text = string.Empty;
                                // Console.WriteLine("TEXT:");
                                // Console.WriteLine(favouritesEntry.Text);
                                newHomePage = new List<string>();
                                newHomePage.Add(favouritesEntry.Text);
                                File.WriteAllLines(homePath, newHomePage);
                                contentTextView.Buffer.Text = string.Empty;
                                favouritesEntry.Visible = false;
                                editHomeOkButton.Visible = false;
                                ShowMessage("Home page updated, you have been redirected to your new home page.");
                                LoadHomePage();
                                // favourites = await ReadFavouritesAsync();
                            // }
                        }
                    }
        };


    }

    private async void DisplayWebContent(string url, string action){

        HideButtons();

        // if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        // {
        //     ShowMessage("Invalid URL.");
        //     return;
        // }

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
                    if (action=="nav")
                    {
                        localHistoryBack.AddLast(currentUrl);
                        localHistoryForward.Clear();
                    }

                    else if (action=="next")
                    {
                        localHistoryBack.AddLast(currentUrl);
                        localHistoryForward.RemoveLast();
                        localHistoryBack.AddLast(currentUrl);
                    }

                    else if (action=="back"){
                        localHistoryBack.RemoveLast();
                        localHistoryForward.AddLast(currentUrl);
                    }

                    else if (action=="home"){
                        if(!startUp){
                            localHistoryBack.AddLast(currentUrl);
                        }
                    }

                    else if (action=="reload"){

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
        app.LoadHomePage();
        app.ShowAll();
        Application.Run();
    }
}