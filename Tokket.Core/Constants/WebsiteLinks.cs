

namespace Tokket.Core
{
    public static class WebsiteLinks
    {
        public const string Tokket = "https://tokket.com";

        public const string TokketPre = "http://tokket-pre.azurewebsites.net/";

        public const string TokketDev = "http://tokket-dev.azurewebsites.net/";

        public const string TokBlitz = "https://tokblitz.com";

        public static string[] Websites { get; set; } = new string[]
        {
        Tokket,
        TokketPre,
        TokketDev,
        TokBlitz
        };
    }
}
