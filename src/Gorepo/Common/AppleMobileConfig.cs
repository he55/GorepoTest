using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Gorepo
{
    public static class AppleMobileConfig
    {
        static readonly Regex s_regex = new Regex("<\\?xml[\\s\\S]+</plist>");
        static readonly string s_mobileConfigTemplate = File.ReadAllText("mobileconfig");

        public static string MakeMobileConfig(string url, string challenge)
        {
            StringBuilder stringBuilder = new StringBuilder(s_mobileConfigTemplate);
            stringBuilder.Replace("{{URL}}", url);
            stringBuilder.Replace("{{Challenge}}", challenge);

            return stringBuilder.ToString();
        }

        public static bool TryGetPlistString(Stream stream, out string plistString)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                Match match = s_regex.Match(streamReader.ReadToEnd());
                plistString = match.Value;
                return match.Success;
            }
        }

        public static PlistDictionary GetPlistConfigModel(string plistString)
        {
            using (StringReader stringReader = new StringReader(plistString))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlistRoot));
                PlistRoot plistRoot = (PlistRoot)xmlSerializer.Deserialize(stringReader);
                return plistRoot.Dictionary;
            }
        }
    }

    [XmlRoot("plist")]
    public class PlistRoot
    {
        [XmlElement("dict")]
        public PlistDictionary Dictionary { get; set; } = null!;
    }

    public class PlistDictionary
    {
        [XmlElement("key")]
        public string[] Keys { get; set; } = null!;

        [XmlElement("string")]
        public string[] Values { get; set; } = null!;
    }
}
