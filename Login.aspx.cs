using System;
using System.Web;
using System.Web.UI;

namespace ATUClient
{
    public partial class Login : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultFocus = "TextBox1";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //The below code create a session cookie that Bulestem creates in the production
            HttpCookie mycookie = new HttpCookie("ATUId");
            mycookie.Value = TextBox1.Text;
            mycookie.Domain = ".ahs.uic.edu";
            Response.Cookies.Add(mycookie);
            Response.Redirect("ATUClientInfo.aspx");
        }
    }
}
