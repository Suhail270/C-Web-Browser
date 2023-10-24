using Gtk;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gdk;

namespace MainApp{

    // public class WebBrowserApp : Application
    // {
    //     public WebBrowserApp(string applicationId, GLib.ApplicationFlags flags)
    //         : base(applicationId, flags)
    //     {
    //     }

    //     protected override void OnStartup()
    //     {
    //         base.OnStartup();

    //         // Your test logic here
    //         // For example:
    //         string testResult = GetTextFromEntry();
    //         // Do something with the result

    //         // Exit the application
    //         Quit();
    //     }
    // }
    public class WebBrowserApp : Gtk.Window
    {
        public string statusCode;
        public event EventHandler<string> AddressEntryTextChanged;
        public Entry addressEntry;
        public Button navigateButton;
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
        private Button reloadButton;
        private Button bulkDownloadButton;
        private Entry bulkDownloadEntry;
        private Button downloadOkButton;
        private string bulkDownloadPath;
        private string[] downloadLines;
        private string bulkDownloadResult;
        private EventHandler downloadButtonClicked;
        private bool controlButton;
        private bool leftArrow;
        private bool rightArrow;
        private bool letterH;
        private bool letterF;
        private AccelGroup accelGroup;
        private Button profileButton;
        private Entry nameEntry;
        private Entry idEntry;
        private Entry emailEntry;
        private Entry phoneEntry;
        private string profilePath = "profile.txt";
        private string[] profileText;
        private Button editProfileButton;
        private Button editProfileOkButton;
        private Grid buttonsGridProfile;
        private Button addButtonProfile;
        private Button editButtonProfile;
        private Button deleteButtonProfile;
        private ComboBox comboBox;
        ListStore listStore;
        CellRendererText cellRendererText;
        private Button addProfileOkButton;
        private Button deleteProfileOkButton;
        private List<string> deleteProfileList;
        private List<string> editProfileList;
        int editIndex;
        private string textFromAddressEntry;
        public WebBrowserApp() : base("F20SC - CW1")
        {
            SetDefaultSize(900, 700);
            SetPosition(WindowPosition.Center);

            var mainVBox = new VBox();
            var mainHBox = new HBox();
            var titleHBox = new HBox();

            addressEntry = new Entry();
            addressEntry.WidthChars = 50;

            comboBox = new ComboBox();
            listStore = new ListStore(typeof(string));
            cellRendererText = new CellRendererText();

            comboBox.PackStart(cellRendererText, true);
            comboBox.AddAttribute(cellRendererText, "text", 0);

            homeEntry = new Entry();
            homeEntry.WidthChars = 50;

            bulkDownloadEntry = new Entry();
            bulkDownloadEntry.WidthChars = 50;

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
            historyButton = new Button("History"); 
            editHomeButton = new Button("Edit Home");
            clearHistoryButton = new Button("Clear History");
            reloadButton = new Button("↻");
            profileButton = new Button("Profile");
            bulkDownloadButton = new Button("Bulk Download");
            editProfileButton = new Button("Edit Profile");
            
            historyButton.Clicked += HistoryButton_ClickedAsync;
            navigateButton.Clicked += NavigateButton_Clicked;
            homeButton.Clicked += HomeButton_Clicked;
            backButton.Clicked += BackButton_Clicked;
            forwardButton.Clicked += ForwardButton_Clicked;
            favouritesButton.Clicked += FavouritesButton_Clicked;
            editHomeButton.Clicked += EditHomeButton_Clicked;
            clearHistoryButton.Clicked += ClearHistoryButton_Clicked;
            reloadButton.Clicked += ReloadButton_Clicked;
            bulkDownloadButton.Clicked += BulkDownloadButton_Clicked;
            profileButton.Clicked += ProfileButton_Clicked;

            addressEntry.Changed += OnAddressEntryChanged;

            mainVBox.PackStart(mainHBox, false, false, 0);
            mainVBox.PackStart(titleHBox, false, false, 0);
        
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

            bulkDownloadEntry = new Entry();
            bulkDownloadEntry.PlaceholderText = "Leave empty for default file: 'bulk.txt'";
            bulkDownloadEntry.Visible = false;
            bulkDownloadEntry.WidthChars = 50;
            mainVBox.PackStart(bulkDownloadEntry, false, false, 10);
            
            mainVBox.PackStart(comboBox, false, false, 10);

            nameEntry = new Entry();
            nameEntry.PlaceholderText = "Enter name";
            nameEntry.Visible = false;
            nameEntry.WidthChars = 50;
            mainVBox.PackStart(nameEntry, false, false, 10);

            idEntry = new Entry();
            idEntry.PlaceholderText = "Enter id";
            idEntry.Visible = false;
            idEntry.WidthChars = 50;
            mainVBox.PackStart(idEntry, false, false, 10);

            emailEntry = new Entry();
            emailEntry.PlaceholderText = "Enter email";
            emailEntry.Visible = false;
            emailEntry.WidthChars = 50;
            mainVBox.PackStart(emailEntry, false, false, 10);

            phoneEntry = new Entry();
            phoneEntry.PlaceholderText = "Enter phone number";
            phoneEntry.Visible = false;
            phoneEntry.WidthChars = 50;
            mainVBox.PackStart(phoneEntry, false, false, 10);

            addProfileOkButton = new Button("OK");
            mainVBox.PackStart(addProfileOkButton, false, false, 10);

            editProfileOkButton = new Button("OK");
            mainVBox.PackStart(editProfileOkButton, false, false, 10);

            deleteProfileOkButton = new Button("OK");
            mainVBox.PackStart(deleteProfileOkButton, false, false, 10);
            
            downloadOkButton = new Button("OK");
            mainVBox.PackStart(downloadOkButton, false, false, 10);

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
            mainHBox.PackStart(profileButton, false, false, 10);
            mainHBox.PackStart(reloadButton, false, false, 10);
            mainHBox.PackStart(addressEntry, false, false, 10);
            mainHBox.PackStart(navigateButton, false, false, 10);
            mainHBox.PackStart(homeButton, false, false, 10);
            mainHBox.PackStart(editHomeButton, false, false, 10);
            mainHBox.PackStart(historyButton, false, false, 10);
            mainHBox.PackStart(favouritesButton, false, false, 10);
            mainHBox.PackStart(bulkDownloadButton, false, false, 10);

            titleHBox.PackStart(titleLabel, false, false, 10);

            titleHBox.PackEnd(clearHistoryButton, false, false, 10);

            var textBuffer = new TextBuffer(null);
            contentTextView.Buffer = textBuffer;
            contentTextView.Editable = false;
            contentTextView.WrapMode = WrapMode.WordChar;
            scrolledWindow.Add(contentTextView);
            mainVBox.PackStart(homeEntry, false, false, 10);
            mainVBox.PackStart(scrolledWindow, true, true, 0);

            buttonsGridProfile = new Grid();
            buttonsGridProfile.ColumnSpacing = 5;
            buttonsGridProfile.RowSpacing = 5;

            addButtonProfile = new Button("Add Profile");
            editButtonProfile = new Button("Edit Profile");
            deleteButtonProfile = new Button("Delete Profile");

            buttonsGridProfile.Attach(addButtonProfile, 0, 0, 1, 1);
            buttonsGridProfile.Attach(editProfileButton, 1, 0, 1, 1);
            buttonsGridProfile.Attach(deleteButtonProfile, 2, 0, 1, 1);

            var alignmentProfile = new Alignment(0.5f, 0.0f, 0.0f, 0.0f);
            alignmentProfile.Add(buttonsGridProfile);
            mainVBox.PackStart(alignmentProfile, false, false, 0);

            buttonsGrid = new Grid();
            buttonsGrid.ColumnSpacing = 5;
            buttonsGrid.RowSpacing = 5;

            addButton = new Button("Add Favorite");
            editButton = new Button("Edit Favorite");
            deleteButton = new Button("Delete Favorite");

            buttonsGrid.Attach(addButton, 0, 0, 1, 1);
            buttonsGrid.Attach(editButton, 1, 0, 1, 1);
            buttonsGrid.Attach(deleteButton, 2, 0, 1, 1);

            addButton.Visible = false;
            editButton.Visible = false;
            deleteButton.Visible = false;

            addOkButton.Visible = false;
            editOkButton.Visible = false;
            deleteOkButton.Visible = false;
            editProfileOkButton.Visible = false;

            buttonsGrid = new Grid();
            buttonsGrid.ColumnSpacing = 5;
            buttonsGrid.RowSpacing = 5;

            var alignment = new Alignment(0.5f, 0.0f, 0.0f, 0.0f);
            alignment.Add(buttonsGrid);
            mainVBox.PackStart(alignment, false, false, 0);

            addButton.Clicked += AddButton_Clicked;

            scrolledWindow.AddWithViewport(contentTextView);
            
            mainVBox.PackStart(buttonsGrid, false, false, 0);

            EnterKeyClicked(addressEntry, NavigateButton_Clicked, "nav");
            EnterKeyClicked(bulkDownloadEntry, DownloadButton_Clicked, "bulkDownload");

            this.KeyPressEvent += MainWindow_KeyPressEvent;

            Add(mainVBox);

            DeleteEvent += (sender, args) => Application.Quit();

            contentTextView.Buffer.Text = "Enter a URL and click 'Go' to view HTML code.";
        }

        
        private void MainWindow_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.r))
            {
                ReloadButton_Clicked(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.h))
            {
                HomeButton_Clicked(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.H))
            {
                HistoryButton_ClickedAsync(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.f))
            {
                FavouritesButton_Clicked(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.d))
            {
                BulkDownloadButton_Clicked(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.z))
            {
                BackButton_Clicked(null, null);
            }

            else if (args.Event.State.HasFlag(ModifierType.ControlMask) && (args.Event.Key == Gdk.Key.x))
            {
                ForwardButton_Clicked(null, null);
            }

            else{
                return;
            }
        }
        private void EnterKeyClicked(Entry entry, EventHandler eventHandler, string action)
        {
            entry.KeyPressEvent += (o, args) =>
            {
                if (args.Event.KeyValue == 65293)
                {
                    eventHandler(o, EventArgs.Empty);
                }
            };
        }
        public async void HideButtons(){
            isFavoritesPage = false;
            clearHistoryButton.Visible = false;
            addButton.Visible = false;
            editButton.Visible = false;
            deleteButton.Visible = false;
            addButtonProfile.Visible = false;
            editButtonProfile.Visible = false;
            deleteButtonProfile.Visible = false;
            favouritesEntry.Visible = false;
            favouritesTitleEntry.Visible = false;
            addOkButton.Visible = false;
            editOkButton.Visible = false;
            deleteOkButton.Visible = false;
            homeEntry.Visible = false;
            editHomeOkButton.Visible = false;
            bulkDownloadEntry.Visible = false;
            downloadOkButton.Visible = false;
            downloadOkButton.Visible = false;
            nameEntry.Visible = false;
            idEntry.Visible = false;
            emailEntry.Visible = false;
            phoneEntry.Visible = false;
            editProfileButton.Visible = false;
            addProfileOkButton.Visible = false;
            editProfileOkButton.Visible = false;
            deleteProfileOkButton.Visible = false;
            comboBox.Visible = false;
        }

        private async Task<string[]> ReadProfileAsync()
        {
            if (File.Exists(profilePath))
            {
                profileText = await File.ReadAllLinesAsync(profilePath);
            }
        
            return profileText;
        }
        private async void ProfileButton_Clicked(object sender, EventArgs e){
        
        HideButtons();

        titleLabel.Text = "Profile";

        contentTextView.Buffer.Text = string.Empty;
        profileText = await ReadProfileAsync();

        if(profileText.Length==0){
            contentTextView.Buffer.Text = "No profile added.";
            return;
        }

        else{
            int num_profiles = profileText.Length/4;

            listStore.Clear();

            for(int i = 0; i<profileText.Length; i+=4){
                listStore.AppendValues(profileText[i]);
            }
        }

        comboBox.Model = listStore;
        comboBox.Visible = true;

        addButtonProfile.Visible = true;
        editButtonProfile.Visible = true;
        deleteButtonProfile.Visible = true;

        addButtonProfile.Clicked += async (s, args) =>
        {
            nameEntry.Text = string.Empty;
            idEntry.Text = string.Empty;
            emailEntry.Text = string.Empty;
            phoneEntry.Text = string.Empty;

            nameEntry.Editable = true;
            idEntry.Editable = true;
            emailEntry.Editable = true;
            phoneEntry.Editable = true;

            titleLabel.Text = "Add Profile";

            addProfileOkButton.Visible = true;

            editProfileOkButton.Visible = false;
            deleteProfileOkButton.Visible = false;

            addProfileOkButton.Clicked += async (okSender, okArgs) =>
            {
                if (string.IsNullOrWhiteSpace(nameEntry.Text))
                {
                    ShowMessage("Please enter your name.");
                }
                else if (string.IsNullOrWhiteSpace(idEntry.Text))
                {
                    ShowMessage("Please enter your ID.");
                }
                else if (string.IsNullOrWhiteSpace(emailEntry.Text))
                {
                    ShowMessage("Please enter your email.");
                }
                else if (string.IsNullOrWhiteSpace(phoneEntry.Text))
                {
                    ShowMessage("Please enter your phone number.");
                }
                else
                {
                        profileText[0] = nameEntry.Text;
                        profileText[1] = idEntry.Text;
                        profileText[2] = emailEntry.Text;
                        profileText[3] = phoneEntry.Text;

                        File.AppendAllText(profilePath, profileText[0]+"\n");
                        File.AppendAllText(profilePath, profileText[1]+"\n");
                        File.AppendAllText(profilePath, profileText[2]+"\n");
                        File.AppendAllText(profilePath, profileText[3]+"\n");

                        addProfileOkButton.Visible = false;
                        nameEntry.Editable = false;
                        idEntry.Editable = false;
                        emailEntry.Editable = false;
                        phoneEntry.Editable = false;

                        listStore.AppendValues(profileText[0]);
                        comboBox.Model = listStore;

                        profileText = await ReadProfileAsync();

                        nameEntry.Text = string.Empty;
                        idEntry.Text = string.Empty;
                        emailEntry.Text = string.Empty;
                        phoneEntry.Text = string.Empty;

                        ShowMessage("Profile created successfully.");
                        return;
                }
            };
            titleLabel.Text = "Profile";
        };

                editProfileButton.Clicked += async (s, args) =>
                {
                    titleLabel.Text = "Edit Profile";

                    editProfileOkButton.Visible = true;

                    addProfileOkButton.Visible = false;
                    deleteProfileOkButton.Visible = false;

                    nameEntry.Editable = true;
                    idEntry.Editable = true;
                    emailEntry.Editable = true;
                    phoneEntry.Editable = true;

                    editProfileOkButton.Clicked += async (okSender, okArgs) =>
                    {
                        if (string.IsNullOrWhiteSpace(nameEntry.Text))
                        {
                            ShowMessage("No profile selected to edit, please select a profile from the drop-down menu.");
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(nameEntry.Text))
                        {
                            ShowMessage("Please enter your name.");
                        }
                        else if (string.IsNullOrWhiteSpace(idEntry.Text))
                        {
                            ShowMessage("Please enter your ID.");
                        }
                        else if (string.IsNullOrWhiteSpace(emailEntry.Text))
                        {
                            ShowMessage("Please enter your email.");
                        }
                        else if (string.IsNullOrWhiteSpace(phoneEntry.Text))
                        {
                            ShowMessage("Please enter your phone number.");
                        }
                        else
                        {
                            try{

                                editProfileList = new List<string>(profileText);

                                
                                editProfileList[editIndex] = nameEntry.Text;
                                editProfileList[editIndex+1] = idEntry.Text;                            
                                editProfileList[editIndex+2] = emailEntry.Text;
                                editProfileList[editIndex+3] = phoneEntry.Text;

                                profileText = editProfileList.ToArray();
                                File.WriteAllLines(profilePath, profileText);

                                editProfileOkButton.Visible = false;
                                nameEntry.Editable = false;
                                idEntry.Editable = false;
                                emailEntry.Editable = false;
                                phoneEntry.Editable = false;

                                profileText = await ReadProfileAsync();

                                listStore.Clear();

                                for(int i = 0; i<profileText.Length; i+=4){
                                    listStore.AppendValues(profileText[i]);
                                }

                                comboBox.Model = listStore;

                                nameEntry.Text = string.Empty;
                                idEntry.Text = string.Empty;
                                emailEntry.Text = string.Empty;
                                phoneEntry.Text = string.Empty;

                                ShowMessage("Profile updated successfully.");

                            }

                        catch (Exception ex)
                        {
                            contentTextView.Buffer.Text = $"Error: {ex.Message}";
                            titleLabel.Text = string.Empty;
                        }
                        }
                    };
                    titleLabel.Text = "Profile";
                };

                deleteButtonProfile.Clicked += async (s, args) =>
                {

                    titleLabel.Text = "Delete Profile";

                    deleteProfileOkButton.Visible = true;

                    editProfileOkButton.Visible = false;
                    addProfileOkButton.Visible = false;

                    deleteProfileOkButton.Clicked += async (okSender, okArgs) =>
                    {
                        if (string.IsNullOrWhiteSpace(nameEntry.Text))
                        {
                            ShowMessage("No profile selected to delete, please select a profile from the drop-down menu.");
                            return;
                        }

                        if(!dialogDisplayed){
                            using (var dialog = new MessageDialog(
                            this,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            $"Are you sure you want to delete {nameEntry.Text} from your profiles list?")
                        )

                        {
                            dialog.Title = "Delete Profile";
                            dialog.WindowPosition = WindowPosition.Center;

                            dialogDisplayed = true;

                            ResponseType response = (ResponseType)dialog.Run();

                            if (response == ResponseType.Yes)
                            {
                                deleteProfileList = new List<string>(profileText);
                                int deleteIndex = deleteProfileList.IndexOf(nameEntry.Text);

                                deleteProfileList.RemoveAt(deleteIndex);
                                deleteProfileList.RemoveAt(deleteIndex);
                                deleteProfileList.RemoveAt(deleteIndex);
                                deleteProfileList.RemoveAt(deleteIndex);

                                profileText = deleteProfileList.ToArray();

                                File.WriteAllLines(profilePath, profileText);

                                profileText = await ReadProfileAsync();
                                
                                listStore.Clear();

                                for(int i = 0; i<profileText.Length; i+=4){
                                    listStore.AppendValues(profileText[i]);
                                }

                                comboBox.Model = listStore;

                                nameEntry.Text = string.Empty;
                                idEntry.Text = string.Empty;
                                emailEntry.Text = string.Empty;
                                phoneEntry.Text = string.Empty;

                                deleteOkButton.Visible = false;
                                dialog.Hide();
                                ShowMessage("Profile deleted successfully.");
                            }
                            else{
                                dialog.Hide();
                                return;
                            }  
                            }
                    };
                };
                titleLabel.Text = "Profile";
                };

        
        comboBox.Changed += (sender, args) =>
        {

            TreeIter iter;
            if (comboBox.GetActiveIter(out iter))

            {
                string activeText = (string)listStore.GetValue(iter, 0);
                int selectedIndex = Array.IndexOf(profileText, activeText);

                while(string.IsNullOrWhiteSpace(activeText)){

                }

                editIndex = Array.IndexOf(profileText, activeText);

                try{
                    nameEntry.Text = profileText[selectedIndex];
                    idEntry.Text = profileText[selectedIndex+1];
                    emailEntry.Text = profileText[selectedIndex+2];
                    phoneEntry.Text = profileText[selectedIndex+3];
                }

                catch (Exception ex)
                {
                    contentTextView.Buffer.Text = $"Error: {ex.Message}";
                    titleLabel.Text = string.Empty;
                }

        };
        };

        nameEntry.Text = string.Empty;
        idEntry.Text = string.Empty;
        emailEntry.Text = string.Empty;
        phoneEntry.Text = string.Empty;

        nameEntry.Visible = true;
        nameEntry.Editable = false;
        idEntry.Visible = true;
        idEntry.Editable = false;
        emailEntry.Visible = true;
        emailEntry.Editable = false;
        phoneEntry.Visible = true;
        phoneEntry.Editable = false;
        editProfileButton.Visible = true;
            
        }

        private async void DownloadButton_Clicked(object sender, EventArgs e){
            downloadOkButton.Clicked -= downloadButtonClicked;
            if (bulkDownloadEntry.Text == string.Empty)
            {
                bulkDownloadEntry.Text = "bulk.txt";
            }
            bulkDownloadPath = bulkDownloadEntry.Text;
            if (!File.Exists(bulkDownloadPath))
            {
                ShowMessage("File does not exist. Please ensure that you have entered the right path.");
            }
            else
            {
                downloadLines = await ReadBulkDownloadAsync();
                contentTextView.Buffer.Text = string.Empty;

                using (HttpClient client = new HttpClient())
                {
                    bulkDownloadResult = "";
                    foreach (string url in downloadLines)
                    {
                        valid = await Validation(url);
                        if (valid)
                        {
                            HttpResponseMessage response = await client.GetAsync(url);
                            int statusCode = (int)response.StatusCode;
                            long bytes = response.Content.Headers.ContentLength ?? 0;

                            bulkDownloadResult += $"<{statusCode}> <{bytes}> <{url}>.\n";
                        }
                        else
                        {
                            bulkDownloadResult += (url + " - Invalid URL.");
                        }
                    }
                }

                contentTextView.Buffer.Text = bulkDownloadResult;
            }
            ShowMessage("Bulk download completed.");
        }
        private async void BulkDownloadButton_Clicked(object sender, EventArgs e)
    {
        titleLabel.Text = "Bulk Download";
        bulkDownloadPath = string.Empty;
        contentTextView.Buffer.Text = string.Empty;
        bulkDownloadEntry.Visible = true;
        bulkDownloadEntry.Text = string.Empty;
        downloadOkButton.Visible = true;
        downloadButtonClicked = null;

        downloadButtonClicked = async (okSender, okArgs) =>
        {
            DownloadButton_Clicked(okSender, okArgs);
        };

        downloadOkButton.Clicked += downloadButtonClicked;
    }

        private async void ReloadButton_Clicked(object sender, EventArgs e){
        DisplayWebContent(currentUrl, "reload");
        }

        private async void ClearHistoryButton_Clicked(object sender, EventArgs e){
            string[] history = await ReadHistoryAsync();

            if (history.Length==0){
                ShowMessage("History is already empty.");
            }

            else{
                dialogDisplayed = false;
                if(!dialogDisplayed){
                            using (var dialog = new MessageDialog(
                            this,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            $"Are you sure you want to clear your history? This is a non-reversible action.")
                        )
                        {
                            dialog.Title = "Clear History";
                            dialog.WindowPosition = WindowPosition.Center;

                            dialogDisplayed = true;

                            ResponseType response = (ResponseType)dialog.Run();

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

                if (hyperlinkTag == null)
                {
                    hyperlinkTag = new TextTag("hyperlink");
                }

                hyperlinkTag.Underline = Pango.Underline.Single;
                contentTextView.Buffer.TagTable.Add(hyperlinkTag);

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

        if (!File.Exists(favouritesPath))
            {
                File.Create(favouritesPath).Close();
            }

        try
        {
            isFavoritesPage = true;

            addButton.Visible = true;
            editButton.Visible = true;
            deleteButton.Visible = true;

            ApplyingHyperlinkTags("favourites");

                addButton.Clicked += (sender, args) =>
            {
                
                favouritesEntry.Visible = true;
                favouritesTitleEntry.Visible = true;
                addOkButton.Visible = true;
                titleLabel.Text = "Add Favourites";

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
                ShowMessage("Please select the URL to edit.");
                
                    navigateText = "";
                    titleText = "";
                    editOkButton.Clicked += async (okSender, okArgs) =>
                    {
                        if (!string.IsNullOrWhiteSpace(favouritesEntry.Text))
                        {
                            valid = await Validation(favouritesEntry.Text);
                        
                            if (!valid)
                            {
                                ShowMessage("Invalid URL.");
                                favouritesEntry.Text = string.Empty;
                                favouritesTitleEntry.Text = string.Empty;
                            }
                            else
                            {
                                    favourites[favouritesIndex] = favouritesTitleEntry.Text + ": " + favouritesEntry.Text;
                                    File.WriteAllLines(favouritesPath, favourites);
                                    favouritesEntry.Visible = false;
                                    favouritesTitleEntry.Visible = false;
                                    editOkButton.Visible = false;
                                    editMode = false;
                                    ApplyingHyperlinkTags("favourites");
                            }
                        }
                        
                    };
                favouritesEntry.Text = string.Empty;
            };

            deleteButton.Clicked += (s, args) =>
            {
                editMode = true;
                favouritesEntry.Text = string.Empty;
                favouritesTitleEntry.Text = string.Empty;
                favouritesEntry.Visible = true;
                deleteOkButton.Visible = true;
                titleLabel.Text = "Delete Favourites";
                dialogDisplayed = false;
    
                ShowMessage("Please select the URL to delete.");
            
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
                            dialog.Title = "Delete URL";
                            dialog.WindowPosition = WindowPosition.Center;

                            dialogDisplayed = true;

                            ResponseType response = (ResponseType)dialog.Run();

                            if (response == ResponseType.Yes)
                            {
                                deleteFavouritesList = new List<string>(favourites);
                                deleteFavouritesList.RemoveAt(favouritesIndex);

                                favourites = deleteFavouritesList.ToArray();

                                File.WriteAllLines(favouritesPath, favourites);

                                favouritesEntry.Visible = false;
                                favouritesTitleEntry.Visible = false;
                                deleteOkButton.Visible = false;
                                editMode = false;
                                dialog.Hide();

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
                                    int start = iter.Offset;
                                    int end = iter.Offset;

                                    while (start >= 0 && contentTextView.Buffer.GetIterAtOffset(start).HasTag(tag))
                                    {
                                        start--;
                                    }

                                    while (end < contentTextView.Buffer.Text.Length && contentTextView.Buffer.GetIterAtOffset(end).HasTag(tag))
                                    {
                                        end++;
                                    }

                                    selectedText = contentTextView.Buffer.GetText(contentTextView.Buffer.GetIterAtOffset(start + 1), contentTextView.Buffer.GetIterAtOffset(end), false);
                                    navigateText = "";
                                    
                                    for(int i = 0; i<selectedText.Length; i++){
                                        if(selectedText[i]==':'){
                                            navigateText = selectedText.Substring(i+2, selectedText.Length - (i + 2));

                                            if (editMode==true){
                                                favouritesTitleEntry.Text = selectedText.Substring(0, i);
                                                favouritesEntry.Text = selectedText.Substring(i+2, selectedText.Length - (i + 2));
                                                favouritesIndex = Array.IndexOf(favourites, selectedText);
                                            }
                                            break;
                                        }
                                    } 
                                    if(editMode==false){
                                    DisplayWebContent(navigateText, "nav");
                                    }
                                }
                            }
                        }
                    }
                };
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
        
        private void BackButton_Clicked(object sender, EventArgs e)
        {
            HideButtons();
            if (localHistoryBack.Count==0){
                ShowMessage("No previous URL.");
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
                                    int start = iter.Offset;
                                    int end = iter.Offset;

                                    while (start >= 0 && contentTextView.Buffer.GetIterAtOffset(start).HasTag(tag))
                                    {
                                        start--;
                                    }

                                    while (end < contentTextView.Buffer.Text.Length && contentTextView.Buffer.GetIterAtOffset(end).HasTag(tag))
                                    {
                                        end++;
                                    }

                                    selectedText = contentTextView.Buffer.GetText(contentTextView.Buffer.GetIterAtOffset(start + 1), contentTextView.Buffer.GetIterAtOffset(end), false);
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
                historyText = new string[0];
            }
            return historyText;
        }

        private async Task<string[]> ReadBulkDownloadAsync()
        {
            downloadLines = await File.ReadAllLinesAsync(bulkDownloadPath);
            return downloadLines;
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
                            valid = await Validation(favouritesEntry.Text);
                        
                            if (!valid)
                            {
                                ShowMessage("Invalid URL.");
                                favouritesEntry.Text = string.Empty;
                                favouritesTitleEntry.Text = string.Empty;
                            }
                            else
                            {
                                    File.WriteAllText(homePath, favouritesEntry.Text);
                                    contentTextView.Buffer.Text = string.Empty;
                                    favouritesEntry.Visible = false;
                                    editHomeOkButton.Visible = false;
                                    ShowMessage("Home page updated, you have been redirected to your new home page.");
                                    LoadHomePage();
                            }
                        }
            };
        }

        private async void DisplayWebContent(string url, string action){

            HideButtons();

            try
            {
                string content = await FetchWebContentAsync(url, action);
                addressEntry.Text = url;

                contentTextView.Buffer.Text = content;

                statusCode = $"HTTP Status Code: {currentUrl}";
                titleLabel.Text = statusCode;

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
                                ShowMessage("Page reloaded.");
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

        public async void LoadHomePage()
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

        public async Task<string> ReadHomePageAsync()
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
                string titlePattern = @"<title\b[^>]*>(.*?)</title>";
                var match = Regex.Match(htmlContent, titlePattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    return addressEntry.Text;
                }
            }
            catch
            {
                return addressEntry.Text;
            }
        }

        private void OnAddressEntryChanged(object sender, EventArgs e)
        {
            textFromAddressEntry = addressEntry.Text;
            AddressEntryTextChanged?.Invoke(this, textFromAddressEntry);
        }

        public string GetTextFromEntry()
        {
            return textFromAddressEntry;
        }
        
        public static string RetrieveTextFromAddressEntry(WebBrowserApp app)
        {
            return app.GetTextFromEntry(); // Access the text using the instance of WebBrowserApp
        }

        public static void Main(string[] args)
        {
            Application.Init();
            var app = new WebBrowserApp();
            app.HideButtons();
            app.LoadHomePage();
            app.addressEntry.Changed += (sender, e) => {
            Console.WriteLine("HOME: " + RetrieveTextFromAddressEntry(app));
            };
            app.ShowAll();
            Application.Run();   
        }
    }

}