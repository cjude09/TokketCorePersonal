using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight;

namespace Tokket.Android.ViewModels
{
    class ToksViewModel : ViewModelBase
    {
        //public async Task InitializeData()
        //{
        //    recyclerView.SetAdapter(null);
        //    shimmerLayout = FindViewById<ShimmerLayout>(Resource.Id.toks_shimmer_view_container);
        //    shimmerLayout.StartShimmerAnimation();
        //    shimmerLayout.Visibility = ViewStates.Visible;
        //    var result = await GetToksData();
        //    tokDataAdapter = new TokDataAdapter(result);
        //    tokDataAdapter.ItemClick += tokDataAdapter.OnGridBackgroundClick;
        //    recyclerView.SetAdapter(tokDataAdapter);
        //    shimmerLayout.Visibility = ViewStates.Invisible;
        //}
        //public async Task<List<Shared.Models.TokModel>> GetToksData()
        //{
        //    tokResult = new List<Shared.Models.TokModel>();
        //    switch (type)
        //    {
        //        case FilterType.TokType:
        //            tokResult = await SharedService.TokService.Instance.GetAllToks(new TokQueryModel() { toktype = filter, streamtoken = strtoken });
        //            break;
        //        case FilterType.Category:
        //            tokResult = await SharedService.TokService.Instance.GetAllToks(new TokQueryModel() { category = filter?.Replace("category-", ""), streamtoken = strtoken });
        //            break;
        //        case FilterType.Group:
        //            tokResult = await SharedService.TokService.Instance.GetAllToks(new TokQueryModel() { tokgroup = filter, streamtoken = strtoken });
        //            break;
        //        default:
        //            tokResult = await SharedService.TokService.Instance.GetAllToks(new TokQueryModel() { streamtoken = strtoken });
        //            break;
        //    }
        //    var toksWithSticker = new List<TokModel>();
        //    tokResult = tokResult.OrderByDescending(x => x.DateCreated.Value).ToList() ?? new List<TokModel>();
        //    var cnt = 0;
        //    foreach (var tok in tokResult)
        //    {
        //        var sticker = StickersTool.Stickers.FirstOrDefault(x => x.Id == (string.IsNullOrEmpty(tok.Sticker) ? tok.Sticker : tok.Sticker.Split("-")[0]));
        //        tok.StickerImage = sticker?.Image ?? string.Empty;
        //        tok.IndexCounter = cnt;
        //        toksWithSticker.Add(tok);
        //        cnt += 1;
        //    }
        //    toksWithSticker = toksWithSticker.ToList();
        //    return tokResult;
        //}
        //public async Task GetSearchData()
    //    {
    //        FilterType type = (FilterType)Settings.FilterTag;
    //        TokQueryModel tokQueryModel = new TokQueryModel();
    //        switch (type)
    //        {
    //            case FilterType.TokType:
    //                tokQueryModel.toktype = filter;
    //                break;
    //            case FilterType.Category:
    //                tokQueryModel.category = filter?.Replace("category-", "");
    //                break;
    //            case FilterType.Group:
    //                tokQueryModel.tokgroup = filter;
    //                break;
    //            default:
    //                break;
    //        }

    //        tokQueryModel.token = Settings.ContinuationToken;
    //        tokQueryModel.loadmore = "yes";
    //        var result = await SharedService.TokService.Instance.GetAllToks(tokQueryModel);
    //        tokDataAdapter.UpdateItems(result);
    //    }
    }
}