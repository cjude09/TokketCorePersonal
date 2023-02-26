
using AndroidX.AppCompat.App;

namespace Tokket.Android.Helpers
{
    public abstract class AppCompatActivityBase : AppCompatActivity
    {
        public static AppCompatActivityBase CurrentActivity { get; private set; }

        internal string ActivityKey { get; private set; }

        internal static string NextPageKey { get; set; }
        
        public static void GoBack()
        {
            CurrentActivity?.OnBackPressed();
        }
        
        protected override void OnResume()
        {
            CurrentActivity = this;
            if (string.IsNullOrEmpty(ActivityKey))
            {
                ActivityKey = NextPageKey;
                NextPageKey = null;
            }
            base.OnResume();
        }
    }
}