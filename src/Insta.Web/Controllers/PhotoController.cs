using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Insta.Processing;
using Insta.Web.Models;
using Domain = Insta.Processing.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Insta.Web.Controllers
{
    [Route("api/[controller]")]
    public class PhotoController : Controller
    {
        private readonly IPhotoRepository _repository;
        private readonly IImageProcessor _imageProcessor;

        public PhotoController(
            IPhotoRepository repository,
            IImageProcessor imageProcessor)
        {
            _imageProcessor = imageProcessor;
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<Result<PhotoDetailed>> Get(int id)
        {
            return await Shield(async () =>
            {
                var photo = await _repository.Get(id);

                var photoMapped = MapToDetailed(photo);

                return photoMapped;
            });
        }

        [HttpGet("{Id}/original")]
        public async Task<IActionResult> GetOriginal(int id) => await GetBinary(id, _repository.GetGetOriginal);

        [HttpGet("{Id}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(int id) => await GetBinary(id, _repository.GetGetThumbnail);

        [HttpGet]
        public async Task<Result<IEnumerable<Photo>>> GetAll()
        {
            return await Shield(async () =>
            {
                var photos = await _repository.GetAll();

                var photosMapped = photos.Select(Map);

                return photosMapped;
            });
        }

        [HttpPost]
        public async Task<Result> Upload()
        {
            var filename = Request.Headers["x-filename"];
            if (string.IsNullOrEmpty(filename))
            {
                return Result.Failure("Missing filename");
            }

            if (Request.ContentLength == 0)
            {
                return Result.Failure("Missing file contents");
            }

            return await Shield(async () =>
            {
                var originalContent = new byte[Request.ContentLength.GetValueOrDefault()];
                using (var memory = new MemoryStream(originalContent))
                {
                    await Request.Body.CopyToAsync(memory);
                }

                var visionAnalysisTask = _imageProcessor.ProcessPhoto(originalContent);
                var thumbnailContentTask = _imageProcessor.CreateThumbnail(originalContent);

                // run parallel independent APIs
                await Task.WhenAll(visionAnalysisTask, thumbnailContentTask);

                await _repository.Add(new Domain.Photo
                {
                    Name = filename,
                    OriginalContent = originalContent,
                    ThumbnailContent = thumbnailContentTask.Result,
                    VisionAnalysis = visionAnalysisTask.Result
                });
            });
        }

        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            return await Shield(async () =>
            {
                await _repository.Delete(id);
            });
        }

        public async Task<IActionResult> GetBinary(int id, Func<int, Task<byte[]>> repositoryAccessor)
        {
            var content = await repositoryAccessor(id);

            if (content == null)
            {
                return NotFound();
            }

            // TODO: add ContentType to Entity
            return File(content, "image/jpeg");
        }

        /// <summary>
        /// Handles exceptions.
        /// </summary>
        /// <remarks>
        /// Normally would be a part of BaseController but here we have only one
        /// meaningfull Controller so let it be here.
        /// </remarks>
        private async Task<Result<TResult>> Shield<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                var result = await action.Invoke();

                return Result<TResult>.Success(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return Result<TResult>.Failure(e.Message);
            }
        }

        /// <summary>
        /// Handles exceptions.
        /// </summary>
        /// <remarks>
        /// Normally would be a part of BaseController but here we have only one
        /// meaningfull Controller so let it be here.
        /// </remarks>
        private async Task<Result> Shield(Func<Task> action)
        {
            try
            {
                await action.Invoke();

                return Result.Success();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return Result.Failure(e.Message);
            }
        }

        // TODO: move to mapper
        private Photo Map(Domain.Photo photo) => photo == null
            ? null
            : new Photo
            {
                Id = photo.Id,
                Name = photo.Name,
                ThumbnailLocation = $"/api/photo/{photo.Id}/thumbnail"
            };

        // TODO: move to mapper
        private PhotoDetailed MapToDetailed(Domain.Photo photo) => photo == null
            ? null 
            : new PhotoDetailed
            {
                Id = photo.Id,
                Name = photo.Name,
                OriginalLocation = $"/api/photo/{photo.Id}/original",
                ProcessingAnalysisResult = Convert(photo.VisionAnalysis)
            };

        private ProcessingAnalysisResult Convert(string raw) => string.IsNullOrEmpty(raw)
            ? null
            : JsonConvert.DeserializeObject<ProcessingAnalysisResult>(raw);
    }
}
