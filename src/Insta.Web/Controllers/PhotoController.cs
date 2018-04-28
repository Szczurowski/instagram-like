using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Insta.Processing;
using Insta.Web.Models;
using Domain = Insta.Processing.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Insta.Web.Controllers
{
    [Route("api/[controller]")]
    public class PhotoController : Controller
    {
        private readonly IPhotoRepository _repository;

        public PhotoController(IPhotoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<Result<PhotoDetailed>> Get(int id)
        {
            var photo = await _repository.Get(id);

            return Result<PhotoDetailed>.Success(MapToDetailed(photo));
        }

        [HttpGet("original/{Id}")]
        public async Task<IActionResult> GetOriginal(int id)
        {
            var content = await _repository.GetGetOriginal(id);

            // TODO: add ContentType to Entity
            return File(content, "image/jpeg");
        }

        [HttpGet("thumbnail/{Id}")]
        public async Task<IActionResult> GetThumbnail(int id)
        {
            var content = await _repository.GetGetThumbnail(id);

            // TODO: add ContentType to Entity
            return File(content, "image/jpeg");
        }

        [HttpGet]
        public async Task<Result<IEnumerable<Photo>>> GetAll()
        {
            var photos = await _repository.GetAll();

            return Result<IEnumerable<Photo>>.Success(photos.Select(Map));
        }

        [HttpPost]
        public async Task<Result> Upload()
        {
            var originalContent = new byte[Request.ContentLength.GetValueOrDefault()];
            using (var memory = new MemoryStream(originalContent))
            {
                await Request.Body.CopyToAsync(memory);
            }

            var filename = Request.Headers["x-filename"];

            await _repository.Add(new Domain.Photo
            {
                Name = filename,
                OriginalContent = originalContent,
                ThumbnailContent = null,
                VisionAnalysis = "{}"
            });

            return Result.Success();
        }

        // TODO: move to mapper
        private Photo Map(Domain.Photo photo) => photo == null
            ? null
            : new Photo
            {
                Id = photo.Id,
                Name = photo.Name,
                ThumbnailLocation = $"/api/photo/thumbnail/{photo.Id}"
            };

        // TODO: move to mapper
        private PhotoDetailed MapToDetailed(Domain.Photo photo) => photo == null
            ? null 
            : new PhotoDetailed
            {
                Id = photo.Id,
                Name = photo.Name,
                OriginalLocation = $"/api/photo/original/{photo.Id}",
                ProcessingAnalysisResult = Convert(photo.VisionAnalysis)
            };

        // TODO: implement JSON->Object converter
        private ProcessingAnalysisResult Convert(string raw) =>
            new ProcessingAnalysisResult();
    }
}
