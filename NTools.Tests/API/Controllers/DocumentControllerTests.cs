using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BazzucaMedia.API.Controllers;

namespace NTools.Tests.API.Controllers
{
    public class DocumentControllerTests
    {
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mockLogger = new Mock<ILogger<DocumentController>>();
            _controller = new DocumentController(_mockLogger.Object);
        }

        #region ValidarCpfOuCnpj - Valid Documents Tests

        [Theory]
        [InlineData("12345678909")]
        [InlineData("123.456.789-09")]
        [InlineData("529.982.247-25")]
        [InlineData("52998224725")]
        public void ValidarCpfOuCnpj_WithValidCpf_ReturnsOkWithTrue(string cpf)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Theory]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        [InlineData("34028316000103")]
        [InlineData("34.028.316/0001-03")]
        public void ValidarCpfOuCnpj_WithValidCnpj_ReturnsOkWithTrue(string cnpj)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cnpj);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Invalid Documents Tests

        [Theory]
        [InlineData("12345678900")]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("123.456.789-00")]
        [InlineData("52998224726")]
        public void ValidarCpfOuCnpj_WithInvalidCpf_ReturnsOkWithFalse(string cpf)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Theory]
        [InlineData("11222333000180")]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("11.222.333/0001-80")]
        [InlineData("11222333000182")]
        public void ValidarCpfOuCnpj_WithInvalidCnpj_ReturnsOkWithFalse(string cnpj)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cnpj);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Edge Cases Tests

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("123456789")]
        [InlineData("12345678901234")]
        public void ValidarCpfOuCnpj_WithInvalidLength_ReturnsOkWithFalse(string documento)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(documento);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Theory]
        [InlineData("abc.def.ghi-jk")]
        [InlineData("abc12345678")]
        [InlineData("!!!########")]
        public void ValidarCpfOuCnpj_WithNonNumericCharacters_ReturnsOkWithFalse(string documento)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(documento);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Theory]
        [InlineData("   12345678909   ")]
        [InlineData("   11222333000181   ")]
        public void ValidarCpfOuCnpj_WithWhitespace_ReturnsOkWithTrue(string documento)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(documento);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Logging Tests

        [Fact]
        public void ValidarCpfOuCnpj_WithValidDocument_LogsInformation()
        {
            // Arrange
            var cpf = "12345678909";

            // Act
            _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("validarCpfOuCnpj")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidarCpfOuCnpj_WithInvalidDocument_LogsInformation()
        {
            // Arrange
            var cpf = "00000000000";

            // Act
            _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("validarCpfOuCnpj")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region ValidarCpfOuCnpj - Response Status Tests

        [Fact]
        public void ValidarCpfOuCnpj_WithValidInput_ReturnsOkStatus()
        {
            // Arrange
            var cpf = "12345678909";

            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void ValidarCpfOuCnpj_WithInvalidInput_ReturnsOkStatusWithFalse()
        {
            // Arrange
            var cpf = "invalid";

            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Multiple Format Tests

        [Theory]
        [InlineData("12345678909", "123.456.789-09")]
        [InlineData("11222333000181", "11.222.333/0001-81")]
        public void ValidarCpfOuCnpj_WithDifferentFormats_ReturnsSameResult(string unformatted, string formatted)
        {
            // Act
            var result1 = _controller.ValidarCpfOuCnpj(unformatted);
            var result2 = _controller.ValidarCpfOuCnpj(formatted);

            // Assert
            var okResult1 = Assert.IsType<OkObjectResult>(result1.Result);
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            Assert.Equal(okResult1.Value, okResult2.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Real-World Scenarios

        [Theory]
        [InlineData("12345678909", true)]
        [InlineData("111.444.777-35", true)]
        [InlineData("11222333000181", true)]
        [InlineData("11.222.333/0001-81", true)]
        [InlineData("12345678900", false)]
        [InlineData("11111111111", false)]
        [InlineData("11222333000180", false)]
        [InlineData("00000000000000", false)]
        [InlineData("", false)]
        [InlineData("abc", false)]
        public void ValidarCpfOuCnpj_RealWorldScenarios_ReturnsExpectedResult(string documento, bool expectedResult)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(documento);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(expectedResult, (bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Repeated Digits Tests

        [Theory]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("22222222222")]
        [InlineData("33333333333")]
        [InlineData("44444444444")]
        [InlineData("55555555555")]
        [InlineData("66666666666")]
        [InlineData("77777777777")]
        [InlineData("88888888888")]
        [InlineData("99999999999")]
        public void ValidarCpfOuCnpj_WithRepeatedDigitsCpf_ReturnsOkWithFalse(string cpf)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        [InlineData("33333333333333")]
        [InlineData("44444444444444")]
        [InlineData("55555555555555")]
        [InlineData("66666666666666")]
        [InlineData("77777777777777")]
        [InlineData("88888888888888")]
        [InlineData("99999999999999")]
        public void ValidarCpfOuCnpj_WithRepeatedDigitsCnpj_ReturnsOkWithFalse(string cnpj)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cnpj);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.False((bool)okResult.Value);
        }

        #endregion

        #region ValidarCpfOuCnpj - Leading Zeros Tests

        [Theory]
        [InlineData("000.000.001-91")]
        [InlineData("00000000191")]
        public void ValidarCpfOuCnpj_WithLeadingZerosCpf_ReturnsOkWithTrue(string cpf)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Theory]
        [InlineData("00.000.000/0001-91")]
        [InlineData("00000000000191")]
        public void ValidarCpfOuCnpj_WithLeadingZerosCnpj_ReturnsOkWithTrue(string cnpj)
        {
            // Act
            var result = _controller.ValidarCpfOuCnpj(cnpj);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        #endregion
    }
}
