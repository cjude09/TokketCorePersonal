using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Tokket.Shared.Services.Interfaces;
using System.Threading.Tasks;
using System.ComponentModel;
using Tokket.Shared.Models;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Core.Util;
using System.IO;
using System.Linq;

namespace Tokket.Shared.Services
{
    public class FileUploadService : IFileUploadService
    {
        private static FileUploadService instance = new FileUploadService();

        public static FileUploadService Instance { get { return instance; } }
        readonly CloudStorageAccount StorageAccount;
        readonly CloudBlobClient BlobClient;
        readonly CloudBlobContainer Container;
        readonly CloudBlobContainer FileContainer;

        public FileUploadService() {
#if DEBUG
            StorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tokketcontent;AccountKey=efDftHZPuN8RNkRjGKPZgv1PJNvjsdFj/s6knamdVEIDIBZtjWVfY3t42cypi4x1Uk7pk9eanNz3lZq0quPvMQ==;EndpointSuffix=core.windows.net");
#elif RELEASE
             StorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tokketcontent;AccountKey=efDftHZPuN8RNkRjGKPZgv1PJNvjsdFj/s6knamdVEIDIBZtjWVfY3t42cypi4x1Uk7pk9eanNz3lZq0quPvMQ==;EndpointSuffix=core.windows.net");
#endif
            BlobClient = StorageAccount.CreateCloudBlobClient();
            Container = BlobClient.GetContainerReference("images");
            FileContainer = BlobClient.GetContainerReference("files");
        }
        
        public void FileManager_ProgressChanged(string fileId, long value, long size, ProgressChangedEventArgs e, FileModel fileReference = null)
        {
            
            Console.WriteLine("Upload: " + value + " at " + (int)(((double)value / size) * 100));
            Console.WriteLine("Percentage: "+e.ProgressPercentage);
            if(fileReference != null)
            {
                fileReference.ProgressValue = (int)(((double)value / size) * 100);
            }

        }

        public async Task<FileResult> FindPDF(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);

                if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("doc", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("docx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("xls", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("ppt", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("pptx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("gif", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    return result;
                else
                    return null;
              

              
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<FileResult> FindPDFWithResult(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);

                if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("doc", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("docx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("xls", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("ppt", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("pptx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("gif", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    return result;
                else
                    return new FileResult("Error", result.ContentType);



            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<List<FileResult>> SelectFiles(PickOptions options)
        {
            try
            {

                var result = await FilePicker.PickMultipleAsync(options);

                return result.ToList();


            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<List<FileResult>> SelectFilesWithValidExtension(PickOptions options)
        {
            try
            {
                var fileList = new List<FileResult>();
                var result = await FilePicker.PickMultipleAsync(options);
                foreach (var item in result)
                {
                    if (item.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("doc", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("docx", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("xls", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("ppt", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("pptx", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("gif", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                        fileList.Add(item);
                    else
                        fileList.Add(new FileResult("Error", item.ContentType));
                }
                return fileList;


            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<FileResult> SelectPicture(MediaPickerOptions options)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(options);

                return result;


            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<FileResult> SelectFile(PickOptions options)
        {
            try
            {

                var result = await FilePicker.PickAsync(options);

                return result;


            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<FileUploadResult> UploadFileAsync(FileModel file, CancellationToken cancellationToken)
        {
            FileUploadResult result = new FileUploadResult();
            result.Id = file.FileName.Replace("\"", "").Replace("?", "").Replace(":", "").Replace("&", "_").Replace("\\", "_").Replace("/", "_").Replace(" ", "").Replace(".", "").ToLower();
            if (string.IsNullOrEmpty(file.Base64)) return result;

            // Split the blob format if needed
            if (file.Base64.Contains(","))
            {
                string[] splitBlob = file.Base64.Split(',');
                file.Base64 = splitBlob[1];
            }

            byte[] imageBytes = Convert.FromBase64String(file.Base64);

            IProgress<StorageProgress> progressHandler = new Progress<StorageProgress>(
             progress => FileManager_ProgressChanged(result.Id, progress.BytesTransferred, file.Size, new ProgressChangedEventArgs(0, null), file)
            );

            CloudBlockBlob blockBlob = FileContainer.GetBlockBlobReference("file-" + file.FileId + file.Extension);
            blockBlob.Properties.ContentType = file.FileType;

            await blockBlob.UploadFromByteArrayAsync(
                  imageBytes, 0, imageBytes.Length,
                  default(AccessCondition),
                  default(BlobRequestOptions),
                  default(OperationContext),
                  progressHandler,
                  cancellationToken
                  );


            result.FileLink = blockBlob.Uri.ToString();
            return result;
        }

       
    }

    public class FileService {
        public async static Task  OpenFileWebBrowser(string uri) {
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        public static async Task<string> ConverFileToBase64(string path) {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader r = new StreamReader(fs);
            var file = r.BaseStream;
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return Convert.ToBase64String(bytes);
        }

        public static int  getFileUploadPercentage(int percent) {

            return percent;
        }
    }
}
