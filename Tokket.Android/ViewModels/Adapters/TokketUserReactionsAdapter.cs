using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Tokket.Shared.Models;
using Tokket.Android.ViewHolders;

namespace Tokket.Android.Adapters
{
    class TokketUserReactionsAdapter : RecyclerView.Adapter
    {

        #region Members/Properties
        public event EventHandler<int> ItemClick;
        View itemView;
        List<TokketUserReaction> Users;
        public override int ItemCount => Users.Count;
        #endregion

        #region Constructor
        public TokketUserReactionsAdapter(List<TokketUserReaction> _users)
        {
            Users = _users;
        }
        public void UpdateItems(List<TokketUserReaction> listUpdate)
        {
            Users.AddRange(listUpdate);
            NotifyDataSetChanged();
        }
        #endregion

        #region Override Events/Methods/Delegates
        UserReactionsViewHolder vh;
        int selectedPosition = -1;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as UserReactionsViewHolder;
            var res = Application.Context.Resources;

            Glide.With(itemView).Load(Users[position].UserPhoto).Apply(RequestOptions.CircleCropTransform().Placeholder(Resource.Drawable.Man3).CircleCrop()).Into(vh.UserPhoto);
            vh.Username.Text = Users[position].DisplayName;

            if (Users[position].Kind == "gema")
            {
                Glide.With(itemView).AsBitmap().Load(Resource.Drawable.baseline_favorite_black_24).Into(vh.UserReactions);
            }
            else if (Users[position].Kind == "gemb")
            {
                Glide.With(itemView).AsBitmap().Load(Resource.Drawable.hundred_24px).Into(vh.UserReactions);
            }
            else if (Users[position].Kind == "gemc")
            {
                Glide.With(itemView).AsBitmap().Load(Resource.Drawable.purple_gem).Into(vh.UserReactions);
            }
            else if (Users[position].Kind == "accurate")
            {
                Glide.With(itemView).AsBitmap().Load(Resource.Drawable.check_black_48dp).Into(vh.UserReactions);
            }
            else if (Users[position].Kind == "inaccurate")
            {
                Glide.With(itemView).AsBitmap().Load(Resource.Drawable.clear_black_48dp).Into(vh.UserReactions);
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.reactionvalues_users_row, parent, false);

            vh = new UserReactionsViewHolder(itemView, OnClick);

            return vh;
        }
        #endregion

        #region Custom Events/Delegates
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
        #endregion
    }
}