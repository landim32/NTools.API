using Newtonsoft.Json;

namespace NTools.DTO.ChatGPT
{
    public class ChatGPTErrorResponse
    {
        [JsonProperty("error")]
        public ChatGPTError Error { get; set; }
    }

    public class ChatGPTError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("param")]
        public string Param { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
