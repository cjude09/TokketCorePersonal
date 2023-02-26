using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Shared.Helpers.Interfaces
{
    public interface IHttpClientHelper
    {
        string Delete(string api);

        Task<string> DeleteAsync(string api);   

        string Post(string api, object message);

        Task<string> PostAsync(string api, object message);

        string Get(string api);

        string GetData(string api);

        Task<string> GetAsync(string api);

        string Put(string api, object message);

        Task<string> PutAsync(string api, object message);
    }
}
