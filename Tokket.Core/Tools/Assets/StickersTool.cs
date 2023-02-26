//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core.Tools
{
    using System.Collections.Generic;
    using Tokket.Core;

    /// <summary>Used to get all stickers.</summary>
    public static class StickersTool
    {
        private const string BASE_URL = "https://tokketcontent.blob.core.windows.net";

        public static List<Sticker> Stickers { get; set; } = new List<Sticker>()
            #region List of stickers
            {
                new Sticker()
                {
                    Id = "sticker_1", PartitionKey = "stickers",
                    Name = "Attention! (Black)",
                    Text = "Attention!",
                    Image = $"{BASE_URL}/images/STICKERS_black_-Attention!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_2", PartitionKey = "stickers",
                    Name = "Did You Know? (Black)",
                    Text = "Did You Know?",
                    Image = $"{BASE_URL}/images/STICKERS_black_-Did-You-Know.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_3", PartitionKey = "stickers",
                    Name = "Gotta Know! (Black)",
                    Text = "Gotta Know!",
                    Image = $"{BASE_URL}/images/STICKERS_black_-Gotta-Know!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_4", PartitionKey = "stickers",
                    Name = "Check This Out! (Black)",
                    Text = "Check This Out!",
                    Image = $"{BASE_URL}/images/STICKERS_black_CheckThisOut.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_5", PartitionKey = "stickers",
                    Name = "Conversation Item! (Black)",
                    Text = "Conversation Item!",
                    Image = $"{BASE_URL}/images/STICKERS_black_Conversation-Item!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_6", PartitionKey = "stickers",
                    Name = "Don't Miss This! (Black)",
                    Text = "Don't Miss This!",
                    Image = $"{BASE_URL}/images/STICKERS_black_Don_t-miss-this.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_7", PartitionKey = "stickers",
                    Name = "Learn This! (Black)",
                    Text = "Learn This!",
                    Image = $"{BASE_URL}/images/STICKERS_black_Learn-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_8", PartitionKey = "stickers",
                    Name = "Open This! (Black)",
                    Text = "Open This!",
                    Image = $"{BASE_URL}/images/STICKERS_black_Open-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_9", PartitionKey = "stickers",
                    Name = "Attention! (Green)",
                    Text = "Attention!",
                    Image = $"{BASE_URL}/images/STICKERS_green_-Attention!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_10", PartitionKey = "stickers",
                    Name = "Did You Know? (Green)",
                    Text = "Did You Know?",
                    Image = $"{BASE_URL}/images/STICKERS_green_-Did-You-Know.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_11", PartitionKey = "stickers",
                    Name = "Gotta Know! (Green)",
                    Text = "Gotta Know!",
                    Image = $"{BASE_URL}/images/STICKERS_green_-Gotta-Know!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_12", PartitionKey = "stickers",
                    Name = "Check This Out! (Green)",
                    Text = "Check This Out!",
                    Image = $"{BASE_URL}/images/STICKERS_green_CheckThisOut.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_13", PartitionKey = "stickers",
                    Name = "Conversation Item! (Green)",
                    Text = "Conversation Item!",
                    Image = $"{BASE_URL}/images/STICKERS_green_Conversation-Item!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_14", PartitionKey = "stickers",
                    Name = "Don't Miss This! (Green)",
                    Text = "Don't Miss This!",
                    Image = $"{BASE_URL}/images/STICKERS_green_Don_t-miss-this.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_15", PartitionKey = "stickers",
                    Name = "Learn This! (Green)",
                    Text = "Learn This!",
                    Image = $"{BASE_URL}/images/STICKERS_green_Learn-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_16", PartitionKey = "stickers",
                    Name = "Open This! (Green)",
                    Text = "Open This!",
                    Image = $"{BASE_URL}/images/STICKERS_green_Open-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_17", PartitionKey = "stickers",
                    Name = "Attention! (Gray)",
                    Text = "Attention!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_-Attention!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_18", PartitionKey = "stickers",
                    Name = "Did You Know? (Gray)",
                    Text = "Did You Know?",
                    Image = $"{BASE_URL}/images/STICKERS_grey_-Did-You-Know.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_19", PartitionKey = "stickers",
                    Name = "Gotta Know! (Gray)",
                    Text = "Gotta Know!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_-Gotta-Know!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_20", PartitionKey = "stickers",
                    Name = "Check This Out! (Gray)",
                    Text = "Check This Out!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_CheckThisOut.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_21", PartitionKey = "stickers",
                    Name = "Conversation Item! (Gray)",
                    Text = "Conversation Item!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_Conversation-Item!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_22", PartitionKey = "stickers",
                    Name = "Don't Miss This! (Gray)",
                    Text = "Don't Miss This!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_Don_t-miss-this.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_23", PartitionKey = "stickers",
                    Name = "Learn This! (Gray)",
                    Text = "Learn This!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_Learn-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_24", PartitionKey = "stickers",
                    Name = "Open This! (Gray)",
                    Text = "Open This!",
                    Image = $"{BASE_URL}/images/STICKERS_grey_Open-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_25", PartitionKey = "stickers",
                    Name = "Attention! (Pink)",
                    Text = "Attention!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_-Attention!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_26", PartitionKey = "stickers",
                    Name = "Did You Know? (Pink)",
                    Text = "Did You Know?",
                    Image = $"{BASE_URL}/images/STICKERS_pink_-Did-You-Know.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_27", PartitionKey = "stickers",
                    Name = "Gotta Know! (Pink)",
                    Text = "Gotta Know!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_-Gotta-Know!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_28", PartitionKey = "stickers",
                    Name = "Check This Out! (Pink)",
                    Text = "Check This Out!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_CheckThisOut.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_29", PartitionKey = "stickers",
                    Name = "Conversation Item! (Pink)",
                    Text = "Conversation Item!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_Conversation-Item!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_30", PartitionKey = "stickers",
                    Name = "Don't Miss This! (Pink)",
                    Text = "Don't Miss This!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_Don_t-miss-this.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_31", PartitionKey = "stickers",
                    Name = "Learn This! (Pink)",
                    Text = "Learn This!",
                    Image = $"{BASE_URL}/images/STICKERS_pinkLearn-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_32", PartitionKey = "stickers",
                    Name = "Open This! (Pink)",
                    Text = "Open This!",
                    Image = $"{BASE_URL}/images/STICKERS_pink_Open-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_33", PartitionKey = "stickers",
                    Name = "Attention! (Yellow)",
                    Text = "Attention!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_-Attention!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_34", PartitionKey = "stickers",
                    Name = "Did You Know? (Yellow)",
                    Text = "Did You Know?",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_-Did-You-Know.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_35", PartitionKey = "stickers",
                    Name = "Gotta Know! (Yellow)",
                    Text = "Gotta Know!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_-Gotta-Know!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_36", PartitionKey = "stickers",
                    Name = "Check This Out! (Yellow)",
                    Text = "Check This Out!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_CheckThisOut.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_37", PartitionKey = "stickers",
                    Name = "Conversation Item! (Yellow)",
                    Text = "Conversation Item!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_Conversation-Item!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_38", PartitionKey = "stickers",
                    Name = "Don't Miss This! (Yellow)",
                    Text = "Don't Miss This!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_Don_t-miss-this.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_39", PartitionKey = "stickers",
                    Name = "Learn This! (Yellow)",
                    Text = "Learn This!",
                    Image = $"{BASE_URL}/images/STICKERS_Yellow_Learn-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                },
                new Sticker()
                {
                    Id = "sticker_40", PartitionKey = "stickers",
                    Name = "Open This! (Yellow)",
                    Text = "Open This!",
                    Image = $"{BASE_URL}/images/STICKERS_yellow_Open-This!.png",
                    PriceCoins = 10, PriceUSD = null, SoftMarker = "database"
                }
            #endregion
        };
    }
}
