using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Authority
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
            #region IdentityServer config.

            var builder = services.AddIdentityServer();

            ////config testuser.
            //builder.AddTestUsers(new List<TestUser> { new TestUser { Username = "test", Password = "test", SubjectId = "test" } });

            #region memory config.

            //ClientCredentials GrantType config.
            builder.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                .AddInMemoryClients(IdentityConfiguration.Clients);

            builder.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources);

            #endregion


            #region identityserver4 entityframework config.

            //var cn = Configuration.GetConnectionString("DefaultConnection");
            //// this adds the config data from DB (clients, resources)
            //builder.AddOperationalStore(options =>
            //{
            //    options.ConfigureDbContext = b =>
            //        b.UseMySql(cn, dbOpts => dbOpts.MigrationsAssembly(GetType().Assembly.FullName));

            //    // this enables automatic token cleanup. this is optional.
            //    options.EnableTokenCleanup = true;
            //})
            //// this adds the operational data from DB (codes, tokens, consents)
            //.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = b =>
            //        b.UseMySql(cn, dbOpts => dbOpts.MigrationsAssembly(typeof(Startup).Assembly.FullName));
            //});

            #endregion

            builder.AddDeveloperSigningCredential();

            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //this.InitializeDatabase(app);

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityConfiguration.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityConfiguration.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in IdentityConfiguration.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
