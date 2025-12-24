using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NTools.ACL;
using NTools.DTO.ChatGPT;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NTools.Tests.ACL
{
    public class ChatGPTClientTests
    {
        private readonly Mock<IOptions<NToolSetting>> _mockSettings;
        private readonly NToolSetting _settings;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<ChatGPTClient>> _mockLogger;

        public ChatGPTClientTests()
        {
            _settings = new NToolSetting
            {
                ApiUrl = "https://api.example.com"
            };

            _mockSettings = new Mock<IOptions<NToolSetting>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = _mockHttpHandler.ToHttpClient();

            _mockLogger = new Mock<ILogger<ChatGPTClient>>();
        }

        #region SendMessageAsync - Success Tests

        [Fact]
        public async Task SendMessageAsync_WithValidMessage_ReturnsResponse()
        {
            // Arrange
            var message = "What is the capital of France?";
            var expectedResponse = "The capital of France is Paris.";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendMessageAsync(message);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData("Hello", "Hi there!")]
        [InlineData("What is AI?", "AI stands for Artificial Intelligence...")]
        [InlineData("Help me", "I'm here to help!")]
        public async Task SendMessageAsync_WithVariousMessages_ReturnsExpectedResponse(string message, string expectedResponse)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendMessageAsync(message);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendMessageAsync_WithLongMessage_ReturnsResponse()
        {
            // Arrange
            var message = new string('A', 1000);
            var expectedResponse = "I received your long message.";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendMessageAsync(message);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        #endregion

        #region SendMessageAsync - HTTP Request Tests

        [Fact]
        public async Task SendMessageAsync_MakesCorrectHttpPostRequest()
        {
            // Arrange
            var message = "Test message";
            var expectedUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .Expect(HttpMethod.Post, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject("Response"));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendMessageAsync(message);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task SendMessageAsync_SendsCorrectJsonContent()
        {
            // Arrange
            var message = "Test message";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .Expect(HttpMethod.Post, apiUrl)
                .With(request =>
                {
                    var content = request.Content.ReadAsStringAsync().Result;
                    var messageRequest = JsonConvert.DeserializeObject<ChatGPTMessageRequest>(content);
                    return messageRequest.Message == message;
                })
                .Respond("application/json", JsonConvert.SerializeObject("Response"));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendMessageAsync(message);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        #endregion

        #region SendMessageAsync - Error Handling Tests

        [Fact]
        public async Task SendMessageAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var message = "Test";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.SendMessageAsync(message));
        }

        [Fact]
        public async Task SendMessageAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var message = "Test";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.SendMessageAsync(message));
        }

        #endregion

        #region SendConversationAsync - Success Tests

        [Fact]
        public async Task SendConversationAsync_WithMultipleMessages_ReturnsResponse()
        {
            // Arrange
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = "Hello" },
                new ChatMessage { Role = "assistant", Content = "Hi there!" },
                new ChatMessage { Role = "user", Content = "How are you?" }
            };
            var expectedResponse = "I'm doing well, thank you!";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendConversationAsync(messages);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendConversationAsync_WithSingleMessage_ReturnsResponse()
        {
            // Arrange
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = "Hello" }
            };
            var expectedResponse = "Hi!";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendConversationAsync(messages);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendConversationAsync_WithSystemMessage_ReturnsResponse()
        {
            // Arrange
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "system", Content = "You are a helpful assistant" },
                new ChatMessage { Role = "user", Content = "Help me" }
            };
            var expectedResponse = "How can I help you?";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendConversationAsync(messages);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        #endregion

        #region SendConversationAsync - HTTP Request Tests

        [Fact]
        public async Task SendConversationAsync_MakesCorrectHttpPostRequest()
        {
            // Arrange
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = "Test" }
            };
            var expectedUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .Expect(HttpMethod.Post, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject("Response"));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendConversationAsync(messages);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        #endregion

        #region SendConversationAsync - Error Handling Tests

        [Fact]
        public async Task SendConversationAsync_WhenApiReturns400_ThrowsHttpRequestException()
        {
            // Arrange
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = "Test" }
            };
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.BadRequest);

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.SendConversationAsync(messages));
        }

        #endregion

        #region SendRequestAsync - Success Tests

        [Fact]
        public async Task SendRequestAsync_WithCustomRequest_ReturnsFullResponse()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-4o",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                },
                Temperature = 0.7,
                MaxCompletionTokens = 500
            };

            var expectedResponse = new ChatGPTResponse
            {
                Id = "chatcmpl-123",
                Object = "chat.completion",
                Created = 1677652288,
                Model = "gpt-4o",
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Index = 0,
                        Message = new ChatMessage { Role = "assistant", Content = "Response" },
                        FinishReason = "stop"
                    }
                },
                Usage = new Usage
                {
                    PromptTokens = 10,
                    CompletionTokens = 20,
                    TotalTokens = 30
                }
            };

            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendRequest";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendRequestAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("chatcmpl-123", result.Id);
            Assert.Equal("gpt-4o", result.Model);
            Assert.Single(result.Choices);
            Assert.Equal("Response", result.Choices[0].Message.Content);
        }

        [Fact]
        public async Task SendRequestAsync_WithDifferentTemperature_ReturnsResponse()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                },
                Temperature = 0.2
            };

            var expectedResponse = new ChatGPTResponse
            {
                Id = "chatcmpl-456",
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Message = new ChatMessage { Role = "assistant", Content = "Precise response" }
                    }
                }
            };

            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendRequest";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendRequestAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("chatcmpl-456", result.Id);
        }

        #endregion

        #region SendRequestAsync - HTTP Request Tests

        [Fact]
        public async Task SendRequestAsync_MakesCorrectHttpPostRequest()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-4o",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };
            var expectedUrl = $"{_settings.ApiUrl}/ChatGPT/sendRequest";

            var response = new ChatGPTResponse { Id = "test" };

            _mockHttpHandler
                .Expect(HttpMethod.Post, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(response));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendRequestAsync(request);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        #endregion

        #region SendRequestAsync - Error Handling Tests

        [Fact]
        public async Task SendRequestAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-4o",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendRequest";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.SendRequestAsync(request));
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task SendMessageAsync_WithEmptyMessage_SendsRequest()
        {
            // Arrange
            var message = "";
            var expectedResponse = "Please provide a message.";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendMessage";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendMessageAsync(message);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendConversationAsync_WithEmptyList_SendsRequest()
        {
            // Arrange
            var messages = new List<ChatMessage>();
            var expectedResponse = "No messages provided.";
            var apiUrl = $"{_settings.ApiUrl}/ChatGPT/sendConversation";

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var client = new ChatGPTClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendConversationAsync(messages);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendRequestAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var request = new ChatGPTRequest
            {
                Model = "gpt-4o",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };
            var apiUrl = $"{customSettings.ApiUrl}/ChatGPT/sendRequest";

            var response = new ChatGPTResponse { Id = "test" };

            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(response));

            var client = new ChatGPTClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendRequestAsync(request);

            // Assert
            Assert.NotNull(result);
        }

        #endregion
    }
}
