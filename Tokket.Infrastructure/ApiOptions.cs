using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Infrastructure
{
    public class ApiOptions
    {
        public const string Location = "ApiOptions";

        public bool IsProd { get; set; }
        public string ServiceId { get; set; }
        public string DevicePlatform { get; set; }

        public string CosmosDBConnectionStringDev { get; set; }
        public string FirebaseAppKeyDev { get; set; }


        //Prod
        public string CosmosDBConnectionString { get; set; }
        public string FirebaseAppKey { get; set; }

        public ApiOptions(string serviceId = "tokkepedia", string devicePlatform="android", bool isprod = false) {

            ServiceId = serviceId;
            DevicePlatform = devicePlatform;
            IsProd = isprod;
            CosmosDBConnectionStringDev = "AccountEndpoint=https://tokketdevelop.documents.azure.com:443/;AccountKey=8Pzkca9R3tXQsLR4H4m6DrUaBkrcQuCTNOQp7YDkw2kHPJnW8THwXhTqGZV76styMbszXUr1FI0vzBEPMPFOYA==;";
            FirebaseAppKeyDev = "AIzaSyD6azgiKhFG4DHv2SXm2-KTtMWOW5430uU";

            CosmosDBConnectionString = "AccountEndpoint=https://tokketinc.documents.azure.com:443/;AccountKey=7v8AcDMr6BP9zAM8YLsuQCry7xkR7Op7n6C8h2yPyedZHbWRXvqJKY5ORW7QsagbcRhV5zxs7Eihh7PrQ3XWPQ==;";
            FirebaseAppKey = "AIzaSyAQ01M0SKJ80SrGqUlz5vpLy-khXKCIoYk";
        }
    }
}
