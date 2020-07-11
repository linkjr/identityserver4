using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Authority
{
    public class IdentityConfiguration
    {
        public static IEnumerable<ApiScope> ApiScopes => new[] { new ApiScope("api") };

        public static IEnumerable<Client> Clients => new[]
        {
            new Client
            {
                ClientId = "client_web_001",
                ClientSecrets = {
                    new Secret("secret".Sha256())
                },
                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "http://localhost:5001/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    //IdentityServerConstants.StandardScopes.Email,
                    "api"
                }
            },
            //Implicit：（授权码）隐藏模式，没有授权码这个中间步骤
            //这种方式把令牌直接传给前端，是很不安全的。
            //因此，只能用于一些安全要求不高的场景，并且令牌的有效期必须非常短，通常就是会话期间（session）有效，浏览器关掉，令牌就失效了。
            new Client
            {
                ClientId = "client_web_002",
                ClientName = "MVC Client",
                AllowedGrantTypes = GrantTypes.Implicit,

                // where to redirect to after login
                RedirectUris = { "http://localhost:5001/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            },
            //ClientCredentials：客户端凭证模式，适用于第三方应用，而不是用户。主要用于保护API资源，没有用户参与
            new Client
            {
                ClientId = "client_api_001",
                ClientSecrets = {
                    new Secret("secret".Sha256())
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = ApiScopes.Select(m=>m.Name).ToArray()
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[] {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
    }
}
