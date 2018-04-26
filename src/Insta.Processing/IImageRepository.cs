using Insta.Processing.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insta.Processing
{
    public interface IImageRepository
    {
        Task<IEnumerable<Photo>> GetAll();

        Task<Photo> Get(int id);

        Task Add(Photo photo);
    }
}
