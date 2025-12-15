using NTools.Domain.Utils;

namespace NTools.Tests.Domain.Utils
{
    public class SlugHelperTests
    {
        #region Basic Functionality Tests

        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("Test String", "test-string")]
        [InlineData("Simple Text", "simple-text")]
        [InlineData("UPPERCASE", "uppercase")]
        [InlineData("MixedCase", "mixedcase")]
        [InlineData("CamelCaseText", "camelcasetext")]
        public void GenerateSlug_WithBasicText_ReturnsSlugifiedText(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GenerateSlug_WithNull_ReturnsEmptyString()
        {
            // Act
            var result = SlugHelper.GenerateSlug(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void GenerateSlug_WithEmptyOrWhitespace_ReturnsEmptyString(string input)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region Accent Removal Tests

        [Theory]
        [InlineData("café", "cafe")]
        [InlineData("naïve", "naive")]
        [InlineData("résumé", "resume")]
        [InlineData("piñata", "pinata")]
        [InlineData("Ação", "acao")]
        [InlineData("Ótimo", "otimo")]
        [InlineData("Através", "atraves")]
        [InlineData("É possível", "e-possivel")]
        [InlineData("Müller", "muller")]
        [InlineData("Zürich", "zurich")]
        [InlineData("Öffentlich", "offentlich")]
        [InlineData("Ñoño", "nono")]
        [InlineData("Señor", "senor")]
        [InlineData("Año Nuevo", "ano-nuevo")]
        [InlineData("Côte d'Ivoire", "cote-divoire")]
        [InlineData("Château", "chateau")]
        [InlineData("Français", "francais")]
        [InlineData("café résumé naïve", "cafe-resume-naive")]
        [InlineData("Múltiplos Ácêntõs Pára Téstãr", "multiplos-acentos-para-testar")]
        public void GenerateSlug_WithAccents_RemovesAccents(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Special Characters Tests

        [Theory]
        [InlineData("Hello! World?", "hello-world")]
        [InlineData("Test & Example", "test-example")]
        [InlineData("Price: $100", "price-100")]
        [InlineData("Email: test@example.com", "email-testexamplecom")]
        [InlineData("C# Programming", "c-programming")]
        [InlineData("C++ Language", "c-language")]
        [InlineData("50% Off!", "50-off")]
        [InlineData("(Example)", "example")]
        [InlineData("[Test]", "test")]
        [InlineData("{Brackets}", "brackets")]
        [InlineData("<HTML>", "html")]
        [InlineData("!!!")]
        [InlineData("@@@")]
        [InlineData("###")]
        public void GenerateSlug_WithSpecialCharacters_RemovesOrHandlesCorrectly(string input, string expected = "")
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Whitespace and Hyphen Tests

        [Theory]
        [InlineData("Multiple   Spaces", "multiple-spaces")]
        [InlineData("Tab\tCharacter", "tab-character")]
        [InlineData("New\nLine", "new-line")]
        [InlineData("Already-Has-Hyphens", "already-has-hyphens")]
        [InlineData("Test--Double--Hyphen", "test-double-hyphen")]
        [InlineData("Multiple---Hyphens", "multiple-hyphens")]
        [InlineData("-Leading Hyphen", "leading-hyphen")]
        [InlineData("Trailing Hyphen-", "trailing-hyphen")]
        [InlineData("-Both Sides-", "both-sides")]
        [InlineData("  Leading Spaces", "leading-spaces")]
        [InlineData("Trailing Spaces  ", "trailing-spaces")]
        [InlineData("  Both Sides  ", "both-sides")]
        public void GenerateSlug_WithWhitespaceAndHyphens_NormalizesCorrectly(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Numbers Tests

        [Theory]
        [InlineData("Product 123", "product-123")]
        [InlineData("Version 2.0", "version-20")]
        [InlineData("Test 001", "test-001")]
        [InlineData("123", "123")]
        [InlineData("2024", "2024")]
        [InlineData("42", "42")]
        public void GenerateSlug_WithNumbers_PreservesNumbers(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("Como Fazer um Bolo de Chocolate", "como-fazer-um-bolo-de-chocolate")]
        [InlineData("10 Dicas para Programação em C#", "10-dicas-para-programacao-em-c")]
        [InlineData("Introdução ao .NET Core", "introducao-ao-net-core")]
        [InlineData("Notebook Dell Inspiron 15 3000", "notebook-dell-inspiron-15-3000")]
        [InlineData("iPhone 13 Pro Max 256GB", "iphone-13-pro-max-256gb")]
        [InlineData("Smart TV 55\" 4K Samsung", "smart-tv-55-4k-samsung")]
        [InlineData("São Paulo - SP", "sao-paulo-sp")]
        [InlineData("Rio de Janeiro/RJ", "rio-de-janeirorj")]
        [InlineData("Brasília (DF)", "brasilia-df")]
        [InlineData("O Senhor dos Anéis", "o-senhor-dos-aneis")]
        [InlineData("Harry Potter e a Pedra Filosofal", "harry-potter-e-a-pedra-filosofal")]
        [InlineData("1984 - George Orwell", "1984-george-orwell")]
        public void GenerateSlug_WithRealWorldExamples_ReturnsValidSlugs(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region URL-Friendly Tests

        [Fact]
        public void GenerateSlug_ResultContainsOnlyValidUrlCharacters()
        {
            // Arrange
            var input = "Test! @#$% Special ^&*() Characters 123";

            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Matches(@"^[a-z0-9-]*$", result);
        }

        [Fact]
        public void GenerateSlug_ResultDoesNotStartOrEndWithHyphen()
        {
            // Arrange
            var input = "---Test---";

            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.DoesNotMatch(@"^-", result);
            Assert.DoesNotMatch(@"-$", result);
        }

        [Fact]
        public void GenerateSlug_ResultDoesNotContainConsecutiveHyphens()
        {
            // Arrange
            var input = "Test   Multiple   Spaces";

            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.DoesNotContain("--", result);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GenerateSlug_WithVeryLongString_ReturnsValidSlug()
        {
            // Arrange
            var input = "This is a very long string that should be converted to a slug without any issues even though it has many many words in it";

            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.NotEmpty(result);
            Assert.DoesNotContain("  ", result);
            Assert.DoesNotContain("--", result);
            Assert.Matches(@"^[a-z0-9-]+$", result);
        }

        #endregion

        #region Idempotency Tests

        [Theory]
        [InlineData("Hello World")]
        [InlineData("Café com Leite")]
        [InlineData("Test! @#$ 123")]
        public void GenerateSlug_CalledTwice_ReturnsSameResult(string input)
        {
            // Act
            var result1 = SlugHelper.GenerateSlug(input);
            var result2 = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Theory]
        [InlineData("hello-world")]
        [InlineData("already-a-slug")]
        [InlineData("test-123")]
        public void GenerateSlug_WithExistingSlug_RemainsUnchanged(string input)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(input, result);
        }

        #endregion

        #region Mixed Content Tests

        [Theory]
        [InlineData("123 ABC 456", "123-abc-456")]
        [InlineData("Test123Mix456", "test123mix456")]
        [InlineData("ABC123", "abc123")]
        [InlineData("São Paulo 2024!", "sao-paulo-2024")]
        [InlineData("José #1 Developer", "jose-1-developer")]
        [InlineData("Café & Restaurante - Centro", "cafe-restaurante-centro")]
        public void GenerateSlug_WithMixedContent_HandlesCorrectly(string input, string expected)
        {
            // Act
            var result = SlugHelper.GenerateSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Performance Tests

        [Fact]
        public void GenerateSlug_WithLargeString_CompletesInReasonableTime()
        {
            // Arrange
            var input = string.Join(" ", Enumerable.Repeat("São Paulo Café Açúcar", 1000));
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = SlugHelper.GenerateSlug(input);
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Slug generation should complete quickly even for large strings");
            Assert.NotEmpty(result);
        }

        #endregion

        #region Consistency Tests

        [Theory]
        [InlineData("Test Article", "test-article", "Another Test", "another-test")]
        [InlineData("Café", "cafe", "Maçã", "maca")]
        public void GenerateSlug_WithDifferentInputs_ProducesDifferentSlugs(string input1, string expected1, string input2, string expected2)
        {
            // Act
            var result1 = SlugHelper.GenerateSlug(input1);
            var result2 = SlugHelper.GenerateSlug(input2);

            // Assert
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
            Assert.NotEqual(result1, result2);
        }

        #endregion
    }
}
