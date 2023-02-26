using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Tokket.Shared.Models;
using Tokket.Shared.Helpers;
using Android.Graphics;
using Tokket.Core;
using Tokket.Shared.Services;
using Tokket.Shared.Models.Chat;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using Tokket.Android.Helpers;
using AndroidX.ConstraintLayout.Widget;

namespace Tokket.Android.Fragments
{
    public class ClassGroupTokPakFragment : AndroidX.Fragment.App.Fragment
    {
        ClassGroupModel classGroup;
        View v;
        internal static ClassGroupTokPakFragment Instance { get; private set; }
        ObservableCollection<TokModel> TokModelCollection { get; set; }
        ObservableCollection<TokPak> TokPakCollection { get; set; }
        Bundle bundle = new Bundle();
        private int REQUEST_ACTIVITY_PREVIEW = 1001;
        private Intent ParentIntent;
        private ClassGroupActivity Context;
        public ClassGroupTokPakFragment(ClassGroupModel _classGroup, Intent parent, ClassGroupActivity context)
        {
            classGroup = _classGroup;
            ParentIntent = parent;
            Context = context;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public void LoadTokPaK() {
            TokPakCollection.Clear();
            Context.RunOnUiThread(async () => await Initialize(isPublicFeed: false));

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            v = inflater.Inflate(Resource.Layout.group_view_tok_pack, container, false);

            Instance = this;

            TokModelCollection = new ObservableCollection<TokModel>();
            var mLayoutManager = new GridLayoutManager(Context, 3);


            int numcol = 1;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                numcol = (int)NumDisplay.tablet + 1;
            }

            bundle = new Bundle();
            TokPakCollection = new ObservableCollection<TokPak>();
            recyclerPages.SetLayoutManager(new GridLayoutManager(Context, numcol));
            AddTokPakButton.Click += OnClickAddTokPakGroup;
            Context.RunOnUiThread(async () => await Initialize(isPublicFeed: false));
            return v;
        }

        private void OnClickAddTokPakGroup(object sender, EventArgs e)
        {
            var intent = new Intent(Context, typeof(AddTokPakActivity));
            var group = JsonConvert.SerializeObject(classGroup);
            intent.PutExtra("GroupId", group);
            StartActivity(intent);
        }

        private async Task Initialize(string paginationToken = "", bool isPublicFeed = false)
        {
            //  showProgress();



            var result = await TokPakService.Instance.GetTokPaksAsync(new TokPakQueryValues()
            {
                paginationid = paginationToken,
                userid = isPublicFeed ? "" : Settings.GetUserModel().UserId,
                groupid = classGroup.Id

            });
            if (result != null) {
                foreach (var item in result.Results.ToList())
                {

                    TokPakCollection.Add(item);
                }
            }

          

            if (TokPakCollection.Count == 0)
            {
                linearNoTokPak.Visibility = ViewStates.Visible;
                linearPage.Visibility = ViewStates.Gone;
            }
            else {
                linearNoTokPak.Visibility = ViewStates.Gone;
                linearPage.Visibility = ViewStates.Visible;
            }
            // hideProgress();

            setRecyclerPagesAdapter();

            BtnAll.Click -= (s, e) => { };
            BtnAll.Click += (s, e) => {
                Context.RunOnUiThread(async () => await Initialize(isPublicFeed: false));
            };
        }


        private void setRecyclerPagesAdapter()
        {
            var adapterDetail = TokPakCollection.GetRecyclerAdapter(BindClassTok, Resource.Layout.activity_view_tok_pak_row);
            recyclerPages.SetAdapter(adapterDetail);
        }

        private void BindClassTok(CachingViewHolder holder, TokPak model, int position)
        {
            var imageUserPhoto = holder.FindCachedViewById<ImageView>(Resource.Id.imageUserPhoto);
            var txtNoPages = holder.FindCachedViewById<TextView>(Resource.Id.txtNoPages);
            var txtDate = holder.FindCachedViewById<TextView>(Resource.Id.txtDate);
            var txtUserDisplayName = holder.FindCachedViewById<TextView>(Resource.Id.txtUserDisplayName);
            var txtUserTitle = holder.FindCachedViewById<TextView>(Resource.Id.txtUserTitle);
            var viewHeaderBackground = holder.FindCachedViewById<View>(Resource.Id.viewHeaderBackground);
            var constraintParent = holder.FindCachedViewById<ConstraintLayout>(Resource.Id.constraintParent);
            var txtCategory = holder.FindCachedViewById<TextView>(Resource.Id.txtCategory);
            var txtHeader = holder.FindCachedViewById<TextView>(Resource.Id.txtHeader);
            var txtItem = holder.FindCachedViewById<TextView>(Resource.Id.txtItem);
            var btnPresentation = holder.FindCachedViewById<Button>(Resource.Id.btnPageType);
            var btnManage = holder.FindCachedViewById<Button>(Resource.Id.btnManage);

            txtCategory.Text = model.Category;
            holder.ItemView.Tag = position;
            txtHeader.SetTextColor(Color.White);
            txtUserDisplayName.Text = model.UserDisplayName;
            txtUserTitle.Text = model.TitleDisplay;
            txtNoPages.Text = "# of pages: " + model.ClassToks.Count().ToString();
            txtDate.Text = model.CreatedTime.ToString("MM/dd/yyyy");

            var userPhoto = model.UserPhoto;

            Glide.With(holder.ItemView).Load(userPhoto).Thumbnail(0.05f).Transition(DrawableTransitionOptions.WithCrossFade()).Apply(RequestOptions.PlaceholderOf(Resource.Animation.loader_animation).Error(Resource.Drawable.no_image)).Into(imageUserPhoto);

            if (btnPresentation != null) {
                switch (model.Type)
                {
                    case TokPakType.Paper:
                        txtHeader.Text = "Tok Pak - Paper";
                        txtHeader.SetBackgroundColor(Color.AliceBlue);
                        txtHeader.SetTextColor(Color.Black);
                        btnPresentation.Text = "Paper";
                        break;
                    case TokPakType.Presentation:
                        txtHeader.Text = "Tok Pak - Presentation";
                        btnPresentation.Text = "Presentation";
                        txtHeader.SetBackgroundColor(Color.Pink);
                        txtHeader.SetTextColor(Color.White);
                        break;
                    case TokPakType.PracticeTest:
                        txtHeader.Text = "Tok Pak - Practice Test";
                        btnPresentation.Text = "Practice Test";
                        txtHeader.SetBackgroundColor(Color.Yellow);
                        txtHeader.SetTextColor(Color.White);
                        break;
                    default:
                        break;
                }

                btnPresentation.Tag = position;
                btnPresentation.Click -= onClickPreview;
                btnPresentation.Click += onClickPreview;
            }
            txtHeader.SetTextColor(Color.White);
            switch (model.Type)
            {
                case TokPakType.Paper:
                    viewHeaderBackground.SetBackgroundColor(Color.AliceBlue);
                    txtHeader.SetTextColor(Color.Black);
                    break;
                case TokPakType.Presentation:
                    viewHeaderBackground.SetBackgroundColor(Color.Pink);
  
                    break;
                case TokPakType.PracticeTest:
                    viewHeaderBackground.SetBackgroundColor(Color.Yellow);
         
                    break;
                default:
                    break;
            }
            for (int i = 0; i < model.ClassToks.Count; i++)
            {
                if (i == 0)
                {
                    txtItem.Text = "• " + model.ClassToks[i].PrimaryFieldText.ToString();
                }
                else
                {
                    txtItem.Text += "\n• " + model.ClassToks[i].PrimaryFieldText.ToString();
                }
            }


            btnManage.Click -= (s, e) => { };
            btnManage.Click += (s, e) => {
                var menu = new PopupMenu(this.Context, btnManage);

                // Call inflate directly on the menu:
                menu.Inflate(Resource.Menu.classgroup_tokpak_menu);
                var edit = menu.Menu.FindItem(Resource.Id.itemEdit);
                var delete = menu.Menu.FindItem(Resource.Id.itemDelete);


                menu.MenuItemClick += async (obj, _event) => {
                    switch (_event.Item.TitleFormatted.ToString().ToLower())
                    {
                        case "edit":
                            var intent = new Intent(Context, typeof(AddTokPakActivity));
                            var group = JsonConvert.SerializeObject(classGroup);
                            intent.PutExtra("tokPak", JsonConvert.SerializeObject(TokPakCollection[position]));
                            intent.PutExtra("GroupId", group);
                            intent.PutExtra("isSave", false);
                            StartActivity(intent);

                            break;
                        case "delete":
                            ClassGroupActivity.Instance.showBlueLoading(ClassGroupActivity.Instance);
                            var result = await TokPakService.Instance.DeleteTokPakAsync(TokPakCollection[position].Id, TokPakCollection[position].PartitionKey);
                            ClassGroupActivity.Instance.hideBlueLoading(ClassGroupActivity.Instance);

                            var message = "Success!";
                            if (!result)
                                message = "Failed to delete!";

                            ClassGroupActivity.Instance.ShowLottieMessageDialog(ClassGroupActivity.Instance, message, result, handlerOkClick: (s, e) =>
                            {
                                TokPakCollection.Remove(TokPakCollection[position]);
                                setRecyclerPagesAdapter();
                            });
                            break;
                    }
                };

                menu.DismissEvent += (s1, _event) => {

                };

                menu.Show();
            };
         
        }

        private void onClickPreview(object sender, EventArgs e)
        {
            int position = (int)(sender as View).Tag;
            bundle.PutInt("position", position);
            Intent nextActivity = new Intent(Context, typeof(TokPakPreviewActivity));
            var modelConvert = JsonConvert.SerializeObject(TokPakCollection[position]);
            nextActivity.PutExtra("tokPak", modelConvert);
            nextActivity.PutExtra("classGroupModel", JsonConvert.SerializeObject(classGroup));
            this.StartActivityForResult(nextActivity, REQUEST_ACTIVITY_PREVIEW);
        }



        public Button btnPresentation => v.FindViewById<Button>(Resource.Id.btnPresentation);
        public Button btnPapers => v.FindViewById<Button>(Resource.Id.btnPapers);
        public Button btnPracticeTest => v.FindViewById<Button>(Resource.Id.btnPracticeTest);
        public Button BtnAll => v.FindViewById<Button>(Resource.Id.btnAll);

        public RecyclerView recyclerPages => v.FindViewById<RecyclerView>(Resource.Id.recyclerPages);
        public LinearLayout linearProgress => v.FindViewById<LinearLayout>(Resource.Id.linearProgress);
        public ProgressBar progressBarCircle => v.FindViewById<ProgressBar>(Resource.Id.progressBarCircle);
        public TextView txtProgress => v.FindViewById<TextView>(Resource.Id.txtProgress);

        public LinearLayout linearPage => v.FindViewById<LinearLayout>(Resource.Id.linearPages);
        public LinearLayout linearNoTokPak => v.FindViewById<LinearLayout>(Resource.Id.linearNoTokPak);

        public Button AddTokPakButton => v.FindViewById<Button>(Resource.Id.addtokpakgroup);
    }
}