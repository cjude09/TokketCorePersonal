using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tokket.Android.Adapters;
using Tokket.Android.Fragments;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core.Tools;
using Tokket.Shared.ViewModels.Base;

namespace Tokket.Android.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        #region Properties
        //public ObservableCollection<TokModel> tokModelLists { get; set; }
        public TokCardDataAdapter tokcardDataAdapter { get; set; }
        public List<TokModel> TokDataList {get; set;}
        #endregion

        public async Task<List<TokModel>> GetToksData(string filter = "", FilterType type = FilterType.None, string sortbyfilter = "standard")
        {
            ImageType imgtype = (ImageType)Settings.FilterImage;

            bool? image = imgtype == ImageType.Both ? null as bool? :
                         imgtype == ImageType.Image ? true : false;


            Settings.FilterTag = Convert.ToInt32(type);
            var toks = new List<TokModel>();
            var tokResult = new List<TokModel>();
            UserModel myAccount = JsonConvert.DeserializeObject<UserModel>(Settings.UserAccount);
            string strtoken = myAccount.StreamToken;
            switch (type)
            {
                case FilterType.None:
                    if (myAccount != null)
                        toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken, image = image});
                    else
                        toks = await TokService.Instance.GetAllFeaturedToks();
                    break;
                case FilterType.TokType:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { toktype = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.Text:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { text = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.Category:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { category = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.Country:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { country = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.User:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { userid = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.Group:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { tokgroup = filter, streamtoken = strtoken, image = image });
                    break;
                case FilterType.Featured:
                    toks = await TokService.Instance.GetAllFeaturedToks();
                    break;
                case FilterType.All:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken, image = image });
                    break;
                case FilterType.Standard:
                    string sorting = "";
                    if (image == null)
                    {
                        sorting = sortbyfilter;
                    }

                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken, sortby = sorting, image = image });
                    break;
                case FilterType.Recent:
                    if (myAccount != null)
                        tokResult = await TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken, sortby = sortbyfilter, image = image });
                    else
                        tokResult = await TokService.Instance.GetAllFeaturedToks();

                    break;
                default:
                    toks = await TokService.Instance.GetToksAsync(new TokQueryValues() { streamtoken = strtoken, image = image });
                    break;
            }

            if (type != FilterType.Recent)
            {
                tokResult = TokService.Instance.AlternateToks(toks);
            }

            var toksWithSticker = new List<TokModel>();
            tokResult = tokResult.OrderByDescending(x => x.DateCreated.Value).ToList() ?? new List<TokModel>();
            var cnt = 0;
            foreach (var tok in tokResult)
            {
                var sticker = StickersTool.Stickers.FirstOrDefault(x => x.Id == (string.IsNullOrEmpty(tok.Sticker) ? tok.Sticker : tok.Sticker.Split("-")[0]));
                tok.StickerImage = sticker?.Image ?? string.Empty;
                tok.IndexCounter = cnt;
                toksWithSticker.Add(tok);
                cnt += 1;
            }
            toksWithSticker = toksWithSticker.ToList();
            return tokResult;
        }
        public async Task GetSearchData(TokQueryValues tokQueryValues)
        {
            tokQueryValues.sortby = Settings.SortByFilter;
            tokQueryValues.token = Settings.ContinuationToken;
            tokQueryValues.loadmore = "yes";
            var result = await TokService.Instance.GetAllToks(tokQueryValues);

            if (Settings.MaintTabInt == (int)MainTab.Home)
            {
                if (Settings.FilterToksHome == (int)FilterToks.Toks)
                {
                    HomeFragment.HFInstance.tokDataAdapter.UpdateItems(result);
                    TokDataList.AddRange(result);
                }
                else if (Settings.FilterToksHome == (int)FilterToks.Cards)
                {
                    tokcardDataAdapter.UpdateItems(result);
                }
            }
            else if (Settings.MaintTabInt == (int)MainTab.Search)
            {
                if (Settings.FilterToksSearch == (int)FilterToks.Toks)
                {
                    HomeFragment.HFInstance.tokDataAdapter.UpdateItems(result);
                    TokDataList.AddRange(result);
                }
                else if (Settings.FilterToksSearch == (int)FilterToks.Cards)
                {
                    tokcardDataAdapter.UpdateItems(result);
                }
            }
        }
    }
}