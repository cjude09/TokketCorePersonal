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
using Tokket.Shared.Models.Tokquest;

namespace Tokket.Android.ViewModels
{
   public class CreateGameDetailsViewModel
    {
        public string ClassSetId { get; set; }
        public string OwnerId { get; set; }
        public string ChosenName { get; set; }
        public string ClassGroupId { get; set; }
        public List<ClassTok> classTokModels { get; set; }

    }
}