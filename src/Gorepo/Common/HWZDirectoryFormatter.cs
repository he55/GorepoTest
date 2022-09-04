using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Gorepo.Common
{
    /// <summary>
    /// Generates an HTML view for a directory.
    /// </summary>
    public class HWZDirectoryFormatter : IDirectoryFormatter
    {
        const string TextHtmlUtf8 = "text/html; charset=utf-8";
        const string PathLinkFormat = "<a href=\"{0}\">{1}</a> / ";
        const string FileOrDirectoryFormat =
@"<li>
    <a href=""{0}"" class=""icon icon-{1}"" title=""{2}"">
        <span class=""name"">{2}</span>
        <span class=""size"">{3}</span>
        <span class=""date"">{4}</span>
    </a>
</li>
";

        static readonly string s_directoryFormatterTemplate = File.ReadAllText("directory");
        static readonly HtmlEncoder _htmlEncoder = HtmlEncoder.Default;

        public async Task GenerateContentAsync(HttpContext context, IEnumerable<IFileInfo> contents)
        {
            PathString requestPath = context.Request.Path;

            StringBuilder stringBuilder = new StringBuilder(s_directoryFormatterTemplate);
            stringBuilder.Replace("{{RequestPath}}", requestPath);
            stringBuilder.Replace("{{PathLinkHtml}}", GetPathLinkHtml(requestPath));
            stringBuilder.Replace("{{FileOrDirectoryHtml}}", GetFileOrDirectoryHtml(requestPath, contents));

            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            context.Response.ContentType = TextHtmlUtf8;
            context.Response.ContentLength = bytes.Length;
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        static string GetPathLinkHtml(PathString pathString)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(PathLinkFormat, "/", "~");

            string cumulativePath = "/";
            foreach (string segment in pathString.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                cumulativePath = cumulativePath + segment + "/";
                stringBuilder.AppendFormat(
                    PathLinkFormat,
                    _htmlEncoder.Encode(cumulativePath),
                    _htmlEncoder.Encode(segment));
            }

            return stringBuilder.ToString();
        }

        static string GetFileOrDirectoryHtml(PathString pathString, IEnumerable<IFileInfo> contents)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!pathString.Equals("/"))
            {
                stringBuilder.AppendFormat(
                    FileOrDirectoryFormat,
                    pathString.Add("/../"),
                    "directory",
                    "..",
                    "",
                    "");
            }

            foreach (IFileInfo subdir in contents.Where(info => info.IsDirectory))
            {
                stringBuilder.AppendFormat(
                    FileOrDirectoryFormat,
                    pathString.Add($"/{subdir.Name}/"),
                    "directory",
                    _htmlEncoder.Encode(subdir.Name),
                    "",
                    subdir.LastModified.LocalDateTime.ToString());
            }

            foreach (IFileInfo file in contents.Where(info => !info.IsDirectory))
            {
                stringBuilder.AppendFormat(
                    FileOrDirectoryFormat,
                    pathString.Add($"/{file.Name}"),
                    "default",
                    _htmlEncoder.Encode(file.Name),
                    file.Length.ToString("n0"),
                    file.LastModified.LocalDateTime.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}
