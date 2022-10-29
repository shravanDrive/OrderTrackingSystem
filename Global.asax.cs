using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

namespace ATUClient
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            if ((Context.Session != null))
            {
                if (Session.IsNewSession)
                {
                    string strCookieHeader = System.Web.HttpContext.Current.Request.Headers["Cookie"];
                    if ((strCookieHeader != null))
                    {
                        if (strCookieHeader.ToUpper().IndexOf("ASP.NET_SESSIONID") >= 0)
                        {
                            //On timeouts, redirect user to timeout page.
                            if (Request.Cookies["ASP.NET_SessionId"] != null)
                            {
                                HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                                myCookie.Expires = DateTime.Now.AddDays(-1d);
                                Response.Cookies.Add(myCookie);
                            }
                            string Baseurl = HttpContext.Current.Request.Url.AbsoluteUri;

                            if (Baseurl.Contains("ATCPHome"))
                            {
                                Response.Redirect("ATCPTimeout.aspx");
                            }
                            else
                            {
                                Response.Redirect("Timeout.aspx");
                            }                            
                        }

                    }
                    else
                    {
                       // System.Web.HttpContext.Current.Response.Redirect("RedirectToBluemStem.html"); shr
                    }
                }
            }

        }

        void Session_End(object sender, EventArgs e)
        {            
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}
