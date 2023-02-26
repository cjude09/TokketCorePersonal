using Newtonsoft.Json;
using Plugin.InAppBilling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Extensions;
using Tokket.Shared.Extensions.Http;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models.Purchase;
using Tokket.Shared.Services.Interfaces;
using Tokket.Core;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class PurchaseService : IPurchaseService
    {
      
        private HttpClientHelper _httpClientHelper;
        public PurchaseService()
        {
            _httpClientHelper = new HttpClientHelper(Config.Configurations.BaseUrl);
        }
        private static PurchaseService _instance = new PurchaseService();

        public static PurchaseService Instance { get { return _instance; } }
        const string serviceId = "tokket";
        const string Bundle = "com.tokket.classtoks";
        HttpClient Client = new HttpClient();
        void InitializeApiClientUser(bool userRequired = false)
        {
            var Users = Settings.GetTokketUser();

            if (userRequired)
            {
                if (Users == null)
                {
                    Users = new TokketUser();
                }
            }
            string userid = Users?.Id, token = Users?.IdToken;
            if (userid == null) userid = "";
            if (token == null) token = "";
          
            Client.DefaultRequestHeaders.Add("userid",userid);
            Client.DefaultRequestHeaders.Add("token",token);
        }
        public async Task<PurchaseResult> PurchaseGroupAsync(GoogleReceipt receipt, TokketUser groupAccount)
        {
            string user = JsonConvert.SerializeObject(groupAccount);
            string convertreciept = JsonConvert.SerializeObject(receipt);
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupaccount", user);
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchasegoogle{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);
         
            var result = await response.Content.ReadAsAsync<PurchaseResult>();
            string convert = JsonConvert.SerializeObject(result) ;
            return result;
        }

        public async Task<PurchaseResult> PurchaseGroupAsync(AppleReceipt receipt, TokketUser groupAccount)
        {
            string user = JsonConvert.SerializeObject(groupAccount);
            string convertreciept = JsonConvert.SerializeObject(receipt);
            _httpClientHelper.Instance.DefaultRequestHeaders.Remove("istest");
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "ios");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("groupaccount", user);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("istest", "true");
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchaseapple{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);

            var result = await response.Content.ReadAsAsync<PurchaseResult>();
            string convert = JsonConvert.SerializeObject(result);
            return result;
        }

        public async Task<PurchaseResult> PurchaseAsync(GoogleReceipt receipt)
        {
            HttpExtensions.RequestHeaders.Clear();

            Client = new HttpClient();

            var idtoken = await SecureStorage.GetAsync("idtoken");
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetTokketUser().Id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("token", idtoken);
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchasegoogle{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");
            HttpResponseMessage response = null;
            PurchaseResult result = new PurchaseResult() { IsSuccess = false, Message = "Purchase Failed" };
            try
            {
                response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);
                result = JsonConvert.DeserializeObject<PurchaseResult>(await response.Content.ReadAsStringAsync());
                //client = new HttpClient();
            }
            catch (Exception ex)
            {
                result = new PurchaseResult() { IsSuccess = false, Message = "Purchase Failed" };
            }
            string convert = JsonConvert.SerializeObject(result);
            return result;
        }

        public async Task<bool> BillingStart(string productId, PurchaseModel product,TokketUser groupAccount = null, string subaccountName = "", string subaccountKey = "",string titleId = "",bool isUnique = false)
        {
            var payload = "payload";
            var billing = CrossInAppBilling.Current;
            bool state = false;
            try
            {
                var connected = await billing.ConnectAsync();

                if (!connected)
                {
             
                    //We are offline or can't connect, don't try to purchase
                    //    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't connect.", "Ok");
                    return state;
                }
                var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                InAppBillingPurchase purchase = new InAppBillingPurchase();
                InAppBillingPurchase pro = new InAppBillingPurchase();
                //check for null just incase
                if (purchases?.Any(p => p.ProductId == productId) ?? false)
                {
                    //Purchase restored
              
                    pro = purchases?.Where(p => p.ProductId == productId).FirstOrDefault();

                    if (pro.ConsumptionState == ConsumptionState.NoYetConsumed)
                    {
                        var con = await CrossInAppBilling.Current.ConsumePurchaseAsync(pro.ProductId, pro.PurchaseToken);
                    }

                }

                purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase,  null);
                //Possibility that a null came through
                if (purchase == null)
                {
                    //Did not purchase
                    //      await Application.Current.MainPage.DisplayAlert("Issue with purchase", "Purchase canceled.", "Ok");
                    //   CocoSharpControlUI.DisplayAlert("Issue with purchase", "Purchase canceled.");
                }
                else if (product.PurchaseType == PurchaseType.Consumable)
                {
                    //Purchased, we can now consume the item or do it later
                    //Handled differently on each platform, no consume on iOS. But Android requires consum


                    string purchaseJSon = JsonConvert.SerializeObject(purchase);
                    #region Android
                    bool consumedItem = await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);
                    if (consumedItem != null)
                    {
                        //Consumed!!
                        //Android: Server purchase AFTER the plugin
                        //  TokGamesApiClients client = new TokGamesApiClients();

                        //TokGamesApiClients.Instance.Users = new TokketUser() { Id = GameService.User?.Id };
                        GoogleReceipt receipt = new GoogleReceipt()
                        {
                            Id = purchase.ProductId,
                            TransactionId = purchase.Id,
                            DeveloperPayload = purchase.Payload,
                            PurchaseToken = purchase.PurchaseToken,
                            BundleId = Bundle
                        };

                        //var result = await TokGamesApiClients.Instance.PurchaseAsync(receipt);
                        if (productId == "groupaccount_tokket")
                        {
                            var result = await PurchaseGroupAsync(receipt, groupAccount);
                            if (result.Message.Contains("created"))
                                state = true;

                        }
                        else if (productId == "subaccount_tokket")
                        {
                            var result = await PurchaseSubaccountAsync(receipt, subaccountName, subaccountKey);
                            if (result.Message.Contains("purchased"))
                                state = true;
                        }
                        else if (productId.Contains("title"))
                        {
                            var result = await PurchaseTitleAsync(receipt, titleId, isUnique);
                            if (result.Message.Contains("purchased"))
                                state = true;
                        }
                        else {
                            var result = await PurchaseAsync(receipt);
                            if (result.Message.Contains("purchased") || result.Message.Contains("created"))
                                state = true;
                        }
                    
                        //Display purchase (optional)
                    }
                    #endregion
                }
                else if (product.PurchaseType == PurchaseType.NonConsumable)
                {
                    //Android: Server purchase AFTER the plugin
               
                        GoogleReceipt receipt = new GoogleReceipt()
                        {
                            Id = purchase.ProductId,
                            TransactionId = purchase.Id,
                            DeveloperPayload = purchase.Payload,
                            PurchaseToken = purchase.PurchaseToken,
                            BundleId = Bundle
                        };

                        var result = await PurchaseAsync(receipt);
                   
                    if (result.Message.Contains("Success"))
                        state = true;
                }    
                
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
               
            }
            finally {
                await billing.DisconnectAsync();

               
            }
            return state;
        }


        public async Task<bool> BillingStartApple(string productId, PurchaseModel product, TokketUser groupAccount = null, string subaccountName = "", string subaccountKey = "", string titleId = "", bool isUnique = false)
        {
            bool state = false;
            try
            {



                    AppleReceipt receipt = new AppleReceipt()
                    {
                        Id = productId,
                        TransactionId = Guid.NewGuid().ToString(),
                        BundleId = Bundle,
                       
                    };


                if (productId == "groupaccount_tokket")
                {
                    var resultTest = await PurchaseGroupAsync(receipt, groupAccount);
                    if (resultTest.Message.Contains("created")) 
                    {
                        state = true;

                    }

                } else if (productId == "membership_tokket") {

                    var result = await PurchaseTitleAsync(receipt, titleId, isUnique);
                    if (result.Message.Contains("purchased"))
                    {
                        state = true;

                    }

                }

            }
            catch (Exception purchaseEx)
            {
                return false;
            }
          
            return state;
        }


        public async Task<PurchaseResult> PurchaseSubaccountAsync(GoogleReceipt receipt, string subaccountName, string subaccountKey)
        {
        
            string convertreciept = JsonConvert.SerializeObject(receipt);
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("subaccountname", subaccountName);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetTokketUser().Id);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("issubaccountowner", "false");
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchasegoogle{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);

            var result = await response.Content.ReadAsAsync<PurchaseResult>();
            string convert = JsonConvert.SerializeObject(result);
            return result;
        }

        public async Task<PurchaseResult> PurchaseTitleAsync(GoogleReceipt receipt, string titleId, bool isUnique = false)
        {
            string convertreciept = JsonConvert.SerializeObject(receipt);
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "android");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("isunique", isUnique.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("titleid", titleId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetTokketUser().Id);
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchasegoogle{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);

            var result = await response.Content.ReadAsAsync<PurchaseResult>();
            string convert = JsonConvert.SerializeObject(result);
            return result;
        }

        public async Task<PurchaseResult> PurchaseTitleAsync(AppleReceipt receipt, string titleId, bool isUnique = false)
        {
            string convertreciept = JsonConvert.SerializeObject(receipt);
            _httpClientHelper.Instance.DefaultRequestHeaders.Clear();
             _httpClientHelper.Instance.DefaultRequestHeaders.Remove("istest");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("serviceid", serviceId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("deviceplatform", "ios");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("isunique", isUnique.ToString());
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("titleid", titleId);
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("istest", "true");
            _httpClientHelper.Instance.DefaultRequestHeaders.Add("userid", Settings.GetTokketUser().Id);
            var url = new Uri($"{Config.Configurations.BaseUrl}{Config.Configurations.ApiPrefix}/purchaseapple{Config.Configurations.CodePrefix}{Config.Configurations.ApiKey}");

            HttpResponseMessage response = await _httpClientHelper.Instance.PostAsJsonAsync(url, receipt);

            var result = await response.Content.ReadAsAsync<PurchaseResult>();
            string convert = JsonConvert.SerializeObject(result);
            return result;
        }

        /*
          #region Google Play Store
        [FunctionName("PurchaseGoogle")]
        public async Task<IActionResult> PurchaseGoogle([HttpTrigger(AuthorizationLevel.Function, "post", Route = "purchasegoogle")]HttpRequest req,
        [Queue("purchasesqueue", Connection = QueueStorage)] IAsyncCollector<ActivityMessage<Dictionary<string, object>>> queue,
        [Queue("usersqueue", Connection = QueueStorage)] IAsyncCollector<ActivityMessage<Dictionary<string, object>>> usersQueue, ILogger log, ExecutionContext context)
        {
            PurchaseResult purchaseResult = new PurchaseResult() { IsSuccess = false };
            string message = "";
            try
            {
                //Valid service id required
                #region Validation
                if (IsInvalidServiceId(req))
                    return new BadRequestObjectResult(GetResultModel(Result.Forbidden, null, "Valid service id required."));
                #endregion

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var receipt = JsonConvert.DeserializeObject<GoogleReceipt>(requestBody);

                if (string.IsNullOrEmpty(receipt.Id) || string.IsNullOrEmpty(receipt.TransactionId))
                    return new BadRequestResult();

                log.LogInformation($"IAP receipt: {receipt.Id}, {receipt.TransactionId}");

                string productSkuId = receipt.Id;
                productSkuId = (productSkuId.EndsWith("_consumable")) ? productSkuId.Replace("_consumable", "") : productSkuId;
                var product = PurchasesTool.GetProduct(productSkuId);

                string devicePlatform = "android";
                string groupAccount = req.Headers["groupaccount"].ToString() ?? null;

                //Verify user
                string userId = req.Headers["userid"].ToString(), idToken = req.Headers["token"].ToString(), serviceId = req.Headers["serviceid"].ToString()?.RemoveDuplicatesDelimit();

                if (string.IsNullOrEmpty(groupAccount)) {
                    var vResult = await _firebaseService.CheckRequestAndVerify(req); var uid = (vResult.ResultObject as FirebaseTokenModel).UserId;
                    if (vResult.ResultEnum == Shared.Helpers.Result.Forbidden) return new UnauthorizedResult();
                }

                #region Subaccount and Titles
                //Specific headers for if a subaccount or title is being purchased
                //Photo not actually supported
                string subaccountName = req.Headers["subaccountname"].ToString(), subaccountPhoto = req.Headers["subaccountphoto"].ToString();
                string titleId = req.Headers["titleid"].ToString();

                bool isUnique = false;
                string uniqueString = req.Headers["isunique"].ToString();
                if (!string.IsNullOrEmpty(uniqueString))
                    isUnique = Convert.ToBoolean(uniqueString);

                bool isSubaccountOwner = false;
                string subOwner = req.Headers["issubaccountowner"].ToString();
                if (!string.IsNullOrEmpty(subOwner))
                    isSubaccountOwner = Convert.ToBoolean(subOwner);

                //Values
                TitleSubaccountValues titleSubaccountValues = new TitleSubaccountValues()
                {
                    TitleId = titleId,
                    IsUnique = isUnique,
                    SubaccountName = subaccountName,
                    SubaccountPhoto = subaccountPhoto,
                    IsSubaccountOwner = isSubaccountOwner
                };
                if (string.IsNullOrEmpty(titleId) && string.IsNullOrEmpty(subaccountName))
                    titleSubaccountValues = null;
                #endregion

                #region Treasure
                string ownerId = (!string.IsNullOrEmpty(req.Headers["ownerid"].ToString())) ? req.Headers["ownerid"].ToString() : null;
                string itemId = (!string.IsNullOrEmpty(req.Headers["itemid"].ToString())) ? req.Headers["itemid"].ToString() : null;
                #endregion

                try
                {
                    var request = _api.AndroidPublishService.Purchases.Products.Get(receipt.BundleId, receipt.Id, receipt.PurchaseToken);
                    var purchaseState = await request.ExecuteAsync();

                    if (purchaseState.DeveloperPayload != receipt.DeveloperPayload)
                    {
                        message = $"IAP invalid, DeveloperPayload did not match!";
                        log.LogInformation(message);
                        purchaseResult.Message = message;
                        return new BadRequestObjectResult(purchaseResult);
                    }
                    if (purchaseState.PurchaseState != 0)
                    {
                        message = $"IAP invalid, purchase was cancelled or refunded!";
                        log.LogInformation(message);
                        purchaseResult.Message = message;
                        return new BadRequestObjectResult(purchaseResult);
                    }
                }
                catch (Exception exc)
                {
                    message = $"IAP invalid, error reported: " + exc.Message;
                    log.LogInformation(message);
                    purchaseResult.Message = message;
                    return new BadRequestObjectResult(purchaseResult);
                }


                log.LogInformation($"IAP Success: {receipt.Id}, {receipt.TransactionId}");

                //Execute content
                //purchaseResult = await ApplyPurchaseContent(receipt.Id, userId, serviceId, "android", false, false, receipt, titleSubaccountValues, itemId, ownerId);

                if (product.Id != "groupaccount_tokket")
                {
                    purchaseResult = await ApplyPurchaseContent(receipt.Id, userId, serviceId, "android", false, false, receipt, titleSubaccountValues, itemId, ownerId);
                }
                else // Group account
                {
                    var user = JsonConvert.DeserializeObject<TokketUser>(groupAccount);
                    user.PurchaseId = receipt.TransactionId;

                    if (await _api.InitFirebaseAppUserAdmin(context.FunctionAppDirectory))
                    {
                        UserRecordArgs newUser = new UserRecordArgs()
                        {
                            Email = user.Email,
                            EmailVerified = false,
                            Password = user.PasswordHash,
                            DisplayName = user.DisplayName,
                            Disabled = false
                        };

                        UserRecord userRecord = await _api.FirebaseAppAdmin.CreateUserAsync(newUser);

                        userId = userRecord.Uid;
                        user.Id = userRecord.Uid;
                        user.PartitionKey = user.Id;
                        user.PasswordHash = null;

                        user.Points = null;
                        user.Coins = null;
                        if (string.IsNullOrEmpty(user.UserPhoto))
                            user.UserPhoto = null;

                        string subaccountId = (user?.AccountType == "group") ? $"{user.Id}-{Guid.NewGuid().ToString()}" : null;
                        List<Task> concurrentTasks = new List<Task>();

                        //List all
                        var userTask = _api.CreateItemAsync<TokketUser>(user, user.PartitionKey, null, DefaultUserDB, DefaultUserCNTR);
                        var subaccountTask = CreateSubaccountAsync(user, subaccountId);
                        var defaultDataTask = CreateDefaultUserDataDocs(user.Id, serviceId, subaccountId);

                        //Add only if needed
                        concurrentTasks.Add(userTask);
                        if (user?.AccountType == "group")
                            concurrentTasks.Add(subaccountTask);
                        concurrentTasks.Add(defaultDataTask);

                        await Task.WhenAll(concurrentTasks);

                        #region Increment service id counter
                        Dictionary<string, object> queueMessage = new Dictionary<string, object>()
                        {
                            { "id", user.Id }, { "pk", user.Id },
                            { "email", user.Email },
                            { "items_label", "users" },
                            { "function_id", "signup" },
                            { "service_id", serviceId?.ToString() },
                            { "device_platform", devicePlatform?.ToString() },
                            { "date", GetDateIdString(TimeframeType.Daily) }
                        };
                        await usersQueue.AddAsync(new ActivityMessage<Dictionary<string, object>>() { Id = user.Id, Kind = "user", Action = "counter", Data = queueMessage });

                        //try { await _sharedDb.IncrementTimeframeCounterAsync("users", "signup", serviceId, devicePlatform); } catch (Exception e) { }
                        #endregion

                        //Result
                        purchaseResult = new PurchaseResult()
                        {
                            UserId = user.Id,
                            ServiceId = serviceId,
                            IsSuccess = true,
                            DevicePlatform = devicePlatform,
                            PriceUSD = product.PriceUSD,
                            ProductId = product.Id,
                            Message = $"Group account {user.Id} created. Purchase id: {user.PurchaseId}",
                            Content = user.Id
                        };
                    }
                }


                if (purchaseResult.IsSuccess) //Create receipt record
                {
                    #region Purchase Metrics: Increment service id counter
                    Dictionary<string, object> queueMessage = new Dictionary<string, object>()
                    {
                        { "id", receipt.Id }, { "pk", receipt.Id },
                        { "product_id", receipt.Id },
                        { "items_label", "purchases" },
                        { "function_id", "purchase" },
                        { "service_id", serviceId?.ToString() },
                        { "device_platform", "android" },
                        { "date", GetDateIdString(TimeframeType.Daily) }
                    };
                    await queue.AddAsync(new ActivityMessage<Dictionary<string, object>>() { Id = receipt.Id, Kind = "purchase", Action = "counter", Data = queueMessage });
                    //var metricTask = _sharedDb.IncrementTimeframeCounterAsync("purchases", "purchase", serviceId, "android", receipt.Id, TimeframeType.Daily, "Tokket", DefaultTransactionsCNTR);
                    //docsTasks.Add(metricTask);
                    #endregion

                    purchaseResult.Id = "receipt-" + receipt.TransactionId;
                    purchaseResult.ServiceId = serviceId;
                    await CreateReceiptAsync(purchaseResult);
                    return new OkObjectResult(purchaseResult);
                }
                else
                {
                    return new BadRequestObjectResult(purchaseResult);
                }
            }
            catch (Exception e)
            {
                purchaseResult.Message = e.Message;
                return new BadRequestObjectResult(purchaseResult);
            }
        }
        #endregion 

         
         
         
         */
    }
}
