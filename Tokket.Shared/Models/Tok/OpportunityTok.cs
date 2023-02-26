using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Helpers;
using Tokket.Shared.Services;
using Tokket.Core.Tools;

namespace Tokket.Shared.Models.Tok
{
    public class OpportunityTok : TokModel
    {
        [JsonProperty(PropertyName = "opportunity_type", NullValueHandling = NullValueHandling.Ignore)]
        public string OpportunityType { get; set; } = "";

        [JsonProperty(PropertyName = "application_deadline", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationDeadline { get; set; }

        [JsonProperty(PropertyName = "amount", NullValueHandling = NullValueHandling.Ignore)]
        public double Amount { get; set; } = 0;

        [JsonProperty(PropertyName = "awards_available", NullValueHandling = NullValueHandling.Ignore)]
        public int AwardsAvailable { get; set; } = 0;

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "email_address", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; } = "";

        [JsonProperty(PropertyName = "address", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddress { get; set; } = "";

        [JsonProperty(PropertyName = "phone_number", NullValueHandling = NullValueHandling.Ignore)]
        public string PhoneNumber { get; set; } = "";

        [JsonProperty(PropertyName = "about_company", NullValueHandling = NullValueHandling.Ignore)]
        public string AboutCompany { get; set; } = "";

        [JsonProperty(PropertyName = "website", NullValueHandling = NullValueHandling.Ignore)]
        public string Website { get; set; } = "";

        [JsonProperty(PropertyName = "requirements", NullValueHandling = NullValueHandling.Ignore)]
        public string Requirements { get; set; } = "";

        [JsonProperty(PropertyName = "opportunity_kind", NullValueHandling = NullValueHandling.Ignore)]
        public string OpportunityKind { get; set; } = "";

        [JsonProperty(PropertyName = "image_opportunity", NullValueHandling = NullValueHandling.Ignore)]
        public string OpportunityImage { get; set; } = "";

        [JsonProperty(PropertyName = "item_country", NullValueHandling = NullValueHandling.Ignore)]
        public string ItemCountry { get; set; } = null;

        [JsonProperty(PropertyName = "item_state", NullValueHandling = NullValueHandling.Ignore)]
        public string ItemState { get; set; } = null;

        #region Shared Files on Training Toks
        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } = "";
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = "";
        [JsonProperty(PropertyName = "state_province", NullValueHandling = NullValueHandling.Ignore)]
        public string StateProvince { get; set; } = "";
        [JsonProperty(PropertyName = "city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; } = "";
        [JsonProperty(PropertyName = "other_detail", NullValueHandling = NullValueHandling.Ignore)]
        public string[] OtherDetail { get; set; } = null;
        #endregion

        #region Course Tok
        [JsonProperty(PropertyName = "how_it_works", NullValueHandling = NullValueHandling.Ignore)]
        public string HowItWorks { get; set; } = "";
        [JsonProperty(PropertyName = "instructor", NullValueHandling = NullValueHandling.Ignore)]
        public string Instructors { get; set; } = null;
        [JsonProperty(PropertyName = "enrolled", NullValueHandling = NullValueHandling.Ignore)]
        public int Enrolled { get; set; } = 0;
        [JsonProperty(PropertyName = "faq", NullValueHandling = NullValueHandling.Ignore)]
        public string FAQ { get; set; } = "";

        [JsonProperty(PropertyName = "school", NullValueHandling = NullValueHandling.Ignore)]
        public string School { get; set; } = "";

        [JsonProperty(PropertyName = "enrollment_options", NullValueHandling = NullValueHandling.Ignore)]
        public string EnrollmentOptions { get; set; } = "";

        [JsonProperty(PropertyName = "days", NullValueHandling = NullValueHandling.Ignore)]
        public string Days { get; set; } = "";

        [JsonProperty(PropertyName = "start_time", NullValueHandling = NullValueHandling.Ignore)]
        public string StartTime { get; set; } = "";
        [JsonProperty(PropertyName = "end_time", NullValueHandling = NullValueHandling.Ignore)]
        public string EndTime { get; set; } = "";

        [JsonProperty(PropertyName = "dates", NullValueHandling = NullValueHandling.Ignore)]
        public string Dates { get; set; } = "";
        [JsonProperty(PropertyName = "units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; } = "";
        [JsonProperty(PropertyName = "enrollment_requirements", NullValueHandling = NullValueHandling.Ignore)]
        public string EnrollmentRequirements { get; set; } = "";

        [JsonProperty(PropertyName = "reservation_requirements", NullValueHandling = NullValueHandling.Ignore)]
        public string ReservationRequirements { get; set; } = "";
        [JsonProperty(PropertyName = "course_materials", NullValueHandling = NullValueHandling.Ignore)]
        public string CourseMaterials { get; set; } = "";
        #endregion

        #region Program Tok
        [JsonProperty(PropertyName = "outline", NullValueHandling = NullValueHandling.Ignore)]
        public string Outline { get; set; } = null;
        [JsonProperty(PropertyName = "cost", NullValueHandling = NullValueHandling.Ignore)]
        public string Cost { get; set; } = "";
        [JsonProperty(PropertyName = "schedule", NullValueHandling = NullValueHandling.Ignore)]
        public string Schedule { get; set; } = "";
        [JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore)]
        public string Level { get; set; } = "";
        [JsonProperty(PropertyName = "about", NullValueHandling = NullValueHandling.Ignore)]
        public string About { get; set; } = "";
        [JsonProperty(PropertyName = "duration", NullValueHandling = NullValueHandling.Ignore)]
        public string Duration { get; set; } = "";
        [JsonProperty(PropertyName = "related_skills", NullValueHandling = NullValueHandling.Ignore)]
        public string RelatedSkills { get; set; } = "";
        [JsonProperty(PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore)]
        public int Students { get; set; } = 0;
        [JsonProperty(PropertyName = "projects", NullValueHandling = NullValueHandling.Ignore)]
        public int Projects { get; set; } = 0;
        [JsonProperty(PropertyName = "assigments", NullValueHandling = NullValueHandling.Ignore)]
        public int Assigments { get; set; } = 0;
        [JsonProperty(PropertyName = "instructor_info", NullValueHandling = NullValueHandling.Ignore)]
        public string InstructorInfo { get; set; } = "";
        #endregion

        #region Tutor Tok
        [JsonProperty(PropertyName = "experience", NullValueHandling = NullValueHandling.Ignore)]
        public string Experience { get; set; } = "";
        [JsonProperty(PropertyName = "other_interest", NullValueHandling = NullValueHandling.Ignore)]
        public string OtherInterest { get; set; } = "";
        [JsonProperty(PropertyName = "education", NullValueHandling = NullValueHandling.Ignore)]
        public string Education { get; set; } = "";

        [JsonProperty(PropertyName = "tutor_description1", NullValueHandling = NullValueHandling.Ignore)]
        public string TutorDescription1 { get; set; } = "";
        [JsonProperty(PropertyName = "tutor_description2", NullValueHandling = NullValueHandling.Ignore)]
        public string TutorDescription2 { get; set; } = "";
        #endregion


    }



    #region Change Log
    //    If in Backend Field, if it says "Detail", instead of an array make it its own string property for OpportunityTok.cs.
    //For example: Reservation Requirement Detail 16
    //Instead of OtherDetails[], do ReservationRequirement "reservation_requirement"
    //Think of this in terms of searching, this needs to be its own line in QueryValues
    #endregion

    public static class TrainingXFExtensions
    {

        // falg setup
        public static async void FlagSetUp(this OpportunityTok item)
        {

            string flagImg = "";

            try
            {
                if (item.UserId == Settings.GetTokketUser().Id)
                {
                    item.UserCountry = Settings.GetTokketUser().Country;
                    item.UserState = Settings.GetTokketUser().State;
                    var user = Settings.GetTokketUser();
                    var ispatch = user.IsPointsSymbolEnabled;
                    if (ispatch != null)
                    {
                        if (ispatch.ToString().ToLower() == "true")
                        {
                            var currentLevel = PointsSymbolsHelper.GetPatchExactResult(user.Points);
                            flagImg = currentLevel.Image;
                        }
                        else
                        {

                            flagImg = SetFlag(item);
                        }
                    }

                }
                else
                {
                    var getUser = await AccountService.Instance.GetUserAsync(item.UserId);
                    var ispatch = getUser.IsPointsSymbolEnabled;
                    if (ispatch != null)
                    {
                        if (ispatch.ToString().ToLower() == "true")
                        {
                            var currentLevel = PointsSymbolsHelper.GetPatchExactResult(getUser.Points);
                            flagImg = currentLevel.Image;
                        }
                        else
                        {

                            flagImg = SetFlag(item);
                        }
                    }
                    flagImg = SetFlag(item);

                }

            }
            catch (Exception)
            {

                flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
            }

            item.UserCountry = flagImg;

        }


        private static string SetFlag(OpportunityTok item)
        {

            var flagImg = "";
            if (string.IsNullOrEmpty(item.UserCountry))
            {
                flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
            }
            else if (item.UserCountry.ToLower() == "us")
            {
                if (!string.IsNullOrEmpty(item.UserState))
                {
                    try
                    {
                        var stateModel = CountryHelper.GetCountryStates("us").Find(x => x.Id == item.UserState);
                        flagImg = stateModel.Image;
                    }
                    catch (Exception)
                    {

                        flagImg = CountryTool.GetCountryFlagJPG1x1(item.UserState);
                    }
                }
                else
                {
                    flagImg = CountryTool.GetCountryFlagJPG1x1(item.UserCountry);
                }
            }
            else
            {
                flagImg = CountryTool.GetCountryFlagJPG1x1(item.UserCountry);
            }
            return flagImg;
        }
    }

}
