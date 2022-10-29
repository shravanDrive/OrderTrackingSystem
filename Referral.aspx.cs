using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Globalization;
using System.Collections;

namespace ATUClient
{
    public partial class Referral : System.Web.UI.Page
    {
        private String agencyName;

        public String AgencyName
        {
            get { return agencyName; }
            set
            {
                agencyName = value;
                CurrentClientLabel.Text = agencyName;
                FirstNameText.Text = value;
                FirstNameText.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session != null && Session["AgencyName"] != null)
            //{
            //    CurrentClientLabel.Text = Session["AgencyName"].ToString();
            //}
            if (!IsPostBack)
            {
                updatepanel_MarkInactive.Visible = false;

                LoadData();
                RDateTextBox.Text = DateTime.Now.ToShortDateString();
                if (Session != null && Session["FirstName"] != null)
                {
                    FirstNameText.Text = Session["FirstName"].ToString();
                    CurrentClientValueLabel.Text = Session["FirstName"].ToString();
                    CurrentClientLabel.Visible = true;
                    CurrentClientValueLabel.Visible = true;
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

                UpdateButton.Visible = false;
                CancelUpdateButton.Visible = false;                

                //if (Session != null && Session["ClientGrid"] != null)
                //{
                //    GridView2.DataSource = (DataTable)Session["ClientGrid"];
                //    GridView2.DataBind();
                //    GridView2.Visible = true;
                //    OReferralDateRow.Visible = true;
                //    PopulateReferralDates();    
                //}
            }

            #region DBCall Session[IsAdmin]
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            string netId = Session["ATUID"].ToString();
            cmd = new SqlCommand("select Role from dbo.LookUpRole where roleid = (select [role] from dbo.LookupATUStaff where netid = @NetId and Active = 1)", con);
            cmd.Parameters.AddWithValue("@NetId", netId);

            var rolesTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(rolesTable);


            if (rolesTable.Rows != null && rolesTable.Rows.Count > 0 && rolesTable.Rows[0][0].ToString() == "Case Manager")
            {
                Session["IsAdmin"] = true;
            }
            else
            {
                Session["IsAdmin"] = false;
            }
            #endregion
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                CancelUpdate();
                updatepanel_MarkInactive.Visible = false;
                MarkInactiveCheckbox.Checked = false;
                MarkInactiveReason.SelectedIndex = 0;
                MarkInactiveReason.Visible = false;

                RefInfoGridGridView.DataSource = null;
                RefInfoGridCell.DataBind();
                PAgencyGridView.DataSource = null;
                PAgencyGridView.DataBind();
                SAgencyGridView.DataSource = null;
                SAgencyGridView.DataBind();

                SAgencyGridView.Visible = false;
                PAgencyGridView.Visible = false;
                RefInfoGridGridView.Visible = false;
                AgenciesInfoRow.Visible = false;

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
                OReferralDateRow.Visible = false;

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

                    PopulateReferralDates();
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
            #region ReferralDateValidation
            bool isValidDate = false;
            if (!String.IsNullOrEmpty(RDateTextBox.Text))
            {
                string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                DateTime d;
                isValidDate = DateTime.TryParseExact(RDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (isValidDate)
                {
                    RefDateValidationLabel.Visible = false;
                }
                else
                {
                    RefDateValidationLabel.Visible = true;
                }

            }

            #endregion ReferralDateValidation

            try
            {
                #region DB Call

                if (isValidDate)
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    SqlCommand cmd1 = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    if (MarkInactiveCheckbox.Checked)
                    {
                        if (MarkInactiveReason.Text != "Choose")
                        {
                            //Mark referral as inactive
                            con.Open();

                            cmd = new SqlCommand("[usp_InactiveReferral_Insert]", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);
                            cmd.Parameters.AddWithValue("@ReasonId", MarkInactiveReason.SelectedValue);
                            cmd.ExecuteNonQuery();


                            Clear();
                            con.Close();

                            MarkInactiveCheckbox.Checked = false;
                            MarkInactiveReason.SelectedIndex = 0;
                            updatepanel_MarkInactive.Visible = false;
                            OReferralDateDropDownList.SelectedIndex = 0;

                            Response.Write("<script language=javascript>alert('Client marked as Inactive.')</script>");
                        }
                    }
                    else
                    {
                        if (Session != null && Session["ClientID"] != null && !RMethodDropDownList.SelectedValue.Equals("Choose") && !RSourceDropDownList.SelectedValue.Equals("Choose"))
                        {
                            con.Open();

                            string serviceTypeIdString = string.Empty;
                            for (int i = 0; i < ServiceTypeCheckBoxList.Items.Count; i++)
                            {
                                if (ServiceTypeCheckBoxList.Items[i].Selected)
                                {
                                    if (string.IsNullOrEmpty(serviceTypeIdString))
                                        serviceTypeIdString = ServiceTypeCheckBoxList.Items[i].Value + ",";
                                    else
                                        serviceTypeIdString = serviceTypeIdString + ServiceTypeCheckBoxList.Items[i].Value + ",";
                                }
                            }

                            #region Client Referral
                            cmd = new SqlCommand("[usp_ClientReferral_Insert]", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ClientID", Convert.ToInt32(Session["ClientID"]));
                            cmd.Parameters.AddWithValue("@MethodID", RMethodDropDownList.SelectedValue);

                            cmd.Parameters.AddWithValue("@SourceID", RSourceDropDownList.SelectedValue);
                            cmd.Parameters.AddWithValue("@ReferralDate", Convert.ToDateTime(RDateTextBox.Text));
                            cmd.Parameters.AddWithValue("@ServiceTypeIds", serviceTypeIdString);

                            if (RSourceDropDownList.SelectedItem.ToString().Trim().Equals("IL-DHS-DRS"))
                            {
                                cmd.Parameters.AddWithValue("@DHSAgencyServiceID", DRSBureauDropDownList.SelectedValue);
                                //if (DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("HSP")) //BBS is added 11/3/2016
                                if (DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("HSP") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("BBS"))
                                {
                                    //Added 10/22/15 Daniel 
                                    if (string.IsNullOrEmpty(DRSRefIdTextBox.Text))
                                        cmd.Parameters.AddWithValue("@DRSReferralID", System.DBNull.Value);
                                    else
                                        cmd.Parameters.AddWithValue("@DRSReferralID", Convert.ToInt32(DRSRefIdTextBox.Text));
                                    //Added 10/22/15 Daniel 
                                    if (string.IsNullOrEmpty(RefClientIdTextBox.Text))
                                        cmd.Parameters.AddWithValue("@ReferralClientId", System.DBNull.Value);
                                    else
                                        cmd.Parameters.AddWithValue("@ReferralClientId", Convert.ToInt32(RefClientIdTextBox.Text));
                                }
                            }


                            SqlParameter referralId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            referralId.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();

                            if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(referralId.Value) > 0)
                            {
                                #region Client Agency Insert
                                foreach (ListItem item in AgenciesCheckBoxList.Items)
                                {
                                    cmd1 = new SqlCommand("[usp_ClientAgency_Insert]", con);
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    if (item.Selected && item.Text.Equals("Primary"))
                                    {
                                        if (!PAgencyDropDownList.SelectedValue.Equals("Choose"))
                                        {
                                            cmd1.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId.Value));
                                            cmd1.Parameters.AddWithValue("@AgencyId", PAgencyDropDownList.SelectedValue);
                                            cmd1.Parameters.AddWithValue("@RelationshipDegree", 1);
                                            if (PContactDropDownList.SelectedValue.Equals(String.Empty)
                                             || PContactDropDownList.SelectedValue.Equals("Choose"))
                                                cmd1.Parameters.AddWithValue("@AgencyContactId", 1);
                                            else
                                                cmd1.Parameters.AddWithValue("@AgencyContactId", PContactDropDownList.SelectedValue);
                                            cmd1.ExecuteNonQuery();
                                        }
                                    }
                                    else if (item.Selected && item.Text.Equals("Secondary"))
                                    {
                                        if (!SAgencyDropDownList.SelectedValue.Equals("Choose"))
                                        {
                                            cmd1.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId.Value));
                                            cmd1.Parameters.AddWithValue("@AgencyId", SAgencyDropDownList.SelectedValue);
                                            cmd1.Parameters.AddWithValue("@RelationshipDegree", 2);
                                            if (SContactDropDownList.SelectedValue.Equals(String.Empty)
                                              || SContactDropDownList.SelectedValue.Equals("Choose"))
                                                cmd1.Parameters.AddWithValue("@AgencyContactId", 1);
                                            else
                                                cmd1.Parameters.AddWithValue("@AgencyContactId", SContactDropDownList.SelectedValue);
                                            cmd1.ExecuteNonQuery();
                                        }
                                    }
                                }
                                #endregion Client Agency Insert

                                //#region Contact Details
                                //cmd1 = new SqlCommand("[usp_AgencyContactDetail_Insert]", con);
                                //cmd1.CommandType = CommandType.StoredProcedure;
                                //if (PNewContactCheckBox.Checked)
                                //{
                                //    cmd1.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId.Value));
                                //    cmd1.Parameters.AddWithValue("@AgencyId", PAgencyDropDownList.SelectedValue);
                                //    cmd1.Parameters.AddWithValue("@Contact", PNameTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Phone", PPhoneTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Email", PEmailTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Extension", PExtensionTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Fax", PFaxTextBox.Text);
                                //    cmd1.ExecuteNonQuery();
                                //}
                                //if (SNewContactCheckBox.Checked)
                                //{
                                //    cmd1.Parameters.Clear();
                                //    cmd1.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId.Value));
                                //    cmd1.Parameters.AddWithValue("@AgencyId", SAgencyDropDownList.SelectedValue);
                                //    cmd1.Parameters.AddWithValue("@Contact", SNameTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Phone", SPhoneTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Email", SEmailTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Extension", SExtensionTextBox.Text);
                                //    cmd1.Parameters.AddWithValue("@Fax", SFaxTextBox.Text);
                                //    cmd1.ExecuteNonQuery();
                                //}
                                //#endregion Contact Details
                            }
                            if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(referralId.Value) > 0)
                                Response.Write("<script language=javascript>alert('Client Record Saved Successfully.')</script>");

                            #endregion Client Referral

                            PopulateReferralDates();

                            Clear();

                            con.Close();
                        }
                    }
                }
                #endregion DB Call
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }


        //Added by Mrinal Dhawan --- 5/23/2017
        protected void Update_Click(object sender, EventArgs e)
        {

            #region ReferralDateValidation
            bool isValidDate = false;
            if (!String.IsNullOrEmpty(RDateTextBox.Text))
            {
                string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                DateTime d;
                isValidDate = DateTime.TryParseExact(RDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (isValidDate)
                {
                    RefDateValidationLabel.Visible = false;
                }
                else
                {
                    RefDateValidationLabel.Visible = true;
                }

            }

            #endregion ReferralDateValidation

            try
            {
                #region DB Call

                if (isValidDate)
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    //SqlCommand cmd1 = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    if (MarkInactiveCheckbox.Checked)
                    {
                        if (MarkInactiveReason.Text != "Choose")
                        {
                            //Mark referral as inactive
                            con.Open();

                            cmd = new SqlCommand("[usp_InactiveReferral_Insert]", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);
                            cmd.Parameters.AddWithValue("@ReasonId", MarkInactiveReason.SelectedValue);                            
                            cmd.ExecuteNonQuery();


                            Clear();
                            con.Close();

                            MarkInactiveCheckbox.Checked = false;
                            MarkInactiveReason.SelectedIndex = 0;
                            updatepanel_MarkInactive.Visible = false;
                            OReferralDateDropDownList.SelectedIndex = 0;

                            Response.Write("<script language=javascript>alert('Client marked as Inactive.')</script>");
                        }
                    }
                    else
                    {
                        if (Session != null && Session["ClientID"] != null && !RMethodDropDownList.SelectedValue.Equals("Choose") && !RSourceDropDownList.SelectedValue.Equals("Choose"))
                        {
                            con.Open();

                            string serviceTypeIdString = string.Empty;
                            for (int i = 0; i < ServiceTypeCheckBoxList.Items.Count; i++)
                            {
                                if (ServiceTypeCheckBoxList.Items[i].Selected)
                                {
                                    if (string.IsNullOrEmpty(serviceTypeIdString))
                                        serviceTypeIdString = ServiceTypeCheckBoxList.Items[i].Value + ",";
                                    else
                                        serviceTypeIdString = serviceTypeIdString + ServiceTypeCheckBoxList.Items[i].Value + ",";
                                }
                            }

                            #region Client Referral
                            cmd = new SqlCommand("[usp_ClientReferral_Update]", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(Session["ReferralId"]));
                            cmd.Parameters.AddWithValue("@ClientID", Convert.ToInt32(Session["ClientID"]));
                            cmd.Parameters.AddWithValue("@MethodID", RMethodDropDownList.SelectedValue);

                            cmd.Parameters.AddWithValue("@SourceID", RSourceDropDownList.SelectedValue);
                            cmd.Parameters.AddWithValue("@ReferralDate", Convert.ToDateTime(RDateTextBox.Text));
                            cmd.Parameters.AddWithValue("@ServiceTypeIds", serviceTypeIdString);

                            if (RSourceDropDownList.SelectedItem.ToString().Trim().Equals("IL-DHS-DRS"))
                            {
                                cmd.Parameters.AddWithValue("@DHSAgencyServiceID", DRSBureauDropDownList.SelectedValue);
                                //if (DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("HSP")) //BBS is added 11/3/2016
                                if (DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("HSP") || DRSBureauDropDownList.SelectedItem.ToString().Trim().Equals("BBS"))
                                {
                                    //Added 10/22/15 Daniel 
                                    if (string.IsNullOrEmpty(DRSRefIdTextBox.Text))
                                        cmd.Parameters.AddWithValue("@DRSReferralID", System.DBNull.Value);
                                    else
                                        cmd.Parameters.AddWithValue("@DRSReferralID", Convert.ToInt32(DRSRefIdTextBox.Text));
                                    //Added 10/22/15 Daniel 
                                    if (string.IsNullOrEmpty(RefClientIdTextBox.Text))
                                        cmd.Parameters.AddWithValue("@ReferralClientId", System.DBNull.Value);
                                    else
                                        cmd.Parameters.AddWithValue("@ReferralClientId", Convert.ToInt32(RefClientIdTextBox.Text));
                                }

                            }
                            #endregion Client Referral


                            //Agency Details
                            #region Client Agency Update




                            foreach (ListItem item in AgenciesCheckBoxList.Items)
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                if (item.Selected && item.Text.Equals("Primary"))
                                {
                                    if (!PAgencyDropDownList.SelectedValue.Equals("Choose"))
                                    {
                                        cmd.Parameters.AddWithValue("@PrimaryAgencyId", PAgencyDropDownList.SelectedValue);
                                        if (PContactDropDownList.SelectedValue.Equals(String.Empty)
                                         || PContactDropDownList.SelectedValue.Equals("Choose"))
                                            cmd.Parameters.AddWithValue("@PrimaryAgencyContactId", 0);
                                        else
                                            cmd.Parameters.AddWithValue("@PrimaryAgencyContactId", PContactDropDownList.SelectedValue);


                                        //cmd.Parameters.AddWithValue("@RelationshipDegree", 1);
                                        //cmd.ExecuteNonQuery();                                                                        
                                        //cmd.Parameters.AddWithValue("@PrimaryContact", PNameTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@PrimaryPhone", PPhoneTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@PrimaryEmail", PEmailTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@PrimaryExtension", PExtensionTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@PrimaryFax", PFaxTextBox.Text);
                                        //cmd1.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@PrimaryAgencyId", 0);
                                        cmd.Parameters.AddWithValue("@PrimaryAgencyContactId", 0);
                                    }
                                }
                                if (item.Selected && item.Text.Equals("Secondary"))
                                {
                                    if (!SAgencyDropDownList.SelectedValue.Equals("Choose"))
                                    {
                                        cmd.Parameters.AddWithValue("@SecondaryAgencyId", SAgencyDropDownList.SelectedValue);

                                        if (SContactDropDownList.SelectedValue.Equals(String.Empty)
                                         || SContactDropDownList.SelectedValue.Equals("Choose"))
                                            cmd.Parameters.AddWithValue("@SecondaryAgencyContactId", 0);
                                        else
                                            cmd.Parameters.AddWithValue("@SecondaryAgencyContactId", SContactDropDownList.SelectedValue);


                                        //cmd.Parameters.AddWithValue("@RelationshipDegree", 2);
                                        //cmd.ExecuteNonQuery();                                    
                                        //cmd.Parameters.AddWithValue("@SecondaryContact", SNameTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@SecondaryPhone", SPhoneTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@SecondaryEmail", SEmailTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@SecondaryExtension", SExtensionTextBox.Text);
                                        //cmd.Parameters.AddWithValue("@SecondaryFax", SFaxTextBox.Text);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@SecondaryAgencyId", 0);
                                        cmd.Parameters.AddWithValue("@SecondaryAgencyContactId", 0);
                                    }
                                }
                            }
                            #endregion Client Agency Update
                            var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            int returnVal = Convert.ToInt32(returnParameter.Value);
                            if (returnVal > 0)
                            {
                                //Response.Write("<script language=javascript>alert('Some error occurred while updating!! Please Contact the Admin for Assitance.')</script>");
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myfunction", "<script type='text/javascript'>$('#alertUpdateFailed').show();</script>", false);
                            }
                            else
                            {
                                Response.Write("<script language=javascript>alert('Client Record Updated Successfully.')</script>");
                            }
                        }
                    }

                    PopulateReferralDates();

                    Clear();

                    con.Close();


                    UpdateButton.Visible = false;
                    SaveButton.Visible = true;
                }
                #endregion DB Call
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        //Added by Mrinal Dhawan --- 5/23/2017
        protected void CancelUpdate_Click(object sender, EventArgs e)
        {
            CancelUpdate();
        }

        //Added by Mrinal Dhawan --- 5/23/2017 
        protected void CancelUpdate()
        {
            RMethodDropDownList.SelectedValue = "Choose";
            RSourceDropDownList.SelectedValue = "Choose";
            RDateTextBox.Text = String.Empty;

            DRSBureauDropDownList.SelectedIndex = 0;
            DRSRefIdTextBox.Text = String.Empty;
            RefClientIdTextBox.Text = String.Empty;
            DRSRefIdRow.Visible = false;//10/21/15 Daniel To make better flow
            RefClientIdRow.Visible = false;//10/21/15 Daniel To make better flow


            foreach (ListItem item in ServiceTypeCheckBoxList.Items)
            {
                item.Selected = false;
            }
            foreach (ListItem item in AgenciesCheckBoxList.Items)
            {
                item.Selected = false;
            }

            PAgencyDropDownList.SelectedIndex = 0;
            SAgencyDropDownList.SelectedIndex = 0;
            if (PContactDropDownList.Items.Count > 0)
                PContactDropDownList.SelectedIndex = 0;
            if (SContactDropDownList.Items.Count > 0)
                SContactDropDownList.SelectedIndex = 0;
            CheckboxSelection();

            UpdateButton.Visible = false;
            CancelUpdateButton.Visible = false;
            SaveButton.Visible = true;
        }

        //Added by Mrinal Dhawan --- 5/23/2017
        protected void btnEditRefInfo_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                if (OReferralDateDropDownList.SelectedItem != null && !OReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();

                    //cmd = new SqlCommand("usp_ClientReferralDetail", con);
                    cmd = new SqlCommand("usp_ClientReferralEditDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //---------------------------------------------------------------------------------------------

                    cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);

                    Session["ReferralId"] = OReferralDateDropDownList.SelectedValue;

                    var referralTable = new DataTable();
                    ArrayList serviceTypes = new ArrayList();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(referralTable);
                    //if (referralTable.Rows.Count > 0)
                    //{
                    //    ListItem ddlItem = new ListItem();
                    //    ddlItem = RMethodDropDownList.Items.FindByText(Convert.ToString(referralTable.Rows[0]["Referral Method"]));
                    //    //RMethodDropDownList.SelectedItem.Text = Convert.ToString(referralTable.Rows[0]["Referral Method"]);
                    //    RMethodDropDownList.SelectedValue = ddlItem.Value;
                    //    //RSourceDropDownList.SelectedItem.Text = Convert.ToString(referralTable.Rows[0]["Referral Source"]);

                    //    ddlItem = RSourceDropDownList.Items.FindByText(Convert.ToString(referralTable.Rows[0]["Referral Source"]));
                    //    //RMethodDropDownList.SelectedItem.Text = Convert.ToString(referralTable.Rows[0]["Referral Method"]);
                    //    RSourceDropDownList.SelectedValue = ddlItem.Value;

                    //    RDateTextBox.Text = OReferralDateDropDownList.SelectedItem.Text;
                    //    foreach (DataRow row in referralTable.Rows)
                    //    {
                    //        serviceTypes.Add(Convert.ToString(row["Service Type"]));
                    //    }
                    //    foreach (ListItem item in ServiceTypeCheckBoxList.Items)
                    //    {
                    //        item.Selected = serviceTypes.Contains(item.Text);
                    //    }
                    //}


                    if (referralTable.Rows.Count > 0)
                    {

                        RMethodDropDownList.SelectedValue = Convert.ToString(referralTable.Rows[0]["MethodId"]);
                        RSourceDropDownList.SelectedValue = Convert.ToString(referralTable.Rows[0]["SourceId"]);
                        RDateTextBox.Text = Convert.ToString(referralTable.Rows[0]["ReferralDate"]);
                        foreach (DataRow row in referralTable.Rows)
                        {
                            serviceTypes.Add(Convert.ToString(row["ServiceTypeId"]));
                        }
                        foreach (ListItem item in ServiceTypeCheckBoxList.Items)
                        {
                            item.Selected = serviceTypes.Contains(item.Value);
                        }

                        if (RSourceDropDownList.SelectedItem.ToString().Trim().Equals("IL-DHS-DRS"))
                        {
                            DRSBureauRow.Visible = true;
                            //if (!DRSBureauDropDownList.SelectedItem.Value.Equals("0"))//10/21/15 Daniel To make better flow
                            //    DRSBureauDropDownList.SelectedIndex = 0;//10/21/15 Daniel To make better flow


                            DRSBureauDropDownList.SelectedValue = Convert.ToString(referralTable.Rows[0]["DHSAgencyServiceId"]);
                            if (!DRSBureauDropDownList.SelectedItem.Value.Equals("0"))
                            {
                                DRSRefIdRow.Visible = true;//10/21/15 Daniel To make better flow
                                RefClientIdRow.Visible = true;//10/21/15 Daniel To make better flow
                                DRSRefIdTextBox.Text = Convert.ToString(referralTable.Rows[0]["DRSReferralID"]);
                                RefClientIdTextBox.Text = Convert.ToString(referralTable.Rows[0]["ReferralClientId"]);
                            }
                        }

                        cmd = new SqlCommand("usp_ClientAgencyEditDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);

                        referralTable = new DataTable();
                        using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(referralTable);
                        ArrayList relationDegreeTypes = new ArrayList();
                        if (referralTable.Rows.Count > 0)
                        {
                            foreach (DataRow row in referralTable.Rows)
                            {
                                if (Convert.ToString(row["RelationshipDegree"]).Equals("1"))
                                {
                                    AgenciesCheckBoxList.Items[0].Selected = true;
                                    CheckboxSelection();
                                    PAgencyDropDownList.SelectedValue = Convert.ToString(row["AgencyId"]);
                                    LoadPrimaryAgencyContact();
                                    Boolean originalContact = Convert.ToBoolean(row["OriginalContact"]);
                                    if (!originalContact)
                                        PContactDropDownList.SelectedValue = Convert.ToString(row["AgencyContactId"]);
                                }
                                else if (Convert.ToString(row["RelationshipDegree"]).Equals("2"))
                                {
                                    AgenciesCheckBoxList.Items[1].Selected = true;
                                    CheckboxSelection();
                                    SAgencyDropDownList.SelectedValue = Convert.ToString(row["AgencyId"]);
                                    LoadSecondaryAgencyContact();
                                    Boolean originalContact = Convert.ToBoolean(row["OriginalContact"]);
                                    if (!originalContact)
                                        SContactDropDownList.SelectedValue = Convert.ToString(row["AgencyContactId"]);

                                }
                            }
                        }
                    }
                    UpdateButton.Visible = true;
                    CancelUpdateButton.Visible = true;
                    SaveButton.Visible = false;
                }
            }
            catch (SqlException se)
            {
                ErrorHandler(se);
            }
            finally
            {
                con.Close();
            }
        }

        protected void btnDeleteRefInfo_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                if (OReferralDateDropDownList.SelectedItem != null && !OReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();

                    //cmd = new SqlCommand("usp_ClientReferralDetail", con);
                    cmd = new SqlCommand("usp_ClientReferral_Delete", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //---------------------------------------------------------------------------------------------

                    cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);

                    Session["ReferralId"] = OReferralDateDropDownList.SelectedValue;
                    //cmd.ExecuteNonQuery();
                    //Response.Write("<script language=javascript>alert('Client Record Deleted Successfully.')</script>");
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    int returnVal = Convert.ToInt32(returnParameter.Value);
                    if (returnVal > 0)
                    {
                        //Response.Write("<script language=javascript>alert('Some error occurred while deleting the record!! Please Contact the Admin for Assitance.')</script>");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myfunction", "<script type='text/javascript'>$('#alertDeleteFailed').show();</script>", false);
                    }
                    else
                    {
                        Response.Write("<script language=javascript>alert('Client Record Deleted Successfully.')</script>");
                    }
                    Clear();
                    PopulateReferralDates();
                }
            }
            catch (SqlException se)
            {
                ErrorHandler(se);
            }
            finally
            {
                con.Close();
            }
        }


        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Clientinsurance.aspx");
        }

        protected void NewSearch_Click(object sender, EventArgs e)
        {
            FirstNameText.Text = "";
            LastNameText.Text = "";

            Clear();
            CurrentClientValueLabel.Text = "";
            SAgencyGridView.Visible = false;
            PAgencyGridView.Visible = false;
            RefInfoGridGridView.Visible = false;
            OReferralDateRow.Visible = false;
            GridView2.Visible = false;
            GridView1.Visible = false;
            AgenciesInfoRow.Visible = false;

            Session["ClientID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ClientInfoGrid"] = null;
            Session["InsuranceInfoGrid"] = null;
            Session["ServiceInfoDataTable"] = null;
            Session["SSN"] = null;

            MarkInactiveReason.SelectedIndex = 0;
            MarkInactiveCheckbox.Checked = false;
            updatepanel_MarkInactive.Visible = false;
        }

        protected void Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("BillingPrioritization.aspx");
        }

        protected void NewAgency_Click(object sender, EventArgs e)
        {
            Session["ReferralObject"] = this;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ffff", "myFunction();", true);
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ffff", "myFunction()", true);
            AgenciesCheckBoxList.SelectedIndex = -1;
            CheckboxSelection();
        }

        protected void NewConact_Click(object sender, EventArgs e)
        {
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
                OReferralDateRow.Visible = true;
                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
            }
            #endregion Populate Grid

            #region Populate Referral Dates

            PopulateReferralDates();

            #endregion Populate Referral Dates
        }

        protected void CheckBoxList_Selection_Changed(object sender, EventArgs e)
        {
            CheckboxSelection();
        }

        protected void ReferralSource_Changed(object sender, EventArgs e)
        {
            if (RSourceDropDownList.SelectedItem.ToString().Trim().Equals("IL-DHS-DRS"))
            {
                DRSBureauRow.Visible = true;
                if (!DRSBureauDropDownList.SelectedItem.Value.Equals("0"))//10/21/15 Daniel To make better flow
                    DRSBureauDropDownList.SelectedIndex = 0;//10/21/15 Daniel To make better flow
                DRSRefIdRow.Visible = false;//10/21/15 Daniel To make better flow
                RefClientIdRow.Visible = false;//10/21/15 Daniel To make better flow
            }
            else
            {
                DRSBureauRow.Visible = false;
                DRSRefIdRow.Visible = false;
                RefClientIdRow.Visible = false;
            }
        }

        protected void Primary_Changed(object sender, EventArgs e)
        {
            PrimaryAgencyChanged();
        }

        protected void PrimaryAgencyChanged()
        {
            try
            {
                #region DB
                //AgenciesInfoRow.Visible = false;
                //RefInfoGridGridView.DataSource = null;
                //RefInfoGridCell.DataBind();
                PAgencyGridView.DataSource = null;
                PAgencyGridView.DataBind();
                //SAgencyGridView.DataSource = null;
                //SAgencyGridView.DataBind();
                PContactDropDownList.Items.Clear();
                //referral date change
                //OReferralDateDropDownList.SelectedIndex = 0;
                if (PAgencyDropDownList.SelectedItem != null && !PAgencyDropDownList.SelectedValue.Equals("Choose"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    DataTable gridTable = new DataTable();
                    gridTable.Columns.Add("Header");
                    gridTable.Columns.Add("Values");

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    con.Open();
                    //This is due to the Referral normalization 6/16/2017 Daniel
                    //cmd = new SqlCommand("select LA.Display AgencyName, AD.OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, InputDate from dbo.AgencyDetail AD join dbo.LookUpAgency LA on AD.AgencyId=LA.AgencyId where AD.AgencyId = @AgencyId", con);
                    cmd = new SqlCommand("select Display AgencyName, OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, A.InputDate from LookUpAgency A join LookUpAgencyContact B on A.AgencyId=B.AgencyId and B.OriginalContact=1 where A.AgencyId= @AgencyId", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@AgencyId", Convert.ToInt32(PAgencyDropDownList.SelectedValue));

                    var detailTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(detailTable);

                    for (int i = 0; i < detailTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < detailTable.Columns.Count; j++)
                        {
                            if (detailTable.Columns[j].ColumnName != "OldID" && detailTable.Columns[j].ColumnName != "InputDate")
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = detailTable.Columns[j].ColumnName;
                                row[1] = detailTable.Rows[i][j].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    PAgencyGridView.DataSource = gridTable;
                    PAgencyGridView.DataBind();
                    PAgencyGridView.Visible = true;
                    AgenciesInfoRow.Visible = true;

                    con.Close();

                    LoadPrimaryAgencyContact();
                }
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void LoadPrimaryAgencyContact()
        {
            #region PrimaryAgencyContact
            if (!PAgencyDropDownList.SelectedValue.Equals(String.Empty)
                            && !PAgencyDropDownList.SelectedValue.Equals("Choose"))
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();


                cmd = new SqlCommand("[usp_GetContactsForAgency]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AgencyId", PAgencyDropDownList.SelectedValue);

                DataTable agencyContacts = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(agencyContacts);
                if (agencyContacts.Rows.Count > 0)
                {
                    PContactDropDownList.Items.Clear();
                    PContactDropDownList.DataSource = agencyContacts;
                    PContactDropDownList.DataValueField = "AgencyContactId";
                    PContactDropDownList.DataTextField = "Contact";
                    PContactDropDownList.DataBind();

                    PContactDropDownList.Items.Insert(0, new ListItem("---Choose---", "Choose"));
                }

                con.Close();
            }
            #endregion
        }

        protected void PContact_Changed(object sender, EventArgs e)
        {
            try
            {
                #region DB
                //AgenciesInfoRow.Visible = false;
                //RefInfoGridGridView.DataSource = null;
                //RefInfoGridGridView.DataBind();
                PAgencyGridView.DataSource = null;
                PAgencyGridView.DataBind();

                //AgenciesInfoRow.Visible = false;
                //RefInfoGridGridView.DataSource = null;
                //RefInfoGridCell.DataBind();
                //PAgencyGridView.DataSource = null;
                //PAgencyGridView.DataBind();
                //SAgencyGridView.DataSource = null;
                //SAgencyGridView.DataBind();

                //referral date change
                //OReferralDateDropDownList.SelectedIndex = 0;
                if (PContactDropDownList.SelectedItem != null && !PContactDropDownList.SelectedValue.Equals("Choose"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    DataTable gridTable = new DataTable();
                    gridTable.Columns.Add("Header");
                    gridTable.Columns.Add("Values");

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    con.Open();
                    //This is due to the Referral normalization 6/16/2017 Daniel
                    //cmd = new SqlCommand("select LA.Display AgencyName, AD.OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, InputDate from dbo.AgencyDetail AD join dbo.LookUpAgency LA on AD.AgencyId=LA.AgencyId where AD.AgencyId = @AgencyId", con);
                    cmd = new SqlCommand("select Display AgencyName, OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, A.InputDate from LookUpAgency A join LookUpAgencyContact B on A.AgencyId=B.AgencyId where B.AgencyContactId= @AgencyContactId", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@AgencyContactId", Convert.ToInt32(PContactDropDownList.SelectedValue));

                    var detailTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(detailTable);

                    for (int i = 0; i < detailTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < detailTable.Columns.Count; j++)
                        {
                            if (detailTable.Columns[j].ColumnName != "OldID" && detailTable.Columns[j].ColumnName != "InputDate")
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = detailTable.Columns[j].ColumnName;
                                row[1] = detailTable.Rows[i][j].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    PAgencyGridView.DataSource = gridTable;
                    PAgencyGridView.DataBind();
                    PAgencyGridView.Visible = true;
                    AgenciesInfoRow.Visible = true;

                    con.Close();
                }
                else
                {
                    PrimaryAgencyChanged();
                }
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Secondary_Changed(object sender, EventArgs e)
        {
            SecondaryAgencyChanged();
        }

        protected void SecondaryAgencyChanged()
        {
            try
            {
                #region DB
                //AgenciesInfoRow.Visible = false;
                //RefInfoGridGridView.DataSource = null;
                // RefInfoGridGridView.DataBind();
                SAgencyGridView.DataSource = null;
                SAgencyGridView.DataBind();
                SContactDropDownList.Items.Clear();
                //referral date change
                //OReferralDateDropDownList.SelectedIndex = 0;
                if (SAgencyDropDownList.SelectedItem != null && !SAgencyDropDownList.SelectedValue.Equals("Choose"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    DataTable gridTable = new DataTable();
                    gridTable.Columns.Add("Header");
                    gridTable.Columns.Add("Values");

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    con.Open();

                    cmd = new SqlCommand("select Display AgencyName, OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, A.InputDate from LookUpAgency A join LookUpAgencyContact B on A.AgencyId=B.AgencyId and B.OriginalContact=1 where B.AgencyId= @AgencyId", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@AgencyId", Convert.ToInt32(SAgencyDropDownList.SelectedValue));

                    var detailTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(detailTable);

                    for (int i = 0; i < detailTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < detailTable.Columns.Count; j++)
                        {
                            if (detailTable.Columns[j].ColumnName != "OldID" && detailTable.Columns[j].ColumnName != "InputDate")
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = detailTable.Columns[j].ColumnName;
                                row[1] = detailTable.Rows[i][j].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    SAgencyGridView.DataSource = gridTable;
                    SAgencyGridView.DataBind();
                    SAgencyGridView.Visible = true;
                    AgenciesInfoRow.Visible = true;
                    con.Close();

                    LoadSecondaryAgencyContact();


                }
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void LoadSecondaryAgencyContact()
        {
            #region SecondaryAgencyContact

            if (!SAgencyDropDownList.SelectedValue.Equals(String.Empty)
                && !SAgencyDropDownList.SelectedValue.Equals("Choose"))
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("[usp_GetContactsForAgency]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AgencyId", SAgencyDropDownList.SelectedValue);

                DataTable agencyContacts = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(agencyContacts);
                if (agencyContacts.Rows.Count > 0)
                {
                    SContactDropDownList.Items.Clear();
                    SContactDropDownList.DataSource = agencyContacts;
                    SContactDropDownList.DataValueField = "AgencyContactId";
                    SContactDropDownList.DataTextField = "Contact";
                    SContactDropDownList.DataBind();
                    SContactDropDownList.Items.Insert(0, new ListItem("---Choose---", "Choose"));
                }

                con.Close();
            }
            #endregion

        }

        protected void SContact_Changed(object sender, EventArgs e)
        {
            try
            {
                #region DB
                //AgenciesInfoRow.Visible = false;
                //RefInfoGridGridView.DataSource = null;
                //RefInfoGridGridView.DataBind();
                SAgencyGridView.DataSource = null;
                SAgencyGridView.DataBind();

                //referral date change
                //OReferralDateDropDownList.SelectedIndex = 0;
                if (SContactDropDownList.SelectedItem != null && !SContactDropDownList.SelectedValue.Equals("Choose"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    DataTable gridTable = new DataTable();
                    gridTable.Columns.Add("Header");
                    gridTable.Columns.Add("Values");

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    con.Open();

                    cmd = new SqlCommand("select Display AgencyName, OldID, Contact, Address, City, State, Zip, Phone, Fax, Email, TTY, A.InputDate from LookUpAgency A join LookUpAgencyContact B on A.AgencyId=B.AgencyId where B.AgencyContactId= @AgencyContactId", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@AgencyContactId", Convert.ToInt32(SContactDropDownList.SelectedValue));

                    var detailTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(detailTable);

                    for (int i = 0; i < detailTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < detailTable.Columns.Count; j++)
                        {
                            if (detailTable.Columns[j].ColumnName != "OldID" && detailTable.Columns[j].ColumnName != "InputDate")
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = detailTable.Columns[j].ColumnName;
                                row[1] = detailTable.Rows[i][j].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    SAgencyGridView.DataSource = gridTable;
                    SAgencyGridView.DataBind();
                    SAgencyGridView.Visible = true;
                    AgenciesInfoRow.Visible = true;

                    con.Close();
                }
                else
                {
                    SecondaryAgencyChanged();
                }
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            try
            {
                //Cancel any updates first.  -- Mrinal
                CancelUpdate();

                #region DBCall
                if (OReferralDateDropDownList.SelectedItem != null && !OReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    DataTable gridTable = new DataTable();
                    gridTable.Columns.Add("Header");
                    gridTable.Columns.Add("Values");

                    btnEditRefInfo.Visible = true;
                    btnDeleteRefInfo.Visible = true;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();

                    #region Populate Referral Information

                    //Convert it to SP 10/26/15 Daniel
                    //cmd = new SqlCommand("select RM.ReferralMethod AS 'Referral Method',RS.ReferralSource AS 'Referral Source', TOS.TypeofService AS 'Service Type' from ClientReferral CR "
                    //        + "join LookUpReferralMethod RM on CR.MethodId = RM.ReferralMethodId "
                    //        + "join LookUpReferralSource RS on CR.SourceId = RS.ReferralSourceId "
                    //        + "left join ClientReferralServiceType RST on CR.ReferralId = RST.ReferralId "
                    //        + "left join LookUpTypeofService TOS on TOS.ServiceId = RST.ServiceTypeId "
                    //        + "where CR.ReferralId = @ReferralId", con);
                    //cmd.CommandType = CommandType.Text;
                    //---------------------------------------------------------------------------------------------

                    //Added Daniel 10/26/2015
                    cmd = new SqlCommand("usp_ClientReferralDetail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //---------------------------------------------------------------------------------------------

                    cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);

                    var referralTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(referralTable);
                    string rSource = string.Empty;
                    string serviceTypeString = string.Empty;
                    for (int i = 0; i < referralTable.Columns.Count; i++)
                    {
                        if (referralTable.Rows.Count > 0)
                        {
                            if (!referralTable.Columns[i].ColumnName.Equals("Service Type"))
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = referralTable.Columns[i].ColumnName;
                                row[1] = referralTable.Rows[0][i].ToString();
                                rSource = referralTable.Rows[0][i].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    for (int i = 0; i < referralTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < referralTable.Columns.Count; j++)
                        {
                            if (referralTable.Columns[j].ColumnName.Equals("Service Type"))
                            {
                                if (string.IsNullOrEmpty(serviceTypeString))
                                    serviceTypeString = referralTable.Rows[i][j].ToString().Trim() + ", ";
                                else
                                    serviceTypeString = serviceTypeString + referralTable.Rows[i][j].ToString().Trim() + ", ";
                            }
                        }
                    }
                    DataRow row1 = gridTable.NewRow();
                    row1[0] = "Service Type";
                    row1[1] = serviceTypeString;
                    gridTable.Rows.Add(row1);


                    if (rSource.Trim().Equals("IL-DHS-DRS"))
                    {
                        cmd = new SqlCommand("select LDHS.Display AS 'DHS Bureau',CRDS.[DRSReferralID] 'DRS Referral ID', CRDS.[ReferralClientId] 'DRS ClientID' from dbo.ClientReferralDHSService DHS "
                                + "join dbo.LookUpDHSAgencyService LDHS on DHS.DHSAgencyServiceId = LDHS.DHSAgencyServiceId "
                                + "LEFT JOIN ClientReferralDHSService CRDS ON DHS.ReferralId = CRDS.ReferralId "
                                + "where DHS.ReferralId = @ReferralId", con);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);
                        var DHSDRSTable = new DataTable();
                        using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(DHSDRSTable);
                        for (int i = 0; i < DHSDRSTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < DHSDRSTable.Columns.Count; j++)
                            {
                                DataRow row = gridTable.NewRow();
                                row[0] = DHSDRSTable.Columns[j].ColumnName;
                                row[1] = DHSDRSTable.Rows[i][j].ToString();
                                gridTable.Rows.Add(row);
                            }
                        }
                    }
                    RefInfoGridGridView.DataSource = gridTable;
                    RefInfoGridGridView.DataBind();
                    RefInfoGridGridView.Visible = true;
                    AgenciesInfoRow.Visible = true;

                    #endregion Populate Referral Information

                    #region Populate Agencies Information
                    DataTable agenciesGridTable = new DataTable();
                    agenciesGridTable.Columns.Add("Header");
                    agenciesGridTable.Columns.Add("Values");


                    cmd = new SqlCommand("usp_GetAgencies_Information", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(OReferralDateDropDownList.SelectedValue));

                    var agenciesTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(agenciesTable);

                    FillAgencyGrid(agenciesTable);

                    #endregion Populate Agencies Information

                    con.Close();

                    //Vivek Code
                    con.Open();
                    cmd = new SqlCommand("SELECT A.Description from LookUpInactiveRefReason A " +
                                "Inner Join InactiveReferrals B ON B.InactiveRefReasonId = A.InactiveRefReasonId " +
                                "where ReferralId = @ReferralId", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ReferralId", OReferralDateDropDownList.SelectedValue);
                    var inactiveRefTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(inactiveRefTable);

                    String reason = String.Empty;
                    if (inactiveRefTable.Rows.Count == 1)
                    {
                        DataRow row = inactiveRefTable.Rows[0];
                        reason = row["Description"].ToString();
                        RefInactiveTable.Visible = true;
                        RefInactiveReason_Show.Text = reason;

                        updatepanel_MarkInactive.Visible = false;
                        MarkInactiveCheckbox.Checked = false;
                        MarkInactiveReason.SelectedIndex = 0;
                        MarkInactiveReason.Visible = false;
                    }
                    else
                        if (inactiveRefTable.Rows.Count == 0)
                        {
                            RefInactiveTable.Visible = false;
                            RefInactiveReason_Show.Text = "";

                            updatepanel_MarkInactive.Visible = true;
                            MarkInactiveCheckbox.Checked = false;
                            MarkInactiveReason.SelectedIndex = 0;
                            MarkInactiveReason.Visible = false;
                        }

                    if (Convert.ToBoolean(Session["IsAdmin"]))
                    {
                        btnDeleteRefInfo.Enabled = true;
                    }
                    else
                    {
                        btnDeleteRefInfo.Enabled = false;
                    }
                }
                else
                {
                    btnEditRefInfo.Visible = false;
                    btnDeleteRefInfo.Visible = false;
                    updatepanel_MarkInactive.Visible = false;
                    MarkInactiveCheckbox.Checked = false;
                    MarkInactiveReason.SelectedIndex = 0;
                    MarkInactiveReason.Visible = false;

                    AgenciesInfoRow.Visible = false;
                    RefInfoGridGridView.DataSource = null;
                    RefInfoGridCell.DataBind();
                    PAgencyGridView.DataSource = null;
                    PAgencyGridView.DataBind();
                    SAgencyGridView.DataSource = null;
                    SAgencyGridView.DataBind();
                }
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void DRS_Changed(object sender, EventArgs e)
        {
            //if (DRSBureauDropDownList.SelectedItem.Text.Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.Text.Trim().Equals("HSP")) //BBS is added 11/3/2016
            if (DRSBureauDropDownList.SelectedItem.Text.Trim().Equals("VR") || DRSBureauDropDownList.SelectedItem.Text.Trim().Equals("HSP") || DRSBureauDropDownList.SelectedItem.Text.Trim().Equals("BBS"))
            {
                DRSRefIdRow.Visible = true;
                RefClientIdRow.Visible = true;
            }
            else
            {
                DRSRefIdRow.Visible = false;
                RefClientIdRow.Visible = false;
            }
        }

        private void FillAgencyGrid(DataTable agenciesTable)
        {
            DataTable primaryAgenciesGridTable = new DataTable();
            primaryAgenciesGridTable.Columns.Add("Header");
            primaryAgenciesGridTable.Columns.Add("Values");

            DataTable secondaryAgenciesGridTable = new DataTable();
            secondaryAgenciesGridTable.Columns.Add("Header");
            secondaryAgenciesGridTable.Columns.Add("Values");

            for (int i = 0; i < agenciesTable.Rows.Count; i++)
            {
                for (int j = 0; j < agenciesTable.Columns.Count; j++)
                {
                    if (agenciesTable.Columns[j].ColumnName != "RelationshipDegree")
                    {
                        if (agenciesTable.Rows[i][0].ToString().Equals("1"))
                        {
                            DataRow row = primaryAgenciesGridTable.NewRow();
                            row[0] = agenciesTable.Columns[j].ColumnName;
                            row[1] = agenciesTable.Rows[i][j].ToString();
                            primaryAgenciesGridTable.Rows.Add(row);
                        }
                        else if (agenciesTable.Rows[i][0].ToString().Equals("2"))
                        {
                            DataRow row = secondaryAgenciesGridTable.NewRow();
                            row[0] = agenciesTable.Columns[j].ColumnName;
                            row[1] = agenciesTable.Rows[i][j].ToString();
                            secondaryAgenciesGridTable.Rows.Add(row);
                        }
                    }
                }
            }

            PAgencyGridView.DataSource = primaryAgenciesGridTable;
            PAgencyGridView.DataBind();
            PAgencyGridView.Visible = true;
            SAgencyGridView.DataSource = secondaryAgenciesGridTable;
            SAgencyGridView.DataBind();
            SAgencyGridView.Visible = true;
            AgenciesInfoRow.Visible = true;
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

                OReferralDateDropDownList.DataSource = refDatesTable;
                OReferralDateDropDownList.DataTextField = "ReferralDate";
                OReferralDateDropDownList.DataValueField = "ReferralId";
                OReferralDateDropDownList.DataBind();

                OReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                OReferralDateRow.Visible = true;
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void Clear()
        {
            RMethodDropDownList.SelectedIndex = 0;
            RSourceDropDownList.SelectedIndex = 0;
            AgenciesCheckBoxList.Items[0].Selected = false;
            AgenciesCheckBoxList.Items[1].Selected = false;

            ServiceTypeCheckBoxList.SelectedIndex = -1;
            DRSBureauDropDownList.SelectedIndex = 0;
            DRSBureauRow.Visible = false;
            DRSRefIdTextBox.Text = "";//Added 10/22/2015 Daniel
            DRSRefIdRow.Visible = false;//Added 10/22/2015 Daniel
            RefClientIdTextBox.Text = "";
            RefClientIdRow.Visible = false;


            RDateTextBox.Text = String.Empty; //Added Mrinal


            updatepanel_MarkInactive.Visible = false;
            MarkInactiveCheckbox.Checked = false;
            MarkInactiveReason.SelectedIndex = 0;
            MarkInactiveReason.Visible = false;

            PNewContactCheckBox.Checked = false;
            PAgencyRow.Visible = false;
            PContactRow.Visible = false;
            PNewContactRow.Visible = false;
            ClearNewAgencyFields();
            ClearNewContactFields();

            //PNameRow.Visible = false;
            //PPhoneRow.Visible = false;
            //PEmailRow.Visible = false;
            //PExtensionRow.Visible = false;
            //PFaxRow.Visible = false;
            //PNameTextBox.Text = "";
            //PPhoneTextBox.Text = "";
            //PEmailTextBox.Text = "";
            //PExtensionTextBox.Text = "";
            //PFaxTextBox.Text = "";

            SNewContactCheckBox.Checked = false;
            SAgencyRow.Visible = false;
            SContactRow.Visible = false;
            SNewContactRow.Visible = false;
            //SNameRow.Visible = false;
            //SPhoneRow.Visible = false;
            //SEmailRow.Visible = false;
            //SExtensionRow.Visible = false;
            //SFaxRow.Visible = false;
            //SNameTextBox.Text = "";
            //SPhoneTextBox.Text = "";
            //SEmailTextBox.Text = "";
            //SExtensionTextBox.Text = "";
            //SFaxTextBox.Text = "";
            AgenciesInfoRow.Visible = false;
            NotFoundLabel.Visible = false; //10/14/15 Daniel
        }

        protected void LoadData()
        {
            try
            {
                #region DBCall
                OReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                #region Referral Method
                cmd = new SqlCommand("Select * from dbo.LookUpReferralMethod where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var refMethodTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refMethodTable);

                RMethodDropDownList.DataSource = refMethodTable;
                RMethodDropDownList.DataTextField = "ReferralMethod";
                RMethodDropDownList.DataValueField = "ReferralMethodId";
                RMethodDropDownList.DataBind();

                RMethodDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));


                #endregion Referral Method

                #region Referral Source
                cmd = new SqlCommand("Select * from dbo.LookUpReferralSource where Active = 1 Order by DisplayOrder ASC", con);
                cmd.CommandType = CommandType.Text;

                var refSourceTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refSourceTable);

                RSourceDropDownList.DataSource = refSourceTable;
                RSourceDropDownList.DataTextField = "ReferralSource";
                RSourceDropDownList.DataValueField = "ReferralSourceId";
                RSourceDropDownList.DataBind();
                RSourceDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));



                #endregion Referral Source

                #region DRS Bureau
                cmd = new SqlCommand("Select * from dbo.LookUpDHSAgencyService where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var DRSTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(DRSTable);

                DRSBureauDropDownList.DataSource = DRSTable;
                DRSBureauDropDownList.DataTextField = "Display";
                DRSBureauDropDownList.DataValueField = "DHSAgencyServiceId";
                DRSBureauDropDownList.DataBind();
                DRSBureauDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));



                #endregion DRS Bureau

                #region Load States for New Agency
                cmd = new SqlCommand("select * from dbo.LookUpUSStates", con);
                cmd.CommandType = CommandType.Text;

                var stateTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(stateTable);

                NAStateDropDownList.DataSource = stateTable;
                NAStateDropDownList.DataTextField = "Name";
                NAStateDropDownList.DataValueField = "State";
                NAStateDropDownList.DataBind();

                NAStateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "-1"));

                #endregion


                #region Primary & Secondary Agencies
                LoadAgencies();


                #endregion Primary & Secondary Agencies

                #region ServiceType

                cmd = new SqlCommand("select * from LookupTypeofService where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var serviceTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);

                ServiceTypeCheckBoxList.DataSource = serviceTypeTable;
                ServiceTypeCheckBoxList.DataTextField = "TypeofService";
                ServiceTypeCheckBoxList.DataValueField = "ServiceId";
                ServiceTypeCheckBoxList.DataBind();

                #endregion ServiceType

                #region MarkInactive

                cmd = new SqlCommand("select * from LookUpInactiveRefReason where Active = 1 Order by DisplayOrder ASC", con);
                cmd.CommandType = CommandType.Text;

                var MarkInactiveTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(MarkInactiveTable);

                MarkInactiveReason.DataSource = MarkInactiveTable;
                MarkInactiveReason.DataTextField = "Description";
                MarkInactiveReason.DataValueField = "InactiveRefReasonId";


                MarkInactiveReason.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                MarkInactiveReason.DataBind();

                #endregion


                con.Close();


                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
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

        protected void RDateCustomValidator_Validate(object sender, ServerValidateEventArgs e)
        {
            DateTime d;
            e.IsValid = DateTime.TryParseExact(e.Value, "mm/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        }

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            CurrentClientValueLabel.Text = firstName + " " + lastName;
            CurrentClientLabel.Visible = true;
            CurrentClientValueLabel.Visible = true;
        }

        private void CheckboxSelection()
        {
            if (AgenciesCheckBoxList.SelectedItem != null)
            {
                bool isPrimary = false;
                bool isSecondary = false;
                foreach (ListItem item in AgenciesCheckBoxList.Items)
                {
                    if (item.Selected && item.Text.Equals("Primary"))
                    {
                        PAgencyRow.Visible = true;
                        PContactRow.Visible = true;
                        PNewContactRow.Visible = true;
                        isPrimary = true;
                        //LoadAgencies();            -Changed by Mrinal... Added in the PageLoad to load it once.
                    }
                    else if (item.Selected && item.Text.Equals("Secondary"))
                    {
                        SAgencyRow.Visible = true;
                        SContactRow.Visible = true;
                        SNewContactRow.Visible = true;
                        isSecondary = true;
                        //LoadAgencies();           -Changed by Mrinal... Added in the PageLoad to load it once.
                    }
                }
                if (!isPrimary)
                {
                    PAgencyRow.Visible = false;
                    PContactRow.Visible = false;
                    PNewContactRow.Visible = false;
                    PAgencyDropDownList.SelectedIndex = 0;  //Mrinal --- Added to clear dropdown seelction on change of checkbox.

                }
                if (!isSecondary)
                {
                    SAgencyRow.Visible = false;
                    SContactRow.Visible = false;
                    SNewContactRow.Visible = false;
                    SAgencyDropDownList.SelectedIndex = 0;  //Mrinal --- Added to clear dropdown seelction on change of checkbox.
                }
            }
            else
            {
                PAgencyRow.Visible = false;
                PContactRow.Visible = false;
                PNewContactRow.Visible = false;
                SAgencyRow.Visible = false;
                SContactRow.Visible = false;
                SNewContactRow.Visible = false;
            }
        }

        private void LoadAgencies()
        {
            try
            {
                PAgencyDropDownList.Items.Clear();
                SAgencyDropDownList.Items.Clear();
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                cmd = new SqlCommand("select * from dbo.LookUpAgency where Active = 1 ORDER BY Display", con);
                cmd.CommandType = CommandType.Text;

                var agencyTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(agencyTable);

                PAgencyDropDownList.DataSource = agencyTable;
                PAgencyDropDownList.DataTextField = "Display";
                PAgencyDropDownList.DataValueField = "AgencyId";

                PAgencyDropDownList.DataBind();
                PAgencyDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));



                SAgencyDropDownList.DataSource = agencyTable;
                SAgencyDropDownList.DataTextField = "Display";
                SAgencyDropDownList.DataValueField = "AgencyId";
                SAgencyDropDownList.DataBind();

                SAgencyDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                CAgencyDropDownList.DataSource = agencyTable;
                CAgencyDropDownList.DataTextField = "Display";
                CAgencyDropDownList.DataValueField = "AgencyId";

                CAgencyDropDownList.DataBind();
                CAgencyDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void MarkInactiveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarkInactiveReason.Visible == false)
            {
                MarkInactiveReason.Visible = true;
                MarkInactiveReason.SelectedIndex = 0;
            }
            else
                if (MarkInactiveReason.Visible == true)
                {
                    MarkInactiveReason.Visible = false;
                    MarkInactiveReason.SelectedIndex = 0;
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

        protected void ContactSave_Click(object sender, EventArgs e)
        {
            #region AgencyName Validation

            bool isAgencySelected = false;
            bool isContactName = false;

            if (!CAgencyDropDownList.SelectedValue.Equals("Choose"))
            {
                isAgencySelected = true;
                SelectAgencyLabel.Visible = false;
            }
            else
            {
                isContactName = false;
                SelectAgencyLabel.Visible = true;
            }


            if (!CNameTextBox.Text.Equals(""))
            {
                isContactName = true;
                ContactErrorLabel.Visible = false;
            }
            else
            {
                isAgencySelected = false;
                ContactErrorLabel.Visible = true;
            }
            #endregion AgencyName Validation

            #region DB Call

            if (isAgencySelected && isContactName)
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("[usp_NewAgencyContact_Insert]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (!CAgencyDropDownList.SelectedValue.Equals("Choose"))
                    cmd.Parameters.AddWithValue("@AgencyId", CAgencyDropDownList.SelectedValue);

                if (CNameTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Contact", CNameTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Contact", System.DBNull.Value);

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

                if (ExtensionTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Extension", ExtensionTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Extension", System.DBNull.Value);

                cmd.Parameters.AddWithValue("@OriginalContact", 0);

                SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                serviceId.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();

                Response.Write("<script language=javascript>alert('New Contact for Agency Created.')</script>");
                //Clear();

                LoadPrimaryAgencyContact();
                LoadSecondaryAgencyContact();

                con.Close();
            }
            #endregion DB Call
        }

        protected void ClearNewContactFields()
        {
            CAgencyDropDownList.SelectedIndex = -1;
            CNameTextBox.Text = String.Empty;
            PhoneTextBox.Text = String.Empty;
            EmailTextBox.Text = String.Empty;
            ExtensionTextBox.Text = String.Empty;
        }

        protected void NASave_Click(object sender, EventArgs e)
        {
            #region AgencyName Validation

            bool isAgencyName = false;
            if (String.IsNullOrEmpty(NAgencyNameText.Text))
            {
                isAgencyName = false;
                NAgencyErrorLabel.Visible = true;
            }
            else
            {
                isAgencyName = true;
                NAgencyErrorLabel.Visible = false;
            }
            #endregion AgencyName Validation

            #region DB Call

            if (isAgencyName)
            {
                AgenciesInfoRow.Visible = false;
                Session["ReferralObject"] = this;
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("[usp_NewAgency_Insert]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Display", NAgencyNameText.Text);

                if (NAAddressTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Address", NAAddressTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Address", System.DBNull.Value);

                if (NACityTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@City", NACityTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@City", System.DBNull.Value);

                if (NAStateDropDownList.SelectedValue != "-1")
                    cmd.Parameters.AddWithValue("@State", NAStateDropDownList.SelectedValue);
                else
                    cmd.Parameters.AddWithValue("@State", System.DBNull.Value);

                if (NAZipTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Zip", NAZipTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Zip", System.DBNull.Value);

                if (NATTYTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@TTY", NATTYTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@TTY", System.DBNull.Value);


                //FOR NEW AGENCYCONTACT WITH DEFAULT ADDRESS 1
                if (NAPhoneTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Phone", NAPhoneTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Phone", System.DBNull.Value);

                if (NAFaxTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Fax", NAFaxTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Fax", System.DBNull.Value);

                if (NAEmailTextBox.Text != "")
                    cmd.Parameters.AddWithValue("@Email", NAEmailTextBox.Text);
                else
                    cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);


                SqlParameter agencyId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                agencyId.Direction = ParameterDirection.ReturnValue;

                Session["AgencyName"] = NAgencyNameText.Text;
                //Referral referral = new Referral(); referral.AgencyName = NAgencyNameText.Text;

                Referral referral = (Referral)Session["ReferralObject"];
                referral.AgencyName = NAgencyNameText.Text;
                AgenciesInfoRow.Visible = true;
                cmd.ExecuteNonQuery();

                Response.Write("<script language=javascript>alert('New referral agency created.')</script>");
                //Clear();

                LoadAgencies();
                //ClientScript.RegisterClientScriptBlock(this.GetType(), "closing", "window.close();", true);

                con.Close();
            }
            #endregion DB Call
        }

        protected void ClearNewAgencyFields()
        {
            NAStateDropDownList.SelectedIndex = -1;
            NAgencyNameText.Text = String.Empty;
            NAAddressTextBox.Text = String.Empty;
            NAFaxTextBox.Text = String.Empty;
            NAEmailTextBox.Text = String.Empty;
            NATTYTextBox.Text = String.Empty;
            NAPhoneTextBox.Text = String.Empty;
            NAZipTextBox.Text = String.Empty;
            NAZipTextBox.Text = String.Empty;
        }
    }
}
