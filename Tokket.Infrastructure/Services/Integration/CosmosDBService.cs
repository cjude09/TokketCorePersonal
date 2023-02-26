using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

using Tokket.Core;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tokket.Core.Models.Extensions;
using Microsoft.Azure.Cosmos.Scripts;
using tokket = Tokket.Core;
namespace Tokket.Infrastructure
{
    public interface IDatabaseService
    {
        Task<GetItemResponse<T>> GetItemAsync<T>(GetItemRequest request);

        Task<GetItemsResponse<T>> GetItemsAsync<T>(GetItemsRequest<T> request);

        Task<CreateItemResponse<T>> CreateItemAsync<T>(CreateItemRequest<T> request);

        Task<UpdateItemResponse<T>> UpdateItemAsync<T>(UpdateItemRequest<T> request);

        Task<DeleteItemResponse<T>> DeleteItemAsync<T>(DeleteItemRequest request);

        Task<PatchItemResponse<T>> PatchItemAsync<T>(PatchItemRequest request);

        Task<GetPartitionedItemsResponse> GetPartitionedItemAsync<T>(GetPartitionedItemsRequest request);

     
        // Task<TokkepediaResponse<T>> IncrementCountAsyncResponse<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = "Tokket", string container = "Knowledge", int retries = 1, bool isClassTok = false, string name = ""); //"$inc"

    }
    public class CosmosDBService : IDatabaseService
    {
        private readonly ApiOptions _options;

        CosmosClient _dbClient;

        public CosmosDBService(IOptions<ApiOptions> options)
        {
            _options = options.Value;
            _dbClient = new CosmosClient(_options.IsProd ? _options.CosmosDBConnectionString : _options.CosmosDBConnectionStringDev);
        }
        public CosmosDBService(ApiOptions options)
        {
            _options = options;
            var connectionString = _options.IsProd ? _options.CosmosDBConnectionString : _options.CosmosDBConnectionStringDev;
            _dbClient = new CosmosClient(connectionString, new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Direct,
                MaxRequestsPerTcpConnection = 40
            });
        }

        public async Task<GetItemResponse<T>> GetItemAsync<T>(GetItemRequest request)
        {
            var response = new GetItemResponse<T>()
            {
                StatusCode = 200//Convert.ToInt32(dbResponse.StatusCode)
            };

            try
            {
                var dbResponse = await _dbClient
               .GetDatabase(request.Database)
               .GetContainer(request.Container)
                .ReadItemAsync<T>(request.Id, new PartitionKey(request.PartitionKey));

                response.StatusCodeEnum = dbResponse.StatusCode;
                response.Result = request.IncludeResourceInResponse ? dbResponse.Resource : default;
            }
            catch (CosmosException ex)
            {
                response.StatusCodeEnum = ex.StatusCode;
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    response.StatusCode = 404;
            }

            return response;
        }

        public async Task<CreateItemResponse<T>> CreateItemAsync<T>(CreateItemRequest<T> request)
        {
            if (request.IsPartitioned)
            {
                //if (string.IsNullOrEmpty(request.IsPartitionedItemsBase))
                //{
                //    throw new ArgumentNullException("Items base required if partitioned.");
                //}
                CreateItemResponse<T> response = new CreateItemResponse<T>() { };

                GetItemResponse<PartitionManager> partitionManager = new GetItemResponse<PartitionManager>();
                #region #1: Decide which partition to insert to by getting the partition manager
                if (request.IsPartitionedDocumentTotal == null)
                {
                    var pkForPartitionManager = $"{request.PartitionKey}-pmgr";
                    // #1: Decide which partition to insert to by getting the partition manager
                    // Example: category-philippines-toks, toks
                    var getRequest = new GetItemRequest();
                    getRequest.Id = pkForPartitionManager;
                    getRequest.PartitionKey = pkForPartitionManager;
                    getRequest.Database = request.Database;
                    getRequest.Container = request.Container;

                    partitionManager = await GetItemAsync<PartitionManager>(getRequest);
                    request.IsPartitionedDocumentTotal = 1;
                    if (partitionManager.Result == null)
                    {
                        PartitionManager newManager = new PartitionManager() { Documents = 1, DeletedDocuments = 0, Id = pkForPartitionManager, PartitionKey = pkForPartitionManager };

                        var dbResponsePartitionManager = await _dbClient
                .GetDatabase(request.Database)
                .GetContainer(request.Container)
                .CreateItemAsync<PartitionManager>(newManager, new PartitionKey(pkForPartitionManager));
                        request.IsPartitionedDocumentTotal = dbResponsePartitionManager.Resource.Documents;
                    }
                    //else
                    //{
                    //    response.RequestCharge += partitionManager.RequestCharge;
                    //    response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
                    //}


                }
                #endregion

                // #2: Calculate Partition number
                long partitionNum = Convert.ToInt32(Math.Truncate((long)request.IsPartitionedDocumentTotal / (double)request.IsPartitionDocumentMax));
                string partitionKey = $"{request.PartitionKey}{partitionNum}";

                // #3: Add document to partition. Increment partition manager
                var item = request.Item.ToDictionary();
                item["pk"] = partitionKey;
                var dbResponse = await _dbClient
       .GetDatabase(request.Database)
       .GetContainer(request.Container)
       .CreateItemAsync<Dictionary<string, object>>(item, new PartitionKey(partitionKey));

                response.Result = default(T);
                response.Item = dbResponse.Resource;
                return response;
            } else if (request.isCounter) {

                T item = (T)GetNewCounter<T>(request.Id, request.PartitionKey, request.isClassTok, request.Name);

                if (item.GetType() == typeof(T) && request.ItemDictionary != null)
                {
                    var dbResponse = await _dbClient
                   .GetDatabase(request.Database)
                   .GetContainer(request.Container)
                   .CreateItemAsync<Dictionary<string, object>>(request.ItemDictionary, new PartitionKey(request.PartitionKey));
                    var response = new CreateItemResponse<T>()
                    {
                        Item = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                        StatusCode = (int)dbResponse.StatusCode
                    };
                    return response;
                }
                else if (item.GetType() != typeof(T) && request.ItemDictionary != null)
                {
                    var dbResponse = await _dbClient
                      .GetDatabase(request.Database)
                      .GetContainer(request.Container)
                      .CreateItemAsync<T>(item, new PartitionKey(request.PartitionKey));

                    var response = new CreateItemResponse<T>()
                    {
                        Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                        StatusCode = (int)dbResponse.StatusCode
                    };
                    return response;
                }
                else {
                    var dbResponse = await _dbClient
                   .GetDatabase(request.Database)
                   .GetContainer(request.Container)
                   .CreateItemAsync<T>(request.Item, new PartitionKey(request.PartitionKey));

                    var response = new CreateItemResponse<T>()
                    {
                        Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                        StatusCode = (int)dbResponse.StatusCode
                    };
                    return response;
                }
               

           
            }
            else {
                var dbResponse = await _dbClient
                .GetDatabase(request.Database)
                .GetContainer(request.Container)
                .CreateItemAsync<T>(request.Item, new PartitionKey(request.PartitionKey));

                var response = new CreateItemResponse<T>()
                {
                    Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                    StatusCode = (int)dbResponse.StatusCode
                };
                return response;
            }
        }

        public async Task<DeleteItemResponse<T>> DeleteItemAsync<T>(DeleteItemRequest request)
        {
            var dbResponse = await _dbClient
                .GetDatabase(request.Database)
                .GetContainer(request.Container)
                .DeleteItemAsync<T>(request.Id, new PartitionKey(request.PartitionKey));

            var response = new DeleteItemResponse<T>()
            {
                Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                StatusCode = Convert.ToInt32(dbResponse.StatusCode.ToString())
            };

            return response;
        }

        public async Task<GetItemsResponse<T>> GetItemsAsync<T>(GetItemsRequest<T> request)
        {
            //int maxCount = request?.MaxItemCount ?? -1;  //(options.MaxItemCount == null) ? -1 : (int)options.MaxItemCount;
            //QueryRequestOptions options = new QueryRequestOptions() { MaxItemCount = request.MaxItemCount };

            // options.PartitionKey = !string.IsNullOrEmpty(request.PartitionKey) ? new PartitionKey(request.PartitionKey) : null;

            var container = _dbClient.GetDatabase(request.Database).GetContainer(request.Container);
            var queryable = container.GetItemLinqQueryable<T>(false, request.ContinuationToken, request.QueryRequestOptions);
            var queryItem = (request.isDescending) ? queryable.OrderByDescending(request.OrderBySelector).Where(request.Predicate) : queryable.OrderBy(request.OrderBySelector).Where(request.Predicate);//

            var iterator = queryItem.ToFeedIterator();
            var results = await iterator.ReadNextAsync();

            GetItemsResponse<T> response = new GetItemsResponse<T>()
            {
                Results = results.Resource,
                ContinuationToken = results.ContinuationToken,
                MaxItemCount = request.MaxItemCount
            };

            return response;


        }

        public async Task<UpdateItemResponse<T>> UpdateItemAsync<T>(UpdateItemRequest<T> request)
        {
            var dbResponse = await _dbClient
                .GetDatabase(request.Database)
                .GetContainer(request.Container)
                .UpsertItemAsync<T>(request.Item, new PartitionKey(request.PartitionKey));

            var response = new UpdateItemResponse<T>()
            {
                Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                StatusCode = 200
            };

            return response;
        }

        public async Task<PatchItemResponse<T>> PatchItemAsync<T>(PatchItemRequest request)
        {
            var container = _dbClient
                .GetDatabase(request.Database)
                .GetContainer(request.Container);

            List<PatchOperation> patchOperations = new List<PatchOperation>();
            ItemResponse<T> dbResponse = null;
            bool success = false; int completedRetries = 0;
            PatchItemResponse<T> response = new PatchItemResponse<T>();

            while (!success && completedRetries < request.Retries)
            {
               
                try
                {
                    foreach (var field in request.FieldsIncrement ?? new Dictionary<string, long>())
                    {
                        patchOperations.Add(PatchOperation.Increment($"/{field.Key}", field.Value));
                    }

                    foreach (var field in request.FieldsUpdate ?? new Dictionary<string, object>())
                    {
                        patchOperations.Add(PatchOperation.Set($"/{field.Key}", field.Value));
                    }

                    dbResponse = await container.PatchItemAsync<T>(
                                   id: request.Id,
                                   partitionKey: new PartitionKey(request.PartitionKey),
                                   patchOperations: patchOperations);


                    response = new PatchItemResponse<T>()
                    {
                        Result = request.IncludeResourceInResponse ? dbResponse.Resource : default,
                        StatusCode = 200
                    };
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    try { 
                    
                    } catch (Exception ex2) { 
                    
                    }
                }
            }
          
          

            //patchOperations.Add(PatchOperation.Increment("/taskNum", 6));



            return response;
        }

        public async Task<GetPartitionedItemsResponse> GetPartitionedItemAsync<T>(GetPartitionedItemsRequest request)
        {
            //if (string.IsNullOrEmpty(request.IsPartitionedItemsBase))
            //{
            //    throw new ArgumentNullException("Items base required if partitioned.");
            //}
         
            GetPartitionedItemsResponse response = new GetPartitionedItemsResponse() { };

            GetItemResponse<PartitionManager> partitionManager = new GetItemResponse<PartitionManager>();
            #region #1: Decide which partition to insert to by getting the partition manager
            if (request.IsPartitionedDocumentTotal == null)
            {
                var pkForPartitionManager = $"{request.PartitionKey}-pmgr";
                // #1: Decide which partition to insert to by getting the partition manager
                // Example: category-philippines-toks, toks
                var getRequest = new GetItemRequest();
                getRequest.Id = pkForPartitionManager;
                getRequest.PartitionKey = pkForPartitionManager;
                getRequest.Database = request.Database;
                getRequest.Container = request.Container;

                partitionManager = await GetItemAsync<PartitionManager>(getRequest);
                request.IsPartitionedDocumentTotal = 1;
                if (partitionManager == null)
                {
                    PartitionManager newManager = new PartitionManager() { Documents = 1, DeletedDocuments = 0, Id = pkForPartitionManager, PartitionKey = pkForPartitionManager };

                    var dbResponsePartitionManager = await _dbClient
            .GetDatabase(request.Database)
            .GetContainer(request.Container)
            .CreateItemAsync<PartitionManager>(newManager, new PartitionKey(pkForPartitionManager));
                    request.IsPartitionedDocumentTotal = dbResponsePartitionManager?.Resource?.Documents ?? 1;
                }
                //else
                //{
                //    response.RequestCharge += partitionManager.RequestCharge;
                //    response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
                //}


            }
            #endregion

            // #2: Calculate Partition number
            long partitionCount = Convert.ToInt32(Math.Truncate((long)request.IsPartitionedDocumentTotal / (double)request.IsPartitionDocumentMax.Value));
          //  string partitionKey = $"{request.IsPartitionedItemsBase}{partitionNum}";

            // #3: Get Docs
            //Setup partitions
            // Update: Docs are now having the same pkbase for each label
            for (int i = 0; i < partitionCount; ++i)
            {
                var str = request.PartitionKey + i;
                  request.Predicate = request.Predicate.And( x => x["pk"].ToString() == str);
            }
            if (partitionCount == 0)
            {
                var str = request.PartitionKey + 0;
         
                  request.Predicate = request.Predicate.And(x => x["pk"].ToString() == str);
            }

            //if (!string.IsNullOrEmpty(request.PartitionKey))
            //{
            //    request.QueryRequestOptions.PartitionKey = new PartitionKey(request.PartitionKey);
            //}
            //else
            //{
            //    request.QueryRequestOptions.PartitionKey = null;
            //}

            // options.PartitionKey = !string.IsNullOrEmpty(request.PartitionKey) ? new PartitionKey(request.PartitionKey) : null;
            var results =await GetItemsAsync(new GetItemsRequest<Dictionary<string,object>>() { Predicate = request.Predicate, OrderBySelector = request.OrderBySelector, Container = "Knowledge",QueryRequestOptions = request.QueryRequestOptions, ContinuationToken = !string.IsNullOrEmpty(request.ContinuationToken) ? request.ContinuationToken : null }); 

            response = new GetPartitionedItemsResponse()
            {
                Results = results.Results,
                ContinuationToken = results.ContinuationToken,
                MaxItemCount = request.MaxItemCount
            };
            return response;
        }


        //public async Task<TokkepediaResponse<T>> IncrementCountAsyncResponse<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = "Tokket", string container = "Knowledge", int retries = 1, bool isClassTok = false, string name = "")
        //{
        //    StoredProcedureExecuteResponse<T> result = null;
        //    TokkepediaResponse<T> response = null;

        //    bool success = false; int completedRetries = 0;
        //    while (!success && completedRetries < retries)
        //    {
        //        try
        //        {
        //            result = await _dbClient.GetDatabase(db).GetContainer(container)
        //            .Scripts.ExecuteStoredProcedureAsync<T>("update", new PartitionKey(partitionKey), new[] { id, updateString });
        //            response = new TokkepediaResponse<T>() { Resource = result.Resource, RequestCharge = result.RequestCharge, StatusCode = result.StatusCode, Etag = result.ETag };

        //            success = true;
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            success = false;
        //            if (ex.Message.Contains("Document not found."))
        //            {
        //                //Create new counter before continuing (Does not count as a retry)
        //                try
        //                {
        //                    response = await CreateNewCounter<T>(id, partitionKey, isClassTok: isClassTok, name: name);
        //                    continue;
        //                }
        //                catch (Exception e)
        //                {
        //                    //Try to increment again if a new counter cannot be created
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                //Error not related to not found, so try again
        //                ++completedRetries;
        //                continue;
        //            }
        //        }
        //    }

        //    if (response == null)
        //        response = new TokkepediaResponse<T>() { RequestCharge = 0 };

        //    return response;
        //}

        //#region Create new counter if "Docucment not found" error
        //public async Task<TokkepediaResponse<T>> CreateNewCounter<T>(string id, string partitionKey,
        //    string db = "Tokket", string container = "Knowledge", bool isClassTok = false, string name = "")
        //{
        //    T item = (T)GetNewCounter<T>(id, partitionKey, isClassTok, name);

        //    var result = await _dbClient.GetDatabase(db).GetContainer(container)
        //        .CreateItemAsync<T>(item, new PartitionKey(partitionKey));
        //    return GetTokkepediaResponse(result);
        //}

        private object GetNewCounter<T>(string id, string partitionKey, bool isClassTok = false, string name = "")
        {
            if (typeof(T) == typeof(tokket.UserCounter))
                return new tokket.UserCounter() { Toks = 1, Id = id, PartitionKey = partitionKey };
            else if (typeof(T) == typeof(tokket.Category))
            {
                var catName = name;
                var cat = new tokket.Category() { Id = id, PartitionKey = partitionKey, Name = catName };
                if (isClassTok)
                    cat.Classtoks = 1;
                else
                    cat.Toks = 1;
                return cat;
            }
            else if (typeof(T) == typeof(tokket.TokType))
                return new tokket.TokType() { Toks = 1, Id = id, PartitionKey = partitionKey };
            else if (typeof(T) == typeof(TokkepediaCounter))
                return new TokkepediaCounter() { Toks = 1, Id = id, PartitionKey = partitionKey };
            else if (typeof(T) == typeof(tokket.TokkepediaUserCounterPersonal))
                return new tokket.TokkepediaUserCounterPersonal() { ClassToks = 1, Id = id, PartitionKey = partitionKey };
            else if (typeof(T) == typeof(TokkepediaUserCounterPersonalExpanded))
                return new TokkepediaUserCounterPersonalExpanded() { ClassSets = 0, ClassToks = 0, ClassToksPrivate = 0, ClassToksPublic = 0, ClassToksGroup = 0, Id = id, PartitionKey = partitionKey };
            else if (typeof(T) == typeof(tokket.AllUserCounter))
                return new tokket.AllUserCounter() { Coins = 0, Points = 0, Id = id, PartitionKey = partitionKey };
            else
                return default(T);
        }
        //#endregion


        private TokkepediaResponse<T> GetTokkepediaResponse<T>(ItemResponse<T> result) => new TokkepediaResponse<T>() { Resource = result.Resource, RequestCharge = result.RequestCharge, StatusCode = result.StatusCode, Etag = result.ETag };

    }

    public class BaseCosmosDBRequest
    {
        public string Id { get; set; }

        public string PartitionKey { get; set; }

        public string Database { get; set; } = "Tokket";

        public string Container { get; set; } = "Users";

        public bool IncludeResourceInResponse { get; set; } = true;

        public bool IsPartitioned { get; set; } = false;

        public string IsPartitionedItemsBase { get; set; }

        public long? IsPartitionedDocumentTotal { get; set; }

        public int? IsPartitionDocumentMax { get; set; } = 1000000;

        public bool isDescending { get; set; } = true;
    }

    public class BaseCosmosDBResponse<T> : BaseResponse<T>
    {
        public override T Result { get; set; }
    }

    public class GetItemRequest : BaseCosmosDBRequest { }

    public class GetItemResponse<T> : BaseCosmosDBResponse<T> { }

    public class GetItemsRequest<T> : BaseCosmosDBRequest
    {
        public Expression<Func<T, bool>> Predicate { get; set; }
        public Expression<Func<T, dynamic>> OrderBySelector { get; set; }
        public string ContinuationToken { get; set; }

        public QueryRequestOptions QueryRequestOptions { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }

    public class GetPartitionedItemsRequest : BaseCosmosDBRequest {
        public Expression<Func<Dictionary<string,object>, bool>> Predicate { get; set; }
        public Expression<Func<Dictionary<string,object>, dynamic>> OrderBySelector { get; set; }
        public string ContinuationToken { get; set; }

        public QueryRequestOptions QueryRequestOptions { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }
    public class CreatePartitionedItemRequest : BaseCosmosDBRequest
    {
        public Dictionary<string, object> Item { get; set; }
        public QueryRequestOptions QueryRequestOptions { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }
    public class GetPartitionedItemsResponse : BaseCosmosDBResponse<Dictionary<string,object>>
    {
        public IEnumerable<Dictionary<string, object>> Results { get; set; }
        public string ContinuationToken { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }

    public class CreatePartitionedItemResponse : BaseCosmosDBResponse<Dictionary<string, object>>
    {
        public IEnumerable<Dictionary<string, object>> Results { get; set; }
        public string ContinuationToken { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }

    public class GetItemsResponse<T> : BaseCosmosDBResponse<T>
    {
        public IEnumerable<T> Results { get; set; }
        public string ContinuationToken { get; set; }
        public int MaxItemCount { get; set; } = -1;
    }

    public class CreateItemRequest<T> : BaseCosmosDBRequest { 
        public T Item { get; set; }


        //Counter Properties
        public bool isCounter { get; set; } = false;

        public string Name { get; set; } = string.Empty;

        public bool isClassTok { get; set; } = false; //I'll leave this name as is

        public Dictionary<string, object> ItemDictionary { get; set; } = null;
    }

    public class CreateItemResponse<T> : BaseCosmosDBResponse<T> {
        public Dictionary<string,object> Item { get; set; }

        public bool IsPartitioned { get; set; }
    }

    public class DeleteItemRequest : BaseCosmosDBRequest { }

    public class DeleteItemResponse<T> : BaseCosmosDBResponse<T> { }

    public class UpdateItemRequest<T> : BaseCosmosDBRequest { public T Item { get; set; } }

    public class UpdateItemResponse<T> : BaseCosmosDBResponse<T> { }

    public class PatchItemRequest : BaseCosmosDBRequest
    {
        public Dictionary<string, long> FieldsIncrement { get; set; } = null;

        public Dictionary<string, object> FieldsUpdate { get; set; } = null;

        //Counter Properties
        public bool isCounter { get; set; } = false;

        public int Retries { get; set; } = 1;

        public string Name { get; set; } = string.Empty;

        public bool isClassTok { get; set; } = false; //I'll leave this name as is

        public Dictionary<string, object> ItemDictionary { get; set; } = null;
    }

    public class PatchItemResponse<T> : BaseCosmosDBResponse<T> { }



    /// <summary></summary>
    //Note: "docid-pk" with no numbers manages all partitions. "docid-pk0" and "docid-pk1...2" manage individual counts. Both parent and child should be updated with each insert
    public class PartitionManager : BaseModel
    {
        /// <summary>Determines how the document is stored</summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "partid-toks";

        /// <summary>Determines how the document is stored</summary>
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; } = "partid-toks";

        /// <summary>Number of documents</summary>
        [JsonProperty(PropertyName = "docs")]
        public long Documents { get; set; }

        /// <summary>Number of deleted documents</summary>
        [JsonProperty(PropertyName = "deleted_docs")]
        public long DeletedDocuments { get; set; }

        /// <summary>Type of document</summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "partitionmanager";
    }


}
