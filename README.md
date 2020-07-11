# IdentityServer4 Demo

> 环境：.net core 3.1

## 项目介绍

### Authority：授权服务

端口：`5000`

### Web：网站资源

端口：`5001`

采用AuthorizetionCode或Implicit授权方式，二者取其一

### Api：接口资源

端口：`5002`

认证：通过jwt认证，认证方式大概为添加请求头`Authorization: Bearer ACCESS_TOKEN`

授权

* ClientCredentials，通过访问授权服务的`/connect/token`即可拿到`access_token`

* AuthorizationCode，搭载Web项目进入授权服务登录后，可通过`HttpContext.GetTokenAsync("access_token")`拿到`access_token`

> *ClientCredentials模式适用于第三方应用，而不是用户。AuthorizationCode可以拿到用户信息*

### Client：请求客户端

主要用于对Api的资源请求，可以用第三方工具（postman等）替代

## 相关链接

[IdentityServer4官方文档](https://identityserver4.readthedocs.io/en/latest/index.html 'IdentityServer4官方文档')
