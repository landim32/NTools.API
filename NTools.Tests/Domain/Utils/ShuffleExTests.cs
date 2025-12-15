using NTools.Domain.Utils;

namespace NTools.Tests.Domain.Utils
{
    public class ShuffleExTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void Shuffle_WithNonEmptyList_ModifiesTheList()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var originalList = new List<int>(list);

            // Act
            list.Shuffle();

            // Assert
            Assert.NotEqual(originalList, list);
        }

        [Fact]
        public void Shuffle_WithNonEmptyList_ContainsSameElements()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var originalList = new List<int>(list);

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(originalList.Count, list.Count);
            foreach (var item in originalList)
            {
                Assert.Contains(item, list);
            }
        }

        [Fact]
        public void Shuffle_WithEmptyList_DoesNotThrowException()
        {
            // Arrange
            var list = new List<int>();

            // Act & Assert
            var exception = Record.Exception(() => list.Shuffle());
            Assert.Null(exception);
        }

        [Fact]
        public void Shuffle_WithSingleElement_RemainsUnchanged()
        {
            // Arrange
            var list = new List<int> { 42 };
            var originalValue = list[0];

            // Act
            list.Shuffle();

            // Assert
            Assert.Single(list);
            Assert.Equal(originalValue, list[0]);
        }

        [Fact]
        public void Shuffle_WithTwoElements_ContainsBothElements()
        {
            // Arrange
            var list = new List<int> { 1, 2 };

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        #endregion

        #region Different Data Types Tests

        [Fact]
        public void Shuffle_WithStringList_WorksCorrectly()
        {
            // Arrange
            var list = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };
            var originalList = new List<string>(list);

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(originalList.Count, list.Count);
            foreach (var item in originalList)
            {
                Assert.Contains(item, list);
            }
        }

        [Fact]
        public void Shuffle_WithObjectList_WorksCorrectly()
        {
            // Arrange
            var list = new List<TestObject>
            {
                new TestObject { Id = 1, Name = "One" },
                new TestObject { Id = 2, Name = "Two" },
                new TestObject { Id = 3, Name = "Three" }
            };
            var originalIds = list.Select(x => x.Id).ToList();

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(originalIds.Count, list.Count);
            foreach (var id in originalIds)
            {
                Assert.Contains(list, x => x.Id == id);
            }
        }

        [Fact]
        public void Shuffle_WithDoubleList_WorksCorrectly()
        {
            // Arrange
            var list = new List<double> { 1.1, 2.2, 3.3, 4.4, 5.5 };
            var originalList = new List<double>(list);

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(originalList.Count, list.Count);
            foreach (var item in originalList)
            {
                Assert.Contains(item, list);
            }
        }

        #endregion

        #region Randomness Tests

        [Fact]
        public void Shuffle_CalledMultipleTimes_ProducesDifferentResults()
        {
            // Arrange
            var list1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var list2 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            list1.Shuffle();
            list2.Shuffle();

            // Assert - with high probability they should be different
            // Note: There's a very small chance they could be the same by random chance
            bool areAllDifferent = false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    areAllDifferent = true;
                    break;
                }
            }
            Assert.True(areAllDifferent, "Two shuffles of the same list should produce different orders (with high probability)");
        }

        [Fact]
        public void Shuffle_WithLargeList_DistributesElementsRandomly()
        {
            // Arrange
            var iterations = 100;
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var positionCounts = new Dictionary<int, int[]>();
            
            for (int i = 0; i < list.Count; i++)
            {
                positionCounts[list[i]] = new int[list.Count];
            }

            // Act - shuffle many times and track positions
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                var tempList = new List<int>(list);
                tempList.Shuffle();
                
                for (int i = 0; i < tempList.Count; i++)
                {
                    positionCounts[tempList[i]][i]++;
                }
            }

            // Assert - each element should appear in different positions
            // (not just always at the same position)
            foreach (var counts in positionCounts.Values)
            {
                var appearsInDifferentPositions = counts.Count(c => c > 0) > 1;
                Assert.True(appearsInDifferentPositions, "Elements should appear in different positions across shuffles");
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Shuffle_WithListOfIdenticalElements_WorksCorrectly()
        {
            // Arrange
            var list = new List<int> { 5, 5, 5, 5, 5 };

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(5, list.Count);
            Assert.All(list, item => Assert.Equal(5, item));
        }

        [Fact]
        public void Shuffle_WithNullElements_WorksCorrectly()
        {
            // Arrange
            var list = new List<string?> { "a", null, "b", null, "c" };
            var nullCount = list.Count(x => x == null);

            // Act
            list.Shuffle();

            // Assert
            Assert.Equal(5, list.Count);
            Assert.Equal(nullCount, list.Count(x => x == null));
        }

        [Fact]
        public void Shuffle_WithArray_WorksCorrectly()
        {
            // Arrange
            var array = new[] { 1, 2, 3, 4, 5 };
            var originalArray = array.ToArray();

            // Act
            ((IList<int>)array).Shuffle();

            // Assert
            Assert.Equal(originalArray.Length, array.Length);
            foreach (var item in originalArray)
            {
                Assert.Contains(item, array);
            }
        }

        [Fact]
        public void Shuffle_WithLargeList_CompletesInReasonableTime()
        {
            // Arrange
            var list = Enumerable.Range(1, 10000).ToList();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            list.Shuffle();
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Shuffle should complete quickly even for large lists");
            Assert.Equal(10000, list.Count);
        }

        #endregion

        #region Multiple Shuffles Tests

        [Fact]
        public void Shuffle_CalledConsecutively_ProducesValidResults()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var originalList = new List<int>(list);

            // Act
            list.Shuffle();
            list.Shuffle();
            list.Shuffle();

            // Assert
            Assert.Equal(originalList.Count, list.Count);
            foreach (var item in originalList)
            {
                Assert.Contains(item, list);
            }
        }

        #endregion

        #region Practical Scenarios

        [Fact]
        public void Shuffle_ForDeckOfCards_WorksCorrectly()
        {
            // Arrange - simulate a deck of cards
            var deck = new List<string>();
            var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
            var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            
            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    deck.Add($"{rank} of {suit}");
                }
            }
            
            var originalDeck = new List<string>(deck);

            // Act
            deck.Shuffle();

            // Assert
            Assert.Equal(52, deck.Count);
            Assert.Equal(originalDeck.Count, deck.Count);
            foreach (var card in originalDeck)
            {
                Assert.Contains(card, deck);
            }
        }

        [Fact]
        public void Shuffle_ForPlayerList_WorksCorrectly()
        {
            // Arrange
            var players = new List<string> { "Alice", "Bob", "Charlie", "Diana", "Eve" };
            var originalPlayers = new List<string>(players);

            // Act
            players.Shuffle();

            // Assert
            Assert.Equal(originalPlayers.Count, players.Count);
            foreach (var player in originalPlayers)
            {
                Assert.Contains(player, players);
            }
        }

        [Fact]
        public void Shuffle_ForQuizQuestions_WorksCorrectly()
        {
            // Arrange
            var questions = new List<QuizQuestion>
            {
                new QuizQuestion { Id = 1, Question = "What is 2+2?" },
                new QuizQuestion { Id = 2, Question = "What is the capital of France?" },
                new QuizQuestion { Id = 3, Question = "Who wrote Romeo and Juliet?" }
            };
            var originalIds = questions.Select(q => q.Id).ToList();

            // Act
            questions.Shuffle();

            // Assert
            Assert.Equal(3, questions.Count);
            foreach (var id in originalIds)
            {
                Assert.Contains(questions, q => q.Id == id);
            }
        }

        #endregion

        #region Type Safety Tests

        [Fact]
        public void Shuffle_PreservesTypeInformation()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            // Act
            list.Shuffle();

            // Assert
            Assert.IsType<List<int>>(list);
            Assert.All(list, item => Assert.IsType<int>(item));
        }

        #endregion

        #region Helper Classes

        private class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class QuizQuestion
        {
            public int Id { get; set; }
            public string Question { get; set; } = string.Empty;
        }

        #endregion
    }
}
