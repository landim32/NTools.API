using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NTools.DTO.Domain;
using NTools.DTO.Settings;
using NTools.DTO.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NTools.Client
{
    public class UserClient: IUserClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NAuthSetting> _nauthSetting;

        public UserClient(IOptions<NAuthSetting> nauthSetting)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
            _nauthSetting = nauthSetting;
        }

        public UserInfo? GetUserInSession(HttpContext httpContext)
        {
            if (httpContext.User.Claims.Count() > 0)
            {
                return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
            }
            return null;
        }

        private UserInfo? GetUserInfoFromJson(string json)
        {
            var result = JsonConvert.DeserializeObject<UserResult>(json);
            if (result == null)
            {
                throw new NullReferenceException("UserResult is null");
            }
            if (!result.Sucesso)
            {
                throw new Exception(result.Mensagem);
            }
            return result.User;
        }

        private bool GetBoolFromJson(string json)
        {
            var result = JsonConvert.DeserializeObject<UserResult>(json);
            if (result == null)
            {
                throw new NullReferenceException("UserResult is null");
            }
            if (!result.Sucesso)
            {
                throw new Exception(result.Mensagem);
            }
            return result.Sucesso;
        }

        public async Task<UserInfo?> GetMeAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getMe");
            response.EnsureSuccessStatusCode();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByIdAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getById/{userId}");
            response.EnsureSuccessStatusCode();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByToken/{token}");
            response.EnsureSuccessStatusCode();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByEmail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetBySlugAsync(string slug)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getBySlug/{slug}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> InsertAsync(UserInfo user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/insert", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> UpdateAsync(UserInfo user, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/update", content);
            response.EnsureSuccessStatusCode();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> LoginWithEmailAsync(LoginParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/loginWithEmail", content);
            response.EnsureSuccessStatusCode();
            return GetUserInfoFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> HasPasswordAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/hasPassword");
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordParam param, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePassword", content);
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> SendRecoveryMailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/sendRecoveryMail/{email}");
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePasswordUsingHash", content);
            response.EnsureSuccessStatusCode();
            return GetBoolFromJson(await response.Content.ReadAsStringAsync());
        }

        public async Task<IList<UserInfo>> ListAsync(int take)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/list/{take}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserListResult>(json);
            if (result == null)
            {
                throw new NullReferenceException("UserListResult is null");
            }
            if (!result.Sucesso)
            {
                throw new Exception(result.Mensagem);
            }
            return result.Users;
        }

        // Para upload de imagem, utilize MultipartFormDataContent
        public async Task<string> UploadImageUserAsync(Stream fileStream, string fileName, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/uploadImageUser", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<StringResult>(json);
            if (result == null)
            {
                throw new NullReferenceException("UserListResult is null");
            }
            if (!result.Sucesso)
            {
                throw new Exception(result.Mensagem);
            }
            return result.Value;
        }
    }

}
