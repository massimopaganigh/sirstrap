namespace Sirstrap.Core.Models.Next
{
    public class SirstrapConfiguration
    {
        public string CdnUrl { get; set; } = "https://setup.rbxcdn.com";

        public bool FetchVersionFromSirHurt { get; set; }

        public bool MultipleInstances { get; set; }
    }
}