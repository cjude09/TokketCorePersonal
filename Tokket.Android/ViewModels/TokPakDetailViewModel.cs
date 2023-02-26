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
using Tokket.Shared.Models;

namespace Tokket.Android.ViewModels
{
    public class TokPakDetailViewModel
    {
        public ClassTokModel classTokModel { get; set; } = new ClassTokModel();
        public List<ClassTokModel> tokItemList { get; set; } = new List<ClassTokModel>();
    }
}