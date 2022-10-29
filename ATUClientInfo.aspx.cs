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
using System.Web.UI.HtmlControls;

namespace ATUClient
{
    public partial class ATUClientInfo : System.Web.UI.Page
    {
        static int validationSectionToShow = 0;
        static Boolean editing_client = false, editing_clientContact = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                updatepanel_Section1.Visible = true;
                updatepanel_Section2.Visible = updatepanel_Section3.Visible = updatepanel_Section4.Visible = false;
                btn_PreviousSection.Enabled = false;
                MessageLabel.Visible = false;
                lbl_Response.Visible = false;
                SaveContactButton.Visible = false;
                SaveButton.Enabled = false;

                lbl_S2StreetVal.Visible = false;
                lbl_S2CityVal.Visible = false;
                lbl_S2StateVal.Visible = false;
                lbl_S2ZipVal.Visible = false;
                lbl_S3StreetVal.Visible = false;
                lbl_S3CityVal.Visible = false;
                lbl_S3StateVal.Visible = false;
                lbl_S3ZipVal.Visible = false;
                lbl_S4RelationshipVal.Visible = false;
                lbl_S4PhoneVal.Visible = false;
                lbl_Validation.Visible = false;
                editing_client = false;
                editing_clientContact = false;
                //******

                Session["SortExpression"] = new Dictionary<string, string>();
                //var data = Response.Cookies["ATUId"].Value;
                //Session["ATUID"] = Request.Cookies["ATUId"].Value; shr
                LoadData();
                //if (Session != null && Session["ClientData"] != null)
                //    PopulateSessionGrid((DataTable)Session["ClientData"]);
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
                if (Session != null && Session["ClientInfoGrid"] != null)
                {
                    DataTable gridTable = (DataTable)Session["ClientInfoGrid"];
                    ClientInfoGridView.DataSource = gridTable;
                    ClientInfoGridView.DataBind();
                    ClientInfoGridView.Visible = true;

                    //Added by vivek 3/15/16
                    updatepanel_Section4.Visible = panel_Section4.Visible = true;
                    updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                    SaveContactButton.Visible = true;
                    SaveContactButton.Text = "Save Contact";
                    btn_PreviousSection.Enabled = btn_NextSection.Enabled = false;
                }
                else
                {
                    if (Session != null && Session["ClientID"] != null)
                    {
                        string clientID = Session["ClientID"].ToString();
                        CreateStatusGrid(clientID);
                        GridView1.Visible = false;
                    }
                }
                Session["DuplicateMessage"] = 0;
            }
            //if (Session != null && Session["ClientID"] != null)
            //    SaveContactRow.Visible = true;
            //To maintain values of these textboxes at postback
            if (DisabilityHiddenField.Value != null)
                DisabilityTextBox.Text = DisabilityHiddenField.Value;

            #region Link Button on page refresh
            if (Session != null && Session["ClientInfoGrid"] != null)
            {
                ClientInfoGridView.DataSource = (DataTable)Session["ClientInfoGrid"];
                foreach (GridViewRow gvr in ClientInfoGridView.Rows)
                {
                    string str = gvr.Cells[1].Text;
                    if (str.Contains("EDIT") && gvr.Cells[0].Text.Equals("CLIENT INFORMATION"))
                    {
                        LinkButton lb = new LinkButton();
                        lb.ID = "EditClientInfo";
                        lb.ToolTip = "Click to edit client information";
                        lb.Click += new EventHandler(lb_Click);


                        lb.Text = gvr.Cells[1].Text;

                        if (gvr.Cells[1].FindControl("EditClientInfo") == null)
                            gvr.Cells[1].Controls.Add(lb);
                    }
                    if (str.Contains("EDIT") && gvr.Cells[0].Text.Contains("Contact"))
                    {
                        LinkButton lb = new LinkButton();
                        lb.ID = gvr.Cells[0].Text;
                        lb.ToolTip = "Click to edit client contact";

                        lb.Click += new EventHandler(lb_ClickContact);

                        lb.Text = gvr.Cells[1].Text;
                        if (gvr.Cells[1].FindControl(gvr.Cells[0].Text) == null)
                            gvr.Cells[1].Controls.Add(lb);
                    }
                }
            }

            #endregion Link Button on page refresh
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            MessageLabel.Text = "";
            MessageLabel.Visible = false;
            lbl_Response.Text = "";
            lbl_Response.Visible = false;
            editing_client = false;
            editing_clientContact = false;
            lbl_Validation.Visible = false;
            lbl_Validation.Text = "";
            SaveButton.Text = "Save";
            Calendar1.Visible = false;

            try
            {
                #region SearchDBCall
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
                //--------------------------------------------------------------------------------------------------

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
                ClientInfoGridView.Visible = false;

                LoadSSN();
                Clear();
                if (GridView1.Rows.Count <= 0)
                {
                    NotFoundLabel.Visible = true;
                    SaveButton.Enabled = true;
                    //Remove the Label when no result 4/22/15 Daniel
                    CurrentClientLabel.Visible = false;
                    CurrentClientValueLabel.Visible = false;
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
                    CreateStatusGrid(clientID);
                    ViewContactPanel(true);
                    RelationshipDropDownList.Enabled = true;
                    GridView1.Visible = false;
                    NotFoundLabel.Visible = false;
                    editing_client = false;
                }
                else
                {
                    NotFoundLabel.Visible = false;
                }
                con.Close();
                #endregion SearchDBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            #region Validate form essential fields

            Boolean formValidStatus = validateform();

            #endregion Validate form essential fields

            if (formValidStatus)
            {
                try
                {
                    #region Calling SP
                    if (Page.IsValid) // && isValidSSN && isValidDOB)
                    {
                        SqlConnection con = null;
                        SqlCommand cmd = null;

                        if (Session != null && Session["DuplicateMessage"] != null && Convert.ToInt32(Session["DuplicateMessage"]).Equals(0) && CheckDuplicates() && !SaveButton.Text.Equals("Update"))
                        {
                            MessageLabel.Visible = true;
                            MessageLabel.Text = "Could be a duplicate record. Click save again to save this record.";
                        }
                        else
                        {
                            MessageLabel.Visible = false;
                            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                            string disabilityString = string.Empty;

                            #region Disabilities
                            //Session["DisabilityDictionary"] = null; added today and should be deleted if not needed
                            Dictionary<string, int> tempDisabilityDict = new Dictionary<string, int>();
                            tempDisabilityDict = (Dictionary<string, int>)Session["DisabilityDictionary"];

                            if (!string.IsNullOrEmpty(DisabilityTextBox.Text))
                            {
                                string[] tempDisabilities = DisabilityHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0; i < tempDisabilities.Length; i++)
                                {
                                    if (string.IsNullOrEmpty(disabilityString))
                                        disabilityString = tempDisabilityDict[tempDisabilities[i].Trim()] + ",";
                                    else
                                        disabilityString = disabilityString + tempDisabilityDict[tempDisabilities[i].Trim()] + ",";
                                }
                            }
                            else
                            {
                                disabilityString = null;
                            }
                            #endregion Disabilities

                            con.Open();
                            #region Save Click
                            if (SaveButton.Text.Equals("Save"))
                            {
                                if (true) //isValidRel && isValidHPhone)
                                {
                                    cmd = new SqlCommand("usp_ClientInfo_Insert", con);
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("@FirstName", FirstNameText.Text);
                                    cmd.Parameters.AddWithValue("@LastName", LastNameText.Text);
                                    //--------------------------------------------------------------------------------------------
                                    if (string.IsNullOrEmpty(SSNValueTextBox.Text) && string.IsNullOrEmpty(SSNValueTextBox1.Text) && string.IsNullOrEmpty(SSNValueTextBox2.Text))
                                    {
                                        cmd.Parameters.AddWithValue("@SSN", System.DBNull.Value);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@SSN", SSNValueTextBox.Text + SSNValueTextBox1.Text + SSNValueTextBox2.Text);
                                    }

                                    //cmd.Parameters.AddWithValue("@SSN", SSNValueTextBox.Text); Not handling NULL 5/19/14 Daniel
                                    if (DOBTextBox.Text != "")
                                    {
                                        DateTime dt = Convert.ToDateTime(DOBTextBox.Text);
                                        cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DOBTextBox.Text));
                                    }
                                    //Handling NULL 5/19/2014---------------------------------------------------
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@DOB", System.DBNull.Value);
                                    }
                                    //--------------------------------------------------------------------------------
                                    //--------------------------------------------------------------------------------
                                    if (!GenderDropDownList.SelectedValue.Equals("Choose"))
                                    {
                                        cmd.Parameters.AddWithValue("@GenderId", GenderDropDownList.SelectedValue);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("GenderId", System.DBNull.Value);
                                    }
                                    //---------------------------------------------------------------------------------------
                                    //cmd.Parameters.AddWithValue("@GenderId", GenderDropDownList.SelectedValue);Not handling NULL 5/19/14 Daniel
                                    if (!EthnicityDropDownList.SelectedValue.Equals("Choose"))
                                        cmd.Parameters.AddWithValue("@EthnicityId", EthnicityDropDownList.SelectedValue);

                                    //--------------------------------------------------------------------------------------
                                    else
                                        cmd.Parameters.AddWithValue("@EthnicityId", System.DBNull.Value);
                                    //--------------------------------------------------------------------------------------

                                    if (!RaceDropDownList.SelectedValue.Equals("Choose"))
                                        cmd.Parameters.AddWithValue("@RaceId", RaceDropDownList.SelectedValue);

                                    //---------------------------------------------------------------------------------------
                                    else
                                        cmd.Parameters.AddWithValue("@RaceId", System.DBNull.Value);
                                    //---------------------------------------------------------------------------------------

                                    //--------------------------------------------------------------------------------------
                                    if (!MobilityDropDownList.SelectedValue.Equals("Choose"))
                                        cmd.Parameters.AddWithValue("@MobilityCodeId", MobilityDropDownList.SelectedValue);
                                    else
                                        cmd.Parameters.AddWithValue("@MobilityCodeId", System.DBNull.Value);

                                    if (disabilityString != "")
                                        cmd.Parameters.AddWithValue("@DisabilityIds", disabilityString);
                                    else
                                        cmd.Parameters.AddWithValue("@DisabilityIds", System.DBNull.Value);


                                    //cmd.Parameters.AddWithValue("@Street", StreetTextBox.Text);
                                    //cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                                    //cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                                    //cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);

                                    //Added by Vivek
                                    //--------------------------------------------------------------------------------------
                                    if (StreetTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@Street", StreetTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@Street", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (CityTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@City", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (StateDropDownList.SelectedValue != "-1")
                                        cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                                    else
                                        cmd.Parameters.AddWithValue("@State", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (ZipTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@Zip", FacilityZipTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@Zip", System.DBNull.Value);
                                    //******************

                                    //------------------------------------------------------------------------------
                                    if (SenateTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@ILSenateDistrictNo", SenateTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@ILSenateDistrictNo", System.DBNull.Value);

                                    //-----------------------------------------------------------------------------
                                    if (HouseTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@ILHouseDistrictNo", HouseTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@ILHouseDistrictNo", System.DBNull.Value);

                                    //-----------------------------------------------------------------------------------
                                    if (GeoCodeTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@GeoCode", GeoCodeTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@GeoCode", System.DBNull.Value);

                                    cmd.Parameters.AddWithValue("@RelationName", RNameTextBox.Text);
                                    if (!RelationshipDropDownList.SelectedValue.Equals("Choose"))
                                        cmd.Parameters.AddWithValue("@RelationshipId", RelationshipDropDownList.SelectedValue);

                                    //---------------------------------------------------------------------------
                                    if (HPhoneTextBox.Text != "" && CellPhoneTextBox.Text == "")
                                    {
                                        cmd.Parameters.AddWithValue("@HomePhone", HPhoneTextBox.Text);
                                        cmd.Parameters.AddWithValue("@CellPhone", System.DBNull.Value);
                                    }
                                    else
                                        if (HPhoneTextBox.Text == "" && CellPhoneTextBox.Text != "")
                                        {
                                            cmd.Parameters.AddWithValue("@HomePhone", System.DBNull.Value);
                                            cmd.Parameters.AddWithValue("@CellPhone", CellPhoneTextBox.Text);
                                        }
                                        else
                                            if (HPhoneTextBox.Text == "" && CellPhoneTextBox.Text == "")
                                            {
                                                cmd.Parameters.AddWithValue("@HomePhone", System.DBNull.Value);
                                                cmd.Parameters.AddWithValue("@CellPhone", System.DBNull.Value);
                                            }
                                            else
                                                if (HPhoneTextBox.Text != "" && CellPhoneTextBox.Text != "")
                                                {
                                                    cmd.Parameters.AddWithValue("@HomePhone", HPhoneTextBox.Text);
                                                    cmd.Parameters.AddWithValue("@CellPhone", CellPhoneTextBox.Text);
                                                }

                                    //---------------------------------------------------------------------------
                                    if (WPhoneTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@WorkPhone", WPhoneTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@WorkPhone", System.DBNull.Value);

                                    //---------------------------------------------------------------------------
                                    if (EmailTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);

                                    //---------------------------------------------------------------------------
                                    if (FNameTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityName", FNameTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityName", System.DBNull.Value);

                                    //----------------------------------------------------------------------------
                                    if (FCNameTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@ContactName", FCNameTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@ContactName", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------
                                    if (FPhoneTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityPhone", FPhoneTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityPhone", System.DBNull.Value);

                                    //----------------------------------------------------------------------------------
                                    if (FFaxTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityFax", FFaxTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityFax", System.DBNull.Value);

                                    //---------------------------------------------------------------------------------
                                    if (FEmailTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityEmail", FEmailTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityEmail", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (TTYTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityTTY", TTYTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityTTY", System.DBNull.Value);

                                    //Added by Vivek
                                    //--------------------------------------------------------------------------------------
                                    if (FacilityStreetTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityStreet", FacilityStreetTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityStreet", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (FacilityCityTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityCity", FacilityCityTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityCity", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (FacilityStateDropDownList.SelectedValue != "-1")
                                        cmd.Parameters.AddWithValue("@FacilityState", FacilityStateDropDownList.SelectedValue);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityState", System.DBNull.Value);

                                    //--------------------------------------------------------------------------------------
                                    if (FacilityZipTextBox.Text != "")
                                        cmd.Parameters.AddWithValue("@FacilityZip", FacilityZipTextBox.Text);
                                    else
                                        cmd.Parameters.AddWithValue("@FacilityZip", System.DBNull.Value);
                                    //******************

                                    cmd.Parameters.AddWithValue("@NetId", Session["ATUID"]);

                                    SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                                    serviceId.Direction = ParameterDirection.ReturnValue;

                                    cmd.ExecuteNonQuery();

                                    if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(serviceId.Value) > 0)
                                    {
                                        //Response.Write("<script language=javascript>alert('Client Record Saved Successfully.')</script>");
                                        lbl_Response.Visible = true;
                                        lbl_Response.Text = "Client Record Saved Successfully.";

                                        Session["ClientID"] = Convert.ToInt32(serviceId.Value);
                                        Session["FirstName"] = FirstNameText.Text;
                                        Session["LastName"] = LastNameText.Text;
                                        Session["SSN"] = SSNValueTextBox.Text + SSNValueTextBox1.Text + SSNValueTextBox2.Text;
                                        CurrentClientValueLabel.Text = FirstNameText.Text + " " + LastNameText.Text;
                                        CreateStatusGrid(serviceId.Value.ToString());
                                        GridView1.Visible = false;
                                        Clear();

                                        //Added by vivek 3/15/16
                                        updatepanel_Section4.Visible = panel_Section4.Visible = true;
                                        updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                                        SaveContactButton.Visible = true;
                                        SaveContactButton.Text = "Save Contact";
                                        lbl_S4PhoneVal.Visible = lbl_S4RelationshipVal.Visible = false;
                                    }
                                    else
                                    {
                                        //Response.Write("<script language=javascript>alert('Duplicate Client Record.')</script>");
                                        MessageLabel.Visible = true;
                                        MessageLabel.Text = "Could be a duplicate record. Click save again to save this record.";
                                        lbl_Validation.Visible = false;
                                    }
                                }
                            }
                            #endregion Save Click

                            #region Update Click
                            else if (SaveButton.Text.Equals("Update"))
                            {
                                cmd = new SqlCommand("usp_ClientInfo_Update", con);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));
                                cmd.Parameters.AddWithValue("@FirstName", FirstNameText.Text);
                                cmd.Parameters.AddWithValue("@LastName", LastNameText.Text);
                                //--------------------------------------------------------------------------------------------
                                if (string.IsNullOrEmpty(SSNValueTextBox.Text) && string.IsNullOrEmpty(SSNValueTextBox1.Text) && string.IsNullOrEmpty(SSNValueTextBox2.Text))
                                {
                                    cmd.Parameters.AddWithValue("@SSN", System.DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@SSN", SSNValueTextBox.Text + SSNValueTextBox1.Text + SSNValueTextBox2.Text);
                                }
                                if (DOBTextBox.Text != "")
                                {
                                    DateTime dt = Convert.ToDateTime(DOBTextBox.Text);
                                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DOBTextBox.Text));
                                }
                                //Handling NULL 5/19/2014---------------------------------------------------
                                else
                                {
                                    cmd.Parameters.AddWithValue("@DOB", System.DBNull.Value);
                                }
                                if (!GenderDropDownList.SelectedValue.Equals("Choose"))
                                {
                                    cmd.Parameters.AddWithValue("@GenderId", GenderDropDownList.SelectedValue);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("GenderId", System.DBNull.Value);
                                }
                                if (!EthnicityDropDownList.SelectedValue.Equals("Choose"))
                                    cmd.Parameters.AddWithValue("@EthnicityId", EthnicityDropDownList.SelectedValue);

                                //--------------------------------------------------------------------------------------
                                else
                                    cmd.Parameters.AddWithValue("@EthnicityId", System.DBNull.Value);
                                //--------------------------------------------------------------------------------------

                                if (!RaceDropDownList.SelectedValue.Equals("Choose"))
                                    cmd.Parameters.AddWithValue("@RaceId", RaceDropDownList.SelectedValue);

                                //---------------------------------------------------------------------------------------
                                else
                                    cmd.Parameters.AddWithValue("@RaceId", System.DBNull.Value);
                                //---------------------------------------------------------------------------------------

                                //--------------------------------------------------------------------------------------
                                if (!MobilityDropDownList.SelectedValue.Equals("Choose"))
                                    cmd.Parameters.AddWithValue("@MobilityCodeId", MobilityDropDownList.SelectedValue);
                                else
                                    cmd.Parameters.AddWithValue("@MobilityCodeId", System.DBNull.Value);


                                if (disabilityString != "")
                                    cmd.Parameters.AddWithValue("@DisabilityIds", disabilityString);
                                else
                                    cmd.Parameters.AddWithValue("@DisabilityIds", System.DBNull.Value);

                                //cmd.Parameters.AddWithValue("@Street", StreetTextBox.Text);
                                //cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                                //cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                                //cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);

                                //Added by Vivek
                                //--------------------------------------------------------------------------------------
                                if (StreetTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@Street", StreetTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@Street", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (CityTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@City", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (StateDropDownList.SelectedValue != "-1")
                                    cmd.Parameters.AddWithValue("@State", StateDropDownList.SelectedValue);
                                else
                                    cmd.Parameters.AddWithValue("@State", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (ZipTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@Zip", ZipTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@Zip", System.DBNull.Value);
                                //******************

                                //------------------------------------------------------------------------------
                                if (SenateTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@ILSenateDistrictNo", SenateTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@ILSenateDistrictNo", System.DBNull.Value);

                                //-----------------------------------------------------------------------------
                                if (HouseTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@ILHouseDistrictNo", HouseTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@ILHouseDistrictNo", System.DBNull.Value);

                                //-----------------------------------------------------------------------------------
                                if (GeoCodeTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@GeoCode", GeoCodeTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@GeoCode", System.DBNull.Value);

                                //---------------------------------------------------------------------------
                                if (FNameTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityName", FNameTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityName", System.DBNull.Value);

                                //----------------------------------------------------------------------------
                                if (FCNameTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@ContactName", FCNameTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@ContactName", System.DBNull.Value);

                                //--------------------------------------------------------------------------------
                                if (FPhoneTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityPhone", FPhoneTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityPhone", System.DBNull.Value);

                                //----------------------------------------------------------------------------------
                                if (FFaxTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityFax", FFaxTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityFax", System.DBNull.Value);

                                //---------------------------------------------------------------------------------
                                if (FEmailTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityEmail", FEmailTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityEmail", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (TTYTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityTTY", TTYTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityTTY", System.DBNull.Value);

                                //Added by Vivek
                                //--------------------------------------------------------------------------------------
                                if (FacilityStreetTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityStreet", FacilityStreetTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityStreet", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (FacilityCityTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityCity", FacilityCityTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityCity", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (FacilityStateDropDownList.SelectedValue != "-1")
                                    cmd.Parameters.AddWithValue("@FacilityState", FacilityStateDropDownList.SelectedValue);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityState", System.DBNull.Value);

                                //--------------------------------------------------------------------------------------
                                if (FacilityZipTextBox.Text != "")
                                    cmd.Parameters.AddWithValue("@FacilityZip", FacilityZipTextBox.Text);
                                else
                                    cmd.Parameters.AddWithValue("@FacilityZip", System.DBNull.Value);
                                //******************

                                cmd.Parameters.AddWithValue("@NetId", Session["ATUID"]);

                                SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                                serviceId.Direction = ParameterDirection.ReturnValue;

                                cmd.ExecuteNonQuery();

                                if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(serviceId.Value) > 0)
                                {
                                    //Response.Write("<script language=javascript>alert('Client Record Updated Successfully.')</script>");
                                    lbl_Response.Visible = true;
                                    lbl_Response.Text = "Client Record Updated Successfully.";

                                    Session["ClientID"] = Convert.ToInt32(serviceId.Value);
                                    Session["FirstName"] = FirstNameText.Text;
                                    Session["LastName"] = LastNameText.Text;
                                    Session["SSN"] = SSNValueTextBox.Text + SSNValueTextBox1.Text + SSNValueTextBox2.Text;
                                    CurrentClientValueLabel.Text = FirstNameText.Text + " " + LastNameText.Text;
                                    CreateStatusGrid(serviceId.Value.ToString());
                                    Clear();

                                    //Added by vivek 3/15/16
                                    updatepanel_Section4.Visible = panel_Section4.Visible = true;
                                    updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                                    SaveContactButton.Visible = true;
                                    SaveContactButton.Text = "Save Contact";
                                    lbl_S4PhoneVal.Visible = lbl_S4RelationshipVal.Visible = false;
                                }
                                else
                                {
                                    //Response.Write("<script language=javascript>alert('Duplicate Client Record.')</script>");
                                    MessageLabel.Visible = true;
                                    MessageLabel.Text = "Duplicate Client Record.";
                                    lbl_Validation.Visible = false;
                                }
                            }
                            #endregion Update Click
                            //SqlParameter serviceId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            //serviceId.Direction = ParameterDirection.ReturnValue;

                            //cmd.ExecuteNonQuery();

                            //if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(serviceId.Value) > 0)
                            //{
                            //    Response.Write("<script language=javascript>alert('Client Record Saved Successfully.')</script>");
                            //    Session["ClientID"] = Convert.ToInt32(serviceId.Value);
                            //    Session["FirstName"] = FirstNameText.Text;
                            //    Session["LastName"] = LastNameText.Text;
                            //    Session["SSN"] = SSNValueTextBox.Text + SSNValueTextBox1.Text + SSNValueTextBox2.Text;
                            //    CurrentClientValueLabel.Text = FirstNameText.Text + " " + LastNameText.Text;
                            //    CreateStatusGrid(serviceId.Value.ToString());
                            //    Clear();
                            //}
                            //else
                            //{
                            //    Response.Write("<script language=javascript>alert('Duplicate Client Record.')</script>");
                            //}
                            con.Close();
                        }
                    }
                    #endregion Calling SP
                }
                catch (Exception ex)
                {
                    ErrorHandler(ex);
                }
            }
            else
            {
                //Show specific Section
                switch (validationSectionToShow)
                {
                    case 1:
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fix the errors displayed above.";
                        updatepanel_Section1.Visible = true;
                        updatepanel_Section2.Visible = updatepanel_Section3.Visible = updatepanel_Section4.Visible = panel_Section4.Visible = false;
                        btn_PreviousSection.Enabled = false;
                        btn_NextSection.Enabled = true;
                        MessageLabel.Visible = false;
                        break;
                    case 2:
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*) and/or fix the displayed error(s).";
                        updatepanel_Section2.Visible = true;
                        updatepanel_Section1.Visible = updatepanel_Section3.Visible = updatepanel_Section4.Visible = panel_Section4.Visible = false;
                        btn_PreviousSection.Enabled = btn_NextSection.Enabled = true;
                        MessageLabel.Visible = false;
                        break;
                    case 3:
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*)";
                        updatepanel_Section3.Visible = true;
                        updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section4.Visible = panel_Section4.Visible = false;
                        btn_PreviousSection.Enabled = btn_NextSection.Enabled = true;
                        MessageLabel.Visible = false;
                        break;
                    case 4:
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*) and/or fix the displayed error(s).";
                        updatepanel_Section4.Visible = panel_Section4.Visible = true;
                        updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                        btn_PreviousSection.Enabled = true;
                        btn_NextSection.Enabled = false;
                        break;
                }
                validationSectionToShow = 0;
            }
        }

        //Validation Code by Vivek
        protected Boolean validateform()
        {
            Boolean returnvar = true;

            #region Section 1 Validation

            #region SSNValidation

            if (String.IsNullOrEmpty(SSNValueTextBox.Text) && String.IsNullOrEmpty(SSNValueTextBox1.Text) && String.IsNullOrEmpty(SSNValueTextBox2.Text))
            {
                SSNFormatLabel.Visible = false;
            }
            else
            {
                if ((String.IsNullOrEmpty(SSNValueTextBox.Text) || SSNValueTextBox.Text.Length != 3)
                    || (String.IsNullOrEmpty(SSNValueTextBox1.Text) || SSNValueTextBox1.Text.Length != 2)
                    || (String.IsNullOrEmpty(SSNValueTextBox2.Text) || SSNValueTextBox2.Text.Length != 4))
                {
                    SSNFormatLabel.Visible = true;

                    if (returnvar == true)
                        returnvar = false;
                    else
                        if (returnvar == false)
                        { }

                    if (validationSectionToShow != 1)
                    {
                        validationSectionToShow = 1;
                    }
                }
                else
                {
                    SSNFormatLabel.Visible = false;
                }
            }

            #endregion SSNValidation

            #region DOBValidation

            bool isValidDOB = false;
            if (!String.IsNullOrEmpty(DOBTextBox.Text))
            {
                string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                DateTime d;
                isValidDOB = DateTime.TryParseExact(DOBTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (isValidDOB)
                {
                    DOBValidationLabel.Visible = false;
                }
                else
                {
                    DOBValidationLabel.Visible = true;

                    if (returnvar == true)
                        returnvar = false;
                    else
                        if (returnvar == false)
                        { }

                    if (validationSectionToShow != 1)
                    {
                        validationSectionToShow = 1;
                    }
                }
            }
            else
                isValidDOB = true;

            #endregion DOBValidation

            #endregion Section 1 Validation

            #region Section 2 Validation

            //#region StreetValidation
            //if (string.IsNullOrEmpty(StreetTextBox.Text))
            //{
            //    lbl_S2StreetVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 2;
            //    }
            //}
            //else
            //{
            //    lbl_S2StreetVal.Visible = false;
            //}
            //#endregion StreetValidation

            //#region CityValidation
            //if (string.IsNullOrEmpty(StreetTextBox.Text))
            //{
            //    lbl_S2CityVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 2;
            //    }
            //}
            //else
            //{
            //    lbl_S2CityVal.Visible = false;
            //}
            //#endregion CityValidation

            //#region StateValidation
            //if (string.IsNullOrEmpty(StreetTextBox.Text))
            //{
            //    lbl_S2StateVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 2;
            //    }
            //}
            //else
            //{
            //    lbl_S2StateVal.Visible = false;
            //}
            //#endregion StateValidation

            //#region ZipValidation
            //if (string.IsNullOrEmpty(StreetTextBox.Text))
            //{
            //    lbl_S2ZipVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 2;
            //    }
            //}
            //else
            //{
            //    lbl_S2ZipVal.Visible = false;
            //}
            //#endregion ZipValidation

            #endregion Section 2 Validation

            #region Section 3 Validation

            //#region StreetValidation
            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
            //{
            //    lbl_S3StreetVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 3;
            //    }
            //}
            //else
            //{
            //    lbl_S3StreetVal.Visible = false;
            //}
            //#endregion StreetValidation

            //#region CityValidation
            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
            //{
            //    lbl_S3CityVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 3;
            //    }
            //}
            //else
            //{
            //    lbl_S3CityVal.Visible = false;
            //}
            //#endregion CityValidation

            //#region StateValidation
            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
            //{
            //    lbl_S3StateVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 3;
            //    }
            //}
            //else
            //{
            //    lbl_S3StateVal.Visible = false;
            //}
            //#endregion StateValidation

            //#region ZipValidation
            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
            //{
            //    lbl_S3ZipVal.Visible = true;

            //    if (returnvar == true)
            //        returnvar = false;
            //    else
            //        if (returnvar == false)
            //        { }

            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
            //    {
            //        validationSectionToShow = 3;
            //    }
            //}
            //else
            //{
            //    lbl_S3ZipVal.Visible = false;
            //}
            //#endregion ZipValidation

            #endregion Section 3 Validation

            #region Section 4 Validation

            if (editing_client == false)
            {
                Boolean status = validateContactInfo();

                if (returnvar == true && status == false)
                {
                    returnvar = false;

                    if (validationSectionToShow != 4 && validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
                    {
                        validationSectionToShow = 4;
                    }
                }
                else
                    if (returnvar == false)
                    { }
            }

            #endregion Section 4 Validation

            return returnvar;
        }

        protected Boolean validateContactInfo()
        {
            Boolean returnvar = true;

            #region ContactInfo Validation
            if (RelationshipDropDownList.SelectedIndex > 0)
            {
                lbl_S4RelationshipVal.Visible = false;
            }
            else
            {
                lbl_S4RelationshipVal.Visible = true;

                if (returnvar == true)
                    returnvar = false;
                else
                    if (returnvar == false)
                    { }
            }
            #endregion ContactInfo Validation

            #region Phone Validation
            if (!string.IsNullOrEmpty(HPhoneTextBox.Text) || !string.IsNullOrEmpty(CellPhoneTextBox.Text))
            {
                //HPhoneRequiredLabel.Visible = false;
                lbl_S4PhoneVal.Visible = false;
            }
            else
            {
                //HPhoneRequiredLabel.Visible = true;
                lbl_S4PhoneVal.Visible = true;
                lbl_S4PhoneVal.Text = "Enter Home or Cell Phone number (Atleast one).";

                if (returnvar == true)
                    returnvar = false;
                else
                    if (returnvar == false)
                    { }
            }
            #endregion Phone Validation

            return returnvar;
        }
        //Validation Code by Vivek ends

        protected void Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("ClientInsurance.aspx");
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
            ClientInfoGridView.DataSource = null;
            ClientInfoGridView.Visible = false;
            SaveButton.Enabled = true;
            //SaveContactRow.Visible = true;
            //updatepanel_SaveContactRow.Visible = true;
            SaveContactButton.Visible = true;

            CurrentClientValueLabel.Text = "";
            GridView1.Visible = false;
            Clear();
            SaveButton.Text = "Save";
            RelationshipDropDownList.Enabled = true;

            updatepanel_Section1.Visible = true;
            updatepanel_Section2.Visible = updatepanel_Section3.Visible = updatepanel_Section4.Visible = false;
            btn_NextSection.Enabled = true;
            btn_PreviousSection.Enabled = false;
            MessageLabel.Text = "";
            MessageLabel.Visible = false;
            lbl_Response.Text = "";
            lbl_Response.Visible = false;

            //Code added by Vivek
            lbl_Validation.Text = "";
            lbl_Validation.Visible = false;
            SaveContactButton.Visible = false;
            SaveButton.Enabled = false;
            editing_client = false;
            editing_clientContact = false;
        }

        protected void SaveContact_Click(object sender, EventArgs e)
        {
            //#region ContactInfo Validation
            //bool isValidRel = false;
            //bool isValidHPhone = false;
            //if (RelationshipDropDownList.SelectedIndex > 0)
            //{
            //    isValidRel = true;
            //    RelRequiredLabel.Visible = false;
            //}
            //else
            //{
            //    isValidRel = false;
            //    RelRequiredLabel.Visible = true;
            //}
            //if (!string.IsNullOrEmpty(HPhoneTextBox.Text))
            //{
            //    isValidHPhone = true;
            //    HPhoneRequiredLabel.Visible = false;
            //}
            //else
            //{
            //    isValidHPhone = false;
            //    HPhoneRequiredLabel.Visible = true;
            //}
            //#endregion ContactInfo Validation

            Boolean status = validateContactInfo();

            try
            {
                if (status) //isValidRel && isValidHPhone)
                {
                    #region ContactSaveDBCall

                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    if (Session != null && Session["ClientID"] != null)
                    {
                        con.Open();

                        if (SaveContactButton.Text.Equals("Save Contact"))
                        {
                            cmd = new SqlCommand("usp_ClientContact_Insert", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            //Edited by Vivek on 3/15/2016
                            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));

                            //---------------------------------------------------------------------------
                            if (RNameTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@RelationName", RNameTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@RelationName", System.DBNull.Value);

                            if (!RelationshipDropDownList.SelectedValue.Equals("Choose"))
                                cmd.Parameters.AddWithValue("@RelationshipId", RelationshipDropDownList.SelectedValue);

                            //---------------------------------------------------------------------------
                            if (HPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@HomePhone", HPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@HomePhone", System.DBNull.Value);

                            //---------------------------------------------------------------------------
                            if (CellPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@CellPhone", CellPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@CellPhone", System.DBNull.Value);

                            //---------------------------------------------------------------------------
                            if (WPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@WorkPhone", WPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@WorkPhone", System.DBNull.Value);

                            //---------------------------------------------------------------------------
                            if (EmailTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);

                            SqlParameter statusId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            statusId.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();

                            if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(statusId.Value) > 0)
                            {
                                //Response.Write("<script language=javascript>alert('Client contact saved successfully.')</script>");
                                lbl_Response.Visible = true;
                                lbl_Response.Text = "Client contact saved successfully.";
                                RelationshipDropDownList.SelectedIndex = 0;
                                RelationshipDropDownList.Enabled = true;
                                RNameTextBox.Text = "";
                                HPhoneTextBox.Text = "";
                                CellPhoneTextBox.Text = "";
                                WPhoneTextBox.Text = "";
                                EmailTextBox.Text = "";
                                CreateStatusGrid(Session["ClientID"].ToString());

                                //Added by vivek 3/15/16
                                updatepanel_Section4.Visible = panel_Section4.Visible = true;
                                updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                                SaveContactButton.Visible = true;
                                SaveContactButton.Text = "Save Contact";
                                lbl_S4PhoneVal.Visible = lbl_S4RelationshipVal.Visible = false;
                            }
                            else
                            {
                                //Response.Write("<script language=javascript>alert('Duplicate record.')</script>");
                                MessageLabel.Visible = true;
                                RelationshipDropDownList.SelectedIndex = 0;
                                RNameTextBox.Text = "";
                                HPhoneTextBox.Text = "";
                                CellPhoneTextBox.Text = "";
                                WPhoneTextBox.Text = "";
                                EmailTextBox.Text = "";
                            }
                        }
                        else if (SaveContactButton.Text.Equals("Update Contact"))
                        {
                            cmd = new SqlCommand("usp_ClientContact_Update", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));

                            if (RNameTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@RelationName", RNameTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@RelationName", System.DBNull.Value);

                            if (!RelationshipDropDownList.SelectedValue.Equals("Choose"))
                                cmd.Parameters.AddWithValue("@RelationshipId", RelationshipDropDownList.SelectedValue);

                            if (HPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@HomePhone", HPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@HomePhone", System.DBNull.Value);

                            if (CellPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@CellPhone", CellPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@CellPhone", System.DBNull.Value);

                            if (WPhoneTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@WorkPhone", WPhoneTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@WorkPhone", System.DBNull.Value);

                            if (EmailTextBox.Text != "")
                                cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                            else
                                cmd.Parameters.AddWithValue("@Email", System.DBNull.Value);

                            SqlParameter statusId = cmd.Parameters.AddWithValue("ReturnValue", SqlDbType.Int);
                            statusId.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();

                            if (cmd.Parameters["ReturnValue"] != null && Convert.ToInt32(statusId.Value) > 0)
                            {
                                //Response.Write("<script language=javascript>alert('Client contact updated successfully.')</script>");
                                //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('Client contact updated successfully.');", true);
                                MessageLabel.Text = "";
                                MessageLabel.Visible = false;
                                lbl_Response.Text = "Client contact updated successfully.";
                                lbl_Response.Visible = true;

                                RelationshipDropDownList.SelectedIndex = 0;
                                RelationshipDropDownList.Enabled = true;
                                RNameTextBox.Text = "";
                                HPhoneTextBox.Text = "";
                                CellPhoneTextBox.Text = "";
                                WPhoneTextBox.Text = "";
                                EmailTextBox.Text = "";
                                CreateStatusGrid(Session["ClientID"].ToString());

                                //Added by vivek 3/15/16
                                updatepanel_Section4.Visible = panel_Section4.Visible = true;
                                updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                                SaveContactButton.Visible = true;
                                SaveContactButton.Text = "Save Contact";
                                lbl_S4PhoneVal.Visible = lbl_S4RelationshipVal.Visible = false;
                            }
                            else
                            {
                                //Response.Write("<script language=javascript>alert('Record not found.')</script>");
                                MessageLabel.Text = "Record not found.";
                                MessageLabel.Visible = true;
                                lbl_Response.Text = "";
                                lbl_Response.Visible = false;

                                RelationshipDropDownList.SelectedIndex = 0;
                                RNameTextBox.Text = "";
                                HPhoneTextBox.Text = "";
                                CellPhoneTextBox.Text = "";
                                WPhoneTextBox.Text = "";
                                EmailTextBox.Text = "";
                            }
                        }
                        con.Close();
                    }
                    #endregion ContactSaveDBCall
                }
                else
                    if (editing_clientContact == true)
                    {
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*) and fix the displayed error(s).";
                        SaveContactButton.Text = "Update Contact";
                        SaveContactButton.Visible = true;
                    }
                    else
                    {
                        SaveContactButton.Text = "Save Contact";
                        SaveContactButton.Visible = false;
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
            Session["ClientID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ClientInfoGrid"] = null;
            Session["InsuranceInfoGrid"] = null;
            Session["ServiceInfoDataTable"] = null;
            Session["SSN"] = null;
            SaveButton.Text = "Save";

            int returnValue;
            int.TryParse(e.CommandArgument.ToString(), out returnValue);

            if (int.TryParse(e.CommandArgument.ToString(), out returnValue))
            {
                #region Populate Client Info

                string str = e.CommandName.ToString();
                string clientId = string.Empty;
                int index = Convert.ToInt32(e.CommandArgument);
                if (str.Equals("Select"))
                {
                    GridViewRow gridRow = GridView1.Rows[index];
                    clientId = gridRow.Cells[1].Text;
                    GridView1.Visible = false;

                    Session["SSN"] = gridRow.Cells[4].Text;
                    CreateStatusGrid(clientId);
                    ViewContactPanel(true);
                    RelationshipDropDownList.Enabled = true;
                }

                #endregion Populate Client Info

                #region Populate Session Grid

                if (str.Equals("Select"))
                {
                    GridViewRow row = GridView1.Rows[index];
                    DataTable dt = new DataTable();
                    DataRow r1 = dt.NewRow();
                    dt.Columns.Add("ClientId");
                    dt.Columns.Add("First_Name");
                    dt.Columns.Add("Last_Name");
                    dt.Columns.Add("SS#");
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        r1[i - 1] = row.Cells[i].Text;
                    }

                    dt.Rows.Add(r1);
                    Session["ClientGrid"] = dt;

                    //Code added by Vivek
                    if (Session != null && Session["FirstName"] != null)
                    {
                        FirstNameText.Text = Session["FirstName"].ToString();
                    }
                    if (Session != null && Session["LastName"] != null)
                    {
                        LastNameText.Text = Session["LastName"].ToString();
                    }
                }
                #endregion Populate Session Grid
            }
        }

        protected void Relationship_Changed(object sender, EventArgs e)
        {
            if (RelationshipDropDownList.SelectedItem.Text.Equals("Self"))
                RNameTextBox.Text = FirstNameText.Text + " " + LastNameText.Text;//Session["FirstName"].ToString() + " " + Session["LastName"].ToString();
            //10/8/15 Daniel Below would not work for new Client 
            //RNameTextBox.Text = Session["FirstName"].ToString() + " " + Session["LastName"].ToString();//FirstNameText.Text + " " + LastNameText.Text;
            else
                RNameTextBox.Text = "";

            RelationshipDropDownList.Focus();
        }

        protected void OnSSN_Changed(object sender, EventArgs e)
        {
            //if (SSNValueTextBox.Text.Length.Equals(3) || SSNValueTextBox.Text.Length.Equals(6))
            //{
            //    SSNValueTextBox.Text = SSNValueTextBox.Text + "-";
            //}
        }

        protected void Sorting_gridView(object sender, GridViewSortEventArgs e)
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;
                string txtLast = LastNameText.Text;
                string txtFirst = FirstNameText.Text;

                //Conver this block to SP 4/22/15 Daniel-----------------------------------------------------
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

                if (searchDataTable != null)
                {
                    DataView dataView = new DataView(searchDataTable);
                    dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection, e.SortExpression);

                    GridView1.DataSource = dataView;
                    GridView1.DataBind();
                    Session["SearchGridDataTable"] = dataView;
                }

                GridView1.Visible = true;

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            foreach (GridViewRow gvr in ClientInfoGridView.Rows)
            {
                string str = gvr.Cells[1].Text;
                if (str.Contains("EDIT") && gvr.Cells[0].Text.Equals("CLIENT INFORMATION"))
                {
                    LinkButton lb = new LinkButton();
                    lb.ID = "EditClientInfo";
                    lb.ToolTip = "Click to edit client information";

                    lb.Click += new EventHandler(lb_Click);

                    lb.Text = gvr.Cells[1].Text;
                    if (gvr.Cells[1].FindControl("EditClientInfo") == null)
                        gvr.Cells[1].Controls.Add(lb);

                    updatepanel_Section1.Visible = true;
                    updatepanel_Section2.Visible = updatepanel_Section3.Visible = updatepanel_Section4.Visible = panel_Section4.Visible = false;
                    btn_NextSection.Enabled = true;
                    btn_PreviousSection.Enabled = false;
                }
                if (str.Contains("EDIT") && gvr.Cells[0].Text.Contains("Contact"))
                {
                    LinkButton lb = new LinkButton();
                    lb.ID = gvr.Cells[0].Text;
                    lb.ToolTip = "Click to edit client contact";

                    lb.Click += new EventHandler(lb_ClickContact);

                    lb.Text = gvr.Cells[1].Text;
                    if (gvr.Cells[1].FindControl(gvr.Cells[0].Text) == null)
                        gvr.Cells[1].Controls.Add(lb);
                }
            }
        }

        protected void lb_Click(object sender, EventArgs e)
        {
            ViewContactPanel(false);
            SaveButton.Enabled = true;
            lbl_Validation.Text = "";
            lbl_Validation.Visible = false;

            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                cmd = new SqlCommand("usp_ClientDetail", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));

                var clientDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientDataTable);

                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < clientDataTable.Columns.Count; j++)
                    {
                        if (clientDataTable.Rows[i][j] != null)
                        {
                            if (clientDataTable.Columns[j].ColumnName.Equals("FirstName"))
                                FirstNameText.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("LastName"))
                                LastNameText.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("SSN"))
                            {
                                if (!string.IsNullOrEmpty(clientDataTable.Rows[i][j].ToString()))
                                {
                                    SSNValueTextBox.Text = clientDataTable.Rows[i][j].ToString().Substring(0, 3);
                                    SSNValueTextBox1.Text = clientDataTable.Rows[i][j].ToString().Substring(3, 2);
                                    SSNValueTextBox2.Text = clientDataTable.Rows[i][j].ToString().Substring(5);
                                }
                            }
                            if (clientDataTable.Columns[j].ColumnName.Equals("DOB"))
                            {
                                if (!string.IsNullOrEmpty(clientDataTable.Rows[i][j].ToString()))
                                    DOBTextBox.Text = Convert.ToDateTime(clientDataTable.Rows[i][j]).ToShortDateString();
                            }
                            if (clientDataTable.Columns[j].ColumnName.Equals("Gender"))
                                GenderDropDownList.SelectedIndex = GenderDropDownList.Items.IndexOf(GenderDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            if (clientDataTable.Columns[j].ColumnName.Equals("Race"))
                                RaceDropDownList.SelectedIndex = RaceDropDownList.Items.IndexOf(RaceDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            if (clientDataTable.Columns[j].ColumnName.Equals("Ethnicity"))
                                EthnicityDropDownList.SelectedIndex = EthnicityDropDownList.Items.IndexOf(EthnicityDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            if (clientDataTable.Columns[j].ColumnName.Equals("MobilityCode"))
                                MobilityDropDownList.SelectedIndex = MobilityDropDownList.Items.IndexOf(MobilityDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            //Disability
                            if (clientDataTable.Columns[j].ColumnName.Equals("Street"))
                                StreetTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("City"))
                                CityTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("State"))
                                StateDropDownList.SelectedIndex = StateDropDownList.Items.IndexOf(StateDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            if (clientDataTable.Columns[j].ColumnName.Equals("Zip"))
                                ZipTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("ILSenateDistrictNo"))
                                SenateTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("ILHouseDistrictNo"))
                                HouseTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("GeoCode"))
                                GeoCodeTextBox.Text = clientDataTable.Rows[i][j].ToString();

                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility Name"))
                                FNameTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            //Code added by Vivek
                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility Street"))
                                FacilityStreetTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility City"))
                                FacilityCityTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility State"))
                                FacilityStateDropDownList.SelectedIndex = FacilityStateDropDownList.Items.IndexOf(FacilityStateDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString()));
                            //FacilityStateDropDownList.SelectedIndex = StateDropDownList.Items.IndexOf(StateDropDownList.Items.FindByText(clientDataTable.Rows[i][j].ToString())); bug 7/20/2016
                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility Zip"))
                                FacilityZipTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            //*******************
                            if (clientDataTable.Columns[j].ColumnName.Equals("Facility Contact Name"))
                                FCNameTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("Phone"))
                                FPhoneTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("Fax"))
                                FFaxTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("Email"))
                                FEmailTextBox.Text = clientDataTable.Rows[i][j].ToString();
                            if (clientDataTable.Columns[j].ColumnName.Equals("TTY"))
                                TTYTextBox.Text = clientDataTable.Rows[i][j].ToString();
                        }
                    }
                }

                #region FetchDisabilities
                //For Disability data
                var disabilityDataTable = GetDBDisabilities(con, cmd, Session["ClientID"].ToString());
                List<int> disabilitiesList = new List<int>();
                string disabilityTextBoxValues = string.Empty;
                foreach (DataRow row in disabilityDataTable.Rows)
                {
                    disabilitiesList.Add(Convert.ToInt32(row["DisabilityId"]));
                }

                for (int i = 0; i < DisabilityCheckBoxList.Items.Count; i++)
                {
                    if (disabilitiesList != null && disabilitiesList.Contains(Convert.ToInt32(DisabilityCheckBoxList.Items[i].Value)))
                    {
                        DisabilityCheckBoxList.Items[i].Selected = true;
                        disabilityTextBoxValues += DisabilityCheckBoxList.Items[i].Text.Trim() + "\n";
                    }
                }
                DisabilityHiddenField.Value = disabilityTextBoxValues;
                DisabilityTextBox.Text = disabilityTextBoxValues;
                #endregion FetchDisabilities

                con.Close();
                SaveButton.Enabled = true;
                SaveButton.Text = "Update";
                SaveContactButton.Enabled = false;
                editing_client = true;
                lbl_Validation.Visible = false;
                lbl_Validation.Text = "";
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected void lb_ClickContact(object sender, EventArgs e)
        {
            ViewContactPanel(true);
            //RelationshipDropDownList.Enabled = false;
            RelationshipDropDownList.Enabled = true;
            SaveButton.Enabled = false;

            //Code by Vivek
            lbl_Validation.Visible = false;
            lbl_Validation.Text = "";
            editing_client = false;
            editing_clientContact = true;
            //*************
            try
            {
                SaveContactButton.Text = "Update Contact";
                Dictionary<int, string> tempConactDict = (Dictionary<int, string>)Session["ClientContactInfo"];
                LinkButton lb = sender as LinkButton;
                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                string[] values = tempConactDict[Convert.ToInt32(lb.ID.Substring(lb.ID.Length - 1, 1))].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int clientID = Convert.ToInt32(values[0]);
                string relationship = values[1];

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                cmd = new SqlCommand("usp_GetClientContact", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientId", clientID);
                cmd.Parameters.AddWithValue("@Relationship", relationship);

                var clientContactTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientContactTable);

                for (int i = 0; i < clientContactTable.Rows.Count; i++)
                {
                    for (int j = 0; j < clientContactTable.Columns.Count; j++)
                    {
                        if (clientContactTable.Rows[i][j] != null)
                        {
                            if (clientContactTable.Columns[j].ColumnName.Equals("Contact Name"))
                                RNameTextBox.Text = clientContactTable.Rows[i][j].ToString();
                            if (clientContactTable.Columns[j].ColumnName.Equals("Relationship"))
                                RelationshipDropDownList.SelectedIndex = RelationshipDropDownList.Items.IndexOf(RelationshipDropDownList.Items.FindByText(clientContactTable.Rows[i][j].ToString()));
                            //RelationshipDropDownList.SelectedIndex = RelationshipDropDownList.SelectedValue;
                            if (clientContactTable.Columns[j].ColumnName.Equals("HomePhone"))
                                HPhoneTextBox.Text = clientContactTable.Rows[i][j].ToString();
                            if (clientContactTable.Columns[j].ColumnName.Equals("CellPhone"))
                                CellPhoneTextBox.Text = clientContactTable.Rows[i][j].ToString();
                            if (clientContactTable.Columns[j].ColumnName.Equals("WorkPhone"))
                                WPhoneTextBox.Text = clientContactTable.Rows[i][j].ToString();
                            if (clientContactTable.Columns[j].ColumnName.Equals("ContactEmail"))
                                EmailTextBox.Text = clientContactTable.Rows[i][j].ToString();
                        }
                    }
                }

                con.Close();
                SaveContactButton.Enabled = true;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection, String sortExpression)
        {
            Dictionary<string, string> sortDict = new Dictionary<string, string>();
            sortDict = (Dictionary<string, string>)Session["SortExpression"];

            string newSortDirection = String.Empty;
            if (!sortDict.ContainsKey(sortExpression))
            {
                newSortDirection = "ASC";
                sortDict.Add(sortExpression, sortDirection.ToString());
            }
            else
            {
                if (sortDict[sortExpression] == "Ascending")
                {
                    sortDict[sortExpression] = "Descending";
                    newSortDirection = "DESC";
                }
                else if (sortDict[sortExpression] == "Descending")
                {
                    sortDict[sortExpression] = "Ascending";
                    newSortDirection = "ASC";
                }
            }
            Session["SortExpression"] = sortDict;
            return newSortDirection;
        }

        private DataRow GetDisabilities(SqlConnection con, SqlCommand cmd, string clientId, DataRow row)
        {
            //Exception handled in higher up the order

            //cmd = new SqlCommand("select C.ClientId,CD.DisabilityId,LD.Disability from dbo.Clients C join dbo.[ClientDisability] CD on C.ClientID = CD.ClientID join dbo.LookUpDisability LD on CD.DisabilityId = LD.DisabilityId where C.ClientID = @ClientId", con);
            //cmd.CommandType = CommandType.Text;

            //cmd.Parameters.AddWithValue("@ClientId", clientId);

            var disabilityDataTable = GetDBDisabilities(con, cmd, clientId);//new DataTable();
            //using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(disabilityDataTable);
            string disabilities = string.Empty;
            for (int k = 0; k < disabilityDataTable.Rows.Count; k++)
            {
                if (string.IsNullOrEmpty(disabilities))
                    disabilities = disabilityDataTable.Rows[k][2].ToString();
                else
                    disabilities = disabilities + ", " + disabilityDataTable.Rows[k][2].ToString();
            }
            row[0] = disabilityDataTable.Columns[2].ColumnName;
            row[1] = disabilities;
            return row;
        }

        private DataTable GetDBDisabilities(SqlConnection con, SqlCommand cmd, string clientId)
        {
            cmd = new SqlCommand("select C.ClientId,CD.DisabilityId,LD.Disability from dbo.Clients C join dbo.[ClientDisability] CD on C.ClientID = CD.ClientID join dbo.LookUpDisability LD on CD.DisabilityId = LD.DisabilityId where C.ClientID = @ClientId", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ClientId", clientId);

            var disabilityDataTable = new DataTable();
            using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(disabilityDataTable);

            return disabilityDataTable;
        }

        protected void LoadData()
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                #region Gender
                cmd = new SqlCommand("select * from dbo.LookUpGender where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var genderTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(genderTable);

                GenderDropDownList.DataSource = genderTable;
                GenderDropDownList.DataTextField = "Gender";
                GenderDropDownList.DataValueField = "GenderId";
                GenderDropDownList.DataBind();

                GenderDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Gender

                #region Ethnicity
                cmd = new SqlCommand("select * from dbo.LookUpEthnicity where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var ethnicityTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(ethnicityTable);

                EthnicityDropDownList.DataSource = ethnicityTable;
                EthnicityDropDownList.DataTextField = "Ethnicity";
                EthnicityDropDownList.DataValueField = "EthnicityId";
                EthnicityDropDownList.DataBind();

                EthnicityDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Ethnicity

                #region Race
                cmd = new SqlCommand("select * from dbo.LookUpRace where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var raceTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(raceTable);

                RaceDropDownList.DataSource = raceTable;
                RaceDropDownList.DataTextField = "Race";
                RaceDropDownList.DataValueField = "RaceId";
                RaceDropDownList.DataBind();

                RaceDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Race

                #region Mobility Code
                cmd = new SqlCommand("select * from dbo.LookUpMobilityCode where Active = 1 order by  MobilityCode", con);
                cmd.CommandType = CommandType.Text;

                var mobilityCodeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(mobilityCodeTable);

                MobilityDropDownList.DataSource = mobilityCodeTable;
                MobilityDropDownList.DataTextField = "MobilityCode";
                MobilityDropDownList.DataValueField = "MobilityCodeId";
                MobilityDropDownList.DataBind();

                MobilityDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Mobility Code

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

                #region FacilityState

                FacilityStateDropDownList.DataSource = stateTable;
                FacilityStateDropDownList.DataTextField = "Name";
                FacilityStateDropDownList.DataValueField = "State";
                FacilityStateDropDownList.DataBind();

                FacilityStateDropDownList.Items.Insert(0, new ListItem("--Choose one--", "-1"));

                #endregion FacilityState

                #region Relationship
                cmd = new SqlCommand("select * from dbo.LookupRelationship where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var relationshipTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(relationshipTable);

                RelationshipDropDownList.DataSource = relationshipTable;
                RelationshipDropDownList.DataTextField = "Relationship";
                RelationshipDropDownList.DataValueField = "RelationshipID";
                RelationshipDropDownList.DataBind();

                RelationshipDropDownList.Items.Insert(0, new ListItem("--Choose one--", "-1"));

                #endregion Relationship

                #region Disability
                cmd = new SqlCommand("select * from dbo.LookUpDisability where Active = 1 order by Disability", con);
                cmd.CommandType = CommandType.Text;

                var disabilityTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(disabilityTable);

                DisabilityCheckBoxList.DataSource = disabilityTable;
                DisabilityCheckBoxList.DataTextField = "Disability";
                DisabilityCheckBoxList.DataValueField = "DisabilityId";
                DisabilityCheckBoxList.DataBind();


                Dictionary<string, int> disabilityDictionary = new Dictionary<string, int>();
                foreach (DataRow row in disabilityTable.Rows)
                {
                    disabilityDictionary.Add(row["Disability"].ToString().Trim(), Convert.ToInt32(row["DisabilityId"]));
                }

                Session["DisabilityDictionary"] = disabilityDictionary;

                #endregion Disability

                con.Close();

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void PopulateSessionGrid(DataTable table)
        {
            ClientInfoGridView.DataSource = table;
            ClientInfoGridView.DataBind();
            ClientInfoGridView.Visible = true;
        }

        private void SessionVariables(string attributeName, string attributeValue)
        {
            SaveButton.Enabled = false;
            if (attributeName.Equals("ClientID"))
            {
                Session["ClientId"] = attributeValue;
                //SaveContactRow.Visible = true;
                SaveContactButton.Visible = true;
                SaveContactButton.Enabled = true;
            }
            else if (attributeName.Equals("FirstName"))
            {
                Session["FirstName"] = attributeValue;
                CurrentClientValueLabel.Text = Session["FirstName"].ToString();

            }
            else if (attributeName.Equals("LastName"))
            {
                Session["LastName"] = attributeValue;
                CurrentClientValueLabel.Text = CurrentClientValueLabel.Text + " " + Session["LastName"].ToString();
            }
        }

        private void CreateStatusGrid(string clientId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            DataTable gridTable = new DataTable();
            Dictionary<int, string> tempContactInfoDict = new Dictionary<int, string>();
            gridTable.Columns.Add("Header");
            gridTable.Columns.Add("Values");

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();// Missing Daniel 4/22/15

            //Converted this block to SP 4/22/15 Daniel-----------------------------------------------------
            //cmd = new SqlCommand("OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert SELECT C.ClientID, CASE WHEN EFirstName IS NOT NULL THEN CAST(DecryptByKey(EFirstName) AS VARCHAR) ELSE EFirstName END AS FirstName, CASE WHEN ELastName IS NOT NULL THEN CAST(DecryptByKey(ELastName) AS VARCHAR) ELSE ELastName END AS LastName, CASE WHEN ESSN IS NOT NULL THEN CAST(DecryptByKey(ESSN) AS VARCHAR) ELSE ESSN END AS SSN, CASE WHEN EDOB IS NOT NULL THEN CAST(DecryptByKey(EDOB) AS datetime) ELSE EDOB END AS DOB, G.Gender, LRC.Race, LE.Ethnicity,MC.MobilityCode, CASE WHEN EStreet IS NOT NULL THEN CAST(DecryptByKey(EStreet) AS VARCHAR) ELSE EStreet END AS Street, CASE WHEN ECity IS NOT NULL THEN CAST(DecryptByKey(ECity) AS VARCHAR) ELSE ECity END AS City, CASE WHEN EZip IS NOT NULL THEN CAST(DecryptByKey(EZip) AS VARCHAR) ELSE EZip END AS Zip, CASE WHEN EILSenateDistrictNo IS NOT NULL THEN CAST(DecryptByKey(EILSenateDistrictNo) AS VARCHAR) ELSE EILSenateDistrictNo END AS ILSenateDistrictNo, CASE WHEN EILHouseDistrictNo IS NOT NULL THEN CAST(DecryptByKey(EILHouseDistrictNo) AS VARCHAR) ELSE EILHouseDistrictNo END AS ILHouseDistrictNo, CASE WHEN EGeoCode IS NOT NULL THEN CAST(DecryptByKey(EGeoCode) AS VARCHAR) ELSE EGeoCode END AS GeoCode, CFC.Name AS 'Facility Name', CFC.ContactName AS 'Facility Contact Name', CASE WHEN EPhone IS NOT NULL THEN CAST(DecryptByKey(EPhone) AS VARCHAR) ELSE EPhone END AS Phone, CASE WHEN EFax IS NOT NULL THEN CAST(DecryptByKey(EFax) AS VARCHAR) ELSE EFax END AS Fax, CASE WHEN CFC.EEmail IS NOT NULL THEN CAST(DecryptByKey(CFC.EEmail) AS VARCHAR) ELSE '' END AS Email, CASE WHEN ETTY IS NOT NULL THEN CAST(DecryptByKey(ETTY) AS VARCHAR) ELSE '' END AS TTY, CC.Name AS 'Contact Name',LR.Relationship, CASE WHEN EHomePhone IS NOT NULL THEN CAST(DecryptByKey(EHomePhone) AS VARCHAR) ELSE EHomePhone END AS HomePhone, CASE WHEN EWorkPhone IS NOT NULL THEN CAST(DecryptByKey(EWorkPhone) AS VARCHAR) ELSE EWorkPhone END AS WorkPhone, CASE WHEN CC.EEmail IS NOT NULL THEN CAST(DecryptByKey(CC.EEmail) AS VARCHAR) ELSE CC.EEmail END AS ContactEmail FROM dbo.Clients C LEFT JOIN dbo.LookUpEthnicity LE ON C.EthnicityId = LE.EthnicityId LEFT JOIN dbo.LookUpRace LRC ON C.RaceId = LRC.RaceId LEFT JOIN dbo.LookUpMobilityCode MC ON C.MobilityCodeId = MC.MobilityCodeId LEFT JOIN dbo.[ClientAddress] CA ON C.ClientID = CA.ClientID LEFT JOIN dbo.LookUpGender G ON C.GenderId = G.GenderId LEFT JOIN dbo.LookUpUSStates USS ON CA.State = USS.State LEFT JOIN dbo.[ClientContact] CC ON C.ClientID = CC.ClientID LEFT JOIN dbo.LookupRelationship LR ON CC.RelationshipId = LR.RelationshipId LEFT JOIN dbo.[ClientFacilityContact] CFC ON C.ClientID = CFC.ClientID WHERE C.ClientID = @ClientId", con);
            //cmd.CommandType = CommandType.Text;
            //---------------------------------------------------------------------------------------------

            //Added 4/22/15 Daniel
            cmd = new SqlCommand("usp_ClientDetail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //----------------------------------------------

            cmd.Parameters.AddWithValue("@ClientId", clientId);

            var clientDataTable = new DataTable();
            using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientDataTable);

            DataRow headerRow = gridTable.NewRow();
            headerRow[0] = "CLIENT INFORMATION";
            headerRow[1] = "EDIT";
            gridTable.Rows.Add(headerRow);
            int contactCount = 0;
            for (int i = 0; i < clientDataTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < clientDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();
                        row[0] = clientDataTable.Columns[j].ColumnName;
                        if (row[0].ToString().Equals("DOB"))
                        {
                            if (clientDataTable.Rows[i][j] != null && clientDataTable.Rows[i][j].ToString() != "")
                                row[1] = Convert.ToDateTime(clientDataTable.Rows[i][j]).ToShortDateString();
                        }
                        else if (row[0].ToString().Equals("SSN"))
                        {
                            if (clientDataTable.Rows[i][j] != null && clientDataTable.Rows[i][j].ToString() != "")
                                row[1] = clientDataTable.Rows[i][j].ToString().Substring(0, 3)
                                                + "-" + clientDataTable.Rows[i][j].ToString().Substring(3, 2)
                                                + "-" + clientDataTable.Rows[i][j].ToString().Substring(5);

                        }
                        else if (clientDataTable.Columns[j].ColumnName.Equals("Contact Name"))
                        {
                            contactCount = i + 1;
                            row[0] = "Contact" + " # " + contactCount;
                            row[1] = "EDIT";
                            gridTable.Rows.Add(row);
                            tempContactInfoDict.Add(contactCount, clientId + "," + clientDataTable.Rows[i]["Relationship"]);

                            row = gridTable.NewRow();
                            row[0] = clientDataTable.Columns[j].ColumnName;
                            row[1] = clientDataTable.Rows[i][j].ToString();
                        }
                        else
                        {
                            row[1] = clientDataTable.Rows[i][j].ToString();
                            SessionVariables(clientDataTable.Columns[j].ColumnName, clientDataTable.Rows[i][j].ToString());
                            CurrentClientLabel.Visible = true;
                            CurrentClientValueLabel.Visible = true;
                        }
                        gridTable.Rows.Add(row);
                        if (clientDataTable.Columns[j].ColumnName.Equals("MobilityCode"))
                        {
                            DataRow row1 = gridTable.NewRow();
                            gridTable.Rows.Add(GetDisabilities(con, cmd, clientId, row1));
                        }
                    }
                }
                //Else statement for displaying contact information
                else
                {
                    for (int j = 0; j < clientDataTable.Columns.Count; j++)
                    {
                        contactCount = i + 1;
                        if (clientDataTable.Columns[j].ColumnName.Equals("Contact Name"))
                        {
                            DataRow row = gridTable.NewRow();
                            row[0] = "Contact" + " # " + contactCount;
                            row[1] = "EDIT";
                            gridTable.Rows.Add(row);
                            row = gridTable.NewRow();
                            tempContactInfoDict.Add(contactCount, clientId + "," + clientDataTable.Rows[i]["Relationship"]);

                            row[0] = clientDataTable.Columns[j].ColumnName;
                            row[1] = clientDataTable.Rows[i][j].ToString();
                            gridTable.Rows.Add(row);
                        }
                        else if (clientDataTable.Columns[j].ColumnName.Equals("Relationship") ||
                            clientDataTable.Columns[j].ColumnName.Equals("HomePhone") ||
                            clientDataTable.Columns[j].ColumnName.Equals("CellPhone") ||
                            clientDataTable.Columns[j].ColumnName.Equals("WorkPhone") ||
                            clientDataTable.Columns[j].ColumnName.Equals("ContactEmail"))
                        {
                            DataRow row = gridTable.NewRow();
                            row[0] = clientDataTable.Columns[j].ColumnName;
                            row[1] = clientDataTable.Rows[i][j].ToString();
                            gridTable.Rows.Add(row);
                        }
                    }
                }
            }
            Session["ClientData"] = gridTable;
            Session["ClientContactInfo"] = tempContactInfoDict;
            ClientInfoGridView.DataSource = gridTable;
            ClientInfoGridView.DataBind();
            ClientInfoGridView.Visible = true;
            Session["ClientInfoGrid"] = gridTable;
            SaveContactButton.Enabled = true;
            con.Close();
        }

        private void Clear()
        {
            //Edited by Vivek on 3/15/16
            SSNValueTextBox.Text = "";
            SSNValueTextBox1.Text = "";
            SSNValueTextBox2.Text = "";
            DOBTextBox.Text = "";
            GenderDropDownList.SelectedIndex = 0;
            RaceDropDownList.SelectedIndex = 0;
            EthnicityDropDownList.SelectedIndex = 0;
            MobilityDropDownList.SelectedIndex = 0;
            DisabilityHiddenField.Value = "";
            DisabilityTextBox.Text = "";
            StreetTextBox.Text = "";
            CityTextBox.Text = "";
            StateDropDownList.SelectedIndex = 0;
            ZipTextBox.Text = "";
            SenateTextBox.Text = "";
            HouseTextBox.Text = "";
            GeoCodeTextBox.Text = "";
            RelationshipDropDownList.SelectedIndex = 0;
            RNameTextBox.Text = "";
            HPhoneTextBox.Text = "";
            CellPhoneTextBox.Text = "";
            WPhoneTextBox.Text = "";
            EmailTextBox.Text = "";
            FNameTextBox.Text = "";
            FacilityStreetTextBox.Text = "";
            FacilityCityTextBox.Text = "";
            FacilityStateDropDownList.SelectedIndex = 0;
            FacilityZipTextBox.Text = "";
            FCNameTextBox.Text = "";
            FPhoneTextBox.Text = "";
            FFaxTextBox.Text = "";
            FEmailTextBox.Text = "";
            TTYTextBox.Text = "";
            NotFoundLabel.Visible = false;

            SSNFormatLabel.Visible = DOBValidationLabel.Visible = false;
            lbl_S2StreetVal.Visible = lbl_S2CityVal.Visible = lbl_S2StateVal.Visible = lbl_S2ZipVal.Visible = false;
            lbl_S3StreetVal.Visible = lbl_S3CityVal.Visible = lbl_S3StateVal.Visible = lbl_S3ZipVal.Visible = false;
            lbl_S4RelationshipVal.Visible = lbl_S4PhoneVal.Visible = false;

            SaveContactButton.Text = "Save Contact";
        }

        private bool CheckDuplicates()
        {
            //Exception Handled in the calling method

            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                //Converted this block to SP 4/22/15 Daniel-----------------------------------------------------
                //cmd = new SqlCommand("OPEN SYMMETRIC KEY ATUKey DECRYPTION BY CERTIFICATE ATUCert SELECT * FROM dbo.Clients WHERE CAST(DecryptByKey(ELastName) AS VARCHAR)  like @LastName AND CAST(DecryptByKey(EFirstName) AS VARCHAR)  like @FirstName AND CAST(DecryptByKey(EDOB) AS VARCHAR)  like @DOB", con);
                //cmd.CommandType = CommandType.Text;
                //-------------------------------------------------------------------------------------------

                //Added 4/22/15 Daniel
                cmd = new SqlCommand("usp_ClientDuplicate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //----------------------------------------------

                cmd.Parameters.AddWithValue("@LastName", LastNameText.Text);
                cmd.Parameters.AddWithValue("@FirstName", FirstNameText.Text);
                //cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DOBTextBox.Text).ToString("yyyy-MM-dd"));

                ////Handling NULL 5/19/2014---------------------------------------------------
                if (DOBTextBox.Text != "")
                {
                    DateTime dt = Convert.ToDateTime(DOBTextBox.Text);
                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DOBTextBox.Text));
                }

                else
                {
                    cmd.Parameters.AddWithValue("@DOB", System.DBNull.Value);
                }
                //---------------------------------------------------------------------------------

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Session["DuplicateMessage"] = Convert.ToInt32(Session["DuplicateMessage"]) + 1;
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

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            PutDateInBox();
        }

        protected void PutDateInBox()
        {
            DOBTextBox.Text = Calendar1.SelectedDate.ToString("MM/dd/yyyy");
            if (!Calendar1.Visible)
                Calendar1.Visible = true;
            else
                Calendar1.Visible = false;
        }

        private void ViewContactPanel(bool value)
        {
            //Code modified by Vivek
            /*
            ContactRow.Visible = value;
            RelationshipRow.Visible = value;
            RNameRow.Visible = value;
            HPhoneRow.Visible = value;
            WPhoneRow.Visible = value;
            EmailRow.Visible = value;
            */
            if (value == true)
            {
                panel_Section4.Visible = true;
                lbl_S4RelationshipVal.Visible = lbl_S4PhoneVal.Visible = false;
                updatepanel_Section4.Visible = true;
                updatepanel_Section1.Visible = updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
                btn_NextSection.Enabled = btn_PreviousSection.Enabled = false;

                SaveContactButton.Visible = true;
                SaveContactButton.Enabled = true;
            }
            else
            {
                updatepanel_Section1.Visible = true;
                updatepanel_Section2.Visible = updatepanel_Section4.Visible = panel_Section4.Visible = false;
                btn_NextSection.Enabled = true;
                btn_PreviousSection.Enabled = false;

                SaveContactButton.Visible = false;
            }
            //SaveContactRow.Visible = true;
            //updatepanel_SaveContactRow.Visible = true;

            lbl_Response.Text = "";
            lbl_Response.Visible = false;
        }

        protected void Calender_Click(object sender, EventArgs e)
        {
            if (!Calendar1.Visible)
                Calendar1.Visible = true;
            else
                Calendar1.Visible = false;
        }

        protected void DOBCustomValidator_Validate(object sender, ServerValidateEventArgs e)
        {
            //DateTime d;
            //e.IsValid = DateTime.TryParseExact(e.Value, "mm/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        }

        //Future Work Daniel 3/10/16
        //protected void ChkBoxAddNewFName_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (ChkBoxAddNewFName.Checked)
        //    {
        //        FNameTextBox.Visible = true;
        //        FNameDropDownList.Visible = false;
        //        FNameDropDownList.SelectedValue = "-1";
        //    }
        //    else
        //    {
        //        FNameTextBox.Visible = false;
        //        FNameDropDownList.Visible = true;
        //    }
        //}

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

        protected void btn_PreviousSection_Click(object sender, EventArgs e)
        {
            if (updatepanel_Section1.Visible)
            {
                //button will be disabled
            }
            else
                if (updatepanel_Section2.Visible)
                {
                    //Validate 2
                    #region Section 2 Validation

                    //#region StreetValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2StreetVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2StreetVal.Visible = false;
                    //}
                    //#endregion StreetValidation

                    //#region CityValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2CityVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2CityVal.Visible = false;
                    //}
                    //#endregion CityValidation

                    //#region StateValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2StateVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2StateVal.Visible = false;
                    //}
                    //#endregion StateValidation

                    //#region ZipValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2ZipVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2ZipVal.Visible = false;
                    //}
                    //#endregion ZipValidation

                    #endregion Section 2 Validation

                    if (lbl_S2StreetVal.Visible || lbl_S2CityVal.Visible || lbl_S2StateVal.Visible || lbl_S2ZipVal.Visible)
                    {
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*)";
                    }
                    else
                    {
                        lbl_Validation.Visible = false;
                        lbl_Validation.Text = "";
                        updatepanel_Section2.Visible = false;
                        updatepanel_Section1.Visible = true;
                        btn_PreviousSection.Enabled = false;
                    }
                }
                else
                    if (updatepanel_Section3.Visible)
                    {
                        //Validate 3
                        #region Section 3 Validation

                        //#region StreetValidation
                        //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                        //{
                        //    lbl_S3StreetVal.Visible = true;
                        //}
                        //else
                        //{
                        //    lbl_S3StreetVal.Visible = false;
                        //}
                        //#endregion StreetValidation

                        //#region CityValidation
                        //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                        //{
                        //    lbl_S3CityVal.Visible = true;
                        //}
                        //else
                        //{
                        //    lbl_S3CityVal.Visible = false;
                        //}
                        //#endregion CityValidation

                        //#region StateValidation
                        //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                        //{
                        //    lbl_S3StateVal.Visible = true;
                        //}
                        //else
                        //{
                        //    lbl_S3StateVal.Visible = false;
                        //}
                        //#endregion StateValidation

                        //#region ZipValidation
                        //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                        //{
                        //    lbl_S3ZipVal.Visible = true;
                        //}
                        //else
                        //{
                        //    lbl_S3ZipVal.Visible = false;
                        //}
                        //#endregion ZipValidation

                        #endregion Section 3 Validation

                        if (lbl_S3StreetVal.Visible || lbl_S3CityVal.Visible || lbl_S3StateVal.Visible || lbl_S3ZipVal.Visible)
                        {
                            lbl_Validation.Visible = true;
                            lbl_Validation.Text = "Please fill the fields marked asterisk (*)";
                        }
                        else
                        {
                            lbl_Validation.Visible = false;
                            lbl_Validation.Text = "";
                            updatepanel_Section3.Visible = false;
                            updatepanel_Section2.Visible = true;
                        }
                    }
                    else
                        if (updatepanel_Section4.Visible)
                        {
                            //Validate 4
                            #region Section 4 Validation

                            Boolean status = validateContactInfo();

                            #endregion Section 4 Validation

                            if (lbl_S4RelationshipVal.Visible || lbl_S4PhoneVal.Visible)
                            {
                                lbl_Validation.Visible = true;
                                lbl_Validation.Text = "Please fill the fields marked asterisk (*) and/or fix the displayed error(s).";
                            }
                            else
                            {
                                lbl_Validation.Visible = false;
                                lbl_Validation.Text = "";
                                panel_Section4.Visible = false;
                                updatepanel_Section3.Visible = true;
                                btn_NextSection.Enabled = true;
                            }
                        }
        }

        protected void btn_NextSection_Click(object sender, EventArgs e)
        {
            if (updatepanel_Section1.Visible)
            {
                //Validate 1
                #region Section 1 Validation

                #region SSNValidation

                if (String.IsNullOrEmpty(SSNValueTextBox.Text) && String.IsNullOrEmpty(SSNValueTextBox1.Text) && String.IsNullOrEmpty(SSNValueTextBox2.Text))
                {
                    SSNFormatLabel.Visible = false;
                }
                else
                {
                    if ((String.IsNullOrEmpty(SSNValueTextBox.Text) || SSNValueTextBox.Text.Length != 3)
                        || (String.IsNullOrEmpty(SSNValueTextBox1.Text) || SSNValueTextBox1.Text.Length != 2)
                        || (String.IsNullOrEmpty(SSNValueTextBox2.Text) || SSNValueTextBox2.Text.Length != 4))
                    {
                        SSNFormatLabel.Visible = true;
                    }
                    else
                    {
                        SSNFormatLabel.Visible = false;
                    }
                }

                #endregion SSNValidation

                #region DOBValidation

                bool isValidDOB = false;
                if (!String.IsNullOrEmpty(DOBTextBox.Text))
                {
                    string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                    DateTime d;
                    isValidDOB = DateTime.TryParseExact(DOBTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                    if (isValidDOB)
                    {
                        DOBValidationLabel.Visible = false;
                    }
                    else
                    {
                        DOBValidationLabel.Visible = true;
                    }
                }
                else
                    isValidDOB = true;

                #endregion DOBValidation

                #endregion Section 1 Validation

                if (SSNFormatLabel.Visible || DOBValidationLabel.Visible)
                {
                    lbl_Validation.Visible = true;
                    lbl_Validation.Text = "Please fix the errors displayed above.";
                }
                else
                {
                    lbl_Validation.Visible = false;
                    lbl_Validation.Text = "";
                    updatepanel_Section1.Visible = false;
                    updatepanel_Section2.Visible = true;
                    btn_PreviousSection.Enabled = true;
                }
            }
            else
                if (updatepanel_Section2.Visible)
                {
                    //Validate 2
                    #region Section 2 Validation

                    //#region StreetValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2StreetVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2StreetVal.Visible = false;
                    //}
                    //#endregion StreetValidation

                    //#region CityValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2CityVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2CityVal.Visible = false;
                    //}
                    //#endregion CityValidation

                    //#region StateValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2StateVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2StateVal.Visible = false;
                    //}
                    //#endregion StateValidation

                    //#region ZipValidation
                    //if (string.IsNullOrEmpty(StreetTextBox.Text))
                    //{
                    //    lbl_S2ZipVal.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_S2ZipVal.Visible = false;
                    //}
                    //#endregion ZipValidation

                    #endregion Section 2 Validation

                    if (lbl_S2StreetVal.Visible || lbl_S2CityVal.Visible || lbl_S2StateVal.Visible || lbl_S2ZipVal.Visible)
                    {
                        lbl_Validation.Visible = true;
                        lbl_Validation.Text = "Please fill the fields marked asterisk (*) and/or fix the displayed error(s).";
                    }
                    else
                    {
                        lbl_Validation.Visible = false;
                        lbl_Validation.Text = "";
                        updatepanel_Section2.Visible = false;
                        updatepanel_Section3.Visible = true;
                    }
                }
                else
                    if (updatepanel_Section3.Visible)
                    {
                        if (editing_client == false)
                        {
                            //Validate 3
                            #region Section 3 Validation

                            //#region StreetValidation
                            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                            //{
                            //    lbl_S3StreetVal.Visible = true;

                            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
                            //    {
                            //        validationSectionToShow = 3;
                            //    }
                            //}
                            //else
                            //{
                            //    lbl_S3StreetVal.Visible = false;
                            //}
                            //#endregion StreetValidation

                            //#region CityValidation
                            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                            //{
                            //    lbl_S3CityVal.Visible = true;

                            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
                            //    {
                            //        validationSectionToShow = 3;
                            //    }
                            //}
                            //else
                            //{
                            //    lbl_S3CityVal.Visible = false;
                            //}
                            //#endregion CityValidation

                            //#region StateValidation
                            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                            //{
                            //    lbl_S3StateVal.Visible = true;

                            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
                            //    {
                            //        validationSectionToShow = 3;
                            //    }
                            //}
                            //else
                            //{
                            //    lbl_S3StateVal.Visible = false;
                            //}
                            //#endregion StateValidation

                            //#region ZipValidation
                            //if (string.IsNullOrEmpty(FacilityStreetTextBox.Text))
                            //{
                            //    lbl_S3ZipVal.Visible = true;

                            //    if (validationSectionToShow != 3 && validationSectionToShow != 2 && validationSectionToShow != 1)
                            //    {
                            //        validationSectionToShow = 3;
                            //    }
                            //}
                            //else
                            //{
                            //    lbl_S3ZipVal.Visible = false;
                            //}
                            //#endregion ZipValidation

                            #endregion Section 3 Validation

                            if (lbl_S3StreetVal.Visible || lbl_S3CityVal.Visible || lbl_S3StateVal.Visible || lbl_S3ZipVal.Visible)
                            {
                                lbl_Validation.Visible = true;
                                lbl_Validation.Text = "Please fill the fields marked asterisk (*)";
                            }
                            else
                            {
                                lbl_Validation.Visible = false;
                                lbl_Validation.Text = "";
                                updatepanel_Section3.Visible = false;
                                updatepanel_Section4.Visible = true;
                                panel_Section4.Visible = true;
                                btn_NextSection.Enabled = false;
                                SaveButton.Enabled = true;
                            }
                        }
                        else
                        {
                            lbl_Validation.Visible = true;
                            lbl_Validation.Text = "Section 4 Unavailable while Editing Client." + "<br />" + "Click Edit on specific contact to edit it.";
                        }
                    }
                    else
                        if (updatepanel_Section4.Visible)
                        {
                            //button will be disabled
                        }
        }
    }
}
