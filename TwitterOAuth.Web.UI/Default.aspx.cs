using System;
using System.Web.UI;
using TwitterOAuth.Impl;
using TwitterOAuth.Interface;

namespace TwitterOAuth.Web.UI
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
         
        }

        protected void OnBtnTwitterLoginClick(object sender, EventArgs e)
        {

        }

        protected string GetUrlParamters()
        {
            ITwitterOAuthManager oAuthManager = new TwitterOAuthManager();
            return oAuthManager.GetUrlParameters();
        }
    }
}