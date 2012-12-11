using System;
using System.Xml;

namespace TwitterOAuth.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            TwitterBasicProfile profile = Method1();
            Console.WriteLine(profile.ScreenName);
        }

        private static TwitterBasicProfile Method1()
        {
            const string XML =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <user> <id>172649510</id> <name>Ziya SARIKAYA</name> <screen_name>ZiyaSAL</screen_name> <location>Turkey, Istanbul</location> <description>What is the difference? Curiosity, rebel and the most important HUMAN.. Mid - Senior .NET developer..</description> <profile_image_url>http://a0.twimg.com/profile_images/2195354421/uGb3sp944168-02_normal.jpg</profile_image_url> <profile_image_url_https>https://si0.twimg.com/profile_images/2195354421/uGb3sp944168-02_normal.jpg</profile_image_url_https> <url>http://www.ziyasal.com</url> <protected>true</protected> <followers_count>37</followers_count> <profile_background_color>dbdbdb</profile_background_color> <profile_text_color>ffffff</profile_text_color> <profile_link_color>217594</profile_link_color> <profile_sidebar_fill_color>bababa</profile_sidebar_fill_color> <profile_sidebar_border_color>b0b0b0</profile_sidebar_border_color> <friends_count>57</friends_count> <created_at>Fri Jul 30 06:45:28 +0000 2010</created_at> <favourites_count>26</favourites_count> <utc_offset>-18000</utc_offset> <time_zone>Quito</time_zone> <profile_background_image_url>http://a0.twimg.com/profile_background_images/324301271/Dark_Angel_Wallpapers_28__darkwallz.blogspot.com_.jpg</profile_background_image_url> <profile_background_image_url_https>https://si0.twimg.com/profile_background_images/324301271/Dark_Angel_Wallpapers_28__darkwallz.blogspot.com_.jpg</profile_background_image_url_https> <profile_background_tile>false</profile_background_tile> <profile_use_background_image>true</profile_use_background_image> <notifications>false</notifications> <geo_enabled>false</geo_enabled> <verified>false</verified> <following>false</following> <statuses_count>149</statuses_count> <lang>en</lang> <contributors_enabled>false</contributors_enabled> <follow_request_sent>false</follow_request_sent> <listed_count>0</listed_count> <show_all_inline_media>false</show_all_inline_media> <default_profile>false</default_profile> <default_profile_image>false</default_profile_image> <is_translator>false</is_translator> <status> <created_at>Thu May 24 06:53:08 +0000 2012</created_at> <id>205551991127162880</id> <text>@SynSynth Happy birthday</text> <source>web</source> <truncated>false</truncated> <favorited>false</favorited> <in_reply_to_status_id></in_reply_to_status_id> <in_reply_to_user_id>16808932</in_reply_to_user_id> <in_reply_to_screen_name>SynSynth</in_reply_to_screen_name> <retweet_count>0</retweet_count> <retweeted>false</retweeted> <geo/> <coordinates/> <place/> <contributors/> </status> </user>";

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(XML);
            TwitterBasicProfile profile = new TwitterBasicProfile();
            XmlNodeList id = xdoc.SelectNodes("user/id");
            if (id != null) profile.Id = id[0].InnerText;
            XmlNodeList name = xdoc.SelectNodes("user/name");
            if (name != null) profile.Name = name[0].InnerText;
            XmlNodeList descr = xdoc.SelectNodes("user/description");
            if (descr != null)
                profile.Description = descr[0].InnerText;
            XmlNodeList loc = xdoc.SelectNodes("user/location");
            if (loc != null)
                profile.Location = loc[0].InnerText;
            XmlNodeList scrName = xdoc.SelectNodes("user/screen_name");
            if (scrName != null)
                profile.ScreenName = scrName[0].InnerText;
            XmlNodeList url = xdoc.SelectNodes("user/url");
            if (url != null) profile.Url = url[0].InnerText;

            return profile;
        }
    }
}
