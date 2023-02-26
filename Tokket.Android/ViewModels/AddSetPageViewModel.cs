using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Helpers;
using Tokket.Core;
using Tokket.Core.Tools;
using SharedService = Tokket.Shared.Services;
using Android.Graphics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tokket.Android.ViewModels
{
    public class AddSetPageViewModel : ViewModelBase
    {
        #region Properties
        AlertDialog.Builder builder;
        Dialog dialog;
        public Activity Instance { get; set; }
        public Set Credentials { get; set; }
        public string Id { get; set; }
        private bool _isAddSet = false;
        public LinearLayout LinearProgress { get; set; }
        public ProgressBar ProgressCircle { get; set; }
        public TextView ProgressText { get; set; }
        public List<TokTypeList> TokGroup { get; set; }
        public ObservableCollection<string> TokGroupList { get; private set; }
        public ObservableCollection<string> PrivacyList { get; private set; }
        public bool IsAddSet
        {
            get
            {
                return _isAddSet;
            }
            set
            {
                if (Set(() => IsAddSet, ref _isAddSet, value))
                {
                    AddSetCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand AddSetCommand { get; set; }
        public RelayCommand UpdateSetCommand { get; set; }
        public RelayCommand DeleteSetCommand { get; set; }
        public RelayCommand CancelSetCommand { get; set; }
        public MySetsViewModel MySetsVm => App.Locator.MySetsPageVM;
        #endregion

        public AddSetPageViewModel()
        {
            Credentials = new Set();
            CancelSetCommand = new RelayCommand(CancelAddSet);
            AddSetCommand = new RelayCommand(async () => await AddSet(), IsAddSet);
            UpdateSetCommand = new RelayCommand(async () => await UpdateSet());
            DeleteSetCommand = new RelayCommand(async () => await DeleteSet(Id));

            TokGroup = TokGroupHelper.TokGroups.OrderBy(item => item.TokGroup).ToList();
            //Tok Group
            TokGroupList = new ObservableCollection<string>();
            TokGroupList.Clear();

            for (int i = 0; i < TokGroup.Count(); i++)
            {
                TokGroupList.Add(TokGroup[i].TokGroup);
            }

            PrivacyList = new ObservableCollection<string>();
            PrivacyList.Clear();
            PrivacyList.Add("Public");
            PrivacyList.Add("Private");
        }

        #region Method/Events
        public void CancelAddSet()
        {
            Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets); //After Cancellation, this will go back to the LeftMenu being selected
            Instance.Finish();
        }
        public async Task AddSet()
        {
            IsAddSet = true;

            Credentials.TokTypeId = $"toktype-{Credentials.TokGroup.ToIdFormat()}-{Credentials.TokType.ToIdFormat()}";

            ProgressCircle.IndeterminateDrawable.SetColorFilter(Color.LightBlue, PorterDuff.Mode.Multiply);
            ProgressText.Text = "Saving...";
            LinearProgress.Visibility = ViewStates.Visible;
            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var result = await SharedService.SetService.Instance.CreateSetAsync(Credentials);
            Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);

            LinearProgress.Visibility = ViewStates.Gone;

            builder = new AlertDialog.Builder(Instance);
            builder.SetMessage(result.ResultMessage.ToString());
            builder.SetTitle("");
            dialog = (AlertDialog)null;
            builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
            {
                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    MySetsVm.SetResult.Add(Credentials);
                    MySetsVm.AssignRecyclerAdapter(MySetsVm.SetResult);
                    SetToNew();
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    Instance.Finish();
                }
            });
            dialog = builder.Create();
            dialog.Show();
            if (result.ResultEnum == Shared.Helpers.Result.Success)
            {
                dialog.SetCanceledOnTouchOutside(false);
            }
            IsAddSet = false;
        }
        public async Task UpdateSet()
        {
            IsAddSet = true;

            ProgressCircle.IndeterminateDrawable.SetColorFilter(Color.LightBlue, PorterDuff.Mode.Multiply);
            ProgressText.Text = "Updating...";
            LinearProgress.Visibility = ViewStates.Visible;
            Instance.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var result = await SharedService.SetService.Instance.UpdateSetAsync(Credentials);
            Instance.Window.ClearFlags(WindowManagerFlags.NotTouchable);
            LinearProgress.Visibility = ViewStates.Gone;

            builder = new AlertDialog.Builder(Instance);
            builder.SetMessage("Updated the set!");
            builder.SetTitle("");
            dialog = (AlertDialog)null;
            builder.SetPositiveButton("OK" ?? "OK", (d, index) =>
            {
                if (result.ResultEnum == Shared.Helpers.Result.Success)
                {
                    var detail = MySetsVm.SetResult.FirstOrDefault(c => c.Id == Credentials.Id);
                    if (detail != null)
                    {
                        MySetsVm.SetResult.Remove(detail);
                        MySetsVm.SetResult.Add(Credentials);
                        MySetsVm.AssignRecyclerAdapter(MySetsVm.SetResult);
                    }

                    SetToNew();
                    Settings.ActivityInt = Convert.ToInt16(ActivityType.LeftMenuSets);
                    Instance.Finish();
                }
            });
            dialog = builder.Create();
            dialog.Show();
            if (result.ResultEnum == Shared.Helpers.Result.Success)
            {
                dialog.SetCanceledOnTouchOutside(false);
            }
            IsAddSet = false;
        }
        public async Task DeleteSet(string id)
        {
            await SharedService.SetService.Instance.DeleteSetAsync(id);
        }
        public void SetToNew()
        {
            Credentials = new Set();
        }
        #endregion
    }
}