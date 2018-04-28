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

        public async Task<string> ProcessPhoto(byte[] photoBytes)
        {
            var uriBase = _webConfiguration.VisionApiUriBase;
            var subscriptionKey = _webConfiguration.VisionApiSubscriptionKey;

            using (var client = new HttpClient())
            {
                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. A third optional parameter is "details".
                string requestParameters = "visualFeatures=Description,Faces&language=en";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                using (ByteArrayContent content = new ByteArrayContent(photoBytes))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json" and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public Task<byte[]> CreateThumbnail(byte[] photoBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}