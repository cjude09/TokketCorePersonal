using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.ObjectModel;
using Tokket.Android.Helpers;

namespace Tokket.Android
{
    [Activity(Label = "Guide", Theme = "@style/CustomAppThemeBlue", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class GuidesActivity : BaseActivity
    {
        #region Guide Instructions
        string[] HTCP = new string[] {
        "1. Go to Tok Pak on the left menu or on a tok group tab.",
        "2. Click on Add Tok Pak button.",
        "3. Enter the name for your Tok Pak.",
        "4. Select the Tok Pak type. You can choose between a Presentation, Paper, or Practice Test.",
        "5. You can add a Tok by a. pasting a Tok Link b. Searching for a Tok c. Adding a Tok.",
        "6. Make sure you hit the “Save” button to save your Tok Pak."
        }; //How to create presentation
        string[] HTCFA = new string[] {
        "1. Click on the “Login” button on the Main Menu",
        "2. On the Create Account tab, select Family for Account Type",
        "3. Enter the first name of the account owner",
        "4. Enter in the last name of the family and fill out the Birthday, Country, and Email of the owner",
        "5. Create a Password and Subaccount owner key",
        "6. Press the Sign Up button and pay $2.99 USD",
        "7. Check your email to verify your account"
        };//How to create Family account
        string[] HTCGA = new string[] {
        "1. Click on the “Login” button on the Main Menu",
        "2. On the Create Account tab, select Organization for Account Type",
        "3. Enter the first name of the account owner",
        "4. Enter in the last name of the family and fill out the Birthday, Country, and Email of the owner",
        "5. Create a Password and Subaccount owner key",
        "6. Press the Sign Up button and pay $2.99 USD",
        "7. Check your email to verify your account"
        };//How to create Group Account
        #endregion

        int InstructionSelected = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.guides_page);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here
            var ins = Intent.GetStringExtra("InstructionSetup");
            InstructionSelected = int.Parse(ins);
            Title = Intent.GetStringExtra("GuideTitle");

            SetUpGuide();
        }

        private void SetUpGuide() {
          var   mLayoutManager = new GridLayoutManager(this.BaseContext, 1);
            InstructionsRecycler.SetLayoutManager(mLayoutManager);
            switch (InstructionSelected) {
                case 1: SetupAdapter(HTCP);
                    ImageMain.SetImageResource(Resource.Drawable.pres);
                    break;
                case 2: SetupAdapter(HTCFA);
                    ImageMain.SetImageResource(Resource.Drawable.fam);
                    break;
                case 3: SetupAdapter(HTCGA);
                    ImageMain.SetImageResource(Resource.Drawable.org);
                    break;
            }
        }

        private void SetupAdapter(string[] items) {
            var collection = new ObservableCollection<string>();
            foreach (var ins in items) {
                collection.Add(ins);
            }
            var adapter = collection.GetRecyclerAdapter(BindInstructions,Resource.Layout.guide_item_row);
            InstructionsRecycler.SetAdapter(adapter);
        }

        private void BindInstructions(RecyclerView.ViewHolder view, string instruction, int position)
        {
            var tokstar = view.ItemView.FindViewById<ImageView>(Resource.Id.tokstar_img);
            var Instruction = view.ItemView.FindViewById<TextView>(Resource.Id.txt_dialogue);
            AssignTokStar(tokstar, position);
            Instruction.Text = instruction;
        }

        private void AssignTokStar(ImageView view,int pos) {
            if (pos > 5) {
                Random rand = new Random();
                pos = rand.Next(0, 5);
            }
            switch (pos) {
                case 0:
                    view.SetBackgroundResource(Resource.Drawable.tokstarguy3crop);
                    break;
                case 1:
                    view.SetBackgroundResource(Resource.Drawable.tokstarguy2crop);
                    break;
                case 2:
                    view.SetBackgroundResource(Resource.Drawable.tokstargirl2crop);
                    break;
                case 3:
                    view.SetBackgroundResource(Resource.Drawable.tokstargirl1crop);
                    break;
                case 4:
                    view.SetBackgroundResource(Resource.Drawable.tokstarguy4crop);
                    break;
                case 5:
                    view.SetBackgroundResource(Resource.Drawable.tokstarguy1crop);
                    break;
             
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


        public ImageView ImageMain => FindViewById<ImageView>(Resource.Id.mainImg_guide);

        public RecyclerView InstructionsRecycler => FindViewById<RecyclerView>(Resource.Id.instructions_rec);
    }
}