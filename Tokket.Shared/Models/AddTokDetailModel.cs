using Tokket.Shared.Models.Base;

namespace Tokket.Shared.Models
{
    public class AddTokDetailModel : BaseModel
    {
        private string _detail;
        private string _englishdetail;
        public string Detail { get => _detail; set { _detail = value; RaisePropertyChanged(propertyName: nameof(Detail)); } }
        public string EnglishDetail { get => _englishdetail; set { _englishdetail = value; RaisePropertyChanged(propertyName: nameof(EnglishDetail)); } }
        public bool ChkAnswer { get; set; }
    }
}
