using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Tokket.Shared.Helpers;

namespace Tokket.Android.Adapters
{
    public class ExpandableListAdapter : BaseExpandableListAdapter
    {

        Context context;
        private List<Tuple<int, string>> mListDataHeader; // header titles

        // child data in format of header title, child title
        private Dictionary<string, List<string>> mListDataChild;
        ExpandableListView expandList;
        public ExpandableListAdapter(Context context, List<Tuple<int, string>> listDataHeader, Dictionary<string, List<string>> listChildData, ExpandableListView mView)
        {
            this.context = context;
            this.mListDataHeader = listDataHeader;
            this.mListDataChild = listChildData;
            this.expandList = mView;
        }

        public override int GroupCount => mListDataHeader.Count;

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return this.mListDataChild[this.mListDataHeader[groupPosition].Item2][childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            int childCount = 0;
            try
            {
                childCount = this.mListDataChild[this.mListDataHeader[groupPosition].Item2].Count;
            }
            catch (Exception) { }
            return childCount;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var childText = (string)GetChild(groupPosition, childPosition);

            if (convertView == null)
            {
                LayoutInflater infalInflater = (LayoutInflater)this.context.GetSystemService(Context.LayoutInflaterService);
                convertView = infalInflater.Inflate(Resource.Layout.navigation_classtok_sub_menu, null);
            }

            TextView txtListChild = (TextView)convertView.FindViewById(Resource.Id.txtItem);

            txtListChild.Text = childText;

            if (mListDataHeader[groupPosition].Item2.ToLower() == "guides")
            {
                txtListChild.SetTextSize(ComplexUnitType.Px, context.Resources.GetDimension(Resource.Dimension._11ssp));
            }
            else
            {
                txtListChild.SetTextSize(ComplexUnitType.Px, context.Resources.GetDimension(Resource.Dimension._12ssp));
            }

            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return this.mListDataHeader[groupPosition].Item2;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            var headerTitle = (string)GetGroup(groupPosition);
            if (convertView == null)
            {
                LayoutInflater infalInflater = (LayoutInflater)this.context.GetSystemService(Context.LayoutInflaterService);
                convertView = infalInflater.Inflate(Resource.Layout.navigation_classtok_header, null);
            }

            ImageView ind = (ImageView)convertView.FindViewById(Resource.Id.iv_navigation);
            if (GetChildrenCount(groupPosition) == 0)
            {
                ind.Visibility = ViewStates.Invisible;
            }
            else
            {
                ind.Visibility = ViewStates.Visible;
                if (this.mListDataChild[this.mListDataHeader[groupPosition].Item2].Count() > 0)
                {
                    if (isExpanded)
                    {
                        ind.SetImageResource(Resource.Drawable.caret_up);
                        //ind.StartAnimation(rotate);
                    }
                    else
                    {
                        ind.SetImageResource(Resource.Drawable.caret_down);
                        //  ind.StartAnimation(rotate);
                    }

                    if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
                    {
                        ind.ImageTintList = ColorStateList.ValueOf(Color.White);
                    }
                }
            }

            /*  ImageView ind = (ImageView)convertView.FindViewById(Resource.Id.iv_navigation);
              if (GetChildrenCount(groupPosition) == 0)
              {
                  ind.Visibility = ViewStates.Invisible;
              }
              else
              {
                  if (this.mListDataChild[this.mListDataHeader[groupPosition]].Count() > 0)
                  {
                      if (isExpanded)
                      {
                          ind.SetImageResource(Resource.Drawable.caret_up);
                          //ind.StartAnimation(rotate);
                      }
                      else
                      {
                          ind.SetImageResource(Resource.Drawable.caret_down);
                          //  ind.StartAnimation(rotate);
                      }
                  }
              }*/

            TextView lblListHeader = (TextView)convertView.FindViewById(Resource.Id.txtTitle);
            ImageView headerIcon = (ImageView)convertView.FindViewById(Resource.Id.imageIcon);
            lblListHeader.SetTypeface(null, TypefaceStyle.Bold);
            lblListHeader.Text = headerTitle;
            headerIcon.SetImageResource(mListDataHeader[groupPosition].Item1);

            if (Settings.CurrentTheme == (int)ThemeStyle.Dark)
            {
                headerIcon.ImageTintList = ColorStateList.ValueOf(Color.White);
            }

            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}