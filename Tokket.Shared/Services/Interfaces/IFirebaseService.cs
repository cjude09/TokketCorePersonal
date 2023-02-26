using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<long> RetrieveExpiration(string token);

        ///<summary>Returns the user id (UID) stored in the token.</summary>
       // Task<ResultModel> CheckRequestAndVerify(HttpRequest req);

        Task<ResultModel> VerifyToken(AuthorizationTokenModel model);
        Task<ResultModel> RefreshToken(string refreshToken);
        FirebaseAdmin.Auth.FirebaseAuth GetFirebaseAppAdmin();
    }
}
