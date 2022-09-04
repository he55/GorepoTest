namespace Gorepo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            FileServerOptions fileServerOptions = new FileServerOptions();
            fileServerOptions.StaticFileOptions.ContentTypeProvider = new HWZFileExtensionContentTypeProvider();
            fileServerOptions.DirectoryBrowserOptions.Formatter = new HWZDirectoryFormatter();
            fileServerOptions.EnableDirectoryBrowsing = true;
            app.UseFileServer(fileServerOptions);

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            const string CT = "text/plain; charset=utf-8";
            app.MapGet("/release", () => Results.File("release", CT));
            app.MapGet("/packages", () => Results.File("packages", CT));
            app.MapGet("/mobileconfig", () =>
            {
                string appUrl = app.Configuration.GetValue<string>("App:AppUrl");
                string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                string url = $"{appUrl}/tools/udid?t={timestamp}";

                string mobileConfig = AppleMobileConfig.MakeMobileConfig(url, timestamp);
                return Results.Text(mobileConfig, "application/x-apple-aspen-config");
            });

            app.Run();
        }
    }
}