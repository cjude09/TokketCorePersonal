using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Newtonsoft.Json;
using Tokket.Android.Fragments;
using Tokket.Shared.Models;
using Tokket.Core;
using Tokket.Shared.Helpers;
using Android.Text;
using Android.Content.PM;
using Tokket.Android.ViewHolders;
using Result = Android.App.Result;
using NetUri = Android.Net.Uri;
using AndroidX.RecyclerView.Widget;
using static Android.App.ActionBar;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Add Game Set", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Add Game Set", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

    public class AddGameSetActivity : BaseActivity
    {
        char[] punctuation = new char[] { '?', '!', '.', ',', '-', '—', ';', ':', ')' };
        internal static AddGameSetActivity Instance { get; private set; }
        string delimiters = "-.,!?:;'\""; //Punctuation
        Dialog popupDialog;
        EditText editCategory, editQuote, editMeaning;
        TextView txtCategoryError, txtMeaningError, txtQuoteError;
        ObservableCollection<QuotesModel> QuotesCollection { get; set; }
        ObservableRecyclerAdapter<QuotesModel, CachingViewHolder> adapterQuotes;
        ObservableCollection<QuotesModel> WordsCollection { get; set; }
        ObservableRecyclerAdapter<QuotesModel, CachingViewHolder> adapterWordsQuote;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_addgameset);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;
            Settings.ActivityInt = (int)ActivityType.AddGameSetActivity;

            QuotesCollection = new ObservableCollection<QuotesModel>();
            var mLayoutManager = new GridLayoutManager(this, 1);
            RecyclerQuotes.SetLayoutManager(mLayoutManager);

            this.Title = Intent.GetStringExtra("buttonTapped");
            TxtGameName.Text = Intent.GetStringExtra("gamename");
            TxtDescription.Text = Intent.GetStringExtra("gamedescription");

            BtnAddQuote.Click += BtnAddEditQuote_Click;

            BtnBrowse.Click += delegate
            {
                BottomSheetUserPhotoFragment bottomsheet = new BottomSheetUserPhotoFragment(this, ImgGameSet);
                bottomsheet.Show(this.SupportFragmentManager, "tag");
            };
        }

        private void BindQuotesViewHolder(CachingViewHolder holder, QuotesModel quotes, int position)
        {
            var txtQuote = holder.FindCachedViewById<TextView>(Resource.Id.txtQuote);
            var txtMeaning = holder.FindCachedViewById<TextView>(Resource.Id.txtMeaning);
            var btnEdit = holder.FindCachedViewById<Button>(Resource.Id.btnEdit);
            var btnDelete = holder.FindCachedViewById<Button>(Resource.Id.btnDelete);

            txtQuote.Text = quotes.Quotes;
            txtMeaning.Text = quotes.Meaning;

            var font = Typeface.CreateFromAsset(Application.Assets, "fa_solid_900.otf");
            btnEdit.Typeface = font;
            btnDelete.Typeface = font;

            btnEdit.Tag = position;
            btnDelete.Tag = position;

            btnDelete.Click -= BtnDeleteQuote_Click;
            btnDelete.Click += BtnDeleteQuote_Click;

            btnEdit.Click -= BtnAddEditQuote_Click;
            btnEdit.Click += BtnAddEditQuote_Click;
        }
        private void BtnDeleteQuote_Click(object sender, EventArgs e)
        {
            int position = 0;
            try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }

            QuotesCollection.RemoveAt(position);

            adapterQuotes = QuotesCollection.GetRecyclerAdapter(BindQuotesViewHolder, Resource.Layout.quotes_item_row);
            RecyclerQuotes.SetAdapter(adapterQuotes);
        }
        private void BtnAddEditQuote_Click (object sender, EventArgs e)
        {
            int position = 0; bool isAdd = true;
            popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.addquote_dialog);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog.Show();

            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            popupDialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
            popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);

            // Access Popup layout fields like below  
            editQuote = popupDialog.FindViewById<EditText>(Resource.Id.editQuote);
            txtQuoteError = popupDialog.FindViewById<TextView>(Resource.Id.txtQuoteError);
            editMeaning = popupDialog.FindViewById<EditText>(Resource.Id.editMeaning);
            txtMeaningError = popupDialog.FindViewById<TextView>(Resource.Id.txtMeaningError);
            editCategory = popupDialog.FindViewById<EditText>(Resource.Id.editCategory);
            txtCategoryError = popupDialog.FindViewById<TextView>(Resource.Id.txtCategoryError);
            var btnAddQuote = popupDialog.FindViewById<Button>(Resource.Id.btnAddQuote);
            var btnCancel = popupDialog.FindViewById<Button>(Resource.Id.btnCancel);
            var btnPreview = popupDialog.FindViewById<Button>(Resource.Id.btnPreview);
            if ((sender as Button).Tag != null) //Edit
            {
                isAdd = false;
                try { position = (int)(sender as Button).Tag; } catch { position = int.Parse((string)(sender as Button).Tag); }
                editQuote.Text = QuotesCollection[position].Quotes;
                editMeaning.Text = QuotesCollection[position].Meaning;
                editCategory.Text = QuotesCollection[position].Category;
                btnAddQuote.Text = "Update Quote";
            }

            btnPreview.Click += delegate
            {
                ShowPreview();
            };

            // Events for that popup layout  
            int quoteError = 0;
            btnAddQuote.Click += delegate
            {
                editQuote.Text = editQuote.Text.Trim();
                editQuote.SetSelection(editQuote.Text.Length);

                quoteError = CheckQuote();
                if (quoteError == 0)
                {
                    txtQuoteError.Visibility = ViewStates.Gone;
                }

                if (quoteError == 1)
                {
                    txtQuoteError.Text = "Quote must have at least 3 words and no more than 28 words.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }
                else if (quoteError == 2)
                {
                    txtQuoteError.Text = "The quote can have no word greater than 18 characters.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }
                else if (quoteError == 3)
                {
                    txtQuoteError.Text = "Word must consist of only the 52 English alphabet characters Aa-Zz.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }
                else if (CheckDuplicatePunctuation(editQuote.Text) == 4)
                {
                    txtQuoteError.Text = "The quote cannot have more than one punctuation/special character.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }
                else if (quoteError == 5)
                {
                    txtQuoteError.Text = "The quote cannot start with punctuation or a special character.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }
                else if (quoteError == 6)
                {
                    txtQuoteError.Text = "Needs to have at least one 3 to 12 letter word to be valid.";
                    txtQuoteError.Visibility = ViewStates.Visible;
                }

                if (editMeaning.Text.Length > 1 && editMeaning.Text.Length < 26)
                {
                }
                else
                {
                    quoteError = 33;
                    txtMeaningError.Text = "Must be between 2 and 25 characters long.";
                    txtMeaningError.Visibility = ViewStates.Visible;
                }

                if (editCategory.Text.Length > 1 && editCategory.Text.Length < 26)
                {
                }
                else
                {
                    quoteError = 33;
                    txtCategoryError.Text = "Must be between 2 and 25 characters long.";
                    txtCategoryError.Visibility = ViewStates.Visible;
                }

                if (quoteError == 0) //proceed Add Quote
                {
                    QuotesModel quotesModel = new QuotesModel();
                    quotesModel.Quotes = editQuote.Text;
                    quotesModel.Meaning = editMeaning.Text;
                    quotesModel.Category = editCategory.Text;

                    if (isAdd)
                    {
                        QuotesCollection.Add(quotesModel);
                    }
                    else
                    {
                        QuotesCollection.RemoveAt(position);
                        QuotesCollection.Insert(position,quotesModel);
                    }

                    adapterQuotes = QuotesCollection.GetRecyclerAdapter(BindQuotesViewHolder, Resource.Layout.quotes_item_row);
                    RecyclerQuotes.SetAdapter(adapterQuotes);

                    popupDialog.Dismiss();
                }

            };

            btnCancel.Click += delegate
            {
                popupDialog.Dismiss();
            };
        }
        private void ShowPreview()
        {
            var popupPreviewDialog = new Dialog(this);
            popupPreviewDialog.SetContentView(Resource.Layout.addquotepreview_dialog);
            popupPreviewDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupPreviewDialog.Show();
            popupPreviewDialog.SetCanceledOnTouchOutside(true);
            popupPreviewDialog.SetCancelable(true);

            // Some Time Layout width not fit with windows size  
            // but Below lines are not necessary  
            double widthD = getLayoutWidth();
            popupPreviewDialog.Window.SetLayout(int.Parse((widthD * 0.90).ToString()), LayoutParams.WrapContent);
            popupPreviewDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.transparent);
            var recyclerWords = popupPreviewDialog.FindViewById<RecyclerView>(Resource.Id.recyclerWords);

            WordsCollection = new ObservableCollection<QuotesModel>();
            var mLayoutQuoteManager = new GridLayoutManager(this, 4);
            recyclerWords.SetLayoutManager(mLayoutQuoteManager);
            
            string[] totalwords = editQuote.Text.Split(" ");
            for (int i = 0; i < totalwords.Length; i++)
            {
                if (i >= 29)
                    break;

                //Check if word contains a char in punctuation
                bool containsPunc = false;
                char chartype = ' ';
                foreach(Char x in punctuation)
                {
                    if (totalwords[i].Contains(x))
                    {
                        chartype = x;
                        containsPunc = true;
                        break;
                    }
                }

                if (containsPunc)
                {
                    int cntx = 0;
                    string[] charsplit = totalwords[i].Split(chartype);
                    foreach (var item in charsplit)
                    {
                        QuotesModel qm = new QuotesModel();
                        qm.Quotes = (cntx == 0) ? (item + chartype) : item;
                        WordsCollection.Add(qm);
                        cntx += 1;
                    }
                }
                else
                {
                    QuotesModel qm = new QuotesModel();
                    qm.Quotes = totalwords[i].ToString();
                    WordsCollection.Add(qm);
                }
            }

            adapterWordsQuote = WordsCollection.GetRecyclerAdapter(BindQuoteWordsViewHolder, Resource.Layout.layout_dummybtn_quote);
            recyclerWords.SetAdapter(adapterWordsQuote);
        }

        private void BindQuoteWordsViewHolder(CachingViewHolder holder, QuotesModel quotes, int position)
        {
            var txtWordss = holder.FindCachedViewById<Button>(Resource.Id.btnWordDummy);
            
            if (quotes.Quotes.Length > 18)
            {
                string getText = quotes.Quotes.Substring(0, 18);
                txtWordss.Text = getText + "..."; //programmatically add ... at the end because maxlength will not word in ellipsize = end
            }
            else
            {
                txtWordss.Text = quotes.Quotes;
            }
        }

        private int CheckQuote()
        {
            int errorcode = 0;
            string[] totalwords = editQuote.Text.Split(" ");
            if (totalwords.Length >= 3 && totalwords.Length <= 28)
            {
            }
            else
            {
                return errorcode = 1;
            }

            for (int i = 0; i < totalwords.Length; i++)
            {
                if (totalwords[i].Length > 18)
                {
                    return errorcode = 2;
                }
                else if (totalwords[i].Length > 2 && totalwords[i].Length < 13)
                {
                    Match match = Regex.Match(totalwords[i].ToString(), "[a-zA-Z]+");
                    if (match.Success)
                    {
                    }
                    else
                    {
                        return errorcode = 3;
                    }
                }
            }

            if (totalwords.Length > 0)
            {
                if (delimiters.Contains(totalwords[0].Substring(0, 1)))
                {
                    return errorcode = 5;
                }
            }

            //Needs to have at least one 3 to 12 letter word to be valid
            bool isNotValid = false;
            for (int i = 0; i < totalwords.Length; i++)
            {
                if (totalwords[i].Length > 2 && totalwords[i].Length < 13)
                {
                    isNotValid = false;
                    break;
                    // valid
                }
                else
                {
                    isNotValid = true;
                }
            }
            if (isNotValid)
            {
                errorcode = 6;
            }

            return errorcode;
        }
        private int CheckDuplicatePunctuation(string source)
        {
            int errorcode = 0;
            HashSet<string> words = new HashSet<string>();

            string[] totalwords = source.Split(" ");

            for (int i = 0; i < totalwords.Length; i++)
            {
                words.Add(totalwords[i]);
            }

            int dashcnt = 0, periodcnt = 0, commacnt = 0, exclamcnt = 0, questioncnt = 0, doubledotcnt = 0, coloncnt = 0, doublequotecnt = 0;
            foreach (string word in words)
            {
                dashcnt = 0; periodcnt = 0; commacnt = 0; exclamcnt = 0; questioncnt = 0; doubledotcnt = 0; coloncnt = 0; doublequotecnt = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    switch (word[i])
                    {
                        case '-':
                            dashcnt += 1;
                            break;
                        case '.':
                            periodcnt += 1;
                            break;
                        case ',':
                            commacnt += 1;
                            break;
                        case '!':
                            exclamcnt += 1;
                            break;
                        case '?':
                            questioncnt += 1;
                            break;
                        case ':':
                            doubledotcnt += 1;
                            break;
                        case ';':
                            coloncnt += 1;
                            break;
                        case '\"':
                            doublequotecnt += 1;
                            break;
                    }
                }
            }

            if (dashcnt > 1 || periodcnt > 1 || commacnt > 1 || exclamcnt > 1 || questioncnt > 1 || doubledotcnt > 1 || coloncnt > 1 || doublequotecnt > 1)
            {
                errorcode = 4;
            }
            return errorcode;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == (int)ActivityType.AddGameSetActivity) && (resultCode == Result.Ok) && (data != null))
            {
                NetUri uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }
            else if ((requestCode == (int)ActivityType.AvatarsActivity) && (resultCode == Result.Ok)) //Avatar
            {
                var avatarString = data.GetStringExtra("Avatar");
                var avatarModel = JsonConvert.DeserializeObject<Avatar>(avatarString);
                Glide.With(this).Load(avatarModel.Image).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(ImgGameSet);
            }
        }

        public void displayImageBrowse()
        {
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            ImgGameSet.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            Settings.ImageBrowseCrop = null;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case HomeId:
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        public TextView TxtGameName => FindViewById<TextView>(Resource.Id.txtGameName);
        public TextView TxtDescription => FindViewById<TextView>(Resource.Id.txtDescription);
        public EditText EditCategory => FindViewById<EditText>(Resource.Id.editCategory);
        public ImageView ImgGameSet => FindViewById<ImageView>(Resource.Id.imgGame);
        public Button BtnBrowse => FindViewById<Button>(Resource.Id.btnBrowse);
        public Button BtnAddQuote => FindViewById<Button>(Resource.Id.btnAddQuote);
        public RecyclerView RecyclerQuotes => FindViewById<RecyclerView>(Resource.Id.recyclerQuotes);
        public Button BtnAddGameSet => FindViewById<Button>(Resource.Id.btnAddGameSet);
    }
}