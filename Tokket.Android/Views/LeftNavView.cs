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
using Config = Tokket.Shared.Config;

namespace Tokket.Android.Views
{
    public static class LeftNavView
    {
        public static List<Tuple<int, string>> LoadClassTokHeaderNavigation(Context context)
        {
            var ListDataHeader = new List<Tuple<int, string>>();
           ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_classsets, "Class Toks"));
           ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_tokchannels, "Tok Channels"));
           ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_tokpaks, "Tok Paks"));
           ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_classsets, "Class Sets"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_studygames, "Study Games"));
            if (Config.Configurations.AlphaToksEnabled)
                ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.alphaguesslogo, "Alpha Toks"));
            if (Config.Configurations.TokQuestEnabled)
                ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.tokquesticon, "Tok Quest"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_tokgroups, context.GetString(Resource.String.tok_group)));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_invite, "Notifications"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_guide, "Guides"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.peerbook_img, "Peerbook"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_opportunity, "Opportunity"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_training, "Training"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.settings_black_36dp, "Settings"));

            return ListDataHeader;
        }

        public static Dictionary<string, List<string>> LoadClassToChildrenNavigation()
        {
            var ListDataChild = new Dictionary<string, List<string>>();
            #region Class Toks
            var ClassTokList = new List<string>();
            ClassTokList.Add("My Class Toks");
            ClassTokList.Add("Public Class Toks");
            ListDataChild.Add("Class Toks", ClassTokList);
            #endregion

            #region Tok Channels
            var TokChannelList = new List<string>();
            TokChannelList.Add("Career");
            TokChannelList.Add("College");
            TokChannelList.Add("Junior School");
            TokChannelList.Add("Health");
            TokChannelList.Add("High School");
            TokChannelList.Add("Recreation");
            TokChannelList.Add("Wisdom");
            ListDataChild.Add("Tok Channels", TokChannelList);
            #endregion

            #region Tok Paks
            var TokPaksList = new List<string>();
            TokPaksList.Add("Add Tok Paks");
            TokPaksList.Add("View My Tok Paks");
            TokPaksList.Add("View Public Tok Paks");
            ListDataChild.Add("Tok Paks", TokPaksList);
            #endregion

            #region Guides
            var Guidelist = new List<string>();
            Guidelist.Add("•How to Add Class Toks");
            Guidelist.Add("•How to Create a Tok Group");
            Guidelist.Add("•How to Create Class Toks");
            Guidelist.Add("•How to Create Presentation");
            Guidelist.Add("•How to Create a Family Account");
            Guidelist.Add("•How to Create an Organization Account");
            ListDataChild.Add("Guides", Guidelist);
            #endregion

            #region Class Sets
            var ClassSetList = new List<string>();
            ClassSetList.Add("My Class Sets");
            ClassSetList.Add("Public Class Sets");
            ListDataChild.Add("Class Sets", ClassSetList);
            #endregion

            #region Study games
            var StudyGamesList = new List<string>();
            StudyGamesList.Add("Tok Cards");
            StudyGamesList.Add("Tok Choice");
            StudyGamesList.Add("Tok Match");
            ListDataChild.Add("Study Games", StudyGamesList);
            #endregion

            return ListDataChild;
        }

        public static List<Tuple<int, string>> LoadTokChannelNavigation(Context context)
        {
            var ListDataHeader = new List<Tuple<int, string>>();
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_classsets, "Family & Friends"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_tokchannels, "Health"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_tokpaks, "How To"));
            ListDataHeader.Add(new Tuple<int, string>(Resource.Drawable.left_nav_classsets, "Personal"));

            return ListDataHeader;
        }
    }
}