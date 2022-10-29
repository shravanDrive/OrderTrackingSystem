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
    public partial class ClientInsurance : System.Web.UI.Page
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
                    CurrentClientLabel.Visible = true;
                    CurrentClientValueLabel.Visible = true;
                }
                if (Session != null && Session["LastName"] != null)
                {
                    LastNameText.Text = Session["LastName"].ToString();
                    CurrentClientValueLabel.Text = CurrentClientValueLabel.Text + " " + Session["LastName"].ToString();
                }
                if (Session != null && Session["InsuranceInfoGrid"] != null)
                {
                    DataTable gridTable = (DataTable)Session["InsuranceInfoGrid"];
                    AddressInfoGridView.DataSource = gridTable;
                    AddressInfoGridView.DataBind();
                    AddressInfoGridView.Visible = true;
                }
                else
                {
                    if (Session != null && Session["ClientID"] != null)
                    {
                        //string clientID = Session["ClientID"].ToString();
                        PopulateAllContacts();
                        //GridView1.Visible = false;
                    }
                }
                //if (Session != null && Session["ClientGrid"] != null)
                //{
                //    GridView2.DataSource = (DataTable)Session["ClientGrid"];
                //    GridView2.DataBind();
                //    GridView2.Visible = true;
                //}
            }
            #region Link Button on page refresh
            if (Session != null && Session["ClientFunding"] != null)
            {
                AddressInfoGridView.DataSource = (DataTable)Session["ClientFunding"];
                foreach (GridViewRow gvr in AddressInfoGridView.Rows)
                {
                    string str = gvr.Cells[0].Text;
                    if (str.Contains("Insurance #"))
                    {
                        LinkButton lb = new LinkButton();
                        lb.ID = "ContactInfoLink";
                        lb.ToolTip = "Click to view contact information";
                        lb.Click += new EventHandler(lb_Click);


                        lb.Text = gvr.Cells[0].Text;

                        if (gvr.Cells[0].FindControl("ContactInfoLink") == null)
                            gvr.Cells[0].Controls.Add(lb);
                    }
                }
            }

            #endregion Link Button on page refresh
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
                AddressInfoGridView.Visible = false;
                AddressInfoGridView1.DataSource = null;
                AddressInfoGridView1.Visible = false;
                ContactInfoLabel.Visible = false;
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
                    string ssn = searchDataTable.Rows[0]["SSN"].ToString();
                    Session["ClientID"] = clientID;
                    Session["SSN"] = ssn;

                    PopulateSessionVariables(searchDataTable.Rows[0]["FirstName"].ToString(), searchDataTable.Rows[0]["LastName"].ToString());
                    GridView1.Visible = false;
                    // 10/14/2015 Daniel remove Not found message if displayed 
                    NotFoundLabel.Visible = false;

                    PopulateAllContacts();
                    AddressInfoGridView1.DataSource = null;
                    AddressInfoGridView1.Visible = false;
                    ContactInfoLabel.Visible = false;
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
                AddressInfoGridView.Visible = false;
                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
            }
            #endregion Populate Grid

            if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicare") && Session != null && Session["SSN"] != null)
            {
                GroupTextBox.Text = Session["SSN"].ToString();
            }
            PopulateAllContacts();
            AddressInfoGridView1.DataSource = null;
            AddressInfoGridView1.Visible = false;
            ContactInfoLabel.Visible = false;
            //Added on 2/26/18 Daniel
            ClearInputArea();
        }

        protected void Carrier_Checked(object sender, EventArgs e)
        {
            if (CarrierCheckBox.Checked)
            {
                NewICarrierRow.Visible = true;
                NewICarrierTextRow.Visible = true;
            }
            else
            {
                NewICarrierRow.Visible = false;
                NewICarrierTextRow.Visible = false;
                NewICarrierTextBox.Text = "";
                NewICarrierSaveLabel.Visible = false;
                ClearNewInsurance();
            }
        }

        protected void InsuranceType_Changed(object sender, EventArgs e)
        {
            SaveButton.Enabled = true;

            #region Dynamic Controls

            CarrierRow.Visible = true;
            PlanRow.Visible = true;
            SaveButton.Enabled = true;

            //Changed if condition Daniel 2/26/18
            //if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicare")
            //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicaid")
            //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("Private Insurance")
            //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("IL-DHS-DRS HS")
            //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("IL-DHS-DRS VR"))
            if (!InsuranceTypeDropDownList.SelectedValue.Equals("Choose"))
            {
                GroupRow.Visible = true;
                InsuranceIdRow.Visible = true;
                NewInsuranceTable.Visible = false;
                MessageLabelRow.Visible = false;

                //NewCarrierLabel.Text = "Carrier :";
                NewPlanLabel.Text = "Plan :";
                NewPlanTextBox.ReadOnly = true;
                //NewCarrierTextBox.ReadOnly = true;
                //AddressInfoGridView.Visible = false;
            }
            //Seems to be no longer valid Daniel 2/23/18
            //else if (!InsuranceTypeDropDownList.SelectedValue.Equals("Choose"))
            //{
            //    //SaveButton.Enabled = false;
            //    //CarrierRow.Visible = false;
            //    //PlanRow.Visible = false;
            //    GroupRow.Visible = false;
            //    InsuranceIdRow.Visible = false;


            //if (!GetNonInsuranceContactRecord())
            //{
            //    NewInsuranceTable.Visible = true;
            //    MessageLabelRow.Visible = true;
            //    MessageLabel.Text = "Contact Information for the selected Client and Payer Type is missing. Please enter.";

            //    //NewCarrierLabel.Text = "Entity Name :";
            //    //NewCarrierTextBox.ReadOnly = false;
            //    NewPlanTextBox.Text = "";
            //    //NewCarrierTextBox.Text = "";
            //    NewPlanLabel.Text = "Contact Name :";
            //    NewPlanTextBox.ReadOnly = false;
            //    AddressInfoGridView.Visible = false;
            //}
            //else
            //{
            //    NewInsuranceTable.Visible = false;
            //    MessageLabelRow.Visible = false;
            //    PopulateAddressInfo();
            //}

        //}
            else if (InsuranceTypeDropDownList.SelectedValue.Equals("Choose"))
            {
                CarrierRow.Visible = false;
                PlanRow.Visible = false;
                GroupRow.Visible = false;
                InsuranceIdRow.Visible = false;
            }

            #endregion Dynamic Controls

            //Added 2/23/18 Daniel
            try
            {
                #region Insurance Carrier

                if (!InsuranceTypeDropDownList.SelectedValue.ToString().Equals("Choose"))
                {

                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();
                    cmd = new SqlCommand("usp_LookupInsuranceCarrier", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@insuranceTypeId", InsuranceTypeDropDownList.SelectedValue);

                    var insuranceCarrierTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(insuranceCarrierTable);

                    CarrierDropDownList.DataSource = insuranceCarrierTable;
                    CarrierDropDownList.DataTextField = "Name";
                    CarrierDropDownList.DataValueField = "InsuranceCarrierId";
                    CarrierDropDownList.DataBind();

                    CarrierDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                    con.Close();

                }
                else
                {
                    CarrierDropDownList.Items.Clear();
                    CarrierDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
                    PlanDropDownList.SelectedIndex = 0;
                }
                #endregion Insurance Carrier
            }

            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

            try
            {
                #region Insurance Plan

                if (!InsuranceTypeDropDownList.SelectedValue.ToString().Equals("Choose"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    //cmd = new SqlCommand("SELECT * FROM dbo.LookUpInsurancePlan WHERE Active = 1 AND InsuranceTypeId = @InsuranceTypeId OR InsurancePlanId = 0", con);
                    cmd = new SqlCommand("SELECT * FROM dbo.LookUpInsurancePlan WHERE Active = 1 AND InsuranceTypeId = @InsuranceTypeId", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@InsuranceTypeId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));

                    var insurancePlanTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(insurancePlanTable);

                    PlanDropDownList.DataSource = insurancePlanTable;
                    PlanDropDownList.DataTextField = "InsurancePlan";
                    PlanDropDownList.DataValueField = "InsurancePlanId";
                    PlanDropDownList.DataBind();

                    PlanDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                    con.Close();
                }
                else
                {
                    PlanDropDownList.Items.Clear();
                    PlanDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
                    CarrierDropDownList.SelectedIndex = 0;
                }
                #endregion Insurance Plan
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

            #region Dynamic Label


            MedicareClaimTextBox.Visible = false;
            ClaimLabel.Visible = false;
            GroupTextBox.Width = 150;
            GroupTextBox.ReadOnly = false;
            GroupTextBox.Text = "";

            //Change in if condition Daniel 2/26/18
            //if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicaid"))
            if (InsuranceTypeDropDownList.SelectedIndex.ToString().Equals("1"))
            {
                GroupLabel.Text = "IL Case ID :";
                InsuranceIdLabel.Text = "IL Recipient ID :";
            }
            //else if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicare"))
            else if (InsuranceTypeDropDownList.SelectedIndex.ToString().Equals("2"))
            {
                GroupLabel.Text = "Medicare ID :";
                InsuranceIdRow.Visible = false;
                MedicareClaimTextBox.Visible = false;
                ClaimLabel.Visible = false;
                GroupTextBox.ReadOnly = false;
                GroupTextBox.Width = 110;
                InsuranceIdTextBox.Text = "";
                //if (Session != null && Session["SSN"] != null)
                //   GroupTextBox.Text = Session["SSN"].ToString();
            }
            //else if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("IL-DHS-DRS HS")
            //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("IL-DHS-DRS VR"))
            //{
            //    GroupLabel.Text = "Case # :";
            //    InsuranceIdRow.Visible = false;
            //}
                // Added on 2/26 Medicaid Managed Daniel
            else if (InsuranceTypeDropDownList.SelectedIndex.ToString().Equals("4"))
            {
                GroupLabel.Text = "Group # :";
                InsuranceIdLabel.Text = "IL Recipient ID :";
            }
            else if (InsuranceTypeDropDownList.SelectedIndex.ToString().Equals("6"))
            {
                GroupRow.Visible = false;
                InsuranceIdLabel.Text = "Member ID :";
            }

            else
            {
                GroupLabel.Text = "Group # :";
                InsuranceIdLabel.Text = "Insurance ID :";
            }

            #endregion Dynamic Label

        }

        protected void Carrier_Changed(object sender, EventArgs e)
        {
            //if (CarrierDropDownList.SelectedIndex > 0 && PlanDropDownList.SelectedIndex > 0)
            //{
            //    NewInsuranceVisibility();
            //}
            //else
            //{
            //    NewInsuranceTable.Visible = false;
            //    MessageLabelRow.Visible = false;
            //}
        }

        protected void Plan_Changed(object sender, EventArgs e)
        {
            //if (PlanDropDownList.SelectedIndex > 0 && CarrierDropDownList.SelectedIndex > 0)
            //{
            //    NewInsuranceVisibility();
            //}
            //else
            //{
            //    NewInsuranceTable.Visible = false;
            //    MessageLabelRow.Visible = false;
            //}
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                if (!InsuranceTypeDropDownList.SelectedValue.Equals("Choose") && !CarrierDropDownList.SelectedValue.Equals("Choose") && !PlanDropDownList.SelectedValue.Equals("Choose") && Session != null && Session["ClientID"] != null)
                {
                    con.Open();

                    cmd = new SqlCommand("usp_ClientInsurance_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    string groupNumber = string.Empty;
                    if (InsuranceTypeDropDownList.SelectedItem.Text.Trim().Equals("Medicare"))
                        groupNumber = GroupTextBox.Text;// + MedicareClaimTextBox.Text;
                    else
                        groupNumber = GroupTextBox.Text;

                    Session["InsuranceTypeId"] = InsuranceTypeDropDownList.SelectedValue;
                    Session["GroupNo"] = groupNumber;

                    cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));
                    cmd.Parameters.AddWithValue("@InsuranceTypeId", InsuranceTypeDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@CarrierId", CarrierDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@InsurancePlanId", PlanDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@GroupNo", groupNumber);
                    cmd.Parameters.AddWithValue("@InsuranceId", InsuranceIdTextBox.Text);

                    if (Session["ClaimContactId"] != null)
                        cmd.Parameters.AddWithValue("@InsuranceClaimContactID", Convert.ToInt32(Session["ClaimContactId"]));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Write("<script language=javascript>alert('Client Funding Information Saved Successfully!!!')</script>");
                        //PopulateInsuranceClaimContact();
                        NewInsuranceVisibility();
                        //Clear();
                        SaveButton.Enabled = false;
                    }
                    else
                    {
                        Response.Write("<script language=javascript>alert('Duplicate Record!!!')</script>");
                    }
                    con.Close();
                }

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Insurance_Save_Click(object sender, EventArgs e)
        {
            #region Validation
            bool isValid = false;
            if (!String.IsNullOrEmpty(ContactNameTextBox.Text))
            {
                isValid = true;
                ErrorLabel.Visible = false;
            }
            else
            {
                isValid = false;
                ErrorLabel.Visible = true;
            }

            #endregion Validation

            try
            {
                #region SP Call
                if (isValid)
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    int clientId = 0;
                    //int carrierId = 0;
                    //int planId = 0;
                    bool isInsurance = false;
                    int insuranceTypeId = 0;
                    string groupNo = string.Empty;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();

                    cmd = new SqlCommand("usp_ClientCarrier_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (Session != null && Session["ClientID"] != null)
                        clientId = Convert.ToInt32(Session["ClientID"]);
                    if (Session != null && Session["InsuranceTypeId"] != null)
                        insuranceTypeId = Convert.ToInt32(Session["InsuranceTypeId"]);
                    if (Session != null && Session["GroupNo"] != null)
                        groupNo = Convert.ToString(Session["GroupNo"]);

                    //if (InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicare")
                    //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("Medicaid")
                    //    || InsuranceTypeDropDownList.SelectedItem.Text.Contains("Private Insurance"))
                    //{
                    //carrierId = Convert.ToInt32(CarrierDropDownList.SelectedValue);
                    //planId = Convert.ToInt32(PlanDropDownList.SelectedValue);
                    isInsurance = true;
                    //}

                    //cmd.Parameters.AddWithValue("@InsuranceCarrierId", carrierId);
                    //cmd.Parameters.AddWithValue("@InsurancePlanId", planId);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@InsuranceTypeId", insuranceTypeId);
                    cmd.Parameters.AddWithValue("@GroupNo", groupNo);
                    cmd.Parameters.AddWithValue("@ContactName", ContactNameTextBox.Text);
                    //cmd.Parameters.AddWithValue("@ContactName", ContactNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
                    cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                    if (StateDropDownList.SelectedValue != "Choose")
                        cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                    else
                        cmd.Parameters.AddWithValue("@State", System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);
                    cmd.Parameters.AddWithValue("@Phone", PhoneTextBox.Text);
                    cmd.Parameters.AddWithValue("@Fax", FaxTextBox.Text);


                    //cmd.Parameters.AddWithValue("@ThirdPartyPayerId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));//Can be removed
                    //Can be removed
                    //cmd.Parameters.AddWithValue("@EntityName", NewCarrierTextBox.Text);//Can be removed

                    SqlParameter claimContactId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                    claimContactId.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(claimContactId.Value) > 0)
                    {
                        Session["ClaimContactId"] = Convert.ToInt32(claimContactId.Value);
                        Clear();
                    }
                    else
                    {
                        Session["ClaimContactId"] = null;
                    }

                    Response.Write("<script language=javascript>alert('Claim Contact Information inserted successfully!!!')</script>");
                    //if (!isInsurance)
                    //    PopulateAddressInfo();
                    ClearNewInsurance();
                    NewInsuranceTable.Visible = false;
                    MessageLabelRow.Visible = false;

                    con.Close();
                }
                #endregion SP Call
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Save_Carrier_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall
                if (!string.IsNullOrEmpty(NewICarrierTextBox.Text))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();

                    cmd = new SqlCommand("INSERT INTO [dbo].[LookUpInsuranceCarrier] ([Name]) VALUES (@CarrierName)", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@CarrierName", NewICarrierTextBox.Text);

                    int returnValue = cmd.ExecuteNonQuery();
                    if (returnValue > 0)
                    {
                        NewICarrierSaveLabel.Visible = true;
                        GetCarrierData();
                    }

                    con.Close();
                }
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("Referral.aspx");

        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("ATUClientInfo.aspx");
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

            FirstNameText.Text = "";
            LastNameText.Text = "";
            Clear();//10/14/15 Daniel
            InsuranceTypeDropDownList.SelectedIndex = 0;
            CarrierDropDownList.SelectedIndex = 0;
            PlanDropDownList.Items.Clear();
            PlanDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            GroupTextBox.Text = "";
            MedicareClaimTextBox.Text = "";
            InsuranceIdTextBox.Text = "";
            AddressInfoGridView.DataSource = null;
            AddressInfoGridView.Visible = false;
            AddressInfoGridView1.DataSource = null;
            AddressInfoGridView1.Visible = false;
            ContactInfoLabel.Visible = false;

            NewInsuranceTable.Visible = false;
            MessageLabelRow.Visible = false;

            GridView1.Visible = false;
            GridView2.Visible = false;

            CarrierRow.Visible = false;
            PlanRow.Visible = false;
            GroupRow.Visible = false;
            InsuranceIdRow.Visible = false;

            CurrentClientValueLabel.Text = "";
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            foreach (GridViewRow gvr in AddressInfoGridView.Rows)
            {
                //string clickButtonValue = "";
                //if (gvr.Cells[0].Text.Contains("Insurance #"))
                //{
                //    clickButtonValue = gvr.Cells[0].Text.Substring(gvr.Cells[0].Text.Length - 2, gvr.Cells[0].Text.Length - 1);
                //}
                string str = gvr.Cells[0].Text;
                if (str.Contains("Insurance #"))
                {
                    LinkButton lb = new LinkButton();
                    lb.ID = "ContactInfoLink";
                    lb.ToolTip = "Click to view contact information";

                    lb.Click += new EventHandler(lb_Click);

                    lb.Text = gvr.Cells[0].Text;
                    if (gvr.Cells[0].FindControl("ContactInfoLink") == null)
                        gvr.Cells[0].Controls.Add(lb);
                }
            }
        }

        protected void lb_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> tempInsuranceDict = (Dictionary<int, string>)Session["InsuranceContactInfo"];
                LinkButton lb = sender as LinkButton;

                string[] values = tempInsuranceDict[Convert.ToInt32(lb.Text.Substring(lb.Text.Length - 1, 1))].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int clientID = 0;
                int insuranceTypeID = 0;
                string groupNo = string.Empty;

                clientID = Convert.ToInt32(values[0]);
                insuranceTypeID = Convert.ToInt32(values[1]);
                if (values.Length > 2)
                    groupNo = values[2];

                //String[] values = tempInsuranceDict[Convert.ToInt32(lb.Text.Substring(0, lb.Text.Length - 1))].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                //Convert it to SP 10/26/15 Daniel
                //cmd = new SqlCommand("SELECT ContactName as 'Name', [Address], City, USS.Name as 'State', Zip, Phone, Fax FROM ClientInsuranceClaimContact ICC JOIN ClientInsurance CI ON ICC.ClientId = CI.ClientId AND ICC.InsuranceTypeId = CI.InsuranceTypeId AND CI.GroupNo = ICC.GroupNo LEFT JOIN LookUpUSStates USS ON ICC.State = USS.State WHERE CI.ClientId = @clientId AND CI.InsuranceTypeId = @insuranceTypeId AND CI.GroupNo = @groupNo", con);
                //cmd.CommandType = CommandType.Text;
                //---------------------------------------------------------------------------------------------

                //Added Daniel 10/26/2015
                cmd = new SqlCommand("usp_ClientInsuranceClaimContact", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //---------------------------------------------------------------------------------------------

                cmd.Parameters.AddWithValue("@clientId", clientID);
                cmd.Parameters.AddWithValue("@insuranceTypeId", insuranceTypeID);
                cmd.Parameters.AddWithValue("@groupNo", groupNo);

                var contactDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(contactDataTable);

                for (int i = 0; i < contactDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < contactDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();
                        row[0] = contactDataTable.Columns[j].ColumnName;
                        row[1] = contactDataTable.Rows[i][j];
                        gridTable.Rows.Add(row);
                    }
                }
                ContactInfoLabel.Visible = true;
                if (contactDataTable.Rows.Count == 0)
                    ContactInfoLabel.Text = "No contact info found";
                else
                    ContactInfoLabel.Text = "Contact Information";

                AddressInfoGridView1.DataSource = gridTable;
                AddressInfoGridView1.DataBind();
                AddressInfoGridView1.Visible = true;
                con.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private bool GetClaimContactRecord()
        {
            //Handled from the calling method

            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                cmd = new SqlCommand("SELECT * FROM dbo.ClientInsuranceClaimContact WHERE InsuranceTypeId = @InsuranceTypeId AND GroupNo = @GroupNo AND ClientID = @ClientID", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@InsuranceTypeId", Convert.ToInt32(Session["InsuranceTypeId"]));
                cmd.Parameters.AddWithValue("@GroupNo", Convert.ToString(Session["GroupNo"]));
                cmd.Parameters.AddWithValue("@ClientID", Convert.ToInt32(Session["ClientID"]));

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return true;
                }
                else
                    return false;
            }
            finally
            {
                con.Close();
            }
        }

        //Seems to be no longer valid Daniel 2/23/18
        //private bool GetNonInsuranceContactRecord()
        //{
        //    try
        //    {
        //        #region DBCall
        //        SqlConnection con = null;
        //        SqlCommand cmd = null;
        //        try
        //        {
        //            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
        //            con.Open();

        //            cmd = new SqlCommand("SELECT * FROM dbo.NonInsuranceClaimContact WHERE ClientId = @ClientId AND ThirdPartyPayerId = @ThirdPartyPayerId", con);
        //            cmd.CommandType = CommandType.Text;

        //            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));
        //            cmd.Parameters.AddWithValue("@ThirdPartyPayerId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            if (reader.Read())
        //            {
        //                return true;
        //            }
        //            else
        //                return false;
        //        }
        //        finally
        //        {
        //            con.Close();
        //        }
        //        #endregion DBCall
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler(ex);
        //        return true;
        //    }
        //}

        private void Clear()
        {
            InsuranceTypeDropDownList.SelectedIndex = 0;
            CarrierDropDownList.SelectedIndex = 0;
            PlanDropDownList.Items.Clear();
            PlanDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            GroupTextBox.Text = "";
            MedicareClaimTextBox.Text = "";
            InsuranceIdTextBox.Text = "";
            NotFoundLabel.Visible = false; //10/14/15 Daniel
        }

        private void ClearNewInsurance()
        {
            ContactNameTextBox.Text = "";
            //NewCarrierTextBox.Text = "";
            NewPlanTextBox.Text = "";
            AddressTextBox.Text = "";
            CityTextBox.Text = "";
            StateDropDownList.SelectedIndex = 0;
            ZipTextBox.Text = "";
            PhoneTextBox.Text = "";
            FaxTextBox.Text = "";
        }

        //Added on 2/26/18 Daniel
        private void ClearInputArea()
        {
            Clear();
            NewInsuranceTable.Visible = false;
            MessageLabelRow.Visible = false;

            CarrierRow.Visible = false;
            PlanRow.Visible = false;
            GroupRow.Visible = false;
            InsuranceIdRow.Visible = false;
        }

        protected void LoadData()
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                #region Insurance Type

                cmd = new SqlCommand("SELECT * FROM dbo.LookUpInsuranceType WHERE Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var insuranceTypeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(insuranceTypeTable);

                InsuranceTypeDropDownList.DataSource = insuranceTypeTable;
                InsuranceTypeDropDownList.DataTextField = "InsuranceType";
                InsuranceTypeDropDownList.DataValueField = "InsuranceTypeId";
                InsuranceTypeDropDownList.DataBind();

                InsuranceTypeDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Insurance Type

                #region Insurance Plan

                PlanDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Insurance Type

                #region Insurance Carrier

                GetCarrierData();

                #endregion Carrier

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

                con.Close();

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void NewInsuranceVisibility()
        {
            bool isPresent = GetClaimContactRecord();
            if (!isPresent)
            {
                NewInsuranceTable.Visible = true;
                MessageLabelRow.Visible = true;
                MessageLabel.Text = "Contact Information for the selected Carrier and Plan is missing. Please enter.";
                //NewCarrierTextBox.Text = CarrierDropDownList.SelectedItem.ToString();
                NewPlanTextBox.Text = PlanDropDownList.SelectedItem.ToString();
                AddressInfoGridView.Visible = false;
            }
            else
            {
                NewInsuranceTable.Visible = false;
                MessageLabelRow.Visible = false;
                PopulateInsuranceClaimContact();
            }
        }

        private void PopulateAddressInfo()
        {
            try
            {
                #region DBCall
                int clientId = 0;
                if (Session != null && Session["ClientID"] != null)
                    clientId = Convert.ToInt32(Session["ClientID"]);

                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                cmd = new SqlCommand("SELECT ClientId, TPP.ThirdPartyPayer, EntityName, ContactName, [Address], City, USS.Name as 'State', Zip, Phone, Fax FROM [dbo].[NonInsuranceClaimContact] CC JOIN dbo.LookUpThirdPartyPayer TPP ON CC.ThirdPartyPayerId = TPP.ThirdPartyPayerId JOIN dbo.LookUpUSStates USS ON CC.State = USS.State WHERE CC.ClientId=@ClientId AND CC.ThirdPartyPayerId = @ThirdPartyPayerId", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@ThirdPartyPayerId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));

                var contactInfoDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(contactInfoDataTable);

                DataRow headerRow = gridTable.NewRow();
                headerRow[0] = "CLAIM CONTACT INFORMATION";
                gridTable.Rows.Add(headerRow);

                for (int i = 0; i < contactInfoDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < contactInfoDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();
                        row[0] = contactInfoDataTable.Columns[j].ColumnName;
                        row[1] = contactInfoDataTable.Rows[i][j].ToString();
                        gridTable.Rows.Add(row);
                    }
                }
                AddressInfoGridView.DataSource = gridTable;
                AddressInfoGridView.DataBind();
                AddressInfoGridView.Visible = true;
                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void PopulateInsuranceClaimContact()
        {
            int clientId = 0;
            if (Session != null && Session["ClientID"] != null)
                clientId = Convert.ToInt32(Session["ClientID"]);

            SqlConnection con = null;
            SqlCommand cmd = null;
            DataTable gridTable = new DataTable();
            gridTable.Columns.Add("Header");
            gridTable.Columns.Add("Values");

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();

            //Convert it to SP 10/26/15 Daniel
            //cmd = new SqlCommand("SELECT CI.ClientID, ContactName, IP.InsurancePlan, [Address], City, USS.Name as 'State', Zip,  Phone, Fax FROM dbo.ClientInsurance CI JOIN dbo.LookUpInsurancePlan IP ON CI.InsurancePlanId = IP.InsurancePlanId JOIN dbo.ClientInsuranceClaimContact ICC ON CI.ClientID = ICC.ClientID JOIN dbo.LookUpUSStates USS ON ICC.State = USS.State WHERE CI.CarrierId = @InsuranceCarrierId AND CI.InsurancePlanId = @InsurancePlanId AND CI.ClientID = @ClientID AND CI.InsuranceTypeId = @InsuranceTypeId", con);
            //cmd.CommandType = CommandType.Text;
            //---------------------------------------------------------------------------------------------

            //Added Daniel 10/26/2015
            cmd = new SqlCommand("usp_ClientInsuranceDetail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //-----------------------------------------------------------------------------------------------

            cmd.Parameters.AddWithValue("@InsuranceCarrierId", Convert.ToInt32(CarrierDropDownList.SelectedValue));
            cmd.Parameters.AddWithValue("@InsurancePlanId", Convert.ToInt32(PlanDropDownList.SelectedValue));
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            cmd.Parameters.AddWithValue("@InsuranceTypeId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));

            var contactInfoDataTable = new DataTable();
            using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(contactInfoDataTable);

            DataRow headerRow = gridTable.NewRow();
            headerRow[0] = "CLAIM CONTACT INFORMATION";
            gridTable.Rows.Add(headerRow);

            for (int i = 0; i < contactInfoDataTable.Rows.Count; i++)
            {
                for (int j = 0; j < contactInfoDataTable.Columns.Count; j++)
                {
                    DataRow row = gridTable.NewRow();
                    row[0] = contactInfoDataTable.Columns[j].ColumnName;
                    row[1] = contactInfoDataTable.Rows[i][j].ToString();
                    gridTable.Rows.Add(row);
                }
            }
            Session["ClientFunding"] = gridTable;
            AddressInfoGridView.DataSource = gridTable;
            AddressInfoGridView.DataBind();
            AddressInfoGridView.Visible = true;
            con.Close();
        }

        //private void PopulateAllContacts()
        //{
        //    int clientId = 0;
        //    if (Session != null && Session["ClientID"] != null)
        //        clientId = Convert.ToInt32(Session["ClientID"]);

        //    SqlConnection con = null;
        //    SqlCommand cmd = null;
        //    DataTable gridTable = new DataTable();
        //    gridTable.Columns.Add("Header");
        //    gridTable.Columns.Add("Values");

        //    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

        //    con.Open();
        //    cmd = new SqlCommand("SELECT CI.InsuranceTypeId,LIC.Name as 'Carrier',InsurancePlan,CI.GroupNo,CI.InsuranceId, ContactName, [Address], City, USS.Name as 'State', Zip, Phone, Fax FROM ClientInsuranceClaimContact ICC JOIN ClientInsurance CI ON ICC.ClientId = CI.ClientId AND ICC.InsuranceTypeId = CI.InsuranceTypeId AND CI.GroupNo = ICC.GroupNo LEFT JOIN LookUpUSStates USS ON ICC.State = USS.State JOIN LookUpInsuranceCarrier LIC ON LIC.InsuranceCarrierId = CI.CarrierId JOIN LookUpInsurancePlan LIP ON LIP.InsurancePlanId = CI.InsurancePlanId WHERE CI.ClientId = @ClientID", con);
        //    cmd.CommandType = CommandType.Text;

        //    //cmd.Parameters.AddWithValue("@InsuranceCarrierId", Convert.ToInt32(CarrierDropDownList.SelectedValue));
        //    //cmd.Parameters.AddWithValue("@InsurancePlanId", Convert.ToInt32(PlanDropDownList.SelectedValue));
        //    cmd.Parameters.AddWithValue("@ClientID", clientId);
        //    //cmd.Parameters.AddWithValue("@InsuranceTypeId", Convert.ToInt32(InsuranceTypeDropDownList.SelectedValue));

        //    var contactInfoDataTable = new DataTable();
        //    using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(contactInfoDataTable);

        //    DataRow headerRow = gridTable.NewRow();
        //    headerRow[0] = "CLAIM CONTACT INFORMATION";
        //    gridTable.Rows.Add(headerRow);

        //    int contactCount = 0;
        //    string groupNumberText = "";
        //    string insuranceIdText = "";
        //    for (int i = 0; i < contactInfoDataTable.Rows.Count; i++)
        //    {
        //        //Contact Label
        //        contactCount = i + 1;
        //        DataRow rowLabel = gridTable.NewRow();
        //        rowLabel[0] = "Contact" + " # " + contactCount;
        //        gridTable.Rows.Add(rowLabel);
        //        for (int j = 1; j < contactInfoDataTable.Columns.Count; j++)
        //        {
        //            DataRow row = gridTable.NewRow();

        //            if (contactInfoDataTable.Rows[i][0].Equals(1))
        //            {
        //                groupNumberText = "IL Case ID :";
        //                insuranceIdText = "IL Recipient ID :";
        //            }
        //            else if (contactInfoDataTable.Rows[i][0].Equals(2))
        //            {
        //                groupNumberText = "Claim # :";
        //                insuranceIdText = "Insurance ID :";
        //            }
        //            else if (contactInfoDataTable.Rows[i][0].Equals(3))
        //            {
        //                groupNumberText = "Group # :";
        //                insuranceIdText = "Insurance ID :";
        //            }

        //            if (contactInfoDataTable.Columns[j].ColumnName.Equals("GroupNo"))
        //                row[0] = groupNumberText;
        //            else if (contactInfoDataTable.Columns[j].ColumnName.Equals("InsuranceId"))
        //                row[0] = insuranceIdText;
        //            else
        //                row[0] = contactInfoDataTable.Columns[j].ColumnName;

        //            if (contactInfoDataTable.Rows[i][j].ToString() == "")
        //                row[1] = "-";
        //            else
        //                row[1] = contactInfoDataTable.Rows[i][j].ToString();
        //            gridTable.Rows.Add(row);
        //        }
        //    }
        //    Session["ClientFunding"] = gridTable;
        //    AddressInfoGridView.DataSource = gridTable;
        //    AddressInfoGridView.DataBind();
        //    AddressInfoGridView.Visible = true;
        //    con.Close();
        //}

        private void PopulateAllContacts()
        {
            try
            {
                #region DBCall

                int clientId = 0;
                if (Session != null && Session["ClientID"] != null)
                    clientId = Convert.ToInt32(Session["ClientID"]);

                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                con.Open();
                cmd = new SqlCommand("SELECT CI.InsuranceTypeId,LIT.InsuranceType as 'InsuranceType',LIC.Name as 'Carrier',InsurancePlan,CI.GroupNo,CI.InsuranceId FROM ClientInsurance CI JOIN LookUpInsuranceCarrier LIC ON LIC.InsuranceCarrierId = CI.CarrierId JOIN LookUpInsurancePlan LIP ON LIP.InsurancePlanId = CI.InsurancePlanId JOIN LookUpInsuranceType LIT ON LIT.InsuranceTypeId = CI.InsuranceTypeId WHERE CI.ClientId = @ClientID", con);
                cmd.CommandType = CommandType.Text;


                cmd.Parameters.AddWithValue("@ClientID", clientId);

                var contactInfoDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(contactInfoDataTable);

                DataRow headerRow = gridTable.NewRow();
                headerRow[0] = "INSURANCE INFORMATION";
                gridTable.Rows.Add(headerRow);

                int contactCount = 0;
                string groupNumberText = "";
                string insuranceIdText = "";
                Dictionary<int, string> tempInsuranceDict = new Dictionary<int, string>();
                for (int i = 0; i < contactInfoDataTable.Rows.Count; i++)
                {
                    //Contact Label
                    contactCount = i + 1;
                    DataRow rowLabel = gridTable.NewRow();
                    rowLabel[0] = "Insurance" + " # " + contactCount;
                    //rowLabel[1] = "Display Contact";
                    gridTable.Rows.Add(rowLabel);

                    tempInsuranceDict.Add(i + 1, clientId + "," + contactInfoDataTable.Rows[i]["InsuranceTypeId"] + "," + contactInfoDataTable.Rows[i]["GroupNo"]);
                    for (int j = 1; j < contactInfoDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();

                        if (contactInfoDataTable.Rows[i]["InsuranceTypeId"].Equals(1))
                        {
                            groupNumberText = "IL Case ID :";
                            insuranceIdText = "IL Recipient ID :";
                        }
                        else if (contactInfoDataTable.Rows[i]["InsuranceTypeId"].Equals(2))
                        {
                            groupNumberText = "Medicare ID :";
                            insuranceIdText = "Insurance ID :";
                        }
                        else if (contactInfoDataTable.Rows[i]["InsuranceTypeId"].Equals(3))
                        {
                            groupNumberText = "Group # :";
                            insuranceIdText = "Insurance ID :";
                        }

                        if (contactInfoDataTable.Columns[j].ColumnName.Equals("GroupNo"))
                            row[0] = groupNumberText;
                        else if (contactInfoDataTable.Columns[j].ColumnName.Equals("InsuranceId"))
                            row[0] = insuranceIdText;
                        else
                            row[0] = contactInfoDataTable.Columns[j].ColumnName;

                        if (contactInfoDataTable.Rows[i][j].ToString() == "")
                            row[1] = "-";
                        else
                            row[1] = contactInfoDataTable.Rows[i][j].ToString();
                        gridTable.Rows.Add(row);
                    }
                }
                Session["InsuranceContactInfo"] = tempInsuranceDict;
                Session["ClientFunding"] = gridTable;
                AddressInfoGridView.DataSource = gridTable;
                AddressInfoGridView.DataBind();
                AddressInfoGridView.Visible = true;
                Session["InsuranceInfoGrid"] = gridTable;
                con.Close();

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void GetCarrierData()
        {
            //Handled from the calling method

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            cmd = new SqlCommand("SELECT * FROM dbo.LookUpInsuranceCarrier WHERE Active = 1", con);
            cmd.CommandType = CommandType.Text;

            var insuranceCarrierTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(insuranceCarrierTable);

            CarrierDropDownList.DataSource = insuranceCarrierTable;
            CarrierDropDownList.DataTextField = "Name";
            CarrierDropDownList.DataValueField = "InsuranceCarrierId";
            CarrierDropDownList.DataBind();

            CarrierDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
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
            CurrentClientLabel.Visible = true;
            CurrentClientValueLabel.Visible = true;
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
