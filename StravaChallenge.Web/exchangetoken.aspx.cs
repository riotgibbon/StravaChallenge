using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IdentityManagement;
using IdentityManagement.Strava;
using Microsoft.Owin;

namespace App.StravaChallenge.Web
{
    public partial class exchangetoken : System.Web.UI.Page
    {
        private HttpClient httpClient = new HttpClient();

        protected async void Page_Load(object sender, EventArgs e)
        {
            string code = null;
            var query = Request.QueryString;
            IList<string> values = query.GetValues("code");
            if (values != null && values.Count == 1)
            {
                code = values[0];
            }
            else
            {
                Response.Redirect("~/", false);
            }
            var details = new StravaDetails
            {
                ClientId = StravaConfiguration.ClientId,
                ClientSecret = StravaConfiguration.ClientSecret,
                Host = StravaConfiguration.StravaHost
            };
            var connectToStrava = new ConnectToStrava(this.httpClient, details);
            string redirectUri = "/stats";
            var tokenResponse = await connectToStrava.SwapCodeForToken(code, redirectUri);
            if (tokenResponse != null && tokenResponse.athlete != null)
            {
                Response.Cookies.Add(new HttpCookie("token", tokenResponse.access_token));
                Response.Write("Hello " + tokenResponse.athlete.firstname);
            }
        }
    
    }
}