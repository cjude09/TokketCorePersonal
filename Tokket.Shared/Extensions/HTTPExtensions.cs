using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tokket.Shared.Extensions.Http
{
    public static class HttpExtensions
    {
        public static HttpRequestHeaders RequestHeaders = new HttpRequestMessage().Headers;


        public static async Task<HttpResponseMessage> GetRequestAsync(
            this HttpClient httpClient, string url)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            foreach (var header in RequestHeaders)
                request.Headers.Add(header.Key, header.Value);
            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PostAsync(url, content);
        }
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
     this HttpClient httpClient, string url, T data, CancellationTokenSource tokenSource)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PostAsync(url, content, tokenSource.Token);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PutAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
          this HttpClient httpClient, string url, T data, CancellationTokenSource token)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PutAsync(url, content, token.Token);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, Uri url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
            this HttpClient httpClient, Uri url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var header in RequestHeaders)
                content.Headers.Add(header.Key, header.Value);

            return await httpClient.PutAsync(url, content);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }

        public static async Task<HttpResponseMessage> DeleteRequestAsync(
            this HttpClient httpClient, string url)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Delete,
            };
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            foreach (var header in RequestHeaders)
                request.Headers.Add(header.Key, header.Value);
            return await httpClient.SendAsync(request);
        }
    }
}
