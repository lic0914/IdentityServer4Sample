using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="mvc",
                    ClientName="Mvc Client",
                    ClientUri="http://localhost:5001",
                    LogoUri="",
                    Description="Description",
                    AllowRememberConsent=true,

                    AllowedGrantTypes=GrantTypes.Implicit,//隐式授权方式
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RequireConsent=true,//用户是否 需要 同意
                    RedirectUris={"http://localhost:5001/signin-oidc"},//客户端做授权地址
                    PostLogoutRedirectUris={"http://localhost:5001/signout-callback-oidc"},//客户端退出授权地址
                    //scopes that client has access to
                    AllowedScopes ={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                    }

                }
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","API Application")
            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
        /// <summary>
        /// 测试用户
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                     SubjectId="100000",
                     Username="lic",
                     Password="password",
                     
                }
            };
        }
    }
}
