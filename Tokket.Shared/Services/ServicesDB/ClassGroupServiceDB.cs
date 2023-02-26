using Newtonsoft.Json;
using Tokket.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Extensions;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Tok;
using Tokket.Core;
using static Tokket.Shared.Services.ServicesDB.DBConstant;
using db = Tokket.Infrastructure;
namespace Tokket.Shared.Services.ServicesDB
{
    public class ClassGroupServiceDB
    {
        private Tokket.Infrastructure.IDatabaseService cosmosDB;
        private Tokket.Infrastructure.FirebaseAuthService firebaseAuth;
        public static ClassGroupServiceDB Instance = new ClassGroupServiceDB();

        private string label = "classgroup";

        private const string membersSuffix = "-classgroupmembers";
        public ClassGroupServiceDB()
        {
            cosmosDB = new db.CosmosDBService(new db.ApiOptions(isprod: Config.Configurations.isProd));
        }

        public async Task<bool> AddClassGroupAsync(ClassGroupModel item)
        {
            bool isSuccess = false;
            #region Upload new image if necessary
            //TokketImage tokketImageRecord = new TokketImage() { UserId = userId };
            //item.Image = await _sharedDb.UploadImageAsync(tokketImageRecord, item.Image, extension, serviceId, devicePlatform);
            #endregion
            #region Modifications
            item.Id = "classgroup-" + Guid.NewGuid().ToString();
            //item.PartitionKey handled in the below function
            #endregion
            item.Members = 1;
            Dictionary<string, object> response = new Dictionary<string, object>();

            //var duplicatesResponse = await _api.CreateMultiPartitionedItemAsync(item.ToDictionary(), new List<string>() { "classgroups", $"{userId}-classgroups" });
            //response.RequestCharge += duplicatesResponse.RequestCharge;
            //response.RequestChargeBreakdown = new List<(string, double?)>();
            //response.RequestChargeBreakdown.AddRange(duplicatesResponse.RequestChargeBreakdown);
            //response.Resource = duplicatesResponse.Resource;


            #region adds the classgroup owner to the members so that it can be retrived to in getclassgroup members
            //TokkepediaResponse<Dictionary<string, object>> addUserResponse = null;
            //TokkepediaResponse<TokketUser> getUserResponse = null;
            //getUserResponse = await _api.GetItemAsyncResponse<TokketUser>(item.UserId, item.UserId, null, DefaultUserDB, DefaultUserCNTR);
            //addUserResponse = await _api.CreatePartitionedItemAsync(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(getUserResponse.Resource)),
            //     $"{item.Id}{membersSuffix}", req.GetItemTotalFromHeader(), 1000000, null, DefaultDB, DefaultCNTR);

            #endregion


            #region Document count
            //Increment
            //var updateString = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() { { "classgroups", 1 } });
            //TokkepediaResponse<dynamic> decresponse = null;
            //try
            //{
            //    decresponse = await _api.IncrementCountAsyncResponse<dynamic>($"{userId}-tokkepediausercounterpersonal", $"{userId}-tokkepediausercounterpersonal", updateString, "update", null, DefaultDB, DefaultCNTR);
            //}
            //catch (Exception ex) { }
            #endregion
            return isSuccess;
        }
        public async Task<bool> UpdateClassGroupAsync(ClassGroupModel item)
        {
            var isSuccess = false;
            #region Upload new image if necessary
            //TokketImage tokketImageRecord = new TokketImage() { UserId = userId, Image = classGroup.Image };
            //item.Image = await _sharedDb.UploadImageAsync(tokketImageRecord, item.Image, extension, serviceId, devicePlatform, true);
            #endregion

            //if (item.Members + 1 != item.Duplicates.Count)
            //{
            //    List<string> stringDuplicate = new List<string>();
            //    var getAllGroups = await _api.GetItemsAsync<ClassGroup>(x => x.Id == classGroup.Id, x => x.CreatedTime, null, null, true, DefaultDB, DefaultCNTR);
            //    do
            //    {
            //        foreach (ClassGroup classGrp in getAllGroups.Results)
            //        {
            //            try
            //            {
            //                stringDuplicate.Add(classGrp.PartitionKey);
            //            }
            //            catch (Exception ex) { }
            //        }
            //        if (!String.IsNullOrEmpty(getAllGroups.ContinuationToken))
            //        {
            //            try
            //            {
            //                getAllGroups = await _api.GetItemsAsync<ClassGroup>(x => x.Id == classGroup.Id, x => x.CreatedTime, getAllGroups.ContinuationToken, null, true, DefaultDB, DefaultCNTR);
            //            }
            //            catch (Exception ex) { }
            //        }
            //    } while (!String.IsNullOrEmpty(getAllGroups.ContinuationToken));
            //    item.Duplicates = stringDuplicate;
            //}



            return isSuccess;
        }

        public async Task<bool> DeleteClassGroupAsync(string id, string pk) {
            bool isSuccess = false;
            var response = await cosmosDB.DeleteItemAsync<ClassGroupModel>(new DeleteItemRequest() { Id = id, PartitionKey = pk, Container = DefaultCNTR });
            if (response != null)
                return false;
            return isSuccess;
        }

        public async Task<ClassGroupModel> GetClassGroupAsync(string id) {
            var itemTotal = 0;
            long docIndex = Convert.ToInt32(Math.Truncate(itemTotal / (double)1000000));
            var response = await cosmosDB.GetItemAsync<ClassGroupModel>(new GetItemRequest() { Id =id,PartitionKey= $"classgroups{docIndex}", Database= DefaultDB,Container= DefaultCNTR });


            return response.Result;
        }

        #region Helper
        private async Task<List<Dictionary<string, object>>> EnrichGroupsWithUser(List<Dictionary<string, object>> groups)
        {
            var items = JsonConvert.DeserializeObject<List<ClassGroupModel>>(JsonConvert.SerializeObject(groups));
            var newList = new List<Dictionary<string, object>>();
            var users = await GetUsersByIds(items.Select(x => x.UserId).Distinct().ToList());

            foreach (var item in items)
            {
                var user = users.Results.FirstOrDefault(x => x.Id == item.UserId);
                if (user != null)
                {
                    #region Enrich
                    item.UserPhoto = user.UserPhoto;
                    item.UserDisplayName = user.DisplayName;

                    item.AccountType = user.AccountType;
                    item.TitleDisplay = user.TitleDisplay;
                    item.SubaccountName = user.SubaccountName;
                    item.TitleEnabled = user.TitleEnabled;
                    item.TitleId = user.TitleId;

                    item.UserCountry = user.Country;
                    item.UserState = user.State;

                    if (user?.IsPointsSymbolEnabled ?? false)
                    {
                        item.IsPointsSymbolEnabled = true;
                        item.PointsSymbolId = user.PointsSymbolId;
                    }
                    #endregion
                }
                newList.Add(item.ToDictionary());
            }

            groups = newList;
            return groups;
        }

        private async Task<List<Dictionary<string, object>>> EnrichGroupWithOwnerData(List<Dictionary<string, object>> groups)
        {
            var items = JsonConvert.DeserializeObject<List<ClassGroupModel>>(JsonConvert.SerializeObject(groups));
            var newList = new List<Dictionary<string, object>>();
            var groupIds = new List<string>();
            foreach (var item in items)
            {
                ClassGroupModel group = new ClassGroupModel();
                bool duplicate = false;

                foreach (string groupId in groupIds)
                {
                    if (groupId == item.Id)
                    {
                        duplicate = true;
                    }
                }
                if (duplicate == true)
                {
                    for (int x = 0; x < 1000000; x++)
                    {
                        var response = await cosmosDB.GetItemAsync<ClassGroupModel>(new GetItemRequest() { Id = item.Id, PartitionKey = $"classgroups{x}" });
                        if (response != null)
                        {
                            group = response.Result;
                            break;
                        }
                    }

                    groupIds.Add(group.Id);
                    var newitem = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(group));
                    newList.Add(newitem);
                }
                else
                {
                    var newitem = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(item));
                    newList.Add(newitem);

                }


            }

            groups = newList;
            return groups;
        }
        private async Task<List<Dictionary<string, object>>> EnrichGroupIdentifyIfMember(List<Dictionary<string, object>> groups, string userId)
        {
            var items = JsonConvert.DeserializeObject<List<ClassGroupModel>>(JsonConvert.SerializeObject(groups));
            var newList = new List<Dictionary<string, object>>();
            var groupIds = new List<string>();
            foreach (var item in items)
            {
                if (item.UserId == userId)
                {
                    item.IsMember = true;
                }
                else
                {
                    var checkmember = await cosmosDB.GetItemAsync<TokketUser>(new GetItemRequest() { Id = userId, PartitionKey = $"{item.Id}{membersSuffix}{0}", Container = DefaultCNTR });
                    //  var checkmember = await _api.GetItemAsyncResponse<TokketUser>(userId, $"{item.Id}{membersSuffix}{0}", null, DefaultDB, DefaultCNTR);
                    // await _api.GetItemAsync<TokketUser>(item.Id,item.Id+"-classgroupmembers");
                    if (checkmember != null)
                        item.IsMember = true;
                }

                //var pendingReq = await _api.GetItemsAsyncResponse<ClassGroupRequest>(x => x.Label == "classgrouprequest" &&
                //               x.GroupId == item.Id &&
                //               x.GroupPartitionKey == item.PartitionKey &&
                //               ((x.Status == Shared.Helpers.RequestStatus.Pending && x.SenderId == userId) ||
                //               (x.Status == Shared.Helpers.RequestStatus.PendingInvite && x.ReceiverId == userId)), z => z._Timestamp);
                //if (pendingReq != null)
                //{
                //    var request = pendingReq.Resource.Results.ToList();
                //    if (request.Count > 0)
                //    {
                //        item.HasPendingRequest = true;
                //    }
                //    else
                //    {
                //        item.HasPendingRequest = false;
                //    }
                //}

                var newitem = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(item));
                newList.Add(newitem);
            }
            groups = newList;
            return groups;
        }
        #endregion
        private async Task<db.GetItemsResponse<TokketUser>> GetUsersByIds(List<string> ids) =>
     await cosmosDB.GetItemsAsync<TokketUser>(new db.GetItemsRequest<TokketUser>() { Predicate = x => x.Label == "user" && ids.Contains(x.PartitionKey), OrderBySelector = x => x.CreatedTime, Container = DefaultCNTR });

        //Base: classgroup-31b58cbf-5626-4b64-b4c9-f9acde34b355{membersSuffix}
        //Pk: classgroup-31b58cbf-5626-4b64-b4c9-f9acde34b355{membersSuffix}0
        private async Task<bool> IsUserMemberOfGroup(string groupId, string userId)
        {
            // Setup query
            Expression<Func<Dictionary<string, object>, bool>> query = x => x["label"].ToString() == "user";
            query = query.And(x => x["id"].ToString() == userId);
            Expression<Func<Dictionary<string, object>, dynamic>> orderBy = (x => x["_ts"]);
            var checkmember = await cosmosDB.GetItemAsync<TokketUser>(new GetItemRequest() { Id = userId, PartitionKey = $"{groupId}{membersSuffix}{0}", Container = DefaultCNTR });

            //   var checkmember = await _api.GetItemAsyncResponse<TokketUser>(userId, $"{groupId}{membersSuffix}{0}", null, DefaultDB, DefaultCNTR);
            if (checkmember != null)
                return true;
            else
                return false;
        }
    }
}
