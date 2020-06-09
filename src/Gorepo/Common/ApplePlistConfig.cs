using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Gorepo
{
    public static class ApplePlistConfig
    {
        private static readonly Regex s_regex = new Regex("<\\?xml[\\s\\S]+</plist>");

        public static PlistDictionary GetPlistConfigModel(string plistString)
        {
            using (StringReader stringReader = new StringReader(plistString))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlistRoot));
                PlistRoot plistRoot = (PlistRoot)xmlSerializer.Deserialize(stringReader);
                return plistRoot.Dictionary;
            }
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
