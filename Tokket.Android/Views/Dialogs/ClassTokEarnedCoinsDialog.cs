using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Bumptech.Glide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models;

namespace Tokket.Android.Custom
{
    public class ClassTokEarnedCoinsDialog : Dialog
    {
        private TextView txtTypeTok;
        private LinearLayout linearParent;
        private ImageView imgCoin;
        private TextView txtEarnedCoins;
        private Button btnContinue;
        public ClassTokEarnedCoinsDialog(Context context, ClassTokModel tok, EventHandler handlerClick) : base(context)
        {
            SetContentView(Resource.Layout.dialog_class_tok_earned_coins);

            Window.SetBackgroundDrawableResource(Resource.Color.transparent);
            this.SetCanceledOnTouchOutside(false);

            txtTypeTok = (TextView)FindViewById(Resource.Id.txtTypeTok);
            linearParent = (LinearLayout)FindViewById(Resource.Id.linearParent);
            imgCoin = (ImageView)FindViewById(Resource.Id.imgCoin);
            txtEarnedCoins = (TextView)FindViewById(Resource.Id.txtEarnedCoins);
            btnContinue = (Button)FindViewById(Resource.Id.btnContinue);

            Glide.With(context)
                .Load(Resource.Drawable.gold)
                .Into(imgCoin);

            btnContinue.Click += handlerClick;
            btnContinue.Click += (s, e) =>
            {
                Dismiss();
            };

            txtEarnedCoins.Text = $"You earned {GetPointsCoins(tok).Item2} coins.";

            linearParent.SetBackgroundResource(Resource.Drawable.tileview_layout);
            GradientDrawable Tokdrawable = (GradientDrawable)linearParent.Background;

            Tokdrawable.SetColor(Color.White);
            Tokdrawable.SetStroke(10, new Color(ContextCompat.GetColor(context, Resource.Color.colorPrimary)));
        }

        private (int, int) GetPointsCoins(ClassTokModel tok)
        {
            int pointsEarned = 0, coinsEarned = 0;
            if (tok?.IsMegaTok ?? false)
            {
                coinsEarned = CoinsMegaTok;
                pointsEarned = 15;
                txtTypeTok.Text = "for adding a Mega Tok!";
            }
            else if (tok.IsDetailBased)
            {
                var typeTok = "Detailed";
                if (tok.TokGroup.ToLower() == "list")
                {
                    typeTok = "List";
                }

                coinsEarned = CoinsDetailedTok;
                pointsEarned = 6;
                txtTypeTok.Text = $"for adding a {typeTok} Tok!";
            }
            else
            {
                coinsEarned = CoinsBasicTok;
                pointsEarned = 2;
                if (tok.TokGroup.ToLower() == "pic")
                {
                    txtTypeTok.Text = "for adding a Pic Tok!";
                }
                else
                {
                    txtTypeTok.Text = "for adding a Basic Tok!";
                }
            }

            return (pointsEarned, coinsEarned);
        }

        public const int CoinsBasicTok = 2;
        public const int CoinsDetailedTok = 5;
        public const int CoinsMegaTok = 10;
    }
}