using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models.Purchase
{
    public enum PurchaseType { Consumable, NonConsumable, Subscription };
    public enum ProductType { NoAds, Strikes, Saved, Coins, Avatars, Tokmoji, Titles, Subaccounts, Teams, MultiplayerRooms, Eliminators, GenericTitles, GroupAccount, RoyaltyMembership, RoyaltyTitles, Treasure, EducationMode };
    public enum TokketId
    {
        Tokket = 0,
        AlphaGuess = 1,
        TokBlitz = 2,
        Tokkepedia = 3,
        TokBlast = 4,
        TokBoom = 0,
    }
    public class PurchaseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Image { get; set; }
        public double PriceUSD { get; set; }
        public int PriceCoins { get; set; }

        public int Quantity { get; set; } = 0;

        public PurchaseType PurchaseType { get; set; }
        public ProductType ProductType { get; set; }
        public TokketId TokketType { get; set; }
    }
}
