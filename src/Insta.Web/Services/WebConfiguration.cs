using Insta.Common;
using Microsoft.Extensions.Configuration;

namespace Insta.Web.Services
{
    public class WebConfiguration : IWebConfiguration
    {
        private readonly IConfiguration _aspnetConfiguration;

        public WebConfiguration(IConfiguration aspnetConfiguration)
        {
            _aspnetConfiguration = aspnetConfiguration;
        }

        public string ConfigurationString => _aspnetConfiguration.GetConnectionString("Default");
    }
}
