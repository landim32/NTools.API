using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BazzucaMedia.API.Controllers;
using NTools.Domain.Services.Interfaces;
using System;
using System.IO;
using System.Text;

namespace NTools.Tests.API.Controllers
{
    public class FileControllerTests
    {
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<ILogger<FileController>> _mockLogger;
        private readonly FileController _controller;

        public FileControllerTests()
        {
            _mockFileService = new Mock<IFileService>();
            _mockLogger = new Mock<ILogger<FileController>>();
            _controller = new FileController(_mockFileService.Object, _mockLogger.Object);
        }

        #region GetFileUrl - Success Tests

        [Fact]
        public void GetFileUrl_WithValidParameters_ReturnsOkWithUrl()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var expectedUrl = "https://example.com/test-bucket/test-file.jpg";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns(expectedUrl);

            // Act
            var result = _controller.GetFileUrl(bucketName, fileName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedUrl, okResult.Value);
        }

        [Theory]
        [InlineData("bucket1", "file1.pdf", "https://s3.com/bucket1/file1.pdf")]
        [InlineData("images", "photo.png", "https://s3.com/images/photo.png")]
        [InlineData("documents", "report.docx", "https://s3.com/documents/report.docx")]
        public void GetFileUrl_WithVariousInputs_ReturnsExpectedUrls(string bucket, string file, string expectedUrl)
        {
            // Arrange
            _mockFileService.Setup(x => x.GetFileUrl(bucket, file))
                .Returns(expectedUrl);

            // Act
            var result = _controller.GetFileUrl(bucket, file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedUrl, okResult.Value);
        }

        [Fact]
        public void GetFileUrl_WithValidInput_CallsServiceOnce()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns("https://example.com/test-bucket/test-file.jpg");

            // Act
            _controller.GetFileUrl(bucketName, fileName);

            // Assert
            _mockFileService.Verify(x => x.GetFileUrl(bucketName, fileName), Times.Once);
        }

        [Fact]
        public void GetFileUrl_WithSpecialCharactersInFileName_ReturnsOk()
        {
            // Arrange
            var bucketName = "bucket";
            var fileName = "folder/subfolder/file with spaces.pdf";
            var expectedUrl = "https://example.com/bucket/folder/subfolder/file with spaces.pdf";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns(expectedUrl);

            // Act
            var result = _controller.GetFileUrl(bucketName, fileName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedUrl, okResult.Value);
        }

        [Fact]
        public void GetFileUrl_WithEmptyFileName_ReturnsOk()
        {
            // Arrange
            var bucketName = "bucket";
            var fileName = string.Empty;
            var expectedUrl = string.Empty;
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns(expectedUrl);

            // Act
            var result = _controller.GetFileUrl(bucketName, fileName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedUrl, okResult.Value);
        }

        #endregion

        #region GetFileUrl - Error Tests

        [Fact]
        public void GetFileUrl_WhenServiceThrowsException_Returns500()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var exceptionMessage = "Service error occurred";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.GetFileUrl(bucketName, fileName);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(exceptionMessage, statusCodeResult.Value);
        }

        [Fact]
        public void GetFileUrl_WhenServiceThrowsException_LogsError()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Throws(new Exception("Service error"));

            // Act
            _controller.GetFileUrl(bucketName, fileName);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error occurred while getting file URL")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region GetFileUrl - Logging Tests

        [Fact]
        public void GetFileUrl_WithValidInput_LogsInformationOnEntry()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns("https://example.com/test-bucket/test-file.jpg");

            // Act
            _controller.GetFileUrl(bucketName, fileName);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Get File Url")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetFileUrl_WithValidInput_LogsInformationOnSuccess()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var expectedUrl = "https://example.com/test-bucket/test-file.jpg";
            _mockFileService.Setup(x => x.GetFileUrl(bucketName, fileName))
                .Returns(expectedUrl);

            // Act
            _controller.GetFileUrl(bucketName, fileName);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Returned URL")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region UploadFile - Success Tests

        [Fact]
        public void UploadFile_WithValidFile_ReturnsOkWithFileName()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var fileContent = "Test file content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var mockFile = CreateMockFormFile(fileName, stream);
            var expectedFileName = "uploaded-file.jpg";

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(expectedFileName);

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedFileName, okResult.Value);
        }

        [Theory]
        [InlineData("document.pdf", "uploaded-document.pdf")]
        [InlineData("image.png", "uploaded-image.png")]
        [InlineData("report.xlsx", "uploaded-report.xlsx")]
        public void UploadFile_WithVariousFileTypes_ReturnsExpectedFileName(string originalName, string uploadedName)
        {
            // Arrange
            var bucketName = "test-bucket";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(originalName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, originalName))
                .Returns(uploadedName);

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(uploadedName, okResult.Value);
        }

        [Fact]
        public void UploadFile_WithValidFile_CallsServiceOnce()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            _mockFileService.Verify(
                x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName),
                Times.Once);
        }

        [Fact]
        public void UploadFile_WithLargeFile_ReturnsOk()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "large-file.bin";
            var largeContent = new byte[10_000_000]; // 10 MB
            var stream = new MemoryStream(largeContent);
            var mockFile = CreateMockFormFile(fileName, stream, largeContent.Length);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region UploadFile - Validation Tests

        [Fact]
        public void UploadFile_WithNullFile_ReturnsBadRequest()
        {
            // Arrange
            var bucketName = "test-bucket";
            IFormFile nullFile = null!;

            // Act
            var result = _controller.UploadFile(bucketName, nullFile);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No file uploaded", badRequestResult.Value);
        }

        [Fact]
        public void UploadFile_WithEmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);
            mockFile.Setup(f => f.FileName).Returns("empty-file.txt");

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No file uploaded", badRequestResult.Value);
        }

        [Fact]
        public void UploadFile_WithNullFile_LogsError()
        {
            // Arrange
            var bucketName = "test-bucket";
            IFormFile nullFile = null!;

            // Act
            _controller.UploadFile(bucketName, nullFile);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("No file uploaded")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void UploadFile_WithEmptyFile_LogsError()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);

            // Act
            _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("No file uploaded")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region UploadFile - Error Tests

        [Fact]
        public void UploadFile_WhenServiceThrowsException_Returns500()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var exceptionMessage = "Upload failed";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(exceptionMessage, statusCodeResult.Value);
        }

        [Fact]
        public void UploadFile_WhenServiceThrowsException_LogsError()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Throws(new Exception("Upload error"));

            // Act
            _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error occurred while uploading file")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void UploadFile_WhenStreamThrowsException_Returns500()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream())
                .Throws(new IOException("Stream error"));

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        #endregion

        #region UploadFile - Logging Tests

        [Fact]
        public void UploadFile_WithValidFile_LogsInformationOnEntry()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Upload file")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void UploadFile_WithValidFile_LogsInformationOnSuccess()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("File name")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region UploadFile - Special Cases

        [Fact]
        public void UploadFile_WithSpecialCharactersInFileName_ReturnsOk()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "file with spaces & special-chars.pdf";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Theory]
        [InlineData("file.jpg")]
        [InlineData("document.pdf")]
        [InlineData("archive.zip")]
        [InlineData("data.json")]
        [InlineData("config.xml")]
        public void UploadFile_WithDifferentExtensions_ReturnsOk(string fileName)
        {
            // Arrange
            var bucketName = "test-bucket";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
            var mockFile = CreateMockFormFile(fileName, stream);

            _mockFileService.Setup(x => x.InsertFromStream(It.IsAny<Stream>(), bucketName, fileName))
                .Returns(fileName);

            // Act
            var result = _controller.UploadFile(bucketName, mockFile.Object);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        #endregion

        #region Helper Methods

        private Mock<IFormFile> CreateMockFormFile(string fileName, Stream stream, long? length = null)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(length ?? stream.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.ContentType).Returns("application/octet-stream");
            
            return mockFile;
        }

        #endregion
    }
}
