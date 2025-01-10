namespace SocinatorInstaller9.Models
{
    public class SocialDominatorModel
    {
        public string Version { get; set; }
        public string ConfigMode { get; set; }
        public string PublicKey { get; set; }
        public string StoragePath { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public string ProductName { get; set; } = "SocialDominator";
    }
}
