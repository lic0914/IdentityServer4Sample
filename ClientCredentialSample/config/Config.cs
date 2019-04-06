using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientCredentialSample.config
{
    public class Config
    {
        /*范围（Scopes）用来定义系统中你想要保护的资源，比如 API。
由于当前演练中我们使用的是内存配置 —— 添加一个 API，你需要做的只是创建一个 ApiResource 类型的实例，并为它设置合适的属性。
         */
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "我的 API")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="client",
                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //客户端认证密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //客户端有权访问的范围

                    AllowedScopes ={"api1"}
                }
            };
        }
    }
}
