using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Core.Tools;
using Tokket.Shared.IoC;
using db = Tokket.Infrastructure;
using System.Linq;
using Tokket.Shared.Helpers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Tokket.Core;
using System.Threading;
using Tokket.Shared.Models.Tok;
using static Tokket.Shared.Services.ServicesDB.DBConstant;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace Tokket.Shared.Services.ServicesDB
{

    
    public class ClassTokServiceDB :Interfaces.IClassTokService
    {
        private static Tokket.Infrastructure.IDatabaseService databaseService;
        public static ClassTokServiceDB Instance = new ClassTokServiceDB();

  
        public ClassTokServiceDB() {
             databaseService = new db.CosmosDBService(new db.ApiOptions(isprod: true));
        }
        #region Classtoks Function
        public async Task<GetClassToksResponse<T>> GetClassToksAsync<T>(GetClassToksRequest request) {
            List<ClassTokModel> classTok = new List<ClassTokModel>();
            string uid = Settings.GetUserModel().UserId;
            int MaxItemCount = 24;
            var values = request.QueryValues;
            // Setup query
            Expression<Func<Dictionary<string, object>, bool>> query = BuildClassTokQuery(values);
            QueryRequestOptions options = new QueryRequestOptions() { MaxItemCount = 24 };

            // Exclude toks in query if class set is not empty
            if (!string.IsNullOrEmpty(values.classsetid))
            {
                long itemTotal = values.itemtotal ?? 0;
                long docIndex = Convert.ToInt32(Math.Truncate(itemTotal / (double)1000000));
                var pkSetToks = $"{values.classsetid}-classtoks{docIndex}";

                // Get all toks under a set //x => x.Label == "classtok" && x.PartitionKey == pkSetToks, z => z._Timestamp
                var toksInSet = await databaseService.GetItemsAsync<ClassTokModel>(new db.GetItemsRequest<ClassTokModel>() { Predicate = x => x.Label == "classtok" && x.PartitionKey == pkSetToks, OrderBySelector = z => z._Timestamp });
                if (toksInSet.Results != null)
                {
                    var result = toksInSet.Results.ToList();
                    if (result.Count() > 0)
                    {
                        var tokIds = result.Select(x => x.Id).Distinct().ToList();
                        tokIds = tokIds.Where(x => x != null).ToList();
                        if (tokIds.Count > 0) // Make sure its not empty
                        {
                            query = query.And(x => !tokIds.Contains(x["id"].ToString())); // Exclude toks in query
                        }
                    }
                }
            }

            if (values.partitionkeybase.Contains("classset"))
            {
                //Set the max item count to -1
                MaxItemCount = -1;
            }

            //Do NOT get Personal Tok if you are not logged in as the current user
            if (!string.IsNullOrEmpty(values?.userid ?? string.Empty) && values?.userid != uid)  // Personal Tok Feed
            {
                values.partitionkeybase = $"{values.userid}-classtoks";
            }

            // Filter if requesting public feed
            else if (values?.publicfeed ?? false)
            {
                // Override pk base
                values.partitionkeybase = "classtoks";
            }

            // Filter if the tok is part of the group
            else if (!string.IsNullOrEmpty(values.groupid))
            {
                // Override pk base
                values.partitionkeybase = $"{values.groupid}-classtoks";
            }

            // Else will retain the set pk base

            // Filter By Option Section, None will skip the switch statement
            switch (values.FilterBy)
            {
                case Shared.Helpers.FilterBy.Class:
                    query = query.And(x => values.FilterItems.Contains(x["tok_type_id"].ToString())); // Class
                    break;
                case Shared.Helpers.FilterBy.Category:
                    query = query.And(x => values.FilterItems.Contains(x["category_id"].ToString())); // Category
                    break;
                case Shared.Helpers.FilterBy.Type:
                    query = query.And(x => values.FilterItems.Contains(x["tok_group"].ToString())); // Type
                    break;
            }
            // Order By
             Expression<Func<Dictionary<string, object>, dynamic>> orderBy = (x => x["created_time"]);
            if (!string.IsNullOrEmpty(values.orderby))
            {
                orderBy = (x => x[values.orderby]);
                if (values.orderby == "reference_id")
                {
                    query = query.And(x => x["reference_id"].ToString() != null); // Type
                }
            }

            //Search Value
            if (!string.IsNullOrEmpty(values.searchvalue))
            {
                query = query.And(x => x["primary_text"].ToString().Contains(values.searchvalue));
            }
            values.paginationid = (string.IsNullOrEmpty(values?.paginationid)) ? values.paginationid : Regex.Unescape(values.paginationid);
            //query, orderBy, values.partitionkeybase, values.paginationid, values.itemtotal, Constants.TOKS_PER_PARTITION, options
            var response = await databaseService.GetPartitionedItemAsync<Dictionary<string, object>>(new db.GetPartitionedItemsRequest() { Predicate = query, OrderBySelector = orderBy, ContinuationToken = values.paginationid, PartitionKey = values.partitionkeybase, MaxItemCount = MaxItemCount, Container = DefaultCNTR, IsPartitioned = true, IsPartitionedDocumentTotal = values.itemtotal,QueryRequestOptions = options, IsPartitionDocumentMax = TOKS_PER_PARTITION, isDescending = values.descending });

            if (response.Results != null)
            {
                var res = response;
                if (res.Results != null)
                {
                    // Includes group information
                    if (res.Results.Count() > 0)
                    {
                        var classToks = JsonConvert.DeserializeObject<List<ClassTokModel>>(JsonConvert.SerializeObject(res.Results));
                        var groupIds = classToks.Select(x => x.GroupId).Distinct().ToList();
                        groupIds = groupIds.Where(x => x != null).ToList();
                        if (groupIds.Count > 0)
                        {
                            long itemTotal = values.itemtotal ?? 0;
                            long docIndex = Convert.ToInt32(Math.Truncate(itemTotal / (double)1000000));
                            var pk = $"classgroups{docIndex}";//x => x.Label == "classgroup" && groupIds.Contains(x.Id) && x.PartitionKey == pk, x => x.CreatedTime, null, null, true
                            var groups = await databaseService.GetItemsAsync<ClassGroupModel>(new db.GetItemsRequest<ClassGroupModel>() { Predicate = x => x.Label == "classgroup" && groupIds.Contains(x.Id) && x.PartitionKey == pk, OrderBySelector = x => x.CreatedTime, Container = DefaultCNTR });
                            var newCollection = new List<Dictionary<string, object>>();

                            foreach (var item in classToks)
                            {
                                if (!string.IsNullOrEmpty(item.GroupId))
                                {

                                    var group = groups.Results?.FirstOrDefault(x => x.Id == item.GroupId) ?? null;
                                    if (group != null)
                                    {
                                        item.Group = group;
                                    }
                                }

                                var convertedItem = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(item));
                                newCollection.Add(convertedItem);
                            }

                            res.Results = newCollection;
                            response.Results = res.Results;
                        }
                    }
                }
            }
           
            response.Results = await EnrichClassToksWithUser(response.Results.ToList());
            response.Results = await EnrichClassToksWithSharedTok(response.Results.ToList());
            response.Results = await EnrichClassToksWithViewCount(response.Results.ToList()) ;
            //foreach (var item in response.Results) {
            //    var convertItem = JsonConvert.SerializeObject(item);
            //    classTok.Add(JsonConvert.DeserializeObject<Diction>(convertItem));
            //}

            var responses = new GetClassToksResponse<T>();
            responses.Results = response.Results;
            responses.ContinuationToken = response.ContinuationToken;
            return responses;
        }

        public async Task<ClassToksItemResponse<T>> GetClassTokAsync<T>(string id, string pk) {
            var tok = await databaseService.GetItemAsync<Dictionary<string,object>>(new db.GetItemRequest() { Id = id, PartitionKey = pk, Container = DefaultCNTR });
            var response = new ClassToksItemResponse<T>();
            response.Result = tok.Result;
            response.StatusCode = tok.StatusCodeEnum;
            return response;
        }

        public async Task<ClassToksItemResponse<T>> AddClassTokAsync<T>(ClassTokModel tok) {
            #region Upload image if necessary
            //if (String.IsNullOrEmpty(tok.TokShare) && String.IsNullOrEmpty(tok.TokSharePk))
            //{
            //    TokketImage tokketImageRecord = new TokketImage() { UserId = tok.UserId };
            //    tok.Image = await _sharedDb.UploadImageAsync(tokketImageRecord, tok.Image, extension, "tokkepedia", devicePlatform);

            //    if (tok.IsMegaTok ?? false)
            //    {
            //        tok.IsDetailBased = false;

            //        //Loop through Tok Sections
            //        for (int i = 0; i < (tok.Sections?.Length ?? 0); ++i)
            //        {
            //            if (tok.Sections[i] != null)
            //            {
            //                if (!string.IsNullOrEmpty(tok.Sections[i].Image))
            //                {
            //                    tokketImageRecord = new TokketImage() { UserId = userId };
            //                    tok.Sections[i].Image = await _sharedDb.UploadImageAsync(tokketImageRecord, tok.Sections[i].Image, extension, "tokkepedia", devicePlatform);
            //                }
            //            }
            //        }
            //    }
            //    else if (tok.IsDetailBased)
            //    {
            //        //Loop through DetailsImages
            //        for (int i = 0; i < (tok.DetailImages?.Length ?? 0); ++i)
            //        {
            //            if (!string.IsNullOrEmpty(tok.DetailImages?[i]))
            //            {
            //                tokketImageRecord = new TokketImage() { UserId = userId };
            //                tok.DetailImages[i] = await _sharedDb.UploadImageAsync(tokketImageRecord, tok.DetailImages[i], extension, "tokkepedia", devicePlatform);
            //            }
            //        }
            //    }
            //}
            #endregion
            bool privateFeed = tok.IsPrivate.Value;//(req.Query["private"].ToString() == "false" || req.Headers["private"].ToString() == "false") ? false : true; //Private
            bool groupFeed = tok.IsGroup;// (req.Query["group"].ToString() == "true" || req.Headers["group"].ToString() == "true") ? true : false;
            bool publicFeed = tok.IsPublic;// (req.Query["public"].ToString() == "true" || req.Headers["public"].ToString() == "true") ? true : false;
                                           //If Group Feed, must be member of that group
            if (groupFeed)
            {
                var isMember = await IsUserMemberOfGroup(tok.GroupId, tok.UserId);
                if (!isMember)
                {
                   // return new BadRequestObjectResult(GetResultModel(Shared.Helpers.Result.Forbidden, null, "Not a member of the group."));
                }
            }

            #region Process Class Tok
            if (string.IsNullOrEmpty(tok.UserCountry))
                tok.UserCountry = "none";

            tok.Id = tok.PrimaryFieldText.GetPrimaryTextId();
            tok.PartitionKey = tok.Id;
            #endregion
            var response = await CreateClassTokParallelAsync( tok, tok.UserId, privateFeed, groupFeed, publicFeed);

            var responses = new ClassToksItemResponse<T>();
            responses.Result = response;
            responses.StatusCode = HttpStatusCode.OK;
            responses.Message = "Class tok created successfully!";
            return responses;
        }

        public async Task<ClassToksItemResponse<T>> UpdateClassTokAsync<T>(ClassTokModel classtok) {
            bool isSuccess = false;
            string uid = Settings.GetTokketUser().Id;
            //Ensure that it's the owner of the item
            var responses = new ClassToksItemResponse<T>();
            if (classtok.UserId != uid )
            {
                if (classtok.GroupId != null)
                {
                    long itemTotal = 0;//req.GetItemTotalFromHeader() ?? 0;
                    long docIndex = Convert.ToInt32(Math.Truncate(itemTotal / (double)1000000));
                    var getGroup = await databaseService.GetItemAsync<ClassGroupModel>(new db.GetItemRequest() { Id = classtok.GroupId, PartitionKey = $"classgroups{docIndex}", Container = DefaultCNTR });
                    //var getGroup = await _api.GetItemAsyncResponse<ClassGroupModel>(classtok.GroupId, $"classgroups{docIndex}", null, DefaultDB, DefaultCNTR);
                    if (uid != getGroup.Result.UserId)
                    {
                        responses.Message = $"No group Id found.";
                        responses.StatusCode = HttpStatusCode.Unauthorized;
                        return responses;
                    }
                }
                else
                {
                    responses.Message = $"No group Id found.";
                    responses.StatusCode = HttpStatusCode.Unauthorized;
                    return responses;
                }
            }
            #region Upload image if necessary
            //TokketImage tokketImageRecord = new TokketImage() { UserId = userId, Image = classtok.Image };
            //updatedTok.Image = await _sharedDb.UploadImageAsync(tokketImageRecord, updatedTok.Image, extension, "tokkepedia", devicePlatform, true);

            //if (updatedTok.IsMegaTok ?? false)
            //{
            //    updatedTok.IsDetailBased = false;

            //    //Loop through Tok Sections
            //    for (int i = 0; i < (updatedTok.Sections?.Length ?? 0); ++i)
            //    {
            //        if (updatedTok.Sections[i] != null)
            //        {
            //            if (!string.IsNullOrEmpty(updatedTok.Sections[i].Image))
            //            {
            //                tokketImageRecord = new TokketImage() { UserId = userId, Image = (string.IsNullOrEmpty(classtok.Sections[i].Image) ? "" : classtok.Sections[i].Image) };
            //                updatedTok.Sections[i].Image = await _sharedDb.UploadImageAsync(tokketImageRecord, updatedTok.Sections[i].Image, extension, "tokkepedia", devicePlatform, true);
            //            }
            //        }
            //    }
            //}
            //else if (updatedTok.IsDetailBased)
            //{
            //    //Loop through DetailsImages
            //    for (int i = 0; i < (updatedTok.DetailImages?.Length ?? 0); ++i)
            //    {
            //        if (!string.IsNullOrEmpty(updatedTok.DetailImages?[i]))
            //        {
            //            tokketImageRecord = new TokketImage() { UserId = userId, Image = (string.IsNullOrEmpty(classtok.DetailImages[i]) ? "" : classtok.DetailImages[i]) };
            //            updatedTok.DetailImages[i] = await _sharedDb.UploadImageAsync(tokketImageRecord, updatedTok.DetailImages[i], extension, "tokkepedia", devicePlatform, true);
            //        }
            //    }
            //}
            var response = await databaseService.UpdateItemAsync<ClassTokModel>(new db.UpdateItemRequest<ClassTokModel>() { Id = classtok.Id, Item = classtok, PartitionKey = classtok.PartitionKey, Container = DefaultCNTR });
            isSuccess = response != null;
            #endregion
         
            return responses;
        }

        public async Task<ClassToksItemResponse<T>> DeleteClassTokAsync<T>(string id, string pk) {
            var isSuccess = true;
        
            var delete = await databaseService.DeleteItemAsync<Dictionary<string,object>>(new db.DeleteItemRequest() { Id = id, PartitionKey = pk, Container = DefaultCNTR });
            isSuccess = delete != null;
            var responses = new ClassToksItemResponse<T>();
            responses.Message = "Delete ClassTok successful";
            responses.StatusCode = HttpStatusCode.OK;
            return responses;
        }

        public async Task<FilterByResponse<T>> FilterBy<T>(ClassTokQueryValues values)
        {
            try
            {
               // db.GetPartitionedItemsResponse<Dictionary<string, object>> response = null;
                var resultList = new List<CommonModel>();
              
                string filterKey = string.Empty;

                Expression<Func<Dictionary<string, object>, bool>> query = null;
                switch (values.FilterBy)
                {
                    case Shared.Helpers.FilterBy.Class:
                        query = (x => x["label"].ToString() == "classtok");
                        filterKey = "tok_type";
                        break;
                    case Shared.Helpers.FilterBy.Category:
                        query = (x => x["label"].ToString() == "category" && x["name"] != null);
                        filterKey = "name";
                        values.partitionkeybase = "categories";
                        break;
                        // Currently not needed for class tok since it only have 3 static group
                        //case Shared.Helpers.FilterBy.Type:
                        //    query = (x => x["label"].ToString() == "toktypelist");
                        //    filterKey = "tok_group";
                        //    values.partitionkeybase = "toktypelist";
                        //    values.itemtotal = -1;
                        //    break;
                }

                // Only for Class Toks
                if (values.FilterBy == Shared.Helpers.FilterBy.Class)
                {
                    if (!string.IsNullOrEmpty(values?.userid ?? string.Empty))  // Personal Tok
                    {
                        values.partitionkeybase = $"{values.userid}-classtoks";
                    }
                    // Filter if requesting public feed
                    else if (values?.publicfeed ?? false)
                    {
                        // Override pk base
                        values.partitionkeybase = "classtoks";
                    }
                    // Filter if the tok is part of the group
                    else if (!string.IsNullOrEmpty(values.groupid))
                    {
                        // Override pk base
                        values.partitionkeybase = $"{values.groupid}-classtoks";
                    }
                    // Else will retain the set pk base
                }

                // Order By
                Expression<Func<Dictionary<string, object>, dynamic>> orderBy = (x => x["_ts"]); // Default, Recent Only
                if (!values.RecentOnly)
                {
                    orderBy = (x => x[filterKey].ToString()); // Change order based on key
                }

                // Currently not needed for class tok since it only have 3 static group
                //if(values.FilterBy == Shared.Helpers.FilterBy.Type)
                //{
                //    var options = new QueryRequestOptions() { PartitionKey = new PartitionKey("toktypelist") };
                //    response = await _api.GetItemsAsyncResponse<Dictionary<string, object>>(query, orderBy, null, options);
                //}
                //else
                var  response = await databaseService.GetPartitionedItemAsync<Dictionary<string,object>>(new db.GetPartitionedItemsRequest() { Predicate= query, OrderBySelector= orderBy,PartitionKey= values.partitionkeybase, ContinuationToken = values.paginationid, IsPartitionedDocumentTotal= values.itemtotal, Container = DefaultCNTR } );

                // Convert Result into common
                if (response != null)
                {
                    if (response.Result != null)
                    {
                        //var distinctedList = response.Results.Distinct(x => x[filterKey].ToString());
                        //foreach (var item in distinctedList)
                        //{
                        //    CommonModel model = new CommonModel();
                        //    switch (values.FilterBy)
                        //    {
                        //        case Shared.Helpers.FilterBy.Class:
                        //            model.LabelIdentifier = "classtok";
                        //            model.Id = item["tok_type_id"].ToString();
                        //            model.Title = item["tok_type"].ToString();
                        //            break;
                        //        case Shared.Helpers.FilterBy.Category:
                        //            model.LabelIdentifier = "category";
                        //            model.Id = item["id"].ToString();
                        //            model.UniqueKey = item["pk"].ToString();
                        //            model.Title = item["name"].ToString();
                        //            break;
                        //            //case Shared.Helpers.FilterBy.Type:
                        //            //    model.LabelIdentifier = "toktypelist";
                        //            //    model.Id = item["id"].ToString();
                        //            //    model.UniqueKey = item["pk"].ToString();
                        //            //    model.Title = item["tok_group"].ToString();
                        //            //    break;
                        //    }
                        //    // Comment out this line if not needed. This will increase the byte array returned. Comsumes more data if too many but can be solved by limiting results in query.
                        //    model.JsonData = JsonConvert.SerializeObject(item);
                        //    resultList.Add(model);
                        //}
                    }

                    if (!values.RecentOnly)
                    {
                        resultList = resultList.OrderBy(x => x.Title).ToList();
                    }

                    var r = new ResultData<CommonModel>() { ContinuationToken = response.ContinuationToken, Results = resultList };
                    var responses = new FilterByResponse<T>();
                    
                    return responses;
                    // return new OkObjectResult(GetResultModel(Shared.Helpers.Result.Success, r, "Success!"));
                }
                else
                {
                    var r = new ResultData<CommonModel>() { Results = resultList };
                    var responses = new FilterByResponse<T>();
                    return responses;
                    //return new BadRequestObjectResult(GetResultModel(Shared.Helpers.Result.Failed, r, "No Result!"));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                var responses = new FilterByResponse<T>();
                return responses;
                //return new BadRequestObjectResult(GetResultModel(Shared.Helpers.Result.Success, null, ex.Message));
            }
        }
        #endregion
        private Expression<Func<Dictionary<string, object>, bool>> BuildClassTokQuery(ClassTokQueryValues values)
        {
            // Setup query
            Expression<Func<Dictionary<string, object>, bool>> query = (x => x["label"].ToString() == "classtok");


            // Other filters here..
            if (!string.IsNullOrEmpty(values.userid)) query = query.And(x => x["user_id"].ToString() == values.userid);
            if (!string.IsNullOrEmpty(values.toktypeid)) query = query.And(x => x["tok_type_id"].ToString() == values.toktypeid);

            if (!string.IsNullOrEmpty(values.category)) query = query.And(x => x["category"].ToString() == values.category);
            if (!string.IsNullOrEmpty(values.tokgroup)) query = query.And(x => x["tok_group"].ToString() == values.tokgroup);
            if (!string.IsNullOrEmpty(values.toktype)) query = query.And(x => x["tok_type"].ToString() == values.toktype);

            if (!string.IsNullOrEmpty(values.level1)) query = query.And(x => x["level1"].ToString() == values.level1);
            if (!string.IsNullOrEmpty(values.level2)) query = query.And(x => x["level2"].ToString() == values.level2);
            if (!string.IsNullOrEmpty(values.level3)) query = query.And(x => x["level3"].ToString() == values.level3);

            #region Search
            if (!string.IsNullOrEmpty(values.searchkey) && !string.IsNullOrEmpty(values.searchvalue)) //Both search key and search value must contain a value
            {
                //Default: Contains + Noncasesensitive

                //Starts with
                if (values.startswith ?? false)
                {
                    //Non Case sensitive by default
                    if ((values.casesensitive ?? false))
                    {
                        query = query.And(x => x[values.searchkey].ToString().ToLower().StartsWith(values.searchvalue.ToLower()));
                    }
                    else
                    {
                        //Search exactly as typed
                        query = query.And(x => x[values.searchkey].ToString().StartsWith(values.searchvalue));
                    }
                }
                //Contains
                else
                {

                    //Non Case sensitive by default
                    if ((values.casesensitive ?? false))
                    {
                        query = query.And(x => x[values.searchkey].ToString().ToLower().Contains(values.searchvalue.ToLower()));
                    }
                    else
                    {
                        //Search exactly as typed
                        query = query.And(x => x[values.searchkey].ToString().Contains(values.searchvalue));
                    }
                }

            }
            #endregion

            #region Tok Share
            if (!string.IsNullOrEmpty(values.tokshare)) query = query.And(x => x["tokshare"].ToString() == values.tokshare);
            if (!string.IsNullOrEmpty(values.toksharepk)) query = query.And(x => x["toksharepk"].ToString() == values.toksharepk);
            #endregion
            return query;
        }

        private async Task<Dictionary<string, object>> CreateClassTokParallelAsync(ClassTokModel tok, string userId, bool privateFeed, bool groupFeed, bool publicFeed)
        {
            List<Task> lstTasks = new List<Task>();
            //new Task<Task<db.CreateItemResponse<Dictionary<string,object>>>>(() => databaseService.CreateItemAsync(new db.CreateItemRequest<Dictionary<string, object>>()));
            //JsonConvert.SerializeObject(tok)), $"{tok.UserId}-classtoks", req.GetItemTotalFromHeader(), 1000000, null, DefaultDB, DefaultCNTR)
            var privateTask = new Task<Task<db.CreateItemResponse<Dictionary<string, object>>>>(() => databaseService.CreateItemAsync(new db.CreateItemRequest<Dictionary<string, object>>() { IsPartitioned = true, Item = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(tok)),
                PartitionKey = $"{tok.UserId}-classtoks", IsPartitionDocumentMax = 1000000, Container = DefaultCNTR,
                IsPartitionedDocumentTotal = null
            }));

            var groupTask = new Task<Task<db.CreateItemResponse<Dictionary<string, object>>>>(() => databaseService.CreateItemAsync(new db.CreateItemRequest<Dictionary<string, object>>()
            {
                IsPartitioned = true,
                Item = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(tok)),
                PartitionKey = $"{tok.GroupId}-classtoks",
                IsPartitionDocumentMax = 1000000,
                Container = DefaultCNTR,
                IsPartitionedDocumentTotal = null
            }));
            var publicTask = new Task<Task<db.CreateItemResponse<Dictionary<string, object>>>>(() => databaseService.CreateItemAsync(new db.CreateItemRequest<Dictionary<string, object>>()
            {
                IsPartitioned = true,
                Item = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(tok)),
                PartitionKey = $"classtoks",
                IsPartitionDocumentMax = 1000000,
                Container = DefaultCNTR,
                IsPartitionedDocumentTotal = null
            }));

            //1. Private
            if (privateFeed)
            {
                privateTask.Start();
                lstTasks.Add(privateTask);
            }


            //2. Group
            if (groupFeed)
            {
                groupTask.Start();
                lstTasks.Add(groupTask);
            }

            //3. Public
            if (publicFeed)
            {
                publicTask.Start();
                lstTasks.Add(publicTask);
            }

            //4. Document count
            int classToksCreated = 0;
            if (privateFeed) ++classToksCreated;
            if (groupFeed) ++classToksCreated;
            if (publicFeed) ++classToksCreated;
            //var updateString = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() { { "classtoks", classToksCreated }, { "classtoks_private", privateFeed ? 1 : 0 }, { "classtoks_group", groupFeed ? 1 : 0 }, { "classtoks_public", privateFeed ? 1 : 0 } });
            //var incString = new db.CreateItemRequest<db.TokkepediaUserCounterPersonalExpanded>()
            //{
            //    ItemDictionary = new Dictionary<string, object>() { { "classtoks", classToksCreated }, { "classtoks_private", privateFeed ? 1 : 0 }, { "classtoks_group", groupFeed ? 1 : 0 }, { "classtoks_public", privateFeed ? 1 : 0 } },
            //    Id = $"{userId}-tokkepediausercounterpersonal",
            //    PartitionKey = $"{userId}-tokkepediausercounterpersonal",
            //    isCounter = true,
            //    Container = DefaultCNTR,
            //};
           // lstTasks.Add(databaseService.CreateItemAsync<db.TokkepediaUserCounterPersonalExpanded>(incString));

            //5. Views counter
            Dictionary<string, object> itemCounter = GetClassTokViewCounter(tok);
           // lstTasks.Add(databaseService.CreateItemAsync<Dictionary<string, object>>(new db.CreateItemRequest<Dictionary<string, object>>() { Item = itemCounter, PartitionKey = itemCounter["pk"].ToString(), Container = DefaultCNTR }));

            await Task.WhenAll(lstTasks);

            //Result to use
        db.CreateItemResponse<Dictionary<string, object>> response = null;
            if (privateFeed)
            {
                var innerTask = await privateTask;
                response = await innerTask;
            }

            else if (groupFeed)
            {
                var innerTask = await groupTask;
                response = await innerTask;
            }
            else if (publicFeed)
            {
                var innerTask = await publicTask;
                response = await innerTask;
            }
            else
                response = null;

            //string id = tok.UserId + "-allusercounter", pk = tok.UserId + "-allusercounter";
            //var userInfo = await GetTokketUser(userId);
            //var incrementUpdateString = GetPointsCoinsString(tok, userInfo);
            //var incCoin = new db.PatchItemRequest()
            //{
            //    FieldsIncrement = incrementUpdateString,
            //    Id = id,
            //    PartitionKey = pk,
            //    Container = DefaultCNTR
            //};
           // var counterResponse = Task.Run(async () => await databaseService.PatchItemAsync<AllUserCounter>(incCoin));
          //  Task.WaitAny(counterResponse);

            //id = tok.CategoryId;
          //  pk = tok.CategoryId;
           // var incrementUpdateString1 = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() { { "classtoks", 0 } }); // To avoid double increment
           // var patchrequest1 = new db.CreateItemRequest<Category>() { Id = id, PartitionKey = pk,  ItemDictionary = new Dictionary<string, object>() { { "classtoks", 0 } }, Container = DefaultCNTR, isCounter = true };
           // var categoryResponse = Task.Run(async () => { await databaseService.CreateItemAsync<Category>(patchrequest1); });

            //var categoryResponse = Task.Run(async () => await databaseService.IncrementCountAsyncResponse<Category>(id, pk, incrementUpdateString1, "update", null, DefaultDB, DefaultCNTR, isClassTok: true, name: tok.Category));
            //var incrementUpdateString2 = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() { { "toks", 1 } });
           // var patchrequest = new db.CreateItemRequest<Category>() { Id = id, PartitionKey = "classtokscategories0", ItemDictionary = new Dictionary<string, object>() { { "toks", 1 } }, Container = DefaultCNTR, isCounter = true };
           // categoryResponse = Task.Run(async () => { await databaseService.CreateItemAsync<Category>(patchrequest); });
            
           // categoryResponse = Task.Run(async () => await databaseService.IncrementCountAsyncResponse<Category>(id, "classtokscategories0", incrementUpdateString2, "update", null, DefaultDB, DefaultCNTR, isClassTok: true, name: tok.Category));

            return response.Result;
        }

        private Dictionary<string, object> GetClassTokViewCounter(ClassTokModel tok) => new Dictionary<string, object>()
        {
            {"id", $"{tok.Id}-views" },
            {"pk", $"{tok.Id}-views" },
            { "label", "reactioncounter" },
            { "kind", "viewcounter" },
            { "item_id", tok.Id },
            { "user_id", tok.UserId },
            { "tiletap_views", 0 },
            { "pagevisit_views", 0 },
            { "tiletap_views_personal", 0 },
            { "page_views_personal", 0 }
        };

        public async Task<TokketUser> GetTokketUser(string id)
        {
            TokketUser item;

            var response = await databaseService.GetItemsAsync<TokketUser>(new db.GetItemsRequest<TokketUser>() { Id = id, PartitionKey = id });
            item = response.Result;
            //item = await _api.GetItemAsyncResponse<TokketUser>(id, id, null, DefaultUserDB, DefaultUserCNTR);
            //item.RequestChargeBreakdown = new List<(string, double?)>();
            //item.RequestChargeBreakdown.Add((id, item.RequestCharge));

            return item;
        }

        private Dictionary<string, long> GetPointsCoinsString(ClassTokModel tok, TokketUser tokketUser)
        {
            var pc = GetPointsCoins(tok);
            var coinDecrease = 0;
            var coinMultiplier = 1;
            if (!String.IsNullOrEmpty(tok.Sticker))
            {
                coinDecrease = 7;
            }
            if (tokketUser.MembershipEnabled == true)
            {
                coinMultiplier = 2;
            }
            //Knowledge collection
            int totalCoins = (pc.Item1 * coinMultiplier) - coinDecrease;
            if (totalCoins == 30)
            {
                totalCoins = 40;
            }
            //var updateString = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() {
            //        { "coins",  totalCoins},
            //        { "points", pc.Item2 }
            //    });
            return new Dictionary<string, long>() {
                    { "coins",  totalCoins},
                    { "points", pc.Item2 }
                };
        }

        //private Dictionary<string, long> GetPointsCoinsString(ClassTokModel tok, TokketUser tokketUser)
        //{
        //    var pc = GetPointsCoins(tok);
        //    var coinDecrease = 0;
        //    var coinMultiplier = 1;
        //    if (!String.IsNullOrEmpty(tok.Sticker))
        //    {
        //        coinDecrease = 7;
        //    }
        //    if (tokketUser.MembershipEnabled == true)
        //    {
        //        coinMultiplier = 2;
        //    }
        //    //Knowledge collection
        //    int totalCoins = (pc.Item1 * coinMultiplier) - coinDecrease;
        //    if (totalCoins == 30)
        //    {
        //        totalCoins = 40;
        //    }
        //    var updateString = StringExtensions.BuildUpdateString("$inc", new Dictionary<string, long>() {
        //            { "coins",  totalCoins},
        //            { "points", pc.Item2 }
        //        });
        //    return new Dictionary<string, long>() {
        //            { "coins",  totalCoins},
        //            { "points", pc.Item2 }
        //        };
        //}


        private (int, int) GetPointsCoins(ClassTokModel tok)
        {
            int pointsEarned = 0, coinsEarned = 0;
            if (tok?.IsMegaTok ?? false)
            {
                coinsEarned = CoinsMegaTok;
                pointsEarned = 15;
            }
            else if (tok.IsDetailBased)
            {
                coinsEarned = CoinsDetailedTok;
                pointsEarned = 6;
            }
            else
            {
                coinsEarned = CoinsBasicTok;
                pointsEarned = 2;
            }

            return (pointsEarned, coinsEarned);
        }
        #region Helpers
        private async Task<ResultData<Dictionary<string, object>>> EnrichClassToksWithUser(ResultData<Dictionary<string, object>> toks)
        {
            var items = toks.Results.ToList();
            var newList = new List<Dictionary<string, object>>();
            var users = await GetUsersByIds(items.Select(x => x["user_id"].ToString()).Distinct().ToList());

            foreach (var item in items)
            {
                var user = users.Results.FirstOrDefault(x => x.Id == item["user_id"].ToString());
                if (user != null)
                {
                    #region Enrich
                    item["user_photo"] = user.UserPhoto;
                    item["user_display_name"] = user.DisplayName;

                    item["account_type"] = user.AccountType;
                    item["title_display"] = user.TitleDisplay;
                    item["subaccount_name"] = user.SubaccountName;
                    item["title_enabled"] = user.TitleEnabled;
                    item["title_id"] = user.TitleId;

                    item["user_country"] = user.Country;
                    item["user_state"] = user.State;

                    if (user?.IsPointsSymbolEnabled ?? false)
                    {
                        item["is_pointssymbol_enabled"] = true;
                        item["pointssymbol_id"] = user.PointsSymbolId;
                    }
                    #endregion
                }
                newList.Add(item);
            }

            toks.Results = newList;
            return toks;
        }

        private async Task<ResultData<Dictionary<string, object>>> EnrichClassToksWithSharedTok(ResultData<Dictionary<string, object>> toks)
        {
            var items = toks.Results.ToList();
            var newList = new List<Dictionary<string, object>>();
            //    var sharedToks = await GetSharedToksByIds(items.Select(x => x["tok_share"].ToString()).Distinct().ToList());
            foreach (var item in items)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item["tok_share"].ToString()) && !string.IsNullOrEmpty(item["tok_share_pk"].ToString()))
                    {
                        var sharedTok = await databaseService.GetItemAsync<ClassTokModel>(new db.GetItemRequest() { Id = item["tok_share"].ToString(), PartitionKey = item["tok_share_pk"].ToString(), Container = DefaultCNTR });
                        //var sharedTok = await _api.GetItemAsyncResponse<ClassTokModel>(item["tok_share"].ToString(), item["tok_share_pk"].ToString());
                        //  var sharedTok = sharedToks.Results.FirstOrDefault(x => x.Id == item["tok_share"].ToString());
                        if (sharedTok.Result != null)
                        {
                            #region Enrich
                            item["shared_tok"] = JsonConvert.SerializeObject(sharedTok.Result);

                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.StackTrace);
#endif
                }


                newList.Add(item);
            }

            if (newList.Count > 0)
                toks.Results = newList;

            return toks;
        }

        private async Task<ResultData<Dictionary<string, object>>> EnrichClassToksWithViewCount(ResultData<Dictionary<string, object>> toks)
        {
            var items = toks.Results.ToList();
            var newList = new List<Dictionary<string, object>>();

            foreach (var item in items)
            {
                var result = await databaseService.GetItemAsync<Dictionary<string, object>>(new db.GetItemRequest() {  Id = item["id"] + "-views", PartitionKey = item["id"] + "-views", Container = DefaultCNTR });
                //var result = await _api.GetItemAsyncResponse<Dictionary<string, object>>(item["id"] + "-views", item["id"] + "-views");
                int viewCount = 0;
                try
                {
                    viewCount = int.Parse(result.Result["tiletap_views_personals"].ToString());
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(e.StackTrace);
#endif
                }
                try
                {
                    viewCount = viewCount + int.Parse(result.Result["tiletap_viewss"].ToString());
                }
                catch (Exception e) {

#if DEBUG
                    Console.WriteLine(e.StackTrace);
#endif
                }
                item["views"] = viewCount;
                newList.Add(item);
            }

            toks.Results = newList;
            return toks;
        }

        private async Task<List<Dictionary<string, object>>> EnrichClassToksWithUser(List<Dictionary<string, object>> toks)
        {
            var items = toks;
            var newList = new List<Dictionary<string, object>>();
            var users = await GetUsersByIds(items.Select(x => x["user_id"].ToString()).Distinct().ToList());

            foreach (var item in items)
            {
                var user = users.Results.FirstOrDefault(x => x.Id == item["user_id"].ToString());
                if (user != null)
                {
                    #region Enrich
                    item["user_photo"] = user.UserPhoto;
                    item["user_display_name"] = user.DisplayName;

                    item["account_type"] = user.AccountType;
                    item["title_display"] = user.TitleDisplay;
                    item["subaccount_name"] = user.SubaccountName;
                    item["title_enabled"] = user.TitleEnabled;
                    item["title_id"] = user.TitleId;

                    item["user_country"] = user.Country;
                    item["user_state"] = user.State;

                    if (user?.IsPointsSymbolEnabled ?? false)
                    {
                        item["is_pointssymbol_enabled"] = true;
                        item["pointssymbol_id"] = user.PointsSymbolId;
                    }
                    #endregion
                }
                newList.Add(item);
            }

            toks = newList;
            return toks;
        }

        private async Task<List<Dictionary<string, object>>> EnrichClassToksWithSharedTok(List<Dictionary<string, object>> toks)
        {
            var items = toks;
            var newList = new List<Dictionary<string, object>>();
            //    var sharedToks = await GetSharedToksByIds(items.Select(x => x["tok_share"].ToString()).Distinct().ToList());
            foreach (var item in items)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item["tok_share"].ToString()) && !string.IsNullOrEmpty(item["tok_share_pk"].ToString()))
                    {
                        var sharedTok = await databaseService.GetItemAsync<ClassTokModel>(new db.GetItemRequest() { Id = item["tok_share"].ToString(), PartitionKey = item["tok_share_pk"].ToString(), Container = DefaultCNTR });
                        //var sharedTok = await _api.GetItemAsyncResponse<ClassTokModel>(item["tok_share"].ToString(), item["tok_share_pk"].ToString());
                        //  var sharedTok = sharedToks.Results.FirstOrDefault(x => x.Id == item["tok_share"].ToString());
                        if (sharedTok.Result != null)
                        {
                            #region Enrich
                            item["shared_tok"] = JsonConvert.SerializeObject(sharedTok.Result);

                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {

                }


                newList.Add(item);
            }

            if (newList.Count > 0)
                toks = newList;

            return toks;
        }

        private async Task<List<Dictionary<string, object>>> EnrichClassToksWithViewCount(List<Dictionary<string, object>> toks)
        {
            var items = toks;
            var newList = new List<Dictionary<string, object>>();

            foreach (var item in items)
            {
                var result = await databaseService.GetItemAsync<Dictionary<string, object>>(new db.GetItemRequest() { Id = item["id"] + "-views", PartitionKey = item["id"] + "-views", Container = DefaultCNTR });
                //var result = await _api.GetItemAsyncResponse<Dictionary<string, object>>(item["id"] + "-views", item["id"] + "-views");
                int viewCount = 0;
                try
                {
                    viewCount = int.Parse(result.Result["tiletap_views_personals"].ToString());
                }
                catch (Exception e) { }
                try
                {
                    viewCount = viewCount + int.Parse(result.Result["tiletap_viewss"].ToString());
                }
                catch (Exception e) { }
                item["views"] = viewCount;
                newList.Add(item);
            }

            toks = newList;
            return toks;
        }


        private async Task<db.GetItemsResponse<TokketUser>> GetUsersByIds(List<string> ids) =>
          await databaseService.GetItemsAsync<TokketUser>(new db.GetItemsRequest<TokketUser>() { Predicate = x => x.Label == "user" && ids.Contains(x.PartitionKey), OrderBySelector = x => x.CreatedTime, Container = DefaultCNTR });

        #endregion

        private async Task<bool> IsUserMemberOfGroup(string groupId, string userId)
        {
            if (string.IsNullOrEmpty(groupId))
                return true;

            // Setup query
            Expression<Func<Dictionary<string, object>, bool>> query = x => x["label"].ToString() == "classgroup";
            //query = query.And(x => x["id"].ToString() == userId);
            Expression<Func<Dictionary<string, object>, dynamic>> orderBy = (x => x["_ts"]);
            var response = await databaseService.GetItemsAsync<Dictionary<string, object>>(new db.GetItemsRequest<Dictionary<string, object>>() { Predicate = query, OrderBySelector = orderBy, PartitionKey = $"{userId}-classgroupsjoined", Container = DefaultCNTR, MaxItemCount = TOKS_PER_PARTITION });
           // TokkepediaResponse<ResultData<Dictionary<string, object>>> response = await _api.GetPartitionedItemsAsync(query, orderBy, $"{userId}-classgroupsjoined", null, Constants.TOKS_PER_PARTITION);

            //Check if member
            if (response.Results.Count() > 0)
                return true;

            //Check if owner (Find group)
            //query = x => x["user_id"].ToString() == userId;
            //query = query.And(x => x["id"].ToString() == groupId);
            Expression<Func<Dictionary<string, object>, bool>> queryTocheckIfOwner = x => x["label"].ToString() == "classgroup";
            queryTocheckIfOwner = queryTocheckIfOwner.And(x => x["user_id"].ToString() == userId);

            response = await databaseService.GetItemsAsync(new db.GetItemsRequest<Dictionary<string, object>>() { Predicate = queryTocheckIfOwner , OrderBySelector = orderBy, PartitionKey =  $"classgroups", Container = DefaultCNTR, MaxItemCount = TOKS_PER_PARTITION });
           // response = await _api.GetPartitionedItemsAsync(queryTocheckIfOwner, orderBy, $"classgroups", null, Constants.TOKS_PER_PARTITION);

            if (response.Results.Count() > 0)
                return true;

            //Not member or owner
            return false;
        }
        private GetClassToksResponse<T> GetClassToksResponse<T>(ItemResponse<T> result) => new GetClassToksResponse<T>() { Resource = result.Resource, RequestCharge = result.RequestCharge, StatusCode = result.StatusCode, Etag = result.ETag };

   
    }
    public class GetClassToksRequest  { 
        public ClassTokQueryValues QueryValues { get; set; }
    }

    public class GetClassToksResponse<T> : BaseClassTokResponse<T> { 
     public Dictionary<string, object> Result { get; set; }
     public IEnumerable<Dictionary<string, object>> Results { get; set; }
    }

    public class FilterByResponse<T> : BaseClassTokResponse<T> { 
    
    }

    public class ClassToksItemResponse<T> : BaseClassTokResponse<T> {
        public Dictionary<string, object> Result { get; set;}
    }

    public class BaseClassTokResponse<T> {
        /// <summary>The resource.</summary>
        [JsonProperty("resource", NullValueHandling = NullValueHandling.Ignore)]
        public T Resource { get; set; }

        /// <summary>Status Code</summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>Request Charge</summary>
        public double? RequestCharge { get; set; }

        /// <summary>Request Charge Breakdown</summary>
        [JsonProperty("RequestChargeBreakdown", NullValueHandling = NullValueHandling.Ignore)]
        public List<(string, double?)> RequestChargeBreakdown { get; set; } = null;

        /// <summary>Used for concurrency.</summary>
        public string Etag { get; set; } = null;

        /// <summary>Request Charge Breakdown</summary>
        [JsonProperty("ContinuationToken", NullValueHandling = NullValueHandling.Ignore)]
        public string ContinuationToken { get; set; } = null;

        /// <summary></summary>
        [JsonProperty("Count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; } = null;

        /// <summary>Message</summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = null;
    }
}
