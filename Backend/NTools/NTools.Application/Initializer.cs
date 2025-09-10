using Core.Domain;
using Core.Domain.Cloud;
using Core.Domain.Repository;
using DB.Infra;
using DB.Infra.Context;
using DB.Infra.Repository;
using NTools.Domain.Impl.Core;
using NTools.Domain.Impl.Factory;
using NTools.Domain.Impl.Services;
using NTools.Domain.Interfaces.Core;
using NTools.Domain.Interfaces.Factory;
using NTools.Domain.Interfaces.Models;
using NTools.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using NTools.Domain;
using NTools.Client;
using LocalAuthHandler = NTools.Domain.LocalAuthHandler;
using System.Configuration;

namespace NTools.Application
{
    public static class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if(scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, string connection, bool scoped = true)
        {
            if (scoped)
                services.AddDbContext<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(connection));
            else
                services.AddDbContextFactory<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(connection));

            #region Infra
            injectDependency(typeof(NAuthContext), typeof(NAuthContext), services, scoped);
            injectDependency(typeof(IUnitOfWork), typeof(UnitOfWork), services, scoped);
            injectDependency(typeof(ILogCore), typeof(LogCore), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>), typeof(UserAddressRepository), services, scoped);
            injectDependency(typeof(IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>), typeof(UserDocumentRepository), services, scoped);
            injectDependency(typeof(IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>), typeof(UserPhoneRepository), services, scoped);
            injectDependency(typeof(IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory>), typeof(UserTokenRepository), services, scoped);
            injectDependency(typeof(IUserRepository<IUserModel, IUserDomainFactory>), typeof(UserRepository), services, scoped);
           
            #endregion

            #region Service
            injectDependency(typeof(IImageService), typeof(ImageService), services, scoped);
            injectDependency(typeof(IUserService), typeof(UserService), services, scoped);
            injectDependency(typeof(IMailerSendService), typeof(MailerSendService), services, scoped);
            #endregion

            injectDependency(typeof(IUserClient), typeof(UserClient), services, scoped);

            #region Factory
            injectDependency(typeof(IUserAddressDomainFactory), typeof(UserAddressDomainFactory), services, scoped);
            injectDependency(typeof(IUserDocumentDomainFactory), typeof(UserDocumentDomainFactory), services, scoped);
            injectDependency(typeof(IUserPhoneDomainFactory), typeof(UserPhoneDomainFactory), services, scoped);
            injectDependency(typeof(IUserTokenDomainFactory), typeof(UserTokenDomainFactory), services, scoped);
            injectDependency(typeof(IUserDomainFactory), typeof(UserDomainFactory), services, scoped);
            #endregion


            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, LocalAuthHandler>("BasicAuthentication", null);

        }
    }
}
