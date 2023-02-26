//using Microsoft.Azure.Cosmos;
//using Microsoft.Azure.Cosmos.Linq;
//using Microsoft.Azure.Cosmos.Scripts;
//using Newtonsoft.Json;
//using Shared;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Tokket.Tokkepedia;
//using Tokket.Tokkepedia.Tools;
//using static Shared.Constants;

//namespace SharedDB
//{
//    public interface IApi
//    {
//        CosmosClient Client { get; set; }
//        FileUploader BlobClient { get; set; }

//        //StreamClient StreamClient { get; set; }
//        //FirebaseAuthProvider AuthProvider { get; set; }

//        Task<T> CreateItemAsync<T>(T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<T> GetItemAsync<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<T> UpdateItemAsync<T>(string id, T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<T> DeleteItemAsync<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<ResultData<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> orderBySelector,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = DefaultDB, string container = DefaultCNTR);
//        Task<T> IncrementCountAsync<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR); //"$inc"
//        Task<T> UpdateFieldsAsync<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR); //"$set"

//        #region TokkepediaResponse variants
//        Task<TokkepediaResponse<T>> CreateItemAsyncResponse<T>(T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<TokkepediaResponse<T>> GetItemAsyncResponse<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<TokkepediaResponse<T>> UpdateItemAsyncResponse<T>(string id, T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<TokkepediaResponse<T>> DeleteItemAsyncResponse<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);
//        Task<TokkepediaResponse<ResultData<T>>> GetItemsAsyncResponse<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> orderBySelector,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR);
//        Task<TokkepediaResponse<T>> IncrementCountAsyncResponse<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR, int retries = 1); //"$inc"
//        #endregion

//        Task<TokkepediaResponse<Dictionary<string, object>>> CreatePartitionedItemAsync(Dictionary<string, object> item, string partitionKeyBase, int partitionMax = 1000000, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge");

//        Task<List<TokkepediaResponse<Dictionary<string, object>>>> CreateMultiPartitionedItemAsync(Dictionary<string, object> item, List<string> partitionKeyBases, bool parallel = false, bool includeOriginalItem = true, int partitionMax = 1000000, ItemRequestOptions options = null, string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR);
//        Task<List<TokkepediaResponse<T>>> UpdateMultiPartitionedItemAsync<T>(string id, T item, string[] partitionKeys, string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR);
//        Task<List<TokkepediaResponse<T>>> DeleteMultiPartitionedItemsAsync<T>(string id, List<string> partitionKeys, string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR);


//        //Special functions
//        Task<TokkepediaResponse<T>> GetAndUpdateItemAsync<T>(string id, string partitionKey, Func<T, T> updates, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR);

//        Task<TokkepediaResponse<ResultData<T>>> GetPartitionedItemsAsyncResponse<T>(Expression<Func<Dictionary<string, object>, bool>> predicate, Expression<Func<Dictionary<string, object>, dynamic>> orderBySelector, string partitionKeyBase, int partitionMax = 1000000,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR);
//    }

//    public class Api : IApi
//    {
//        public CosmosClient Client { get; set; }
//        public FileUploader BlobClient { get; set; }

//        public Api()
//        {
//            Client = new CosmosClient(Environment.GetEnvironmentVariable(Shared.Constants.CosmosLocation));
//        }

//        //For unit test only
//        //public Api(){}

//        public Api(string cosmosConnectionString)
//        {
//            Client = new CosmosClient(cosmosConnectionString);
//        }

//        public Api(string cosmosConnectionString, string blobConnectionString)
//        {
//            Client = new CosmosClient(cosmosConnectionString);
//            BlobClient = new FileUploader(blobConnectionString);
//        }

//        public async Task<T> CreateItemAsync<T>(T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            return (await Client.GetDatabase(db).GetContainer(container).CreateItemAsync<T>(item, new PartitionKey(partitionKey), options)).Resource;
//        }

//        public async Task<T> DeleteItemAsync<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            return (await Client.GetDatabase(db).GetContainer(container).DeleteItemAsync<T>(id, new PartitionKey(partitionKey), options)).Resource;
//        }

//        public async Task<T> GetItemAsync<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            try { return (await Client.GetDatabase(db).GetContainer(container).ReadItemAsync<T>(id, new PartitionKey(partitionKey), options)).Resource; }
//            catch (Exception ex) { return default; }
//        }

//        public async Task<T> UpdateItemAsync<T>(string id, T item, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            return (await Client.GetDatabase(db).GetContainer(container).ReplaceItemAsync<T>(item, id, new PartitionKey(partitionKey), options)).Resource;
//        }

//        public async Task<ResultData<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> orderBySelector,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = DefaultDB, string container = DefaultCNTR)
//        {
//            int maxCount = (options?.MaxItemCount == null) ? -1 : (int)options.MaxItemCount;

//            var _container = Client.GetDatabase(db).GetContainer(container);
//            var queryable = _container.GetItemLinqQueryable<T>(false, continuationToken, options);
//            var queryItem = queryable.OrderByDescending(orderBySelector).Where(predicate);

//            //Do not use, Take, load more will not work
//            //if (maxCount > 0)
//            //    queryItem = queryItem.Take(maxCount);

//            var iterator = queryItem.ToFeedIterator();
//            var results = await iterator.ReadNextAsync();

//            ResultData<T> resultData = new ResultData<T>()
//            {
//                Results = results.Resource,
//                ContinuationToken = results.ContinuationToken,
//                Limit = maxCount
//            };

//            return resultData;
//        }

//        public async Task<T> IncrementCountAsync<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            var counter = await Client.GetDatabase(db).GetContainer(container)
//                .Scripts.ExecuteStoredProcedureAsync<T>("update", new PartitionKey(partitionKey), new[] { id, updateString });

//            return counter.Resource;
//        }

//        public async Task<T> UpdateFieldsAsync<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            var counter = await Client.GetDatabase(db).GetContainer(container)
//                .Scripts.ExecuteStoredProcedureAsync<T>("update", new PartitionKey(partitionKey), new[] { id, updateString });

//            return counter.Resource;
//        }

//        #region Response
//        public async Task<TokkepediaResponse<T>> CreateItemAsyncResponse<T>(T item, string partitionKey, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge")
//        {
//            var result = await Client.GetDatabase(db).GetContainer(container).CreateItemAsync<T>(item, new PartitionKey(partitionKey), options);
//            return GetTokkepediaResponse(result);
//        }

//        public async Task<TokkepediaResponse<T>> GetItemAsyncResponse<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = DefaultDB, string container = DefaultCNTR)
//        {
//            try
//            {
//                var result = await Client.GetDatabase(db).GetContainer(container).ReadItemAsync<T>(id, new PartitionKey(partitionKey), options);
//                return GetTokkepediaResponse(result);
//            }
//            catch (Exception ex)
//            {
//                return default;
//            }
//        }

//        public async Task<TokkepediaResponse<T>> UpdateItemAsyncResponse<T>(string id, T item, string partitionKey, ItemRequestOptions options, string db = "Tokket", string container = "Knowledge")
//        {
//            try
//            {
//                var result = await Client.GetDatabase(db).GetContainer(container).ReplaceItemAsync<T>(item, id, new PartitionKey(partitionKey), options);
//                return GetTokkepediaResponse(result);
//            }
//            catch (Exception ex)
//            {
//                return default;
//            }
//        }

//        public async Task<TokkepediaResponse<T>> DeleteItemAsyncResponse<T>(string id, string partitionKey, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge")
//        {
//            var result = await Client.GetDatabase(db).GetContainer(container).DeleteItemAsync<T>(id, new PartitionKey(partitionKey), options);
//            return GetTokkepediaResponse(result);
//        }

//        public async Task<TokkepediaResponse<T>> IncrementCountAsyncResponse<T>(string id, string partitionKey, string updateString, string storedProcedureId = "update", StoredProcedureRequestOptions options = null, string db = "Tokket", string container = "Knowledge", int retries = 1)
//        {
//            StoredProcedureExecuteResponse<T> result = null;
//            TokkepediaResponse<T> response = null;

//            bool success = false; int completedRetries = 0;
//            while (!success && completedRetries < retries)
//            {
//                try
//                {
//                    result = await Client.GetDatabase(db).GetContainer(container)
//                    .Scripts.ExecuteStoredProcedureAsync<T>("update", new PartitionKey(partitionKey), new[] { id, updateString });
//                    response = GetTokkepediaResponse(result);

//                    success = true;
//                    return response;
//                }
//                catch (Exception ex)
//                {
//                    success = false;
//                    if (ex.Message.Contains("Document not found."))
//                    {
//                        //Create new counter before continuing (Does not count as a retry)
//                        try
//                        {
//                            response = await CreateNewCounter<T>(id, partitionKey);
//                            continue;
//                        }
//                        catch (Exception e)
//                        {
//                            //Try to increment again if a new counter cannot be created
//                            continue;
//                        }
//                    }
//                    else
//                    {
//                        //Error not related to not found, so try again
//                        ++completedRetries;
//                        continue;
//                    }
//                }
//            }

//            if (response == null)
//                response = new TokkepediaResponse<T>() { RequestCharge = 0 };

//            return response;
//        }

//        public async Task<TokkepediaResponse<ResultData<T>>> GetItemsAsyncResponse<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> orderBySelector,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR)
//        {
//            try
//            {
//                var _container = Client.GetDatabase(db).GetContainer(container);
//                var queryable = _container.GetItemLinqQueryable<T>(false, continuationToken, options);
//                var queryItem = (descending) ? queryable.OrderByDescending(orderBySelector).Where(predicate) : queryable.OrderBy(orderBySelector).Where(predicate);
//                var iterator = queryItem.ToFeedIterator();
//                var results = await iterator.ReadNextAsync();
//                return GetTokkepediaResponse(results);
//            }
//            catch (Exception ex)
//            {
//                return default;
//            }
//        }

//        #region Create new counter if "Docucment not found" error
//        public async Task<TokkepediaResponse<T>> CreateNewCounter<T>(string id, string partitionKey,
//            string db = "Tokket", string container = "Knowledge")
//        {
//            T item = (T)GetNewCounter<T>(id, partitionKey);

//            var result = await Client.GetDatabase(db).GetContainer(container)
//                .CreateItemAsync<T>(item, new PartitionKey(partitionKey));
//            return GetTokkepediaResponse(result);
//        }

//        private object GetNewCounter<T>(string id, string partitionKey)
//        {
//            if (typeof(T) == typeof(UserCounter))
//                return new UserCounter() { Toks = 1, Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(Category))
//                return new Category() { Toks = 1, Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(TokType))
//                return new TokType() { Toks = 1, Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(TokkepediaCounter))
//                return new TokkepediaCounter() { Toks = 1, Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(GemCounter))
//                return new GemCounter() { Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(CommentCounter))
//                return new CommentCounter() { Id = id, PartitionKey = partitionKey };
//            else if (typeof(T) == typeof(ReactionCounter))
//                return new ReactionCounter() { Id = id, PartitionKey = partitionKey };
//            //else if (typeof(T) == typeof(ReplyCounter))
//            //    return new ReplyCounter() { Id = id, PartitionKey = partitionKey };
//            else
//                return default(T);
//        }
//        #endregion

//        private TokkepediaResponse<T> GetTokkepediaResponse<T>(ItemResponse<T> result) => new TokkepediaResponse<T>() { Resource = result.Resource, RequestCharge = result.RequestCharge, StatusCode = result.StatusCode, Etag = result.ETag };
//        private TokkepediaResponse<T> GetTokkepediaResponse<T>(StoredProcedureExecuteResponse<T> result) => new TokkepediaResponse<T>() { Resource = result.Resource, RequestCharge = result.RequestCharge, StatusCode = result.StatusCode, Etag = result.ETag };
//        private TokkepediaResponse<ResultData<T>> GetTokkepediaResponse<T>(FeedResponse<T> result) =>
//            new TokkepediaResponse<ResultData<T>>()
//            {
//                Resource = new ResultData<T>()
//                {
//                    Limit = result.Count,
//                    Results = result.ToList()
//                },
//                RequestCharge = result.RequestCharge,
//                StatusCode = result.StatusCode,
//                Etag = result.ETag,
//                ContinuationToken = result.ContinuationToken,
//                Count = result.Count
//            };

//        #endregion

//        public async Task<TokkepediaResponse<Dictionary<string, object>>> CreatePartitionedItemAsync(Dictionary<string, object> item, string partitionKeyBase, int partitionMax = 1000000, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge")
//        {
//            TokkepediaResponse<Dictionary<string, object>> response = new TokkepediaResponse<Dictionary<string, object>>() { RequestCharge = 0, RequestChargeBreakdown = new List<(string, double?)>() };

//            // #1: Decide which partition to insert to by getting the partition manager
//            // Example: category-philippines-toks, toks
//            var partitionManager = await GetItemAsyncResponse<PartitionManager>(partitionKeyBase, partitionKeyBase, null, db, container);
//            if (partitionManager == null)
//            {
//                PartitionManager manager = new PartitionManager() { Documents = 1, DeletedDocuments = 0, Id = partitionKeyBase, PartitionKey = partitionKeyBase };
//                partitionManager = await CreateItemAsyncResponse<PartitionManager>(manager, manager.PartitionKey, null, db, container);
//                response.RequestCharge += partitionManager.RequestCharge;
//                response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
//            }
//            else
//            {
//                response.RequestCharge += partitionManager.RequestCharge;
//                response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
//            }

//            // #2: Calculate Partition number
//            long partitionNum = Convert.ToInt32(Math.Floor(partitionManager.Resource.Documents / (double)partitionMax));
//            string partitionKey = $"{partitionKeyBase}{partitionNum}";

//            // #3: Add document to partition. Increment partition manager
//            item["pk"] = partitionKey;
//            var created = await CreateItemAsyncResponse<Dictionary<string, object>>(item, partitionKey, null, db, container);
//            response.RequestCharge += created.RequestCharge;
//            response.RequestChargeBreakdown.Add(("created", created.RequestCharge));

//            response.Resource = created.Resource;
//            return response;
//        }

//        #region Multi Partitioned
//        public async Task<List<TokkepediaResponse<Dictionary<string, object>>>> CreateMultiPartitionedItemAsync(Dictionary<string, object> item, List<string> partitionKeyBases, bool parallel = false, bool includeOriginalItem = true, int partitionMax = 1000000, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge")
//        {
//            List<TokkepediaResponse<Dictionary<string, object>>> response = new List<TokkepediaResponse<Dictionary<string, object>>>()
//            {
//                //new TokkepediaResponse<Dictionary<string, object>>(){ RequestCharge = 0, RequestChargeBreakdown = new List<(string, double?)>() }
//            };

//            //Each Tok duplicate has all pks duplicates
//            //First get all duplicates
//            List<string> lstPartitionKeys = new List<string>();
//            foreach (string partitionKeyBase in partitionKeyBases)
//            {
//                TokkepediaResponse<Dictionary<string, object>> r = new TokkepediaResponse<Dictionary<string, object>>() { RequestCharge = 0, RequestChargeBreakdown = new List<(string, double?)>() };

//                // #1: Decide which partition to insert to by getting the partition manager
//                // Example: category-philippines-toks, toks
//                var partitionManager = await GetItemAsyncResponse<PartitionManager>(partitionKeyBase, partitionKeyBase, null, db, container);
//                if (partitionManager == null)
//                {
//                    PartitionManager manager = new PartitionManager() { Documents = 1, DeletedDocuments = 0, Id = partitionKeyBase, PartitionKey = partitionKeyBase };
//                    partitionManager = await CreateItemAsyncResponse<PartitionManager>(manager, manager.PartitionKey, null, db, container);
//                    r.RequestCharge += partitionManager?.RequestCharge;
//                    r.RequestChargeBreakdown.Add(("partitionManager", partitionManager?.RequestCharge));
//                }
//                else
//                {
//                    r.RequestCharge += partitionManager?.RequestCharge;
//                    r.RequestChargeBreakdown.Add(("partitionManager", partitionManager?.RequestCharge));
//                }

//                // #2: Calculate Partition number
//                // Example: category-philippines-toks0, toks0
//                long partitionNum = Convert.ToInt64(Math.Truncate(partitionManager.Resource.Documents / (double)partitionMax));
//                string partitionKey = $"{partitionKeyBase}{partitionNum}";

//                lstPartitionKeys.Add(partitionKey);
//            }
//            item["duplicates"] = lstPartitionKeys;

//            //Create Original
//            Dictionary<string, object> mainTok;
//            if (includeOriginalItem)
//                mainTok = await CreateItemAsync(item, item["id"].ToString().Trim()); // Important=

//            //Create duplicates
//            foreach (string partitionKey in lstPartitionKeys)
//            {
//                // #3: Add document to partition. Increment partition manager
//                item["pk"] = partitionKey;
//                var created = await CreateItemAsyncResponse(item, partitionKey, null, db, container);

//                //response[num].RequestCharge += created.RequestCharge;
//                //response[num].RequestChargeBreakdown.Add(("created", created.RequestCharge));

//                //response[num].Resource = created.Resource;
//                //num++;

//                //Add to list
//                //response.Add(r);
//            }

//            return response;
//        }




//        public async Task<List<TokkepediaResponse<T>>> UpdateMultiPartitionedItemAsync<T>(string id, T item, string[] partitionKeys, string db = "Tokket", string container = "Knowledge")
//        {
//            List<TokkepediaResponse<T>> responses = new List<TokkepediaResponse<T>>();// { RequestCharge = 0, RequestChargeBreakdown = new List<(string, double?)>() };

//            List<Task<TokkepediaResponse<T>>> tasks = new List<Task<TokkepediaResponse<T>>>();

//            foreach (string pk in partitionKeys)
//            {
//                Dictionary<string, object> myItem;
//                if (item is Dictionary<string, object>)
//                    myItem = item as Dictionary<string, object>; //Does not convert, sets myItem to null if not the same type
//                else
//                    myItem = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(item));

//                myItem["pk"] = pk;
//                tasks.Add(UpdateItemAsyncResponse(id, item, pk, null, db, container));
//            }

//            await Task.WhenAll(tasks);

//            for (int i = 0; i < tasks.Count; ++i)
//            {
//                TokkepediaResponse<T> response = await tasks[i];
//                responses.Add(response);
//            }

//            return responses;
//        }

//        public async Task<List<TokkepediaResponse<T>>> DeleteMultiPartitionedItemsAsync<T>(string id, List<string> partitionKeys, string db = "Tokket", string container = "Knowledge")
//        {
//            List<TokkepediaResponse<T>> responses = new List<TokkepediaResponse<T>>();

//            List<Task<TokkepediaResponse<T>>> tasks = new List<Task<TokkepediaResponse<T>>>();

//            foreach (string pk in partitionKeys)
//            {
//                tasks.Add(DeleteItemAsyncResponse<T>(id, pk, null, db, container));
//            }

//            await Task.WhenAll(tasks);

//            for (int i = 0; i < tasks.Count; ++i)
//            {
//                TokkepediaResponse<T> response = await tasks[i];
//                responses.Add(response);
//            }

//            return responses;
//        }

//        #endregion

//        public async Task<TokkepediaResponse<T>> GetAndUpdateItemAsync<T>(string id, string partitionKey, Func<T, T> updates, ItemRequestOptions options = null, string db = "Tokket", string container = "Knowledge")
//        {
//            TokkepediaResponse<T> response;

//            //Get item
//            response = await GetItemAsyncResponse<T>(id, partitionKey, options, db, container);
//            if (response == null) response = new TokkepediaResponse<T>() { StatusCode = System.Net.HttpStatusCode.BadRequest, RequestCharge = 0 };

//            //Action to execute
//            var newItem = updates(response.Resource);

//            //Update Item
//            if (options == null)
//                options = new ItemRequestOptions();
//            options.IfMatchEtag = response.Etag;
//            TokkepediaResponse<T> newResponse;
//            try
//            {
//                newResponse = await UpdateItemAsyncResponse<T>(id, newItem, partitionKey, options, db, container);
//                response.RequestCharge += newResponse.RequestCharge;
//            }
//            catch (Exception ex)
//            {
//                //Could not update
//                response = new TokkepediaResponse<T>() { StatusCode = System.Net.HttpStatusCode.BadRequest, RequestCharge = 0 };
//            }

//            return response;
//        }

//        #region Partitioned Reads
//        public async Task<TokkepediaResponse<ResultData<T>>> GetPartitionedItemsAsyncResponse<T>(Expression<Func<Dictionary<string, object>, bool>> predicate, Expression<Func<Dictionary<string, object>, dynamic>> orderBySelector, string partitionKeyBase, int partitionMax = 1000000,
//            string continuationToken = null, QueryRequestOptions options = null, bool descending = true,
//            string db = Shared.Constants.DefaultDB, string container = Shared.Constants.DefaultCNTR)
//        {
//            TokkepediaResponse<T> response = new TokkepediaResponse<T>() { RequestCharge = 0, RequestChargeBreakdown = new List<(string, double?)>() };

//            // #1: Decide which partition to insert to by getting the partition manager
//            // Example: category-philippines-toks, toks
//            var partitionManager = await GetItemAsyncResponse<PartitionManager>(partitionKeyBase, partitionKeyBase, null, db, container);
//            if (partitionManager == null)
//            {
//                PartitionManager manager = new PartitionManager() { Documents = 0, DeletedDocuments = 0, Id = partitionKeyBase, PartitionKey = partitionKeyBase };
//                partitionManager = await CreateItemAsyncResponse<PartitionManager>(manager, manager.PartitionKey, null, db, container);
//                response.RequestCharge += partitionManager.RequestCharge;
//                response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
//            }
//            else
//            {
//                response.RequestCharge += partitionManager.RequestCharge;
//                response.RequestChargeBreakdown.Add(("partitionManager", partitionManager.RequestCharge));
//            }

//            // #2: Calculate Partitions number to add to query
//            // Example: category-philippines-toks0, toks0
//            long partitionNum = Convert.ToInt32(Math.Floor(partitionManager.Resource.Documents / (double)partitionMax));
//            List<string> partitions = new List<string>();
//            for (int i = 0; i < partitionNum + 1; ++i)
//                partitions.Add(partitionKeyBase + i.ToString());
//            predicate = predicate.And(x => partitions.Contains(x["pk"].ToString()));

//            // #3: Query
//            int maxCount = -1;
//            if (options == null)
//                options = new QueryRequestOptions() { MaxItemCount = -1 };

//            var _container = Client.GetDatabase(db).GetContainer(container);
//            var queryable = _container.GetItemLinqQueryable<Dictionary<string, object>>(false, continuationToken, options);
//            var queryItem = queryable.OrderByDescending(orderBySelector).Where(predicate);
//            var iterator = queryItem.ToFeedIterator();
//            var results = await iterator.ReadNextAsync();

//            ResultData<T> resultData = new ResultData<T>()
//            {
//                Results = JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(results.Resource)),
//                ContinuationToken = results.ContinuationToken,
//                Limit = maxCount
//            };

//            return GetTokkepediaResponse<T>(results, resultData);
//        }

//        private TokkepediaResponse<ResultData<T>> GetTokkepediaResponse<T>(FeedResponse<Dictionary<string, object>> results, ResultData<T> resultData) => new TokkepediaResponse<ResultData<T>>() { Resource = resultData, RequestCharge = results.RequestCharge, StatusCode = results.StatusCode, Etag = results.ETag };
//        #endregion
//    }
//}
