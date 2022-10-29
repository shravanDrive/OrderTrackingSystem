using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace ATUClient
{
    public partial class EvaluationForm : System.Web.UI.Page
    {
        List<string> serviceTypes;
        static List<string> clinicians = new List<string>();
        static string atuId ; //Pavan
        static List<int> list = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClinicianTextBox.Text = "";
                clinicians.Clear();

                CliniciansCheckBoxList.Visible = false;
                RoleLabel.Visible = false;

                chkbx_ShowClinicians.Checked = false;

                CliniciansCheckBoxList.Visible = false;
                for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
                {
                    ListItem item = CliniciansCheckBoxList.Items[i];
                    if (item.Selected)
                        item.Selected = false;
                }

                NotFoundLabel.Visible = false;
                ClinicianTextBox.Visible = false;
                ClinicianTextBox.Text = "";
                clinicians.Clear();

                LoadData();
                Session["ServiceTypes"] = serviceTypes;
            }
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {

                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;
                string txtLast = LastNameText.Text.Trim();
                string txtFirst = FirstNameText.Text.Trim();

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

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

                if (GridView1.Rows.Count <= 0)
                {
                    NotFoundLabel.Visible = true;
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
                    Session["SSN"] = searchDataTable.Rows[0]["SSN"].ToString(); ;

                    PopulateSessionVariables(searchDataTable.Rows[0]["FirstName"].ToString(), searchDataTable.Rows[0]["LastName"].ToString());
                    CurrentClientLabel.Visible = true;
                    CurrentClientValueLabel.Visible = true;
                    GridView1.Visible = false;
                    NotFoundLabel.Visible = false;

                    PopulateReferralDates();
                    //PopulateDateGrid(dt1);

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

        protected void NewSearch_Click(object sender, EventArgs e)
        {
            Session["ClientID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ClientInfoGrid"] = null;
            Session["InsuranceInfoGrid"] = null;
            Session["ServiceInfoDataTable"] = null;
            Session["SSN"] = null;
            Session["ServiceTypes"] = null;
            
            FirstNameText.Text = "";
            LastNameText.Text = "";

            Clear();
            CurrentClientValueLabel.Text = "";
            GridView1.Visible = false;

            chkbx_ShowClinicians.Checked = false;

            CliniciansCheckBoxList.Visible = false;
            for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
            {
                ListItem item = CliniciansCheckBoxList.Items[i];
                if (item.Selected)
                    item.Selected = false;
            }

            ClinicianTextBox.Visible = false;
            ClinicianTextBox.Text = "";
            ReferralDateDropDownList.Items.Clear();
            clinicians.Clear();
            //ReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "0"));
        }

        private void PopulateReferralDates()
        {
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
        }

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            CurrentClientValueLabel.Text = firstName + " " + lastName;
            Session["ClientName"] = CurrentClientValueLabel.Text;
        }

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

        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            ReferralDateChanged();
        }

        protected void ReferralDateChanged()
        {
            try
            {
                int referralIndex = ReferralDateDropDownList.SelectedIndex;
                Clear();
                ReferralDateDropDownList.SelectedIndex = referralIndex;

                if (!ReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    #region Populate Client Data

                    //DataTable dt1 = GetServiceDataTable(Convert.ToInt32(Session["ClientID"]));

                    #endregion

                    //PopulateReferralDates();


 
                    loadReferralTabST();
                }
                else
                {
 
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void grdSearch_Changing(object sender, GridViewPageEventArgs e)
        {
            if (Session != null && Session["SearchGridDataTable"] != null)
            {
                GridView1.DataSource = Session["SearchGridDataTable"];
                GridView1.PageIndex = e.NewPageIndex;
                GridView1.DataBind();

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
            int clientId = 0;
            int index = Convert.ToInt32(e.CommandArgument);
            if (str.Equals("Select"))
            {
                GridViewRow row = GridView1.Rows[index];
                DataTable dt = new DataTable();
                DataRow r1 = dt.NewRow();
                dt.Columns.Add("ClientID");
                dt.Columns.Add("First_Name");
                dt.Columns.Add("Last_Name");
                dt.Columns.Add("SS#");
                clientId = Convert.ToInt32(row.Cells[1].Text);
                Session["ClientID"] = clientId;
                Session["SSN"] = row.Cells[4].Text;

                for (int i = 1; i < row.Cells.Count; i++)
                {
                    r1[i - 1] = row.Cells[i].Text;
                }

                dt.Rows.Add(r1);
                GridView1.Visible = false;
                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
                CurrentClientLabel.Visible = true;
                CurrentClientValueLabel.Visible = true;
                //CancelUpdate();
                Clear();

            }
            #endregion Populate Grid

            try
            {
                #region Populate Client Data

                //DataTable dt1 = GetServiceDataTable(clientId);

                #endregion Populate Client Data

                PopulateReferralDates();

                //PopulateDateGrid(dt1);

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

                //Email Admins - Mrinal
                List<String> referralServiceTypes = new List<String>();
                foreach (DataRow row in serviceTypeTable.Rows)
                {
                    if (string.IsNullOrEmpty(labelValue))
                        labelValue = row["TypeofService"].ToString();
                    else
                        labelValue += "," + row["TypeofService"];
                    referralServiceTypes.Add(row["TypeofService"].ToString());
                }
                if (referralServiceTypes.Count > 0)
                {
                    Session["referralServiceTypes"] = referralServiceTypes;
                }

                //ReferralTabSTLabel.Visible = true;
                //ReferralTabSTLabel.Text = "Referral Tab's Service Types: ";
                //if (string.IsNullOrEmpty(labelValue))
                //    ReferralTabSTLabel.Text += "NONE";
                //else
                //    ReferralTabSTLabel.Text += labelValue;
                con.Close();
            }
        }

        protected void ServiceType_Changed(object sender, EventArgs e)
        {
            //CPTCodeButton.Visible = false;
            int j = 0;
            for (int i = 0; i < ServiceTypeCheckBoxList.Items.Count; i++)
            {
                if (ServiceTypeCheckBoxList.Items[i].Selected)
                {
                    j++;
                }
            }
            if (j > 1)
            {
                //HiddenCPTCodes.Value = String.Empty;
                //DropDownList1.Visible = true;
                //CPTCodeButton.Visible = false;
                //CPTCodeListTextBox.Visible = false;
            }
            else
            {
                //DropDownList1.Visible = false;
                //HiddenCPTCodes.Value = String.Empty;
                //CPTCodeButton.Visible = true;
                //CPTCodeListTextBox.Visible = true;
            }
        }

        protected void LoadData()
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                #region ServiceType

                cmd = new SqlCommand("select * from LookupTypeofService where Active = 1 and ServiceId in (2,6)", con);
                cmd.CommandType = CommandType.Text;

                var serviceTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);

                ServiceTypeCheckBoxList.DataSource = serviceTypeTable;
                ServiceTypeCheckBoxList.DataTextField = "TypeofService";
                ServiceTypeCheckBoxList.DataValueField = "ServiceId";
                ServiceTypeCheckBoxList.DataBind();

                #endregion ServiceType

                #region ClinicianVisibility
                string netId = Session["ATUID"].ToString();
                cmd = new SqlCommand("select Role from dbo.LookUpRole where roleid = (select [role] from dbo.LookupATUStaff where netid = @NetId and Active = 1)", con);
                cmd.Parameters.AddWithValue("@NetId", netId);

                var clinicianVisibleTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(clinicianVisibleTable);


                if (clinicianVisibleTable.Rows != null && clinicianVisibleTable.Rows.Count > 0 && clinicianVisibleTable.Rows[0][0].ToString() == "Case Manager")
                {
                    updatepanel_ClinicianRow.Visible = true;
                    RoleLabel.Text = "Case Manager";
                    Session["IsAdmin"] = true;
                }
                else
                {
                    updatepanel_ClinicianRow.Visible = false;
                    RoleLabel.Text = "Clinician";
                    Session["IsAdmin"] = false;
                }
                #endregion ClinicianVisibility

                #region ATUClinician

                cmd = new SqlCommand("select * from LookupATUStaff where Active = 1 order by Name", con);
                cmd.CommandType = CommandType.Text;

                var clinicianTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(clinicianTable);

                //ClinicianListBox.DataSource = clinicianTable;
                //ClinicianListBox.DataTextField = "Name";
                //ClinicianListBox.DataValueField = "NetID";
                //ClinicianListBox.DataBind();

                CliniciansCheckBoxList.DataSource = clinicianTable;
                CliniciansCheckBoxList.DataTextField = "Name";
                CliniciansCheckBoxList.DataValueField = "ATUStaffId";
                CliniciansCheckBoxList.DataBind();


                Dictionary<string, string> dictClinicians = new Dictionary<string, string>();
                foreach (DataRow row in clinicianTable.Rows)
                {
                    dictClinicians.Add(row[0].ToString(), row[1].ToString());
                }

                Session["CliniciansDictionary"] = dictClinicians;

                #endregion ATUClinician

                #region Referral Dates

                ReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Referral Dates


                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

                //Added Daniel 8/27/18
        protected void ServiceTypeValidation(object source, ServerValidateEventArgs args)
        {
            //var isValid = false;

            args.IsValid = false;

            foreach (ListItem c in ServiceTypeCheckBoxList.Items)
            {
                if (c.Selected)
                {
                    args.IsValid = true;
                }
            }
        }

        private void PopulateDateGrid(DataTable dt1)
        {
            if (dt1.Rows.Count > 0)
            {
                Session["ServiceInfoDataTable"] = dt1;
             }
            else
            {
 
            }
        }

        protected void chkbx_ShowClinicians_CheckedChanged(object sender, EventArgs e)
        {
            ShowCliniciansChanged();
        }

        protected void ShowCliniciansChanged()
        {
            if (chkbx_ShowClinicians.Checked)
            {
                CliniciansCheckBoxList.Visible = true;
                ClinicianTextBox.Visible = false;
            }
            else
                if (!chkbx_ShowClinicians.Checked)
                {
                    if (clinicians.Count <= 0)
                    {
                        CliniciansCheckBoxList.Visible = false;
                        ClinicianTextBox.Visible = false;
                        ClinicianTextBox.Text = "";
                    }
                    else
                    {
                        CliniciansCheckBoxList.Visible = false;
                        ClinicianTextBox.Visible = true;
                    }
                }
        }

        protected void CliniciansCheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var height = ClinicianTextBox.Height;

            string lastSelectedValue = string.Empty;
            string result = Request.Form["__EVENTTARGET"];
            string[] checkedBox = result.Split('$'); ;
            int index = int.Parse(checkedBox[checkedBox.Length - 1]);
            Boolean isChecked = false;
            if (CliniciansCheckBoxList.Items[index].Selected)
            {
                list.Add(index+1);
                lastSelectedValue = CliniciansCheckBoxList.Items[index].ToString();
                isChecked = true;
            }
            else
            {
                lastSelectedValue = CliniciansCheckBoxList.Items[index].ToString();
            }

   

            if (isChecked)
            {
                ClinicianTextBox.Text = "";

                clinicians.Add(lastSelectedValue);
   
            }

            else
                if (!isChecked)
                {
                    clinicians.Remove(lastSelectedValue);
                }
            
          // 
            
            for (int i = 0; i < clinicians.Count; i++)
            {
                if (i == 0)
                    ClinicianTextBox.Text = clinicians[i]; 
                else
                {
                    ClinicianTextBox.Text += "\n";
                    ClinicianTextBox.Text += clinicians[i];
                }
            }
            ClinicianTextBox.Height = height;
        }

        private void Clear()
        {
            ReferralDateDropDownList.SelectedIndex = -1;
            ServiceTypeCheckBoxList.SelectedIndex = -1;
            CliniciansCheckBoxList.SelectedIndex = -1;
            ClinicianTextBox.Text = "";

            clinicians.Clear();                     //Mrinal
            ClinicianTextBox.Text = "";
            ClinicianHiddenField.Value = "";
            //NoDataLabel.Visible = false;

            NotFoundLabel.Visible = false;
        }


        public void AddPatient(object sender, EventArgs e)
        {
            String navigateUrl = "http://support.iidd.uic.edu/ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEvaluationForm&rs:Command=Render&";
            String clientId = Session["ClientID"].ToString();
            navigateUrl += "ClientId="+clientId+"&";
            atuId = String.Join(",", list.Select(p => p.ToString()).ToArray());
            String aId = atuId;
            navigateUrl += "ATUid=" +aId;
            navigateUrl += "&rs:Format=Excel&rc:Parameters=false&rs:ClearSession=True";

            string redirect = "<script>window.open('" + navigateUrl + "');</script>";
            Response.Write(redirect);
        }
    }
}
