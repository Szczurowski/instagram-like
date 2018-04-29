using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Insta.Common;

namespace Insta.Processing
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly IWebConfiguration _webConfiguration;

        public ImageProcessor(IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }

        private string uriBase => _webConfiguration.VisionApiUriBase;
        private string subscriptionKey => _webConfiguration.VisionApiSubscriptionKey;

        public async Task<string> ProcessPhoto(byte[] photoBytes) =>
            await GetCloudContent(
                photoBytes, "analyze", "visualFeatures=Description,Faces&language=en", x => x.ReadAsStringAsync());

        // TODO: astract away dimensions into condiguration
        public async Task<byte[]> CreateThumbnail(byte[] photoBytes) => 
            await GetCloudContent(
                photoBytes, "generateThumbnail", "width=200&height=150&smartCropping=true", x => x.ReadAsByteArrayAsync());

        private async Task<TResult> GetCloudContent<TResult>(
            byte[] photoBytes,
            string apiName,
            string requestParameters,
            Func<HttpContent, Task<TResult>> resultFactory)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                string uri = $"{uriBase}/{apiName}?{requestParameters}";

                using (ByteArrayContent content = new ByteArrayContent(photoBytes))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the resulting data.
                        var result = await resultFactory(response.Content);
                        return result;
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    throw new ApplicationException(error);
                }
            }
        }
    }
}