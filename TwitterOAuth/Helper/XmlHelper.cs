using System.Xml;

namespace TwitterOAuth.Helper
{
    public static class XmlHelper
    {
        public static TwitterBasicProfile GetProfileFromXmlString(string xml)
        {
            TwitterBasicProfile result = new TwitterBasicProfile();

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            XmlNodeList id = xdoc.SelectNodes("user/id");
            if (id != null) result.Id = id[0].InnerText;
            XmlNodeList name = xdoc.SelectNodes("user/name");
            if (name != null) result.Name = name[0].InnerText;
            XmlNodeList descr = xdoc.SelectNodes("user/description");
            if (descr != null)
                result.Description = descr[0].InnerText;
            XmlNodeList loc = xdoc.SelectNodes("user/location");
            if (loc != null)
                result.Location = loc[0].InnerText;
            XmlNodeList scrName = xdoc.SelectNodes("user/screen_name");
            if (scrName != null)
                result.ScreenName = scrName[0].InnerText;
            XmlNodeList url = xdoc.SelectNodes("user/url");
            if (url != null) result.Url = url[0].InnerText;

            return result;
        }
    }
}