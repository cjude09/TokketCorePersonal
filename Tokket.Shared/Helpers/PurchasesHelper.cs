using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Core;
using Tokket.Shared.Models.Purchase;

namespace Tokket.Shared.Helpers
{
    public class PurchasesHelper
    {
        #region Default Members
        //private static ShippingDetail _defaultShippingDetails = new ShippingDetail
        //{
        //    Name = new Name
        //    {
        //        FullName = "Tokket Inc."
        //    },
        //    AddressPortable = new AddressPortable
        //    {
        //        AddressLine1 = "N/a",
        //        AddressLine2 = "N/a",
        //        AdminArea2 = "N/a",
        //        AdminArea1 = "N/a",
        //        PostalCode = "0",
        //        CountryCode = "US"
        //    }
        //};
        //public static ApplicationContext _defaultApplicationContext = new ApplicationContext
        //{
        //    BrandName = "TOKKET INC",
        //    LandingPage = "BILLING",
        //    UserAction = "CONTINUE",
        //    ShippingPreference = "NO_SHIPPING"
        //};
        #endregion


        private static List<PurchaseModel> _products = new List<PurchaseModel>() {
            #region List of products
            //Non TokBlitz, template only
            //new PurchaseModel(){ Id="coins_1", Name="50 Coins", Type="consumable", PriceUSD = 0.99, Quantity=50, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Coins },
            //new PurchaseModel(){ Id="coins_2", Name="128 Coins", Type="consumable", PriceUSD = 0.99, Quantity=128, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Coins },
            //new PurchaseModel(){ Id="no_ads", Name="No Ads", Type="nonconsumable", PriceUSD = 0.99, Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.NoAds },
            //---END
            
            //Avatars
            new PurchaseModel(){ Id="avatar_1", Name="Lani Jaramillo", Type="nonconsumable", PriceUSD = 0, PriceCoins = 3, Image="https://tokketcontent.blob.core.windows.net/avatar/avatar1.jpg", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Avatars },
            new PurchaseModel(){ Id="avatar_2", Name="Joleen Hackett", Type="nonconsumable", PriceUSD = 0,  PriceCoins = 3, Image="https://tokketcontent.blob.core.windows.net/avatar/avatar2.jpg", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Avatars },
            new PurchaseModel(){ Id="avatar_3", Name="Eloisa Tobin", Type="nonconsumable", PriceUSD = 0, PriceCoins = 3, Image="https://tokketcontent.blob.core.windows.net/avatar/avatar3.jpg", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Avatars },
            new PurchaseModel(){ Id="avatar_4", Name="Arron	Crider", Type="nonconsumable", PriceUSD = 0, PriceCoins = 3, Image="https://tokketcontent.blob.core.windows.net/avatar/avatar4.jpg", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Avatars },
            new PurchaseModel(){ Id="avatar_5", Name="Russ Yost", Type="nonconsumable", PriceUSD = 0, PriceCoins = 3, Image="https://tokketcontent.blob.core.windows.net/avatar/avatar5.jpg", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Avatars },
            //Tokmoji
            new PurchaseModel(){ Id="tokmoji_1", Name="Cool", Type="nonconsumable", PriceUSD = 0.00, PriceCoins = 5, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_Cool.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },
            new PurchaseModel(){ Id="tokmoji_2", Name="Thanks for posting this", Type="nonconsumable", PriceUSD = 0.99,  PriceCoins = 5, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_Thanks-for-posting-this.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },
            new PurchaseModel(){ Id="tokmoji_3", Name="That's incorrect", Type="nonconsumable", PriceUSD = 0.99, PriceCoins = 45, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_That_s-incorrect.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },
            new PurchaseModel(){ Id="tokmoji_4", Name="That's incorrect (Style 2)", Type="nonconsumable", PriceUSD = 0.99, PriceCoins = 5, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_That_s-incorrect2.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },
            new PurchaseModel(){ Id="tokmoji_5", Name="That's interesting", Type="nonconsumable", PriceUSD = 0.99, PriceCoins = 5, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_That_s-Intresting.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },
            new PurchaseModel(){ Id="tokmoji_6", Name="Wow", Type="nonconsumable", PriceUSD = 0.99, PriceCoins = 5, Image="https://tokketcontent.blob.core.windows.net/images/TOKMOJI_wow.png", Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Tokmoji, TokketType = TokketId.Tokkepedia },

            #region Tok Blitz
            //Eliminators
            new PurchaseModel(){ Id="eliminators_tokblitz1", Name="144 Strikes", Type="consumable", PriceUSD = 2.99, Quantity=144, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="eliminators_tokblitz2", Name="288 Strikes", Type="consumable", PriceUSD = 4.99, Quantity=288, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="eliminators_tokblitz3", Name="624 Strikes", Type="consumable", PriceUSD = 9.99, Quantity=624, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="eliminators_tokblitz4", Name="1800 Strikes", Type="consumable", PriceUSD = 19.99, Quantity=1800, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="no_ads_tokblitz", Name="No Ads (Tok Blitz)", Type="nonconsumable", PriceUSD = 2.99, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.NoAds, TokketType = TokketId.TokBlitz },
            
            //Saved Games
            //new PurchaseModel(){ Id="tokblitzcoins_saved1", Name="4 Saved Games", Type="nonconsumable", PriceCoins = 90, Quantity=4, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="saved_tokblitz1", Name="15 Saved Games", Type="nonconsumable", PriceUSD = 3.99, Quantity=25, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="saved_tokblitz2", Name="30 Saved Games", Type="nonconsumable", PriceUSD = 5.99, Quantity=50, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlitz },
            
            //Tok Blitz Coin Purchase
            //Uncomment later once coins enabled
            //new PurchaseModel(){ Id="tokblitzcoins_eliminators1", Name="7 eliminators", Type="consumable", PriceCoins = 10, Quantity=7, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            //new PurchaseModel(){ Id="tokblitzcoins_eliminators2", Name="50 Coins", Type="consumable", PriceCoins = 60, Quantity=50, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlitz },
            
            //Teams
            new PurchaseModel(){ Id="teams_tokblitz1", Name="2 Teams", Type="consumable", PriceUSD = 1.99, Quantity=2, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Teams, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="teams_tokblitz2", Name="4 Teams", Type="consumable", PriceUSD = 2.99, Quantity=4, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Teams, TokketType = TokketId.TokBlitz },
            new PurchaseModel(){ Id="teams_tokblitz3", Name="8 Teams", Type="consumable", PriceUSD = 4.99, Quantity=8, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Teams, TokketType = TokketId.TokBlitz },
            
            //Room purchase
            new PurchaseModel(){ Id="room_purchased_tokblitz", Name="Multiplayer Personal Room (Tok Blitz)", Type="nonconsumable", PriceUSD = 2.99, Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.MultiplayerRooms, TokketType = TokketId.TokBlitz },
            #endregion

            //Title
            new PurchaseModel(){ Id="title_tokket", Name="Title", Type="nonconsumable", PriceUSD = 4.99, Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Titles, TokketType = TokketId.Tokket },

            //Subaccount
            new PurchaseModel(){ Id="subaccount_tokket", Name="Subaccount", Type="nonconsumable", PriceUSD = 2.99, Quantity=1, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Subaccounts, TokketType = TokketId.Tokket },

            #endregion

            #region Tok Blast
            //Tok Blast
            new PurchaseModel(){ Id="eliminators_tokblast1", Name="72 eliminators", Type="consumable", PriceUSD = 1.99, Quantity=72, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="eliminators_tokblast2", Name="144 eliminators", Type="consumable", PriceUSD = 2.99, Quantity=144, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="eliminators_tokblast3", Name="288 eliminators", Type="consumable", PriceUSD = 4.99, Quantity=288, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="eliminators_tokblast4", Name="624 eliminators", Type="consumable", PriceUSD = 9.99, Quantity=624, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="eliminators_tokblast5", Name="1800 eliminators", Type="consumable", PriceUSD = 19.99, Quantity=1800, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="tokblast_noads", Name="No Ads (Tok Blitz)", Type="nonconsumable", PriceUSD = 2.99, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.NoAds, TokketType = TokketId.TokBlast },
            //Saved Games
            new PurchaseModel(){ Id="saved_tokblast_coins1", Name="4 Saved Games", Type="nonconsumable", PriceCoins = 90, Quantity=4, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="saved_tokblast1", Name="15 Saved Games", Type="nonconsumable", PriceUSD = 3.99, Quantity=25, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="saved_tokblast2", Name="30 Saved Games", Type="nonconsumable", PriceUSD = 5.99, Quantity=50, PurchaseType = PurchaseType.NonConsumable, ProductType = ProductType.Saved, TokketType = TokketId.TokBlast },
            //Tok Blast Coin Purchase
            new PurchaseModel(){ Id="eliminators_tokblast_coins1", Name="7 eliminators", Type="consumable", PriceCoins = 10, Quantity=7, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            new PurchaseModel(){ Id="eliminators_tokblast_coins2", Name="50 Coins", Type="consumable", PriceCoins = 60, Quantity=50, PurchaseType = PurchaseType.Consumable, ProductType = ProductType.Eliminators, TokketType = TokketId.TokBlast },
            #endregion
        };

        /// <summary>Gets a product by its id.</summary>
        public static PurchaseModel GetProduct(string id)
        {
            return _products.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>Get all products available in the platform.</summary>
        public static List<PurchaseModel> GetProducts()
        {
            return _products;
        }

        //public static OrderRequest GetOrderRequest(this PurchaseModel purchase, string userId)
        //{
        //    OrderRequest orderRequest = new OrderRequest()
        //    {
        //        CheckoutPaymentIntent = "CAPTURE",

        //        ApplicationContext = _defaultApplicationContext,
        //        PurchaseUnits = new List<PurchaseUnitRequest>
        //        {
        //          new PurchaseUnitRequest{
        //            ReferenceId =  purchase.Id,
        //            Description = purchase.Name,
        //            CustomId = purchase.Id,
        //            SoftDescriptor = "",
        //            AmountWithBreakdown = new AmountWithBreakdown
        //            {
        //              CurrencyCode = "USD",
        //              Value = purchase.PriceUSD.ToString(),
        //              AmountBreakdown = new AmountBreakdown
        //              {
        //                ItemTotal = new Money
        //                {
        //                  CurrencyCode = "USD",
        //                  Value = purchase.PriceUSD.ToString()
        //                },
        //                TaxTotal = new Money
        //                {
        //                  CurrencyCode = "USD",
        //                  Value = "0.00"
        //                }
        //              }
        //            },
        //            Items = new List<Item>
        //            {
        //              new Item
        //              {
        //                Name = purchase.Id,
        //                Description = purchase.Name,
        //                Sku = purchase.Id,
        //                UnitAmount = new Money
        //                {
        //                  CurrencyCode = "USD",
        //                  Value = purchase.PriceUSD.ToString()
        //                },
        //                Quantity = purchase.Quantity.ToString(),
        //                Category = "DIGITAL_GOODS"
        //              },
        //            },
        //            ShippingDetail = _defaultShippingDetails
        //          }
        //        }
        //    };

        //    return orderRequest;
        //}
    }
}