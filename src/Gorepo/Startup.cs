using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gorepo
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
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRazorPages();

            services.AddSingleton<IDirectoryFormatter, HWZDirectoryFormatter>();

            services.AddDbContextPool<GorepoContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("sqlite")));

            services.AddHttpClient("wed", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration.GetValue<string>("App:wed"));
                httpClient.Timeout = TimeSpan.FromSeconds(3.0);
            });
            services.AddSingleton<WeChatService>();
            services.AddSingleton<OrderService>();

            // services.AddHostedService<Worker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            GorepoContext context)
        {
            context.Database.EnsureCreated();

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

            // app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = new HWZFileExtensionContentTypeProvider()
            });

            /*
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                Formatter = app.ApplicationServices.GetService<IDirectoryFormatter>()
            });
            */

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapGet("/status", async context =>
                {
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("/pull_order");
                });
            });
        }
    }
}
