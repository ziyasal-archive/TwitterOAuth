using System;
using System.Net;
using System.Security.Cryptography;
using TwitterOAuth.Enum;

namespace TwitterOAuth.Interface
{
    public interface ITwitterOAuthClient
    {
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string CallBackUrl { get; set; }
        string Token { get; set; }
        string TokenSecret { get; set; }
        string OAuthVerifier { get; set; }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        string AuthorizationLinkGet();

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>TThe pðarameter for request token, or a null string.</returns>
        string GetAuthUrlParameters();

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        /// <param name="oauthVerifier">An oauth_verifier parameter is provided to the client either in the pre-configured callback URL</param>
        void AccessTokenGet(string authToken, string oauthVerifier);

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        string OAuthWebRequest(RequestMethod method, string url, string postData);

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        string WebRequest(RequestMethod method, string url, string postData);

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        string WebResponseGet(HttpWebRequest webRequest);

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        string UrlEncode(string value);

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"> </param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <param name="normalizedUrl"> </param>
        /// <param name="normalizedRequestParameters"> </param>
        /// <param name="timeStamp"> </param>
        /// <returns>The signature base</returns>
        string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret,
                                     string callBackUrl, string oauthVerifier, string httpMethod,
                                     string timeStamp, string nonce, string signatureType,
                                     out string normalizedUrl, out string normalizedRequestParameters);

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash);

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"> </param>
        /// <param name="normalizedUrl"> </param>
        /// <param name="normalizedRequestParameters"> </param>
        /// <param name="timeStamp"> </param>
        /// <returns>A base64 string of the hash value</returns>
        string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                 string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod,
                                 string timeStamp, string nonce, out string normalizedUrl,
                                 out string normalizedRequestParameters);

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"> </param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="timeStamp"> </param>
        /// <param name="normalizedUrl"> </param>
        /// <param name="normalizedRequestParameters"> </param>
        /// <returns>A base64 string of the hash value</returns>
        string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                 string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod,
                                 string timeStamp, string nonce, SignatureTypes signatureType,
                                 out string normalizedUrl, out string normalizedRequestParameters);

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        string GenerateTimeStamp();

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        string GenerateNonce();
    }
}