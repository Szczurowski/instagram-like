using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Insta.Web.Models;
using Microsoft.AspNetCore.Http;
using Domain = Insta.Processing.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Insta.Web.Controllers
{
    [Route("api/[controller]")]
    public class PhotoController : Controller
    {
        private static readonly Domain.Photo[] Photos = {
            new Domain.Photo(),
            new Domain.Photo()
        };

        [HttpGet("{id}")]
        public Result<PhotoDetailed> Get(int id)
        {
            var photo = Photos.First();
            photo.Id = id;

            return Result<PhotoDetailed>.Success(MapToDetailed(photo));
        }

        [HttpGet]
        public Result<IEnumerable<Photo>> GetAll()
        {
            return Result<IEnumerable<Photo>>.Success(Photos.Select(Map));
        }

        [HttpPost]
        public async Task<Result> Upload()
        {
            var content = new byte[Request.ContentLength.GetValueOrDefault()];
            using (var memory = new MemoryStream(content))
            {
                await Request.Body.CopyToAsync(memory);
            }

            return Result.Success();
        }

        // TODO: Implement
        [HttpGet("preview/{Id}")]
        public Result<byte[]> Download(int id)
        {
            return Result<byte[]>.Failure("Not implemented yet");
        }

        private Photo Map(Domain.Photo photo) =>
            new Photo
            {
                Id = photo.Id,
                Name = photo.Name,
                ThumbnailLocation = photo.ThumbnailLocation
            };

        private PhotoDetailed MapToDetailed(Domain.Photo photo) =>
            new PhotoDetailed
            {
                Id = photo.Id,
                Name = photo.Name,
                OriginalLocation = photo.OriginalLocation,
                ProcessingAnalysisResult = Convert(photo.VisionAnalysis)
            };

        // TODO: implement JSON->Object converter
        private ProcessingAnalysisResult Convert(string raw) =>
            new ProcessingAnalysisResult();
    }
}
