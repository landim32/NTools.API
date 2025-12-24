using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NTools.Domain.Services;
using NTools.DTO.ChatGPT;
using NTools.DTO.Settings;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NTools.Tests.Domain.Services
{
    public class ChatGPTServiceTests
    {
        private readonly Mock<IOptions<ChatGPTSetting>> _mockChatGPTSettings;
        private readonly ChatGPTSetting _chatGPTSetting;
        private readonly HttpClient _httpClient;

        public ChatGPTServiceTests()
        {
            _chatGPTSetting = new ChatGPTSetting
            {
                ApiUrl = "https://api.openai.com/v1/chat/completions",
                ApiKey = "test-api-key-12345",
                Model = "gpt-3.5-turbo"
            };

            _mockChatGPTSettings = new Mock<IOptions<ChatGPTSetting>>();
            _mockChatGPTSettings.Setup(x => x.Value).Returns(_chatGPTSetting);

            _httpClient = new HttpClient();
        }

        private static ChatGPTResponse CreateTestChatGPTResponse(string content)
        {
            return new ChatGPTResponse
            {
                Id = "chatcmpl-123",
                Object = "chat.completion",
                Created = 1677652288,
                Model = "gpt-3.5-turbo",
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Index = 0,
                        Message = new ChatMessage
                        {
                            Role = "assistant",
                            Content = content
                        },
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
        }

        [Fact]
        public void Constructor_WithValidSettings_InitializesSuccessfully()
        {
            // Act
            var service = new ChatGPTService(_httpClient, _mockChatGPTSettings.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChatGPTService(null, _mockChatGPTSettings.Object));
        }

        [Fact]
        public void Constructor_WithNullSettings_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ChatGPTService(_httpClient, null));
        }

        [Fact]
        public async Task SendMessageAsync_WithValidMessage_ReturnsResponse()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var expectedResponse = CreateTestChatGPTResponse("Hello! How can I help you today?");
            var responseJson = JsonConvert.SerializeObject(expectedResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            // Act
            var result = await service.SendMessageAsync("Hello");

            // Assert
            Assert.Equal("Hello! How can I help you today?", result);
        }

        [Fact]
        public async Task SendConversationAsync_WithMultipleMessages_ReturnsResponse()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var expectedResponse = CreateTestChatGPTResponse("Based on our conversation, I can help with that.");
            var responseJson = JsonConvert.SerializeObject(expectedResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = "Hello" },
                new ChatMessage { Role = "assistant", Content = "Hi there!" },
                new ChatMessage { Role = "user", Content = "Can you help me?" }
            };

            // Act
            var result = await service.SendConversationAsync(messages);

            // Assert
            Assert.Equal("Based on our conversation, I can help with that.", result);
        }

        [Fact]
        public async Task SendRequestAsync_WithCustomRequest_ReturnsFullResponse()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var expectedResponse = CreateTestChatGPTResponse("Custom response");
            var responseJson = JsonConvert.SerializeObject(expectedResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                },
                Temperature = 0.7,
                MaxCompletionTokens = 1000
            };

            // Act
            var result = await service.SendRequestAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("chatcmpl-123", result.Id);
            Assert.Single(result.Choices);
            Assert.Equal("Custom response", result.Choices[0].Message.Content);
        }

        [Fact]
        public async Task SendRequestAsync_WhenApiReturnsErrorWithMessage_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var errorResponse = new ChatGPTErrorResponse
            {
                Error = new ChatGPTError
                {
                    Message = "Invalid API key",
                    Type = "invalid_request_error",
                    Code = "invalid_api_key"
                }
            };
            var errorJson = JsonConvert.SerializeObject(errorResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(errorJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.SendRequestAsync(request));

            Assert.Equal("Invalid API key", exception.Message);
        }

        [Fact]
        public async Task SendRequestAsync_WhenApiReturnsErrorWithoutMessage_ThrowsUnknownError()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var errorResponse = new ChatGPTErrorResponse
            {
                Error = new ChatGPTError
                {
                    Message = null
                }
            };
            var errorJson = JsonConvert.SerializeObject(errorResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.SendRequestAsync(request));

            Assert.Equal("Unknown error", exception.Message);
        }

        [Fact]
        public async Task SendRequestAsync_WhenApiReturnsNullError_ThrowsUnknownError()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("null")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.SendRequestAsync(request));

            Assert.Equal("Unknown error", exception.Message);
        }

        [Fact]
        public async Task SendMessageAsync_WhenResponseHasNoChoices_ReturnsEmptyString()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var response = new ChatGPTResponse
            {
                Id = "chatcmpl-123",
                Object = "chat.completion",
                Created = 1677652288,
                Model = "gpt-3.5-turbo",
                Choices = new List<Choice>(),
                Usage = new Usage()
            };
            var responseJson = JsonConvert.SerializeObject(response);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            // Act
            var result = await service.SendMessageAsync("Test");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task SendMessageAsync_WhenResponseIsNull_ReturnsEmptyString()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("null")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            // Act
            var result = await service.SendMessageAsync("Test");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("What is the weather today?")]
        [InlineData("Can you help me with programming?")]
        public async Task SendMessageAsync_WithVariousMessages_ReturnsResponse(string message)
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var expectedResponse = CreateTestChatGPTResponse($"Response to: {message}");
            var responseJson = JsonConvert.SerializeObject(expectedResponse);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new ChatGPTService(httpClient, _mockChatGPTSettings.Object);

            // Act
            var result = await service.SendMessageAsync(message);

            // Assert
            Assert.Equal($"Response to: {message}", result);
        }

        [Fact]
        public void ChatGPTRequest_Serialization_ProducesCorrectJson()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "user", Content = "Test" }
                },
                Temperature = 0.7,
                MaxCompletionTokens = 1000
            };

            // Act
            var json = JsonConvert.SerializeObject(request);
            var deserialized = JsonConvert.DeserializeObject<ChatGPTRequest>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(request.Model, deserialized.Model);
            Assert.Equal(request.Temperature, deserialized.Temperature);
            Assert.Equal(request.MaxCompletionTokens, deserialized.MaxCompletionTokens);
            Assert.Single(deserialized.Messages);
        }

        [Fact]
        public void ChatGPTResponse_Deserialization_ParsesCorrectly()
        {
            // Arrange
            var responseJson = @"{
                ""id"": ""chatcmpl-123"",
                ""object"": ""chat.completion"",
                ""created"": 1677652288,
                ""model"": ""gpt-3.5-turbo"",
                ""choices"": [{
                    ""index"": 0,
                    ""message"": {
                        ""role"": ""assistant"",
                        ""content"": ""Hello there!""
                    },
                    ""finish_reason"": ""stop""
                }],
                ""usage"": {
                    ""prompt_tokens"": 10,
                    ""completion_tokens"": 20,
                    ""total_tokens"": 30
                }
            }";

            // Act
            var response = JsonConvert.DeserializeObject<ChatGPTResponse>(responseJson);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("chatcmpl-123", response.Id);
            Assert.Equal("gpt-3.5-turbo", response.Model);
            Assert.Single(response.Choices);
            Assert.Equal("Hello there!", response.Choices[0].Message.Content);
            Assert.Equal(30, response.Usage.TotalTokens);
        }

        [Fact]
        public void ChatGPTErrorResponse_Deserialization_ParsesCorrectly()
        {
            // Arrange
            var errorJson = @"{
                ""error"": {
                    ""message"": ""Invalid API key"",
                    ""type"": ""invalid_request_error"",
                    ""param"": null,
                    ""code"": ""invalid_api_key""
                }
            }";

            // Act
            var errorResponse = JsonConvert.DeserializeObject<ChatGPTErrorResponse>(errorJson);

            // Assert
            Assert.NotNull(errorResponse);
            Assert.NotNull(errorResponse.Error);
            Assert.Equal("Invalid API key", errorResponse.Error.Message);
            Assert.Equal("invalid_request_error", errorResponse.Error.Type);
            Assert.Equal("invalid_api_key", errorResponse.Error.Code);
        }

        [Fact]
        public void ChatGPTSetting_ConfigurationValues_AreAccessible()
        {
            // Assert
            Assert.Equal("https://api.openai.com/v1/chat/completions", _chatGPTSetting.ApiUrl);
            Assert.Equal("test-api-key-12345", _chatGPTSetting.ApiKey);
            Assert.Equal("gpt-3.5-turbo", _chatGPTSetting.Model);
        }

        [Fact]
        public void ChatMessage_WithValidData_CreatesCorrectly()
        {
            // Arrange & Act
            var message = new ChatMessage
            {
                Role = "user",
                Content = "Hello, ChatGPT!"
            };

            // Assert
            Assert.Equal("user", message.Role);
            Assert.Equal("Hello, ChatGPT!", message.Content);
        }

        [Theory]
        [InlineData("user", "Hello")]
        [InlineData("assistant", "Hi there!")]
        [InlineData("system", "You are a helpful assistant")]
        public void ChatMessage_WithVariousRoles_CreatesCorrectly(string role, string content)
        {
            // Arrange & Act
            var message = new ChatMessage
            {
                Role = role,
                Content = content
            };

            // Assert
            Assert.Equal(role, message.Role);
            Assert.Equal(content, message.Content);
        }

        [Fact]
        public void Usage_WithTokenCounts_CalculatesCorrectly()
        {
            // Arrange & Act
            var usage = new Usage
            {
                PromptTokens = 50,
                CompletionTokens = 100,
                TotalTokens = 150
            };

            // Assert
            Assert.Equal(50, usage.PromptTokens);
            Assert.Equal(100, usage.CompletionTokens);
            Assert.Equal(150, usage.TotalTokens);
        }

        [Fact]
        public void ChatGPTRequest_WithDefaultValues_HasCorrectDefaults()
        {
            // Arrange & Act
            var request = new ChatGPTRequest();

            // Assert
            Assert.Equal(0.7, request.Temperature);
            Assert.Null(request.MaxCompletionTokens);
        }

        [Fact]
        public void ChatGPTRequest_WithMultipleMessages_SerializesCorrectly()
        {
            // Arrange
            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "system", Content = "You are helpful" },
                    new ChatMessage { Role = "user", Content = "Hello" },
                    new ChatMessage { Role = "assistant", Content = "Hi!" },
                    new ChatMessage { Role = "user", Content = "Help me" }
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(request);
            var deserialized = JsonConvert.DeserializeObject<ChatGPTRequest>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(4, deserialized.Messages.Count);
            Assert.Equal("system", deserialized.Messages[0].Role);
            Assert.Equal("user", deserialized.Messages[1].Role);
        }
    }
}
