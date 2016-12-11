TwitterOAuth
============
> Twitter OAuth library for .net apps,

[Nuget Package](https://nuget.org/packages/TwitterOAuth)

To Install:
```sh
Install-Package TwitterOAuth
```

## Documentation

Add following configuration to **appSettings** below **web.config** file
```xml
<add key="Tw-ApiKey" value="your consumer key here"/>
<add key="Tw-AppSecret" value="your consumer secret here"/>
<add key="Tw-RedirectUri" value="your redirected uri here"/>
```
1. Setup Authentication Link (Button)

**Markup**
```html
 <a href='http://twitter.com/oauth/authorize?<%=GetUrlParamters()%>'>Login with Twitter</a>
```
**Code Behind**
```csharp
protected string GetUrlParamters()
{
    ITwitterOAuthManager oAuthManager = new TwitterOAuthManager();
    return oAuthManager.GetUrlParameters();
 }
```
2. After Twitter redirected to referrer url,
**Redirected page code behind**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
     ITwitterOAuthManager oAuthManager = new TwitterOAuthManager();

     if (oAuthManager.CheckTwitterOAuthRequest(Request.QueryString))
     {
          TwitterBasicProfile result = oAuthManager.Authenticate(Request.QueryString);
          txtHtmlArea.InnerHtml = Server.HtmlEncode(result.ScreenName);
     }
}
```
**Redirected page markup**
```html
<div id="txtHtmlArea" runat="server"></div>
```
