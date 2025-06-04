namespace Sirstrap.Core.Models.Next
{
    public interface ISrddConfiguration
    {
        Task SetFromArgumentsAsync(string[] arguments, CancellationToken cancellationToken);
    }
}