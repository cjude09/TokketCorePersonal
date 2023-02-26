using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokket.Android.TokQuest
{
    [Activity(Label = "TokQuestMain")]
    public class TokQuestMain : BaseActivity
    {
        int pageNumber = 1; string optionSelected = ""; List<string> selectedOptionList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TokQuest_mainpage);

            selectedOptionList = new List<string>();
            pageNumber = 1; //Setting page as #1 will be the main page

            btnJoin.Click += btn_click;
            btnHostGame.Click += btn_click;
            btnCreateGame.Click += btn_click;
            btnViewSets.Click += btn_click;
            btnCancelGame.Click += BtnCancelGame_Click;              
        }

        private void showPageUI()
        {
            btnCancelGame.Visibility = ViewStates.Visible;

            //Set back to original colors
            btnJoin.SetBackgroundColor(Color.ParseColor("#92D050"));
            btnHostGame.SetBackgroundColor(Color.ParseColor("#0070C0"));
            btnCreateGame.SetBackgroundColor(Color.ParseColor("#dc3545"));
            btnViewSets.SetBackgroundColor(Color.ParseColor("#ffc107"));

            switch (pageNumber)
            {
                case 2:
                    if (optionSelected == "join game")
                    {
                        btnJoin.Visibility = ViewStates.Visible;
                        btnJoin.Text = "JOIN VIA GROUP";

                        btnHostGame.Visibility = ViewStates.Visible;
                        btnHostGame.Text = "JOIN VIA CODE";

                        btnCreateGame.Visibility = ViewStates.Gone;
                        btnViewSets.Visibility = ViewStates.Gone;
                    }
                    else if (optionSelected == "host game")
                    {
                        btnJoin.Visibility = ViewStates.Visible;
                        btnJoin.Text = "HOST FOR GROUP";

                        btnHostGame.Visibility = ViewStates.Visible;
                        btnHostGame.Text = "HOST PUBLICLY";

                        btnCreateGame.Visibility = ViewStates.Gone;
                        btnViewSets.Visibility = ViewStates.Gone;
                    }
                    else if (optionSelected == "create game")
                    {
                        btnJoin.Visibility = ViewStates.Visible;
                        btnJoin.Text = "CREATE NEW GAME";

                        btnHostGame.Visibility = ViewStates.Visible;
                        btnHostGame.Text = "CONVERT SET TO GAME";

                        btnCreateGame.Visibility = ViewStates.Gone;
                        btnViewSets.Visibility = ViewStates.Gone;
                    }
                    else if (optionSelected == "view sets")
                    {
                        btnJoin.Visibility = ViewStates.Visible;
                        btnJoin.Text = "VIEW PUBLIC SETS";

                        btnHostGame.Visibility = ViewStates.Visible;
                        btnHostGame.Text = "VIEW PRIVATE SETS";

                        btnCreateGame.Visibility = ViewStates.Visible;
                        btnCreateGame.Text = "VIEW DRAFTS";

                        btnJoin.SetBackgroundColor(Color.ParseColor("#28a745"));
                        btnHostGame.SetBackgroundColor(Color.ParseColor("#007bff"));
                        btnCreateGame.SetBackgroundColor(Color.ParseColor("#800080"));
                    }
                    else
                    {
                        //If none of the above
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color
                    }
                    break;
                case 3:
                    if (optionSelected == "join via group" || optionSelected == "host for group")
                    {
                        btnJoin.Visibility = ViewStates.Visible;
                        btnJoin.Text = "SELECT CLASS GROUP";

                        btnJoin.SetBackgroundColor(Color.ParseColor("#0070c0"));

                        btnHostGame.Visibility = ViewStates.Gone;
                        btnCreateGame.Visibility = ViewStates.Gone;
                        btnViewSets.Visibility = ViewStates.Gone;
                    }
                    else if(optionSelected == "join via code")
                    {
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color
                        showViaCodeDialog();
                    }
                    else if (optionSelected == "create new game")
                    {
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color

                        Intent intent = new Intent(this, typeof(CreateGameSetActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        //If none of the above
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color
                    }
                    break;
                case 4:
                    if (optionSelected == "select class group")
                    {
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color

                        Intent intent = new Intent(this, typeof(TokquestClassgroupActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        //If none of the above
                        removePrevious(); //Call removePrevious if it opens a different Activity
                        showPageUI(); //Show page UI so it won't change color
                    }
                    break;
                default:
                    //Default will be the main page
                    btnJoin.Visibility = ViewStates.Visible;
                    btnJoin.Text = "JOIN GAME";

                    btnHostGame.Visibility = ViewStates.Visible;
                    btnHostGame.Text = "HOST GAME";

                    btnCreateGame.Visibility = ViewStates.Visible;
                    btnCreateGame.Text = "CREATE GAME";

                    btnViewSets.Visibility = ViewStates.Visible;
                    btnViewSets.Text = "VIEW SETS";

                    btnCancelGame.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void BtnCancelGame_Click(object sender, EventArgs e)
        {
            removePrevious();
            showPageUI();
        }

        private void removePrevious()
        {
            if (selectedOptionList.Count() > 0)
            {
                //Remove last row
                selectedOptionList.RemoveAt(selectedOptionList.Count() - 1);
            }

            //Get the latest last row
            if (selectedOptionList.Count() > 0)
            {
                //Remove last row
                optionSelected = selectedOptionList[selectedOptionList.Count() - 1];
            }

            pageNumber -= 1;
        }

        private void btn_click(object sender, EventArgs e)
        {
            if (pageNumber == 1)
            {
                optionSelected = ""; //reset to nothing
            }

            if (optionSelected != (sender as Button).Text.ToLower())
            {
                pageNumber += 1;
                optionSelected = (sender as Button).Text.ToLower();
                selectedOptionList.Add(optionSelected);
                showPageUI();
            }
        }

        private void showViaCodeDialog()
        {
            var codeDialog = new Dialog(this);
            codeDialog.SetContentView(Resource.Layout.dialog_tok_quest_code);
            codeDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            codeDialog.Show();

            // Access Popup layout fields like below  
            var txtCode = codeDialog.FindViewById<EditText>(Resource.Id.txtCode);
            var btnCancel = codeDialog.FindViewById<Button>(Resource.Id.btnCancel);
            var btnEnterCode = codeDialog.FindViewById<Button>(Resource.Id.btnEnterCode);


            btnEnterCode.Click += delegate { 
                
            
            };

            // Events for that popup layout  
            btnCancel.Click += delegate
            {
                codeDialog.Dismiss();
            };
        }

        public Button btnCreateGame => FindViewById<Button>(Resource.Id.btnCreateGame);
        public Button btnViewSets => FindViewById<Button>(Resource.Id.btnViewSets);
        public Button btnJoin => FindViewById<Button>(Resource.Id.btnJoinGame);
        public Button btnHostGame => FindViewById<Button>(Resource.Id.btnHostGame);
        public Button btnCancelGame => FindViewById<Button>(Resource.Id.btnCancel);
    }
}