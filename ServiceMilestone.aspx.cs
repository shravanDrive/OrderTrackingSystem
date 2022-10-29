using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace ATUClient
{
    public partial class ServiceMilestone : System.Web.UI.Page
    {
        List<string> serviceTypes;
        static List<string> clinicians = new List<string>();

        Dictionary<String, int> cptCodesDictionary = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Vivek Code
                ClinicianTextBox.Text = "";
                clinicians.Clear();

                updatepanel_FabRow.Visible = false;
                updatePanel_BillingInfo.Visible = false;
                updatepanel_BillingInfoRow.Visible = false;
                updatepanel_CaseCloseMessageRow.Visible = false;
                updatepanel_CaseClosedRow.Visible = false;
                updatepanel_OtherReasonRow.Visible = false;
                panel_DoctorInfo.Visible = false;
                updatepanel_DoctorRow.Visible = false;
                updatepanel_DoctorCheckBoxRow.Visible = false;
                updatepanel_DoctorMessageRow.Visible = false;
                CliniciansCheckBoxList.Visible = false;

                RoleLabel.Visible = false;
                RadioButtonList1.Visible = false;
                ActionDateValidationLabel.Visible = false;
                StatusPanel.Visible = false;
                DropDownList1.Visible = false;
                CPTCodeListTextBox.Visible = false;
                CPTCodeListTextBox.Visible = false;
                ReferralTabSTLabel.Visible = false;
                GridView2.Visible = false;
                GridView4.Visible = false;
                GridView5.Visible = false;
                DoctorInfoLabel.Visible = false;
                NotFoundLabel.Visible = false;
                NoDataLabel.Visible = false;

                CancelUpdateButton.Visible = false;
                UpdateButton.Visible = false;

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
                clinicians.Clear();
                //Vivek Code

                cptCodesDictionary = new Dictionary<string, int>();
                CPTCodeButton.Visible = true;
                CPTCodeListTextBox.Visible = true;
                //Session["ATUID"] = Request.Cookies["ATUId"].Value; shr
                serviceTypes = new List<string>();
                LoadData();
                Session["ServiceTypes"] = serviceTypes;

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
                if (Session != null && (Session["FirstName"] != null || Session["LastName"] != null))
                {
                    Session["ClientName"] = CurrentClientValueLabel.Text;
                }

                if (Session != null && Session["ServiceInfoDataTable"] != null)
                {
                    PopulateReferralDates();
                }
                else
                {
                    if (Session != null && Session["ClientID"] != null)
                    {
                        //DataTable dt1 = GetServiceDataTable(Convert.ToInt32(Session["ClientID"]));
                        PopulateReferralDates();
                        //PopulateDateGrid(GetServiceDataTable(Convert.ToInt32(Session["ClientID"])));

                        SaveButton.Enabled = true;
                        //updatepanel_CaseCloseMessageRow.Visible = false;
                        CaseCloseMessageLabel.Visible = false;
                        //GridView4.Visible = false; Same reason as above
                        //StatusRow.Visible = true; Same reason as above
                    }
                }
            }
            //To maintain values of these textboxes at postback
            if (ClinicianHiddenField.Value != null)
                //ClinicianTextBox.Text = ClinicianHiddenField.Value;
                if (HiddenCPTCodes.Value != null)
                    CPTCodeListTextBox.Text = HiddenCPTCodes.Value;

            #region Link Button on page refresh
            if (Session != null && Session["ServiceInfoDataTable"] != null)
            {
                GridView3.DataSource = (DataTable)Session["ServiceInfoDataTable"];
                foreach (GridViewRow row in GridView3.Rows)
                {
                    //string str = gvr.Cells[2].Text;
                    Label lbl_LomnWritten = (row.Cells[0].FindControl("lbl_ActionName") as Label);
                    string str = lbl_LomnWritten.Text;

                    if (str.Trim().Equals("LOMN Written"))
                    {
                        LinkButton lb = new LinkButton();
                        string DocID = (row.Cells[0].FindControl("lbl_DoctorID") as Label).Text;
                        //lb.ID = "DoctorLink_" + DocID;

                        //Appending the InputByNetId to Enable/Disable Edit Button
                        string InputByNetId = (row.Cells[0].FindControl("lbl_InputNetId") as Label).Text;
                        lb.ID = "DoctorLink_" + DocID + "_" + InputByNetId;

                        lb.ToolTip = "Click to view Doctor Contact.";
                        lb.Click += new EventHandler(lb_Click);
                        lb.Font.Size = FontUnit.Small;

                        lbl_LomnWritten.Visible = false;
                        lb.Text = str;
                        //if (row.Cells[2].FindControl("DoctorLink_" + DocID) == null)
                        if (row.Cells[3].FindControl("DoctorLink_" + DocID + "_" + InputByNetId) == null)
                            row.Cells[3].Controls.Add(lb);
                    }
                }
            }

            #endregion Link Button on page refresh
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                GridView3.Visible = false;
                GridView4.Visible = false;
                GridView5.Visible = false;
                DoctorInfoLabel.Visible = false;
                StatusPanel.Visible = false;
                ReferralTabSTLabel.Visible = false;
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
                GridView2.Visible = false;

                LoadSSN();

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

                    SaveButton.Enabled = true;
                    updatepanel_CaseCloseMessageRow.Visible = false;
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
            if (Page.IsValid)
            {
                if (Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !ddlist_tasks.SelectedValue.Equals("Choose") && !ActionDatesDropDownList.SelectedValue.Equals("0"))
                {
                    #region ActionDateValidation
                    bool isValidDate = false;
                    if (!String.IsNullOrEmpty(ActionDatesTextBox.Text))
                    {
                        string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                        DateTime d;
                        isValidDate = DateTime.TryParseExact(ActionDatesTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                        if (isValidDate)
                        {
                            ActionDateValidationLabel.Visible = false;
                        }
                        else
                        {
                            ActionDateValidationLabel.Visible = true;
                            ActionDateValidationLabel.Text = "Invalid Date Format";
                        }
                    }
                    else
                    {
                        ActionDateValidationLabel.Visible = true;
                        ActionDateValidationLabel.Text = "Enter date.";
                    }

                    #endregion ActionDateValidation

                    try
                    {
                        #region Database Calls
                        if (isValidDate)
                        {
                            SaveData();
                            int clientId = Convert.ToInt32(Session["ClientID"]);
                            DataTable dt1 = GetServiceDataTable(clientId);
                            int referralIndex = ReferralDateDropDownList.SelectedIndex;
                            Clear();
                            ReferralDateDropDownList.SelectedIndex = referralIndex;
                            PopulateDateGrid(dt1);
                            updatepanel_DoctorMessageRow.Visible = false;
                            clinicians.Clear();
                        }
                        #endregion Database Calls
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler(ex);
                    }
                }
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

        //Added by Mrinal Dhawan --- 5/23/2017
        protected void CancelUpdate_Click(object sender, EventArgs e)
        {
            CancelUpdate();
        }

        //Added by Mrinal Dhawan --- 5/23/2017 
        protected void CancelUpdate()
        {
            string selectedReferralValue = ReferralDateDropDownList.SelectedValue;
            Clear();
            ReferralDateDropDownList.SelectedValue = selectedReferralValue;
            chkbx_ShowClinicians.Checked = false;
            ShowCliniciansChanged();
            foreach (ListItem item in CliniciansCheckBoxList.Items)
            {
                item.Selected = false;
            }
            chkbx_ShowClinicians.Checked = false;
            foreach (ListItem item in CliniciansCheckBoxList.Items)
            {
                item.Selected = false;
            }
            UpdateButton.Visible = false;
            CancelUpdateButton.Visible = false;
            SaveButton.Visible = true;
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !ddlist_tasks.SelectedValue.Equals("Choose") && !ActionDatesDropDownList.SelectedValue.Equals("0"))
                {
                    #region ActionDateValidation
                    bool isValidDate = false;
                    if (!String.IsNullOrEmpty(ActionDatesTextBox.Text))
                    {
                        string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                        DateTime d;
                        isValidDate = DateTime.TryParseExact(ActionDatesTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                        if (isValidDate)
                        {
                            ActionDateValidationLabel.Visible = false;
                        }
                        else
                        {
                            ActionDateValidationLabel.Visible = true;
                            ActionDateValidationLabel.Text = "Invalid Date Format";
                        }
                    }
                    else
                    {
                        ActionDateValidationLabel.Visible = true;
                        ActionDateValidationLabel.Text = "Enter date.";
                    }

                    #endregion ActionDateValidation

                    try
                    {
                        #region Database Calls
                        if (isValidDate)
                        {
                            UpdateData();
                            int clientId = Convert.ToInt32(Session["ClientID"]);
                            DataTable dt1 = GetServiceDataTable(clientId);
                            int referralIndex = ReferralDateDropDownList.SelectedIndex;
                            Clear();
                            ReferralDateDropDownList.SelectedIndex = referralIndex;
                            PopulateDateGrid(dt1);
                            updatepanel_DoctorMessageRow.Visible = false;
                            clinicians.Clear();
                        }
                        #endregion Database Calls
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler(ex);
                        //Response.Write("<script language=javascript>alert('Not able to update records!!!')</script>");
                    }
                }
            }
        }

        protected void Doctor_Save_Click(object sender, EventArgs e)
        {
            try
            {
                #region DB
                if (!string.IsNullOrEmpty(MDFNameTextBox.Text))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    con.Open();

                    /* Modified the insert stored procedure so that it can be used for Save and Update Both */
                    //cmd = new SqlCommand("[usp_DoctorContact_Insert]", con);

                    cmd = new SqlCommand("[usp_DoctorContact_InsertOrUpdate]", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Update Parameter, which will be 0 in case of a new contact                    
                    cmd.Parameters.AddWithValue("@LOMNContactId", Convert.ToInt32(Session["DoctorID"]));

                    cmd.Parameters.AddWithValue("@MDFName", MDFNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@MDLName", MDLNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@NPI", NPITextBox.Text);
                    cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
                    cmd.Parameters.AddWithValue("@City", CityTextBox.Text);

                    if (StateDropDownList.SelectedValue != "Choose")
                        cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                    else
                        cmd.Parameters.AddWithValue("@State", System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);
                    cmd.Parameters.AddWithValue("@Fax", FaxTextBox.Text);
                    cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);

                    SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                    serviceId.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();


                    if (Session["DoctorID"] != null)
                    {
                        FillDoctorData(Convert.ToString(Session["DoctorID"]));
                        Response.Write("<script language=javascript>alert('Doctor Record Update Successfully!!!')</script>");
                        String selectedDoctorId = Convert.ToString(Session["DoctorID"]);
                        ClearDoctorContact();
                        PopulateDoctorData();
                        DoctorDropDownList.SelectedValue = selectedDoctorId;
                        panel_DoctorInfo.Visible = false;
                    }
                    else if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(serviceId.Value) > 0)
                    {
                        //updatepanel_DoctorRow.Visible = true;
                        //updatepanel_DoctorCheckBoxRow.Visible = true;
                        updatepanel_DoctorMessageRow.Visible = true;
                        DoctorMessageLabel.Text = "Doctor Record Saved Successfully";
                        DoctorCheckBox.Checked = false;



                        ClearDoctorContact();
                        PopulateDoctorData();
                        DoctorDropDownList.SelectedValue = Convert.ToString(serviceId.Value);
                    }
                    else
                    {
                        updatepanel_DoctorMessageRow.Visible = true;
                        DoctorMessageLabel.Text = "Doctor with NPI - " + NPITextBox.Text + " already present in the database";
                    }

                    con.Close();

                    DoctorSaveButton.Text = "Save Doctor Contact";
                    Session["DoctorID"] = null;
                }
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("BillingPrioritization.aspx");
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
            GridView2.Visible = false;
            GridView3.Visible = false;
            GridView4.Visible = false;
            GridView5.Visible = false;
            DoctorInfoLabel.Visible = false;
            StatusPanel.Visible = false;

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

        protected void On_RadioValue_Changed(object sender, EventArgs e)
        {
            //BillingInfoLabelRow.Visible = false;
            updatePanel_BillingInfo.Visible = false;
            //BillingInfoRow.Visible = false;
            updatepanel_BillingInfoRow.Visible = false;

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();
            try
            {
                #region FollowUp
                string strCommand = "select * from LookUpFollowAction where FollowTypeId = (select FollowTypeId from LookUpFollowType where display = @followUpType) and Active = 1 order by DisplayOrder";


                string selectedValue = RadioButtonList1.SelectedValue.ToString();

                cmd = new SqlCommand(strCommand, con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@followUpType", selectedValue);

                var followUpTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(followUpTable);

                ActionDatesDropDownList.DataSource = followUpTable;
                ActionDatesDropDownList.DataTextField = "Display";
                ActionDatesDropDownList.DataValueField = "FollowActionId";
                ActionDatesDropDownList.DataBind();

                ActionDatesDropDownList.Items.Insert(0, new ListItem("--Choose one--", "0"));

                #endregion FollowUp
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            con.Close();
        }

        protected void On_Tasks_Changed(object sender, EventArgs e)
        {
            TaskChanged();
        }

        protected void TaskChanged()
        {
            //BillingInfoLabelRow.Visible = false;
            updatePanel_BillingInfo.Visible = false;
            //BillingInfoRow.Visible = false;
            updatepanel_BillingInfoRow.Visible = false;
            updatepanel_FabRow.Visible = false;
            updatepanel_CaseClosedRow.Visible = false;
            updatepanel_OtherReasonRow.Visible = false;

            string taskSelectedValue = ddlist_tasks.SelectedItem.ToString();
            if (!taskSelectedValue.Equals("--Choose one--"))
            //&& !taskSelectedValue.Equals("Case Closed"))//Temporary Condition. To be removed
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                try
                {
                    #region ActionDates
                    string cmdIntake = "select ActionId = IntakeActionId, Display from LookUpIntakeAction where Active = 1 order by DisplayOrder";
                    string cmdEval = "select ActionId = EvalActionId, Display from LookUpEvalAction where Active = 1 order by DisplayOrder";
                    string cmdImplementation = "select ActionId = ImplementActionId,Display from LookUpImplementAction where Active = 1 order by DisplayOrder";
                    string cmdFollowUp = "select ActionId = FollowActionId,Display from LookUpFollowAction where FollowTypeId = (select FollowTypeId from LookUpFollowType where display = @followUpType) and Active = 1 order by DisplayOrder";
                    string cmdTraining = "select ActionId = TrainingActionId,Display from dbo.LookUpTrainingAction where Active = 1 order by DisplayOrder";
                    string cmdCaseClose = "select ActionId = ReferCaseCloseId,Display from dbo.LookUpCaseCloseAction where Active = 1";
                    string cmdEquipment = "select ActionId = EquipActionId,Display from dbo.LookUpEquipmentAction where Active = 1 order by DisplayOrder";

                    string strCommand = null;
                    string selectedFollowUpValue = null;

                    switch (taskSelectedValue)
                    {
                        case "Intake":
                            strCommand = cmdIntake;
                            break;
                        case "Evaluation":
                            strCommand = cmdEval;
                            break;
                        case "Implementation":
                            strCommand = cmdImplementation;
                            break;
                        case "Follow-Up":
                            strCommand = cmdFollowUp;
                            selectedFollowUpValue = RadioButtonList1.SelectedValue.ToString();
                            cmd = new SqlCommand(strCommand, con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@followUpType", selectedFollowUpValue);
                            break;
                        case "Training":
                            strCommand = cmdTraining;
                            break;
                        case "Case Cancelled":
                            strCommand = cmdCaseClose;
                            break;
                        case "Equipment":
                            strCommand = cmdEquipment;
                            break;
                        default:
                            break;
                    }

                    //Akshat
                    if (taskSelectedValue != "Follow-Up")// && taskSelectedValue != "Evaluation")
                    {
                        RadioButtonList1.Visible = false;
                        cmd = new SqlCommand(strCommand, con);
                        cmd.CommandType = CommandType.Text;
                    }
                    else
                        RadioButtonList1.Visible = true;

                    var taskDatesTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(taskDatesTable);

                    ActionDatesDropDownList.DataSource = taskDatesTable;
                    ActionDatesDropDownList.DataTextField = "Display";
                    ActionDatesDropDownList.DataValueField = "ActionId";
                    ActionDatesDropDownList.DataBind();

                    ActionDatesDropDownList.Items.Insert(0, new ListItem("--Choose one--", "0"));
                    #endregion ActionDates
                }
                catch (Exception ex)
                {
                    ErrorHandler(ex);
                }

                con.Close();

                ClearDoctorContact();
                updatepanel_DoctorRow.Visible = false;
                DoctorCheckBox.Checked = false;
                updatepanel_DoctorCheckBoxRow.Visible = false;
            }
            else
            {
                ActionDatesDropDownList.Items.Clear();
                ActionDatesDropDownList.Items.Insert(0, new ListItem("--Choose one--", "0"));

            }
        }

        protected void On_ActionDate_Changed(object sender, EventArgs e)
        {
            ActionDateChanged();
        }

        protected void ActionDateChanged()
        {
            string selectedValue = ActionDatesDropDownList.SelectedItem.ToString();

            #region BillingVisibility

            //Praharsh
            if (selectedValue.Equals("Evaluation Date") || selectedValue.Equals("Implementation Date") || selectedValue.Equals("Recheck Date") || selectedValue.Equals("Consult Date") || selectedValue.Equals("Training Date") || selectedValue.Equals("Follow-Up Date") || selectedValue.Equals("Implementation Performed"))
            {
                updatePanel_BillingInfo.Visible = true;
                updatepanel_BillingInfoRow.Visible = true;
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
                    DropDownList1.Visible = true;
                    CPTCodeButton.Visible = false;
                    CPTCodeListTextBox.Visible = false;
                    //break;
                }
                else
                {
                    DropDownList1.Visible = false;
                    CPTCodeButton.Visible = true;
                    CPTCodeListTextBox.Visible = true;
                    //break;
                }
            }
            else
            {
                updatePanel_BillingInfo.Visible = false;
                updatepanel_BillingInfoRow.Visible = false;
            }
            #endregion BillingVisibility

            #region FabHours
            if (selectedValue.Equals("Fabrication Date"))
                updatepanel_FabRow.Visible = true;
            else
                updatepanel_FabRow.Visible = false;


            #endregion FabHours

            #region Doctor Contact

            if (selectedValue.Equals("LOMN Written"))
            {
                updatepanel_DoctorRow.Visible = true;
                updatepanel_DoctorCheckBoxRow.Visible = true;
            }
            else
            {
                updatepanel_DoctorRow.Visible = false;
                updatepanel_DoctorCheckBoxRow.Visible = false;
            }

            #endregion Doctor Contact

            #region CaseClose Reason
            //updatepanel_OtherReasonRow.Visible = false;
            CaseClosedReasonDropDownList.SelectedIndex = 0;
            if (selectedValue.Equals("Referral Case Cancelled"))
            {
                updatepanel_CaseClosedRow.Visible = true;
            }
            else
            {
                updatepanel_CaseClosedRow.Visible = false;
            }

            #endregion CaseClose Reason

            //ClearDoctorContact();
        }


        protected void On_CaseCloseReason_Changed(object sender, EventArgs e)
        {
            CaseCloseReasonChange();
        }

        protected void CaseCloseReasonChange()
        {
            string reasonSelectedValue = CaseClosedReasonDropDownList.SelectedItem.ToString();
            if (reasonSelectedValue.Equals("Other"))
            {
                updatepanel_OtherReasonRow.Visible = true;
            }
            else
            {
                updatepanel_OtherReasonRow.Visible = false;
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
                CancelUpdate();
                Clear();
                ReferralDateDropDownList.SelectedIndex = referralIndex;
                GridView4.Visible = false;
                GridView5.Visible = false;
                DoctorInfoLabel.Visible = false;

                if (!ReferralDateDropDownList.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    #region Populate Client Data

                    DataTable dt1 = GetServiceDataTable(Convert.ToInt32(Session["ClientID"]));

                    #endregion

                    //PopulateReferralDates();
                    PopulateDateGrid(dt1);

                    if (CheckCaseClose())
                    {
                        SaveButton.Enabled = false;
                        updatepanel_CaseCloseMessageRow.Visible = true;
                    }
                    else
                    {
                        SaveButton.Enabled = true;
                        updatepanel_CaseCloseMessageRow.Visible = false;
                    }
                    loadReferralTabST();
                }
                else
                {
                    GridView3.DataSource = null;
                    GridView3.DataBind();
                    GridView4.DataSource = null;
                    GridView4.DataBind();
                    GridView5.DataSource = null;
                    GridView5.DataBind();
                    StatusPanel.Visible = false;
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
                GridView2.DataSource = dt;
                GridView2.DataBind();
                GridView1.Visible = false;
                //GridView2.Visible = true; //No Need to display because of current Client: label
                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
                CurrentClientLabel.Visible = true;
                CurrentClientValueLabel.Visible = true;
                //CancelUpdate();
                Clear();
                UpdateButton.Visible = false;
                CancelUpdateButton.Visible = false;
                SaveButton.Visible = true;

            }
            #endregion Populate Grid

            try
            {
                #region Populate Client Data

                //DataTable dt1 = GetServiceDataTable(clientId);

                #endregion Populate Client Data

                PopulateReferralDates();

                //PopulateDateGrid(dt1);

                SaveButton.Enabled = true;
                updatepanel_CaseCloseMessageRow.Visible = false;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Doctor_Changed(object sender, EventArgs e)
        {
            if (DoctorDropDownList.SelectedItem.Text != "--Choose one--")
            {
                var doctorData = GetDoctorData();
                for (int i = 0; i < doctorData.Rows.Count; i++)
                {
                    if (doctorData.Rows[i]["LOMNContactId"].ToString().Equals(DoctorDropDownList.SelectedValue))
                    {
                        MDFNameTextBox.Text = doctorData.Rows[i]["FirstName"].ToString();
                        MDLNameTextBox.Text = doctorData.Rows[i]["LastName"].ToString();
                        NPITextBox.Text = doctorData.Rows[i]["NPI"].ToString();
                        AddressTextBox.Text = doctorData.Rows[i]["Address"].ToString();
                        CityTextBox.Text = doctorData.Rows[i]["City"].ToString();
                        ZipTextBox.Text = doctorData.Rows[i]["Zip"].ToString();
                        FaxTextBox.Text = doctorData.Rows[i]["Fax"].ToString();
                        EmailTextBox.Text = doctorData.Rows[i]["Email"].ToString();

                        updatepanel_DoctorInfo.Visible = true;
                        updatepanel_DoctorMessageRow.Visible = false;
                    }
                }
            }
        }

        //Removed since dropdown is no longer used Daniel 6/12/17
        protected void CPTCode_Changed(object sender, EventArgs e)
        {
            CPTCodesChanged();
        }

        protected void CPTCodesChanged()
        {
            try
            {
                if (!DropDownList1.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    if (CheckInValidCPTCode() && !updatepanel_CaseCloseMessageRow.Visible)
                    {
                        SaveButton.Enabled = false;
                        Response.Write("<script language=javascript>alert('Select a valid CPT Code')</script>");
                    }
                    else if (updatepanel_CaseCloseMessageRow.Visible)
                    {
                        SaveButton.Enabled = false;
                    }
                    else
                        SaveButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void lb_Click(object sender, EventArgs e)
        {
            try
            {
                GridView4.DataSource = GridView5.DataSource = null;
                GridView4.DataBind();
                GridView5.DataBind();
                #region DBCall
                //SqlConnection con = null;
                //SqlCommand cmd = null;

                //con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                //cmd = new SqlCommand("select FirstName, LastName, NPI, Address, City, State, Zip, Fax, Email from doctorcontact where LOMNContactId like @doctorID", con);
                //cmd.CommandType = CommandType.Text;
                LinkButton lb = sender as LinkButton;

                string[] split_to_get_DocID = lb.ID.Split('_');
                Session["DoctorID"] = split_to_get_DocID[1];
                //cmd.Parameters.AddWithValue("@doctorID", split_to_get_DocID[1]);
                Session["InputBy"] = split_to_get_DocID[2];
                FillDoctorData(split_to_get_DocID[1]);
                //var clientDataTable = new DataTable();
                //using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientDataTable);

                //DoctorInfoLabel.Visible = true;
                //GridView4.DataSource = GridView5.DataSource = clientDataTable;
                //GridView4.DataBind();
                //GridView5.DataBind();
                //GridView4.Visible = GridView5.Visible = true;

                //con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void FillDoctorData(String LOMNContactId)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                cmd = new SqlCommand("select LOMNContactId, FirstName, LastName, NPI, Address, City, State, Zip, Fax, Email from doctorcontact where LOMNContactId like @doctorID", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@doctorID", LOMNContactId);

                var clientDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientDataTable);

                DoctorInfoLabel.Visible = true;
                GridView4.DataSource = GridView5.DataSource = clientDataTable;
                GridView4.DataBind();
                GridView5.DataBind();
                GridView4.Visible = GridView5.Visible = true;

                con.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Doctor_Checked(object sender, EventArgs e)
        {
            #region Doctor Contact

            if (DoctorCheckBox.Checked)
            {
                /*
                DocInfoLabelRow.Visible = true;
                MDFNameRow.Visible = true;
                MDLNameRow.Visible = true;
                NPIRow.Visible = true;
                AddressRow.Visible = true;
                CityRow.Visible = true;
                StateRow.Visible = true;
                ZipRow.Visible = true;
                FaxRow.Visible = true;
                EmailRow.Visible = true;
                DoctorButtonRow.Visible = true;
                */

                //MDFNameTextBox.ReadOnly = false;
                //MDLNameTextBox.ReadOnly = false;
                //NPITextBox.ReadOnly = false;
                //AddressTextBox.ReadOnly = false;
                //CityTextBox.ReadOnly = false;
                //ZipTextBox.ReadOnly = false;
                //FaxTextBox.ReadOnly = false;
                //EmailTextBox.ReadOnly = false;

                MDFNameTextBox.Text = "";
                MDLNameTextBox.Text = "";
                NPITextBox.Text = "";
                AddressTextBox.Text = "";
                CityTextBox.Text = "";
                StateDropDownList.SelectedIndex = 0;
                ZipTextBox.Text = "";
                FaxTextBox.Text = "";
                EmailTextBox.Text = "";

                DoctorDropDownList.SelectedIndex = 0;
                DoctorSaveButton.Enabled = true;

                DoctorSaveButton.Text = "Save Doctor Contact";
                Session["DoctorId"] = null;

                panel_DoctorInfo.Visible = true;
                updatepanel_DoctorMessageRow.Visible = false;
            }
            else
            {
                ClearDoctorContact();
                updatepanel_DoctorMessageRow.Visible = false;
            }

            #endregion Doctor Contact
        }

        protected void ServiceType_Changed(object sender, EventArgs e)
        {
            CPTCodeButton.Visible = false;
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
                HiddenCPTCodes.Value = String.Empty;
                DropDownList1.Visible = true;
                CPTCodeButton.Visible = false;
                CPTCodeListTextBox.Visible = false;
            }
            else
            {
                DropDownList1.Visible = false;
                HiddenCPTCodes.Value = String.Empty;
                CPTCodeButton.Visible = true;
                CPTCodeListTextBox.Visible = true;
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

                //cmd = new SqlCommand("select * from LookupTypeofService where Active = 1", con);
                //cmd.CommandType = CommandType.Text;

                //var serviceTypeTable = new DataTable();
                //using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);

                //ServiceTypeListBox.DataSource = serviceTypeTable;
                //ServiceTypeListBox.DataTextField = "TypeofService";
                //ServiceTypeListBox.DataValueField = "ServiceId";
                //ServiceTypeListBox.DataBind();

                cmd = new SqlCommand("select * from LookupTypeofService where Active = 1", con);
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
                CliniciansCheckBoxList.DataValueField = "NetID";
                CliniciansCheckBoxList.DataBind();


                Dictionary<string, string> dictClinicians = new Dictionary<string, string>();
                foreach (DataRow row in clinicianTable.Rows)
                {
                    dictClinicians.Add(row[1].ToString(), row[2].ToString());
                }

                Session["CliniciansDictionary"] = dictClinicians;

                #endregion ATUClinician

                #region Tasks

                cmd = new SqlCommand("select * from LookUpTask where Active = 1 order by DisplayOrder", con);
                cmd.CommandType = CommandType.Text;

                var taskTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(taskTable);

                ddlist_tasks.DataSource = taskTable;
                ddlist_tasks.DataTextField = "Display";
                ddlist_tasks.DataValueField = "TaskId";
                ddlist_tasks.DataBind();

                ddlist_tasks.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Tasks

                #region RadioButton

                cmd = new SqlCommand("select Display from LookUpFollowType where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var followUpTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(followUpTypeTable);

                RadioButtonList1.DataSource = followUpTypeTable;
                RadioButtonList1.DataTextField = "Display";
                RadioButtonList1.DataValueField = "Display";
                RadioButtonList1.DataBind();
                RadioButtonList1.SelectedIndex = 0;
                #endregion RadioButton

                #region Referral Dates

                ReferralDateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Referral Dates

                #region State
                cmd = new SqlCommand("select * from dbo.LookUpUSStates", con);
                cmd.CommandType = CommandType.Text;

                var stateTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(stateTable);

                StateDropDownList.DataSource = stateTable;
                StateDropDownList.DataTextField = "Name";
                StateDropDownList.DataValueField = "State";
                StateDropDownList.DataBind();

                StateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion State

                #region Doctor
                PopulateDoctorData();
                #endregion Doctor

                #region CPT Codes DropDown

                cmd = new SqlCommand("select * from LookUpCPTCodes where Active = 1 order by DisplayOrder", con);
                cmd.CommandType = CommandType.Text;

                var cptCodesTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(cptCodesTable);

                DataTable newDT = new DataTable();
                newDT.Columns.Add("CPTCodeId");
                newDT.Columns.Add("CPTDisplayText");
                newDT.Columns.Add("DisplayOrder");

                for (int i = 0; i < cptCodesTable.Rows.Count; i++)
                {
                    DataRow row = newDT.NewRow();
                    row[0] = cptCodesTable.Rows[i]["CPTCodeId"];
                    row[1] = cptCodesTable.Rows[i]["CPTCode"] + " - " + cptCodesTable.Rows[i]["Description"];
                    row[2] = cptCodesTable.Rows[i]["DisplayOrder"];
                    newDT.Rows.Add(row);
                }

                DropDownList1.DataSource = newDT;
                DropDownList1.DataTextField = "CPTDisplayText";
                DropDownList1.DataValueField = "CPTCodeId";
                DropDownList1.DataBind();

                DropDownList1.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                CPTCodesCheckBoxList.DataSource = newDT;
                CPTCodesCheckBoxList.DataTextField = "CPTDisplayText";
                CPTCodesCheckBoxList.DataValueField = "CPTCodeId";
                CPTCodesCheckBoxList.DataBind();


                #endregion CPT Codes DropDown

                #region CaseClose Reasons

                cmd = new SqlCommand("select * from LookUpCaseCloseReason where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var caseReasonTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(caseReasonTable);

                CaseClosedReasonDropDownList.DataSource = caseReasonTable;
                CaseClosedReasonDropDownList.DataTextField = "Description";
                CaseClosedReasonDropDownList.DataValueField = "ReasonId";
                CaseClosedReasonDropDownList.DataBind();

                CaseClosedReasonDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion CaseClose Reasons

                #region CPT Codes Modal Window

                cmd = new SqlCommand("select * from LookUpCPTCodes where Active = 1  AND NOCPTCode = 0 order by DisplayOrder", con);
                cmd.CommandType = CommandType.Text;

                var cptCodesTable1 = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(cptCodesTable1);

                DataTable cptCodeDT = new DataTable();
                cptCodeDT.Columns.Add("CPTCodeId");
                cptCodeDT.Columns.Add("CPTDisplayText");
                cptCodeDT.Columns.Add("DisplayOrder");

                for (int i = 0; i < cptCodesTable1.Rows.Count; i++)
                {
                    DataRow row = cptCodeDT.NewRow();
                    row[0] = cptCodesTable1.Rows[i]["CPTCodeId"];
                    row[1] = cptCodesTable1.Rows[i]["CPTCode"] + " - " + cptCodesTable1.Rows[i]["Description"];
                    row[2] = cptCodesTable1.Rows[i]["DisplayOrder"];
                    cptCodeDT.Rows.Add(row);

                    if (!cptCodesDictionary.ContainsKey(cptCodesTable1.Rows[i]["CPTCode"].ToString()))
                        cptCodesDictionary.Add(cptCodesTable1.Rows[i]["CPTCode"].ToString(), Convert.ToInt32(cptCodesTable1.Rows[i]["CPTCodeId"]));
                }

                CPTCodesCheckBoxList.DataSource = cptCodeDT;
                CPTCodesCheckBoxList.DataTextField = "CPTDisplayText";
                CPTCodesCheckBoxList.DataValueField = "CPTCodeId";
                CPTCodesCheckBoxList.DataBind();

                Session["CPTCodesDictionary"] = cptCodesDictionary;

                //string str = "";
                //foreach (int key in cptCodesDictionary.Keys)
                //{
                //    str += "Key :" + key + " Value :" + cptCodesDictionary[key] + "\r\n"; 
                //}

                //CurrentClientLabel.Visible = true;
                //CurrentClientLabel.Text = str;

                #endregion CPT Codes Modal Window

                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void SaveData()
        {
            //Exception Handled from the calling method
            //Commented out to normalize service type Data in service Daniel 5/31/2017
            //int[] serviceTypeIds = new int[ServiceTypeCheckBoxList.Items.Count];
            int clientId = 0;
            //int j = 0;
            string clinicianNetIds = string.Empty;

            //for (int i = 0; i < ServiceTypeCheckBoxList.Items.Count; i++)
            //{
            //    if (ServiceTypeCheckBoxList.Items[i].Selected)
            //    {
            //        serviceTypeIds[j] = Convert.ToInt32(ServiceTypeCheckBoxList.Items[i].Value);
            //        j++;
            //    }
            //}

            clientId = Convert.ToInt32(Session["ClientID"]);//Convert.ToInt32(GridView2.Rows[0].Cells[0].Text);

            #region Clinicians

            Dictionary<string, string> tempCliniciansDict = new Dictionary<string, string>();
            tempCliniciansDict = (Dictionary<string, string>)Session["CliniciansDictionary"];
            if (ClinicianTextBox.Text != "")
            {
                //string[] tempClinicians = ClinicianHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] tempClinicians = ClinicianTextBox.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tempClinicians.Length; i++)
                {
                    if (string.IsNullOrEmpty(clinicianNetIds))
                        clinicianNetIds = tempCliniciansDict[tempClinicians[i].Trim()] + ",";
                    else
                        clinicianNetIds = clinicianNetIds + tempCliniciansDict[tempClinicians[i].Trim()] + ",";
                }
            }
            else
            {
                //clinicianNetIds = null; ClinicianNeid should be assigned by the loginid 5/19/15 Daniel
                clinicianNetIds = Session["ATUID"].ToString() + ",";
            }
            #endregion Clinicians

            #region DatabaseCalling Code
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            //CPT Codes
            #region CPTCodes
            string cptCodesString = string.Empty;
            Dictionary<string, int> tempCPTDict = new Dictionary<string, int>();
            tempCPTDict = (Dictionary<string, int>)Session["CPTCodesDictionary"];
            if (CPTCodeListTextBox.Visible)
            {
                string[] tempCPTCodes = HiddenCPTCodes.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tempCPTCodes.Length; i++)
                {
                    if (string.IsNullOrEmpty(cptCodesString))
                        cptCodesString = tempCPTDict[tempCPTCodes[i]] + ",";
                    else
                        cptCodesString = cptCodesString + tempCPTDict[tempCPTCodes[i]] + ",";
                }
            }
            else if (DropDownList1.Visible && !DropDownList1.SelectedItem.Text.Trim().Equals("--Choose one--"))
            {
                cptCodesString = DropDownList1.SelectedValue + ",";
            }
            #endregion CPTCodes

            //con.Open(); Move down after if statement
            int returnValue = 0;
            //Commented out to normalize service type Data in service Daniel 5/31/2017
            //foreach (int value in serviceTypeIds)
            //{
            //if (value > 0)
            //Added below to normalize service type Data in service Daniel 5/31/2017

            //Email Admin            
            List<String> referralServiceTypes = new List<String>();
            //List<String> serviceServiceTypes = new List<String>(); 
            String extraServiceTypes = String.Empty;
            if (Session["referralServiceTypes"] != null)
                referralServiceTypes = (List<String>)Session["referralServiceTypes"];
            Boolean flagExtraServiceTypes = false;

            if (Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !ddlist_tasks.SelectedValue.Equals("Choose") && !ActionDatesDropDownList.SelectedValue.Equals("Choose"))
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

                        if (!referralServiceTypes.Contains(ServiceTypeCheckBoxList.Items[i].Text))
                        {
                            flagExtraServiceTypes = true;
                            if (string.IsNullOrEmpty(extraServiceTypes))
                                extraServiceTypes = ServiceTypeCheckBoxList.Items[i].Text + ",";
                            else
                                extraServiceTypes = extraServiceTypes + ServiceTypeCheckBoxList.Items[i].Text + ",";
                        }
                    }
                }
                //Move below after saving successfully Daniel 5/1/2018
                //if (flagExtraServiceTypes)
                //    NotifyCaseManager(extraServiceTypes, Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 1);

                //if (ddlist_tasks.SelectedValue.Equals("3") && ActionDatesDropDownList.SelectedValue.Equals("5"))
                //    NotifyCaseManager(Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 2);

                //End of Added
                #region Service
                cmd = new SqlCommand("[usp_ServiceInfo_Insert]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //Parameters
                cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(ReferralDateDropDownList.SelectedValue));
                //cmd.Parameters.AddWithValue("@ServiceTypeId", value);
                cmd.Parameters.AddWithValue("@ServiceTypeIds", serviceTypeIdString);
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@TaskId", Convert.ToInt32(ddlist_tasks.SelectedValue));
                cmd.Parameters.AddWithValue("@ActionId", Convert.ToInt32(ActionDatesDropDownList.SelectedValue));
                cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(ActionDatesTextBox.Text));
                //Input Date has default value
                cmd.Parameters.AddWithValue("@InputNetId", Session["ATUID"].ToString());

                cmd.Parameters.AddWithValue("@ClinicianNetIds", clinicianNetIds);

                if (ActionDatesDropDownList.SelectedItem.Text.Equals("LOMN Written"))
                {
                    cmd.Parameters.AddWithValue("@LOMNCOntactId", DoctorDropDownList.SelectedValue);
                }
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Evaluation Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }

                //Daniel 9/1/2015 Adding Fabrication Hour
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Fabrication Date"))
                {
                    if (!string.IsNullOrEmpty(FabTextBox.Text))
                        cmd.Parameters.AddWithValue("@FabHour", Convert.ToDecimal(FabTextBox.Text));
                }
                //--- Fabrication Hour end
                //Daniel 10/6/2015 Implementation Billing data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Implementation minutes end

                //Consult Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Consult Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Consult minutes end
                //Training Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Training Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Training minutes end
                //Follow-Up Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Follow-Up Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Follow-Up minutes end
                //Implementation Performed Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Performed"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Implementation Performed minutes end

                if (ActionDatesDropDownList.SelectedItem.ToString().Trim().Equals("Referral Case Cancelled"))
                {
                    cmd.Parameters.AddWithValue("@CaseReasonId", Convert.ToInt32(CaseClosedReasonDropDownList.SelectedValue));
                    if (CaseClosedReasonDropDownList.SelectedItem.ToString().Trim().Equals("Other"))
                        cmd.Parameters.AddWithValue("@OtherReason", OtherReasonTextBox.Text);
                }

                SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                serviceId.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                if (cmd.Parameters["ReturnValue"] != null)
                {
                    returnValue = Convert.ToInt32(serviceId.Value);
                }
                #endregion Service
            }
            //}

            #endregion DatabaseCalling Code

            if (returnValue > 0)
            {
                Response.Write("<script language=javascript>alert('Saved Successfully.')</script>");

                if (flagExtraServiceTypes)
                    NotifyCaseManager(extraServiceTypes, Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 1);
                if (ddlist_tasks.SelectedValue.Equals("3") && ActionDatesDropDownList.SelectedValue.Equals("31"))
                    NotifyCaseManager(Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 2);

                chkbx_ShowClinicians.Checked = false;

                CliniciansCheckBoxList.Visible = false;
                for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
                {
                    ListItem item = CliniciansCheckBoxList.Items[i];
                    if (item.Selected)
                        item.Selected = false;
                }

                ClinicianTextBox.Visible = false;
            }
            else
            {
                Response.Write("<script language=javascript>alert('Not able to insert records!!!')</script>");
            }
            con.Close();

        }


        protected void UpdateData()
        {
            //Exception Handled from the calling method
            //Commented out to normalize service type Data in service Daniel 5/31/2017            
            int clientId = 0;
            string clinicianNetIds = string.Empty;

            clientId = Convert.ToInt32(Session["ClientID"]);//Convert.ToInt32(GridView2.Rows[0].Cells[0].Text);

            #region Clinicians

            Dictionary<string, string> tempCliniciansDict = new Dictionary<string, string>();
            tempCliniciansDict = (Dictionary<string, string>)Session["CliniciansDictionary"];
            if (ClinicianTextBox.Text != "")
            {
                //string[] tempClinicians = ClinicianHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] tempClinicians = ClinicianTextBox.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tempClinicians.Length; i++)
                {
                    if (string.IsNullOrEmpty(clinicianNetIds))
                        clinicianNetIds = tempCliniciansDict[tempClinicians[i].Trim()] + ",";
                    else
                        clinicianNetIds = clinicianNetIds + tempCliniciansDict[tempClinicians[i].Trim()] + ",";
                }
            }
            else
            {
                //clinicianNetIds = null; ClinicianNeid should be assigned by the loginid 5/19/15 Daniel
                clinicianNetIds = Session["ATUID"].ToString() + ",";
            }
            #endregion Clinicians

            #region DatabaseCalling Code
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            //CPT Codes
            #region CPTCodes
            string cptCodesString = string.Empty;
            Dictionary<string, int> tempCPTDict = new Dictionary<string, int>();
            tempCPTDict = (Dictionary<string, int>)Session["CPTCodesDictionary"];
            if (CPTCodeListTextBox.Visible)
            {
                string[] tempCPTCodes = HiddenCPTCodes.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tempCPTCodes.Length; i++)
                {
                    if (string.IsNullOrEmpty(cptCodesString))
                        cptCodesString = tempCPTDict[tempCPTCodes[i]] + ",";
                    else
                        cptCodesString = cptCodesString + tempCPTDict[tempCPTCodes[i]] + ",";
                }
            }
            else if (DropDownList1.Visible && !DropDownList1.SelectedItem.Text.Trim().Equals("--Choose one--"))
            {
                cptCodesString = DropDownList1.SelectedValue + ",";
            }
            #endregion CPTCodes

            //con.Open(); Move down after if statement
            //int returnValue = 0;
            //Commented out to normalize service type Data in service Daniel 5/31/2017
            //foreach (int value in serviceTypeIds)
            //{
            //if (value > 0)
            //Added below to normalize service type Data in service Daniel 5/31/2017
            //Email Admin            
            List<String> referralServiceTypes = new List<String>();
            //List<String> serviceServiceTypes = new List<String>(); 
            String extraServiceTypes = String.Empty;
            if (Session["referralServiceTypes"] != null)
                referralServiceTypes = (List<String>)Session["referralServiceTypes"];
            Boolean flagExtraServiceTypes = false;

            if (Session != null && Session["ClientID"] != null && !ReferralDateDropDownList.SelectedValue.Equals("Choose") && !ddlist_tasks.SelectedValue.Equals("Choose") && !ActionDatesDropDownList.SelectedValue.Equals("Choose"))
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

                        if (!referralServiceTypes.Contains(ServiceTypeCheckBoxList.Items[i].Text))
                        {
                            flagExtraServiceTypes = true;
                            if (string.IsNullOrEmpty(extraServiceTypes))
                                extraServiceTypes = ServiceTypeCheckBoxList.Items[i].Text + ",";
                            else
                                extraServiceTypes = extraServiceTypes + ServiceTypeCheckBoxList.Items[i].Text + ",";
                        }
                    }
                }

                //if (flagExtraServiceTypes)
                //    NotifyCaseManager(extraServiceTypes, Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 1);

                //if (ddlist_tasks.SelectedValue.Equals(3) && ActionDatesDropDownList.SelectedValue.Equals(5))
                //    NotifyCaseManager(Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 2);
                //End of Added
                #region Service
                cmd = new SqlCommand("[usp_ServiceInfo_Update]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //Parameters
                cmd.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(Session["ServiceId"]));
                cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(ReferralDateDropDownList.SelectedValue));
                //cmd.Parameters.AddWithValue("@ServiceTypeId", value);
                cmd.Parameters.AddWithValue("@ServiceTypeIds", serviceTypeIdString);
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@TaskId", Convert.ToInt32(ddlist_tasks.SelectedValue));
                cmd.Parameters.AddWithValue("@ActionId", Convert.ToInt32(ActionDatesDropDownList.SelectedValue));
                cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(ActionDatesTextBox.Text));
                //Input Date has default value
                cmd.Parameters.AddWithValue("@InputNetId", Session["InputBy"].ToString());

                cmd.Parameters.AddWithValue("@ClinicianNetIds", clinicianNetIds);

                if (ActionDatesDropDownList.SelectedItem.Text.Equals("LOMN Written"))
                {
                    cmd.Parameters.AddWithValue("@LOMNCOntactId", DoctorDropDownList.SelectedValue);
                }
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Evaluation Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }

                //Daniel 9/1/2015 Adding Fabrication Hour
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Fabrication Date"))
                {
                    if (!string.IsNullOrEmpty(FabTextBox.Text))
                        cmd.Parameters.AddWithValue("@FabHour", Convert.ToDecimal(FabTextBox.Text));
                }
                //--- Fabrication Hour end
                //Daniel 10/6/2015 Implementation Billing data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Implementation minutes end
                //Praharsh
                //Consult Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Consult Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Consult minutes end
                //Training Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Training Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Training minutes end
                //Follow-Up Date Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Follow-Up Date"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Follow-Up minutes end
                //Implementation Performed Billing Data
                if (ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Performed"))
                {
                    cmd.Parameters.AddWithValue("@BillingCPTCodeIds", cptCodesString);
                    if (!string.IsNullOrEmpty(TextBox1.Text))
                        cmd.Parameters.AddWithValue("@BillingUnit", Convert.ToInt32(TextBox1.Text));
                }
                //Implementation Performed minutes end
                if (ActionDatesDropDownList.SelectedItem.ToString().Trim().Equals("Referral Case Cancelled"))
                {
                    cmd.Parameters.AddWithValue("@CaseReasonId", Convert.ToInt32(CaseClosedReasonDropDownList.SelectedValue));
                    if (CaseClosedReasonDropDownList.SelectedItem.ToString().Trim().Equals("Other"))
                        cmd.Parameters.AddWithValue("@OtherReason", OtherReasonTextBox.Text);
                }

                //SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                //serviceId.Direction = ParameterDirection.ReturnValue;

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
                    Response.Write("<script language=javascript>alert('Service Record Updated Successfully.')</script>");

                    if (flagExtraServiceTypes)
                        NotifyCaseManager(extraServiceTypes, Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 1);
                    if (ddlist_tasks.SelectedValue.Equals("3") && ActionDatesDropDownList.SelectedValue.Equals("31"))
                        NotifyCaseManager(Convert.ToString(ReferralDateDropDownList.SelectedItem.Text), Convert.ToString(ActionDatesDropDownList.SelectedItem.Text), 2);


                }
                #endregion Service
            }
            //}

            #endregion DatabaseCalling Code
            //This is for insert code which is not appropriate for Update since it is returning returnParameter.Value Daniel 5/2/2018
            //if (returnValue > 0)
            //{
            //    Response.Write("<script language=javascript>alert('Updated Successfully.')</script>");


            chkbx_ShowClinicians.Checked = false;

            CliniciansCheckBoxList.Visible = false;
            for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
            {
                ListItem item = CliniciansCheckBoxList.Items[i];
                if (item.Selected)
                    item.Selected = false;
            }

            ClinicianTextBox.Visible = false;
            //}
            //else
            //{
            //Response.Write("<script language=javascript>alert('Not able to update records!!!')</script>");
            //}
            con.Close();

            SaveButton.Visible = true;
            UpdateButton.Visible = false;
            CancelUpdateButton.Visible = false;

        }

        private DataTable GetServiceDataTable(int clientId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();

            cmd = new SqlCommand("[usp_GetServiceData]", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ClientId", clientId);
            cmd.Parameters.AddWithValue("@ReferralId", ReferralDateDropDownList.SelectedValue);
            var searchDataTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(searchDataTable);

            /*
            DataTable dt1 = new DataTable();
                        
            dt1.Columns.Add("TypeofService");
            dt1.Columns.Add("Task");
            dt1.Columns.Add("Action");
            dt1.Columns.Add("MDName");
            dt1.Columns.Add("Date");
            dt1.Columns.Add("Clinician");

            //Added because Session["ServiceTypes"] is set to NULL when new search Daniel 6/3/2015
            serviceTypes = new List<string>();
            Session["ServiceTypes"] = serviceTypes;
            //

			for (int i = 0; i < searchDataTable.Rows.Count; i++)
            {

                DataRow row1 = dt1.NewRow();
                string strCommand = null;
                switch (searchDataTable.Rows[i]["Display"].ToString())
                {
                    case "Intake":
                        strCommand = "select Display from LookUpIntakeAction where IntakeActionId = @ActionId";
                        break;
                    case "Evaluation":
                        strCommand = "select Display from LookUpEvalAction where EvalActionId = @ActionId";
                        break;
                    case "Implementation":
                        strCommand = "select Display from LookUpImplementAction where ImplementActionId = @ActionId";
                        break;
                    case "Follow-Up":
                        strCommand = "select Display from LookUpFollowAction where FollowActionId = @ActionId";
                        break;
                    case "Training":
                        strCommand = "select Display from dbo.LookUpTrainingAction where TrainingActionId = @ActionId";
                        break;
                    case "Case Closed":
                        strCommand = "select Display from dbo.LookUpCaseCloseAction where ReferCaseCloseId = @ActionId";
                        break;
                    case "Equipment":
                        strCommand = "select Display from dbo.LookUpEquipmentAction where EquipActionId = @ActionId";
                        break;
                    default:
                        break;
                }
                cmd = new SqlCommand(strCommand, con);
                cmd.Parameters.AddWithValue("@ActionId", searchDataTable.Rows[i]["ActionId"]);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //row1[0] = searchDataTable.Rows[i]["ReferralDate"].ToString();
                    //row1[1] = searchDataTable.Rows[i]["TypeofService"].ToString();
                    //row1[2] = searchDataTable.Rows[i]["Display"].ToString();
                    //row1[3] = reader.GetString(0);//Action
                    //row1[4] = searchDataTable.Rows[i]["FirstName"].ToString();
                    //row1[5] = searchDataTable.Rows[i]["Date"].ToString();
                    row1[0] = searchDataTable.Rows[i]["TypeofService"].ToString();
                    row1[1] = searchDataTable.Rows[i]["Display"].ToString();
                    row1[2] = reader.GetString(0);//Action
                    row1[3] = searchDataTable.Rows[i]["FirstName"].ToString();
                    row1[4] = searchDataTable.Rows[i]["Date"].ToString();
                    row1[5] = searchDataTable.Rows[i]["Clinician"].ToString();
                    dt1.Rows.Add(row1);
                }

                reader.Close();

				List<string> tempServiceTypes = (List<string>)Session["ServiceTypes"];
                if (!tempServiceTypes.Contains(searchDataTable.Rows[i]["TypeofService"].ToString().Trim()))
                {
                    tempServiceTypes.Add(searchDataTable.Rows[i]["TypeofService"].ToString().Trim());
                    Session["ServiceTypes"] = tempServiceTypes;
                }

            }
            */
            return searchDataTable;
        }

        private void Clear()
        {
            ReferralDateDropDownList.SelectedIndex = -1;
            //ServiceTypeListBox.SelectedIndex = -1;
            ServiceTypeCheckBoxList.SelectedIndex = -1;
            //ClinicianListBox.SelectedIndex = -1;
            CliniciansCheckBoxList.SelectedIndex = -1;
            ClinicianTextBox.Text = "";
            ddlist_tasks.SelectedIndex = 0;
            ActionDatesDropDownList.Items.Clear();
            ActionDatesDropDownList.Items.Insert(0, new ListItem("--Choose one--", "0"));
            ActionDatesTextBox.Text = "";
            DoctorDropDownList.SelectedIndex = 0;
            updatepanel_DoctorRow.Visible = false;
            updatepanel_DoctorCheckBoxRow.Visible = false;
            DoctorMessageLabel.Visible = false;
            updatepanel_CaseClosedRow.Visible = false;
            updatepanel_OtherReasonRow.Visible = false;
            updatePanel_BillingInfo.Visible = false;
            updatepanel_BillingInfoRow.Visible = false;
            updatepanel_FabRow.Visible = false;//Clear Fab9/28/2016
            FabTextBox.Text = "";
            ReferralTabSTLabel.Visible = false;
            RadioButtonList1.SelectedIndex = -1;    //Mrinal
            RadioButtonList1.Visible = false;      //Mrinal

            CPTCodeListTextBox.Text = "";
            HiddenCPTCodes.Value = "";
            clinicians.Clear();                     //Mrinal
            ClinicianTextBox.Text = "";
            ClinicianHiddenField.Value = "";
            NoDataLabel.Visible = false;

            ClearDoctorContact();
            NotFoundLabel.Visible = false;
        }

        private void ClearDoctorContact()
        {
            #region Clear Doctor Contact Info

            /*
            DocInfoLabelRow.Visible = false;
            MDFNameRow.Visible = false;
            MDLNameRow.Visible = false;
            NPIRow.Visible = false;
            AddressRow.Visible = false;
            CityRow.Visible = false;
            StateRow.Visible = false;
            ZipRow.Visible = false;
            FaxRow.Visible = false;
            EmailRow.Visible = false;
            DoctorButtonRow.Visible = false;
            */


            MDFNameTextBox.Text = "";
            MDLNameTextBox.Text = "";
            NPITextBox.Text = "";
            AddressTextBox.Text = "";
            CityTextBox.Text = "";
            StateDropDownList.SelectedIndex = 0;
            ZipTextBox.Text = "";
            FaxTextBox.Text = "";
            EmailTextBox.Text = "";
            Session["DoctorID"] = null;
            panel_DoctorInfo.Visible = false;

            #endregion Clear Doctor Contact Info
        }

        private void PopulateReferralDates()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            cmd = new SqlCommand("SELECT ReferralId, convert(char(10), ReferralDate, 101) ReferralDate FROM ClientReferral WHERE ClientID = @ClientId ORDER BY ReferralDate ", con);
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

        private void PopulateDoctorData()
        {
            //Exception handled in the calling method
            var doctorTable = GetDoctorData();
            var tempTable = new DataTable();
            tempTable.Columns.Add("LOMNContactId");
            tempTable.Columns.Add("Display");
            for (int i = 0; i < doctorTable.Rows.Count; i++)
            {
                DataRow row = tempTable.NewRow();
                row["LOMNContactId"] = doctorTable.Rows[i][0];
                string str = doctorTable.Rows[i]["LastName"].ToString() + ", " + doctorTable.Rows[i]["FirstName"].ToString().Substring(0, 1) + ". - " + doctorTable.Rows[i]["City"].ToString();
                row["Display"] = str;
                tempTable.Rows.Add(row);
            }

            DoctorDropDownList.DataSource = tempTable;
            DoctorDropDownList.DataTextField = "Display";
            DoctorDropDownList.DataValueField = "LOMNContactId";
            DoctorDropDownList.DataBind();

            DoctorDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
        }

        private DataTable GetDoctorData()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("select * from dbo.DoctorContact", con);
            cmd.CommandType = CommandType.Text;

            var doctorTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(doctorTable);

            con.Close();

            return doctorTable;
        }

        private void PopulateDateGrid(DataTable dt1)
        {
            if (dt1.Rows.Count > 0)
            {
                GridView3.DataSource = dt1;
                GridView3.DataBind();
                StatusPanel.Visible = true;
                Session["ServiceInfoDataTable"] = dt1;
                GridView3.Visible = true;
                NoDataLabel.Visible = false;
            }
            else
            {
                GridView3.Visible = false;
                GridView3.DataSource = null;
                NoDataLabel.Visible = true;
                StatusPanel.Visible = true;
            }
        }

        ///*
        //private void PopulateDateGrid1(DataTable dt1)
        //{
        //    GridView3.DataSource = null;
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Dates");
        //    dt.Columns.Add("Values");

        //    List<string> tempServiceTypes = (List<string>)Session["ServiceTypes"];

        //    string selectedDate = ActionDatesDropDownList.SelectedItem.ToString();
        //    string actionDateValue = ActionDatesTextBox.Text;

        //    for (int i = 0; i < ServiceTypeCheckBoxList.Items.Count; i++)
        //    {
        //        if (ServiceTypeCheckBoxList.Items[i].Selected)
        //        {
        //            if (!tempServiceTypes.Contains(ServiceTypeCheckBoxList.Items[i].Text.Trim()))
        //            {
        //                tempServiceTypes.Add(ServiceTypeCheckBoxList.Items[i].Text.Trim());
        //                Session["ServiceTypes"] = tempServiceTypes;
        //            }
        //        }
        //    }


        //    Dictionary<string, string> statusDict = new Dictionary<string, string>();

        //    if (dt1.Rows.Count > 0)
        //    {
        //        Adding rows to the data grid with data
        //        for (int i = 0; i < tempServiceTypes.Count; i++)
        //        {
        //            DataRow row = dt.NewRow();
        //            row[0] = "Service Type";
        //            row[1] = tempServiceTypes[i];
        //            statusDict.Clear();
        //            dt.Rows.Add(row);
        //            int flag = 1;
        //            for (int j = 0; j < dt1.Rows.Count; j++)
        //            {
        //                if (!statusDict.ContainsKey(dt1.Rows[j][1].ToString()))
        //                    statusDict.Add(dt1.Rows[j][1].ToString(), tempServiceTypes[i].Trim());
        //                if (dt1.Rows[j][0].ToString().Trim().Equals(tempServiceTypes[i].Trim()))
        //                {
        //                    if (!(statusDict.ContainsKey(dt1.Rows[j][1].ToString()) && statusDict[dt1.Rows[j][1].ToString()].Equals(tempServiceTypes[i].Trim())))
        //                    {
        //                        DataRow row1 = dt.NewRow();
        //                        row1[0] = dt1.Rows[j][1];//Tasks Label
        //                        dt.Rows.Add(row1);
        //                        DataRow row2 = dt.NewRow();
        //                        row2[0] = dt1.Rows[j][2];
        //                        row2[1] = dt1.Rows[j][4];
        //                        dt.Rows.Add(row2);
        //                        if (dt1.Rows[j][2].ToString().Equals("LOMN Written"))
        //                        {
        //                            DataRow row3 = dt.NewRow();
        //                            row3[0] = "Doctor Name";
        //                            row3[1] = dt1.Rows[j][3];
        //                            dt.Rows.Add(row3);
        //                        }
        //                        statusDict.Add(dt1.Rows[j][1].ToString(), tempServiceTypes[i].Trim());
        //                        flag++;
        //                    }
        //                    else
        //                    {
        //                        DataRow row1 = dt.NewRow();
        //                        row1[0] = dt1.Rows[j][2];
        //                        row1[1] = dt1.Rows[j][4];
        //                        dt.Rows.Add(row1);
        //                        if (dt1.Rows[j][2].ToString().Equals("LOMN Written"))
        //                        {
        //                            DataRow row3 = dt.NewRow();
        //                            row3[0] = "Doctor Name";
        //                            row3[1] = dt1.Rows[j][3];
        //                            dt.Rows.Add(row3);
        //                        }
        //                    }
        //                    DataRow dr = dt.NewRow();
        //                    dr[0] = "Clinician";
        //                    dr[1] = dt1.Rows[j][5];
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }

        //        GridView3.DataSource = dt;
        //        GridView3.DataBind();
        //        GridView3.Visible = true;
        //        StatusPanel.Visible = true;
        //        Session["ServiceInfoDataTable"] = dt;
        //        NoDataLabel.Visible = false;
        //    }
        //    else
        //    {
        //        GridView3.Visible = false;
        //        GridView3.DataSource = null;
        //        NoDataLabel.Visible = true;
        //        StatusPanel.Visible = true;
        //    }
        //}
        //*/

        protected void ActionDateCustomValidator_Validate(object sender, ServerValidateEventArgs e)
        {
            DateTime d;
            e.IsValid = DateTime.TryParseExact(e.Value, "mm/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        }

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            CurrentClientValueLabel.Text = firstName + " " + lastName;
            Session["ClientName"] = CurrentClientValueLabel.Text;
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

        private bool CheckInValidCPTCode()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            cmd = new SqlCommand("SELECT NoCPTCode FROM dbo.LookUpCPTCodes WHERE CPTCodeId = @CPTCodeId", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@CPTCodeId", Convert.ToInt32(DropDownList1.SelectedValue));

            var cptCodeTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(cptCodeTable);

            if (cptCodeTable.Rows.Count > 0 && Convert.ToBoolean(cptCodeTable.Rows[0][0]))
                return true;
            else
                return false;
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
                lastSelectedValue = CliniciansCheckBoxList.Items[index].ToString();
                isChecked = true;
            }
            else
            {
                lastSelectedValue = CliniciansCheckBoxList.Items[index].ToString();
                isChecked = false;
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

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Guidance.aspx");
        }

        protected void GridView3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[6].Controls[0]).OnClientClick = "if (!window.confirm('Are you sure you want to delete this item?')) return false;"; // add any JS you want here
            }

            foreach (GridViewRow row in GridView3.Rows)
            {
                Label lbl_LomnWritten = (row.Cells[0].FindControl("lbl_ActionName") as Label);
                string str = lbl_LomnWritten.Text;

                if (str.Trim().Equals("LOMN Written"))
                {
                    LinkButton lb = new LinkButton();
                    string DocID = (row.Cells[0].FindControl("lbl_DoctorID") as Label).Text;
                    //lb.ID = "DoctorLink_" + DocID;

                    //Appending the InputByNetId to Enable/Disable Edit Button
                    string InputByNetId = (row.Cells[0].FindControl("lbl_InputNetId") as Label).Text;
                    lb.ID = "DoctorLink_" + DocID + "_" + InputByNetId;


                    lb.ToolTip = "Click to view Doctor Contact.";
                    lb.Click += new EventHandler(lb_Click);
                    lb.Font.Size = FontUnit.Small;

                    lbl_LomnWritten.Visible = false;
                    lb.Text = str;
                    //if (row.Cells[2].FindControl("DoctorLink_" + DocID) == null)
                    if (row.Cells[3].FindControl("DoctorLink_" + DocID + "_" + InputByNetId) == null)
                        row.Cells[3].Controls.Add(lb);
                }
            }
        }

        protected void GridView3_DataBound(object sender, EventArgs e)
        {
            if (Session != null)
            {
                if (!Convert.ToBoolean(Session["IsAdmin"]))
                {
                    foreach (GridViewRow row in GridView3.Rows)
                    {
                        if (row.Cells[0].FindControl("lbl_InputNetId") != null)
                        {
                            string inputNetId = (row.Cells[0].FindControl("lbl_InputNetId") as Label).Text;
                            if (!inputNetId.Equals(Convert.ToString(Session["ATUID"])))
                            {
                                row.Cells[0].Enabled = false;
                                row.Cells[6].Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        protected void GridView3_RowEdit(object sender, GridViewEditEventArgs e)
        {
            DoctorInfoLabel.Visible = false;
            GridView4.DataSource = GridView5.DataSource = null;
            GridView4.DataBind();
            GridView5.DataBind();
            SaveButton.Visible = false;
            UpdateButton.Visible = true;
            CancelUpdateButton.Visible = true;

            int serviceId = Convert.ToInt32(GridView3.DataKeys[e.NewEditIndex].Value);
            Session["ServiceId"] = serviceId;
            ClinicianTextBox.Text = String.Empty;
            clinicians.Clear();
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("usp_GetServiceDataByServiceId", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ReferralId", ReferralDateDropDownList.SelectedValue);
            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));
            cmd.Parameters.AddWithValue("@ServiceId", serviceId);

            var serviceTypeTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(serviceTypeTable);
            String followTypeId = String.Empty;

            if (serviceTypeTable.Rows.Count > 0)
            {
                foreach (DataRow row in serviceTypeTable.Rows)
                {
                    ddlist_tasks.SelectedValue = Convert.ToString(row["TaskId"]);

                    if (ddlist_tasks.SelectedItem.ToString().Equals("Follow-Up"))
                    {
                        followTypeId = Convert.ToString(row["FollowTypeId"]);
                    }
                    if (Convert.ToString(followTypeId).Equals("1"))
                    {
                        RadioButtonList1.SelectedIndex = 0;
                    }
                    else if (Convert.ToString(followTypeId).Equals("2"))
                    {
                        RadioButtonList1.SelectedIndex = 1;
                    }

                    TaskChanged();

                    if (Convert.ToString(row["TaskId"]).Equals("3") && Convert.ToString(row["Action"]).Equals("5"))
                    {
                        ActionDatesDropDownList.SelectedValue = "0";
                    }
                    else
                    {
                        ActionDatesDropDownList.SelectedValue = Convert.ToString(row["Action"]);
                    }
                    //ActionDatesDropDownList.SelectedValue = Convert.ToString(row["Action"]); This would not work for deactivated service date Implementation Completed Daniel 9/14/18

                    ActionDatesTextBox.Text = Convert.ToString(row["Date"]);
                    FabTextBox.Text = Convert.ToString(row["Hour"]);


                    String serviceTypeIds = Convert.ToString(row["ServiceTypeIds"]);
                    List<String> serviceTypes = serviceTypeIds.Split(',').ToList();

                    foreach (ListItem item in ServiceTypeCheckBoxList.Items)
                    {
                        item.Selected = serviceTypes.Contains(item.Value);
                    }
                    ActionDateChanged();

                    String billingCodeIds = Convert.ToString(row["BillingCodeIds"]);
                    List<String> billingCodesId = billingCodeIds.Split(',').ToList();
                    List<String> billingCodes = new List<String>();
                    if (serviceTypes.Count == 1)
                    {
                        HiddenCPTCodes.Value = billingCodeIds;
                        foreach (ListItem item in CPTCodesCheckBoxList.Items)
                        {
                            item.Selected = billingCodesId.Contains(item.Value);
                            if (item.Selected)
                            {
                                string[] codeText;
                                codeText = item.Text.Split('-');
                                billingCodes.Add(codeText[0].Trim());
                            }
                        }
                    }
                    else
                    {
                        if (billingCodesId.Count > 0)
                        {
                            if (!billingCodesId[0].Equals(String.Empty))
                                DropDownList1.SelectedValue = billingCodesId[0];
                        }
                    }
                    if (ActionDatesDropDownList.SelectedItem.Text.Equals("Evaluation Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Recheck Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Consult Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Training Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Follow-Up Date")
                        || ActionDatesDropDownList.SelectedItem.Text.Equals("Implementation Performed"))
                    {
                        for (int i = 0; i < billingCodes.Count; i++)
                        {
                            if (i == 0)
                                CPTCodeListTextBox.Text = billingCodes[i];
                            else
                            {
                                CPTCodeListTextBox.Text += "\n";
                                CPTCodeListTextBox.Text += billingCodes[i];
                            }
                        }

                        TextBox1.Text = Convert.ToString(row["Minutes"]);
                    }

                    if (ActionDatesDropDownList.SelectedItem.Text.Equals("Referral Case Cancelled"))
                    {
                        CaseClosedReasonDropDownList.SelectedValue = Convert.ToString(row["ReferralCloseReasonId"]);
                        CaseCloseReasonChange();
                        if (CaseClosedReasonDropDownList.SelectedItem.Text.Equals("Other"))
                        {
                            OtherReasonTextBox.Text = Convert.ToString(row["ReasonDescription"]);
                        }
                    }

                    Session["InputBy"] = Convert.ToString(row["InputNetId"]);

                    String clinicianNetIds = Convert.ToString(row["ClinicianNetId"]);
                    List<String> cliniciansNetIdsList = clinicianNetIds.Split(',').ToList();

                    foreach (ListItem item in CliniciansCheckBoxList.Items)
                    {
                        item.Selected = cliniciansNetIdsList.Contains(item.Value);
                        if (item.Selected)
                            clinicians.Add(item.Text);
                    }
                    ShowCliniciansChanged();
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

                    if (!Convert.ToString(row["LOMNContactId"]).Equals(String.Empty))
                    {
                        DoctorDropDownList.SelectedValue = Convert.ToString(row["LOMNContactId"]);
                    }
                }
            }
        }

        protected void GridView3_RowDelete(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int serviceId = Convert.ToInt32(GridView3.DataKeys[e.RowIndex].Value);

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("usp_ServiceInfo_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
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
                    Response.Write("<script language=javascript>alert('Service Record Deleted Successfully.')</script>");
                }
                con.Close();

                ReferralDateChanged();
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void GridView4_RowEdit(object sender, GridViewEditEventArgs e)
        {
            try
            {
                #region DB
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();

                cmd = new SqlCommand("[usp_GetDoctorContactDetails]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LOMNContactId", Convert.ToString(GridView4.DataKeys[e.NewEditIndex].Value));

                var doctorDetails = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(doctorDetails);

                if (doctorDetails.Rows.Count > 0)
                {
                    foreach (DataRow row in doctorDetails.Rows)
                    {
                        ClearDoctorContact();
                        panel_DoctorInfo.Visible = true;
                        Session["DoctorID"] = Convert.ToString(row["LOMNContactId"]);
                        MDFNameTextBox.Text = Convert.ToString(row["FirstName"]);
                        MDLNameTextBox.Text = Convert.ToString(row["LastName"]);
                        NPITextBox.Text = Convert.ToString(row["NPI"]);
                        AddressTextBox.Text = Convert.ToString(row["Address"]);
                        CityTextBox.Text = Convert.ToString(row["City"]);
                        if (!Convert.ToString(row["State"]).Equals(String.Empty))
                            StateDropDownList.SelectedValue = Convert.ToString(row["State"]);
                        ZipTextBox.Text = Convert.ToString(row["Zip"]);
                        FaxTextBox.Text = Convert.ToString(row["Fax"]);
                        EmailTextBox.Text = Convert.ToString(row["Email"]);
                    }
                }

                con.Close();
                DoctorSaveButton.Text = "Update Doctor Contact";
                #endregion DB
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void GridView4_DataBound(object sender, EventArgs e)
        {
            if (Session != null)
            {
                if (!Convert.ToBoolean(Session["IsAdmin"]))
                {
                    foreach (GridViewRow row in GridView4.Rows)
                    {
                        if (Session["InputBy"] != null)
                        {
                            string inputNetId = Convert.ToString(Session["InputBy"]);
                            if (!inputNetId.Equals(Convert.ToString(Session["ATUID"])))
                            {
                                row.Cells[0].Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        protected void NotifyCaseManager(String extraServiceTypes, String referralDate, String actionType, int purpose)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();
            try
            {
                #region Email CaseManager
                //string strCommand = "select * from LookUpEmailHeaders where PurposeId = 1"; paramatize
                string strCommand = "select * from LookUpEmailHeaders where PurposeId =@PurposeId";


                cmd = new SqlCommand(strCommand, con);
                cmd.CommandType = CommandType.Text;
                //Added Daniel 4/30/2018 to paramatize
                cmd.Parameters.AddWithValue("@PurposeId", purpose);

                var dt = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = Convert.ToString(dt.Rows[0]["smtpClient"]);
                    MailMessage mail = new MailMessage();

                    //////Setting From , To and CC
                    String fromEmail = Convert.ToString(Session["ATUID"]) + "@uic.edu";
                    mail.From = new MailAddress(fromEmail, "ATU Service");                  //Comment this line at the time of testing.
                    //mail.From = new MailAddress(Convert.ToString(dt.Rows[0]["toEmail"]), "ATU Service");   ///Uncomment this line to make the from email id as the to email id.
                    mail.To.Add(Convert.ToString(dt.Rows[0]["toEmail"]));
                    if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["ccEmail"])))
                        mail.CC.Add(Convert.ToString(dt.Rows[0]["ccEmail"]));
                    mail.CC.Add(new MailAddress(fromEmail));                                //Comment this line at the time of testing.
                    mail.Subject = Convert.ToString(dt.Rows[0]["emailSubject"]);

                    StringBuilder sb = new StringBuilder();

                    //Company Logo
                    string applicationPath = HttpContext.Current.Request.MapPath(Request.ApplicationPath);
                    string physicalPath = applicationPath + @"\images\Logo.bmp";

                    Attachment oAttachment = new Attachment(physicalPath);

                    oAttachment.ContentId = "logo";
                    mail.Attachments.Add(oAttachment);

                    sb.AppendFormat("<img src='{0}' style='width: 448px; height: 86px' />", "cid:logo");
                    sb.AppendFormat("<br /><h3>{0}</h3><br/>", Convert.ToString(dt.Rows[0]["response"]));
                    sb.Append("<table border='1px' style='text-align:center; width:30%;'>");
                    sb.AppendFormat("<tr><td><b>Referral Client</b></td><td>{0}</td></tr>", Convert.ToString(Session["ClientName"])); //Session["ClientName"] is populated in page load or PopulateSessionVariables
                    //sb.AppendFormat("<tr><td><b>Referral Client</b></td><td>{0}</td></tr>", Convert.ToString(Session["FirstName"]) + " " + Convert.ToString(Session["LastName"])); // No Need for above solution 10/16/17 Daniel
                    sb.AppendFormat("<tr><td><b>Referral Date</b></td><td>{0}</td></tr>", referralDate);
                    sb.AppendFormat("<tr><td><b>Action</b></td><td>{0}</td></tr>", actionType);
                    sb.AppendFormat("<tr><td><b>Input By</b></td><td>{0}</td></tr>", Convert.ToString(Session["ATUID"]));
                    sb.AppendFormat("<tr><td><b>Extra Service Types</b></td><td>{0}</td></tr>", extraServiceTypes);


                    //sb.AppendFormat("<tr><td><b>Assitantship</b></td><td>{0}</td></tr>", ddlAssistantship.SelectedItem);
                    //sb.AppendFormat("<tr><td><b>Time</b></td><td>{0}</td></tr>", DateTime.Now);
                    //sb.AppendFormat("<tr><td>Description:</td><td>{0}</td></tr>", txtComments.Text.Trim());
                    //sb.AppendFormat("<tr><td>Email is:</td><td>{0}</td></tr>", txtemail.Text.Trim());
                    sb.Append("</table>");

                    mail.Body = Convert.ToString(sb);
                    mail.IsBodyHtml = true;
                    smtpClient.Send(mail);


                    #endregion CaseManager
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            con.Close();
        }


        protected void NotifyCaseManager(String referralDate, String actionType, int purpose)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();
            try
            {
                #region Email CaseManager
                //string strCommand = "select * from LookUpEmailHeaders where PurposeId = 1"; paramatize
                string strCommand = "select * from LookUpEmailHeaders where PurposeId =@PurposeId";


                cmd = new SqlCommand(strCommand, con);
                cmd.CommandType = CommandType.Text;
                //Added Daniel 4/30/2018 to paramatize
                cmd.Parameters.AddWithValue("@PurposeId", purpose);

                var dt = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = Convert.ToString(dt.Rows[0]["smtpClient"]);
                    MailMessage mail = new MailMessage();

                    //////Setting From , To and CC
                    String fromEmail = Convert.ToString(Session["ATUID"]) + "@uic.edu";
                    mail.From = new MailAddress(fromEmail, "ATU Service");                  //Comment this line at the time of testing.
                    //mail.From = new MailAddress(Convert.ToString(dt.Rows[0]["toEmail"]), "ATU Service");   ///Uncomment this line to make the from email id as the to email id.
                    mail.To.Add(Convert.ToString(dt.Rows[0]["toEmail"]));
                    if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["ccEmail"])))
                        mail.CC.Add(Convert.ToString(dt.Rows[0]["ccEmail"]));
                    mail.CC.Add(new MailAddress(fromEmail));                                //Comment this line at the time of testing.
                    mail.Subject = Convert.ToString(dt.Rows[0]["emailSubject"]);

                    StringBuilder sb = new StringBuilder();

                    //Company Logo
                    string applicationPath = HttpContext.Current.Request.MapPath(Request.ApplicationPath);
                    string physicalPath = applicationPath + @"\images\Logo.bmp";

                    Attachment oAttachment = new Attachment(physicalPath);

                    oAttachment.ContentId = "logo";
                    mail.Attachments.Add(oAttachment);

                    sb.AppendFormat("<img src='{0}' style='width: 448px; height: 86px' />", "cid:logo");
                    sb.AppendFormat("<br /><h3>{0}</h3><br/>", Convert.ToString(dt.Rows[0]["response"]));
                    sb.Append("<table border='1px' style='text-align:center; width:30%;'>");
                    sb.AppendFormat("<tr><td><b>Referral Client</b></td><td>{0}</td></tr>", Convert.ToString(Session["ClientName"])); //Session["ClientName"] is populated in page load or PopulateSessionVariables
                    //sb.AppendFormat("<tr><td><b>Referral Client</b></td><td>{0}</td></tr>", Convert.ToString(Session["FirstName"]) + " " + Convert.ToString(Session["LastName"])); // No Need for above solution 10/16/17 Daniel
                    sb.AppendFormat("<tr><td><b>Referral Date</b></td><td>{0}</td></tr>", referralDate);
                    sb.AppendFormat("<tr><td><b>Action</b></td><td>{0}</td></tr>", actionType);
                    sb.AppendFormat("<tr><td><b>Input By</b></td><td>{0}</td></tr>", Convert.ToString(Session["ATUID"]));


                    //sb.AppendFormat("<tr><td><b>Assitantship</b></td><td>{0}</td></tr>", ddlAssistantship.SelectedItem);
                    //sb.AppendFormat("<tr><td><b>Time</b></td><td>{0}</td></tr>", DateTime.Now);
                    //sb.AppendFormat("<tr><td>Description:</td><td>{0}</td></tr>", txtComments.Text.Trim());
                    //sb.AppendFormat("<tr><td>Email is:</td><td>{0}</td></tr>", txtemail.Text.Trim());
                    sb.Append("</table>");

                    mail.Body = Convert.ToString(sb);
                    mail.IsBodyHtml = true;
                    smtpClient.Send(mail);


                    #endregion CaseManager
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            con.Close();
        }


    }
}




//https://forums.asp.net/t/1442129.aspx?How+to+only+allow+certain+rows+of+gridview+to+have+Delete+