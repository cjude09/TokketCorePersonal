using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Tokket.Android.Helpers
{
    public class TimerTask
    {
        string TimeElapsed = "";
        Activity activity;
        Timer timer;
        int hour = 0, min = 0, sec = 0;
        public void TimerStart(Activity _activity)
        {
            activity = _activity;
            timer = new Timer();
            timer.Interval = 1000; // 1 second  
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public void TimerStop()
        {
            timer.Dispose();
            timer = null;
        }

        public string GetTimeElapsed()
        {
            return TimeElapsed;
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec++;
            if (sec == 60)
            {
                min++;
                sec = 0;
            }
            if (min == 60)
            {
                hour++;
                min = 0;
            }
            activity.RunOnUiThread(() => { TimeElapsed = $"{hour}:{min}:{sec}"; });
        }
    }
}
