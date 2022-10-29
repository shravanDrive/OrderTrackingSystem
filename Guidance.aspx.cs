using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace ATUClient
{
    public partial class Guidance : System.Web.UI.Page
    {
        List<string> serviceTypes;
        Dictionary<int, string> ADLDict;
        Dictionary<int, string> ADLAbilityDegreeDict;
        List<string> ADLDegreeList = new List<string>();
        List<string> ADL = new List<string>();
        List<string> ADLList = new List<string>();
        List<string> DisabilityList = new List<string>();
        String[] listofVisibleUpdatePanels = new String[500];
        int countOfControls = 0;
        String[] selectedADL_Cleanedarray = new String[500];
        static String[,] MobilityAidVals;

        static String selected_ADLArea = null, selected_DisabilitiesArea = null;
        static String selected_ADLArea_Order = null, selected_DisabilitiesArea_Order = null;
        static int queryOrderByCount_ADL = 0, queryOrderByCount_Disability;
        static DataTable refDatesTable, ADLTable, ADLAbilityDegreeTable, GuideDisabilityDegreeTable, DisabilityTable;
        static DataTable mobAid_ViewData, ADL_ViewData, Disability_ViewData;
        Dictionary<string, string> ADLSelectionDict = new Dictionary<string, string>();
        Dictionary<string, string> DisabilitySelectionDict = new Dictionary<string, string>();
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ReferralId"] = null;

                lbl_CurrentClientName.Visible = false;
                lbl_ErrorGeneric.Visible = lbl_Response.Visible = false;
                btn_Save.Enabled = false;
                lbl_SavedDataHeader.Visible = false;
                GrdView_MobAidView.Visible = GrdView_ADLView.Visible = GrdView_DisabilityView.Visible = false;
                ddlst_ReferralDate.Dispose();

                updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;

                btn_PreviousSection.Enabled = false;

                //Session["ATUID"] = Request.Cookies["ATUId"].Value; shr
                serviceTypes = new List<string>();

                //To be implemented when we bring the default data from database tables...
                LoadData();

                Session["ServiceTypes"] = serviceTypes;

                if (Session != null && Session["FirstName"] != null)
                {
                    txtbx_FN.Text = Session["FirstName"].ToString();
                    lbl_CurrentClientName.Text = textInfo.ToTitleCase(Session["FirstName"].ToString());
                    lbl_CurrentClient.Visible = true;
                    lbl_CurrentClientName.Visible = true;
                }
                if (Session != null && Session["LastName"] != null)
                {
                    txtbx_LN.Text = Session["LastName"].ToString();
                    lbl_CurrentClientName.Text = lbl_CurrentClientName.Text + " " + textInfo.ToTitleCase(Session["LastName"].ToString());
                }

                if (Session != null && Session["ServiceInfoDataTable"] != null)
                {
                    //DataTable gridTable = (DataTable)Session["degreeabilityTable"];
                    //GridView3.DataSource = gridTable;
                    //GridView3.DataBind();
                    //GridView3.Visible = true;

                    PopulateReferralDates();
                }
                else
                {
                    if (Session != null && Session["ClientID"] != null)
                    {
                        PopulateReferralDates();
                        btn_Save.Enabled = true;
                    }
                }

                selected_ADLArea = null;
                selected_ADLArea_Order = null;
                queryOrderByCount_ADL = 0;
                Session["ADLSelections"] = null;
                Reset_Edit_Grids();//8/22/16 Daniel
            }
            else
            {

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

                #region ADL Area in CheckboxList
                con.Open();

                cmd = new SqlCommand("select * from LookupADLArea where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var adlareaTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(adlareaTable);

                chkbxlst_ADLArea.DataSource = adlareaTable;
                chkbxlst_ADLArea.DataTextField = "Description";
                chkbxlst_ADLArea.DataValueField = "ADLAreaId";
                chkbxlst_ADLArea.DataBind();

                con.Close();

                #endregion ADL Area in CheckboxList

                #region ADL Degree Ability

                con.Open();

                cmd = new SqlCommand("select * from LookupADLAbilityDegree where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                ADLAbilityDegreeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(ADLAbilityDegreeTable);

                con.Close();
                #endregion ADL Degree Ability

                #region ADL Mobility Aid

                con.Open();

                cmd = new SqlCommand("select * from LookUpADLMobilityAid where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                var ADLMobilityTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(ADLMobilityTable);

                GrdView_MobilityAid.DataSource = ADLMobilityTable;
                GrdView_MobilityAid.DataBind();

                con.Close();

                #endregion ADL Mobility Aid

                #region Guide Disability Area

                con.Open();

                cmd = new SqlCommand("select * from LookUpGuideDisabilityArea where Active = 1 order by DisplayOrder ASC", con);
                cmd.CommandType = CommandType.Text;

                var GuideDisabilityArea = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(GuideDisabilityArea);

                chkbxlst_DisabilitiesArea.DataSource = GuideDisabilityArea;
                chkbxlst_DisabilitiesArea.DataTextField = "Description";
                chkbxlst_DisabilitiesArea.DataValueField = "GuideDisabilityAreaId";
                chkbxlst_DisabilitiesArea.DataBind();

                con.Close();

                for (int i = 0; i < chkbxlst_DisabilitiesArea.Items.Count; i++)
                {
                    chkbxlst_DisabilitiesArea.Items[i].Selected = true;

                    if (i == 0)
                    {
                        selected_DisabilitiesArea = chkbxlst_DisabilitiesArea.Items[i].ToString();
                        selected_DisabilitiesArea_Order = chkbxlst_DisabilitiesArea.Items[i].ToString() + "' THEN " + queryOrderByCount_ADL + "";
                        queryOrderByCount_Disability++;
                    }
                    else
                    {
                        selected_DisabilitiesArea += "','" + chkbxlst_DisabilitiesArea.Items[i].ToString();
                        selected_DisabilitiesArea_Order = selected_DisabilitiesArea_Order + " WHEN B.Description = '" + chkbxlst_DisabilitiesArea.Items[i].ToString() + "' THEN " + queryOrderByCount_Disability + "";
                        queryOrderByCount_Disability++;
                    }
                }

                #endregion Guide Disability Area

                #region Guide Disability Default

                con.Open();

                cmd = new SqlCommand("select * from LookUpGuideDisability where Active = 1", con);
                cmd.CommandType = CommandType.Text;

                GuideDisabilityDegreeTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(GuideDisabilityDegreeTable);

                GrdView_Disabilities.DataSource = GuideDisabilityDegreeTable;
                GrdView_Disabilities.DataBind();

                con.Close();
                #endregion Guide Disability Default

                #region Referral Dates

                ddlst_ReferralDate.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Referral Dates

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        protected int getMobilityAidId(String mobAid)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("select ADLMobilityAidId from LookUpADLMobilityAid where Active = 1 and Description = '" + mobAid + "';", con);
            cmd.CommandType = CommandType.Text;

            Object val = cmd.ExecuteScalar();

            con.Close();

            return Convert.ToInt32(val);
        }

        protected int getADLId(String Adl)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("select ADLId from LookupADL where Active = 1 and Description = '" + Adl + "';", con);
            cmd.CommandType = CommandType.Text;

            Object val = cmd.ExecuteScalar();

            con.Close();

            return Convert.ToInt32(val);
        }

        protected int getAbilityDegreeId(String AdlAbility)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("SELECT ADLAbilityDegreeId FROM LookUpADLAbilityDegree WHERE Active = 1 and Description = '" + AdlAbility + "';", con);
            cmd.CommandType = CommandType.Text;

            Object val = cmd.ExecuteScalar();

            con.Close();

            return Convert.ToInt32(val);
        }

        protected int getDisabilityId(String disability)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("select GuideDisabilityId from LookUpGuideDisability where Active = 1 and Description = '" + disability + "';", con);
            cmd.CommandType = CommandType.Text;

            Object val = cmd.ExecuteScalar();

            con.Close();

            return Convert.ToInt32(val);
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;
                string txtLast = txtbx_LN.Text.Trim();
                string txtFirst = txtbx_FN.Text.Trim();

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

                GrdView_MobAidView.Visible = GrdView_ADLView.Visible = GrdView_DisabilityView.Visible = false;
                lbl_SavedDataHeader.Visible = false;

                if (GridView1.Rows.Count <= 0)
                {
                    NotFoundLabel.Visible = true;
                    lbl_CurrentClient.Visible = true;
                    lbl_CurrentClientName.Visible = true;
                }
                else
                    if (GridView1.Rows.Count == 1)
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

                        GridView1.Visible = false;
                        GridViewRow row = GridView1.Rows[0];
                        PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);
                        lbl_CurrentClientName.Visible = true;
                        NotFoundLabel.Visible = false;

                        PopulateReferralDates();

                        btn_Save.Enabled = true;
                    }
                    else
                    {
                        NotFoundLabel.Visible = false;
                    }

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

            txtbx_FN.Text = "";
            txtbx_LN.Text = "";

            Clear();
            lbl_CurrentClientName.Text = "";
            GridView1.Visible = false;
            GrdView_MobAidView.Visible = false;
            GrdView_ADLView.Visible = false; //8/22/16 Daniel
            GrdView_DisabilityView.Visible = false; //8/22/16 Daniel
            Reset_Edit_Grids();//8/22/16 Daniel
        }

        private void Clear()
        {
            NotFoundLabel.Visible = false;
            ddlst_ReferralDate.SelectedIndex = 0;//8/22/16 Daniel
            lbl_SavedDataHeader.Visible = false; //8/22/16 Daniel
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

            string str = e.CommandName.ToString();
            int clientId = 0;
            int index = Convert.ToInt32(e.CommandArgument);
            if (str.Equals("Select"))
            {

                GridViewRow row = GridView1.Rows[index];

                clientId = Convert.ToInt32(row.Cells[1].Text);
                Session["ClientID"] = clientId;

                GridView1.Visible = false;

                PopulateSessionVariables(row.Cells[2].Text, row.Cells[3].Text);

                txtbx_LN.Text = row.Cells[3].Text;
                txtbx_FN.Text = row.Cells[2].Text;

                lbl_CurrentClientName.Visible = true;

                PopulateReferralDates();
            }
            #endregion Populate Grid
        }

        protected void lb_Click(object sender, EventArgs e)
        {
            try
            {
                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("Header");
                gridTable.Columns.Add("Values");

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                cmd = new SqlCommand("select FirstName, LastName, NPI, Address, City, State, Zip, Fax, Email from doctorcontact where firstname like @doctorName", con);
                cmd.CommandType = CommandType.Text;
                LinkButton lb = sender as LinkButton;
                cmd.Parameters.AddWithValue("@doctorName", lb.Text);

                var clientDataTable = new DataTable();
                using (var myDataAdapter = new SqlDataAdapter(cmd)) myDataAdapter.Fill(clientDataTable);

                for (int i = 0; i < clientDataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < clientDataTable.Columns.Count; j++)
                    {
                        DataRow row = gridTable.NewRow();
                        row[0] = clientDataTable.Columns[j].ColumnName;
                        row[1] = clientDataTable.Rows[i][j];
                        gridTable.Rows.Add(row);
                    }
                }
                //DoctorInfoLabel.Visible = true;
                //GridView4.DataSource = gridTable;
                //GridView4.DataBind();
                //GridView4.Visible = true;
                con.Close();
                #endregion DBCall
            }
            catch (Exception ex)
            {
                lbl_ErrorGeneric.Visible = true;
                lbl_ErrorGeneric.Text = ex.Message.ToString();
            }
        }

        private void PopulateReferralDates()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            con.Open();

            cmd = new SqlCommand("SELECT C.ReferralId, convert(char(10), C.ReferralDate, 101) ReferralDate " +
                    "FROM ClientReferral C INNER JOIN LookUpReferralSource S ON S.ReferralSourceId = C.SourceId " +
                    "WHERE C.ClientID = @ClientId and " +
                //"S.ReferralSourceId in ('13') " +
                //"S.ReferralSourceId in (select distinct ReferralSourceId from LookUpProjectRefsource) " + 10/3/17 Daniel: query is not descriptive
                    "S.ReferralSourceId in (select ReferralSourceId from LookUpProjectRefsource where ProjectId=1 and Active=1) " +
                    "ORDER BY ReferralDate DESC", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientID"]));

            refDatesTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refDatesTable);

            ddlst_ReferralDate.DataSource = refDatesTable;
            ddlst_ReferralDate.DataTextField = "ReferralDate";
            ddlst_ReferralDate.DataValueField = "ReferralId";
            ddlst_ReferralDate.DataBind();

            con.Close();

            ddlst_ReferralDate.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
        }

        protected void populateSavedData(String referralId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            Boolean flag1, flag2, flag3;
            flag1 = flag2 = flag3 = false;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

            #region View Mobility Aid Saved Data
            con.Open();

            cmd = new SqlCommand("select M.Description MobilityAid, G.Quantity from GuideADLMobilityAid G " +
                        "INNER JOIN ClientReferral C ON C.ReferralId = G.ReferralId " +
                        "INNER JOIN LookUpADLMobilityAid M ON M.ADLMobilityAidId = G.ADLMobilityAidId " +
                        "where G.ReferralId = @ReferralId;", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId));

            mobAid_ViewData = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(mobAid_ViewData);

            //check if the data is available in MobAid data set or not
            if (mobAid_ViewData.Rows.Count != 0)
            {
                NotFoundLabel.Visible = false;

                lbl_SavedDataHeader.Visible = true;
                GrdView_MobAidView.Visible = true;
                GrdView_MobAidView.DataSource = mobAid_ViewData;
                GrdView_MobAidView.DataBind();

                flag1 = true;
            }
            else
            {
                NotFoundLabel.Visible = true;
                lbl_SavedDataHeader.Visible = false;
                GrdView_MobAidView.Visible = false;

                flag1 = false;
            }

            con.Close();
            #endregion View Mobility Aid Saved Data

            #region View ADL Saved Data
            con.Open();

            cmd = new SqlCommand("select Ar.Description ADLArea, A.Description ADL, D.Description ADLAbility from GuideADLAbilityDegree G " +
                        "INNER JOIN ClientReferral C ON C.ReferralId = G.ReferralId " +
                        "INNER JOIN LookUpADLAbilityDegree D ON D.ADLAbilityDegreeId = G.ADLAbilityDegreeId " +
                        "INNER JOIN LookupADL A on A.ADLId = G.ADLId " +
                        "INNER JOIN LookupADLArea Ar ON Ar.ADLAreaId = A.ADLAreaId " +
                        "where G.ReferralId = @ReferralId " +
                        "Order By ADLArea, ADL, ADLAbility;", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId));

            ADL_ViewData = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(ADL_ViewData);

            //check if the data is available in MobAid data set or not
            if (ADL_ViewData.Rows.Count != 0)
            {
                NotFoundLabel.Visible = false;

                lbl_SavedDataHeader.Visible = true;
                GrdView_ADLView.Visible = true;
                GrdView_ADLView.DataSource = ADL_ViewData;
                GrdView_ADLView.DataBind();

                flag2 = true;
            }
            else
            {
                NotFoundLabel.Visible = true;
                lbl_SavedDataHeader.Visible = false;
                GrdView_ADLView.Visible = false;

                flag2 = false;
            }

            con.Close();
            #endregion View ADL Saved Data

            #region View Disability Saved Data
            con.Open();

            cmd = new SqlCommand("select Ar.Description DisabilityArea,	D.Description Disability, G.Answer Answer " +
                        "from GuideDisabilityDetail G " +
                        "INNER JOIN ClientReferral C ON C.ReferralId = G.ReferralId " +
                        "INNER JOIN LookUpGuideDisability D on D.GuideDisabilityId = G.GuideDisabilityId " +
                        "INNER JOIN LookUpGuideDisabilityArea Ar ON Ar.GuideDisabilityAreaId = D.GuideDisabilityAreaId " +
                        "where G.ReferralId = @ReferralId " +
                        "Order By DisabilityArea, Disability, Answer;", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@ReferralId", Convert.ToInt32(referralId));

            Disability_ViewData = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(Disability_ViewData);

            //check if the data is available in MobAid data set or not
            if (Disability_ViewData.Rows.Count != 0)
            {
                NotFoundLabel.Visible = false;

                lbl_SavedDataHeader.Visible = true;
                GrdView_DisabilityView.Visible = true;
                GrdView_DisabilityView.DataSource = Disability_ViewData;
                GrdView_DisabilityView.DataBind();

                flag3 = true;
            }
            else
            {
                NotFoundLabel.Visible = true;
                lbl_SavedDataHeader.Visible = false;
                GrdView_DisabilityView.Visible = false;

                flag3 = false;
            }

            con.Close();
            #endregion View Disability Saved Data

            #region Which region to show? and whether to enable save button?
            if (flag1 == true || flag2 == true || flag3 == true)
            {
                btn_Save.Enabled = false;

                if (flag1 == true)
                {
                    GrdView_MobAidView.Visible = true;
                }
                else
                    GrdView_MobAidView.Visible = false;

                if (flag2 == true)
                {
                    GrdView_ADLView.Visible = true;
                }
                else
                    GrdView_ADLView.Visible = false;

                if (flag3 == true)
                {
                    GrdView_DisabilityView.Visible = true;
                }
                else
                    GrdView_DisabilityView.Visible = false;
            }
            else
            {
                GrdView_MobAidView.Visible = GrdView_ADLView.Visible = GrdView_DisabilityView.Visible = false;
                btn_Save.Enabled = true;
            }
            #endregion Which region to show? and whether to enable save button?
        }

        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            String referralID = ddlst_ReferralDate.SelectedValue;
            Session["ReferralID"] = referralID.ToString();

            try
            {
                if (!ddlst_ReferralDate.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    lbl_ErrorGeneric.Visible = false;
                    lbl_ErrorGeneric.Text = "";

                    //Try to populate the saved data                    
                    populateSavedData(ddlst_ReferralDate.SelectedValue);

                    //Refresh the grid and set to Section 1
                    Reset_Edit_Grids();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        public void Reset_Edit_Grids()
        {
            updatepanel_Section1.Visible = true;
            updatepanel_Section2.Visible = updatepanel_Section3.Visible = false;
            btn_PreviousSection.Enabled = false;
            btn_NextSection.Enabled = true;
            btn_Save.Enabled = false;
            chkbxlst_ADLArea.ClearSelection();
            Session["ADLSelections"] = null;
            selected_ADLArea = selected_ADLArea_Order = null;

            foreach (GridViewRow row in GrdView_MobilityAid.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    TextBox txtRow = (row.Cells[1].FindControl("txtbx_InputMobAidVals") as TextBox);
                    txtRow.Text = "";
                }
            }
            GrdView.DataSource = null;
            GrdView.DataBind();
            Session["DisabilitySelections"] = null; //8/22/16 Daniel reset session variable
            foreach (GridViewRow row in GrdView_Disabilities.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    RadioButtonList radio = (row.Cells[1].FindControl("radiobtnlst_Disabilities") as RadioButtonList);
                    radio.ClearSelection();
                }
            }
        }

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            lbl_CurrentClientName.Text = textInfo.ToTitleCase(firstName) + " " + textInfo.ToTitleCase(lastName);
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //Following information is required to be saved in the tables
                // *** 1 Mobility Aid ***
                // List of Mobility Aid Label for which we have a quantity filled

                // *** 2 Degree Ability ***
                // List of ADL Labels for which we have a checkbox checked
                // ADL ID for those ADL Labels
                // Values of corresponding checkboxes

                // *** 3  Guide Disabilities ***
                // List of Disability Labels for which we have a radio button selected
                // GuideDisabilityId for those Disability Labels
                // Values of corresponding radiobuttons

                // *** 4 input date ***
                // getdate()

                // *** 5 Input user id ***
                // logged in user from step 1 in application

                // *** 6 Referral ID ***

                //Fetch the net id of logged in user
                HttpCookie myCookie = new HttpCookie("ATUId");
                myCookie = Request.Cookies["ATUId"];
                String inputNetId = myCookie.Value.ToString();

                //Fetch the referral id if stored
                String referralId;
                try
                {
                    referralId = Session["ReferralId"].ToString();
                }
                catch (Exception e1)
                {
                    referralId = null;
                }

                if (inputNetId == null || inputNetId.Trim().Equals(String.Empty))
                {
                    lbl_ErrorGeneric.Visible = true;
                    lbl_ErrorGeneric.Text = "Input ID for logged in user could not be retrieved. Please re login and try again.";
                }
                else
                    if (referralId == null || referralId.Trim().Equals(String.Empty))
                    {
                        lbl_ErrorGeneric.Visible = true;
                        lbl_ErrorGeneric.Text = "Referral ID could not be determined. Please select a client and try saving again.";
                    }
                    else
                    {
                        lbl_ErrorGeneric.Visible = false;
                        lbl_ErrorGeneric.Text = "";

                        SqlConnection con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                        SqlCommand cmd = null;
                        SqlDataAdapter adpt = new SqlDataAdapter();

                        #region Saving Mobility Aid data

                        //Inserting Mobility Aid data to table
                        DataTable dt_MobAid_BatchInsert = new DataTable();
                        dt_MobAid_BatchInsert.Columns.Add("ReferralId");
                        dt_MobAid_BatchInsert.Columns.Add("ADLMobilityAidId");
                        dt_MobAid_BatchInsert.Columns.Add("Quantity");
                        dt_MobAid_BatchInsert.Columns.Add("InputNetId");

                        var MobAid_values = new object[4];

                        foreach (GridViewRow row in GrdView_MobilityAid.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                TextBox txtRow = (row.Cells[1].FindControl("txtbx_InputMobAidVals") as TextBox);

                                string selected_MobAid_Label = (row.Cells[1].FindControl("lbl_MobAid") as Label).Text;
                                string val = txtRow.Text;

                                //Prepare a datatable for batch insert
                                if (!(val == "" || val == null || val == "0"))
                                {
                                    //inserting values to datatable rows
                                    MobAid_values[0] = referralId;
                                    MobAid_values[1] = getMobilityAidId(selected_MobAid_Label);
                                    MobAid_values[2] = val;
                                    MobAid_values[3] = inputNetId;

                                    dt_MobAid_BatchInsert.Rows.Add(MobAid_values);
                                }
                                else
                                {
                                    //Just skip that row and dont add it to the datatable for batch insert
                                }
                            }
                        }
                        //Batch Insert for the MobAid datatable
                        cmd = new SqlCommand("usp_GuidanceInfoMobAid_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.UpdatedRowSource = UpdateRowSource.None;

                        // Set the Parameter with appropriate Source Column Name
                        cmd.Parameters.Add("@ReferralId", SqlDbType.Int, 4, dt_MobAid_BatchInsert.Columns[0].ColumnName);
                        cmd.Parameters.Add("@ADLMobilityAidId", SqlDbType.Int, 4, dt_MobAid_BatchInsert.Columns[1].ColumnName);
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int, 4, dt_MobAid_BatchInsert.Columns[2].ColumnName);
                        cmd.Parameters.Add("@InputNetId", SqlDbType.VarChar, 50, dt_MobAid_BatchInsert.Columns[3].ColumnName);

                        adpt.InsertCommand = cmd;
                        // Specify the number of records to be Inserted/Updated in one go. Default is 1.
                        adpt.UpdateBatchSize = 0;

                        con.Open();
                        int recordsInserted = adpt.Update(dt_MobAid_BatchInsert);
                        con.Close();

                        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Mobility Aid Data Saved", "alert('" + "Number of records affected : " + recordsInserted.ToString() + "');", true);

                        #endregion Saving Mobility Aid data

                        #region Saving ADL data

                        //Inserting ADL data to table
                        DataTable dt_ADL_BatchInsert = new DataTable();
                        dt_ADL_BatchInsert.Columns.Add("ReferralId");
                        dt_ADL_BatchInsert.Columns.Add("ADLId");
                        dt_ADL_BatchInsert.Columns.Add("ADLAbilityDegreeId");
                        dt_ADL_BatchInsert.Columns.Add("InputNetId");

                        var ADLvalues = new object[4];

                        if (Session["ADLSelections"] != null)
                        {
                            Dictionary<String, string> ADLSelectSetDict = (Dictionary<String, string>)Session["ADLSelections"];

                            String[,] ADL_Selected_Session_Array = new String[ADLSelectSetDict.Count, 2];
                            string[] temp_dict2array = ADLSelectSetDict.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).ToArray();

                            for (int s = 0; s < temp_dict2array.Length; s++)
                            {
                                String[] temp_selectedSessionArray = temp_dict2array[s].Split(new string[] { "=" }, StringSplitOptions.None);
                                ADL_Selected_Session_Array[s, 0] = temp_selectedSessionArray[0];
                                ADL_Selected_Session_Array[s, 1] = temp_selectedSessionArray[1];

                                //inserting values to datatable rows
                                ADLvalues[0] = referralId;
                                ADLvalues[1] = getADLId(ADL_Selected_Session_Array[s, 0]);
                                ADLvalues[2] = getAbilityDegreeId(ADL_Selected_Session_Array[s, 1]);
                                ADLvalues[3] = inputNetId;

                                dt_ADL_BatchInsert.Rows.Add(ADLvalues);
                            }
                        }

                        //foreach (GridViewRow row in GrdView.Rows)
                        //{
                        //    if (row.RowType == DataControlRowType.DataRow)
                        //    {
                        //        CheckBoxList chkRow = (row.Cells[1].FindControl("chkbxlstADL") as CheckBoxList);

                        //        string selected_ADL_Label = (row.Cells[1].FindControl("lblADL") as Label).Text;
                        //        string val = chkRow.SelectedValue;

                        //        //Prepare a datatable for batch insert
                        //        if (!(val == "" || val == null))
                        //        {
                        //            //inserting values to datatable rows
                        //            ADLvalues[0] = referralId;
                        //            ADLvalues[1] = getADLId(selected_ADL_Label);
                        //            ADLvalues[2] = val;
                        //            ADLvalues[3] = inputNetId;

                        //            dt_ADL_BatchInsert.Rows.Add(ADLvalues);
                        //        }
                        //        else
                        //        {
                        //            //Just skip that row and dont add it to the datatable for batch insert
                        //        }
                        //    }
                        //}

                        //Batch Insert for the MobAid datatable
                        cmd = new SqlCommand("usp_GuidanceADLInfo_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.UpdatedRowSource = UpdateRowSource.None;

                        //Set the Parameter with appropriate Source Column Name
                        cmd.Parameters.Add("@ReferralId", SqlDbType.Int, 4, dt_ADL_BatchInsert.Columns[0].ColumnName);
                        cmd.Parameters.Add("@ADLId", SqlDbType.Int, 4, dt_ADL_BatchInsert.Columns[1].ColumnName);
                        cmd.Parameters.Add("@ADLAbilityDegreeId", SqlDbType.Int, 4, dt_ADL_BatchInsert.Columns[2].ColumnName);
                        cmd.Parameters.Add("@InputNetId", SqlDbType.VarChar, 50, dt_ADL_BatchInsert.Columns[3].ColumnName);

                        adpt.InsertCommand = cmd;
                        // Specify the number of records to be Inserted/Updated in one go. Default is 1.
                        adpt.UpdateBatchSize = 0;

                        con.Open();
                        int AdlrecordsInserted = adpt.Update(dt_ADL_BatchInsert);
                        con.Close();

                        #endregion Saving ADL data

                        #region Saving Guidance Disability Data

                        //Inserting Guidance Disability data to table
                        DataTable dt_Disability_BatchInsert = new DataTable();
                        dt_Disability_BatchInsert.Columns.Add("ReferralId");
                        dt_Disability_BatchInsert.Columns.Add("GuideDisabilityId");
                        dt_Disability_BatchInsert.Columns.Add("Answer");
                        dt_Disability_BatchInsert.Columns.Add("InputNetId");

                        var Disabilityvalues = new object[4];

                        if (Session["DisabilitySelections"] != null)
                        {
                            Dictionary<String, string> DisabilitySelectSetDict = (Dictionary<String, string>)Session["DisabilitySelections"];

                            String[,] Disability_Selected_Session_Array = new String[DisabilitySelectSetDict.Count, 2];
                            string[] temp_dict2array = DisabilitySelectSetDict.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).ToArray();

                            for (int s = 0; s < temp_dict2array.Length; s++)
                            {
                                String[] temp_selectedSessionArray = temp_dict2array[s].Split(new string[] { "=" }, StringSplitOptions.None);
                                Disability_Selected_Session_Array[s, 0] = temp_selectedSessionArray[0];
                                Disability_Selected_Session_Array[s, 1] = temp_selectedSessionArray[1];

                                if (!(Disability_Selected_Session_Array[s, 1] == "" || Disability_Selected_Session_Array[s, 1] == null))
                                {
                                    Boolean boolVal = false;
                                    if (Disability_Selected_Session_Array[s, 1].ToString().ToLower() == "yes")
                                        boolVal = true;
                                    else
                                        boolVal = false;

                                    //inserting values to datatable rows
                                    Disabilityvalues[0] = referralId;
                                    Disabilityvalues[1] = getDisabilityId(Disability_Selected_Session_Array[s, 0]);
                                    Disabilityvalues[2] = boolVal;
                                    Disabilityvalues[3] = inputNetId;

                                    dt_Disability_BatchInsert.Rows.Add(Disabilityvalues);
                                }
                            }
                        }
                        //foreach (GridViewRow row in GrdView_Disabilities.Rows)
                        //{
                        //    if (row.RowType == DataControlRowType.DataRow)
                        //    {
                        //        RadioButtonList radioRow = (row.Cells[1].FindControl("radiobtnlst_Disabilities") as RadioButtonList);

                        //        string selected_Disability_Label = (row.Cells[1].FindControl("lbl_Disabilities") as Label).Text;
                        //        string val = radioRow.Text;

                        //        //Prepare a datatable for batch insert
                        //        if (!(val == "" || val == null))
                        //        {
                        //            Boolean boolVal = false;
                        //            if(val.ToString().ToLower()=="yes") 
                        //                boolVal = true; 
                        //            else 
                        //                boolVal = false;

                        //            //inserting values to datatable rows
                        //            Disabilityvalues[0] = referralId;
                        //            Disabilityvalues[1] = getDisabilityId(selected_Disability_Label);
                        //            Disabilityvalues[2] = boolVal;
                        //            Disabilityvalues[3] = inputNetId;

                        //            dt_Disability_BatchInsert.Rows.Add(Disabilityvalues);
                        //        }
                        //        else
                        //        {
                        //            //Just skip that row and dont add it to the datatable for batch insert
                        //        }
                        //    }
                        //}

                        //Batch Insert for the MobAid datatable
                        cmd = new SqlCommand("usp_GuidanceDisabilityInfo_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.UpdatedRowSource = UpdateRowSource.None;

                        //Set the Parameter with appropriate Source Column Name
                        cmd.Parameters.Add("@ReferralId", SqlDbType.Int, 4, dt_Disability_BatchInsert.Columns[0].ColumnName);
                        cmd.Parameters.Add("@GuideDisabilityId", SqlDbType.Int, 4, dt_Disability_BatchInsert.Columns[1].ColumnName);
                        cmd.Parameters.Add("@Answer", SqlDbType.Bit, 1, dt_Disability_BatchInsert.Columns[2].ColumnName);
                        cmd.Parameters.Add("@InputNetId", SqlDbType.VarChar, 50, dt_Disability_BatchInsert.Columns[3].ColumnName);

                        adpt.InsertCommand = cmd;
                        // Specify the number of records to be Inserted/Updated in one go. Default is 1.
                        adpt.UpdateBatchSize = 0;

                        con.Open();
                        int DisabilityRecordsInserted = adpt.Update(dt_Disability_BatchInsert);
                        con.Close();

                        if (recordsInserted == 0 && AdlrecordsInserted == 0 && DisabilityRecordsInserted == 0)
                        {
                            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ADL Data Saved", "alert('Atleast 1 record should be selected to save the data.');", true);
                            lbl_ErrorGeneric.Visible = true;
                            lbl_ErrorGeneric.Text = "Atleast 1 record should be selected from either 3 sections to save the data.";
                        }
                        else
                        {
                            lbl_Response.Visible = true;
                            lbl_Response.Text = "Data saved successfully.";
                        }
                        #endregion Saving Disability Data

                        //Disable save button to prevent the double saving of data; BUT NOT IF 0 records were updated
                        if (!(recordsInserted == 0 && AdlrecordsInserted == 0 && DisabilityRecordsInserted == 0))
                            btn_Save.Enabled = false;
                        else
                            btn_Save.Enabled = true;

                        //Show saved data if saved successfully
                        populateSavedData(ddlst_ReferralDate.SelectedValue);

                        //Reset edit grids
                        Reset_Edit_Grids();
                    }
            }
        }

        protected void btn_NextSection_Click(object sender, EventArgs e)
        {
            if (updatepanel_Section1.Visible) //We had Mobility Aid section visible and we clicked next.. >> Save Mobility Aid entries
            {
                //check if all the entries are non numeric and between 1 to 9
                foreach (GridViewRow row in GrdView_MobilityAid.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        TextBox txtRow = (row.Cells[1].FindControl("txtbx_InputMobAidVals") as TextBox);
                        String textValue = txtRow.Text;

                        //Check if the value is numeric or not
                        Boolean isNumeric = Regex.IsMatch(textValue, @"^\d+$");

                        if (isNumeric)
                        {
                            if ((Convert.ToInt32(textValue) > Convert.ToInt32("0") && Convert.ToInt32(textValue) <= Convert.ToInt32("9")))
                            {
                                btn_PreviousSection.Enabled = true;

                                updatepanel_Section2.Visible = true;
                                updatepanel_Section1.Visible = false;

                                lbl_ErrorGeneric.Visible = false;
                                lbl_ErrorGeneric.Text = "";
                            }
                            else
                            {
                                btn_PreviousSection.Enabled = false;

                                updatepanel_Section2.Visible = false;
                                updatepanel_Section1.Visible = true;

                                lbl_ErrorGeneric.Visible = true;
                                lbl_ErrorGeneric.Text = "Mobility Aid values cannot be 0. Range between 1 to 9.";
                                break;
                            }
                        }
                        else
                            if (textValue == "")
                            {
                                btn_PreviousSection.Enabled = true;

                                updatepanel_Section2.Visible = true;
                                updatepanel_Section1.Visible = false;

                                lbl_ErrorGeneric.Visible = false;
                                lbl_ErrorGeneric.Text = "";
                            }
                            else
                            {
                                btn_PreviousSection.Enabled = false;

                                updatepanel_Section2.Visible = false;
                                updatepanel_Section1.Visible = true;

                                lbl_ErrorGeneric.Visible = true;
                                lbl_ErrorGeneric.Text = "Please enter numeric values only.";
                                break;
                            }
                    }
                }
            }
            else
                if (updatepanel_Section2.Visible)
                {
                    btn_PreviousSection.Enabled = true;
                    btn_NextSection.Enabled = false;

                    updatepanel_Section3.Visible = true;
                    updatepanel_Section2.Visible = false;

                    btn_Save.Enabled = true;
                }
                else
                    if (updatepanel_Section3.Visible)
                    {
                        //next button is not enabled here
                    }
        }

        protected void btn_PreviousSection_Click(object sender, EventArgs e)
        {
            if (updatepanel_Section1.Visible)
            {
                //previous button is not enabled here
            }
            else
                if (updatepanel_Section2.Visible)
                {
                    btn_PreviousSection.Enabled = false;
                    btn_NextSection.Enabled = true;

                    updatepanel_Section2.Visible = false;
                    updatepanel_Section1.Visible = true;
                }
                else
                    if (updatepanel_Section3.Visible)
                    {
                        btn_PreviousSection.Enabled = true;
                        btn_NextSection.Enabled = true;

                        updatepanel_Section3.Visible = false;
                        updatepanel_Section2.Visible = true;
                    }
        }

        protected void chkbxlst_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Clear the dictionary and recreate it
            ADLSelectionDict.Clear();

            foreach (GridViewRow row in GrdView.Rows)
            {
                int countcols = GrdView.Columns.Count;

                if (row.RowType == DataControlRowType.DataRow)
                {
                    int inRow = row.RowIndex;

                    CheckBoxList chkRow = (row.Cells[1].FindControl("chkbxlstADL") as CheckBoxList);

                    string lastClickededValue = string.Empty;
                    string latestSelectedValue = string.Empty;
                    string result = Request.Form["__EVENTTARGET"];
                    string[] checkedBox = result.Split('$');
                    int latestValueChangeOnRow = Convert.ToInt32(checkedBox[3].Substring(3)) - 2;

                    int index = chkRow.SelectedIndex;
                    int latestindex = int.Parse(checkedBox[checkedBox.Length - 1]);

                    if (latestValueChangeOnRow == inRow)
                    {
                        if (latestindex >= 0 && index >= 0)
                        {
                            try
                            {
                                lastClickededValue = chkRow.Items[latestindex].ToString();

                                //uncheck all other indexes
                                for (int i = 0; i < chkRow.Items.Count; i++)
                                {
                                    if (i != latestindex)
                                    {
                                        String val = chkRow.Items[i].ToString();
                                        ListItem listItem = chkRow.Items.FindByText(val);

                                        if (listItem != null)
                                            listItem.Selected = false;
                                    }
                                }
                            }
                            catch (Exception er)
                            {

                            }
                        }
                    }
                }
            }

            foreach (GridViewRow row in GrdView.Rows)
            {
                int countcols = GrdView.Columns.Count;

                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBoxList chkRow = (row.Cells[1].FindControl("chkbxlstADL") as CheckBoxList);
                    string val = string.Empty;

                    int index = chkRow.SelectedIndex;

                    if (index >= 0)
                    {
                        val = chkRow.Items[index].ToString();
                        try
                        {
                            string selected_ADL_Label = (row.Cells[1].FindControl("lblADL") as Label).Text;
                            //string selected_ADL_Checkbox = (row.Cells[1].FindControl("chkbxlstADL") as CheckBoxList).SelectedItem.ToString();
                            string selected_ADL_Checkbox = val;
                            ADLSelectionDict.Add(selected_ADL_Label, selected_ADL_Checkbox);
                        }
                        catch (Exception er)
                        {

                        }
                    }
                }
            }

            Session["ADLSelections"] = ADLSelectionDict;

        }

        protected void chkbxlst_ADLArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkbxlst_ADLArea.Enabled = false;
            GrdView.Enabled = false;

            try
            {
                string lastSelectedValue = string.Empty;
                string result = Request.Form["__EVENTTARGET"];
                string[] checkedBox = result.Split('$'); ;

                int index = int.Parse(checkedBox[checkedBox.Length - 1]);
                Boolean isChecked = false;

                if (chkbxlst_ADLArea.Items[index].Selected)
                {
                    isChecked = true;
                    lastSelectedValue = chkbxlst_ADLArea.Items[index].ToString();
                }
                else
                {
                    lastSelectedValue = chkbxlst_ADLArea.Items[index].ToString();
                    isChecked = false;
                }

                if (isChecked)
                {
                    if (selected_ADLArea == null)
                    {
                        selected_ADLArea = lastSelectedValue;
                        selected_ADLArea_Order = lastSelectedValue + "' THEN " + queryOrderByCount_ADL + "";
                        queryOrderByCount_ADL++;
                    }
                    else
                    {
                        selected_ADLArea = lastSelectedValue + "','" + selected_ADLArea;
                        selected_ADLArea_Order = selected_ADLArea_Order + " WHEN B.Description = '" + lastSelectedValue + "' THEN " + queryOrderByCount_ADL + "";
                        queryOrderByCount_ADL++;
                    }
                }
                else
                    if (!isChecked)
                    {
                        String[] temp_selectedADLArea_array = selected_ADLArea.Split(new string[] { "','" }, StringSplitOptions.None);
                        String[] temp1_selectedADLAreaOrder_array = selected_ADLArea_Order.Split(new string[] { " WHEN B.Description = '" }, StringSplitOptions.None);
                        String[] temp2_selectedADLAreaOrder_array = new String[temp1_selectedADLAreaOrder_array.Length];
                        for (int i = 0; i < temp1_selectedADLAreaOrder_array.Length; i++)
                        {
                            String[] tempSplit = temp1_selectedADLAreaOrder_array[i].Split(new string[] { "'" }, StringSplitOptions.None);
                            temp2_selectedADLAreaOrder_array[i] = tempSplit[0].ToString();
                        }
                        List<String> temp_selectedADLArea_list = new List<String>();
                        List<String> temp_selectedADLAreaOrder_list = new List<String>();
                        temp_selectedADLArea_list.AddRange(temp_selectedADLArea_array);
                        temp_selectedADLAreaOrder_list.AddRange(temp2_selectedADLAreaOrder_array);

                        for (int i = 0; i < temp_selectedADLArea_list.Count; i++)
                        {
                            if (temp_selectedADLArea_list[i].Equals(lastSelectedValue))
                            {
                                //int pos = temp_selectedADLArea_list.IndexOf(lastSelectedValue);
                                temp_selectedADLArea_list.RemoveAt(temp_selectedADLArea_list.IndexOf(lastSelectedValue));
                                break;
                            }
                        }
                        for (int i = 0; i < temp_selectedADLAreaOrder_list.Count; i++)
                        {
                            if (temp_selectedADLAreaOrder_list[i].Equals(lastSelectedValue))
                            {
                                //int pos = temp_selectedADLArea_list.IndexOf(lastSelectedValue);
                                temp_selectedADLAreaOrder_list.RemoveAt(temp_selectedADLAreaOrder_list.IndexOf(lastSelectedValue));
                                break;
                            }
                        }

                        if (temp_selectedADLArea_list.Count != 0 && (temp_selectedADLArea_list.Count == temp_selectedADLAreaOrder_list.Count))
                        {
                            queryOrderByCount_ADL = 0;

                            for (int i = 1; i <= temp_selectedADLArea_list.Count; i++)
                            {
                                if (i == 1)
                                {
                                    selected_ADLArea = temp_selectedADLArea_list[i - 1];
                                    selected_ADLArea_Order = temp_selectedADLAreaOrder_list[i - 1] + "' THEN " + queryOrderByCount_ADL + "";
                                    queryOrderByCount_ADL++;
                                }
                                else
                                {
                                    selected_ADLArea = selected_ADLArea + "','" + temp_selectedADLArea_list[i - 1];
                                    selected_ADLArea_Order = selected_ADLArea_Order + " WHEN B.Description = '" + temp_selectedADLAreaOrder_list[i - 1] + "' THEN " + queryOrderByCount_ADL + "";
                                    queryOrderByCount_ADL++;
                                }
                            }
                        }
                        else
                        {
                            selected_ADLArea = "";
                        }
                    }

                ADLAbilityDegreeDict = new Dictionary<int, string>();
                ADLDict = new Dictionary<int, string>();

                #region DBCall
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                #region ADL

                cmd = new SqlCommand("select A.ADLId, A.Description from LookupADL A inner join LookupADLArea B on B.ADLAreaId = A.ADLAreaId where A.Active = 1 and B.Description in ('" + selected_ADLArea + "') ORDER BY CASE WHEN B.Description = '" + selected_ADLArea_Order + " END DESC", con);
                cmd.CommandType = CommandType.Text;

                ADLTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(ADLTable);

                foreach (DataRow row in ADLTable.Rows)
                {
                    ADLDict.Add(Convert.ToInt32(row["ADLId"]), row["Description"].ToString().Trim());
                }
                Session["ADL"] = ADLDict;

                con.Close();
                #endregion ADL

                #endregion DBCall

                int columns = ADLAbilityDegreeDict.Count + 1;

                ADLDict = (Dictionary<int, string>)Session["ADL"];

                foreach (string str in ADLDict.Values)
                {
                    ADLList.Add(str);
                }

                int rows = ADLList.Count + 1;

                CreateDynamicControls();
            }
            catch (Exception ex)
            {
                //header_panel.Visible = false;
                Console.WriteLine(ex);
            }

            //Setting it

            if (Session["ADLSelections"] == null)
            {
                //nothing to be set in checkboxes
            }
            else
            {
                Dictionary<String, string> ADLSelectSetDict = (Dictionary<String, string>)Session["ADLSelections"];
                //List<string> SelectSetKeysList = new List<string>();
                //List<string> SelectSetValuesList = new List<string>();
                String[,] ADL_Selected_Session_Array = new String[ADLSelectSetDict.Count, 2];
                string[] temp_dict2array = ADLSelectSetDict.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).ToArray();

                for (int s = 0; s < temp_dict2array.Length; s++)
                {
                    String[] temp_selectedSessionArray = temp_dict2array[s].Split(new string[] { "=" }, StringSplitOptions.None);
                    ADL_Selected_Session_Array[s, 0] = temp_selectedSessionArray[0];
                    ADL_Selected_Session_Array[s, 1] = temp_selectedSessionArray[1];
                }

                foreach (GridViewRow row in GrdView.Rows)
                {
                    int countcols = GrdView.Columns.Count;
                    int countrows = GrdView.Rows.Count;

                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBoxList chkRow = (row.Cells[1].FindControl("chkbxlstADL") as CheckBoxList);

                        string selected_ADL_Label = (row.Cells[1].FindControl("lblADL") as Label).Text;

                        int len = ADL_Selected_Session_Array.Length;

                        for (int i = 0; i < (len / 2); i++)
                        {
                            string val = ADL_Selected_Session_Array[i, 1];
                            if (ADL_Selected_Session_Array[i, 0] == selected_ADL_Label)
                            {
                                ListItem listItem = chkRow.Items.FindByText(val);

                                if (listItem != null)
                                    listItem.Selected = true;
                            }
                        }
                    }
                }
            }

            Thread.Sleep(100);
            chkbxlst_ADLArea.Enabled = true;
            GrdView.Enabled = true;
        }

        public void CreateDynamicControls()
        {
            foreach (DataRow row in ADLAbilityDegreeTable.Rows)
            {
                ADLAbilityDegreeDict.Add(Convert.ToInt32(row["ADLAbilityDegreeId"]), row["Description"].ToString().Trim());
            }
            Session["ADLAbilityDegree"] = ADLAbilityDegreeDict;

            //#endregion ADLAbilityDegree

            ADLAbilityDegreeDict = (Dictionary<int, string>)Session["ADLAbilityDegree"];

            foreach (string str in ADLAbilityDegreeDict.Values)
            {
                ADLDegreeList.Add(str);
            }

            #region UI Creation

            //Code for adding all the rows
            GrdView.DataSource = ADLTable;
            GrdView.DataBind();

            //header_panel.Visible = true;

            #endregion UI Formation

            listofVisibleUpdatePanels[countOfControls] = "updatepanel_" + selected_ADLArea;
            countOfControls++;
        }

        protected void GrdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBoxList chkbxlstADL = (CheckBoxList)e.Row.FindControl("chkbxlstADL");

                chkbxlstADL.DataSource = ADLAbilityDegreeTable;
                chkbxlstADL.DataTextField = "Description";
                chkbxlstADL.DataValueField = "ADLAbilityDegreeId";

                chkbxlstADL.DataBind();
            }
        }

        protected void chkbxlst_DisabilitiesArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkbxlst_DisabilitiesArea.Enabled = false;
            GrdView_Disabilities.Enabled = false;

            try
            {
                string lastSelectedValue = string.Empty;
                string result = Request.Form["__EVENTTARGET"];
                string[] checkedBox = result.Split('$'); ;

                int index = int.Parse(checkedBox[checkedBox.Length - 1]);
                Boolean isChecked = false;

                if (chkbxlst_DisabilitiesArea.Items[index].Selected)
                {
                    isChecked = true;
                    lastSelectedValue = chkbxlst_DisabilitiesArea.Items[index].ToString();
                }
                else
                {
                    lastSelectedValue = chkbxlst_DisabilitiesArea.Items[index].ToString();
                    isChecked = false;
                }

                if (isChecked)
                {
                    if (selected_DisabilitiesArea == null)
                    {
                        selected_DisabilitiesArea = lastSelectedValue;
                        selected_DisabilitiesArea_Order = lastSelectedValue + "' THEN " + queryOrderByCount_ADL + "";
                        queryOrderByCount_ADL++;
                    }
                    else
                    {
                        selected_DisabilitiesArea = lastSelectedValue + "','" + selected_DisabilitiesArea;
                        selected_DisabilitiesArea_Order = selected_DisabilitiesArea_Order + " WHEN B.Description = '" + lastSelectedValue + "' THEN " + queryOrderByCount_Disability + "";
                        queryOrderByCount_Disability++;
                    }
                }
                else
                    if (!isChecked)
                    {
                        String[] temp_selectedDisabilityArea_array = selected_DisabilitiesArea.Split(new string[] { "','" }, StringSplitOptions.None);
                        List<String> temp_selectedDisabilityArea_list = new List<String>();
                        temp_selectedDisabilityArea_list.AddRange(temp_selectedDisabilityArea_array);

                        for (int i = 0; i < temp_selectedDisabilityArea_list.Count; i++)
                        {
                            if (temp_selectedDisabilityArea_list[i].Equals(lastSelectedValue))
                            {
                                //int pos = temp_selectedADLArea_list.IndexOf(lastSelectedValue);
                                temp_selectedDisabilityArea_list.RemoveAt(temp_selectedDisabilityArea_list.IndexOf(lastSelectedValue));
                                break;
                            }
                        }
                        if (temp_selectedDisabilityArea_list.Count != 0)
                        {
                            for (int i = 1; i <= temp_selectedDisabilityArea_list.Count; i++)
                            {
                                if (i == 1)
                                {
                                    selected_DisabilitiesArea = temp_selectedDisabilityArea_list[i - 1];
                                }
                                else
                                {
                                    selected_DisabilitiesArea = selected_DisabilitiesArea + "','" + temp_selectedDisabilityArea_list[i - 1];
                                }
                            }
                        }
                        else
                        {
                            selected_DisabilitiesArea = "";
                        }
                    }

                #region DB Call
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();

                cmd = new SqlCommand("select A.GuideDisabilityId, A.Description from LookUpGuideDisability A inner join LookUpGuideDisabilityArea B on B.GuideDisabilityAreaId = A.GuideDisabilityAreaId where B.Description in ('" + selected_DisabilitiesArea + "') ORDER BY CASE WHEN B.Description = '" + selected_DisabilitiesArea_Order + " END DESC", con);
                cmd.CommandType = CommandType.Text;

                DisabilityTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(DisabilityTable);

                GrdView_Disabilities.DataSource = DisabilityTable;
                GrdView_Disabilities.DataBind();

                //foreach (DataRow row in DisabilityTable.Rows)
                //{
                //    DisabilityDict.Add(Convert.ToInt32(row["GuideDisabilityId"]), row["Description"].ToString().Trim());
                //}
                //Session["Disability"] = DisabilityDict;

                con.Close();
                #endregion DB Call

                //DisabilityDict = (Dictionary<int, string>)Session["Disability"];

                //foreach (string str in DisabilityDict.Values)
                //{
                //    DisabilityList.Add(str);
                //}

                //int rows = ADLList.Count + 1;

                //CreateDynamicControls();
            }
            catch (Exception ex)
            {
                //header_panel.Visible = false;
                Console.WriteLine(ex);
            }

            //Setting it

            if (Session["DisabilitySelections"] == null)
            {
                //nothing to be set in radiobutton list
            }
            else
            {
                Dictionary<String, string> DisabilitySelectSetDict = (Dictionary<String, string>)Session["DisabilitySelections"];

                String[,] Disability_Selected_Session_Array = new String[DisabilitySelectSetDict.Count, 2];
                string[] temp_dict2array = DisabilitySelectSetDict.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).ToArray();

                for (int s = 0; s < temp_dict2array.Length; s++)
                {
                    String[] temp_selectedSessionArray = temp_dict2array[s].Split(new string[] { "=" }, StringSplitOptions.None);
                    Disability_Selected_Session_Array[s, 0] = temp_selectedSessionArray[0];
                    Disability_Selected_Session_Array[s, 1] = temp_selectedSessionArray[1];
                }

                foreach (GridViewRow row in GrdView_Disabilities.Rows)
                {
                    int countcols = GrdView.Columns.Count;
                    int countrows = GrdView.Rows.Count;

                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        RadioButtonList radio = (row.Cells[1].FindControl("radiobtnlst_Disabilities") as RadioButtonList);

                        string selected_Disability_Label = (row.Cells[1].FindControl("lbl_Disabilities") as Label).Text;

                        int len = Disability_Selected_Session_Array.Length;

                        for (int i = 0; i < (len / 2); i++)
                        {
                            string val = Disability_Selected_Session_Array[i, 1];
                            if (Disability_Selected_Session_Array[i, 0] == selected_Disability_Label)
                            {
                                ListItem listItem = radio.Items.FindByText(val);

                                if (listItem != null)
                                    listItem.Selected = true;
                            }
                        }
                    }
                }
            }

            Thread.Sleep(100);

            chkbxlst_DisabilitiesArea.Enabled = true;
            GrdView_Disabilities.Enabled = true;
        }

        private int _tabIndex = 0;

        public int TabIndex
        {
            get
            {
                _tabIndex++;
                return _tabIndex;
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
            //ExceptionFile.ExceptionUtility.LogException(ex, errorHandlerSource);
            Response.Write("<script language=javascript>alert('Error!!! Email sent to the support team. Please contact for further info.')</script>");

            //Email Notification to the support team
            if (Session != null && Session["ATUID"] != null)
            {
                //ExceptionFile.EmailUtility.SendEmail(Session["ATUID"].ToString(), ex.StackTrace);
            }
        }

        protected void radiobtnlst_Disabilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisabilitySelectionDict.Clear();

            foreach (GridViewRow row in GrdView_Disabilities.Rows)
            {
                int countcols = GrdView_Disabilities.Columns.Count;

                if (row.RowType == DataControlRowType.DataRow)
                {
                    RadioButtonList radio = (row.Cells[1].FindControl("radiobtnlst_Disabilities") as RadioButtonList);
                    string val = string.Empty;

                    int index = radio.SelectedIndex;

                    if (index >= 0)
                    {
                        val = radio.Items[index].ToString();
                        try
                        {
                            string selected_Disabilities_Label = (row.Cells[1].FindControl("lbl_Disabilities") as Label).Text;
                            string selected_Disabilities_RadioButton = val;
                            DisabilitySelectionDict.Add(selected_Disabilities_Label, selected_Disabilities_RadioButton);
                        }
                        catch (Exception er)
                        {

                        }
                    }
                }
            }

            Session["DisabilitySelections"] = DisabilitySelectionDict;
        }

        protected void btn_Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("EquipmentReceived.aspx");
        }
    }
}