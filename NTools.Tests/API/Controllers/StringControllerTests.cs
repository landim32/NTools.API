using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BazzucaMedia.API.Controllers;
using System;

namespace NTools.Tests.API.Controllers
{
    public class StringControllerTests
    {
        private readonly Mock<ILogger<StringController>> _mockLogger;
        private readonly StringController _controller;

        public StringControllerTests()
        {
            _mockLogger = new Mock<ILogger<StringController>>();
            _controller = new StringController(_mockLogger.Object);
        }

        #region GenerateSlug - Success Tests

        [Fact]
        public void GenerateSlug_WithSimpleString_ReturnsOkWithSlug()
        {
            // Arrange
            var input = "Hello World";
            var expectedSlug = "hello-world";

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedSlug, okResult.Value);
        }

        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("Test String", "test-string")]
        [InlineData("Multiple   Spaces", "multiple-spaces")]
        [InlineData("UPPERCASE", "uppercase")]
        [InlineData("lowercase", "lowercase")]
        public void GenerateSlug_WithVariousInputs_ReturnsExpectedSlug(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("Olá Mundo", "ola-mundo")]
        [InlineData("Café com Pão", "cafe-com-pao")]
        [InlineData("São Paulo", "sao-paulo")]
        [InlineData("Ação e Reação", "acao-e-reacao")]
        public void GenerateSlug_WithAccentedCharacters_RemovesAccents(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("Test@123", "test123")]
        [InlineData("Hello!World", "helloworld")]
        [InlineData("Test#String$", "teststring")]
        [InlineData("Price: $100", "price-100")]
        public void GenerateSlug_WithSpecialCharacters_RemovesSpecialChars(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void GenerateSlug_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(string.Empty, okResult.Value);
        }

        [Fact]
        public void GenerateSlug_WithWhitespace_ReturnsEmptyString()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(string.Empty, okResult.Value);
        }

        [Theory]
        [InlineData("Test-String", "test-string")]
        [InlineData("Already-Has-Hyphens", "already-has-hyphens")]
        [InlineData("Multiple---Hyphens", "multiple-hyphens")]
        public void GenerateSlug_WithHyphens_NormalizesHyphens(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("123 Numbers", "123-numbers")]
        [InlineData("Test 123", "test-123")]
        [InlineData("2024 Year", "2024-year")]
        public void GenerateSlug_WithNumbers_KeepsNumbers(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        #endregion

        #region GenerateSlug - Logging Tests

        [Fact]
        public void GenerateSlug_WithValidInput_LogsInformation()
        {
            // Arrange
            var input = "Test String";

            // Act
            _controller.GenerateSlug(input);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Generate Slug")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GenerateSlug_WithValidInput_LogsInputAndOutput()
        {
            // Arrange
            var input = "Test String";

            // Act
            _controller.GenerateSlug(input);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString()!.Contains("Generate Slug") &&
                        o.ToString()!.Contains(input)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region OnlyNumbers - Success Tests

        [Fact]
        public void OnlyNumbers_WithMixedInput_ReturnsOnlyDigits()
        {
            // Arrange
            var input = "abc123def456";
            var expected = "123456";

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("123", "123")]
        [InlineData("abc123", "123")]
        [InlineData("123abc", "123")]
        [InlineData("a1b2c3", "123")]
        [InlineData("Test123Test", "123")]
        public void OnlyNumbers_WithVariousInputs_ExtractsOnlyNumbers(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void OnlyNumbers_WithNoNumbers_ReturnsEmptyString()
        {
            // Arrange
            var input = "abcdef";

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(string.Empty, okResult.Value);
        }

        [Fact]
        public void OnlyNumbers_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(string.Empty, okResult.Value);
        }

        [Theory]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("123.456.789-09", "12345678909")]
        [InlineData("R$ 1.234,56", "123456")]
        [InlineData("+55 11 1234-5678", "551112345678")]
        public void OnlyNumbers_WithFormattedNumbers_ExtractsDigitsOnly(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("Test@123#456", "123456")]
        [InlineData("Price: $99.99", "9999")]
        [InlineData("123!@#456$%^789", "123456789")]
        public void OnlyNumbers_WithSpecialCharacters_IgnoresSpecialChars(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void OnlyNumbers_WithOnlyNumbers_ReturnsAllNumbers()
        {
            // Arrange
            var input = "1234567890";

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(input, okResult.Value);
        }

        [Theory]
        [InlineData("   123   ", "123")]
        [InlineData("123\n456", "123456")]
        [InlineData("123\t456", "123456")]
        public void OnlyNumbers_WithWhitespace_IgnoresWhitespace(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        #endregion

        #region OnlyNumbers - Logging Tests

        [Fact]
        public void OnlyNumbers_WithValidInput_LogsInformation()
        {
            // Arrange
            var input = "abc123";

            // Act
            _controller.OnlyNumbers(input);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Extract only numbers")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void OnlyNumbers_WithValidInput_LogsInputAndOutput()
        {
            // Arrange
            var input = "abc123";

            // Act
            _controller.OnlyNumbers(input);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString()!.Contains("Extract only numbers") &&
                        o.ToString()!.Contains(input)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region GenerateShortUniqueString - Success Tests

        [Fact]
        public void GenerateShortUniqueString_ReturnsOkWithString()
        {
            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.IsType<string>(okResult.Value);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsNonEmptyString()
        {
            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = okResult.Value as string;
            Assert.NotNull(value);
            Assert.NotEmpty(value);
        }

        [Fact]
        public void GenerateShortUniqueString_GeneratesUniqueStrings()
        {
            // Act
            var result1 = _controller.GenerateShortUniqueString();
            var result2 = _controller.GenerateShortUniqueString();

            // Assert
            var okResult1 = Assert.IsType<OkObjectResult>(result1.Result);
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            var value1 = okResult1.Value as string;
            var value2 = okResult2.Value as string;
            
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotEqual(value1, value2);
        }

        [Fact]
        public void GenerateShortUniqueString_ContainsOnlyBase62Characters()
        {
            // Arrange
            var validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = okResult.Value as string;
            Assert.NotNull(value);
            Assert.All(value, c => Assert.Contains(c, validChars));
        }

        [Fact]
        public void GenerateShortUniqueString_MultipleCallsGenerateDifferentValues()
        {
            // Arrange
            var generatedStrings = new HashSet<string>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var result = _controller.GenerateShortUniqueString();
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var value = okResult.Value as string;
                Assert.NotNull(value);
                generatedStrings.Add(value);
            }

            // Assert
            Assert.Equal(100, generatedStrings.Count);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsReasonableLength()
        {
            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = okResult.Value as string;
            Assert.NotNull(value);
            Assert.InRange(value.Length, 1, 50);
        }

        #endregion

        #region GenerateShortUniqueString - Logging Tests

        [Fact]
        public void GenerateShortUniqueString_LogsInformation()
        {
            // Act
            _controller.GenerateShortUniqueString();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Generate short unique string")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void GenerateShortUniqueString_LogsGeneratedValue()
        {
            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = okResult.Value as string;
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString()!.Contains("Generate short unique string") &&
                        o.ToString()!.Contains(value!)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Response Status Tests

        [Fact]
        public void GenerateSlug_ReturnsOkStatus()
        {
            // Arrange
            var input = "test";

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void OnlyNumbers_ReturnsOkStatus()
        {
            // Arrange
            var input = "abc123";

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsOkStatus()
        {
            // Act
            var result = _controller.GenerateShortUniqueString();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Theory]
        [InlineData("Test", "test")]
        [InlineData("T", "t")]
        [InlineData("AB", "ab")]
        public void GenerateSlug_WithShortStrings_WorksCorrectly(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void GenerateSlug_WithVeryLongString_ReturnsSlug()
        {
            // Arrange
            var input = new string('a', 1000);
            var expected = new string('a', 1000);

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("?????? ???", "")]
        [InlineData("????", "")]
        [InlineData("?????", "")]
        public void GenerateSlug_WithNonLatinCharacters_RemovesThemOrHandlesAppropriately(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void OnlyNumbers_WithVeryLongInput_ExtractsNumbers()
        {
            // Arrange
            var input = string.Concat(Enumerable.Range(0, 100).Select(i => $"a{i}"));

            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = okResult.Value as string;
            Assert.NotNull(value);
            Assert.True(value.All(char.IsDigit));
        }

        [Theory]
        [InlineData("-123", "123")]
        [InlineData("+456", "456")]
        [InlineData("0000", "0000")]
        public void OnlyNumbers_WithSignsAndZeros_ExtractsDigitsOnly(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void GenerateSlug_WithLeadingAndTrailingSpaces_TrimsCorrectly()
        {
            // Arrange
            var input = "   Test String   ";
            var expected = "test-string";

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public void GenerateSlug_WithLeadingAndTrailingHyphens_TrimsHyphens()
        {
            // Arrange
            var input = "-Test-";
            var expected = "test";

            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("Product Name 2024", "product-name-2024")]
        [InlineData("My First Blog Post!", "my-first-blog-post")]
        [InlineData("São Paulo - Brasil", "sao-paulo-brasil")]
        [InlineData("C# Programming Guide", "c-programming-guide")]
        public void GenerateSlug_WithRealWorldExamples_GeneratesValidSlugs(string input, string expected)
        {
            // Act
            var result = _controller.GenerateSlug(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Theory]
        [InlineData("Order #12345", "12345")]
        [InlineData("Invoice: INV-2024-001", "2024001")]
        [InlineData("SKU: ABC123XYZ", "123")]
        [InlineData("Telefone: (11) 98765-4321", "11987654321")]
        public void OnlyNumbers_WithRealWorldExamples_ExtractsNumbers(string input, string expected)
        {
            // Act
            var result = _controller.OnlyNumbers(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        #endregion
    }
}
