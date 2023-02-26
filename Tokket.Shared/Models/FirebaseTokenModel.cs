using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class AuthorizationTokenModel
    {
        [JsonProperty(PropertyName = "id_token")]
        public string IdToken { get; set; }
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
        public string StreamToken { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        public string UserPhoto { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty(PropertyName = "localId")]
        public string LocalId { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "project_id")]
        public string ProjectId { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "hash_key")]
        public string HashKey { get; set; }
        [JsonProperty(PropertyName = "token_expiry")]
        public long TokenExpiration { get; set; }
    }
}
