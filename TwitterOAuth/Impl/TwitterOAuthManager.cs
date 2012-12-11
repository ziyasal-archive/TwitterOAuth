using System;
using System.Collections.Specialized;
using System.Web;
using TwitterOAuth.Enum;
using TwitterOAuth.Helper;
using TwitterOAuth.Interface;

namespace TwitterOAuth.Impl
{
    public class TwitterOAuthManager : ITwitterOAuthManager
    {
        const string VERIFY_CREDENTIALS_URL = "http://twitter.com/account/verify_credentials.xml";
        const string UPDATE_URL = "http://twitter.com/statuses/update.xml";

        private readonly TwitterOAuthClient _twitterOAuthClient;

        public TwitterOAuthManager()
        {
            _twitterOAuthClient = new TwitterOAuthClient();
        }
        public bool CheckTwitterOAuthRequest(NameValueCollection queryString)
        {
            return queryString["oauth_token"] != null;
        }

        public string GetAuthorizationUrl()
        {
            return _twitterOAuthClient.AuthorizationLinkGet();
        }

        public TwitterBasicProfile Authenticate(NameValueCollection queryString)
        {
            string xml = null;
            _twitterOAuthClient.AccessTokenGet(queryString["oauth_token"], queryString["oauth_verifier"]);

            if (_twitterOAuthClient.TokenSecret.Length > 0)
            {
                xml = _twitterOAuthClient.OAuthWebRequest(RequestMethod.GET, VERIFY_CREDENTIALS_URL, String.Empty);
            }

            return XmlHelper.GetProfileFromXmlString(xml);
        }

        public string PostTweet()
        {
            //POST Test
            string result = _twitterOAuthClient.OAuthWebRequest(RequestMethod.POST, UPDATE_URL, "status=" + _twitterOAuthClient.UrlEncode("Hello @ziyasal - Testing the .NET twitterOAuth API"));
            return result;
        }

        public string GetUrlParameters()
        {
            return _twitterOAuthClient.GetAuthUrlParameters();
        }
    }
}