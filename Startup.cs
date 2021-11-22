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
            //services.AddSession(); ��ͳ��Ȩ
            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");//û��¼�������·��
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");//ûȨ���������·��
                })
                
                ;
            services.AddAuthorization(optins =>
            {
                //������Ȩ����
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
            // app.UseSession();��ͳ��Ȩ
            app.UseRouting();

            app.UseAuthentication();//����û��Ƿ��¼
            app.UseAuthorization(); //��Ȩ�������û��Ȩ�ޣ��Ƿ��ܹ����ʹ���


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
