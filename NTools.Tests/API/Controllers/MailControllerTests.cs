using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BazzucaMedia.API.Controllers;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.MailerSend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTools.Tests.API.Controllers
{
    public class MailControllerTests
    {
        private readonly Mock<IMailerSendService> _mockMailService;
        private readonly Mock<ILogger<MailController>> _mockLogger;
        private readonly MailController _controller;

        public MailControllerTests()
        {
            _mockMailService = new Mock<IMailerSendService>();
            _mockLogger = new Mock<ILogger<MailController>>();
            _controller = new MailController(_mockMailService.Object, _mockLogger.Object);
        }

        #region Sendmail - Success Tests

        [Fact]
        public async Task Sendmail_WithValidEmail_ReturnsOkWithTrue()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithValidEmail_ReturnsOkWithFalse()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithValidEmail_CallsServiceOnce()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            await _controller.Sendmail(mailInfo);

            // Assert
            _mockMailService.Verify(x => x.Sendmail(mailInfo), Times.Once);
        }

        [Fact]
        public async Task Sendmail_WithMultipleRecipients_ReturnsOk()
        {
            // Arrange
            var mailInfo = new MailerInfo
            {
                From = new MailerRecipientInfo { Email = "sender@example.com", Name = "Sender" },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo { Email = "recipient1@example.com", Name = "Recipient 1" },
                    new MailerRecipientInfo { Email = "recipient2@example.com", Name = "Recipient 2" },
                    new MailerRecipientInfo { Email = "recipient3@example.com", Name = "Recipient 3" }
                },
                Subject = "Test Email",
                Text = "Test content",
                Html = "<p>Test content</p>"
            };
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithHtmlContent_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Html = "<html><body><h1>Test Email</h1><p>This is a test email</p></body></html>";
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithTextContent_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = "This is a plain text email with multiple lines.\nLine 2\nLine 3";
            mailInfo.Html = null;
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Theory]
        [InlineData("Welcome to our service")]
        [InlineData("Password Reset Request")]
        [InlineData("Your order has been shipped")]
        [InlineData("Meeting Reminder")]
        public async Task Sendmail_WithDifferentSubjects_ReturnsOk(string subject)
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = subject;
            _mockMailService.Setup(x => x.Sendmail(It.IsAny<MailerInfo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region Sendmail - Error Tests

        [Fact]
        public async Task Sendmail_WhenServiceThrowsException_Returns500()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var exceptionMessage = "Failed to send email";
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(exceptionMessage, statusCodeResult.Value);
        }

        [Fact]
        public async Task Sendmail_WhenServiceThrowsException_LogsError()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            await _controller.Sendmail(mailInfo);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error sending mail")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Sendmail_WhenServiceThrowsTimeoutException_Returns500()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var exceptionMessage = "Request timeout";
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ThrowsAsync(new TimeoutException(exceptionMessage));

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(exceptionMessage, statusCodeResult.Value);
        }

        #endregion

        #region Sendmail - Logging Tests

        [Fact]
        public async Task Sendmail_WithValidEmail_LogsInformation()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            await _controller.Sendmail(mailInfo);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Sending email")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Sendmail_WithValidEmail_LogsRecipientAndSubject()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            _mockMailService.Setup(x => x.Sendmail(mailInfo))
                .ReturnsAsync(true);

            // Act
            await _controller.Sendmail(mailInfo);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString()!.Contains("Sending email") &&
                        o.ToString()!.Contains(mailInfo.Subject)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region IsValidEmail - Valid Email Tests

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("user_name@example.org")]
        [InlineData("123@example.com")]
        [InlineData("test@subdomain.example.com")]
        public void IsValidEmail_WithValidEmails_ReturnsOkWithTrue(string email)
        {
            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithValidEmail_ReturnsOk()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region IsValidEmail - Invalid Email Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid")]
        [InlineData("invalid@")]
        [InlineData("@example.com")]
        [InlineData("invalid @example.com")]
        [InlineData("invalid@example")]
        public void IsValidEmail_WithInvalidEmails_ReturnsOkWithFalse(string email)
        {
            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithNull_ReturnsOkWithFalse()
        {
            // Arrange
            string email = null!;

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithEmptyString_ReturnsOkWithFalse()
        {
            // Arrange
            var email = string.Empty;

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithWhitespace_ReturnsOkWithFalse()
        {
            // Arrange
            var email = "   ";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        #endregion

        #region IsValidEmail - Logging Tests

        [Fact]
        public void IsValidEmail_WithValidEmail_LogsInformationOnEntry()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            _controller.IsValidEmail(email);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Verify if email")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void IsValidEmail_WithValidEmail_LogsInformationOnSuccess()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            _controller.IsValidEmail(email);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Email validation result")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void IsValidEmail_WithInvalidEmail_LogsValidationResult()
        {
            // Arrange
            var email = "invalid-email";

            // Act
            _controller.IsValidEmail(email);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Email validation result")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region IsValidEmail - Edge Cases

        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("invalid", false)]
        [InlineData("user.name@example.co.uk", true)]
        [InlineData("@example.com", false)]
        [InlineData("user@", false)]
        public void IsValidEmail_WithVariousInputs_ReturnsExpectedResult(string email, bool expectedResult)
        {
            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(expectedResult, (bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithVeryLongEmail_ReturnsOk()
        {
            // Arrange
            var email = "very.long.email.address.with.many.dots@subdomain.example.com";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public void IsValidEmail_WithSpecialCharactersInLocalPart_ReturnsOk()
        {
            // Arrange
            var email = "user+tag_123@example.com";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region IsValidEmail - Response Status Tests

        [Fact]
        public void IsValidEmail_WithValidInput_ReturnsOkStatus()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void IsValidEmail_WithInvalidInput_ReturnsOkStatusWithFalse()
        {
            // Arrange
            var email = "invalid";

            // Act
            var result = _controller.IsValidEmail(email);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        #endregion

        #region Sendmail - Special Cases

        [Fact]
        public async Task Sendmail_WithUnicodeSubject_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = "???? - Test Email - ???????? ??????";
            _mockMailService.Setup(x => x.Sendmail(It.IsAny<MailerInfo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithUnicodeContent_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = "Unicode content: ???? ?????? ??? ????? ???????";
            _mockMailService.Setup(x => x.Sendmail(It.IsAny<MailerInfo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithLongContent_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = new string('A', 10000);
            _mockMailService.Setup(x => x.Sendmail(It.IsAny<MailerInfo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Sendmail_WithEmptySubject_ReturnsOk()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = string.Empty;
            _mockMailService.Setup(x => x.Sendmail(It.IsAny<MailerInfo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Sendmail(mailInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region Helper Methods

        private MailerInfo CreateValidMailInfo()
        {
            return new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "sender@example.com",
                    Name = "Sender Name"
                },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo
                    {
                        Email = "recipient@example.com",
                        Name = "Recipient Name"
                    }
                },
                Subject = "Test Subject",
                Text = "Test content",
                Html = "<p>Test content</p>"
            };
        }

        #endregion
    }
}
