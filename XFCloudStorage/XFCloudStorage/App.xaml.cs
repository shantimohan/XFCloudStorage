using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFCloudStorage
{
    public partial class App : Application
    {
        public enum FileOps
        {
            None,
            List,
            Read,
            Write
        };

        public enum StorageServices
        {
            GoogleDrive,
            OneDrive,
            DropBox,
            iCloud
        };

        public static StorageServices ServiceSelected = StorageServices.GoogleDrive;
        public static bool InitGoogleDriveService = false;
        public static bool InitOneDriveServie = false;
        public static bool InitDropBoxService = false;
        public static bool InitiCloudService = false;
        public static FileOps OpOnFile = FileOps.None;

        public App(Uri uri = null)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
