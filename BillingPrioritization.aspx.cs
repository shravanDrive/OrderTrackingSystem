using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ATUClient
{
    public partial class BillingPrioritization : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

                if (Session != null && Session["FirstName"] != null)
                {
                    FirstNameText.Text = Session["FirstName"].ToString();
                    CurrentClientValueLabel.Text = Session["FirstName"].ToString();
                }
                if (Session != null && Session["LastName"] != null)
                {
                    LastNameText.Text = Session["LastName"].ToString();
                    CurrentClientValueLabel.Text = CurrentClientValueLabel.Text + " " + Session["LastName"].ToString();
                }
                if (Session != null && Session["ClientId"] != null)
                {
                    PopulateReferralDates();
                }
                //Akshat
                UpdateButton.Visible = false;
            }
            //To maintain values of these textboxes at postback
            if (ServicePhaseHiddenField.Value != null)
                ServicePhaseTextBox.Text = ServicePhaseHiddenField.Value;
            LoadServicePhaseWindowData();
        }

        //Akshat
        protected void Delete_BillingByPhaseRecord()
        {   try
            {SqlConnection con = null;
            SqlCommand cmd = null;
            int referralId = Convert.ToInt32(ReferralDateDropDownList.SelectedValue);
            if (Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose"))
            {
            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            //cmd = new SqlCommand("[USP_AddProductDeliveryInfo]", con);
            cmd = new SqlCommand("DELETE FROM BillingPrioritization WHERE ReferralId = @ReferralId",con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@ReferralId", referralId);
            cmd.ExecuteNonQuery();
            con.Close();
            }}
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;
                int serviceReturnValue = 0;
                int contactReturnValue = 0;
                Delete_BillingByPhaseRecord();
                string[] tempServiceHiddenField = ServicePhaseHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if ((Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !TPPDropDownList.SelectedValue.Equals("Choose") && tempServiceHiddenField != null && tempServiceHiddenField.Length > 0))
                {
                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    //Fetching service types and corresponding tasks

                    int referralId = Convert.ToInt32(ReferralDateDropDownList.SelectedValue);
                    int tppId = Convert.ToInt32(TPPDropDownList.SelectedValue);
                    con.Open();
                    #region Service and Tasks Region
                    foreach (string serviceString in tempServiceHiddenField)
                    {
                        string taskIdString = string.Empty;
                        String[] serviceTaskData = serviceString.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        int serviceId = 0;
                        if (Session != null && Session["ServiceTypes"] != null)
                            serviceId = ((Dictionary<int, string>)Session["ServiceTypes"]).FirstOrDefault(x => x.Value == serviceTaskData[0].Trim()).Key;

                        String[] tasks = serviceTaskData[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string task in tasks)
                        {
                            if (Session != null && Session["BillingTasks"] != null)
                            {
                                if (string.IsNullOrEmpty(taskIdString))
                                    taskIdString = ((Dictionary<int, string>)Session["BillingTasks"]).FirstOrDefault(x => x.Value == task.Trim()).Key + ",";
                                else
                                    taskIdString = taskIdString + ((Dictionary<int, string>)Session["BillingTasks"]).FirstOrDefault(x => x.Value == task.Trim()).Key + ",";
                            }
                        }

                        #region DataBase Calling
                        if (serviceId != 0 && !string.IsNullOrEmpty(taskIdString))
                        {


                            cmd = new SqlCommand("[usp_BillingPrioritization_Update]", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ReferralId", referralId);
                            cmd.Parameters.AddWithValue("@ServiceTypeId", serviceId);
                            cmd.Parameters.AddWithValue("@TaskIds", taskIdString);
                            cmd.Parameters.AddWithValue("@ThirdPartyPayerId", tppId);

                            SqlParameter returnValue = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            returnValue.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();

                            if (cmd.Parameters["ReturnValue"] != null)
                            {
                                serviceReturnValue = Convert.ToInt32(returnValue.Value);
                            }


                        }
                        #endregion DataBase Calling
                    }
                    #endregion Service and Tasks Region

                    #region ContactInfo Region

                    //Fetching service types and corresponding tasks
                    string[] tempContactInfoHiddenField = ContactInfoHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (tempContactInfoHiddenField.Length > 0)
                    {
                        //Fetching all Contact Info
                        string entityName = "";
                        string contactName = "";
                        string address = "";
                        string city = "";
                        string state = "";
                        string zip = "";
                        string phone = "";
                        string extension = "";
                        string fax = "";
                        string email = "";

                        if (tempContactInfoHiddenField[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            entityName = tempContactInfoHiddenField[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            contactName = tempContactInfoHiddenField[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            address = tempContactInfoHiddenField[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[3].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            city = tempContactInfoHiddenField[3].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[4].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            state = tempContactInfoHiddenField[4].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            zip = tempContactInfoHiddenField[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            phone = tempContactInfoHiddenField[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[7].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            extension = tempContactInfoHiddenField[7].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            fax = tempContactInfoHiddenField[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[9].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            email = tempContactInfoHiddenField[9].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();


                        cmd = new SqlCommand("[usp_ThirdPartyPayerContact_Update]", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ReferralId", referralId);
                        cmd.Parameters.AddWithValue("@ThirdPartyPayerId", tppId);
                        cmd.Parameters.AddWithValue("@EntityName", entityName);
                        cmd.Parameters.AddWithValue("@ContactName", contactName);

                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@Zip", zip);

                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Extension", extension);
                        cmd.Parameters.AddWithValue("@Fax", fax);
                        cmd.Parameters.AddWithValue("@Email", email);


                        SqlParameter returnValue1 = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                        returnValue1.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();

                        if (cmd.Parameters["ReturnValue"] != null)
                        {
                            contactReturnValue = Convert.ToInt32(returnValue1.Value);
                        }
                        con.Close();
                    #endregion ContactInfo Region

                    }
                    if (serviceReturnValue > 0 && contactReturnValue > 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully.')</script>");
                        Clear();
                    }
                    else if (serviceReturnValue == 0 && contactReturnValue != 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully. Duplicate service type records not saved.')</script>");
                        Clear();
                    }
                    else if (contactReturnValue == 0 && serviceReturnValue != 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully.')</script>");
                        Clear();
                    }
                    else
                    {
                        Response.Write("<script language=javascript>alert('Not Saved!!!. Issue!!!')</script>");
                    }
                }
                BStatusGridView.DataSource = null;

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }            
        }
        protected void CancelUpdate_Click(object sender, EventArgs e)
        {
            CancelUpdate();
        }
        protected void CancelUpdate()
        {
            Clear();
            UpdateButton.Visible = false;
            CancelUpdateButton.Visible = false;
            SaveButton.Visible = true;
        }
        protected void btnEditRefInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReferralDateDropDownList.SelectedItem != null && !ReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    UpdateButton.Visible = true;
                    CancelUpdateButton.Visible = true;
                    SaveButton.Visible = false;
                }
            }
            catch (SqlException se)
            {
                ErrorHandler(se);
            }
        }
        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                ContactInfoGridView.Visible = false;
                BStatusGridView.Visible = false;

                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;
                string txtLast = LastNameText.Text.Trim();
                string txtFirst = FirstNameText.Text.Trim();

                //Convert this block to SP 4/22/15 Daniel-----------------------------------------------------
                //string cmdString = null;
                //string lastCommand = "OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert select ClientID, CASE WHEN EFirstName IS NOT NULL THEN CAST(DecryptByKey(EFirstName) AS VARCHAR) ELSE '' END AS FirstName, CASE WHEN ELastName IS NOT NULL THEN CAST(DecryptByKey(ELastName) AS VARCHAR) ELSE '' END AS LastName, CASE WHEN ESSN IS NOT NULL THEN CAST(DecryptByKey(ESSN) AS VARCHAR) ELSE 'No SS#' END AS SSN from dbo.Clients where CAST(DecryptByKey(ELastName) AS VARCHAR)  like @last";
                //string firstCommand = "OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert select ClientID, CASE WHEN EFirstName IS NOT NULL THEN CAST(DecryptByKey(EFirstName) AS VARCHAR) ELSE '' END AS FirstName, CASE WHEN ELastName IS NOT NULL THEN CAST(DecryptByKey(ELastName) AS VARCHAR) ELSE '' END AS LastName, CASE WHEN ESSN IS NOT NULL THEN CAST(DecryptByKey(ESSN) AS VARCHAR) ELSE 'No SS#' END AS SSN from dbo.Clients where CAST(DecryptByKey(EFirstName) AS VARCHAR)  like @first";
                //string firstandlastCommand = "OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert select ClientID, CASE WHEN EFirstName IS NOT NULL THEN CAST(DecryptByKey(EFirstName) AS VARCHAR) ELSE '' END AS FirstName, CASE WHEN ELastName IS NOT NULL THEN CAST(DecryptByKey(ELastName) AS VARCHAR) ELSE '' END AS LastName, CASE WHEN ESSN IS NOT NULL THEN CAST(DecryptByKey(ESSN) AS VARCHAR) ELSE 'No SS#' END AS SSN from dbo.Clients where CAST(DecryptByKey(ELastName) AS VARCHAR)  like @last and CAST(DecryptByKey(EFirstName) AS VARCHAR)  like @first";


                //if (LastNameText.Text != "" && FirstNameText.Text != "")
                //{
                //    cmdString = firstandlastCommand;
                //    txtLast = '%' + txtLast + '%';
                //    txtFirst = '%' + txtFirst + '%';
                //}
                //else if (FirstNameText.Text != "")
                //{
                //    cmdString = firstCommand;
                //    txtFirst = '%' + txtFirst + '%';
                //}
                //else if (LastNameText.Text != "")
                //{
                //    cmdString = lastCommand;
                //    txtLast = '%' + txtLast + '%';
                //}
                //else if (LastNameText.Text == "" && FirstNameText.Text == "")
                //{
                //    cmdString = "OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert select ClientID, CASE WHEN EFirstName IS NOT NULL THEN CAST(DecryptByKey(EFirstName) AS VARCHAR) ELSE '' END AS FirstName, CASE WHEN ELastName IS NOT NULL THEN CAST(DecryptByKey(ELastName) AS VARCHAR) ELSE '' END AS LastName, CASE WHEN ESSN IS NOT NULL THEN CAST(DecryptByKey(ESSN) AS VARCHAR) ELSE 'No SS#' END AS SSN from dbo.Clients";
                //}
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                //cmd = new SqlCommand(cmdString, con);
                //Added 4/22/15 Daniel
                cmd = new SqlCommand("usp_ClientSearch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //----------------------------------------------

                cmd.Parameters.AddWithValue("@last", txtLast);
                cmd.Parameters.AddWithValue("@first", txtFirst);

                var searchDataTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(searchDataTable);

                GridView1.DataSource = searchDataTable;
                GridView1.DataBind();
                Session["SearchGridDataTable"] = searchDataTable;
                GridView1.Visible = true;
                GridView2.Visible = false;

                LoadSSN();

                if (GridView1.Rows.Count <= 0)
                {
                    NotFoundLabel.Visible = true;
                    //Remove the Label when no result 4/22/15 Daniel
                    CurrentClientLabel.Visible = true;
                    CurrentClientValueLabel.Visible = true;
                }
                else if (GridView1.Rows.Count == 1)
                {
                    Session["ClientID"] = null;
                    Session["FirstName"] = null;
                    Session["LastName"] = null;
                    Session["ClientInfoGrid"] = null;
                    Session["InsuranceInfoGrid"] = null;
                    Session["ServiceInfoDataTable"] = null;
                    Session["SSN"] = null;

                    string clientID = searchDataTable.Rows[0]["ClientID"].ToString();
                    Session["ClientID"] = clientID;
                    Session["SSN"] = searchDataTable.Rows[0]["SSN"].ToString();

                    PopulateSessionVariables(searchDataTable.Rows[0]["FirstName"].ToString(), searchDataTable.Rows[0]["LastName"].ToString());
                    GridView1.Visible = false;
                    // 10/14/2015 Daniel remove Not found message if displayed 
                    NotFoundLabel.Visible = false;

                    #region Populate Referral Dates

                    PopulateReferralDates();
                    SaveButton.Enabled = true;
                    CaseCloseMessageRow.Visible = false;

                    #endregion Populate Referral Dates
                }
                else
                    NotFoundLabel.Visible = false;
                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;
                int serviceReturnValue = 0;
                int contactReturnValue = 0;
                string[] tempServiceHiddenField = ServicePhaseHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if ((Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !TPPDropDownList.SelectedValue.Equals("Choose") && tempServiceHiddenField != null && tempServiceHiddenField.Length > 0))
                {
                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    //Fetching service types and corresponding tasks

                    int referralId = Convert.ToInt32(ReferralDateDropDownList.SelectedValue);
                    int tppId = Convert.ToInt32(TPPDropDownList.SelectedValue);

                    con.Open();
                    #region Service and Tasks Region
                    foreach (string serviceString in tempServiceHiddenField)
                    {
                        string taskIdString = string.Empty;
                        String[] serviceTaskData = serviceString.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        int serviceId = 0;
                        if (Session != null && Session["ServiceTypes"] != null)
                            serviceId = ((Dictionary<int, string>)Session["ServiceTypes"]).FirstOrDefault(x => x.Value == serviceTaskData[0].Trim()).Key;

                        String[] tasks = serviceTaskData[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string task in tasks)
                        {
                            if (Session != null && Session["BillingTasks"] != null)
                            {
                                if (string.IsNullOrEmpty(taskIdString))
                                    taskIdString = ((Dictionary<int, string>)Session["BillingTasks"]).FirstOrDefault(x => x.Value == task.Trim()).Key + ",";
                                else
                                    taskIdString = taskIdString + ((Dictionary<int, string>)Session["BillingTasks"]).FirstOrDefault(x => x.Value == task.Trim()).Key + ",";
                            }
                        }

                        #region DataBase Calling
                        if (serviceId != 0 && !string.IsNullOrEmpty(taskIdString))
                        {


                            cmd = new SqlCommand("[usp_BillingPrioritization_Insert]", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ReferralId", referralId);
                            cmd.Parameters.AddWithValue("@ServiceTypeId", serviceId);
                            cmd.Parameters.AddWithValue("@TaskIds", taskIdString);
                            cmd.Parameters.AddWithValue("@ThirdPartyPayerId", tppId);

                            SqlParameter returnValue = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            returnValue.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();

                            if (cmd.Parameters["ReturnValue"] != null)
                            {
                                serviceReturnValue = Convert.ToInt32(returnValue.Value);
                            }


                        }
                        #endregion DataBase Calling
                    }
                    #endregion Service and Tasks Region

                    #region ContactInfo Region

                    //Fetching service types and corresponding tasks
                    string[] tempContactInfoHiddenField = ContactInfoHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (tempContactInfoHiddenField.Length > 0)
                    {
                        //Fetching all Contact Info
                        string entityName = "";
                        string contactName = "";
                        string address = "";
                        string city = "";
                        string state = "";
                        string zip = "";
                        string phone = "";
                        string extension = "";
                        string fax = "";
                        string email = "";

                        if (tempContactInfoHiddenField[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            entityName = tempContactInfoHiddenField[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            contactName = tempContactInfoHiddenField[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            address = tempContactInfoHiddenField[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[3].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            city = tempContactInfoHiddenField[3].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[4].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            state = tempContactInfoHiddenField[4].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            zip = tempContactInfoHiddenField[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            phone = tempContactInfoHiddenField[6].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[7].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            extension = tempContactInfoHiddenField[7].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            fax = tempContactInfoHiddenField[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                        if (tempContactInfoHiddenField[9].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            email = tempContactInfoHiddenField[9].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();


                        cmd = new SqlCommand("[usp_ThirdPartyPayerContact_Insert]", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ReferralId", referralId);
                        cmd.Parameters.AddWithValue("@ThirdPartyPayerId", tppId);
                        cmd.Parameters.AddWithValue("@EntityName", entityName);
                        cmd.Parameters.AddWithValue("@ContactName", contactName);

                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@Zip", zip);

                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Extension", extension);
                        cmd.Parameters.AddWithValue("@Fax", fax);
                        cmd.Parameters.AddWithValue("@Email", email);


                        SqlParameter returnValue1 = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                        returnValue1.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();

                        if (cmd.Parameters["ReturnValue"] != null)
                        {
                            contactReturnValue = Convert.ToInt32(returnValue1.Value);
                        }
                        con.Close();
                    #endregion ContactInfo Region

                    }
                    if (serviceReturnValue > 0 && contactReturnValue > 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully.')</script>");
                        Clear();
                    }
                    else if (serviceReturnValue == 0 && contactReturnValue != 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully. Duplicate service type records not saved.')</script>");
                        Clear();
                    }
                    else if (contactReturnValue == 0 && serviceReturnValue != 0)
                    {
                        Response.Write("<script language=javascript>alert('Billing Prioritization Saved Successfully.')</script>");
                        Clear();
                    }
                    else
                    {
                        Response.Write("<script language=javascript>alert('Not Saved!!!. Issue!!!')</script>");
                    }
                }
                BStatusGridView.DataSource = null;

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("ServiceMilestone.aspx");
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Referral.aspx");
        }

        protected void NewSearch_Click(object sender, EventArgs e)
        {
            FirstNameText.Text = "";
            LastNameText.Text = "";

            Clear();
            CurrentClientValueLabel.Text = "";
            ContactInfoGridView.Visible = false;
            BStatusGridView.Visible = false;
            GridView2.Visible = false;
            GridView1.Visible = false;


            Session["ClientID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ClientInfoGrid"] = null;
            Session["InsuranceInfoGrid"] = null;
            Session["ServiceInfoDataTable"] = null;
            Session["SSN"] = null;
        }

        protected void grdSearch_Changing(object sender, GridViewPageEventArgs e)
        {
            if (Session != null && Session["SearchGridDataTable"] != null)
            {
                GridView1.DataSource = Session["SearchGridDataTable"];
                GridView1.PageIndex = e.NewPageIndex;
                GridView1.DataBind();

                LoadSSN();
            }
        }

        protected void gridSelect_Command(object sender, GridViewCommandEventArgs e)
        {
            #region Populate Grid
            Session["ClientID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ClientInfoGrid"] = null;
            Session["InsuranceInfoGrid"] = null;
            Session["ServiceInfoDataTable"] = null;
            Session["SSN"] = null;

            string str = e.CommandName.ToString();
            string clientId = string.Empty;
            int index = Convert.ToInt32(e.CommandArgument);
            if (str.Equals("Select"))
            {
                GridViewRow row = GridView1.Rows[index];
                DataTable dt = new DataTable();
                DataRow r1 = dt.NewRow();
                dt.Columns.Add("ClientId");
                dt.Columns.Add("First_Name");
                dt.Columns.Add("Last_Name");
                dt.Columns.Add("SS#");
                clientId = row.Cells[1].Text;
                Session["ClientID"] = clientId;
                Session["SSN"] = row.Cells[4].Text;
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    r1[i - 1] = row.Cells[i].Text;
                }

                dt.Rows.Add(r1);
                GridView2.DataSource = dt;
                GridView2.DataBind();
                GridView1.Visible = false;
                //GridView2.Visible = true;
                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
            }
            #endregion Populate Grid

            #region Populate Referral Dates

            PopulateReferralDates();
            SaveButton.Enabled = true;
            CaseCloseMessageRow.Visible = false;

            #endregion Populate Referral Dates

        }

        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            ContactInfoGridView.Visible = false;

            try
            {   
                //CancelUpdate();
                if (!ReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    GetBillingRecords();
                    if (CheckCaseClose())
                    {
                        SaveButton.Enabled = false;
                        CaseCloseMessageRow.Visible = true;
                    }
                    else
                    {   
                        btnEditRefInfo.Visible = true;
                        SaveButton.Enabled = true;
                        CaseCloseMessageRow.Visible = false;
                    }
                    if (!TPPDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                    {
                        fetchContactInformation();
                    }
                    loadReferralTabST();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void ThirdPartyPayer_Changed(object sender, EventArgs e)
        {
            if (Session != null && Session["tppAtypicalValues"] != null)
            {
                Dictionary<int, bool> tppDict = (Dictionary<int, bool>)Session["tppAtypicalValues"];
                ContactInfoGridView.Visible = false;
                if (TPPDropDownList.SelectedIndex != 0 && tppDict.ContainsKey(Convert.ToInt32(TPPDropDownList.SelectedValue)))
                {
                    if (tppDict[Convert.ToInt32(TPPDropDownList.SelectedValue)])
                    {
                        NewContactCheckboxRow.Visible = true;
                        ContactInfoRow.Visible = true;
                        if (!ReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                        {
                            fetchContactInformation();
                        }
                    }
                    else
                    {
                        NewContactCheckboxRow.Visible = false;
                        ContactInfoRow.Visible = false;
                        ContactCheckBox.Checked = false;
                    }
                }
                else
                {
                    NewContactCheckboxRow.Visible = false;
                    ContactInfoRow.Visible = false;
                    ContactCheckBox.Checked = false;
                }
            }
        }

        private void LoadData()
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                #region ServiceType

                cmd = new SqlCommand("select * from LookupTypeofService where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var serviceTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);

                Dictionary<int, string> servicetypeDict = new Dictionary<int, string>();
                foreach (DataRow row in serviceTypeTable.Rows)
                {
                    servicetypeDict.Add(Convert.ToInt32(row["ServiceId"]), row["TypeofService"].ToString().Trim());
                }
                Session["ServiceTypes"] = servicetypeDict;
                #endregion ServiceType

                #region Third Party Payer

                cmd = new SqlCommand("select * from LookUpThirdPartyPayer where Active = 1 order by SortOrder", con);
                cmd.CommandType = CommandType.Text;

                var tPPTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tPPTable);

                TPPDropDownList.DataSource = tPPTable;
                TPPDropDownList.DataTextField = "ThirdPartyPayer";
                TPPDropDownList.DataValueField = "ThirdPartyPayerId";
                TPPDropDownList.DataBind();

                TPPDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                Dictionary<int, bool> tppDict = new Dictionary<int, bool>();
                foreach (DataRow row in tPPTable.Rows)
                {
                    tppDict.Add(Convert.ToInt32(row["ThirdPartyPayerId"]), Convert.ToBoolean(row["Atypical"]));
                }
                Session["tppAtypicalValues"] = tppDict;

                #endregion Third Party Payer

                #region Referral Dates

                ReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Referral Dates

                #region TaskList

                cmd = new SqlCommand("select TaskId, BillingTask from dbo.LookUpTask where BillingTask IS NOT NULL", con);
                cmd.CommandType = CommandType.Text;

                var taskTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(taskTable);

                Dictionary<int, string> taskDict = new Dictionary<int, string>();
                foreach (DataRow row in taskTable.Rows)
                {
                    taskDict.Add(Convert.ToInt32(row["TaskId"]), row["BillingTask"].ToString().Trim());
                }
                Session["BillingTasks"] = taskDict;
                #endregion TaskList

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

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void LoadServicePhaseWindowData()
        {
            Dictionary<int, string> taskDict = (Dictionary<int, string>)Session["BillingTasks"];
            Dictionary<int, string> servicetypeDict = (Dictionary<int, string>)Session["ServiceTypes"];
            List<string> taskList = new List<string>();
            List<string> serviceList = new List<string>();
            
            foreach (string str in taskDict.Values)
            {
                taskList.Add(str);
            }
            
            foreach (string str in servicetypeDict.Values)
            {
                serviceList.Add(str);
            }

            int columns = taskDict.Count + 1;
            int rows = servicetypeDict.Count + 1;
            int taskLabelCount = 0;
            int serviceLabelCount = 0;

            #region UI Formation
            for (int i = 0; i < rows; i++)
            {
                TableRow row = new TableRow();
                for (int j = 0; j < 2; j++)
                {
                    TableCell cell = new TableCell();
                    //Adding blank cell at (0,0) position
                    if (i == 0 && j == 0)
                    {
                        Label label = new Label();
                        label.ID = "LabelX";
                        label.Text = string.Empty;
                        label.Visible = true;
                        cell.Controls.Add(label);
                        row.Cells.Add(cell);
                        ServicePhasesTable.Rows.Add(row);
                    }
                    //Code for adding all the tasks
                    else if (i == 0)
                    {
                        foreach (string str in taskList)
                        {
                            Label label = new Label();
                            label.ID = "TaskLabel" + taskLabelCount;
                            taskLabelCount++;
                            label.Text = str + "&nbsp&nbsp";
                            label.Visible = true;
                            cell.Controls.Add(label);
                        }
                        row.Cells.Add(cell);
                        ServicePhasesTable.Rows.Add(row);
                    }
                    // Code for adding all the service types
                    else if (j == 0)
                    {
                        Label label = new Label();
                        label.ID = "ServiceLabel" + serviceLabelCount;
                        label.Text = serviceList[serviceLabelCount];
                        label.Visible = true;
                        cell.Controls.Add(label);

                        row.Cells.Add(cell);
                        ServicePhasesTable.Rows.Add(row);
                    }
                    //Code for adding all the checkboxes
                    else if (j == 1)
                    {
                        CheckBoxList checkboxList = new CheckBoxList();
                        checkboxList.ID = "CheckBoxList" + serviceLabelCount;
                        serviceLabelCount++;
                        foreach (string str in taskList)
                        {
                            checkboxList.Items.Add("");
                        }
                        checkboxList.RepeatColumns = 4;
                        checkboxList.RepeatDirection = RepeatDirection.Horizontal;
                        checkboxList.Visible = true;
                        cell.Controls.Add(checkboxList);
                        row.Cells.Add(cell);
                        ServicePhasesTable.Rows.Add(row);
                    }
                }
            }
            #endregion UI Formation
            ServiceCount.Value = servicetypeDict.Count.ToString();

            loadReferralTabST();
        }

        private void PopulateReferralDates()
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                cmd = new SqlCommand("SELECT ReferralId, convert(char(10), ReferralDate, 101) ReferralDate FROM ClientReferral WHERE ClientID = @ClientId ORDER BY ReferralDate DESC", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));

                var refDatesTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refDatesTable);

                ReferralDateDropDownList.DataSource = refDatesTable;
                ReferralDateDropDownList.DataTextField = "ReferralDate";
                ReferralDateDropDownList.DataValueField = "ReferralId";
                ReferralDateDropDownList.DataBind();

                ReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

        }

        private void PopulateStatusData(DataTable statusDataTable)
        {
            Dictionary<string, int> dictServiceType = new Dictionary<string, int>();
            Dictionary<string, int> dictTPP = new Dictionary<string, int>();
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Label");
            tempTable.Columns.Add("Value");
            int initialRowValue = 0;
            for (int i = 0; i < statusDataTable.Rows.Count; i++)
            {
                if (dictServiceType.Count == 0 && !dictServiceType.ContainsKey(statusDataTable.Rows[i][1].ToString()))
                {
                    dictServiceType.Add(statusDataTable.Rows[i][1].ToString(), 1);
                    initialRowValue = i;
                }
                else if (dictServiceType.ContainsKey(statusDataTable.Rows[i][1].ToString()))
                    dictServiceType[statusDataTable.Rows[i][1].ToString()] = dictServiceType[statusDataTable.Rows[i][1].ToString()] + 1;
                else
                {
                    i--;
                    for (int j = initialRowValue; j < dictServiceType.ElementAt(0).Value + initialRowValue; j++)
                    {
                        if (!dictTPP.ContainsKey(statusDataTable.Rows[j][2].ToString()))
                            dictTPP.Add(statusDataTable.Rows[j][2].ToString(), 1);
                        else
                            dictTPP[statusDataTable.Rows[j][2].ToString()] = dictTPP[statusDataTable.Rows[j][2].ToString()] + 1;
                    }
                    DataRow dr = tempTable.NewRow();
                    dr[0] = "SERVICE TYPE";
                    dr[1] = dictServiceType.ElementAt(0).Key;
                    tempTable.Rows.Add(dr);
                    int tempRowValue = initialRowValue;
                    foreach (var value in dictTPP)
                    {
                        DataRow dr1 = tempTable.NewRow();
                        dr1[0] = value.Key;
                        string taskString = string.Empty;
                        int tempVar = 0;
                        for (int l = tempRowValue; l < value.Value + tempRowValue; l++)
                        {
                            if (string.IsNullOrEmpty(taskString))
                                taskString = statusDataTable.Rows[l][3].ToString();
                            else
                                taskString = taskString + ", " + statusDataTable.Rows[l][3].ToString();
                            tempVar = l + 1;
                        }
                        tempRowValue = tempVar;
                        dr1[1] = taskString;
                        tempTable.Rows.Add(dr1);
                    }
                    dictServiceType.Clear();
                    dictTPP.Clear();
                }
            }
            if (dictServiceType.Count != 0)
            {
                for (int j = initialRowValue; j < dictServiceType.ElementAt(0).Value + initialRowValue; j++)
                {
                    if (!dictTPP.ContainsKey(statusDataTable.Rows[j][2].ToString()))
                        dictTPP.Add(statusDataTable.Rows[j][2].ToString(), 1);
                    else
                        dictTPP[statusDataTable.Rows[j][2].ToString()] = dictTPP[statusDataTable.Rows[j][2].ToString()] + 1;
                }
                DataRow dr = tempTable.NewRow();
                dr[0] = "SERVICE TYPE";
                dr[1] = dictServiceType.ElementAt(0).Key;
                tempTable.Rows.Add(dr);
                int tempRowValue = initialRowValue;
                foreach (var value in dictTPP)
                {
                    DataRow dr1 = tempTable.NewRow();
                    dr1[0] = value.Key;
                    string taskString = string.Empty;
                    int tempVar = 0;
                    for (int l = tempRowValue; l < value.Value + tempRowValue; l++)
                    {
                        if (string.IsNullOrEmpty(taskString))
                            taskString = statusDataTable.Rows[l][3].ToString();
                        else
                            taskString = taskString + ", " + statusDataTable.Rows[l][3].ToString();
                        tempVar = l + 1;
                    }
                    tempRowValue = tempVar;
                    dr1[1] = taskString;
                    tempTable.Rows.Add(dr1);
                }
                dictServiceType.Clear();
                dictTPP.Clear();
            }
            BStatusGridView.DataSource = tempTable;
            BStatusGridView.DataBind();
            BStatusGridView.Visible = true;
        }

        private void GetBillingRecords()
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                cmd = new SqlCommand("SELECT BP.ReferralId, TOS.TypeofService,TPP.ThirdPartyPayer, BillingTask AS Display FROM dbo.BillingPrioritization BP JOIN dbo.LookUpTypeofService TOS ON BP.ServiceTypeId = TOS.ServiceId JOIN dbo.LookUpThirdPartyPayer TPP ON BP.ThirdPartyPayerId = TPP.ThirdPartyPayerId JOIN dbo.LookUpTask T ON BP.TaskId = T.TaskId WHERE ReferralId = @ReferralId ORDER BY TOS.TypeofService, TPP.ThirdPartyPayer, T.Display", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(ReferralDateDropDownList.SelectedValue));
                var statusDataTable = new DataTable();
                using (var sqlAdapter = new SqlDataAdapter(cmd)) sqlAdapter.Fill(statusDataTable);

                PopulateStatusData(statusDataTable);

                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void Clear()
        {
            ReferralDateDropDownList.SelectedIndex = 0;
            //ServiceTypeDropDownList.SelectedIndex = 0;
            //TasksCheckBoxList.SelectedIndex = -1;
            TPPDropDownList.SelectedIndex = 0;
            ServicePhaseHiddenField.Value = "";
            ServicePhaseTextBox.Text = "";
            ContactInfoHiddenField.Value = "";
            ContactInfoTextBox.Text = "";
            ContactCheckBox.Checked = false;
            NewContactCheckboxRow.Visible = false;
            ContactInfoRow.Visible = false;
            NotFoundLabel.Visible = false;

        }

        protected void LoadSSN()
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                string str = row.Cells[row.Cells.Count - 1].Text;
                if (!str.Equals("No SS#"))
                {
                    char[] chArr = new char[str.Length];
                    int j = 0;
                    for (int i = str.Length - 1; i >= 0; i--)
                    {
                        if (j <= 3)
                        {
                            chArr[i] = str[i];
                            j++;
                        }
                        else
                            chArr[i] = '*';
                    }
                    row.Cells[row.Cells.Count - 1].Text = new string(chArr);
                }
            }
        }

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            CurrentClientValueLabel.Text = firstName + " " + lastName;
        }

        private bool CheckCaseClose()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            //cmd = new SqlCommand("SELECT * FROM dbo.DetailReferralCaseClose WHERE ReferralId = @ReferralId", con);
            cmd = new SqlCommand("SELECT * FROM Service WHERE ReferralId = @ReferralId and TaskId=6 and actionid=1", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(ReferralDateDropDownList.SelectedValue));

            var refDatesTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refDatesTable);

            if (refDatesTable.Rows.Count > 0)
                return true;
            else
                return false;

        }

        private void fetchContactInformation()
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                cmd = new SqlCommand("SELECT TOP(1) EntityName,ContactName,[Address],City,[State],Zip,Phone,Extension,Fax,Email FROM dbo.ThirdPartyPayerContact WHERE ReferralId = @ReferralId AND ThirdPartyPayerId = @TPP ORDER BY InputDate DESC", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(ReferralDateDropDownList.SelectedValue));
                cmd.Parameters.AddWithValue("@TPP", Convert.ToInt32(TPPDropDownList.SelectedValue));
                var contactDataTable = new DataTable();
                using (var sqlAdapter = new SqlDataAdapter(cmd)) sqlAdapter.Fill(contactDataTable);

                //Populate data in the grid view
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                if (contactDataTable.Rows.Count > 0)
                {
                    DataRow headerRow = gridTable.NewRow();
                    headerRow[0] = "Contact Information";
                    gridTable.Rows.Add(headerRow);
                    for (int j = 0; j < contactDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();
                        row[0] = contactDataTable.Columns[j].ColumnName;
                        row[1] = contactDataTable.Rows[0][j].ToString();
                        gridTable.Rows.Add(row);
                    }
                    ContactInfoGridView.DataSource = gridTable;
                    ContactInfoGridView.DataBind();
                    ContactInfoGridView.Visible = true;
                }

                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void loadReferralTabST()
        {
            if (!ReferralDateDropDownList.SelectedValue.Equals("Choose"))
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("SELECT ReferralId,TypeofService FROM ClientReferralServiceType CRST JOIN LookUpTypeofService TOS ON TOS.ServiceId = CRST.ServiceTypeId WHERE ReferralId = @ReferralId", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@ReferralId", ReferralDateDropDownList.SelectedValue);

                var serviceTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);

                string labelValue = "";
                foreach (DataRow row in serviceTypeTable.Rows)
                {
                    if (string.IsNullOrEmpty(labelValue))
                        labelValue = row["TypeofService"].ToString();
                    else
                        labelValue += "," + row["TypeofService"];
                }
                ReferralTabSTLabel.Visible = true;
                ReferralTabSTLabel.Text = "Referral Tab's Service Types: ";
                if (string.IsNullOrEmpty(labelValue))
                    ReferralTabSTLabel.Text += "NONE";
                else
                    ReferralTabSTLabel.Text += labelValue;
                con.Close();
            }
        }

        /// <summary>
        /// Method to log errors and send error email
        /// </summary>
        /// <param name="ex"></param>
        private void ErrorHandler(Exception ex)
        {
            //Logging Errors
            string errorHandlerSource = Request.QueryString["handler"];
            if (errorHandlerSource == null)
            {
                errorHandlerSource = "Error Page";
            }
            ExceptionFile.ExceptionUtility.LogException(ex, errorHandlerSource);
            Response.Write("<script language=javascript>alert('Error!!! Email sent to the support team. Please contact for further info.')</script>");

            //Email Notification to the support team
            if (Session != null && Session["ATUID"] != null)
            {
                ExceptionFile.EmailUtility.SendEmail(Session["ATUID"].ToString(), ex.StackTrace);
            }
        }
    }
}
