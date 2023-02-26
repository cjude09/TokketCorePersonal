using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services;
using Tokket.Core;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Tokket.Android.ViewModels
{
    public class TokInfoViewModel : ObservableObject
    {
        public ObservableCollection<ReactionModel> CommentsCollection { get; private set; }

        public ObservableCollection<ReactionModel> InaccuratesCollection { get; private set; }
        public ObservableCollection<Tokmoji> TokMojiCollection { get; private set; }
        public ProgressBar CircleProgress { get; set; }
        public int commentsloaded = 0;
        public double StarCountAverage = 0;
        public bool hasInaccurates { get; private set; } = false;
        public TokInfoViewModel()
        {
            CommentsCollection = new ObservableCollection<ReactionModel>();
            CommentsCollection.Clear();

            TokMojiCollection = new ObservableCollection<Tokmoji>();
            TokMojiCollection.Clear();

            InaccuratesCollection = new ObservableCollection<ReactionModel>();
            InaccuratesCollection.Clear();
        }
        public async Task LoadComments(string tokid,string continuationtoken = "", string fromCaller  = "")
        {
            ReactionQueryValues reactionQueryValues = new ReactionQueryValues();
            reactionQueryValues.item_id = tokid;
            reactionQueryValues.kind = "comments";
            reactionQueryValues.detail_number = -1;
            reactionQueryValues.pagination_id = continuationtoken;
            reactionQueryValues.user_likes = true;
            reactionQueryValues.userid = Settings.GetTokketUser().Id;
            if (!string.IsNullOrEmpty(continuationtoken))
            {
                //Load More
                CircleProgress.Visibility = ViewStates.Visible;
            }
            hasInaccurates = false;
            var resultComments = await ReactionService.Instance.GetReactionsAsync(reactionQueryValues, fromCaller);
            resultComments.Results = resultComments.Results.OrderByDescending(x => x.CreatedTime).ToList();
            foreach (var comment in resultComments.Results)
            {
                if (comment.Kind != "inaccurate")
                    CommentsCollection.Add(comment);

                if (comment.Kind == "inaccurate") {
                    hasInaccurates = true;
                    InaccuratesCollection.Add(comment);
                }
                 
            }

            if (!string.IsNullOrEmpty(continuationtoken))
            {
                commentsloaded += resultComments.Results.Count();
                CircleProgress.Visibility = ViewStates.Gone;
            }

        }
        public string LoadCommentsCache(string fromCaller)
        {
            var resultComments = ReactionService.Instance.GetReactionsCache(fromCaller);
            if (resultComments.Results != null)
            {
                resultComments.Results = resultComments.Results.OrderByDescending(x => x.CreatedTime).ToList();
                foreach (var comment in resultComments.Results)
                {
                    if (comment.Kind != "inaccurate")
                        CommentsCollection.Add(comment);

                    if (comment.Kind == "inaccurate")
                    {
                        hasInaccurates = true;
                        InaccuratesCollection.Add(comment);
                    }

                }
            }            
            return resultComments.ContinuationToken;
        }
        public async Task LoadRatings(string tokid, string continuationtoken = "")
        {
            ReactionQueryValues reactionQueryValues = new ReactionQueryValues();
            reactionQueryValues.item_id = tokid;
            reactionQueryValues.kind = "star_rating";
            reactionQueryValues.detail_number = -1;
            reactionQueryValues.pagination_id = continuationtoken;
            reactionQueryValues.user_likes = true;
            reactionQueryValues.userid = Settings.GetTokketUser().Id;
            if (!string.IsNullOrEmpty(continuationtoken))
            {
                //Load More
                CircleProgress.Visibility = ViewStates.Visible;
            }

            var resultComments = await ReactionService.Instance.GetReactionsAsync(reactionQueryValues);
            resultComments.Results = resultComments.Results.OrderByDescending(x => x.CreatedTime).ToList();
            foreach (var comment in resultComments.Results)
            {
                CommentsCollection.Add(comment);
            }
            List<double> average = new List<double>();
            foreach (var comment in resultComments.Results)
            {

                average.Add(comment.StarRatingCount.Value);
            }
            if (average.Count >0)
                 StarCountAverage = average.Average();
            else
                StarCountAverage = 0;

            if (!string.IsNullOrEmpty(continuationtoken))
            {
                commentsloaded += resultComments.Results.Count();
                CircleProgress.Visibility = ViewStates.Gone;
            }

         
        }
        public async Task<List<ReactionModel>> LoadRating(string tokid, string continuationtoken = "")
        {
            ReactionQueryValues reactionQueryValues = new ReactionQueryValues();
            reactionQueryValues.item_id = tokid;
            reactionQueryValues.kind = "star_rating";
            reactionQueryValues.detail_number = -1;
            reactionQueryValues.pagination_id = continuationtoken;
            reactionQueryValues.user_likes = true;
            reactionQueryValues.userid = Settings.GetTokketUser().Id;
            if (!string.IsNullOrEmpty(continuationtoken))
            {
                //Load More
                CircleProgress.Visibility = ViewStates.Visible;
            }

            var resultComments = await ReactionService.Instance.GetReactionsAsync(reactionQueryValues);
            resultComments.Results = resultComments.Results.OrderByDescending(x => x.CreatedTime).ToList();


            return resultComments.Results.ToList();

        }
        public async Task LoadTokMoji()
        {
            //var resultTokMoji = PurchasesHelper.GetProducts().Where(x => x.ProductType == ProductType.Tokmoji).ToList();
            var resultTokMoji = await TokMojiService.Instance.GetTokmojisAsync();
            if (resultTokMoji != null) {
                var resultList = resultTokMoji.Results.ToList();
                foreach (var stickers in resultList)
                {
                    TokMojiCollection.Add(stickers);
                }
            }
          
        }
    }
}