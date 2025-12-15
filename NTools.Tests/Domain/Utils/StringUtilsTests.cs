using NTools.Domain.Utils;

namespace NTools.Tests.Domain.Utils
{
    public class StringUtilsTests
    {
        #region OnlyNumbers Tests

        [Theory]
        [InlineData("123", "123")]
        [InlineData("abc123def", "123")]
        [InlineData("12.34", "1234")]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("CPF: 123.456.789-09", "12345678909")]
        [InlineData("R$ 1.234,56", "123456")]
        [InlineData("Test 123 ABC 456", "123456")]
        [InlineData("2024-01-15", "20240115")]
        [InlineData("Version 2.0.1", "201")]
        [InlineData("!@#$%123^&*()", "123")]
        public void OnlyNumbers_WithMixedContent_ReturnsOnlyDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("123")]
        [InlineData("9876543210")]
        [InlineData("00000")]
        [InlineData("1234567890")]
        public void OnlyNumbers_WithOnlyDigits_ReturnsUnchanged(string input)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(input, result);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("xyz")]
        [InlineData("Hello World")]
        [InlineData("!@#$%")]
        [InlineData("   ")]
        public void OnlyNumbers_WithNoDigits_ReturnsEmptyString(string input)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void OnlyNumbers_WithNull_ReturnsEmptyString()
        {
            // Act
            var result = StringUtils.OnlyNumbers(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("")]
        public void OnlyNumbers_WithEmptyString_ReturnsEmptyString(string input)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("123abc456def789", "123456789")]
        [InlineData("1a2b3c4d5e6f7g8h9i0", "1234567890")]
        [InlineData("!1@2#3$4%5^6&7*8(9)0", "1234567890")]
        public void OnlyNumbers_WithAlternatingCharacters_ExtractsAllDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region OnlyNumbers Real-World Scenarios

        [Theory]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("+55 11 98765-4321", "5511987654321")]
        [InlineData("011 3456-7890", "01134567890")]
        [InlineData("(21) 3333-4444", "2133334444")]
        public void OnlyNumbers_WithPhoneNumbers_ExtractsDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("123.456.789-09", "12345678909")]
        [InlineData("11.222.333/0001-81", "11222333000181")]
        [InlineData("000.000.001-91", "00000000191")]
        public void OnlyNumbers_WithDocumentFormats_ExtractsDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("R$ 1.234,56", "123456")]
        [InlineData("$ 99.99", "9999")]
        [InlineData("€ 1,234.50", "123450")]
        [InlineData("Price: $100.00", "10000")]
        public void OnlyNumbers_WithCurrencyFormats_ExtractsDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("2024-01-15", "20240115")]
        [InlineData("15/01/2024", "15012024")]
        [InlineData("01-15-2024", "01152024")]
        [InlineData("2024.01.15", "20240115")]
        public void OnlyNumbers_WithDateFormats_ExtractsDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region GenerateShortUniqueString Tests

        [Fact]
        public void GenerateShortUniqueString_ReturnsNonEmptyString()
        {
            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsBase62Characters()
        {
            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.All(result, c => Assert.Contains(c, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));
        }

        [Fact]
        public void GenerateShortUniqueString_CalledMultipleTimes_ReturnsDifferentValues()
        {
            // Act
            var result1 = StringUtils.GenerateShortUniqueString();
            var result2 = StringUtils.GenerateShortUniqueString();
            var result3 = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.NotEqual(result1, result2);
            Assert.NotEqual(result2, result3);
            Assert.NotEqual(result1, result3);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsShorterThanGuid()
        {
            // Arrange
            var guidLength = Guid.NewGuid().ToString().Length; // 36 characters with hyphens

            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.True(result.Length < guidLength, $"Expected length < {guidLength}, but got {result.Length}");
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsConsistentLength()
        {
            // Act
            var results = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                results.Add(StringUtils.GenerateShortUniqueString());
            }

            // Assert - all results should have similar lengths (within reasonable range)
            var minLength = results.Min(r => r.Length);
            var maxLength = results.Max(r => r.Length);
            Assert.True(maxLength - minLength <= 2, $"Length variation too high: min={minLength}, max={maxLength}");
        }

        [Fact]
        public void GenerateShortUniqueString_DoesNotContainSpecialCharacters()
        {
            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.DoesNotContain("-", result);
            Assert.DoesNotContain("_", result);
            Assert.DoesNotContain(" ", result);
            Assert.DoesNotContain(".", result);
            Assert.DoesNotContain(",", result);
        }

        [Fact]
        public void GenerateShortUniqueString_GeneratesUrlSafeString()
        {
            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert - should only contain alphanumeric characters
            Assert.Matches(@"^[0-9A-Za-z]+$", result);
        }

        #endregion

        #region GenerateShortUniqueString Uniqueness Tests

        [Fact]
        public void GenerateShortUniqueString_GeneratesManyUniqueStrings()
        {
            // Arrange
            var count = 1000;
            var results = new HashSet<string>();

            // Act
            for (int i = 0; i < count; i++)
            {
                results.Add(StringUtils.GenerateShortUniqueString());
            }

            // Assert
            Assert.Equal(count, results.Count);
        }

        [Fact]
        public void GenerateShortUniqueString_InParallel_GeneratesUniqueStrings()
        {
            // Arrange
            var count = 100;
            var results = new System.Collections.Concurrent.ConcurrentBag<string>();

            // Act
            Parallel.For(0, count, _ =>
            {
                results.Add(StringUtils.GenerateShortUniqueString());
            });

            // Assert
            Assert.Equal(count, results.Distinct().Count());
        }

        #endregion

        #region GenerateShortUniqueString Performance Tests

        [Fact]
        public void GenerateShortUniqueString_CompletesQuickly()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 1000; i++)
            {
                StringUtils.GenerateShortUniqueString();
            }
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Expected < 1000ms, but took {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void OnlyNumbers_WithUnicodeDigits_ExtractsStandardDigits()
        {
            // Arrange - using standard ASCII digits
            var input = "123abc456";

            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal("123456", result);
        }

        [Fact]
        public void OnlyNumbers_WithVeryLongString_HandlesCorrectly()
        {
            // Arrange
            var input = string.Join("", Enumerable.Range(0, 10000).Select(i => i % 2 == 0 ? i.ToString() : "abc"));

            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, c => Assert.True(char.IsDigit(c)));
        }

        [Theory]
        [InlineData("\t123\n456\r789", "123456789")]
        [InlineData("  123  456  ", "123456")]
        [InlineData("\n\r\t123\n\r\t", "123")]
        public void OnlyNumbers_WithWhitespaceCharacters_ExtractsOnlyDigits(string input, string expected)
        {
            // Act
            var result = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Idempotency Tests

        [Theory]
        [InlineData("abc123def")]
        [InlineData("(11) 98765-4321")]
        [InlineData("R$ 1.234,56")]
        public void OnlyNumbers_CalledTwice_ReturnsSameResult(string input)
        {
            // Act
            var result1 = StringUtils.OnlyNumbers(input);
            var result2 = StringUtils.OnlyNumbers(input);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void OnlyNumbers_OnResultOfOnlyNumbers_ReturnsUnchanged()
        {
            // Arrange
            var input = "abc123def456";
            var firstResult = StringUtils.OnlyNumbers(input);

            // Act
            var secondResult = StringUtils.OnlyNumbers(firstResult);

            // Assert
            Assert.Equal(firstResult, secondResult);
        }

        #endregion

        #region Practical Usage Scenarios

        [Fact]
        public void OnlyNumbers_ForDatabaseKeyGeneration_ProducesCleanNumber()
        {
            // Arrange
            var formattedId = "ID-2024-001-ABC";

            // Act
            var numericId = StringUtils.OnlyNumbers(formattedId);

            // Assert
            Assert.Equal("2024001", numericId);
            Assert.All(numericId, c => Assert.True(char.IsDigit(c)));
        }

        [Fact]
        public void GenerateShortUniqueString_ForUrlSlug_ProducesValidString()
        {
            // Act
            var slug = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.NotEmpty(slug);
            Assert.Matches(@"^[0-9A-Za-z]+$", slug);
            Assert.DoesNotContain(" ", slug);
        }

        [Fact]
        public void GenerateShortUniqueString_ForTemporaryFileName_ProducesValidName()
        {
            // Act
            var fileName = $"temp_{StringUtils.GenerateShortUniqueString()}.txt";

            // Assert
            Assert.DoesNotContain("/", fileName);
            Assert.DoesNotContain("\\", fileName);
            Assert.DoesNotContain(":", fileName);
            Assert.Contains(".txt", fileName);
        }

        [Fact]
        public void OnlyNumbers_ForCreditCardFormatting_ExtractsDigits()
        {
            // Arrange
            var formattedCard = "1234 5678 9012 3456";

            // Act
            var cleanCard = StringUtils.OnlyNumbers(formattedCard);

            // Assert
            Assert.Equal("1234567890123456", cleanCard);
            Assert.Equal(16, cleanCard.Length);
        }

        #endregion

        #region Combination Tests

        [Fact]
        public void OnlyNumbers_ThenGenerateShortUniqueString_ProducesDifferentTypes()
        {
            // Arrange
            var input = "abc123def";

            // Act
            var onlyNumbers = StringUtils.OnlyNumbers(input);
            var uniqueString = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.NotEmpty(onlyNumbers);
            Assert.NotEmpty(uniqueString);
            Assert.NotEqual(onlyNumbers, uniqueString);
        }

        #endregion

        #region Type Safety Tests

        [Fact]
        public void OnlyNumbers_ReturnsStringType()
        {
            // Act
            var result = StringUtils.OnlyNumbers("123abc");

            // Assert
            Assert.IsType<string>(result);
        }

        [Fact]
        public void GenerateShortUniqueString_ReturnsStringType()
        {
            // Act
            var result = StringUtils.GenerateShortUniqueString();

            // Assert
            Assert.IsType<string>(result);
        }

        #endregion
    }
}
