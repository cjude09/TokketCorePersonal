using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokket.Shared.Models;
using Tokket.Shared.Extensions;
using Tokket.Android.Helpers;
using Android.Graphics;
using Android.Content.Res;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Practice Test", Theme = "@style/CustomAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Practice Test", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class TokPakPracticeTestActivity : BaseActivity
    {
        SpannableStringBuilder ssbName;
        ISpannable spannableResText;
        int roundNo = 0, hintNum = 1, correctGuessedCnt = 0;
        List<ClassTokModel> ListtokModels;
        internal static TokPakPracticeTestActivity Instance { get; private set; }
        List<string> ListDetail, ListAnswers;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_tok_pak_practice_test);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Instance = this;

            var classtokModelString = Intent.GetStringExtra("classtokModel");
            ListtokModels = JsonConvert.DeserializeObject<List<ClassTokModel>>(classtokModelString);
            DisplayRounds();

            var title = Intent.GetStringExtra("TitleActivity");
            Title =  title;
            NextButton.Click += (sender, e) =>
            {
                //Call Next Button Event
                CallNextButtonEvent(sender, e);
            };

            ButtonAnswer1.Click += (sender, e) =>
            {
                if (ButtonAnswer1.Text == LinearContents.ContentDescription)
                {
                    correctGuessedCnt += 1;
                    ShowAlertMessage("Your guess is correct!");
                    CallNextButtonEvent(sender, e);
                }
                else
                {
                    ShowAlertMessage("You have " + (3 - hintNum) + " guesses left!");
                    ShowNextHint();
                    hintNum += 1;
                    ButtonAnswer1.Visibility = ViewStates.Gone;
                }
            };

            ButtonAnswer2.Click += (sender, e) =>
            {
                if (ButtonAnswer2.Text == LinearContents.ContentDescription)
                {
                    correctGuessedCnt += 1;
                    ShowAlertMessage("Your guess is correct!");
                    CallNextButtonEvent(sender, e);
                }
                else
                {
                    ShowAlertMessage("You have " + (3 - hintNum) + " guesses left!");
                    ShowNextHint();
                    hintNum += 1;
                    ButtonAnswer2.Visibility = ViewStates.Gone;
                }
            };

            ButtonAnswer3.Click += (sender, e) =>
            {
                if (ButtonAnswer3.Text == LinearContents.ContentDescription)
                {
                    correctGuessedCnt += 1;
                    ShowAlertMessage("Your guess is correct!");
                    CallNextButtonEvent(sender, e);
                }
                else
                {
                    ShowAlertMessage("You have " + (3 - hintNum) + " guesses left!");
                    ShowNextHint();
                    hintNum += 1;
                    ButtonAnswer3.Visibility = ViewStates.Gone;
                }
            };

            ButtonAnswer4.Click += (sender, e) =>
            {
                if (ButtonAnswer4.Text == LinearContents.ContentDescription)
                {
                    correctGuessedCnt += 1;
                    ShowAlertMessage("Your guess is correct!");
                    CallNextButtonEvent(sender, e);
                }
                else
                {
                    ShowAlertMessage("You have " + (3 - hintNum) + " guesses left!");
                    ShowNextHint();
                    ButtonAnswer4.Visibility = ViewStates.Gone;
                }
            };
        }

        private void DisplayRounds()
        {
            if (roundNo < ListtokModels.Count)
            {
                LoadChoices(ListtokModels[roundNo].Id);
                CategoryDisplay.Text = "Category: " + ListtokModels[roundNo].Category;
                LinearContents.RemoveAllViews();
                if (ListtokModels[roundNo].IsDetailBased)
                {
                    ListAnswers.Add(ListtokModels[roundNo].PrimaryFieldText);
                    LinearContents.ContentDescription = ListtokModels[roundNo].PrimaryFieldText;
                    ListDetail = ListtokModels[roundNo].Details.ToList();
                    if (ListtokModels[roundNo].Details.Length > 3) //Randomize the detail
                    {
                        ListDetail = ListtokModels[roundNo].Details.ToList();
                        ListDetail = ListDetail.Shuffle().ToList();
                    }

                    for (int i = 0; i < ListDetail.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(ListDetail[i]))
                        {
                            View view = LayoutInflater.Inflate(Resource.Layout.tokchoice_detailview, null);
                            TextView TextFrontDisplay = view.FindViewById<TextView>(Resource.Id.lblTokChoiceCardFront);
                            TextFrontDisplay.Tag = i;

                            //Show the back for the first row
                            if (i == 0)
                            {
                                //TextFrontDisplay.Text = ListDetail[i];

                                ssbName = new SpannableStringBuilder(ListDetail[i]);
                                spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                                TextFrontDisplay.SetText(spannableResText, TextView.BufferType.Spannable);
                            }
                            else
                            {
                                TextFrontDisplay.SetText(Html.FromHtml("<font color='#FFFFFF'>Show Hint</font>", FromHtmlOptions.ModeLegacy), TextView.BufferType.Spannable);
                                TextFrontDisplay.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#884bdf"));
                            }

                            view.Tag = i;

                            LinearContents.AddView(view);
                        }
                    }
                }
                else
                {
                    string Guesstext = "";
                    if (ListtokModels[roundNo].TokGroup.ToLower() == "quote")
                    {
                        ListAnswers.Add(ListtokModels[roundNo].SecondaryFieldText);
                        LinearContents.ContentDescription = ListtokModels[roundNo].SecondaryFieldText;
                        Guesstext = ListtokModels[roundNo].PrimaryFieldText;
                    }
                    else
                    {
                        LinearContents.ContentDescription = ListtokModels[roundNo].PrimaryFieldText;
                        ListAnswers.Add(ListtokModels[roundNo].PrimaryFieldText);
                        Guesstext = ListtokModels[roundNo].SecondaryFieldText;
                    }

                    int start = 0;
                    ListDetail = new List<string>();
                    if (string.IsNullOrEmpty(Guesstext))
                        Guesstext = "";
                    var charperline = Guesstext.Length / 3;
                    int guesslength = Guesstext.Length;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 2)
                        {
                            ListDetail.Add(Guesstext.Substring(start, guesslength));
                        }
                        else
                        {
                            ListDetail.Add(Guesstext.Substring(start, charperline));
                        }
                        start += charperline;
                        guesslength -= charperline;
                    }

                    //string[] arrDetails = Guesstext.Split(" ");

                    //ListDetail = arrDetails.ToList();

                    if (ListDetail.Count > 3) //Randomize the detail
                    {
                        ListDetail = ListDetail.Shuffle().ToList();
                    }

                    TextView TextFrontDisplay;
                    for (int i = 0; i < ListDetail.Count; i++)
                    {
                        View view = LayoutInflater.Inflate(Resource.Layout.tokchoice_detailview, null);
                        TextFrontDisplay = view.FindViewById<TextView>(Resource.Id.lblTokChoiceCardFront);
                        TextFrontDisplay.Tag = i;

                        //Show the back for the first row
                        if (i == 0)
                        {
                            //TextFrontDisplay.Text = ListDetail[i];

                            ssbName = new SpannableStringBuilder(ListDetail[i]);
                            spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                            TextFrontDisplay.SetText(spannableResText, TextView.BufferType.Spannable);
                        }
                        else
                        {
                            TextFrontDisplay.SetText(Html.FromHtml("<font color='#FFFFFF'>Show Hint</font>", FromHtmlOptions.ModeLegacy), TextView.BufferType.Spannable);
                            TextFrontDisplay.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor("#884bdf"));
                        }

                        view.Tag = i;
                        LinearContents.AddView(view);

                    }
                }

                ListAnswers = ListAnswers.Shuffle().ToList();
                for (int i = 0; i < ListAnswers.Count; i++)
                {
                    if (i == 0)
                    {
                        //ButtonAnswer1.Text = ListAnswers[0];

                        ssbName = new SpannableStringBuilder(ListAnswers[0]);
                        spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                        ButtonAnswer1.SetText(spannableResText, TextView.BufferType.Spannable);
                    }
                    else if (i == 1)
                    {
                        //ButtonAnswer2.Text = ListAnswers[1];

                        ssbName = new SpannableStringBuilder(ListAnswers[1]);
                        spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                        ButtonAnswer2.SetText(spannableResText, TextView.BufferType.Spannable);
                    }
                    else if (i == 2)
                    {
                        //ButtonAnswer3.Text = ListAnswers[2];
                        ssbName = new SpannableStringBuilder(ListAnswers[2]);
                        spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                        ButtonAnswer3.SetText(spannableResText, TextView.BufferType.Spannable);
                    }
                    else if (i == 3)
                    {
                        //ButtonAnswer4.Text = ListAnswers[3];
                        ssbName = new SpannableStringBuilder(ListAnswers[3]);
                        spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                        ButtonAnswer4.SetText(spannableResText, TextView.BufferType.Spannable);
                    }
                }

                roundNo += 1;

                RoundDisplay.Text = "Tok #" + roundNo + " of " + ListtokModels.Count; //setList.TokIds.Count;
            }
            else
            {
                var alertDiag = new AlertDialog.Builder(this);
                alertDiag.SetTitle("Set Complete");
                alertDiag.SetMessage("You completed " + correctGuessedCnt + " toks in the set.");
                alertDiag.SetPositiveButton(Html.FromHtml("<font color='#dc3545'>Continue</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                {
                    Finish();
                });
                alertDiag.SetNegativeButton(Html.FromHtml("<font color='#007bff'>Redo Set</font>", FromHtmlOptions.ModeLegacy), (senderAlert, args) =>
                {
                    roundNo = 0;
                    hintNum = 1;

                    DisplayRounds();

                    ButtonAnswer1.Visibility = ViewStates.Visible;
                    ButtonAnswer2.Visibility = ViewStates.Visible;
                    ButtonAnswer3.Visibility = ViewStates.Visible;
                    ButtonAnswer4.Visibility = ViewStates.Visible;
                });
                var diag = alertDiag.Create();
                diag.Show();
            }
        }
        private void LoadChoices(string tokid)
        {
            ListAnswers = new List<string>();
            var RandomTokModels = ListtokModels.Shuffle().ToList();

            for (int a = ListAnswers.Count; a < 3; a++)
            {
                try
                {
                    if (RandomTokModels[a].Id != tokid)
                    {
                        if (RandomTokModels[a].IsDetailBased)
                        {
                            if (!string.IsNullOrEmpty(RandomTokModels[a].PrimaryFieldText))
                            {
                                ListAnswers.Add(RandomTokModels[a].PrimaryFieldText);
                            }
                        }
                        else
                        {
                            if (RandomTokModels[a].TokGroup.ToLower() == "quote")
                            {
                                if (!string.IsNullOrEmpty(RandomTokModels[a].SecondaryFieldText))
                                {
                                    ListAnswers.Add(RandomTokModels[a].SecondaryFieldText);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(RandomTokModels[a].PrimaryFieldText))
                                {
                                    ListAnswers.Add(RandomTokModels[a].PrimaryFieldText);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void CallNextButtonEvent(object sender, EventArgs e)
        {
            hintNum = 1;
            DisplayRounds();

            ButtonAnswer1.Visibility = ViewStates.Visible;
            ButtonAnswer2.Visibility = ViewStates.Visible;
            ButtonAnswer3.Visibility = ViewStates.Visible;
            ButtonAnswer4.Visibility = ViewStates.Visible;
        }
        private void ShowAlertMessage(string alertmssg = "")
        {
            var mssgDialog = new AlertDialog.Builder(this);
            var alertMssg = mssgDialog.Create();
            alertMssg.SetTitle("");
            alertMssg.SetMessage(alertmssg);
            alertMssg.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), (d, fv) => { });
            alertMssg.Show();
            alertMssg.SetCancelable(false);
        }

        [Java.Interop.Export("OnShowHint")]
        public void OnTapViewFlipperTokBack(View v)
        {
            var TextFrontDisplay = v.FindViewById<TextView>(Resource.Id.lblTokChoiceCardFront);
            int vtag = (int)TextFrontDisplay.Tag;

            if (vtag > 0)
            {
                TextFrontDisplay.Text = ListDetail[(int)TextFrontDisplay.Tag];
                TextFrontDisplay.BackgroundTintList = null;
            }
        }

        private void ShowNextHint()
        {
            View v = LinearContents.GetChildAt(hintNum);
            if (v != null)
            {
                var TextFrontDisplay = v.FindViewById<TextView>(Resource.Id.lblTokChoiceCardFront);
                int vtag = (int)TextFrontDisplay.Tag;

                //TextFrontDisplay.Text = ListDetail[(int)TextFrontDisplay.Tag];
                ssbName = new SpannableStringBuilder(ListDetail[(int)TextFrontDisplay.Tag]);
                spannableResText = SpannableHelper.AddStickersSpannable(this, ssbName);

                TextFrontDisplay.SetText(spannableResText, TextView.BufferType.Spannable);

                TextFrontDisplay.BackgroundTintList = null;
            }
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
        #region Properties
        public TextView RoundDisplay => this.FindViewById<TextView>(Resource.Id.lblTokChoiceRoundDisplay);
        public TextView CategoryDisplay => this.FindViewById<TextView>(Resource.Id.TokChoiceCategory);
        public Button NextButton => this.FindViewById<Button>(Resource.Id.BtnTokChoiceNext);
        public ProgressBar CircleProgress => this.FindViewById<ProgressBar>(Resource.Id.circleprogressTokChoiceDetails);
        public LinearLayout LinearContents => this.FindViewById<LinearLayout>(Resource.Id.LinearTokChoiceContents);
        public Button ButtonAnswer1 => this.FindViewById<Button>(Resource.Id.BtnTokChoiceAnswer1);
        public Button ButtonAnswer2 => this.FindViewById<Button>(Resource.Id.BtnTokChoiceAnswer2);
        public Button ButtonAnswer3 => this.FindViewById<Button>(Resource.Id.BtnTokChoiceAnswer3);
        public Button ButtonAnswer4 => this.FindViewById<Button>(Resource.Id.BtnTokChoiceAnswer4);
        public TextView TextSubtitle => this.FindViewById<TextView>(Resource.Id.TextSubtitle);
        #endregion
    }
}