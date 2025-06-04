namespace Sirstrap.Core.Models.Next
{
    public class SirstrapConfiguration
    {
        public string CdnUrl { get; set; } = GlobalConstants.ROBLOX_CDN_URI;

        public bool FetchVersionFromSirHurt { get; set; }

        public bool MultipleInstances { get; set; }
    }
}