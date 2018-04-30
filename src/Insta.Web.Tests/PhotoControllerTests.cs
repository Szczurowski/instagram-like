using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Insta.Processing;
using Insta.Web.Controllers;
using Insta.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using Xunit;
using Photo = Insta.Processing.Domain.Photo;

namespace Insta.Web.Tests
{
    
    public class PhotoControllerTests
    {
        private readonly PhotoController sut;
        private readonly Mock<IPhotoRepository> _mockRepository;
        private readonly Mock<IImageProcessor> _mockProcessor;

        public PhotoControllerTests()
        {
            _mockRepository = new Mock<IPhotoRepository>();
            _mockProcessor = new Mock<IImageProcessor>();

            this.sut = new PhotoController(_mockRepository.Object, _mockProcessor.Object);
        }

        [Fact]
        public async Task Get_WhenPhotoFoundInRepository_ShouldMapIt()
        {
            const int id = 21;

            _mockRepository.Setup(x => x.Get(id))
                .Returns(Task.FromResult(new Photo { Name = "foo" }));

            var result = await this.sut.Get(id);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBe(true);
            result.Content.ShouldNotBeNull();
            result.Content.Name.ShouldBe("foo");
        }

        [Fact]
        public async Task Get_RepositoryThrows_ShouldYieldFailedResult()
        {
            const int id = 21;

            _mockRepository.Setup(x => x.Get(id))
                .Throws(new ApplicationException("bar"));

            var result = await this.sut.Get(id);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBe(false);
            result.Message.ShouldBe("bar");
        }

        [Fact]
        public async Task Upload_MissingFilesName_ShouldYieldFailedResult()
        {
            this.sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            this.sut.ControllerContext.HttpContext.Request.Headers.Add("x-filename", new StringValues(string.Empty));

            var result = await this.sut.Upload();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBe(false);
            result.Message.ShouldBe("Missing filename");
        }

        [Fact]
        public async Task Upload_MissingContent_ShouldYieldFailedResult()
        {
            this.sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            this.sut.ControllerContext.HttpContext.Request.Headers.Add("x-filename", new StringValues("test.jpg"));
            this.sut.ControllerContext.HttpContext.Request.ContentLength = 0;

            var result = await this.sut.Upload();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBe(false);
            result.Message.ShouldBe("Missing file contents");
        }

        [Fact]
        public async Task Upload_WhenProperInput_ShouldYieldSuccess()
        {
            Result result = null;

            _mockProcessor.Setup(x => x.CreateThumbnail(It.IsAny<byte[]>()))
                .Returns(Task.FromResult(new byte[100]));
            _mockProcessor.Setup(x => x.ProcessPhoto(It.IsAny<byte[]>()))
                .Returns(Task.FromResult("foo"));

            using (var bodyStream = new MemoryStream(new byte[100]))
            {
                this.sut.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                };
                this.sut.ControllerContext.HttpContext.Request.Headers.Add("x-filename", new StringValues("test.jpg"));
                this.sut.ControllerContext.HttpContext.Request.ContentLength = 100;
                this.sut.ControllerContext.HttpContext.Request.Body = bodyStream;

                result = await this.sut.Upload();
            }

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBe(true);
        }
    }
}
