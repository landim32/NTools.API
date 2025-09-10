using DB.Infra.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NTools.Domain.Impl.Models;
using NTools.Domain.Impl.Services;
using NTools.Domain.Interfaces.Factory;
using NTools.Domain.Interfaces.Models;
using NTools.Domain.Interfaces.Services;
using NTools.DTO.Domain;
using NTools.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NTools.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IUserDomainFactory _userFactory;

        public UserController(IUserService userService, IImageService imageService, IUserDomainFactory userFactory)
        {
            _userService = userService;
            _imageService = imageService;
            _userFactory = userFactory;
        }

        [Authorize]
        [HttpPost("uploadImageUser")]
        public ActionResult<StringResult> UploadImageUser(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var fileName = _imageService.InsertToUser(file.OpenReadStream(), userSession.UserId);
                return new StringResult()
                {
                    Value = _imageService.GetImageUrl(fileName)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getMe")]
        [Authorize]
        public ActionResult<UserResult> GetMe()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getByToken/{token}")]
        public ActionResult<UserResult> GetByToken(string token)
        {
            try
            {
                var user = _userService.GetUserByToken(token);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getById/{userId}")]
        public ActionResult<UserResult> GetById(long userId)
        {
            try
            {
                var user = _userService.GetUserByID(userId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getByEmail/{email}")]
        public ActionResult<UserResult> GetByEmail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
                }
                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getBySlug/{slug}")]
        public ActionResult<UserResult> GetBySlug(string slug)
        {
            try
            {
                var user = _userService.GetBySlug(slug);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
                }
                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("insert")]
        public ActionResult<UserResult> Insert([FromBody] UserInfo user)
        {
            try
            {
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var newUser = _userService.Insert(user);
                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(newUser)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("update")]
        public ActionResult<UserResult> Update([FromBody] UserInfo user)
        {
            try
            {
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (userSession.UserId != user.UserId)
                {
                    throw new Exception("Only can update your user");
                }

                var updatedUser = _userService.Update(user);
                return new UserResult()
                {
                    User = _userService.GetUserInfoFromModel(updatedUser)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("loginWithEmail")]
        public ActionResult<UserTokenResult> LoginWithEmail([FromBody]LoginParam param)
        {
            try
            {
                var user = _userService.LoginWithEmail(param.Email, param.Password);
                if (user == null)
                {
                    return new UserTokenResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                var fingerprint = Request.Headers["X-Device-Fingerprint"].FirstOrDefault();
                var userAgent = Request.Headers["User-Agent"].FirstOrDefault();

                var ipAddr = Request.HttpContext.Connection?.RemoteIpAddress?.ToString();

                if (Request.Headers?.ContainsKey("X-Forwarded-For") == true)
                {
                    ipAddr = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                }
                var token = _userService.CreateToken(user.UserId, ipAddr, userAgent, fingerprint);
                return new UserTokenResult()
                {
                    Token = token.Token,
                    User = _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("hasPassword")]
        public ActionResult<StatusResult> HasPassword()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }
                return new StatusResult
                {
                    Sucesso = _userService.HasPassword(user.UserId),
                    Mensagem = "Password verify successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changePassword")]
        public ActionResult<StatusResult> ChangePassword([FromBody]ChangePasswordParam param)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                _userService.ChangePassword(user.UserId, param.OldPassword, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sendRecoveryMail/{email}")]
        public async Task<ActionResult<StatusResult>> SendRecoveryMail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new StatusResult
                    {
                        Sucesso = false,
                        Mensagem = "Email not exist"
                    };
                }
                await _userService.SendRecoveryEmail(email);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Recovery email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changePasswordUsingHash")]
        public ActionResult<StatusResult> ChangePasswordUsingHash([FromBody] ChangePasswordUsingHashParam param)
        {
            try
            {
                _userService.ChangePasswordUsingHash(param.RecoveryHash, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list/{take}")]
        public ActionResult<UserListResult> list(int take)
        {
            try
            {
                return new UserListResult
                {
                    Sucesso = true,
                    Users = _userService.ListUsers(take).Select(x => _userService.GetUserInfoFromModel(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
