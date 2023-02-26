using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core
{
    public interface IAdminService
    {
        public PatchSuperAdminSetRoleResponse PatchSuperAdminSetRole(PatchSuperAdminSetRoleRequest request);
        public Task<PatchSuperAdminSetRoleResponse> PatchSuperAdminSetRoleAsync(PatchSuperAdminSetRoleRequest request);

        public Task<GetUsersAdminResponse> GetUsersAdminAsync(GetUsersAdminRequest request);

        public Task<PostUserAdminResponse> PostUserAdminAsync(PostUserAdminRequest request);

        public Task<PatchUserAdminResponse> PatchUserAdminAsync(PatchUserAdminRequest request);
        public Task<DeleteUserAdminResponse> DeleteUserAdminAsync(DeleteUserAdminRequest request);
    }
    #region Requests and Responses

    public class PostUserAdminRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level0; }
        public AuthenticatedUser Item { get; set; }

        public string UserId { get; set; }

        public string RoleId
        {
            get => RoleId;
            set
            {
                if (Roles.ValidRoles.Contains(value))
                    RoleId = value;
                else
                    throw new InvalidOperationException($"Invalid role: {value}");
            }
        }
    }

    public class PostUserAdminResponse : BaseResponse<bool>
    {
        public override bool Result { get; set; }

        public AuthenticatedUser Item { get; set; }
    }

    public class PatchUserAdminRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level0; }
        public AuthenticatedUser Item { get; set; }

        public string UserId { get; set; }

        public string RoleId
        {
            get => RoleId;
            set
            {
                if (Roles.ValidRoles.Contains(value))
                    RoleId = value;
                else
                    throw new InvalidOperationException($"Invalid role: {value}");
            }
        }
    }

    public class PatchUserAdminResponse : BaseResponse<bool>
    {
        public override bool Result { get; set; }
    }

    public class DeleteUserAdminRequest : BaseAuthenticatedRequest
    {
        protected override string RequiredRole { get => Roles.Level0; }

        /// <summary>
        /// The id of the user admin to delete.
        /// </summary>
        public string Item { get; set; }

        public string UserId { get; set; }

        public string RoleId
        {
            get => RoleId;
            set
            {
                if (Roles.ValidRoles.Contains(value))
                    RoleId = value;
                else
                    throw new InvalidOperationException($"Invalid role: {value}");
            }
        }
    }

    public class DeleteUserAdminResponse : BaseResponse<bool>
    {
        public override bool Result { get; set; }
    }

    #endregion

    public static class SamplesTool
    {
        public static List<AuthenticatedUser> GetSampleUsersAdmin()
        {
            return new List<AuthenticatedUser>()
        {
            new AuthenticatedUser()
            {
                Id = "usera",
                DisplayName = "User",
                Role = Roles.Level1,
                Email = "user@gmail.com",
                FirstName = "User",
                LastName = "User",
                InternshipStart = DateTime.Now,
                School = "University",
                Major = "Computer Science",
                DegreeType = "Bachelors",
                GraduationMonth = "May",
                GraduationYear = "2025"
            },
            new AuthenticatedUser()
            {
                Id = "usera",
                DisplayName = "User",
                Role = Roles.Level1,
                Email = "user@gmail.com",
                FirstName = "User",
                LastName = "User",
                InternshipStart = DateTime.Now,
                School = "University",
                Major = "Computer Science",
                DegreeType = "Bachelors",
                GraduationMonth = "May",
                GraduationYear = "2025"
            },
            new AuthenticatedUser()
            {
                Id = "usera",
                DisplayName = "User",
                Role = Roles.Level1,
                Email = "user@gmail.com",
                FirstName = "User",
                LastName = "User",
                InternshipStart = DateTime.Now,
                School = "University",
                Major = "Computer Science",
                DegreeType = "Bachelors",
                GraduationMonth = "May",
                GraduationYear = "2025"
            }
        };
        }
    }
}
