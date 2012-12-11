using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using TwitterOAuth.Base;
using TwitterOAuth.Enum;
using TwitterOAuth.Interface;

namespace TwitterOAuth.Impl
{
    public class TwitterOAuthClient : OAuthBase, ITwitterOAuthClient
    {
        public const string REQUEST_TOKEN = "http://twitter.com/oauth/request_token";
        public const string AUTHORIZE = "http://twitter.com/oauth/authorize";
        public const string ACCESS_TOKEN = "http://twitter.com/oauth/access_token";

        private string _consumerKey = "";
        private string _consumerSecret = "";
        private string _oauthVerifier = "";
        private string _token = "";
        private string _tokenSecret = "";
        private string _callbackUrl = "";

        #region Properties

        public string ConsumerKey
        {
            get
            {
                if (_consumerKey.Length == 0)
                {
                    _consumerKey = ConfigurationManager.AppSettings["Tw-ApiKey"];
                }
                return _consumerKey;
            }
            set { _consumerKey = value; }
        }

        public string ConsumerSecret
        {
            get
            {
                if (_consumerSecret.Length == 0)
                {
                    _consumerSecret = ConfigurationManager.AppSettings["Tw-AppSecret"];
                }
                return _consumerSecret;
            }
            set { _consumerSecret = value; }
        }
        public string CallBackUrl
        {
            get
            {
                if (_callbackUrl.Length == 0)
                {
                    _callbackUrl = ConfigurationManager.AppSettings["Tw-RedirectUri"];
                }
                return _callbackUrl;
            }
            set { _callbackUrl = value; }
        }

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        public string TokenSecret
        {
            get { return _tokenSecret; }
            set { _tokenSecret = value; }
        }


        public string OAuthVerifier
        {
            get { return _oauthVerifier; }
            set { _oauthVerifier = value; }
        }

        #endregion

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            string ret = null;

            string response = OAuthWebRequest(RequestMethod.GET, REQUEST_TOKEN, String.Empty);
            if (response.Length > 0)
            {
                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);

                if (qs["oauth_callback_confirmed"] != null)
                {
                    if (qs["oauth_callback_confirmed"] != "true")
                    {
                        throw new Exception("OAuth callback not confirmed.");
                    }
                }

                if (qs["oauth_token"] != null)
                {
                    ret = AUTHORIZE + "?oauth_token=" + qs["oauth_token"];
                }
            }
            return ret;
        }
        public string GetAuthUrlParameters()
        {
            string ret = null;

            string response = OAuthWebRequest(RequestMethod.GET, REQUEST_TOKEN, String.Empty);
            if (response.Length > 0)
            {
                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);

                if (qs["oauth_callback_confirmed"] != null)
                {
                    if (qs["oauth_callback_confirmed"] != "true")
                    {
                        throw new Exception("OAuth callback not confirmed.");
                    }
                }

                if (qs["oauth_token"] != null)
                {
                    ret = "oauth_token=" + qs["oauth_token"];
                }
            }
            return ret;
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        /// <param name="oauthVerifier">An oauth_verifier parameter is provided to the client either in the pre-configured callback URL</param>
        public void AccessTokenGet(string authToken, string oauthVerifier)
        {
            Token = authToken;
            OAuthVerifier = oauthVerifier;

            string response = OAuthWebRequest(RequestMethod.GET, ACCESS_TOKEN, String.Empty);

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    Token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    TokenSecret = qs["oauth_token_secret"];
                }
            }
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string OAuthWebRequest(RequestMethod method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";


            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == RequestMethod.POST || method == RequestMethod.DELETE)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];
                    }
                    if (url.IndexOf("?", StringComparison.Ordinal) > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            var uri = new Uri(url);

            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();

            //Generate Signature
            string sig = GenerateSignature(uri,
                                           ConsumerKey,
                                           ConsumerSecret,
                                           Token,
                                           TokenSecret,
                                           CallBackUrl,
                                           OAuthVerifier,
                                           method.ToString(),
                                           timeStamp,
                                           nonce,
                                           out outUrl,
                                           out querystring);

            querystring += "&oauth_signature=" + UrlEncode(sig);

            //Convert the querystring to postData
            if (method == RequestMethod.POST || method == RequestMethod.DELETE)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            ret = WebRequest(method, outUrl + querystring, postData);

            return ret;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(RequestMethod method, string url, string postData)
        {
            string responseData = "";

            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;

            if (webRequest != null)
            {
                webRequest.Method = method.ToString();
                webRequest.ServicePoint.Expect100Continue = false;
                //webRequest.UserAgent  = "Identify your application please.";
                //webRequest.Timeout = 20000;

                if (method == RequestMethod.POST || method == RequestMethod.DELETE)
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";

                    //POST the data.
                    using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        try
                        {
                            requestWriter.Write(postData);
                        }
                        finally
                        {
                            requestWriter.Close();
                        }
                    }
                }

                responseData = WebResponseGet(webRequest);
            }
            webRequest = null;

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = null;
            Stream responseStream = null;

            try
            {
                responseStream = webRequest.GetResponse().GetResponseStream();

                if (responseStream != null)
                {
                    responseReader = new StreamReader(responseStream);
                    responseData = responseReader.ReadToEnd();
                }
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (responseReader != null)
                {
                    responseReader.Close();
                }
            }

            return responseData;
        }
    }
}