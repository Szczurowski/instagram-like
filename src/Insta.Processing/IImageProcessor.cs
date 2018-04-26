using System.Threading.Tasks;

namespace Insta.Processing
{
    public interface IImageProcessor
    {
        Task<string> ProcessPhoto(byte[] photoBytes);

        Task<byte[]> CreateThumbnail(byte[] photoBytes);
    }
}
