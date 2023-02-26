using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Tokket.Android.ViewModels;

namespace Tokket.Android.Helpers
{
    public static class LocalSettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        #region Setting Constants

        private const string SettingsKey = "T0KK3P3D!@";
        private static readonly string SettingsDefault = string.Empty;

        #endregion

        #region SavedData
        public static string TokMojidrawable
        {
            get => AppSettings.GetValueOrDefault(nameof(TokMojidrawable), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TokMojidrawable), value);
        }
        public static List<TokMojiDrawableViewModel> GetTokMojiDrawable() => JsonConvert.DeserializeObject<List<TokMojiDrawableViewModel>> (TokMojidrawable);
        #endregion
    }
}