using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Tokket.Android.Adapters;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Core.Tools;
using static Tokket.Android.Helpers.MenuHeaderHelper;

namespace Tokket.Android.Fragments
{
    public class TokTypeBasicFragment : AndroidX.Fragment.App.Fragment
    {
        View v;
        public static int TYPE_ITEM = 0;
        public static int TYPE_SEPORATOR = 1;
        List<TokTypeList> tokgroups_basic = new List<TokTypeList>();
        List<IMenuItemsType> itemBasic = new List<IMenuItemsType>();
        int colorIndex = 0;
        List<string> materialColors = new List<string> { "f44336", "E91E63", "9C27B0", "673AB7", "3F51B5", "2196F3", "03A9F4", "00BCD4", "009688", "4CAF50", "8BC34A", "CDDC39", "efdc3c", "FFC107", "FF9800", "FF5722" };
        List<string> materialLightColors = new List<string> { "ef9a9a", "F48FB1", "CE93D8", "B39DDB", "9FA8DA", "90CAF9", "81D4FA", "80DEEA", "80CBC4", "A5D6A7", "C5E1A5", "E6EE9C", "FFF59D", "FFE082", "FFCC80", "FFAB91" };
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            v = inflater.Inflate(Resource.Layout.toktypebasic_page, container, false);

            BrowseModel vm = new BrowseModel();
            vm.TokGroups = TokGroupTool.TokGroups;
            itemBasic.Clear();
            List<TokTypeList> groups = new List<TokTypeList>();
            foreach (var group in vm.TokGroups)
            {
                groups.Add(group);
            }
            groups = groups.OrderBy(x => x.TokTypes.Count()).ToList();
            vm.TokGroups = groups;
            //Tok Types in alphabetical order logic
            List<List<(string, string)>> listOfLists = new List<List<(string, string)>>();
            for (int i = 0; i < vm.TokGroups.Count; ++i)
            {
                List<(string, string)> listTuple = new List<(string, string)>();
                for (int j = 0; j < vm.TokGroups[i].TokTypes.Length; ++j)
                {
                    listTuple.Add((vm.TokGroups[i].TokTypes[j], vm.TokGroups[i].TokTypeIds[j]));
                }
                listTuple = listTuple.OrderBy(x => x.Item1).ToList(); //Alpha
                listOfLists.Add(listTuple);
            }
            for (int i = 0; i < vm.TokGroups.Count; ++i)
            {
                vm.TokGroups[i].TokTypes = listOfLists[i].Select(x => x.Item1).ToArray();
                vm.TokGroups[i].TokTypeIds = listOfLists[i].Select(x => x.Item2).ToArray();
            }

            tokgroups_basic.Clear();
            for (int i = 0; i < vm.TokGroups.Count; i++)
            {
                var tokgroup = vm.TokGroups[i];
                if (!tokgroup.IsDetailBased)
                {
                    tokgroups_basic.Add(tokgroup);
                }

            }

            colorIndex = 0;
            for (int i = 0; i < tokgroups_basic.Count; i++)
            {
                if (colorIndex >= materialColors.Count) { colorIndex = 0; }
                itemBasic.Add(new MenuHeaderItem(tokgroups_basic[i].TokGroup, "#" + materialColors[colorIndex]));
                for (int z = 0; z < tokgroups_basic[i].TokTypes.Length; z++)
                {
                    itemBasic.Add(new MenuContentItem(tokgroups_basic[i].TokTypes[z], "", "#" + materialLightColors[colorIndex]));
                }
                colorIndex += 1;
            }

            //var adapterBasic = new BasicTokTypeAdapter(MainActivity.Instance, itemBasic);
            //ListBasic.Adapter = adapterBasic;

            var mLayoutManager = new LinearLayoutManager(MainActivity.Instance);
            RecyclerBasic.SetLayoutManager(mLayoutManager);
            var BasicAdapter = new TokGroupBasicAdapter(MainActivity.Instance,itemBasic);
            RecyclerBasic.SetAdapter(BasicAdapter);

            return v;
        }
        //public ListView ListBasic => v.FindViewById<ListView>(Resource.Id.listViewBasicTokType);
        public RecyclerView RecyclerBasic => v.FindViewById<RecyclerView>(Resource.Id.RecyclerViewBasicTokType);
    }
}