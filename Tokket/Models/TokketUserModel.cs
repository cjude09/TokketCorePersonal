using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tokket.Core;
using Tokket.Core.Tools;
using Tokket.Shared.Helpers;

namespace Tokket.Models
{
    public class TokketUserModel : TokketUser, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isOwner;
        private bool isMember;

        private bool isInvited;

        public string _imageFlag;
        public string _stateOrCountry;
        public string _stateOrCountryText;

        public bool IsOwner { get => isOwner; set { SetProperty(ref isOwner, value); } }
        public bool IsMember { get => isMember; set { SetProperty(ref isMember, value); } }

        public bool IsInvited { get => isInvited; set { SetProperty(ref isInvited, value); } }

        public string DisplayPhoto => UserPhoto ?? "man3.png";
        public string ImageFlag { get => _imageFlag; set { SetProperty(ref _imageFlag, value); } }
        public string StateOrCountry { get => _stateOrCountry; set { SetProperty(ref _stateOrCountry, value); } }
        public string StateOrCountryText { get => _stateOrCountryText; set { SetProperty(ref _stateOrCountryText, value); } }

        protected virtual bool SetProperty<T>(
          ref T backingStore, T value,
          [CallerMemberName] string propertyName = "",
          Action onChanged = null,
          Func<T, T, bool> validateValue = null)
        {
            //if value didn't change
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            //if value changed but didn't validate
            if (validateValue != null && !validateValue(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }


    public static class TokketUserExtensions
    {
        public static void UserImageFlagSetUp(this TokketUserModel item)
        {

            string flagImg = "";

            try
            {
                if (item.Id == Settings.GetTokketUser().Id)
                {
                    item.Country = Settings.GetTokketUser().Country;
                    item.State = Settings.GetTokketUser().State;
                }

                if (string.IsNullOrEmpty(item.Country))
                {
                    flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
                }
                else if (item.Country.ToLower() == "us")
                {
                    if (!string.IsNullOrEmpty(item.State))
                    {
                        try
                        {
                            var stateModel = CountryHelper.GetCountryStates("us").Find(x => x.Id == item.State);
                            flagImg = stateModel.Image;
                        }
                        catch (Exception)
                        {

                            flagImg = CountryTool.GetCountryFlagJPG1x1(item.State);
                        }
                    }
                    else
                    {
                        flagImg = CountryTool.GetCountryFlagJPG1x1(item.Country);
                    }
                }
                else
                {
                    flagImg = CountryTool.GetCountryFlagJPG1x1(item.Country);
                }
            }
            catch (Exception)
            {


                flagImg = "https://tokketcontent.blob.core.windows.net/pointssymbol48/set1-black%20(0).jpg";
            }

            item.ImageFlag = flagImg;

        }


        public static void UserStateOrCountrySetUp(this TokketUserModel item)
        {

            string stateOrCountry = "";
            string stateOrCountryText = "";


            try
            {


                if (string.IsNullOrEmpty(item.Country))
                {
                    stateOrCountry = "COUNTRY";
                    stateOrCountryText = "US";

                }
                else if (item.Country.ToLower() == "us")
                {
                    if (!string.IsNullOrEmpty(item.State))
                    {
                        stateOrCountry = "STATE";
                        stateOrCountryText = item.State.ToUpper();

                    }
                    else
                    {
                        stateOrCountry = "COUNTRY";
                        stateOrCountryText = "US";

                    }
                }
                else
                {
                    stateOrCountry = "COUNTRY";
                    stateOrCountryText = item.Country.ToUpper();

                }
            }
            catch (Exception)
            {
                stateOrCountry = "COUNTRY";
                stateOrCountryText = "NA";
            }

            item.StateOrCountry = stateOrCountry;
            item.StateOrCountryText = stateOrCountryText;


        }

    }


}
