using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokket.Core;

namespace Tokket.Infrastructure
{
    public class AdminService : IAdminService
    {
        private readonly ApiOptions _options;

        IAuthService _authService;
        IDatabaseService _dbService;

        public AdminService(IDatabaseService dbService, IAuthService authService)
        {
            _dbService = dbService;
            _authService = authService;
        }


        public async Task<DeleteUserAdminResponse> DeleteUserAdminAsync(DeleteUserAdminRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<GetUsersAdminResponse> GetUsersAdminAsync(GetUsersAdminRequest request)
        {
            var dbRequest = new GetItemsRequest<AuthenticatedUser>()
            {
                //PartitionKey = "usersadmin",
                Predicate = (x => x.Role == Roles.Level1 || x.Role == Roles.Level0),
                OrderBySelector = x => x.CreatedTime,
            };

            var dbResponse = await _dbService.GetItemsAsync<AuthenticatedUser>(dbRequest);

            //dbResponse.Results.ToList();
            return new GetUsersAdminResponse() { Result = dbResponse.Results.ToList() }; //Result = SamplesTool.GetSampleUsersAdmin()
        }

        public async Task<PatchUserAdminResponse> PatchUserAdminAsync(PatchUserAdminRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<PostUserAdminResponse> PostUserAdminAsync(PostUserAdminRequest request)
        {
            var dbRequest = new UpdateItemRequest<AuthenticatedUser>()
            {
                Id = request.Item.Id,
                PartitionKey = request.Item.PartitionKey,
                Item = request.Item
            };

            var dbResponse = await _dbService.UpdateItemAsync<AuthenticatedUser>(dbRequest);

            //dbResponse.Results.ToList();
            return new PostUserAdminResponse() { Result = true, Item = dbResponse.Result };
        }

        public PatchSuperAdminSetRoleResponse PatchSuperAdminSetRole(PatchSuperAdminSetRoleRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<PatchSuperAdminSetRoleResponse> PatchSuperAdminSetRoleAsync(PatchSuperAdminSetRoleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
