using Gorepo.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
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

            services.AddSingleton<IDirectoryFormatter, Html5DirectoryFormatter>();
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

            // app.UseHttpsRedirection();

            FileExtensionContentTypeProvider fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            fileExtensionContentTypeProvider.Mappings[".plist"] = "application/octet-stream";
            fileExtensionContentTypeProvider.Mappings[".ipa"] = "application/octet-stream";
            fileExtensionContentTypeProvider.Mappings[".mobileconfig"] = "application/x-apple-aspen-config";
            fileExtensionContentTypeProvider.Mappings[".bz2"] = "application/x-bzip2";
            fileExtensionContentTypeProvider.Mappings[".deb"] = "application/vnd.debian.binary-package";

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = fileExtensionContentTypeProvider,
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                Formatter = app.ApplicationServices.GetService<IDirectoryFormatter>()
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
