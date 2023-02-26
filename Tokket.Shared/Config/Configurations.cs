using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Config
{
    public static class Configurations
    {
        public const string ApiPrefix = "/v1";
        //public const string ApiKey = "4d511486b72a41a6ae8d6716a101d0c4";
        public const string CodePrefix = "?code=";
        public const string ServiceId = "tokkepedia";
        public const string DevicePlatform = "android";
        public const bool isProd = false;

        // Cris Note: 
        //      [IMPORTANT - Please Read] 
        //      Please don't change this if you don't know what your changing.
        //      If you want to test in production just change your build into "RELEASE" mode beside "iPhoneSimulator/Any CPU"
        //      Or Don't forget to REVERT IT BACK if you are trying to do BREAKPOINTS something in DEBUG mode.
//#if DEBUG
//        public const string Url = " https://tokkepediab.com/";
//        public const string BaseUrl = "https://tokkepedia.azure-api.net";
//        public const string ApiKey = "bdf93a29c82d41e48daf2700247220e5";
//        public const string UrlRoom = "https://tokroomapipre.azurewebsites.net/api/";
//        public const string ApiKeyRoom = "zakiHRu5BWnaoEkQQWIheD6pQAZwlC1nWepgwEBxzM3AhXzRYg4/uQ==";
//#elif RELEASE
//        public const string Url = " https://tokkepediab.com/";
//        public const string BaseUrl = "https://tokkepedia.azure-api.net";
//        public const string ApiKey = "bdf93a29c82d41e48daf2700247220e5";
//        public const string UrlRoom = "https://tokroomapipre.azurewebsites.net/api/";
//        public const string ApiKeyRoom = "zakiHRu5BWnaoEkQQWIheD6pQAZwlC1nWepgwEBxzM3AhXzRYg4/uQ==";
//#endif

        // [For temporary debugging for both DEBUG and RELEASE]
        public const string Url = "https://tokkepedia-dev.azurewebsites.net/";
        public const string BaseUrl = "https://tokkepediadev.azure-api.net";
        public const string ApiKey = "4d511486b72a41a6ae8d6716a101d0c4";
        public const string UrlRoom = "https://tokroomapidev.azurewebsites.net/api/";
        public const string ApiKeyRoom = "fdk0G3IZq9bBqdPaZU7WJuCBOy7uDIMcRzEm7ufWntz6tVHcm5JyKA==";



        public static bool AlphaToksEnabled = false;
        public static bool TokQuestEnabled = false;
    }
}
