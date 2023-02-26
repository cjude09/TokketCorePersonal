using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokket.Android
{
    [Activity(Label = "FacebookShareActivity")]
    public class FacebookShareActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //FacebookClientManager.Initialize(this);
            // Create your application here
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //FacebookClientManager.OnActivityResult(requestCode, resultCode, data);
        }
    }
}