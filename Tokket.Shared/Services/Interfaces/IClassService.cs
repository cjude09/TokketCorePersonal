using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Services.ServicesDB;
using Tokket.Core;
using Tokket.Core.Tools;
using Tokket.Shared.Models.Tokquest;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IClassService
    {
        Task<ClassGroupModel> AddClassGroupAsync(ClassGroupModel item);
        Task<bool> UpdateClassGroupAsync(ClassGroupModel item);
        Task<bool> DeleteClassGroupAsync(string id, string pk);
        void SetCacheGroupGroupAsync(string fromCaller, List<ClassGroupModel> list);
        Task<ClassGroupModel> GetClassGroupAsync(string id);
        ResultData<ClassGroupModel> GetCachedClassGroupAsync(string fromCaller);
        Task<ResultData<ClassGroupModel>> GetClassGroupAsync(ClassGroupQueryValues queryValues, string fromCaller = "");
        Task<ClassGroupRequestModel> GetClassGroupRequestAsync(string id, string pk);
        Task<ClassGroupRequestModel> RequestClassGroupAsync(ClassGroupRequestModel item);
        Task<ResultData<ClassGroupRequestModel>> GetClassGroupJoinRequests(string continuationtoken, string groupid, RequestStatus requestStatus = RequestStatus.Pending);
        Task<ResultData<ClassGroupRequestModel>> GetClassGroupRequestAsync(ClassGroupRequestQueryValues queryValues);
        Task<bool> AcceptRequest(string id, string pk, ClassGroupModel model);
        Task<bool> DeclineRequest(string id, string pk, ClassGroupModel model);
        Task<bool> LeaveClassGroupAsync(string id, string pk);

        Task<bool> RemoveMemberClassGroupAsync(string id, string pk,ClassGroupModel groupModel);
        Task<ResultData<TokketUser>> GetGroupMembers(string groupid, string paginationid = "");
        Task<bool> AddClassSetAsync(ClassSetModel item, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> UpdateClassSetAsync(ClassSetModel item, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteClassSetAsync(string id, string pk);
        Task<ClassSetModel> GetClassSetAsync(string id, string pk);
        ResultData<ClassSetModel> GetCacheClassSetAsync(string fromCaller);
        Task<ResultData<ClassSetModel>> GetClassSetAsync(ClassSetQueryValues queryValues, CancellationToken cancellationToken = default(CancellationToken), string fromCaller = "");
        void SetCacheClassSetAsync(string fromCaller, List<ClassSetModel> classSetList);
        Task<ResultModel> AddClassToksAsync(ClassTokModel item, CancellationToken cancellationToken = default(CancellationToken));
        Task<ResultModel> UpdateClassToksAsync(ClassTokModel item, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteClassToksAsync(string id, string pk);
        Task<ClassTokModel> GetClassTokAsync(string id, string pk);
        Task<ResultData<ClassTokModel>> GetClassToksAsync(ClassTokQueryValues queryValues, CancellationToken cancellationToken = default(CancellationToken), string fromCaller = "");
        string AddNewToksFoundCache(string fromCaller = "", string resource = "");
        ResultData<ClassTokModel> GetCacheClassToksAsync(string fromCaller = "");
        Task<ResultData<CommonModel>> GetMoreFilterOptions(ClassTokQueryValues queryValues, CancellationToken token = default(CancellationToken));
        Task<bool> AddClassToksToClassSetAsync(string classsetId, string pk, List<string> classtokIds, List<string> classtokPks);
        Task<bool> DeleteClassToksFromClassSetAsync(ClassSetModel classset, List<string> classtokIds);
        Task<ResultData<ClassGroupRequestModel>> GetClassGroupInvites(ClassGroupRequestQueryValues queryValues);

        Task<List<TokModel>> GetClassGroupDocs(ClassGroupModel classGroup, string paginationId = null);

        Task UploadDocsToGroup(ClassGroupModel classGroup,List<FileModel> fileList);
        Task<ResultModel> AddClassSetsToGroupAsync(string groupId, string pk, List<string> classsetIds);

        //ClassToksServiceDB refactored section

        Task<ResultData<ClassTokModel>> GetClassToksAsync(GetClassToksRequest request);
        void SetCacheClassToksAsync(string fromCaller, List<ClassTokModel> list);
        Task<ResultData<ClassGroup>> GetCommunitiesAsync(ClassGroupQueryValues queryValues);
    }
}
