using Microsoft.AspNetCore.Http;
using NTools.DTO.Domain;
using NTools.DTO.User;
using System.IO;
using System.Threading.Tasks;

namespace NTools.Client
{
    public interface IUserClient
    {
        UserInfo? GetUserInSession(HttpContext httpContext);
        Task<UserInfo?> GetMeAsync(string token);
        Task<UserInfo?> GetByIdAsync(long userId);
        Task<UserInfo?> GetByTokenAsync(string token);
        Task<UserInfo?> GetByEmailAsync(string email);
        Task<UserInfo?> GetBySlugAsync(string slug);
        Task<UserInfo?> InsertAsync(UserInfo user);
        Task<UserInfo?> UpdateAsync(UserInfo user, string token);
        Task<UserInfo?> LoginWithEmailAsync(LoginParam param);
        Task<bool> HasPasswordAsync(string token);
        Task<bool> ChangePasswordAsync(ChangePasswordParam param, string token);
        Task<bool> SendRecoveryMailAsync(string email);
        Task<bool> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param);
        Task<IList<UserInfo>> ListAsync(int take);
        Task<string> UploadImageUserAsync(Stream fileStream, string fileName, string token);
    }
}