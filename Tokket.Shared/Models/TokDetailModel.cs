using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models.Base;

namespace Tokket.Shared.Models
{
    public class TokDetailModel : BaseModel
    {
        private string _title;
        private string _detaildesc;
        private string _detailtrans;
        private string _image;
        public string Id
        {
            get;
            set;
        }
        public bool IsAnswer
        {
            get;
            set;
        }

        public string Title { get => _title; set { _title = value; RaisePropertyChanged(propertyName: nameof(Title)); } }
        public string DetailDesc { get => _detaildesc; set { _detaildesc = value; RaisePropertyChanged(propertyName: nameof(DetailDesc)); } }
        public string DetailTrans { get => _detailtrans; set { _detailtrans = value; RaisePropertyChanged(propertyName: nameof(DetailTrans)); } }
        public string Image { get => _image; set { _image = value; RaisePropertyChanged(propertyName: nameof(Image)); } }
    }
}
