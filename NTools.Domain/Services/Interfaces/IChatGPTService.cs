using NTools.DTO.ChatGPT;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTools.Domain.Services.Interfaces
{
    public interface IChatGPTService
    {
        Task<string> SendMessageAsync(string message);
        Task<string> SendConversationAsync(List<ChatMessage> messages);
        Task<ChatGPTResponse> SendRequestAsync(ChatGPTRequest request);
    }
}
