using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using NTools.Domain.Impl.Services;
using NTools.Domain.Interfaces.Factory;
using NTools.Domain.Interfaces.Models;
using NTools.Domain.Interfaces.Services;
using NTools.DTO.MailerSend;
using NTools.DTO.User;

namespace NTools.Test;

public class UserServiceTests
{
    private UserService CreateService(
        Mock<IUserDomainFactory>? userFactory = null,
        Mock<IUserTokenDomainFactory>? tokenFactory = null,
        Mock<IUserPhoneDomainFactory>? phoneFactory = null,
        Mock<IUserAddressDomainFactory>? addrFactory = null,
        Mock<IMailerSendService>? mailer = null,
        Mock<IImageService>? image = null)
    {
        userFactory ??= new Mock<IUserDomainFactory>();
        tokenFactory ??= new Mock<IUserTokenDomainFactory>();
        phoneFactory ??= new Mock<IUserPhoneDomainFactory>();
        addrFactory ??= new Mock<IUserAddressDomainFactory>();
        mailer ??= new Mock<IMailerSendService>();
        image ??= new Mock<IImageService>();

        return new UserService(
            userFactory.Object,
            phoneFactory.Object,
            addrFactory.Object,
            tokenFactory.Object,
            mailer.Object,
            image.Object);
    }

    [Fact]
    public void LoginWithEmail_CallsModelAndReturnsResult()
    {
        var userModel = new Mock<IUserModel>();
        var loggedUser = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();

        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.LoginWithEmail("mail@test.com", "pwd", userFactory.Object))
                  .Returns(loggedUser.Object);

        var service = CreateService(userFactory: userFactory);

        var result = service.LoginWithEmail("mail@test.com", "pwd");

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.LoginWithEmail("mail@test.com", "pwd", userFactory.Object), Times.Once);
        Assert.Same(loggedUser.Object, result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void HasPassword_CallsModelAndReturnsResult(bool hasPassword)
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();

        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.HasPassword(5, userFactory.Object)).Returns(hasPassword);

        var service = CreateService(userFactory: userFactory);

        var result = service.HasPassword(5);

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.HasPassword(5, userFactory.Object), Times.Once);
        Assert.Equal(hasPassword, result);
    }

    [Fact]
    public void GetUserByEmail_CallsModelAndReturnsResult()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetByEmail("mail@test.com", userFactory.Object)).Returns(userModel.Object);

        var service = CreateService(userFactory: userFactory);

        var result = service.GetUserByEmail("mail@test.com");

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetByEmail("mail@test.com", userFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
    }

    [Fact]
    public void GetUserByToken_TokenNotFound_ReturnsNull()
    {
        var tokenModel = new Mock<IUserTokenModel>();
        var tokenFactory = new Mock<IUserTokenDomainFactory>();
        tokenFactory.Setup(f => f.BuildUserTokenModel()).Returns(tokenModel.Object);
        tokenModel.Setup(m => m.GetByToken("token123", tokenFactory.Object)).Returns((IUserTokenModel?)null);

        var userFactory = new Mock<IUserDomainFactory>();

        var service = CreateService(userFactory: userFactory, tokenFactory: tokenFactory);

        var result = service.GetUserByToken("token123");

        tokenFactory.Verify(f => f.BuildUserTokenModel(), Times.Once);
        tokenModel.Verify(m => m.GetByToken("token123", tokenFactory.Object), Times.Once);
        userFactory.Verify(f => f.BuildUserModel(), Times.Never);
        Assert.Null(result);
    }

    [Fact]
    public void GetUserByToken_TokenFound_ReturnsUser()
    {
        var tokenModel = new Mock<IUserTokenModel>();
        tokenModel.SetupGet(t => t.UserId).Returns(5);
        var tokenFactory = new Mock<IUserTokenDomainFactory>();
        tokenFactory.Setup(f => f.BuildUserTokenModel()).Returns(tokenModel.Object);
        tokenModel.Setup(m => m.GetByToken("token123", tokenFactory.Object)).Returns(tokenModel.Object);

        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetById(5, userFactory.Object)).Returns(userModel.Object);

        var service = CreateService(userFactory: userFactory, tokenFactory: tokenFactory);

        var result = service.GetUserByToken("token123");

        tokenFactory.Verify(f => f.BuildUserTokenModel(), Times.Once);
        tokenModel.Verify(m => m.GetByToken("token123", tokenFactory.Object), Times.Once);
        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetById(5, userFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
    }

    [Fact]
    public void GetByStripeId_CallsModelAndReturnsResult()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetByStripeId("stripe_123", userFactory.Object)).Returns(userModel.Object);

        var service = CreateService(userFactory: userFactory);

        var result = service.GetByStripeId("stripe_123");

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetByStripeId("stripe_123", userFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
    }

    [Fact]
    public void GetBySlug_CallsModelAndReturnsResult()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetBySlug("sluggy", userFactory.Object)).Returns(userModel.Object);

        var service = CreateService(userFactory: userFactory);

        var result = service.GetBySlug("sluggy");

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetBySlug("sluggy", userFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
    }

    [Fact]
    public void ListUsers_CallsModelAndReturnsResult()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var user1 = new Mock<IUserModel>().Object;
        var user2 = new Mock<IUserModel>().Object;
        var users = new List<IUserModel> { user1, user2 };
        userModel.Setup(m => m.ListUsers(2, userFactory.Object)).Returns(users);

        var service = CreateService(userFactory: userFactory);

        var result = service.ListUsers(2);

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.ListUsers(2, userFactory.Object), Times.Once);
        Assert.Collection(result,
            u => Assert.Same(user1, u),
            u => Assert.Same(user2, u));
    }

    [Fact]
    public void ChangePassword_HasPassword_VerifiesOldPasswordAndUpdates()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        userModel.Setup(m => m.HasPassword(10, userFactory.Object)).Returns(true);

        var existingUser = new Mock<IUserModel>();
        existingUser.SetupGet(u => u.UserId).Returns(10);
        existingUser.SetupGet(u => u.Email).Returns("mail@test.com");
        userModel.Setup(m => m.GetById(10, userFactory.Object)).Returns(existingUser.Object);
        userModel.Setup(m => m.LoginWithEmail("mail@test.com", "oldpwd", userFactory.Object)).Returns(existingUser.Object);

        var service = CreateService(userFactory: userFactory);

        service.ChangePassword(10, "oldpwd", "newpwd");

        userFactory.Verify(f => f.BuildUserModel(), Times.Exactly(2));
        userModel.Verify(m => m.HasPassword(10, userFactory.Object), Times.Once);
        userModel.Verify(m => m.GetById(10, userFactory.Object), Times.Once);
        userModel.Verify(m => m.LoginWithEmail("mail@test.com", "oldpwd", userFactory.Object), Times.Once);
        userModel.Verify(m => m.ChangePassword(10, "newpwd", userFactory.Object), Times.Once);
    }

    [Fact]
    public void ChangePassword_NoExistingPassword_UpdatesDirectly()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        userModel.Setup(m => m.HasPassword(10, userFactory.Object)).Returns(false);

        var existingUser = new Mock<IUserModel>();
        existingUser.SetupGet(u => u.UserId).Returns(10);
        existingUser.SetupGet(u => u.Email).Returns("mail@test.com");
        userModel.Setup(m => m.GetById(10, userFactory.Object)).Returns(existingUser.Object);

        var service = CreateService(userFactory: userFactory);

        service.ChangePassword(10, string.Empty, "newpwd");

        userModel.Verify(m => m.LoginWithEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IUserDomainFactory>()), Times.Never);
        userModel.Verify(m => m.ChangePassword(10, "newpwd", userFactory.Object), Times.Once);
    }

    [Theory]
    [InlineData(0, "127.0.0.1", "UA", "fp")]
    [InlineData(1, "", "UA", "fp")]
    [InlineData(1, "127.0.0.1", "", "fp")]
    [InlineData(1, "127.0.0.1", "UA", "")]
    public void CreateToken_InvalidArguments_ThrowsException(long userId, string ip, string ua, string fingerprint)
    {
        var service = CreateService();
        Assert.Throws<Exception>(() => service.CreateToken(userId, ip, ua, fingerprint));
    }

    [Fact]
    public void CreateToken_ValidArguments_CallsInsertAndReturnsModel()
    {
        var tokenModel = new Mock<IUserTokenModel>();
        tokenModel.SetupAllProperties();
        var tokenFactory = new Mock<IUserTokenDomainFactory>();
        tokenFactory.Setup(f => f.BuildUserTokenModel()).Returns(tokenModel.Object);
        tokenModel.Setup(m => m.Insert(tokenFactory.Object)).Returns(tokenModel.Object);

        var service = CreateService(tokenFactory: tokenFactory);

        var result = service.CreateToken(1, "127.0.0.1", "UA", "fp");

        tokenFactory.Verify(f => f.BuildUserTokenModel(), Times.Once);
        tokenModel.Verify(m => m.Insert(tokenFactory.Object), Times.Once);
        Assert.Equal(1, result.UserId);
        Assert.Equal("127.0.0.1", result.IpAddress);
        Assert.Equal("UA", result.UserAgent);
        Assert.Equal("fp", result.Fingerprint);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public void ChangePasswordUsingHash_UserNotFound_ThrowsException()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetByRecoveryHash("hash", userFactory.Object)).Returns((IUserModel?)null);

        var service = CreateService(userFactory: userFactory);

        Assert.Throws<Exception>(() => service.ChangePasswordUsingHash("hash", "newpass"));
    }

    [Fact]
    public void ChangePasswordUsingHash_Valid_CallsChangePassword()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var existingUser = new Mock<IUserModel>();
        existingUser.SetupGet(u => u.UserId).Returns(10);
        userModel.Setup(m => m.GetByRecoveryHash("hash", userFactory.Object)).Returns(existingUser.Object);

        var service = CreateService(userFactory: userFactory);

        service.ChangePasswordUsingHash("hash", "newpass");

        userModel.Verify(m => m.ChangePassword(10, "newpass", userFactory.Object), Times.Once);
    }

    [Fact]
    public async Task SendRecoveryEmail_UserNotFound_ThrowsException()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetByEmail("mail@test.com", userFactory.Object)).Returns((IUserModel?)null);

        var service = CreateService(userFactory: userFactory);

        await Assert.ThrowsAsync<Exception>(() => service.SendRecoveryEmail("mail@test.com"));
    }

    [Fact]
    public async Task SendRecoveryEmail_Valid_SendsEmailAndReturnsTrue()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var existingUser = new Mock<IUserModel>();
        existingUser.SetupGet(u => u.UserId).Returns(10);
        existingUser.SetupGet(u => u.Email).Returns("mail@test.com");
        existingUser.SetupGet(u => u.Name).Returns("Tester");
        userModel.Setup(m => m.GetByEmail("mail@test.com", userFactory.Object)).Returns(existingUser.Object);
        userModel.Setup(m => m.GenerateRecoveryHash(10, userFactory.Object)).Returns("hash123");

        var mailer = new Mock<IMailerSendService>();
        mailer.Setup(m => m.Sendmail(It.IsAny<MailerInfo>())).ReturnsAsync(true);

        var service = CreateService(userFactory: userFactory, mailer: mailer);

        var result = await service.SendRecoveryEmail("mail@test.com");

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetByEmail("mail@test.com", userFactory.Object), Times.Once);
        userModel.Verify(m => m.GenerateRecoveryHash(10, userFactory.Object), Times.Once);
        mailer.Verify(m => m.Sendmail(It.Is<MailerInfo>(mail =>
            mail.Subject == "[NoChainSwap] Password Recovery Email" &&
            mail.To.Count == 1 &&
            mail.To[0].Email == "mail@test.com" &&
            mail.Html.Contains("https://nochainswap.org/recoverypassword/hash123"))), Times.Once);
        Assert.True(result);
    }

    [Fact]
    public void Insert_ValidUser_InsertsModelAndRelatedData()
    {
        var userModel = new Mock<IUserModel>();
        userModel.SetupAllProperties();
        userModel.Setup(m => m.Insert(It.IsAny<IUserDomainFactory>())).Returns(userModel.Object);
        userModel.Setup(m => m.GetByEmail("mail@test.com", It.IsAny<IUserDomainFactory>())).Returns((IUserModel?)null);
        userModel.Setup(m => m.ExistSlug(It.IsAny<long>(), It.IsAny<string>())).Returns(false);
        userModel.Object.UserId = 42;

        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var phoneModel = new Mock<IUserPhoneModel>();
        phoneModel.SetupAllProperties();
        phoneModel.Setup(m => m.Insert(It.IsAny<IUserPhoneDomainFactory>())).Returns(phoneModel.Object);
        var phoneFactory = new Mock<IUserPhoneDomainFactory>();
        phoneFactory.Setup(f => f.BuildUserPhoneModel()).Returns(phoneModel.Object);

        var addrModel = new Mock<IUserAddressModel>();
        addrModel.SetupAllProperties();
        addrModel.Setup(m => m.Insert(It.IsAny<IUserAddressDomainFactory>())).Returns(addrModel.Object);
        var addrFactory = new Mock<IUserAddressDomainFactory>();
        addrFactory.Setup(f => f.BuildUserAddressModel()).Returns(addrModel.Object);

        var service = CreateService(userFactory: userFactory, phoneFactory: phoneFactory, addrFactory: addrFactory);

        var user = new UserInfo
        {
            Name = "Tester",
            Email = "mail@test.com",
            Password = "pwd",
            Phones = new List<UserPhoneInfo> { new UserPhoneInfo { Phone = "123456789" } },
            Addresses = new List<UserAddressInfo> { new UserAddressInfo { ZipCode = "12345678", Address = "Street", Complement = "Comp", Neighborhood = "Neigh", City = "City", State = "ST" } }
        };

        var result = service.Insert(user);

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetByEmail("mail@test.com", userFactory.Object), Times.Once);
        userModel.Verify(m => m.Insert(userFactory.Object), Times.Once);
        userModel.Verify(m => m.ChangePassword(42, "pwd", userFactory.Object), Times.Once);
        phoneFactory.Verify(f => f.BuildUserPhoneModel(), Times.Once);
        phoneModel.Verify(m => m.Insert(phoneFactory.Object), Times.Once);
        addrFactory.Verify(f => f.BuildUserAddressModel(), Times.Once);
        addrModel.Verify(m => m.Insert(addrFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
        Assert.Equal(42, phoneModel.Object.UserId);
        Assert.Equal("123456789", phoneModel.Object.Phone);
        Assert.Equal(42, addrModel.Object.UserId);
    }

    [Fact]
    public void Update_ValidUser_UpdatesModelAndRelatedData()
    {
        var userModel = new Mock<IUserModel>();
        userModel.SetupAllProperties();
        userModel.Setup(m => m.GetById(42, It.IsAny<IUserDomainFactory>())).Returns(userModel.Object);
        userModel.Setup(m => m.GetByEmail("mail@test.com", It.IsAny<IUserDomainFactory>())).Returns((IUserModel?)null);
        userModel.Setup(m => m.Update(It.IsAny<IUserDomainFactory>())).Returns(userModel.Object);
        userModel.Setup(m => m.ExistSlug(It.IsAny<long>(), It.IsAny<string>())).Returns(false);
        userModel.Object.UserId = 42;

        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var phoneModel = new Mock<IUserPhoneModel>();
        phoneModel.SetupAllProperties();
        phoneModel.Setup(m => m.Insert(It.IsAny<IUserPhoneDomainFactory>())).Returns(phoneModel.Object);
        phoneModel.Setup(m => m.DeleteAllByUser(42));
        var phoneFactory = new Mock<IUserPhoneDomainFactory>();
        phoneFactory.Setup(f => f.BuildUserPhoneModel()).Returns(phoneModel.Object);

        var addrModel = new Mock<IUserAddressModel>();
        addrModel.SetupAllProperties();
        addrModel.Setup(m => m.Insert(It.IsAny<IUserAddressDomainFactory>())).Returns(addrModel.Object);
        addrModel.Setup(m => m.DeleteAllByUser(42));
        var addrFactory = new Mock<IUserAddressDomainFactory>();
        addrFactory.Setup(f => f.BuildUserAddressModel()).Returns(addrModel.Object);

        var service = CreateService(userFactory: userFactory, phoneFactory: phoneFactory, addrFactory: addrFactory);

        var user = new UserInfo
        {
            UserId = 42,
            Name = "Tester",
            Email = "mail@test.com",
            Phones = new List<UserPhoneInfo> { new UserPhoneInfo { Phone = "123456789" } },
            Addresses = new List<UserAddressInfo> { new UserAddressInfo { ZipCode = "12345678", Address = "Street", Complement = "Comp", Neighborhood = "Neigh", City = "City", State = "ST" } }
        };

        var result = service.Update(user);

        userFactory.Verify(f => f.BuildUserModel(), Times.Once);
        userModel.Verify(m => m.GetById(42, userFactory.Object), Times.Once);
        userModel.Verify(m => m.GetByEmail("mail@test.com", userFactory.Object), Times.Once);
        userModel.Verify(m => m.Update(userFactory.Object), Times.Once);
        phoneFactory.Verify(f => f.BuildUserPhoneModel(), Times.Exactly(2));
        phoneModel.Verify(m => m.DeleteAllByUser(42), Times.Once);
        phoneModel.Verify(m => m.Insert(phoneFactory.Object), Times.Once);
        addrFactory.Verify(f => f.BuildUserAddressModel(), Times.Exactly(2));
        addrModel.Verify(m => m.DeleteAllByUser(42), Times.Once);
        addrModel.Verify(m => m.Insert(addrFactory.Object), Times.Once);
        Assert.Same(userModel.Object, result);
        Assert.Equal(42, phoneModel.Object.UserId);
        Assert.Equal("123456789", phoneModel.Object.Phone);
        Assert.Equal(42, addrModel.Object.UserId);
    }
}

