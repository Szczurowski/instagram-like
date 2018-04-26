using System.Collections.Generic;
using System.Linq;
using Insta.Web.Models;
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
        public PhotoDetailed Get(int id)
        {
            return MapToDetailed(Photos.First());
        }

        [HttpGet]
        public IEnumerable<Photo> WeatherForecasts()
        {
            return Photos.Select(Map);
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

        private ProcessingAnalysisResult Convert(string raw) =>
            new ProcessingAnalysisResult();
    }
}
