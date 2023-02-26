using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Helpers;
using Tokket.Shared.Models;
using Tokket.Shared.Models.SignalR;
using Tokket.Shared.Models.Tokquest;
using Tokket.Shared.Services.Interfaces;
using Xamarin.Essentials;

namespace Tokket.Shared.Services
{
    public class TokquestGameService : ITokQuestGameServices
    {
        private string baseUrl = "https://tokroomapidev.azurewebsites.net/api/";
        private string ApiPrefix = "/v1";
        private string ApiKey = "fdk0G3IZq9bBqdPaZU7WJuCBOy7uDIMcRzEm7ufWntz6tVHcm5JyKA==";
        private string CodePrefix = "?code=";
        public static TokquestGameService instance = new TokquestGameService();
        public static TokquestGameService Instance { get { return instance; } }
      
        private HttpClientHelper _httpClientHelper;
        public HubConnection Connections;


        public TokquestGameService()
        {

        }



        public async Task<bool> AddToClassGroupRoom(string getuserId, bool isteacher, TokQuestMultiplayer tokquestMultiplayer, TokquestPlayer tokquestPlayer)
        {
            if (isteacher)
            {
                var gamePlay = await TokquestService.Instance.GetGamesets(tokquestMultiplayer.pk);
                var game = gamePlay.Results.Where(x => x.Id == tokquestMultiplayer.gameId).FirstOrDefault();
                tokquestMultiplayer.gameLength = game.GameListObject.Count();

            }

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform", "web");
            _httpClient.BaseAddress = new Uri($"{Config.Configurations.UrlRoom}addtoclassgroup/{getuserId}/{tokquestMultiplayer.id}/{isteacher}{CodePrefix}{Config.Configurations.ApiKeyRoom}");
            var result = await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(JsonConvert.SerializeObject(tokquestPlayer)));

            return result.IsSuccessStatusCode;
        }


        public async Task<bool> SendMessage(string id, GameMessage gameMessage)
        {
           
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform", "web");
            _httpClient.BaseAddress = new Uri($"{Config.Configurations.UrlRoom}sendmessageroom/{id}{CodePrefix}{Config.Configurations.ApiKeyRoom}");
            var result = await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(JsonConvert.SerializeObject(gameMessage)));
            return result.IsSuccessStatusCode;
        }



        public async Task<bool> GameStart(string id, TokQuestMultiplayer tokquestMultiplayer)
        {
            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform", "web");
            _httpClient.BaseAddress = new Uri($"{Config.Configurations.UrlRoom}gamestarter/{id}{CodePrefix}{Config.Configurations.ApiKeyRoom}");
            HttpResponseMessage responseGet = await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(JsonConvert.SerializeObject(tokquestMultiplayer)));
            return responseGet.IsSuccessStatusCode;
         
        }

        public async Task<SignalRConnectionInfo> GetSignalRConnectionInfoGroup(string id, bool IsTokQuest)
        {

            var userid = Settings.GetUserModel().UserId;
            var idtoken = await SecureStorage.GetAsync("idtoken");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("userid", userid);
            _httpClient.DefaultRequestHeaders.Add("token", idtoken);
            _httpClient.DefaultRequestHeaders.Add("serviceid", "tokquest");
            _httpClient.DefaultRequestHeaders.Add("deviceplatform", "web");
            _httpClient.BaseAddress = new Uri($"{Config.Configurations.UrlRoom}connectroom/{id}{CodePrefix}{Config.Configurations.ApiKeyRoom}");
            HttpResponseMessage responseGet = await _httpClient.PostAsync(_httpClient.BaseAddress,null);
            var res = JsonConvert.DeserializeObject<SignalRConnectionInfo>(await responseGet.Content.ReadAsStringAsync());

            return res;
            //throw new NotImplementedException();


        }
    }
}
