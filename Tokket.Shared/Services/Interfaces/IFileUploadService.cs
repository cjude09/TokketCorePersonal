using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tokket.Shared.Models;
using Xamarin.Essentials;

namespace Tokket.Shared.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileResult> FindPDF(PickOptions options);
        Task<FileUploadResult> UploadFileAsync(FileModel file, CancellationToken cancellationToken);

        void FileManager_ProgressChanged(string fileId, long value, long size, ProgressChangedEventArgs e, FileModel fileReference = null);

        Task<List<FileResult>> SelectFiles(PickOptions options);

        Task<FileResult> SelectFile(PickOptions options);
        Task<FileResult> SelectPicture(MediaPickerOptions options);
    }
}
