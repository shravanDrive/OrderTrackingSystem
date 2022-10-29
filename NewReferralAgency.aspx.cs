using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace ATUClient
{
    public partial class NewReferralAgency : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            #region AgencyName Validation

            bool isAgencyName = false;
            if (String.IsNullOrEmpty(AgencyNameText.Text))
            {
                isAgencyName = false;
                AgencyErrorLabel.Visible = true;
            }
            else
            {
                isAgencyName = true;
                AgencyErrorLabel.Visible = false;
            }
            #endregion AgencyName Validation

            #region DB Call
            
            if (isAgencyName)
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("[usp_NewReferralAgency_Insert]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AgencyName", AgencyNameText.Text);

                if (ContactTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@ContactName", ContactTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@ContactName", System.DBNull.Value);

                if (AddressTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Address", System.DBNull.Value);

                if (CityTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@City", System.DBNull.Value);

                if (StateDropDownList.SelectedValue != "-1")
                    cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                else
                    cmd.Parameters.AddWithValue("@State", System.DBNull.Value);

                if (ZipTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Zip", System.DBNull.Value);

                if (PhoneTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Phone", PhoneTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Phone", System.DBNull.Value);

                if (FaxTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Fax", FaxTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Fax", System.DBNull.Value);

                if (EmailTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);

                if (TTYTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@TTY", TTYTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@TTY", System.DBNull.Value);


                SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                serviceId.Direction = ParameterDirection.ReturnValue;

                Session["AgencyName"] = AgencyNameText.Text;
                //Referral referral = new Referral(); referral.AgencyName = AgencyNameText.Text;

                Referral referral = (Referral)Session["ReferralObject"];
                referral.AgencyName = AgencyNameText.Text;
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(serviceId.Value) > 0)
                {
                    Response.Write("<script language=javascript>alert('New referral agency created.')</script>");
                    Clear();
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "closing", "window.close();", true);
                }
                con.Close();
            }
            #endregion DB Call
        }

        protected void LoadData()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            #region State
            cmd = new SqlCommand("select * from dbo.LookUpUSStates", con);
            cmd.CommandType = CommandType.Text;

            var stateTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(stateTable);

            StateDropDownList.DataSource = stateTable;
            StateDropDownList.DataTextField = "Name";
            StateDropDownList.DataValueField = "State";
            StateDropDownList.DataBind();

            StateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "-1"));

            #endregion State

            con.Close();
             
        }

        private void Clear()
        {

            AgencyNameText.Text = "";
            ContactTextBox.Text = "";
            AddressTextBox.Text = "";
            CityTextBox.Text = "";
            StateDropDownList.SelectedIndex = 0;
            ZipTextBox.Text = "";
            PhoneTextBox.Text = "";
            FaxTextBox.Text = "";
            EmailTextBox.Text = "";
            TTYTextBox.Text = "";

        }
    }
}
