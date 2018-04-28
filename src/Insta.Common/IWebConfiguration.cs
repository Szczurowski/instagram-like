namespace Insta.Common
{
    public interface IWebConfiguration
    {
        string ConfigurationString { get; }

        string VisionApiSubscriptionKey { get; }

        string VisionApiUriBase { get; }
    }
}
