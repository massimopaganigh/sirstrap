namespace Sirstrap.Core.Models.Next
{
    public class Manifest
    {
        public bool Valid { get; set; } = true;

        public List<string> Packages { get; set; } = [];
    }
}