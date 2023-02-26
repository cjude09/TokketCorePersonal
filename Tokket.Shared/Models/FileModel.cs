using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Tokket.Shared.Models
{
    public class FileModel : INotifyPropertyChanged
    {
        private bool _isUploading;
        private decimal _progressValue = 0.0M;

        public string FileId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public string FullPath { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string Base64 { get; set; }
        public string FileUrl { get; set; }
        public bool IsUploading { get => _isUploading; set { SetProperty(ref _isUploading, value); } }
        public decimal ProgressValue { get => _progressValue; set { SetProperty(ref _progressValue, value); } }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }

    public class FileUploadResult
    {
        public string FileLink { get; set; }
        public string Id { get; set; }
    }

    public class FileUploadCancellationTokens
    {
        public List<string> fileIds { get; set; } = new List<string>();
        public List<CancellationTokenSource> SourceToken { get; set; } = new List<CancellationTokenSource>();
    }

    public class FileCommentModel    
    { 
           public string Uri { get; set; }
            public string Name { get; set; }
        public string Extension { get; set; }
    }
}
