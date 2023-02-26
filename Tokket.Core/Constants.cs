using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Core
{


    ///// <summary> Shared libraries used by all api projects.</summary>
    //namespace Shared;

    ///// <summary> Shared constants used by many projects.</summary>
    //public static class Constants
    //{
    //    /// <summary> Determines whether to use development or production data.</summary>
    //    public const bool IS_PROD = true;
    //    public static bool UnitTestMode { get; set; } = false;

    //    public static int StartMonth { get; set; } = 1;
    //    public static int StartDay { get; set; } = 1;
    //    public static int StartYear { get; set; } = 2020;

    //    #region Defaults
    //    /// <summary> Default Cosmos DB Database. </summary>
    //    public const string DefaultDB = "Tokket";

    //    /// <summary> Default Cosmos DB Container. </summary>
    //    public const string DefaultCNTR = "Knowledge";

    //    /// <summary> Default Cosmos DB User Database. </summary>
    //    public const string DefaultUserDB = "Tokket";

    //    /// <summary> Default Cosmos DB User Container. </summary>
    //    public const string DefaultUserCNTR = "Users";

    //    /// <summary> Default Cosmos DB Transactions Container. </summary>
    //    public const string DefaultTransactionsCNTR = "Transactions";
    //    #endregion

    //    #region Settings
    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string CosmosLocation = (IS_PROD) ? "CosmosDBConnectionStringProd" : "CosmosDBConnectionStringDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string FirebaseAppKeyLocation = (IS_PROD) ? "FirebaseAppKeyProd" : "FirebaseAppKeyDev";

    //    /// <summary> Name of the Firebase Admin file to use. </summary>
    //    public const string FirebaseAdminFileName = (IS_PROD) ? "firebaseadminprod.json" : "firebaseadmindev.json";

    //    /// <summary> Name of the Firebase Admin file to use. </summary>
    //    public const string FirebaseAuthDomain = (IS_PROD) ? "tokket-inc.firebaseapp.com" : "tokket-app.firebaseapp.com";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string StreamKeyLocation = (IS_PROD) ? "StreamApiKeyProd" : "StreamApiKeyDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string StreamSecretLocation = (IS_PROD) ? "StreamApiSecretProd" : "StreamApiSecretDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string BlobStorageLocation = (IS_PROD) ? "BlobStorageConnectionStringProd" : "BlobStorageConnectionStringDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string QueueStorage = (IS_PROD) ? "QueueStorageProd" : "QueueStorageDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string ServiceBusLocation = (IS_PROD) ? "ServiceBusProd" : "ServiceBusDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string DefaultPhotoUrl = (IS_PROD) ? "DefaultPhotoURL_Prod" : "DefaultPhotoURL_Dev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string RedirectURL = (IS_PROD) ? "RedirectURLProd" : "RedirectURLDev";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string PayPalId = (IS_PROD) ? "PayPalProdId" : "PayPalDevId";

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string PayPalSecret = (IS_PROD) ? "PayPalProdSecret" : "PayPalDevSecret";
    //    #endregion 

    //    /// <summary> Name of the setting in local.settings.json. </summary>
    //    public const string ToksQueueName = (IS_PROD) ? "toksqueue" : "toksqueuedev";

    //    #region Azure Content Moderator
    //    public static readonly string CMAzureRegion = "westus";
    //    public static readonly string CMAzureBaseURL = $"https://{CMAzureRegion}.api.cognitive.microsoft.com";
    //    public static readonly string CMSubscriptionKey = (IS_PROD)
    //        ? "QueueStorageProd"
    //        : "QueueStorageDev";
    //    #endregion

    //    #region Service Limits
    //    public const int DisplayNameMax = 25;
    //    public const int TitleMax = 25;
    //    public const int WebsiteMax = 100;
    //    public const int BioMax = 160;
    //    public const int TitlesMaxPerPartition = 1000000;

    //    //Tok
    //    public const int TOKS_PER_PARTITION = 1000000;

    //    //Set
    //    public const long SETS_PER_PARTITION = 1000000;
    //    public const int SetNameMax = 35;
    //    public const int SetDescriptionMax = 5000;

    //    //Reactions
    //    public const int BrilliantGemCoinCost = 5;
    //    public const int PreciousGemCoinCost = 10;
    //    public const int BrilliantGemValue = 10;
    //    public const int PreciousGemValue = 15;

    //    public const int GROUPS_PER_PARTITION = 1000000;

    //    public const int TOKFEED_COUNT = 8;
    //    public const int CLASSTOKFEED_COUNT = 16;
    //    public const int CLASSGROUPFEED_COUNT = 16;
    //    public const int RECENT_SEARCHES_LIMIT = 10;
    //    public const int FOLLOW_ITEM_LIMIT = 5;

    //    public static string[] RoyaltyTitlePrefixes { get; set; } = new string[]
    //    {
    //        "king", "queen", "prince", "princess", "duke of", "duchess of", "duke-of", "duchess-of", "duke_of", "duchess_of"
    //    };
    //    #endregion

    //    #region Points and Coins
    //    public const int CoinsBasicTok = 2;
    //    public const int CoinsDetailedTok = 5;
    //    public const int CoinsMegaTok = 10;

    //    public const int CoinsAvatar = 10;

    //    public const long FOLLOW_POINTS = 5;
    //    public const long FOLLOW_COINS = 0;
    //    public const long REACTION_POINTS = 5;
    //    public const long REACTION_COINS = 0;
    //    public const long SET_POINTS = 50;
    //    public const long SET_COINS = 1;
    //    public const long REPLICATE_POINTS = 50;
    //    public const long REPLICATE_COINS = 1;
    //    public const long NONDETAILED_POINTS = 100;
    //    public const long NONDETAILED_COINS = 2;
    //    public const long DETAILED_POINTS = 200;
    //    public const long DETAILED_COINS = 4;
    //    #endregion

    //    public const int SavedGamesMax = 50;
    //    public const int TeamsMax = 15;

    //    static Constants()
    //    {
    //    }

    //    //Constant functions
    //    public static ResultModel GetResultModel(Shared.Helpers.Result result, object obj = null, string message = null) => new ResultModel()
    //    {
    //        ResultObject = obj,
    //        ResultMessage = message,
    //        ResultEnum = result
    //    };

    //    #region Service Id
    //    public static bool IsInvalidServiceId(HttpRequest req)
    //    {
    //        string serviceId = req.Headers["serviceid"].ToString()?.RemoveDuplicatesDelimit();
    //        if (string.IsNullOrEmpty(serviceId))
    //            serviceId = req.Query["serviceid"].ToString();
    //        return !TokketServicesTool.TokketAll.Contains(serviceId);
    //    }

    //    public static bool IsInvalidDevicePlatform(HttpRequest req)
    //    {
    //        string devicePlatform = req.Headers["deviceplatform"].ToString();
    //        if (string.IsNullOrEmpty(devicePlatform))
    //            devicePlatform = req.Query["deviceplatform"].ToString();
    //        return !TokketServicesTool.TokketDevicePlatforms.Contains(devicePlatform);
    //    }

    //    public static bool IsInvalidServiceId(string serviceId) => !TokketServicesTool.TokketAll.Contains(serviceId);

    //    public static bool IsInvalidDevicePlatform(string devicePlatform) => !TokketServicesTool.TokketDevicePlatforms.Contains(devicePlatform);

    //    public static string GetServiceId(HttpRequest req)
    //    {
    //        string serviceId = req.Headers["serviceid"].ToString()?.RemoveDuplicatesDelimit();
    //        if (string.IsNullOrEmpty(serviceId))
    //            serviceId = req.Query["serviceid"].ToString();
    //        return serviceId;
    //    }

    //    public static string GetDevicePlatform(HttpRequest req)
    //    {
    //        string devicePlatform = req.Headers["deviceplatform"].ToString();
    //        if (string.IsNullOrEmpty(devicePlatform))
    //            devicePlatform = req.Query["deviceplatform"].ToString();
    //        return devicePlatform;
    //    }
    //    #endregion
    //}
}
