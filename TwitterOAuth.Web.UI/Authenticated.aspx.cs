using System;
using System.Web.UI;
using TwitterOAuth.Impl;
using TwitterOAuth.Interface;

namespace TwitterOAuth.Web.UI
{
    public partial class Authenticated : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ITwitterOAuthManager oAuthManager = new TwitterOAuthManager();

            if (oAuthManager.CheckTwitterOAuthRequest(Request.QueryString))
            {
                TwitterBasicProfile result = oAuthManager.Authenticate(Request.QueryString);
                txtHtmlArea.InnerHtml = Server.HtmlEncode(result.ScreenName);
            }
        }
    }
}