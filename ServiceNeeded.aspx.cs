using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace ATUClient
{
    public partial class ServiceNeeded : System.Web.UI.Page
    {
         #region Data members
        // Existing
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
        // Existing

        bool[] checkedList = Enumerable.Repeat<bool>(false, 9).ToArray();
        
        //Retained the existing code for the search boxk

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                ViewState["checkedList"] = checkedList;

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
                
                if(ddlst_ReferralDate.SelectedItem==null)
                    PopulateReferralDates();
            }
            else
            {
                if (Session != null && Session["ClientID"] != null)
                {
                    if (ddlst_ReferralDate.SelectedItem == null)
                        PopulateReferralDates();
                }
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

 
                con.Close();

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
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

        private void PopulateSessionVariables(string firstName, string lastName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            Session["FirstName"] = firstName;
            Session["LastName"] = lastName;
            lbl_CurrentClientName.Text = textInfo.ToTitleCase(firstName) + " " + textInfo.ToTitleCase(lastName);
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


        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                ddlst_ReferralDate.Items.Clear();

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

                //GrdView_MobAidView.Visible = GrdView_ADLView.Visible = GrdView_DisabilityView.Visible = false;
               // lbl_SavedDataHeader.Visible = false;

                if (GridView1.Rows.Count <= 0)
                {
                    NotFoundLabel.Visible = true;
                    lbl_CurrentClient.Visible = false;
                    lbl_CurrentClientName.Visible = false;
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
                        lbl_CurrentClient.Visible = true;
                        lbl_CurrentClientName.Visible = true;
                        NotFoundLabel.Visible = false;

                        PopulateReferralDates();

                       // btn_Save.Enabled = true;
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
            NotFoundLabel.Visible = false;

            lbl_CurrentClientName.Text = "";
            GridView1.Visible = false;

            ddlst_ReferralDate.Items.Clear();

            serviceNeededControls.Style.Add("display", "none");

            //ResetControls();
        }


        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            String referralID = ddlst_ReferralDate.SelectedValue;
            Session["ReferralID"] = referralID.ToString();

            try
            {
                //ResetControls();
                

                if (!ddlst_ReferralDate.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    BindServiceList();
                    DisplayServicesNeeded();
                    serviceNeededControls.Style.Add("display", "table-row-group");
                    
                }
                else
                {
                    //batchMethodControls.Style.Add("display", "none");

                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private void BindServiceList()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetLookUpServiceNeededInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            servicesList.DataSource = tab;
            servicesList.DataTextField = "ServiceNeeded";
            servicesList.DataValueField = "LookUpServiceNeededID";
            servicesList.DataBind();
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

                lbl_CurrentClient.Visible = true;
                lbl_CurrentClientName.Visible = true;

                PopulateReferralDates();
            }
            #endregion Populate Grid
        }


        protected void ServiceList_Selected(object sender, EventArgs e)
        {
            int index=0;

            checkedList = (bool[])ViewState["checkedList"];

            for (int i = 0; i < checkedList.Length; i++)
            {
                if (checkedList[i] != servicesList.Items[i].Selected)
                    index = i;
            }


            if (checkedList[index] == true)
            {
                //ViewState["curIndex"] = index;
                RemoveService(index);
                checkedList[index] = false;
                ViewState["checkedList"] = checkedList;

            }
            else
            {
                checkedList[index] = true;
                ViewState["checkedList"] = checkedList;
                ViewState["curIndex"] = index;
                servicesList.Enabled = false;
                commentBox.Style.Add("display", "table-row");
                addButtons.Style.Add("display", "block");
            }

        }

       
        private void RemoveService(int index)
        {

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_DeActivateServiceNeeded", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LookUpServiceNeededID", servicesList.Items[index].Value);
            cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
            
            //----------------------------------------------

            if (con.State == ConnectionState.Open)
                con.Close();

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            DisplayServicesNeeded();
            
        }

        public void AddServiceBtnClick(object sender, EventArgs e)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_AddServiceNeeded", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LookUpServiceNeededID", servicesList.Items[(int)ViewState["curIndex"]].Value);
            cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
            cmd.Parameters.AddWithValue("@NetID", Request.Cookies["ATUId"].Value);
            cmd.Parameters.AddWithValue("@Comments", comments.Text);
            //----------------------------------------------

            var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
            output.Direction = ParameterDirection.Output;


            if (con.State == ConnectionState.Open)
                con.Close();

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


            DisplayServicesNeeded();

            commentBox.Style.Add("display", "none");
            addButtons.Style.Add("display", "none");
            servicesList.Enabled = true;

        }

        private void DisplayServicesNeeded()
        {

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetServiceNeededInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            ServicesGV.DataSource = tab;
            ServicesGV.DataBind();

            //check if returned data contains any rows and display the gridview accordingly
            if (tab.Rows.Count>0)
                ServicesGV.Visible = true;
            else
                ServicesGV.Visible = false;

            ViewState["servicesNeeded"] = tab;


            foreach (DataRow row in tab.Rows)
            {
                checkedList[(int)row[0]-1] = servicesList.Items[(int)row[0]-1].Selected = true;

            }
            ViewState["checkedList"] = checkedList;
 
        }

        public void CancelBtnClick(object sender, EventArgs e)
        {
            //get the current index that was clicked
            int cur = (int)ViewState["curIndex"];

            //uncheck the box
            servicesList.Items[cur].Selected = false;

            servicesList.Enabled = true;

            commentBox.Style.Add("display", "none");
            addButtons.Style.Add("display", "none");
            servicesList.Enabled = true;
        }


       }

        
    }
#endregion