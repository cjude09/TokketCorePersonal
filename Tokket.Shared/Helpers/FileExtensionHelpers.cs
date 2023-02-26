using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Helpers
{
    public class FileExtensionHelpers
    {
        private static List<string> ImageExtensionList { get { return new List<string>() { 
            "jpg", "jpeg" , "jpe", "jif", "jfif", "jfi",
            "png", "gif", "webp", "tiff", "tif", "psd",
            "raw", "arw", "cr2", "nrw","k25","bmp", "dib",
            "heif", "heic", "ind", "indd", "indt", "jp2",
            "j2k", "jpf", "jpx", "jpm", "mj2", "svg", "svgz",
            "ai","eps",
        }; } }

        public static bool IsImageUri(string uri) {
            bool AnImage = false;
            if (!string.IsNullOrEmpty(uri))
            {
                foreach (var ext in ImageExtensionList)
                {
                    var getextensionuri = uri.Substring(uri.Length - ext.Length, ext.Length);
                    if (getextensionuri.ToLower() == ext)
                    {
                        AnImage = true;
                        break;
                    }
                }
            }

            return AnImage;
        } 
    }
}
