using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;
using Xamarin.Auth;

namespace XFCloudStorage.Droid
{
    [Activity(Label = "CustomUrlSchemeInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataSchemes = new[] { "com.esmsmartsolutions.xfcloudstorage" }, DataPath = "/oauth2redirect")]
    public class ActivityCustomUrlSchemeInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            global::Android.Net.Uri uri_android = Intent.Data;
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            var uri = new Uri(Intent.Data.ToString());
            AuthenticatorHelper.OAuth2Authenticator.OnPageLoading(uri);
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
            this.Finish();
            return;
        }
    }
}