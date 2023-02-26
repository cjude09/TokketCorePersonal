using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Tokket.Core;

namespace Tokket.Infrastructure
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly ApiOptions _options;

        private FirebaseAuthConfig FirebaseConfig { get; set; }
        public FirebaseAuthClient AuthProvider { get; set; }
        #region Members and Properties
        //private readonly FirebaseConfig FirebaseConfig;  // This is for 4.0.0 alpha-2
        //public readonly FirebaseAuthProvider AuthProvider;  // This is for 4.0.0 alpha-2
        private readonly FirebaseApp FirebaseApp;
        public readonly FirebaseAdmin.Auth.FirebaseAuth FirebaseAppAdmin;
#if DEBUG
        public const bool IS_PROD = true;
#elif RELEASE
         public const bool IS_PROD = true;
#endif
        #region Constants
        /// <summary> Name of the Firebase Admin file to use. </summary>
        public const string FirebaseAdminFileName = IS_PROD ? "firebaseadminprod.json" : "firebaseadmindev.json";
        #endregion
        #endregion


        public FirebaseAuthService(IOptions<ApiOptions> options)
        {
            _options = options.Value;

            FirebaseConfig = new FirebaseAuthConfig()
            {
                ApiKey = _options.IsProd ? _options.FirebaseAppKey : _options.FirebaseAppKeyDev,
                AuthDomain = _options.IsProd ? "tokket-inc.firebaseapp.com" : "tokket-app.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            };
            AuthProvider = new FirebaseAuthClient(FirebaseConfig);
        }

        public FirebaseAuthService(ApiOptions options)
        {
            _options = options;

            FirebaseConfig = new FirebaseAuthConfig()
            {
                ApiKey = _options.IsProd ? _options.FirebaseAppKey : _options.FirebaseAppKeyDev,
                AuthDomain = _options.IsProd ? "tokket-inc.firebaseapp.com" : "tokket-app.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() },
                
            };
            AuthProvider = new FirebaseAuthClient(FirebaseConfig);
            var json = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"tokket-inc\",\n  \"private_key_id\": \"fa97d3073a6ed3e98e6df2094da565bfe994f5b6\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDv4VyxeKTqcDQQ\\nhuVMoFb6cefAV16uObAZc1Io6GpR05BgmZGzVShxz3SPGTXu8ALoqJmTbYWMueeu\\nYYMwXKSVInl2gta7LkhTNL/Gmi/3jX3sDkyY+tSx0WA6da2t9Db1ra3cOqpwUCmU\\nAQi3E8/cUFkloKftYUgZ1Aez5wD0oi6GJG7WHpVzNRa/G1G9yBRFuizgMc5X+7n1\\n+4CPSMOjCgNiwat6wTgBrYMdimaUn6jHyCo3uGFP7xBCl5yfJoactD1QmT+9JQUX\\nhG8h7Sekt5GyGLOUNUQg+OBGDzXyuy8ZuX71u9Qqb2HEzVaf6bB6AbKkRfcAi1h5\\nxcd0djGJAgMBAAECggEAAr6HhrE8Y8E2W0ULzQFdlV5luhXVy7T1vojK9RRZmQeM\\nVIqVN+G+xXSL3xI5A3uD8QDyxtS/LSG4VI6hh1Vb/afID3LGPZx1L9x/4CgF6fVY\\nrjFor2xZMBKW4PxVKRMM6OkcftM9/WxxWVPcs2l63IifyHYqzaSqBD+gee6n3GST\\n652NCiElv319ZKwLvvAfkO+8az5v2RAJrNTtLKAHWGrlmV8gsxc2BaiaYy+7Cdk2\\nBeloED9eIb/f16GwbAJqTiqjk8F6XInB1Y+hm2/dLLkTx3mmS1fk7rXVgeBIK+qh\\nLUBGGrolxNc243t2jV0A7/IpL28ib8vKuSO6J3hsrQKBgQD5pNp2Z44V2WAU0RXt\\nMZtlbhQPBPWWxT7JNI8A8xcDlgmFQ9qfhwmI4NA5tP3sh+/umDJXx8ySTN1NiweH\\nmXId+ZlNUpp8UkPW2GPVzwSHelyHOMd/sAuKITy+ifVuxoRH3KuR8pXFviQz3PIH\\nWSLcWL1A9iDUzNRU5qvuYiLZNQKBgQD1/N7fs2M2kc/s9fsBtED/Z0uDBFftpmkI\\ns49+p06yqtao4XMvurnzSqYgl4ry5RQoFwZsNN/QVnc5dAfpa4L2T0yI6TvBykYJ\\nWNVAnBLXc0EBPKY3ZL0jNt97jIpwdnY1ugp5sC0kiSbSgF0MPctrv+vhC4HLlWBH\\noaEhpaIVhQKBgQDMk8XbRh1v1kUgif9X81EPG9ggPsYrGdTL+eA+vPbdH3UJ2oMs\\nO/MaUnEQ+TslPHjoo5yNxtPkCE1KoGY5Pwv/eG2iqdCjlJ63T3jw28cwZpuwzFzg\\nTJoIRhiLZG7WqqP4Z+PJpGwMMjdksOk+EFO7EpV0yL465OgT8zxuC8nXCQKBgCZD\\nKR++tYX9dEw1js+bDCkuFg7RFCRBKEFUPNPEjnc4H5+xQcuAzf/L8r9LEy/o9hOu\\nUS9vogi0CmODat+h+4L4nr0FGmhwYCiACtu76ypcIRZiKrCfGNRraqO7HqWTp5t/\\nzrVS9BKkyscfdFm9GvEtrzEYKx5Ro+JAf+HN88JJAoGAaWvBsQqBwDmPYKGL5B/Z\\nGxOcHx+I2ffQecogei1/1DbvhhLzcXjeY4EhQmTECs+YQh8rzex/yC8FUtzmHrsu\\niiuIvmCBqYDTVG+iotYYrWzdyEPi6wVAoBT+K/SS5EZGdjW8BbfSe6GaYj6iKmhw\\ngmfe8ZfW9T8D/4VQwC3hZA4=\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"firebase-adminsdk-5wxgq@tokket-inc.iam.gserviceaccount.com\",\n  \"client_id\": \"104102177281861874891\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-5wxgq%40tokket-inc.iam.gserviceaccount.com\"\n}\n";
            FirebaseApp = FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromJson(json) });
            FirebaseAppAdmin = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            //  FirebaseApp = FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile($"{path}/{Constants.FirebaseAdminFileName}") });
            //  FirebaseAppAdmin = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
        }

   
        public async Task<PostAuthorizationLoginResponse> PostAuthorizationLoginAsync(PostAuthorizationLoginRequest request)
        {
            Firebase.Auth.UserCredential link = await AuthProvider.SignInWithEmailAndPasswordAsync(request.Email, request.Password);

            var response = new PostAuthorizationLoginResponse();
            var model = new AuthorizationTokenModel()
            {
                Id = link.User.Uid
                //AuthToken = await link.User.GetIdTokenAsync()
            };
            response.Result = model;

            return response;
        }

        //PostVerifyAuthorizationTokenModelAsync
    }
}
