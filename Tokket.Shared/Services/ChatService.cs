using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Tokket.Shared.Models.Chat;
using Tokket.Shared.Models.SignalR;
using Tokket.Shared.Services.Interfaces;
using Xamarin.Essentials;
namespace Tokket.Shared.Services
{
    public class ChatService : IChatService
    {
        private static ChatService instance = new ChatService();
        public static ChatService Instance { get { return instance; } }
        public HubConnection Connections;
        //    "BaseUrl": "https://tokroomapidev.azurewebsites.net/api/",
        //"ApiPrefix": "/v1",
        //"ApiKey": "fdk0G3IZq9bBqdPaZU7WJuCBOy7uDIMcRzEm7ufWntz6tVHcm5JyKA==",
        //"CodePrefix": "?code="
        //private string baseUrl = "https://tokroomapidev.azurewebsites.net/api/";
        //private string ApiPrefix = "v1/";
        //private string ApiKey = "fdk0G3IZq9bBqdPaZU7WJuCBOy7uDIMcRzEm7ufWntz6tVHcm5JyKA==";
        //private string CodePrefix = "?code=";
       

        public async Task<bool> AddToClassGroupRoomChat(string id, string classgroupid)
        {
            bool success = false;
            try
            {

                if (Connections.State == HubConnectionState.Connected)
                {
                    MainThread.BeginInvokeOnMainThread(async () => {

                        var _httpClient = new HttpClient();
                        string url = $"{Config.Configurations.UrlRoom}addtoclassgroupchat/{id}/{classgroupid}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKeyRoom}";
                        var item = await _httpClient.PostAsync(url, null);
                        success = item.IsSuccessStatusCode;

                    });
                 
                }


                

                //Task.WhenAll(Task.Factory.StartNew(() =>
                //{ var send = true;
                //    while (send)
                //    {
                //        if (Connections.State == HubConnectionState.Connected) {
                //            MainThread.BeginInvokeOnMainThread(async () => {

                //                var _httpClient = new HttpClient();
                //                string url = $"{baseUrl}addtoclassgroupchat/{id}/{classgroupid}{CodePrefix}{ApiKey}";
                //                var item = await _httpClient.PostAsync(url, null);
                //                success = item.IsSuccessStatusCode;

                //            });
                //            break;

                //        }

                //    };

                //}));






            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        public async Task<SignalRConnectionInfo> GetSignalRConnectionInfoGroupChat(string id)
        {
            SignalRConnectionInfo res = null;
            try
            {
                var _httpClient = new HttpClient();
                var result = await _httpClient.PostAsync($"{Config.Configurations.UrlRoom}connectroom/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKeyRoom}", null);
                res = JsonConvert.DeserializeObject<SignalRConnectionInfo>(await result.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
            }
            return res;
        }
        public async Task<bool> SendMessageTokChat(string classgroupid, TokChatMessage item)
        {
            bool success = false;
            try
            {
                var _httpClient = new HttpClient();
                var items = JsonConvert.SerializeObject(item);
                var result = await _httpClient.PostAsync($"{Config.Configurations.UrlRoom}sendmessagechat/{classgroupid}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKeyRoom}", new StringContent(JsonConvert.SerializeObject(item)));
                if (result.IsSuccessStatusCode == false)
                    success = false;
                else
                    success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        //Use UserLocalId
        public async Task InitChatHub(string id)
        {
            var connectionInfo = await GetSignalRConnectionInfoGroupChat(id);

            Task.Run(() =>
            {
                try
                {
                    if (Connections != null)
                    {
                        Connections.StopAsync();
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("cannot stop the music");
                }

               


            });

            Connections = new HubConnectionBuilder()
                    .WithUrl(connectionInfo.Url, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
                    }).Build();

            try
            {
                await Connections.StartAsync();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                await Connections.StopAsync();
            }
        

          






        }



        public void ChatOnFirstLoad(Action<TokChat> action)
        {
            //Connections.On<TokChat>("TokChatFirstLoad", model=> {
            //    Debug.WriteLine(model.ToString());
            
            //}) ;

        }
        public void ChatOnRecieve(Action<TokChat> action)
        {
            Connections.On<TokChat>("tokchat", action);
        }
        public async Task InitChatHub(string id, Action<TokChat> action1, Action<TokChat> action2)
        {
            var connectionInfo = await GetSignalRConnectionInfoGroupChat(id);
            if (Connections != null)
            {
                await Connections.StopAsync().ContinueWith(async a => {
                    await Connections.DisposeAsync();
                });
            }
            Connections = new HubConnectionBuilder()
                     .WithUrl(connectionInfo.Url, options =>
                     {
                         options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
                     }).Build();
            Connections.StartAsync();

            //Connections.On<TokChat>("TokChatFirstLoad", model =>
            //{
            //    Debug.WriteLine(model.ToString());
            //    //action1.Invoke(model);
            //});
            Connections.On<TokChat>("tokchat", model =>
            {
                action2.Invoke(model);
            });

        }
        public async Task InitChatHub(string id, ObservableCollection<TokModel> toks, List<string> commentlist)
        {
            var connectionInfo = await GetSignalRConnectionInfoGroupChat(id);
            if (Connections != null)
            {
                await Connections.StopAsync().ContinueWith(async a => {
                    await Connections.DisposeAsync();
                });
            }
            Connections = new HubConnectionBuilder()
                     .WithUrl(connectionInfo.Url, options =>
                     {
                         options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
                     }).Build();
            Connections.StartAsync();
        }

        public async Task DeleteMessage(string id, TokChatMessage message)
        {
            var _httpClient = new HttpClient();
            message.pk = id + "-tokchats0";
            
            var content = JsonConvert.SerializeObject(message);
            var result = await _httpClient.PostAsync($"{Config.Configurations.UrlRoom}deletemessagechat/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKeyRoom}", new StringContent(content)) ;

            var getres = result;


        }

        public async Task UpdateMessage(string id, TokChatMessage message)
        {
            var _httpClient = new HttpClient();
            message.pk = id + "-tokchats0";
            var content = JsonConvert.SerializeObject(message);
            var result = await _httpClient.PostAsync($"{Config.Configurations.UrlRoom}updatemessagechat/{id}{Config.Configurations.CodePrefix}{Config.Configurations.ApiKeyRoom}", new StringContent(content));

            var gg = result;
        }
    }
}