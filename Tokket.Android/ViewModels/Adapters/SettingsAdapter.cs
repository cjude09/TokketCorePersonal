using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Newtonsoft.Json;
using Plugin.Permissions;
using Xamarin.Essentials;
using static Android.App.ActionBar;
using SharedHelpers = Tokket.Shared.Helpers;

namespace Tokket.Android.Adapters
{
    public class SettingsAdapter : BaseAdapter<string>
    {
        Button btnDownloadPDF;
        private Dialog popupDialog;
        string[] items;
        Activity context;
        int positionx = 0;
        public SettingsAdapter(Activity context, string[] items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            positionx = position;
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            { 
                view = context.LayoutInflater.Inflate(Resource.Layout.settings_row, null);
            }
            view.FindViewById<TextView>(Resource.Id.lblSettingsRow).Text = items[position];
            return view;
        }
        public async void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(positionx == 0) //Personal Data
            {
                popupDialog = new Dialog(context);
                popupDialog.SetContentView(Resource.Layout.personaldata_page);
                popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                popupDialog.Show();

                // Some Time Layout width not fit with windows size  
                // but Below lines are not necessary  
                double widthD = MainActivity.Instance.getLayoutWidth();
                popupDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
                popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

                // Access Popup layout fields like below  
                btnDownloadPDF = popupDialog.FindViewById<Button>(Resource.Id.btnDownloadPDF);

                // Events for that popup layout  
                btnDownloadPDF.Click += delegate
                {
                    PdfClickHandler();
                };
            }
            else if (positionx == 1) //Change Password
            {
                Intent changePasswordActivity = new Intent(context, typeof(ChangePasswordActivity));
                context.StartActivity(changePasswordActivity);
            }
            else if (positionx == 2) //Privacy Policy
            {
                await Xamarin.Essentials.Browser.OpenAsync("https://tokket.com/privacy", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = System.Drawing.Color.AliceBlue,
                    PreferredControlColor = System.Drawing.Color.Violet
                });
            }
            else if (positionx == 3) //Terms of Service
            {
                await Xamarin.Essentials.Browser.OpenAsync("https://tokket.com/terms", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = System.Drawing.Color.AliceBlue,
                    PreferredControlColor = System.Drawing.Color.Violet
                });
            }
            else if (positionx == 4) //About
            {
                await Xamarin.Essentials.Browser.OpenAsync("https://tokket.com/about", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = System.Drawing.Color.AliceBlue,
                    PreferredControlColor = System.Drawing.Color.Violet
                });
            }
            else if (positionx == 5) //Contact
            {
                List<string> ListRecipients = new List<string>();
                ListRecipients.Add("support@tokkepedia.com");
                var device = DeviceInfo.Model;
                var manufacturer = DeviceInfo.Manufacturer;
                var version = DeviceInfo.VersionString;
                var platform = DeviceInfo.Platform;

                string bodyemail = "\n" + "\n" + "\n" + "\n" + (manufacturer.Substring(0,1).ToUpper() + manufacturer.Substring(1)) + " " + device + " " + platform + " "+version;
                await SendEmail("", bodyemail, ListRecipients);
            }
            else if (positionx == 6) //More Settings
            {
                Intent moreSettingsActivity = new Intent(context, typeof(MoreSettingsActivity));
                context.StartActivity(moreSettingsActivity);
            }
            else if (positionx == 7) //Log Out
            {
                MainActivity.Instance.Finish();
                Intent logoutActivity = new Intent(context, typeof(LoginActivity));
                logoutActivity.AddFlags(ActivityFlags.ClearTop);
                SecureStorage.Remove("idtoken");
                SecureStorage.Remove("refreshtoken");
                SecureStorage.Remove("userid");

                SharedHelpers.Settings.UserAccount = string.Empty;

                context.StartActivity(logoutActivity);
                context.Finish();
            }
        }
        public async Task SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }
        public override int Count
        {
            get { return items.Length; }
        }

        public override string this[int position]
        {
            get { return items[position]; }
        }
        private void PdfClickHandler()
        {
            var webClient = new WebClient();
            webClient.DownloadDataCompleted += (s, e) => {
                var data = e.Result;
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string localFilename = $"{"tokkepedia_personalData"}.pdf";
                System.IO.File.WriteAllBytes(Path.Combine(documentsPath, localFilename), data);
                context.RunOnUiThread(() => {
                    var objBuilder = new AlertDialog.Builder(context);
                    objBuilder.SetTitle("Done");
                    objBuilder.SetMessage("File downloaded and saved");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", async (s3, ev) => {
                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(Path.Combine(documentsPath, localFilename))
                        });
                    });
                    objDialog.Show();
                    objDialog.SetCanceledOnTouchOutside(false);
                });
            };
            var url = new Uri(($"{Tokket.Shared.Config.Configurations.Url}/report/personaldata.pdf?userId={SharedHelpers.Settings.GetUserModel().UserId}"), UriKind.Absolute);
            webClient.DownloadDataAsync(url);
        }

        /*private async void BtnDownLoadPDF_Click(object sender, System.EventArgs e0)
        {
            var webClient = new WebClient();
            webClient.DownloadDataCompleted += (s, e) => {
                var data = ObjectToByteArray(JsonConvert.SerializeObject(SharedHelpers.Settings.GetTokketUser())); //e.Result;
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();

                string localFilename = $"{"tokkepedia_personalData"}.pdf";

                System.IO.File.WriteAllBytes(Path.Combine(documentsPath, localFilename), data);

                context.RunOnUiThread(() => {
                    var objBuilder = new AlertDialog.Builder(context);
                    objBuilder.SetTitle("Done");
                    objBuilder.SetMessage("File downloaded and saved");
                    objBuilder.SetCancelable(false);

                    AlertDialog objDialog = objBuilder.Create();
                    objDialog.SetButton((int)(DialogButtonType.Positive), "OK", async (s3, ev) =>{
                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(Path.Combine(documentsPath, localFilename))
                        });
                    });
                    objDialog.Show();
                    objDialog.SetCanceledOnTouchOutside(false);
                });
            };

            var url = new Uri(("User/_personalData.pdf"), UriKind.Relative);
            webClient.DownloadDataAsync(url);
        }*/
        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                return ms.ToArray();
            }
        }
    }

    class SettingsAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}