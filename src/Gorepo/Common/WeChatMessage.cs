using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Gorepo.Common
{
    public class WeChatMessage
    {
        public static object GetMessageInfo(string xmlMessage)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlMessage);

            string GetNodeValueString(string xpath)
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode(xpath);
                return xmlNode == null ? "" : xmlNode.InnerText;
            }

            var messageInfo = new
            {
                fromusername = GetNodeValueString("/msg/fromusername"),
                appmsg_title = GetNodeValueString("/msg/appmsg/title"),
                appmsg_des = GetNodeValueString("/msg/appmsg/des"),
                appmsg_template_id = GetNodeValueString("/msg/appmsg/template_id"),

                appinfo_appname = GetNodeValueString("/msg/appinfo/appname"),
                appinfo_version = GetNodeValueString("/msg/appinfo/version"),
                publisher_username = GetNodeValueString("/msg/appmsg/mmreader/publisher/username"),
                publisher_nickname = GetNodeValueString("/msg/appmsg/mmreader/publisher/nickname"),
                header_title = GetNodeValueString("/msg/appmsg/mmreader/template_header/title"),
                header_pub_time = GetNodeValueString("/msg/appmsg/mmreader/template_header/pub_time"), // 10 位时间戳

                // 收款金额
                detail_content_key_0 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/topline/key/word"),
                detail_content_value_0 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/topline/value/word"),

                // 收款方备注
                detail_content_key_1 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[1]/key/word"),
                detail_content_value_1 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[1]/value/word"),

                // 汇总
                detail_content_key_2 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[2]/key/word"),
                detail_content_value_2 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[2]/value/word"),

                // 备注
                detail_content_key_3 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[3]/key/word"),
                detail_content_value_3 = GetNodeValueString("/msg/appmsg/mmreader/template_detail/line_content/lines/line[3]/value/word")
            };

            return messageInfo;
        }

        public static async Task<WeChatMessageItem[]> GetWeChatMessageItemAsync(HttpClient httpClient)
        {
            return await JsonSerializer.DeserializeAsync<WeChatMessageItem[]>(
                await httpClient.GetStreamAsync("api/messages"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    public class WeChatMessageItem
    {
        public int Timestamp { get; set; }
        public string MessageId { get; set; }
        public string Message { get; set; }
    }
}
