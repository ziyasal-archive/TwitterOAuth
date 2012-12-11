using System.Collections.Specialized;
using System.Web;

namespace TwitterOAuth.Interface
{
    public interface ITwitterOAuthManager
    {
        bool CheckTwitterOAuthRequest(NameValueCollection queryString);
        string GetAuthorizationUrl();
        TwitterBasicProfile Authenticate(NameValueCollection request);
        string PostTweet();
        string GetUrlParameters();
    }
}