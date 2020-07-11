using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = false;
                    //options.ExpireTimeSpan = TimeSpan.FromSeconds(1);
                })
                // AuthorizationCode
                .AddOpenIdConnect(options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Authority = "http://localhost:5000";

                    options.ClientId = "client_web_001";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.SaveTokens = true;
                });
                //// ClientCredentials
                //.AddOpenIdConnect(options =>
                //{
                //    options.RequireHttpsMetadata = false;

                //    options.Authority = "http://localhost:5000";

                //    options.ClientId = "client_web_002";

                //    options.SaveTokens = true;
                //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
