using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tokket.Core
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync();
    }
}
