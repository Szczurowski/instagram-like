using Insta.Processing.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insta.Processing
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetAll();

        Task<Photo> Get(int id);

        Task<byte[]> GetGetOriginal(int id);

        Task<byte[]> GetGetThumbnail(int id);

        Task Add(Photo photo);
    }
}
