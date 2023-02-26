using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers.Interfaces;
using Tokket.Shared.Services;

namespace Tokket.Shared.Helpers
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private HttpClient _client;
        private string _apiUrl = string.Empty;
        public HttpClientHelper(string url)
        {
            var handler = new HttpClientHandler();
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(60),
                MaxResponseContentBufferSize = 2097152 // 2MB
            };
            _client.DefaultRequestHeaders.Add("serviceid", "tokkepedia"); //Valid: "tokket", "tokblitz", "tokkepedia"
#if __IOS__
            _client.DefaultRequestHeaders.Add("deviceplatform", "ios"); //Valid: "web", "android", "ios"
#elif __ANDROID__
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"
#else
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"
#endif

            _apiUrl = url;
        }

        public HttpClientHelper(string url, string token)
        {
            var handler = new HttpClientHandler();
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(60),
                MaxResponseContentBufferSize = 2097152 // 2MB
            };
            if (!string.IsNullOrEmpty(token)) _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Add("serviceid", "tokkepedia"); //Valid: "tokket", "tokblitz", "tokkepedia"
            _client.DefaultRequestHeaders.Add("deviceplatform", "ios"); //Valid: "web", "android", "ios"
#if __ANDROID__
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"    
#else
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"
#endif
            _apiUrl = url;
        }

        public HttpClientHelper(string url, string username, string password)
        {
            var handler = new HttpClientHandler();
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(60),
                MaxResponseContentBufferSize = 2097152 // 2MB
            };
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
            _client.DefaultRequestHeaders.Add("serviceid", "tokkepedia"); //Valid: "tokket", "tokblitz", "tokkepedia"
            _client.DefaultRequestHeaders.Add("deviceplatform", "ios"); //Valid: "web", "android", "ios"
#if __ANDROID__
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"     
#else
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"
#endif
            _apiUrl = url;

        }

        public HttpClient Instance => _client;

        public void ClearHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("serviceid", "tokkepedia"); //Valid: "tokket", "tokblitz", "tokkepedia"
            _client.DefaultRequestHeaders.Add("deviceplatform", "android"); //Valid: "web", "android", "ios"
            _client.DefaultRequestHeaders.Add("itemtotal", "-1"); 
        }

        public string Delete(string api) => DeleteAsync(api).Result;

        public async Task<string> DeleteAsync(string api)
        {
            string url = _apiUrl + api;
            var response = await _client.DeleteAsync(url);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Accepted:
                case System.Net.HttpStatusCode.BadRequest:
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.Forbidden:
                case System.Net.HttpStatusCode.NotFound:
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Unauthorized:
                    return await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        public string Post(string api, object message)
        {
            string url = _apiUrl + api;
            try
            {
                var model = JsonConvert.SerializeObject(message);
                var response = _client.PostAsync(url, new StringContent(model, Encoding.UTF8, "application/json")).GetAwaiter().GetResult(); // Alternate
                //var response = await _client.PostAsJsonAsync(url, message); // Original - Not Implemented for Mono

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Unauthorized:
                        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Asyncronously post data to api with header and body
        ///     Accepts status codes result(s): 200, 201, 400, 202, 403, 404, 401
        ///     Else will throw an exception
        /// </summary>
        /// <param name="api">API Url</param>
        /// <param name="message">Message or Body</param>
        /// <returns>returns a Deserialized Json Object in String format</returns>
        public async Task<string> PostAsync(string api, object message)
        {
            string url = _apiUrl + api;
            try
            {
                var model = JsonConvert.SerializeObject(message);
                var response = await _client.PostAsync(url, new StringContent(model, Encoding.UTF8, "application/json")); // Alternate
                //var response = await _client.PostAsJsonAsync(url, message); // Original - Not Implemented for Mono

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Unauthorized:
                        return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return string.Empty;
        }

        public string Get(string api)
        {
            return GetAsync(api).Result;
        }

        public string GetData(string api)
        {
            try
            {
                string url = _apiUrl + api;
                var response = _client.GetAsync(url).GetAwaiter().GetResult();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.NoContent:
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return string.Empty;
        }
        /// <summary>
        ///     Asyncronously get data from the api with header and body
        ///     Accepts status codes result(s): 200, 201, 400, 202, 403, 404, 401
        ///     Else will throw an exception
        /// </summary>
        /// <param name="api">API Url</param>
        /// <returns>returns a Deserialized Json Object in String format</returns>
        public async Task<string> GetAsync(string api)
        {
            try
            {
                string url = _apiUrl + api;
                var response = await _client.GetAsync(url);
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.NoContent:
                    return await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return string.Empty;
        }

        public string GetCachedAsync<T>(string url, bool forceRefresh = false)
        {
            var json = string.Empty;

            if (!CrossConnectivity.Current.IsConnected)
                json = Barrel.Current.Get<string>(url);

            try
            {
                if (!forceRefresh && !Barrel.Current.IsExpired(url))
                    json = Barrel.Current.Get<string>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return json;
        }

        public void SetCachedAsync<T>(string url, string json, int days = 7)
        {
            try
            {
                Barrel.Current.Add(url, json, TimeSpan.FromDays(days));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get information from server {ex}");
                //probably re-throw here :)
            }
        }

        public string Put(string api, object message)
        {
            return PutAsync(api, message).Result;
        }


        public async Task<string> PutAsync(string api, object message)
        {
            string url = _apiUrl + api;
            try
            {
                var model = JsonConvert.SerializeObject(message);
                var response = await _client.PutAsync(url, new StringContent(model, Encoding.UTF8, "application/json"));
                //var response = await _client.PutAsJsonAsync(url, message); // Original - Not Implemented for Mono
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Accepted:
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.Forbidden:
                    case System.Net.HttpStatusCode.NotFound:
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Unauthorized:
                        return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return string.Empty;
        }

        public string PostToken(string api, FormUrlEncodedContent message)
        {
            return PostTokenAsync(api, message).Result;
        }


        public async Task<string> PostTokenAsync(string api, FormUrlEncodedContent message)
        {

            string url = _apiUrl + api;
            var response = _client.PostAsync(url, message).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }
    }
}
