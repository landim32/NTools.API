using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NTools.Domain.Services
{
    public class MailerSendService : IMailerSendService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<MailerSendSetting> _mailSettings;

        public MailerSendService(HttpClient httpClient, IOptions<MailerSendSetting> mailSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _mailSettings = mailSettings ?? throw new ArgumentNullException(nameof(mailSettings));
        }

        public async Task<bool> Sendmail(MailerInfo email)
        {
            email.From.Email = _mailSettings.Value.MailSender;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mailSettings.Value.ApiToken);
            var jsonContent = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_mailSettings.Value.ApiUrl, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorStr = await response.Content.ReadAsStringAsync();
                var msgErro = JsonConvert.DeserializeObject<MailerErrorInfo>(errorStr);
                if (msgErro != null && !string.IsNullOrEmpty(msgErro.Message))
                {
                    throw new InvalidOperationException(msgErro.Message);
                }
                throw new InvalidOperationException("Unknown error");
            }
            return await Task.FromResult(true);
        }
    }
}
