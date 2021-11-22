using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SessionAuthorized.Demo.Filters;
using SessionAuthorized.Demo.Filters.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SessionAuthorized.Demo
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
            //services.AddSession(); 传统鉴权
            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");//没登录跳到这个路径
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");//没权限跳到这个路径
                })
                
                ;
            services.AddAuthorization(optins =>
            {
                //增加授权策略
                optins.AddPolicy("customPolicy", polic =>
                {
                    polic.AddRequirements(new CustomAuthorizationRequirement("Policy01")
                       // ,new CustomAuthorizationRequirement("Policy02")
                        );
                });
            });

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // app.UseSession();传统鉴权
            app.UseRouting();

            app.UseAuthentication();//检测用户是否登录
            app.UseAuthorization(); //授权，检测有没有权限，是否能够访问功能


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
