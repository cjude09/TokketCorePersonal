using System;
using System.Collections.Generic;
using System.Text;
using Tokket.Shared.Models;

namespace Tokket.Shared.ViewModels
{
    public class ClassSetViewModel
    {
        public ClassSetModel ClassSet { get; set; }
        public List<ClassTokModel> ClassToks { get; set; }
        public string Token { get; set; }
        public bool IsSignedIn { get; set; }
    }
}
