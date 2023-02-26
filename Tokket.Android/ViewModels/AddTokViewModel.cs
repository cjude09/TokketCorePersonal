using System;
using System.Collections.Generic;
using System.Linq;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Shared.Helpers;
using Tokket.Core;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Tokket.Android.ViewModels
{
    public class AddTokViewModel : ObservableObject
    {
        #region Properties
        public TokModel TokModel { get; set; }
        public List<TokTypeList> TokGroup { get; set; }
        public ObservableCollection<TokModel> TokModelCollection { get; private set; }
        #endregion

        #region Commands

        #endregion
        public AddTokViewModel()
        {
            TokModel = new TokModel();
            TokGroup = TokGroupHelper.TokGroups.OrderBy(item => item.TokGroup).ToList();
            TokModelCollection = new ObservableCollection<TokModel>();
            TokModelCollection.Clear();

            TokModelCollection.Add(TokModel);
        }
        #region Methods/Events

        #endregion
    }
}