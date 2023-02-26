using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models.Tok;
using Tokket.Android.Adapters;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Android.Graphics;
using Android.Webkit;
using Android.Text;
using Newtonsoft.Json;
using Result = Android.App.Result;

namespace Tokket.Android
{
    [Activity(Label = "AddTrainingActivity")]
    public class AddTrainingActivity : BaseActivity
    {
        bool isEdit = false;
        OpportunityTok OpportunityTok = new OpportunityTok();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addtraining_view);
            ArrayAdapter<string> adp1 = new ArrayAdapter<String>(this,
                             Resource.Layout.support_simple_spinner_dropdown_item, new string[] { "Choose Type","Courses","Programs","Tutors" });
            adp1.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            TrainingTypeSpinner.Adapter = adp1;
            TrainingTypeSpinner.ItemSelected += TrainingType_Selected;

            CancelButton.Click += (obj, _event) => { Finish(); };
            isEdit = !string.IsNullOrEmpty(Intent.GetStringExtra("isEdit"));
            loadCountries();
            if (isEdit)
            {
                OpportunityTok = JsonConvert.DeserializeObject<OpportunityTok>(Intent.GetStringExtra("trainingModel"));
                SaveOpportunityButton.Text = $"Update {OpportunityTok.Type.FirstLetterToUpperCase()}";
                FillUpFields();
            }

            SaveOpportunityButton.Click += SaveEditTraining_Click;
            // Create your application here
        }

        private async void SaveEditTraining_Click(object sender, EventArgs e)
        {
            linearLayoutProgress.Visibility = ViewStates.Visible;
            InitData();
            OpportunityTok.PrimaryFieldText = TrainingName.Text;
            switch (TrainingTypeSpinner.GetItemAtPosition(TrainingTypeSpinner.SelectedItemPosition).ToString())
            {
                case "Courses": 
                    OpportunityTok.Type = "course";
                    OpportunityTok.Name = School.Text;
                    OpportunityTok.School = School.Text;
                    OpportunityTok.Description = Description.Text;
                    OpportunityTok.HowItWorks = HIW.Text;
                    OpportunityTok.Instructors = Instructors.Text;
                    OpportunityTok.EnrollmentOptions = EnrollmentOptions.Text;
                    OpportunityTok.SourceLink = Link.Text;
                    OpportunityTok.Enrolled = string.IsNullOrEmpty(NOPE.Text)? 0: int.Parse(NOPE.Text);
                    OpportunityTok.City = City.Text;
                    OpportunityTok.FAQ = FAQ.Text;
                    OpportunityTok.Days = Days.Text;
                    OpportunityTok.Dates = Date.Text;
                    OpportunityTok.StartTime = StartTime.Text;
                    OpportunityTok.EndTime = EndTime.Text;
                    OpportunityTok.Units = Units.Text;
                    OpportunityTok.EnrollmentRequirements = EnrollmentRequirements.Text;
                    OpportunityTok.ReservationRequirements = ReservationRequirements.Text;
                    OpportunityTok.CourseMaterials = CourseMaterials.Text;

                    break;
                case "Programs":
                    OpportunityTok.Type = "program";
                    OpportunityTok.Cost = Cost.Text;
                    OpportunityTok.Schedule = Schedule.Text;
                    OpportunityTok.Description = Description.Text;
                    OpportunityTok.Duration = Duration.Text;
                    OpportunityTok.RelatedSkills = RSACL.Text;
                    OpportunityTok.Enrolled = string.IsNullOrEmpty(NOPE.Text) ? 0 : int.Parse(NOPE.Text);
                    OpportunityTok.Projects = string.IsNullOrEmpty(NOP.Text) ? 0 : int.Parse(NOP.Text);
                    OpportunityTok.Assigments = string.IsNullOrEmpty(NOA.Text) ? 0 : int.Parse(NOA.Text);
                    OpportunityTok.InstructorInfo = InstructorsInfo.Text;
                    break;
                case "Tutors":
                    OpportunityTok.Type = "tutor";
                    OpportunityTok.Education = Educatoin.Text;
                    OpportunityTok.Experience = Experience.Text;
                     break;
            }
            OpportunityTok.ItemCountry = CountrySpinner.SelectedItem.ToString();

            OpportunityTok.ItemState = !string.IsNullOrEmpty(StateSpinner.SelectedItem?.ToString()) ? StateSpinner.SelectedItem.ToString() : string.Empty;

            if (!string.IsNullOrEmpty(ImageDisplay.ContentDescription))
            {
                if (!URLUtil.IsValidUrl(ImageDisplay.ContentDescription))
                {
                    OpportunityTok.Image = "data:image/jpeg;base64," + ImageDisplay.ContentDescription;
                }
            }
            var result = new ResultModel() { };
            if (!isEdit)
            {
                result = await TokService.Instance.CreateOpportunityAsync(OpportunityTok);
            }
            else
            {

                result = await TokService.Instance.UpdateOpportunityAsync(OpportunityTok);
            }
            if (result.ResultEnum == Tokket.Shared.Helpers.Result.Success)
            {
                alertMessage("", "Training successfully saved!", 0, (d, fv) => {
                    //   OpportunityTok = JsonConvert.DeserializeObject<OpportunityTok>(result.ResultObject.ToString());
                    if (!isEdit)
                    {
                        var resultData = JsonConvert.SerializeObject(result.ResultObject);
                        Intent intent = new Intent();
                        intent.PutExtra("TrainingData", resultData);
                        SetResult(Result.Ok, intent);
                    }
                    else
                    {
                        Intent intent = new Intent();
                        intent.PutExtra("updatedOpportunity", JsonConvert.SerializeObject(OpportunityTok));
                        TrainingListActivity.Instance.UpdateOpportunityItem(OpportunityTok);
                        SetResult(Result.Ok, intent);

                    }
                    Finish();
                });
            }
            else
            {
                alertMessage("", "Training failed saved!", 0);
            }
            linearLayoutProgress.Visibility = ViewStates.Gone;
        }

        private void TrainingType_Selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (TrainingTypeSpinner.GetItemAtPosition(e.Position).ToString()) {
                case "Courses": CoursesSelected(); break;
                case "Programs": ProgramSelected(); break;
                case "Tutors": TutorSelected(); break;
            }
        }

        private void CoursesSelected() {
            NameLinear.Visibility = ViewStates.Visible;
            CategoryLinear.Visibility = ViewStates.Visible;
            Description.Visibility = ViewStates.Visible;
            HIWLinear.Visibility = ViewStates.Visible;
            FAQLinear.Visibility = ViewStates.Visible;
            InstructorsLinear.Visibility = ViewStates.Visible;
            InstructorsInfo.Visibility = ViewStates.Gone;
            CountryLinear.Visibility = ViewStates.Visible;
            CityLinear.Visibility = ViewStates.Visible;
            NOPELinear.Visibility = ViewStates.Visible;
            DetailsLinear.Visibility = ViewStates.Gone;
            LinkLinear.Visibility = ViewStates.Visible;
            Link.Hint = "Source Link";
            TrainingName.Hint = "Course Name";
            DescriptionLinear.Visibility = ViewStates.Visible;
            ProgramsLayout.Visibility = ViewStates.Gone;
            TutorsLinear.Visibility = ViewStates.Gone;
            CourseLinear.Visibility = ViewStates.Visible;
        }

        private void ProgramSelected()
        {
            NameLinear.Visibility = ViewStates.Visible;
            TrainingName.Hint = "Program Name";
            CategoryLinear.Visibility = ViewStates.Visible;
            Description.Visibility = ViewStates.Visible;
            HIWLinear.Visibility = ViewStates.Gone;
            InstructorsLinear.Visibility = ViewStates.Visible;
            InstructorsInfo.Visibility = ViewStates.Gone;
            CategoryLinear.Visibility = ViewStates.Visible;
            CountryLinear.Visibility = ViewStates.Visible;
            CityLinear.Visibility = ViewStates.Visible;
            NOPELinear.Visibility = ViewStates.Visible;
            DetailsLinear.Visibility = ViewStates.Visible;
            LinkLinear.Visibility = ViewStates.Visible;
            CourseLinear.Visibility = ViewStates.Gone;
            Instructors.Hint = "Instructors Name";
            FAQLinear.Visibility = ViewStates.Gone;
            ProgramsLayout.Visibility = ViewStates.Visible;
            TutorsLinear.Visibility = ViewStates.Gone;
        }

        private void TutorSelected()
        {
            NameLinear.Visibility = ViewStates.Visible;
            TrainingName.Hint = "Tutor Name";
            CategoryLinear.Visibility = ViewStates.Visible;
            Description.Visibility = ViewStates.Visible;
            HIWLinear.Visibility = ViewStates.Gone;
            InstructorsLinear.Visibility = ViewStates.Gone;
            InstructorsInfo.Visibility = ViewStates.Gone;
            CategoryLinear.Visibility = ViewStates.Visible;
            CountryLinear.Visibility = ViewStates.Visible;
            CityLinear.Visibility = ViewStates.Visible;
            NOPELinear.Visibility = ViewStates.Gone;
            DetailsLinear.Visibility = ViewStates.Visible;
            LinkLinear.Visibility = ViewStates.Gone;
            CourseLinear.Visibility = ViewStates.Gone;
            Instructors.Hint = "Instructors Name";
            FAQLinear.Visibility = ViewStates.Gone;
            ProgramsLayout.Visibility = ViewStates.Gone;
            TutorsLinear.Visibility = ViewStates.Visible;
        }


        private void InitData()
        {


            var user = Settings.GetTokketUser();
            if (!isEdit)
            {
                OpportunityTok = new OpportunityTok();
                OpportunityTok.UserPhoto = user.UserPhoto;
                OpportunityTok.UserId = user.Id;
                OpportunityTok.UserDisplayName = user.DisplayName;
                OpportunityTok.Category = Category.Text;
                OpportunityTok.CategoryId = "category-" + OpportunityTok.Category.ToIdFormat();
                OpportunityTok.TokGroup = "";
                OpportunityTok.TokType = "";
                OpportunityTok.TokTypeId = "";
            }
            else
            {
               
              
            }




        }

        private void FillUpFields() {
            TrainingName.Text = OpportunityTok.PrimaryFieldText;
            Category.Text = OpportunityTok.Category;
            int countryIndex = CountryTool.GetCountries().FindIndex(c=>c.Name == OpportunityTok.ItemCountry);
            CountrySpinner.SetSelection(countryIndex);
            if (!string.IsNullOrEmpty(OpportunityTok.ItemState)) {
                StateLinear.Visibility = ViewStates.Visible;
                int stateIndex = CountryHelper.GetCountryStates("us").FindIndex(s =>s.Name == OpportunityTok.ItemState);
                StateSpinner.SetSelection(stateIndex);
            }
            switch (OpportunityTok.Type)
            {
                case "course":
                    TrainingTypeSpinner.SetSelection(1);
                    School.Text = OpportunityTok.School;
                    Description.Text = OpportunityTok.Description;
                    HIW.Text = OpportunityTok.HowItWorks;
                    Instructors.Text = OpportunityTok.Instructors;
                    EnrollmentOptions.Text = OpportunityTok.EnrollmentOptions;
                    Link.Text = OpportunityTok.SourceLink;
                    NOPE.Text = OpportunityTok.Enrolled.ToString();
                    City.Text = OpportunityTok.City;
                    FAQ.Text = OpportunityTok.FAQ;
                    Days.Text = OpportunityTok.Days;
                    Date.Text = OpportunityTok.Dates;
                    StartTime.Text = OpportunityTok.StartTime;
                    EndTime.Text = OpportunityTok.EndTime;
                    Units.Text = OpportunityTok.Units;
                    EnrollmentRequirements.Text = OpportunityTok.EnrollmentRequirements;
                    ReservationRequirements.Text = OpportunityTok.ReservationRequirements;
                    CourseMaterials.Text = OpportunityTok.CourseMaterials;
                    CoursesSelected();
                    break;
                case "program":
                    TrainingTypeSpinner.SetSelection(2);
                    Cost.Text = OpportunityTok.Cost;
                    Schedule.Text = OpportunityTok.Schedule;
                    Description.Text = OpportunityTok.Description;
                    Duration.Text = OpportunityTok.Duration;
                    RSACL.Text = OpportunityTok.RelatedSkills;
                    NOPE.Text = OpportunityTok.Enrolled.ToString();
                    NOP.Text = OpportunityTok.Projects.ToString();
                    NOA.Text = OpportunityTok.Assigments.ToString();
                    InstructorsInfo.Text = OpportunityTok.InstructorInfo;
                    ProgramSelected();
                    break;
                case "tutor":
                    TrainingTypeSpinner.SetSelection(3);
                    Educatoin.Text = OpportunityTok.Education;
                    Experience.Text = OpportunityTok.Experience;
                    TutorSelected();
                    break;

            }
        }

        public void loadCountries()
        {
            CountrySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(txtCountry_ItemSelected);
            List<CountryModel> countryModels = CountryHelper.GetCountries();
            List<string> countriesList = new List<string>();
            for (int i = 0; i < countryModels.Count(); i++)
            {
                countriesList.Add(countryModels[i].Name);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.support_simple_spinner_dropdown_item, countriesList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.support_simple_spinner_dropdown_item);
            CountrySpinner.Adapter = adapter;
        }
        private void txtCountry_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (spinner.GetItemAtPosition(e.Position).ToString().ToLower() == "united states")
            {
                //linearState.Visibility = ViewStates.Visible;
                StateLinear.Visibility = ViewStates.Visible;
                loadState("us");
            }
            else
            {
                StateLinear.Visibility = ViewStates.Gone;
                StateSpinner.Adapter = null;
            }
        }
        public void loadState(string countryId)
        {
            List<Tokket.Shared.Models.StateModel> stateModel = CountryHelper.GetCountryStates(countryId);
            List<string> statelist = new List<string>();
            List<string> imageStateList = new List<string>();
            for (int i = 0; i < stateModel.Count(); i++)
            {
                statelist.Add(stateModel[i].Name);
                imageStateList.Add(stateModel[i].Image);
            }

            SpinnerStateAdapter adapter = new SpinnerStateAdapter(this, Resource.Layout.signup_page_state_row, statelist, imageStateList);
            StateSpinner.Adapter = adapter;

            /*var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, statelist);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            txtState.Adapter = adapter;*/
        }

        [Java.Interop.Export("OnClickAddTokImgMain")]
        public void OnClickAddTokImgMain(View v)
        {
            Settings.BrowsedImgTag = -1;
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)ActivityType.AddOpportunityActivity);
        }
        [Java.Interop.Export("OnClickRemoveTokImgMain")]
        public void OnClickRemoveTokImgMain(View v)
        {
            ImageDisplay.SetImageBitmap(null);
            //Yearbook.Image = null;
            ImageDisplay.ContentDescription = "";
            BrowseImgButton.Visibility = ViewStates.Visible;
            RemoveImgButton.Visibility = ViewStates.Gone;
        }

        private void alertMessage(string title, string message, int icon, EventHandler<DialogClickEventArgs> Okhandler = null)
        {
            if (Okhandler == null)
            {
                Okhandler = (d, fv) => { };
            }
            var dialog = new AlertDialog.Builder(this);
            var alertDialog = dialog.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(icon);
            alertDialog.SetMessage(message);
            alertDialog.SetButton(Html.FromHtml("<font color='#007bff'>OK</font>", FromHtmlOptions.ModeLegacy), Okhandler);
            alertDialog.Show();
            alertDialog.SetCanceledOnTouchOutside(false);
        }
        public void displayImageBrowse()
        {
            //Main Image
            ImageDisplay.SetImageBitmap(null);
            if (Settings.BrowsedImgTag == -1)
            {
                //AddTokVm.TokModel.Image = Settings.ImageBrowseCrop;
                ImageDisplay.ContentDescription = Settings.ImageBrowseCrop;
                byte[] imageMainBytes = Convert.FromBase64String(Settings.ImageBrowseCrop);
                ImageDisplay.SetImageBitmap((BitmapFactory.DecodeByteArray(imageMainBytes, 0, imageMainBytes.Length)));
                BrowseImgButton.Visibility = ViewStates.Gone;
                RemoveImgButton.Visibility = ViewStates.Visible;
            }


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
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)ActivityType.AddYearbookActivity) && (resultCode == Result.Ok) && (data != null))
            {
                var uri = data.Data;
                Intent nextActivity = new Intent(this, typeof(CropImageActivity));
                Settings.ImageBrowseCrop = (string)uri;
                Settings.ActivityInt = (int)ActivityType.AddOpportunityActivity;
                this.StartActivityForResult(nextActivity, (int)ActivityType.AddOpportunityActivity);

                if (Settings.BrowsedImgTag != -1)
                {
                    int vtag = Settings.BrowsedImgTag;


                }
            }
        }

        Spinner TrainingTypeSpinner => FindViewById<Spinner>(Resource.Id.trainingSpinner);
        LinearLayout ProgramsLayout => FindViewById<LinearLayout>(Resource.Id.linear_Programs);
        LinearLayout TutorsLinear => FindViewById<LinearLayout>(Resource.Id.TutorLinear);

        LinearLayout CourseLinear => FindViewById<LinearLayout>(Resource.Id.linear_Courses);

        LinearLayout NameLinear => FindViewById<LinearLayout>(Resource.Id.NameLinear);
        LinearLayout CategoryLinear => FindViewById<LinearLayout>(Resource.Id.CategoryLinear);
        LinearLayout DescriptionLinear => FindViewById<LinearLayout>(Resource.Id.DescriptionLinear);

        LinearLayout HIWLinear => FindViewById<LinearLayout>(Resource.Id.HIWLinear);
        LinearLayout InstructorsLinear => FindViewById<LinearLayout>(Resource.Id.InstructorsLinear);
        LinearLayout CountryLinear => FindViewById<LinearLayout>(Resource.Id.country_linear);
        LinearLayout StateLinear => FindViewById<LinearLayout>(Resource.Id.linearState);
        LinearLayout CityLinear => FindViewById<LinearLayout>(Resource.Id.CityLinear);

        LinearLayout NOPELinear => FindViewById<LinearLayout>(Resource.Id.NOPELinear);

        LinearLayout LinkLinear => FindViewById<LinearLayout>(Resource.Id.LinkLinear);

        LinearLayout FAQLinear => FindViewById<LinearLayout>(Resource.Id.FAQLinear);

        LinearLayout DetailsLinear => FindViewById<LinearLayout>(Resource.Id.detailsLinear);

        EditText TrainingName => FindViewById<EditText>(Resource.Id.txtT_Name);

        EditText Category => FindViewById<EditText>(Resource.Id.txtT_Category);

        EditText NOL => FindViewById<EditText>(Resource.Id.txtT_NOL);

        EditText Cost => FindViewById<EditText>(Resource.Id.txtT_Cost);

        EditText Schedule => FindViewById<EditText>(Resource.Id.txtT_Schedule);

        EditText Duration => FindViewById<EditText>(Resource.Id.txtT_Duration);

        EditText RSACL => FindViewById<EditText>(Resource.Id.txtT_RSACL);

        EditText NOS => FindViewById<EditText>(Resource.Id.txtT_NOS);

        EditText NOP => FindViewById<EditText>(Resource.Id.txtT_NOP);

        EditText NOA => FindViewById<EditText>(Resource.Id.txtT_NOA);
        EditText Educatoin => FindViewById<EditText>(Resource.Id.txtT_Education);
        EditText Experience => FindViewById<EditText>(Resource.Id.txtT_Experience);
        EditText EnrollmentOptions => FindViewById<EditText>(Resource.Id.txtT_EnrollmentOptions);

        EditText StartTime => FindViewById<EditText>(Resource.Id.txtT_StartTime);

        EditText EndTime => FindViewById<EditText>(Resource.Id.txtT_EndTime);
        EditText Days => FindViewById<EditText>(Resource.Id.txtT_Days);
        EditText Date => FindViewById<EditText>(Resource.Id.txtT_Dates);
        EditText Units => FindViewById<EditText>(Resource.Id.txtT_Units);
        EditText EnrollmentRequirements => FindViewById<EditText>(Resource.Id.txtT_EnrolmentRequirements);
        EditText ReservationRequirements => FindViewById<EditText>(Resource.Id.txtT_ReservationRequirements);
        EditText CourseMaterials => FindViewById<EditText>(Resource.Id.txtT_CourseMaterials);
        EditText PA => FindViewById<EditText>(Resource.Id.txtT_PA);

        EditText Requirements => FindViewById<EditText>(Resource.Id.txtT_Requirements);

        EditText Enail => FindViewById<EditText>(Resource.Id.txtT_Email);

        EditText Description => FindViewById<EditText>(Resource.Id.txtT_Description);

        EditText HIW => FindViewById<EditText>(Resource.Id.txtT_HIW);

        EditText Instructors => FindViewById<EditText>(Resource.Id.txtT_Instructors);

        TextInputLayout IntructorsInfoInput => FindViewById<TextInputLayout>(Resource.Id.txtInputT_InstructorsInfo);
        EditText InstructorsInfo => FindViewById<EditText>(Resource.Id.txtT_InstructorsInfo);

        Spinner CountrySpinner => FindViewById<Spinner>(Resource.Id.spn_country);

        Spinner StateSpinner => FindViewById<Spinner>(Resource.Id.spn_state);

        EditText City => FindViewById<EditText>(Resource.Id.txtT_City);

        EditText NOPE => FindViewById<EditText>(Resource.Id.txtT_NOPE);

        EditText Link => FindViewById<EditText>(Resource.Id.txtT_Link);

        EditText School => FindViewById<EditText>(Resource.Id.txtT_School);
        EditText FAQ => FindViewById<EditText>(Resource.Id.txtT_FAQ);
        EditText Detail1 => FindViewById<EditText>(Resource.Id.txtT_d1);

        EditText Detail2 => FindViewById<EditText>(Resource.Id.txtT_d2);

        Button SaveOpportunityButton => FindViewById<Button>(Resource.Id.btnAddTraining);
        ImageView ImageDisplay => FindViewById<ImageView>(Resource.Id.img_browse);

        Button BrowseImgButton => FindViewById<Button>(Resource.Id.btnBrowseImage);
        Button RemoveImgButton => FindViewById<Button>(Resource.Id.btnRemoveImage);
        LinearLayout linearLayoutProgress => FindViewById<LinearLayout>(Resource.Id.LinearProgress_ClassGroup);

        TextView CancelButton => FindViewById<TextView>(Resource.Id.btnCancel);


    }
}