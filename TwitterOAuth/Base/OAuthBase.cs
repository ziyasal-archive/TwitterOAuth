using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TwitterOAuth.Enum;

namespace TwitterOAuth.Base
{
    public class OAuthBase
    {
        protected const string O_AUTH_VERSION = "1.0";
        protected const string O_AUTH_PARAMETER_PREFIX = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        protected const string O_AUTH_CONSUMER_KEY_KEY = "oauth_consumer_key";
        protected const string O_AUTH_CALLBACK_KEY = "oauth_callback";
        protected const string O_AUTH_VERSION_KEY = "oauth_version";
        protected const string O_AUTH_SIGNATURE_METHOD_KEY = "oauth_signature_method";
        protected const string O_AUTH_SIGNATURE_KEY = "oauth_signature";
        protected const string O_AUTH_TIMESTAMP_KEY = "oauth_timestamp";
        protected const string O_AUTH_NONCE_KEY = "oauth_nonce";
        protected const string O_AUTH_TOKEN_KEY = "oauth_token";
        protected const string O_AUTH_TOKEN_SECRET_KEY = "oauth_token_secret";
        protected const string O_AUTH_VERIFIER_KEY = "oauth_verifier";

        protected const string HMACSHA1_SIGNATURE_TYPE = "HMAC-SHA1";
        protected const string PLAIN_TEXT_SIGNATURE_TYPE = "PLAINTEXT";
        protected const string RSASHA1_SIGNATURE_TYPE = "RSA-SHA1";

        protected Random RandomGenerator = new Random();

        protected string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(O_AUTH_PARAMETER_PREFIX))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public string UrlEncode(string value)
        {
            var result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int) symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

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
        public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret,
                                            string callBackUrl, string oauthVerifier, string httpMethod,
                                            string timeStamp, string nonce, string signatureType,
                                            out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(O_AUTH_VERSION_KEY, O_AUTH_VERSION));
            parameters.Add(new QueryParameter(O_AUTH_NONCE_KEY, nonce));
            parameters.Add(new QueryParameter(O_AUTH_TIMESTAMP_KEY, timeStamp));
            parameters.Add(new QueryParameter(O_AUTH_SIGNATURE_METHOD_KEY, signatureType));
            parameters.Add(new QueryParameter(O_AUTH_CONSUMER_KEY_KEY, consumerKey));

            if (!string.IsNullOrEmpty(callBackUrl))
            {
                parameters.Add(new QueryParameter(O_AUTH_CALLBACK_KEY, UrlEncode(callBackUrl)));
            }


            if (!string.IsNullOrEmpty(oauthVerifier))
            {
                parameters.Add(new QueryParameter(O_AUTH_VERIFIER_KEY, oauthVerifier));
            }

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(O_AUTH_TOKEN_KEY, token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

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
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                        string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod,
                                        string timeStamp, string nonce, out string normalizedUrl,
                                        out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, callBackUrl, oauthVerifier,
                                     httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out normalizedUrl,
                                     out normalizedRequestParameters);
        }

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
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                        string tokenSecret, string callBackUrl, string oauthVerifier, string httpMethod,
                                        string timeStamp, string nonce, SignatureTypes signatureType,
                                        out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, callBackUrl,
                                                                 oauthVerifier, httpMethod, timeStamp, nonce,
                                                                 HMACSHA1_SIGNATURE_TYPE, out normalizedUrl,
                                                                 out normalizedRequestParameters);

                    var hmacsha1 = new HMACSHA1();
                    hmacsha1.Key =
                        Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret),
                                                              string.IsNullOrEmpty(tokenSecret)
                                                                  ? ""
                                                                  : UrlEncode(tokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return RandomGenerator.Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }

    }
}