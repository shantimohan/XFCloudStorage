using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Forms;
using XFCloudStorage.Services;

namespace XFCloudStorage
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // Google Drive vars and constants
        private string scope = "https://www.googleapis.com/auth/drive.file";
        private string clientId = "";
        private string redirectUrl = "";

        private string clientIdAndroid = "852843316466-0bgi1o3os1rvo74tidsso74482jan4v4.apps.googleusercontent.com";
        private string redirecUrlAndroid = "com.esmsmartsolutions.xfcloudstorage:/oauth2redirect";

        private string clientIdiOS = "852843316466-5131ijijom1ej1asa8etn9ansk5q3ur2.apps.googleusercontent.com";
        private string redirecUrliOS = "com.esmsmartsolutions.XFCloudStorage:/oauth2redirect";
        //private string redirecUrliOS = "com.googleusercontent.apps.852843316466-5131ijijom1ej1asa8etn9ansk5q3ur2:/oauth2redirect";

        private string clientIdUWP = "852843316466-r928lct9qu6jb0himnknone7dnst6kj1.apps.googleusercontent.com";
        //private string clientIdUWP = "852843316466-2eng3it5uh5ia1o2d3uiu03jh4o4k8t9.apps.googleusercontent.com";
        private string clientSecretUWP = "f0DEsvt63mgoPc41BQYoZAX9";
        //private string clientSecretUWP = "";
        private string redirecUrlUWP = "com.esmsmartsolutions.xfcloudstorage:/oauth2redirect";
        //private string redirecUrlUWP = "com.googleusercontent.apps.852843316466-2eng3it5uh5ia1o2d3uiu03jh4o4k8t9:/oauth2redirect";

        public OAuth2Authenticator auth;
        public DriveService driveService;
        public GoogleDriveServiceHelper googleDriveHelper;
        private FileData fileData;

        public MainPage()
        {
            InitializeComponent();

            FileName.Text = "TestFile_";
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    FileName.Text += "Android";
                    break;
                case Device.iOS:
                    FileName.Text += "iOS";
                    break;
                case Device.UWP:
                    FileName.Text += "UWP";
                    break;
            }
            FileName.Text += ".txt";

            Contents2Save.Text += $" This file is saved from Android to file named {FileName.Text}";
        }

        private void OnCloudStorageServiceSelected(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            GoogleDrive.BackgroundColor = Color.Transparent;
            OneDrive.BackgroundColor = Color.Transparent;
            DropBox.BackgroundColor = Color.Transparent;
            iCloud.BackgroundColor = Color.Transparent;

            if (btn.Text.StartsWith("Goo"))
            {
                InitializeGoogleDriveService();
                App.ServiceSelected = App.StorageServices.GoogleDrive;
                GoogleDrive.BackgroundColor = Color.LightBlue;
            }
            else if (btn.Text.StartsWith("One"))
            {
                App.ServiceSelected = App.StorageServices.OneDrive;
                OneDrive.BackgroundColor = Color.LightBlue;
            }
            else if (btn.Text.StartsWith("Dro"))
            {
                App.ServiceSelected = App.StorageServices.DropBox;
                DropBox.BackgroundColor = Color.LightBlue;
            }
            else if (btn.Text.StartsWith("iCl"))
            {
                App.ServiceSelected = App.StorageServices.iCloud;
                iCloud.BackgroundColor = Color.LightBlue;
            }
        }

        private void OnClearContents2Save(object sender, EventArgs e)
        {
            Contents2Save.Text = "";
        }

        private async void OnSave2File(object sender, EventArgs e)
        {
            Debug.WriteLine($"Cloud Storage: In OnSave2File()...");
            string storageService = "";
            ContentsRead.Text = "";

            this.IsBusy = true;

            try
            {
                if (App.ServiceSelected == App.StorageServices.GoogleDrive)
                {
                    if (App.InitGoogleDriveService)
                    {
                        storageService = "Google Drive";
                        string id = googleDriveHelper.GetFileID(FileName.Text.Trim());
                        if (string.IsNullOrEmpty(id))
                            id = await googleDriveHelper.CreateFile();
                        await googleDriveHelper.SaveFile(id, FileName.Text.Trim(), Contents2Save.Text);
                    }
                }
                else if (App.ServiceSelected == App.StorageServices.OneDrive)
                {
                    if (OneDriveServiceInitialized())
                    {
                        storageService = "One Drive";

                    }
                }
                else if (App.ServiceSelected == App.StorageServices.DropBox)
                {
                    if (DropBoxServiceInitialized())
                    {
                        storageService = "DropBox";

                    }
                }
                else if (App.ServiceSelected == App.StorageServices.iCloud)
                {
                    if (icloudServiceInitialized())
                    {
                        storageService = "iCloud";

                    }
                }

                await DisplayAlert("Cloud Storage", $"Saved '{FileName.Text.Trim()}' successfully to {storageService}", "Continue...");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Cloud Storage", $"Error saving '{FileName.Text.Trim()}' to {storageService} - {ex.Message}", "Continue...");
            }

            this.IsBusy = false;
        }

        private async void OnReadFromFile(object sender, EventArgs e)
        {
            Debug.WriteLine($"Cloud Storage: In OnReadFromFile()...");
            string storageService = "";
            bool fileExists = true;
            try
            {
                if (App.ServiceSelected == App.StorageServices.GoogleDrive)
                {
                    if (App.InitGoogleDriveService)
                    {
                        storageService = "Google Drive";
                        string id = googleDriveHelper.GetFileID(FileName.Text.Trim());
                        if (!string.IsNullOrEmpty(id))
                        {
                            fileData = await googleDriveHelper.ReadFile(id);
                            ContentsRead.Text = fileData.Content;
                        }
                        else
                        {
                            ContentsRead.Text = "*** File does not exist ***";
                            fileExists = false;
                        }
                    }
                }
                else if (App.ServiceSelected == App.StorageServices.OneDrive)
                {
                    if (OneDriveServiceInitialized())
                    {
                        storageService = "One Drive";

                    }
                }
                else if (App.ServiceSelected == App.StorageServices.DropBox)
                {
                    if (DropBoxServiceInitialized())
                    {
                        storageService = "DropBox";

                    }
                }
                else if (App.ServiceSelected == App.StorageServices.iCloud)
                {
                    if (icloudServiceInitialized())
                    {
                        storageService = "iCloud";

                    }
                }

                if (fileExists)
                    await DisplayAlert("Cloud Storage", $"Read '{FileName.Text.Trim()}' successfully from {storageService}", "Continue...");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Cloud Storage", $"Error reading '{FileName.Text.Trim()}' from {storageService} - {ex.Message}", "Continue...");
            }
        }

        #region Google Drive Methods
        private void InitializeGoogleDriveService()
        {
            if (auth == null)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        clientId = clientIdAndroid;
                        redirectUrl = redirecUrlAndroid;
                        break;
                    case Device.iOS:
                        clientId = clientIdiOS;
                        redirectUrl = redirecUrliOS;
                        break;
                    case Device.UWP:
                        clientId = clientIdUWP;
                        redirectUrl = redirecUrlUWP;
                        break;
                }

                auth = new OAuth2Authenticator(
                    clientId,
                    Device.RuntimePlatform == Device.UWP ? clientSecretUWP : string.Empty,
                    scope,
                    new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                    new Uri(redirectUrl),
                    new Uri("https://www.googleapis.com/oauth2/v4/token"),
                    isUsingNativeUI: true);
                auth.Completed += Auth_Completed;

                AuthenticatorHelper.OAuth2Authenticator = auth;
            }

            OAuthLoginPresenter presenter = new OAuthLoginPresenter();
            presenter.Login(auth);
        }

        private async void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            Debug.WriteLine($"Cloud Storage: In event Auth_Completed()...");

            if (e.IsAuthenticated)
            {

                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets()
                    {
                        ClientId = clientId,
                    }
                };
                initializer.Scopes = new[] { scope };
                initializer.DataStore = new FileDataStore("Google.Apis.Auth");

                var flow = new GoogleAuthorizationCodeFlow(initializer);
                var user = "CloudStorage";
                var token = new TokenResponse()
                {
                    AccessToken = e.Account.Properties["access_token"],
                    ExpiresInSeconds = Convert.ToInt64(e.Account.Properties["expires_in"]),
                    RefreshToken = e.Account.Properties["refresh_token"],
                    Scope = e.Account.Properties["scope"],
                    TokenType = e.Account.Properties["token_type"]
                };

                UserCredential userCredential = new UserCredential(flow, user, token);
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = "CloudStorage",
                });
                googleDriveHelper = new GoogleDriveServiceHelper(driveService);

                App.InitGoogleDriveService = true;
            }
            else
            {
                await DisplayAlert("Cloud Storage", "Authentication failed", "Continue...");
                App.InitGoogleDriveService = false;
            }
        }
        #endregion

        private bool OneDriveServiceInitialized()
        {
            if (!App.InitOneDriveServie)
            {

            }

            return App.InitOneDriveServie;
        }

        private bool DropBoxServiceInitialized()
        {
            if (!App.InitDropBoxService)
            {

            }

            return App.InitDropBoxService;
        }

        private bool icloudServiceInitialized()
        {
            if (!App.InitiCloudService)
            {

            }

            return App.InitiCloudService;
        }
    }

    public static class AuthenticatorHelper
    {
        public static OAuth2Authenticator OAuth2Authenticator { get; set; }
    }
}
