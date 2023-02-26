using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.RecyclerView.Widget;
using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Android.Helpers;
using Tokket.Android.ViewHolders;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services;
using Result = Android.App.Result;
using Settings = Tokket.Shared.Helpers.Settings;

namespace Tokket.Android
{
#if (_TOKKEPEDIA)
    [Activity(Label = "Create Game Set", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode  | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif

#if (_CLASSTOKS)
    [Activity(Label = "Create Game Set", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class CreateGameSetActivity : BaseActivity
    {
        private int REQUEST_BROWSE_IMAGE = 1000;
        int questionPage = 0;
        int pageNumber = 1;
        GameSetModel gameSetModel;
        List<GameSetModel> gameSetList;
        internal static CreateGameSetActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_create_game_set);

            gameSetModel = new GameSetModel();
            gameSetList = new List<GameSetModel>();
            RecyclerDetail.SetLayoutManager(new LinearLayoutManager(this));

            pageNumber = 1;

            Instance = this;
            loadGameType();
            loadQuestionType();
            loadPageUI();

            btnCancel.Click += delegate
            {
                if (pageNumber > 1)
                {
                    //Go back to first page
                    pageNumber = 1;
                    loadPageUI();
                }
                else
                {
                    //If first page
                    Finish();
                }
            };

            btnContinue.Click += async(s, e) =>
            {
                if (questionPage  > 0)
                {
                    //Create game set
                    if (checkToProceed())
                    {
                        await CreateGameSet();
                    }
                }
                else
                {
                    btn_previous.Enabled = false;

                    gameSetModel.Category = txtCategory.Text;
                    gameSetModel.GameTitle = txtTitle.Text;
                }

                pageNumber += 1;
                loadPageUI();
            };

            btn_previous.Click += delegate
            {
                rememberAnswers();
                questionPage -= 1;
                pageNumber -= 1;
                loadQuestionsUI();
            };

            btn_next.Click += delegate
            {
                if (checkToProceed())
                {
                    rememberAnswers();
                    questionPage += 1;
                    pageNumber += 1;
                    loadQuestionsUI();
                }
            };

            btnPreviewQA.Click += delegate
            {
                if (checkToProceed())
                {
                    showPreviewModeDialog();
                }
            };

            btnBrowseImage.Click += delegate
            {
                if (string.IsNullOrEmpty(imageView_browse.ContentDescription))
                {
                    Settings.ActivityInt = (int)ActivityType.TokQuestCreateGame;
                    Settings.BrowsedImgTag = -1;
                    Intent = new Intent();
                    Intent.SetType("image/*");
                    Intent.SetAction(Intent.ActionGetContent);
                    StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), REQUEST_BROWSE_IMAGE);
                }
                else
                {
                    imageView_browse.SetImageBitmap(null);
                    imageView_browse.ContentDescription = "";
                    btnBrowseImage.Text = "Browse...";
                    btnBrowseImage.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.colorAccent);
                }
            };

            spinnerEvents();
        }

        private async Task CreateGameSet()
        {
            var user = Settings.GetTokketUser();
            var gameObject = new gameObject();
            gameObject.game_name_owner = user.DisplayName;
            gameObject.flag = (user.State == null) ? user?.Country : user?.State;
            gameObject.game_name_owner_title = user?.TitleId;
            gameObject.UserId = user.Id;
            gameObject.IsPublic = chkPublic.Checked;
            gameObject.Name = txtName.Text;
            gameObject.category = txtCategory.Text;
            gameObject.UserCountry = user.Country;
            if (!string.IsNullOrEmpty(imageView_browse.ContentDescription))
            {
                gameObject.Image = "data:image/jpeg;base64," + imageView_browse.ContentDescription;
            }

            var gameDetailsList = new List<GameDetails>();
            foreach (var item in gameSetList)
            {
                if (!string.IsNullOrEmpty(item.Question))
                {
                    var gameDetails = new GameDetails();
                    gameDetails.QuestionKind = item.QuestionType;
                    gameDetails.question = item.Question;

                    var answerList = new List<string>();
                    var choiceList = new List<string>();
                    foreach (var answer in item.AnswerList)
                    {
                        choiceList.Add(answer.Detail);

                        if (item.QuestionType.ToLower() == "sequence")
                        {
                            answerList.Add(answer.Detail);
                        }
                        else
                        {
                            if (answer.ChkAnswer)
                            {
                                answerList.Add(answer.Detail);
                            }
                        }
                    }
                    gameDetails.choices = choiceList;
                    gameDetails.answer = answerList;
                    gameDetailsList.Add(gameDetails);
                }
            }

            gameObject.GameListObject = gameDetailsList;

            showBlueLoading(this);
            ResultModel result = await TokquestService.Instance.CreateGameset(gameObject);
            hideBlueLoading(this);
            if (result.ResultEnum == Shared.Helpers.Result.Success)
            {
                showAlertDialog(this, "Successfully created a game!");
            }
            else
            {
                showAlertDialog(this, "Failed creating a game! " + result.ResultMessage);
            }
        }
        private void spinnerEvents()
        {
            spinnerSubGameType.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerSubGameType_ItemSelected);
            spinnerSubGameType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerSubGameType_ItemSelected);

            spinnerQuestionType.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerQuestionType_ItemSelected);
            spinnerQuestionType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerQuestionType_ItemSelected);

            spinnerAnswer.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerAnswer_ItemSelected);
            spinnerAnswer.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerAnswer_ItemSelected);
        }
        private void rememberAnswers()
        {
            //Get the data first before moving to the next page
            if (spinnerQuestionType.FirstVisiblePosition > 0)
            {
                gameSetList[questionPage].QuestionType = spinnerQuestionType.GetItemAtPosition(spinnerQuestionType.FirstVisiblePosition).ToString();
            }
                 
            gameSetList[questionPage].Question = txtQuestion.Text;
            gameSetList[questionPage].AnswerPosition = spinnerAnswer.FirstVisiblePosition;             
        }

        private void loadPageUI()
        {
            if (pageNumber == 1)
            {
                txtTitle.Text = "Create Game Set";
                linear_create_game_set.Visibility = ViewStates.Visible;
                linear_create_questions.Visibility = ViewStates.Gone;
                btnContinue.Text = "Continue";
                txtPageNumber.Visibility = ViewStates.Gone;
                btn_previous.Visibility = ViewStates.Gone;
                btn_next.Visibility = ViewStates.Gone;
                txtQuestionStatus.Visibility = ViewStates.Gone;
                btnPreviewQA.Visibility = ViewStates.Gone;
            }
            else
            {
                txtTitle.Text = "Create question and answer for: " + txtName.Text;
                var selectedNumber = spinnerSubGameType.GetItemAtPosition(spinnerSubGameType.FirstVisiblePosition);
                txtPageNumber.Text = (questionPage + 1).ToString() + " of " + selectedNumber;

                linear_create_game_set.Visibility = ViewStates.Gone;
                linear_create_questions.Visibility = ViewStates.Visible;

                txtPageNumber.Visibility = ViewStates.Visible;
                btn_previous.Visibility = ViewStates.Visible;
                btn_next.Visibility = ViewStates.Visible;
                txtQuestionStatus.Visibility = ViewStates.Visible;
                btnPreviewQA.Visibility = ViewStates.Visible;
            }
        }

        private bool checkToProceed()
        {
            bool isProceed = true;
            switch (spinnerQuestionType.FirstVisiblePosition)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    isProceed = false;
                    showAlertDialog(this, "Please select a question type");
                    break;
            }

            switch (spinnerAnswer.FirstVisiblePosition)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    isProceed = false;
                    showAlertDialog(this, "Please add answer!");
                    break;
            }

            if (string.IsNullOrEmpty(txtQuestion.Text))
            {
                isProceed = false;
                showAlertDialog(this, "Please add question!");
            }

            return isProceed;
        }

        private void loadQuestionsUI()
        {
            if (questionPage == 0) //Which is considered as first page of questionnaire
            {
                btn_previous.Enabled = false;
            }
            else
            {
                btnContinue.Text = "Create Game";
                btn_previous.Enabled = true;
            }

            var selectedNumber = spinnerSubGameType.GetItemAtPosition(spinnerSubGameType.FirstVisiblePosition);
            txtPageNumber.Text = (questionPage + 1).ToString() + " of " + selectedNumber;

            if (questionPage < Convert.ToInt16(selectedNumber.ToString()) - 1)
            {
                btn_next.Enabled = true;
            }
            else
            {
                btn_next.Enabled = false;
            }

            if (gameSetList[questionPage].QuestionType == null)
            {
                spinnerQuestionType.SetSelection(0);
                txtQuestion.Text = "";
                spinnerAnswer.SetSelection(0);
            }
            else
            {
                txtQuestion.Text = gameSetList[questionPage].Question;

                if (gameSetList[questionPage].QuestionType.ToLower() == "true or false")
                {
                    spinnerQuestionType.SetSelection(1);
                    spinnerAnswer.SetSelection(gameSetList[questionPage].AnswerPosition);
                }
                else if (gameSetList[questionPage].QuestionType.ToLower() == "multiple choice")
                {
                    spinnerQuestionType.SetSelection(2);
                    spinnerAnswer.SetSelection(gameSetList[questionPage].AnswerPosition);
                }
                else if (gameSetList[questionPage].QuestionType.ToLower() == "sequence")
                {
                    spinnerQuestionType.SetSelection(3);
                    spinnerAnswer.SetSelection(gameSetList[questionPage].AnswerPosition);
                }
            }

            int trueFalseCount = 0;
            int multipleCount = 0;
            int sequenceCount = 0;
            foreach (var item in gameSetList)
            {
                if (!string.IsNullOrEmpty(item.QuestionType))
                {
                    if (item.QuestionType.ToLower() == "true or false")
                    {
                        trueFalseCount += 1;
                    }
                    else if (item.QuestionType.ToLower() == "multiple choice")
                    {
                        multipleCount += 1;
                    }
                    else if (item.QuestionType.ToLower() == "sequence")
                    {
                        sequenceCount += 1;
                    }
                }
            }

            txtQuestionStatus.Text = "T/F: " + trueFalseCount + "| MC: " + multipleCount + " | Sequence: " + sequenceCount;
        }

        private void loadGameType()
        {
            spinnerGameType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerGameType_ItemSelected);
            List<string> gameTypeList = new List<string>();

            gameTypeList.Add("Choose...");
            gameTypeList.Add("Basic");
            gameTypeList.Add("Advanced");
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, gameTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerGameType.Adapter = Aadapter;
        }


        private void spinnerGameType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (e.Position == 2)
            {
                //Advanced
                txtSubGameType.Text = "Select # of Questions(12,24,36,48,60)";
                txtAdvancedGame.Visibility = ViewStates.Visible;
            }
            else
            {
                txtAdvancedGame.Visibility = ViewStates.Gone;
                txtSubGameType.Text = "Select # of Questions";
            };

            if (e.Position > 0)
            {
                loadSubGameType(e.Position);
            };
        }

        private void loadSubGameType(int position)
        {
            List<string> subGameTypeList = new List<string>();

            if (position == 2) //Advanced
            {
                var valueCount = 12;
                for (int i = 0; i < 5; i++)
                {
                    subGameTypeList.Add(valueCount.ToString());
                    valueCount += 12;
                }
            }
            else
            {
                subGameTypeList.Add("Select # of questions...");

                var valueCount = 5;
                for (int i = 0; i < 10; i++)
                {
                    subGameTypeList.Add(valueCount.ToString());
                    valueCount += 5;
                }
            }
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, subGameTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerSubGameType.Adapter = Aadapter;
        }


        private void spinnerSubGameType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var itemSelected = spinner.GetItemAtPosition(e.Position).ToString();

            if (e.Position > 0)
            {
                gameSetList = new List<GameSetModel>();
                for (int i = 0; i < Convert.ToInt16(itemSelected); i++)
                {
                    var gameModel = new GameSetModel();
                    gameSetList.Add(gameModel);
                }
            }
        }

        private void loadQuestionType()
        {
            List<string> gameTypeList = new List<string>();
            gameTypeList.Add("Select Question Type...");
            gameTypeList.Add("True or False");
            gameTypeList.Add("Multiple Choice");
            gameTypeList.Add("Sequence");
            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, gameTypeList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerQuestionType.Adapter = Aadapter;
        }


        private void spinnerQuestionType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            RecyclerDetail.Visibility = ViewStates.Gone;
            Spinner spinner = (Spinner)sender;
            if (e.Position == 1)
            {
                //True or False
            }
            else if (e.Position == 2)
            {
                //Multiple Choice
            }
            else if (e.Position == 3)
            {
                //Sequence
            };

            if (e.Position > 0)
            {
                loadSpinnerAnswer(e.Position);
            }
        }

        private void loadSpinnerAnswer(int position)
        {
            RecyclerDetail.Visibility = ViewStates.Gone;
            List<string> newList = new List<string>();
            newList.Add("Select...");
            if (position == 1) //true or false
            {
                newList.Add("True");
                newList.Add("False");
            }
            else if (position == 2)
            {
                //Multiple Choice
                newList.Add("2");
                newList.Add("3");
                newList.Add("4");
                newList.Add("5");
                newList.Add("6");
            }
            else if (position == 3)
            {
                //Sequence
                newList.Add("2");
                newList.Add("3");
                newList.Add("4");
                newList.Add("5");
                newList.Add("6");
            };

            ArrayAdapter<string> Aadapter = new BITAdapter(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, newList);
            Aadapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            spinnerAnswer.Adapter = Aadapter;
            spinnerAnswer.Tag = position;

            //set the default answer
            spinnerAnswer.SetSelection(gameSetList[questionPage].AnswerPosition);
        }

        private void spinnerAnswer_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var itemSelected = spinner.GetItemAtPosition(e.Position).ToString();

            var selectedInt = 0;
            bool isNumber = int.TryParse(itemSelected, out selectedInt);

            if (e.Position == 0)
            {
                RecyclerDetail.Visibility = ViewStates.Gone;
            }

            
            switch ((sender as View).Tag.ToString())
            {
                case "0":
                    break;
                case "1":
                    //True or False
                    gameSetList[questionPage].AnswerTrueFalse = itemSelected;
                    gameSetList[questionPage].AnswerList = new ObservableCollection<AddTokDetailModel>();

                    var itemAnswer = new AddTokDetailModel();
                    itemAnswer.Detail = "True";
                    itemAnswer.ChkAnswer = (itemSelected.ToLower() == "true") ? true : false;
                    gameSetList[questionPage].AnswerList.Add(itemAnswer);

                    itemAnswer = new AddTokDetailModel();
                    itemAnswer.Detail = "False";
                    itemAnswer.ChkAnswer = (itemSelected.ToLower() == "true") ? true : false;
                    gameSetList[questionPage].AnswerList.Add(itemAnswer);
                    break;
                default:
                    if (!isNumber)
                    {
                        return;
                    }

                    selectedInt = Convert.ToInt16(itemSelected);
                    var listCount = 0;
                    if (gameSetList[questionPage].AnswerList != null)
                    {
                        listCount = gameSetList[questionPage].AnswerList.Count();

                    }
                    else
                    {
                        gameSetList[questionPage].AnswerList = new ObservableCollection<AddTokDetailModel>();
                    }

                    for (int i = listCount; i < selectedInt; i++)
                    {
                        AddTokDetailModel multipleDetail = new AddTokDetailModel();
                        gameSetList[questionPage].AnswerList.Add(multipleDetail);
                    }

                    //If selected int is greater than the list of answers, remove it
                    if (selectedInt < gameSetList[questionPage].AnswerList.Count())
                    {
                        for (int i = gameSetList[questionPage].AnswerList.Count(); i > selectedInt; i--)
                        {
                            gameSetList[questionPage].AnswerList.RemoveAt(i-1);
                        }
                    }
                    
                    SetRecyclerDetailAnswers();
                    break;
            }
        }

        private void SetRecyclerDetailAnswers()
        {
            RecyclerDetail.Visibility = ViewStates.Visible;

            var adapterDetail = gameSetList[questionPage].AnswerList.GetRecyclerAdapter(BindAnswerDetail, Resource.Layout.row_game_set_multiple_choice);
            RecyclerDetail.SetAdapter(adapterDetail);
        }

        private void chkAnswerValueChanged(object sender, EventArgs e)
        {
            int position = Convert.ToInt16((sender as View).Tag.ToString());
            int cntCheckMultiple = 0;
            for (int i = 0; i < gameSetList[questionPage].AnswerList.Count(); i++)
            {
                if (gameSetList[questionPage].AnswerList[i].ChkAnswer)
                {
                    cntCheckMultiple += 1;
                }
            }

            if (cntCheckMultiple > 1)
            {
                var childView = RecyclerDetail.GetChildAt(position);
                var chkView = childView.FindViewById<CheckBox>(Resource.Id.chkAnswer);
                if (gameSetList[questionPage].AnswerList[position].ChkAnswer)
                {
                    chkView.Checked = false;
                    gameSetList[questionPage].AnswerList[position].ChkAnswer = false;
                }
            }
        }

        private void BindAnswerDetail(CachingViewHolder holder, AddTokDetailModel model, int position)
        {
            var txtDesc = holder.FindCachedViewById<TextView>(Resource.Id.txtDesc);
            var txtName = holder.FindCachedViewById<EditText>(Resource.Id.txtName);
            var CheckAnswer = holder.FindCachedViewById<CheckBox>(Resource.Id.chkAnswer);

            var detailBinding = new Binding<string, string>(model,
                                                  () => model.Detail,
                                                  txtName,
                                                  () => txtName.Text,
                                                  BindingMode.TwoWay);

            var chkAnswerBinding = new Binding<bool, bool>(model,
                                                 () => model.ChkAnswer,
                                                 CheckAnswer,
                                                 () => CheckAnswer.Checked,
                                                 BindingMode.TwoWay);

            if (spinnerQuestionType.FirstVisiblePosition == 2) //Show checkbox if selected position is multiple choice
            {
                CheckAnswer.Tag = position;
                CheckAnswer.CheckedChange -= chkAnswerValueChanged;
                CheckAnswer.CheckedChange += chkAnswerValueChanged;

                CheckAnswer.Visibility = ViewStates.Visible;
                var letter = "A";
                switch (position)
                {
                    case 0:
                        letter = "A: ";
                        break;
                    case 1:
                        letter = "B: ";
                        break;
                    case 2:
                        letter = "C: ";
                        break;
                    case 3:
                        letter = "D: ";
                        break;
                    case 4:
                        letter = "E: ";
                        break;
                    case 5:
                        letter = "F: ";
                        break;
                    default:
                        letter = "G: ";
                        break;
                }
                txtDesc.Text = letter;
            }
            else
            {
                CheckAnswer.Visibility = ViewStates.Gone;
                txtDesc.Text = (position + 1).ToString() + " : ";
            }
        }

        private void showPreviewModeDialog()
        {
            var previewDialog = new Dialog(this);
           previewDialog.SetContentView(Resource.Layout.dialog_game_set_preview);
           previewDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            previewDialog.Show();

            // Access Popup layout fields like below  
            var txtQuestionDialog = previewDialog.FindViewById<TextView>(Resource.Id.txtQuestion);
            var recyclerAnswer = previewDialog.FindViewById<RecyclerView>(Resource.Id.recyclerAnswer);
            var btnContinue = previewDialog.FindViewById<Button>(Resource.Id.btnContinue);

            txtQuestionDialog.Text = txtQuestion.Text;
            // Events for that popup layout  
            btnContinue.Click += delegate
            {
                previewDialog.Dismiss();
            };

            recyclerAnswer.SetLayoutManager(new LinearLayoutManager(this));
            var collectionPreview = new ObservableCollection<AddTokDetailModel>();
                                              
            if (spinnerQuestionType.GetItemAtPosition(spinnerQuestionType.FirstVisiblePosition).ToString().ToLower() == "true or false")
            {
                var modelItem = new AddTokDetailModel();
                modelItem.EnglishDetail = "A";
                modelItem.Detail = "True";
                collectionPreview.Add(modelItem);

                modelItem = new AddTokDetailModel();
                modelItem.EnglishDetail = "B";
                modelItem.Detail = "False";
                collectionPreview.Add(modelItem);
            }
            else
            {
                for (int i = 0; i < gameSetList[questionPage].AnswerList.Count; i++)
                {
                    if (spinnerQuestionType.GetItemAtPosition(spinnerQuestionType.FirstVisiblePosition).ToString().ToLower() == "sequence")
                    {
                        var modelItem = new AddTokDetailModel();
                        modelItem.EnglishDetail = (i + 1).ToString();
                        modelItem.Detail = gameSetList[questionPage].AnswerList[i].Detail;
                        collectionPreview.Add(modelItem);
                    }
                    else
                    {  
                        var letter = "A";
                        switch (i)
                        {
                            case 0:
                                letter = "A: ";
                                break;
                            case 1:
                                letter = "B: ";
                                break;
                            case 2:
                                letter = "C: ";
                                break;
                            case 3:
                                letter = "D: ";
                                break;
                            case 4:
                                letter = "E: ";
                                break;
                            case 5:
                                letter = "F: ";
                                break;
                            default:
                                letter = "G: ";
                                break;
                        }

                        var modelItem = new AddTokDetailModel();
                        modelItem.EnglishDetail = letter;
                        modelItem.Detail = gameSetList[questionPage].AnswerList[i].Detail;
                        modelItem.ChkAnswer = gameSetList[questionPage].AnswerList[i].ChkAnswer;
                        collectionPreview.Add(modelItem);
                    }
                }
            }


            var adapterDetail = collectionPreview.GetRecyclerAdapter(BindAnswerPreviewDetail, Resource.Layout.row_game_set_multiple_choice);
            recyclerAnswer.SetAdapter(adapterDetail);
        }

        private void BindAnswerPreviewDetail(CachingViewHolder holder, AddTokDetailModel model, int position)
        {
            var txtDesc = holder.FindCachedViewById<TextView>(Resource.Id.txtDesc);
            var txtName = holder.FindCachedViewById<EditText>(Resource.Id.txtName);
            var CheckAnswer = holder.FindCachedViewById<CheckBox>(Resource.Id.chkAnswer);

           /*var detailBinding = new Binding<string, string>(model,
                                                  () => model.Detail,
                                                  txtName,
                                                  () => txtName.Text,
                                                  BindingMode.TwoWay);

            var chkAnswerBinding = new Binding<bool, bool>(model,
                                                 () => model.ChkAnswer,
                                                 CheckAnswer,
                                                 () => CheckAnswer.Checked,
                                                 BindingMode.TwoWay);*/

            txtDesc.Text = model.EnglishDetail;
            txtName.Text = model.Detail;
            CheckAnswer.Checked = model.ChkAnswer;
            if (spinnerQuestionType.GetItemAtPosition(spinnerQuestionType.FirstVisiblePosition).ToString().ToLower() == "multiple choice")
            {
                CheckAnswer.Visibility = ViewStates.Visible;
            }
            else
            {
                CheckAnswer.Visibility = ViewStates.Gone;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == REQUEST_BROWSE_IMAGE) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                this.StartActivityForResult(nextActivity, requestCode);
            }
        }

        public void displayImageBrowse()
        {
            //Main Image
            imageView_browse.SetImageBitmap(null);
            //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
            imageView_browse.ContentDescription = Settings.ImageBrowseCrop;
            byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
            imageView_browse.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
            btnBrowseImage.Text = "Remove Image";
            btnBrowseImage.BackgroundTintList = AppCompatResources.GetColorStateList(this, Resource.Color.red_500);

            Settings.ImageBrowseCrop = null;
        }

        public TextView txtAnswer => FindViewById<TextView>(Resource.Id.txtAnswer);
        public TextView txtTitle => FindViewById<TextView>(Resource.Id.txtTitle);
        public TextView txtAdvancedGame => FindViewById<TextView>(Resource.Id.txtAdvancedGame);
        public TextView txtSubGameType => FindViewById<TextView>(Resource.Id.txtSubGameType);
        public TextView btnCancel => FindViewById<TextView>(Resource.Id.btnCancel);
        public Button btnContinue => FindViewById<Button>(Resource.Id.btnContinue);
        public ImageView imageView_browse => FindViewById<ImageView>(Resource.Id.imageView_browse);
        public Button btnBrowseImage => FindViewById<Button>(Resource.Id.btnBrowseImage);
        public TextView txtPageNumber => FindViewById<TextView>(Resource.Id.txtPageNumber);
        public TextView txtQuestionStatus => FindViewById<TextView>(Resource.Id.txtQuestionStatus);
        public ImageButton btn_previous => FindViewById<ImageButton>(Resource.Id.btn_previous);
        public ImageButton btn_next => FindViewById<ImageButton>(Resource.Id.btn_next);
        public Button btnPreviewQA => FindViewById<Button>(Resource.Id.btnPreviewQA);
        public EditText txtQuestion => FindViewById<EditText>(Resource.Id.txtQuestion);
        public CheckBox chkPublic => FindViewById<CheckBox>(Resource.Id.chkPublic);
        public CheckBox chkPrivateGroup => FindViewById<CheckBox>(Resource.Id.chkPrivateGroup);
        public EditText txtName => FindViewById<EditText>(Resource.Id.txtName);
        public EditText txtCategory => FindViewById<EditText>(Resource.Id.txtCategory);
        public Spinner spinnerGameType => FindViewById<Spinner>(Resource.Id.spinnerGameType);
        public Spinner spinnerSubGameType => FindViewById<Spinner>(Resource.Id.spinnerSubGameType);
        public Spinner spinnerQuestionType => FindViewById<Spinner>(Resource.Id.spinnerQuestionType);
        public Spinner spinnerAnswer => FindViewById<Spinner>(Resource.Id.spinnerAnswer);
        public LinearLayout linear_create_game_set => FindViewById<LinearLayout>(Resource.Id.linear_create_game_set);
        public LinearLayout linear_create_questions => FindViewById<LinearLayout>(Resource.Id.linear_create_questions);
        public RecyclerView RecyclerDetail => FindViewById<RecyclerView>(Resource.Id.recyclerAnswer);
    }
}