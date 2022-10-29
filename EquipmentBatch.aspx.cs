using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections;
using System.Globalization;
using System.Net.Mail;


namespace ATUClient
{
    public partial class EquipmentBatch : System.Web.UI.Page
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


        //Vishwas
        private int? atAreaId, tier1Id, tier2Id, equipId;
        private DataTable productInfo = new DataTable();
        private DataTable productInfoMaunal;
        private static DataTable OverallProjectInfo;
        private DataTable batchedItems = new DataTable();
        private DataTable OverallProjectCostItems = new DataTable();
        private List<int> originalQtyManual = new List<int>();
        private static Dictionary<string, int> originalQtyExceptManual = new Dictionary<string, int>();
        private static List<int> minQty = new List<int>();
        private bool saveClicked = false;
        private bool genReportClicked = false;
        private static bool isOPDisplayed = false;
        private Dictionary<int, int> originalQtyMaster = new Dictionary<int, int>();

        private Dictionary<int, List<int>> selectedRowsForReport = new Dictionary<int, List<int>>();
        private Dictionary<int, List<string>> selectedProductCodesForReport = new Dictionary<int, List<string>>();

        private Dictionary<int, int> delivered = new Dictionary<int, int>();
        private static Dictionary<int, int> returned = new Dictionary<int, int>();
        public virtual bool ShowSelectButton { get; set; }

        #endregion



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

                productInfo.Clear();
            }

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
                    //btn_Save.Enabled = true;
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
                ResetControls();
                manualControls.Style.Add("display", "none");
                batchMethodControls.Style.Add("display", "none");
                barcodeTextControls.Style.Add("display", "none");
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

            ResetControls();
            manualControls.Style.Add("display", "none");
            batchMethodControls.Style.Add("display", "none");
            barcodeTextControls.Style.Add("display", "none");

        }



        protected void ReferralDate_Changed(object sender, EventArgs e)
        {
            String referralID = ddlst_ReferralDate.SelectedValue;
            Session["ReferralID"] = referralID.ToString();

            try
            {
                ResetControls();
                manualControls.Style.Add("display", "none");
                barcodeTextControls.Style.Add("display", "none");

                if (!ddlst_ReferralDate.SelectedItem.Text.Trim().Equals("--Choose one--"))
                {
                    BindATArea();
                    DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);
                    batchMethodControls.Style.Add("display", "table-row-group");
                    ScanRB.Checked = true;
                    ManualRB.Checked = false;
                    BarcodeTextRB.Checked = false;
                    BatchTypeCheckedChanged(ScanRB as object, new EventArgs());
                }
                else
                {
                    batchMethodControls.Style.Add("display", "none");

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


     //Retrieve scanned barcode from the UI and send it to the procedure to get the product info
        public void HandleBarcode(object sender, EventArgs e)
        {
            errorMsg.Visible = false;
            warningMsg.Visible = false;
            int res;

            if (!int.TryParse(quantityScan.Text, out res) || Convert.ToInt32(quantityScan.Text) < 1)
            {
                errorMsg.InnerHtml = "Entered quantity is invalid. Please try again.";
                errorMsg.Visible = true;
            }
            else
            {
                AddItemWithBarcode(barcode.Text, Convert.ToInt32(quantityScan.Text));
       
            }        
        }

        private void AddItemWithBarcode(string barcode, int quantity)
        {
            errorMsg.Visible = false;

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetEquipmentProductInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Barcode", barcode);

            //----------------------------------------------

            var tempTable = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
            con.Close();



            if (tempTable.Rows.Count > 0)
            {
                //if (!originalQtyExceptManual.Keys.Contains(barcode)) ---remove orgqtyexcmanual
                //    originalQtyExceptManual.Add(barcode, Convert.ToInt32(tempTable.Rows[0]["Quantity"]));

                if(ViewState["originalQtyManual"]!=null)
                    originalQtyMaster = (Dictionary<int, int>)ViewState["originalQtyMaster"];

                if (!originalQtyMaster.Keys.Contains(Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"])))
                {
                    originalQtyMaster.Add(Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"]), Convert.ToInt32(tempTable.Rows[0]["Quantity"]));
                    ViewState["originalQtyMaster"] = originalQtyMaster;
                }
                int existingQty = originalQtyMaster[Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"])];

                if (existingQty < quantity)
                {
                    errorMsg.InnerHtml = "There are only " + existingQty.ToString() + " items left for this product.";
                    errorMsg.Visible = true;
                }
                else
                {
                    tempTable.Rows[0]["Quantity"] = quantity.ToString();

                    originalQtyMaster[Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"])] -= quantity;

                    //if (!originalQtyMaster.Keys.Contains(Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"])))
                    //    originalQtyMaster.Add(Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"]), existingQty);
                    //else
                    //    originalQtyMaster[Convert.ToInt32(tempTable.Rows[0]["EquipmentProductId"])] = existingQty;


                    productInfo.Merge(tempTable);

                    BatchGridView.DataSource = productInfo;
                    BatchGridView.DataBind();
                    BatchGridView.Visible = true;
                    saveItems.Visible = true;

                    if (BatchGridView.Rows.Count <= 0)
                        batchFailed.Style.Add("display", "inherit");
                    else
                        itemsHeader.Style.Add("display", "inherit");

                    ViewState["productInfo"] = productInfo;
                    ViewState["originalQtyMaster"] = originalQtyMaster;

                }

            }
            else
            {
                errorMsg.InnerHtml = "Record not found. Please check the barcode or the inventory.";
                errorMsg.Visible = true;
            }
        }

        public void BatchTypeCheckedChanged(Object sender, EventArgs e)
        {
            errorMsg.Visible = false;
            RadioButton rbtn = (RadioButton)sender;
            if (rbtn.ID == ManualRB.ID)
            {
                barcodeTextControls.Style.Add("display", "none");
                scanType.Style.Add("display", "none");
                //AtAreaDd.SelectedIndex = 0;
                Tier1Dd.Items.Clear();
                Tier2Dd.Items.Clear();
                EquipDd.Items.Clear();
                productDd.Items.Clear();
                quantityTB.Text = string.Empty;

                manualControls.Style.Add("display", "table-row-group");

                // SHR
                UnloadForATArea();
            }

            else if (rbtn.ID == ScanRB.ID)
            {
                scanType.Style.Add("display", "table-row");
                manualControls.Style.Add("display", "none");
                barcodeTextControls.Style.Add("display", "none");

            }

            else
            {
                barcodeTB.Text = string.Empty;
                scanType.Style.Add("display", "none");
                manualControls.Style.Add("display", "none");
                barcodeTextControls.Style.Add("display", "table-row-group");
            }

        }

        private void UnloadForTier1()
        {
            tier2.Style.Add("display", "none");
            UnloadForTier2();
        }

        private void UnloadForEquip()
        {
            prod.Style.Add("display", "none");
            UnloadForProd();
        }

        private void UnloadForProd()
        {
            submitOP.Style.Add("display", "none");
            submitSupp.Style.Add("display", "none");
            //submitSupp.Style.Add("display", "none");
            //submitSupp.Style.Add("display", "none");
            //submitSupp.Style.Add("display", "none");
            prodQty.Style.Add("display", "none");
        }

        private void UnloadForTier2()
        {
            Equip.Style.Add("display", "none");
            EquipDd.Style.Add("display", "none");
            UnloadForEquip();
        }

        private void UnloadForATArea()
        {
            tier1.Style.Add("display", "none");
            UnloadForTier1();
        }


        public void AddProdBtnClick(object sender, EventArgs e)
        {
            errorMsg.Visible = false;
            warningMsg.Visible = false;

            int res;

            if (!int.TryParse(quantityTB2.Text, out res) || Convert.ToInt32(quantityTB2.Text) < 1)
            {
                errorMsg.InnerHtml = "Entered quantity is invalid. Please try again.";
                errorMsg.Visible = true;
            }
            else
            {
                AddItemWithBarcode(barcodeTB.Text, Convert.ToInt32(quantityTB2.Text));

            }
        }


   #region Manual product selection

        private void BindATArea()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("usp_GetEquipmentATArea", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            AtAreaDd.DataSource = tab;
            AtAreaDd.DataTextField = "Description";
            AtAreaDd.DataValueField = "EquipmentATAreaId";
            AtAreaDd.DataBind();

            AtAreaDd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
        }


        public void ATArea_SelectionChanged(object sender, EventArgs e)
        {
            UnloadForTier1();
            if (AtAreaDd.SelectedIndex >= 1)
            {
                atAreaId = Convert.ToInt32(AtAreaDd.SelectedItem.Value);
                ViewState["atAreaId"] = atAreaId;

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("usp_GetTier1FromATArea", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                Tier1Dd.Items.Clear();
                Tier1Dd.DataSource = tab;
                Tier1Dd.DataTextField = "Tier1";
                Tier1Dd.DataValueField = "Tier1Id";
                Tier1Dd.DataBind();

                Tier1Dd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                tier1.Style.Add("display", "table-row");

            }
            else
                Tier1Dd.Items.Clear();

            Tier2Dd.Items.Clear();

            EquipDd.Items.Clear();

            productDd.Items.Clear();

            quantityTB.Text = "";

          
        }


        public void Tier1_SelectionChanged(object sender, EventArgs e)
        {
            UnloadForTier1();
            if (Tier1Dd.SelectedIndex != 0)
            {
                tier1Id = Convert.ToInt32(Tier1Dd.SelectedItem.Value);

                ViewState["tier1Id"] = tier1Id;

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("usp_GetTier2", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ATAreaId", (int)ViewState["atAreaId"]);
                cmd.Parameters.AddWithValue("@Tier1Id", tier1Id);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                Tier2Dd.Items.Clear();
                Tier2Dd.DataSource = tab;
                Tier2Dd.DataTextField = "Tier2";
                Tier2Dd.DataValueField = "Tier2Id";
                Tier2Dd.DataBind();

                Tier2Dd.Items.Insert(0, new ListItem("--Choose one--", "0"));

                if (Tier2Dd.Items.Count > 2)
                {
                    EquipDd.Items.Clear();
                    productDd.Items.Clear();
                    tier2.Style.Add("display", "table-row");
                    quantityTB.Text = "";
                }
                else if (Tier2Dd.Items.Count == 2)
                {
                    tier2.Style.Add("display", "none");
                    tier2Id = Convert.ToInt32(Tier2Dd.Items[1].Value);
                    ViewState["tier2Id"] = tier2Id;
                    LoadEquipDd();
                    productDd.Items.Clear();
                    quantityTB.Text = "";
                }
                else
                {
                    tier2.Style.Add("display", "none");
                    tier2Id = null;
                    ViewState["tier2Id"] = tier2Id;
                    LoadEquipDd();
                    productDd.Items.Clear();
                    quantityTB.Text = "";
                }

            }
        }

        public void Tier2_SelectionChanged(object sender, EventArgs e)
        {
            UnloadForEquip();
            tier2Id = Convert.ToInt32(Tier2Dd.SelectedItem.Value);
            ViewState["tier2Id"] = tier2Id;

            LoadEquipDd();
        }

        private void LoadEquipDd()
        {
            UnloadForProd();
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("usp_EquipmentList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ATAreaId", (int)ViewState["atAreaId"]);
            cmd.Parameters.AddWithValue("@Tier1Id", (int)ViewState["tier1Id"]);
            //cmd.Parameters.AddWithValue("@Tier2Id", tier2Id);
            if (tier2Id != null)
            {
                cmd.Parameters.AddWithValue("@Tier2Id", (int)ViewState["tier2Id"]);
            }
            else
                cmd.Parameters.AddWithValue("@Tier2Id", DBNull.Value);

            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);


            EquipDd.Items.Clear();
            EquipDd.DataSource = tab;
            EquipDd.DataTextField = "Equipment";
            EquipDd.DataValueField = "EquipmentUniqueId";
            EquipDd.DataBind();

            EquipDd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

            DataColumn[] columns = tab.Columns.Cast<DataColumn>().ToArray();
            bool anyFieldContainsVal = tab.AsEnumerable()
                .Any(row => columns.Any(col => row[col].ToString() == "Overall Project"));

            if (anyFieldContainsVal)
            {
                isOPDisplayed = true;
                EquipDd.Style.Add("display", "none");
                Equip.Style.Add("display", "none");
                equipId = null;
                ViewState["equipId"] = equipId;
                submitOP.Style.Add("display", "table-row");
                submitSupp.Style.Add("display", "table-row");
                //submitSupp.Style.Add("display", "none");
                //submitSupp.Style.Add("display", "none");
            }
            else
            {
                isOPDisplayed = false;
                Equip.Style.Add("display", "table-row");
                EquipDd.Style.Add("display", "table-row");
                productDd.Items.Clear();
                quantityTB.Text = "";
            }

        }

        public void Equip_SelectionChanged(object sender, EventArgs e)
        {

            if (EquipDd.SelectedIndex == 0)
                return;

           equipId = Convert.ToInt32(EquipDd.SelectedItem.Value);
           ViewState["equipId"] = equipId;


           SqlConnection con = null;
           SqlCommand cmd = null;

           con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
           con.Open();
           cmd = new SqlCommand("USP_GetEquipmentProductInfoManually", con);
           cmd.CommandType = CommandType.StoredProcedure;
           cmd.Parameters.AddWithValue("@EquipmentUniqueId", equipId);
          
           //----------------------------------------------
           productInfoMaunal = new DataTable();
           using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(productInfoMaunal);


           productDd.Items.Clear();
           productDd.DataSource = productInfoMaunal;
           productDd.DataTextField = "Description";
           productDd.DataValueField = "EquipmentProductId";
           productDd.DataBind();

           productDd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

           prod.Style.Add("display", "table-row");

           prodQty.Style.Add("display", "table-row");

           originalQtyManual.Clear();

           minQty.Clear();  

           if (productInfoMaunal != null)
               foreach (DataRow g in productInfoMaunal.Rows)
               {
                   //g[0]-ProdId, g[1]-Desc, g[2]-Quantity, g[3]-MinQty, g[4]-Cost

                   originalQtyManual.Add(Convert.ToInt32(g[2]));
                   minQty.Add(Convert.ToInt32(g[3]));

                   if (!originalQtyMaster.Keys.Contains(Convert.ToInt32(g[0])))
                       originalQtyMaster.Add(Convert.ToInt32(g[0]), Convert.ToInt32(g[2]));
         
               }

           quantityTB.Text = "1";

            ViewState["productInfoMaunal"]=productInfoMaunal;
            ViewState["originalQtyManual"]=originalQtyManual;
            ViewState["originalQtyMaster"] = originalQtyMaster;
            ViewState["minQty"] = minQty;

        }

        public void ProdAddBtnClick(object sender, EventArgs e)
        {

            errorMsg.Visible = false;


            if(ViewState["originalQtyManual"]!=null)
                originalQtyManual = (List<int>)ViewState["originalQtyManual"];

            if (ViewState["originalQtyMaster"] != null)
                originalQtyMaster = (Dictionary<int,int>)ViewState["originalQtyMaster"];

            
            var quantity = Convert.ToInt32(quantityTB.Text);

         
             if(ValidateQuantity(quantityTB))
             {
                 if(ViewState["productInfoMaunal"]!=null)
                    productInfoMaunal = (DataTable)ViewState["productInfoMaunal"];

                 productInfoMaunal.Rows[productDd.SelectedIndex - 1]["Quantity"] = quantityTB.Text;

                 itemsHeader.Style.Add("display", "inherit");

                 if (ViewState["productInfo"] != null)
                     productInfo = (DataTable)ViewState["productInfo"];

                 if (productInfo.Rows.Count < 1)
                     productInfo = productInfoMaunal.Clone();

                 productInfo.Rows.Add(productInfoMaunal.Rows[productDd.SelectedIndex-1].ItemArray);

                 BatchGridView.DataSource = productInfo;
                 BatchGridView.DataBind();
                 BatchGridView.Visible = true;
                 saveItems.Visible = true;

                 originalQtyMaster[Convert.ToInt32(productDd.SelectedItem.Value)] -= quantity;

                 ViewState["productInfo"] = productInfo;

                 ViewState["originalQtyMaster"] = originalQtyMaster;

                 ViewState["productInfoMaunal"] = productInfoMaunal;

                 //if (!originalQtyMaster.Keys.Contains(Convert.ToInt32(productDd.SelectedItem.Value))) --- remove originalqtymanual member
                 //    originalQtyMaster.Add(Convert.ToInt32(productDd.SelectedItem.Value), originalQtyManual[originalQtyManual.Count - 1]);
                 //else
                 //    originalQtyMaster[Convert.ToInt32(productDd.SelectedItem.Value)] = originalQtyManual[originalQtyManual.Count - 1];
             }
        }

        public void OPAddBtnClick(object sender, EventArgs e)
        {
            OverallProjectInfo = new DataTable(); 
            OverallProjectInfo.Columns.Add("EquipmentProductId", typeof(string));
            OverallProjectInfo.Columns.Add("Description", typeof(string)); 
            OverallProjectInfo.Columns.Add("Quantity", typeof(string));
            OverallProjectInfo.Columns.Add("MinQuantity", typeof(string));
            OverallProjectInfo.Columns.Add("Cost", typeof(string));

            OverallProjectInfo.Rows.Add(256, "Overall Project:(ATU $)", 1,2,TextBoxOP.Text);
            OverallProjectInfo.Rows.Add(256, "Overall Project:(Supp $)", 1, 2,TextBoxSupp.Text);
            BatchGridView.DataSource = OverallProjectInfo;
            BatchGridView.DataBind();
            BatchGridView.Visible = true;
            saveItems.Visible = true;
        }


        private bool ValidateQuantity(TextBox quantityTB)
        {
            var quantity = Convert.ToInt32(quantityTB.Text);

            if (productDd.SelectedIndex == 0)
            {
                errorMsg.InnerHtml = "Please select a product to add.";
                errorMsg.Visible = true;
                return false;

            }

            else if (quantity < 1)
            {
                errorMsg.InnerHtml = "Quantity should be atleast 1";
                errorMsg.Visible = true;
                return false;
            }
            else if (originalQtyMaster[Convert.ToInt32(productDd.SelectedItem.Value)] < 1)
            {
                errorMsg.InnerHtml = "There are no more items left for this product. Please check inventory.";
                errorMsg.Visible = true;
                return false;
            }

            else if (quantity > originalQtyMaster[Convert.ToInt32(productDd.SelectedItem.Value)])
            {
                errorMsg.InnerHtml = "Quantity should be less than or equal to " + originalQtyMaster[Convert.ToInt32(productDd.SelectedItem.Value)];
                errorMsg.Visible = true;
                return false;
            }
            else
                return true;

        }

   #endregion

        public void RemoveItemClick(object sender, GridViewDeleteEventArgs e)
        {
            if (isOPDisplayed)
            {
                OverallProjectInfo.Rows.RemoveAt(e.RowIndex);
                BatchGridView.DataSource = OverallProjectInfo;
                BatchGridView.DataBind();

                if (BatchGridView.Rows.Count == 0)
                {
                    saveItems.Visible = false;
                    itemsHeader.Style.Add("display", "none");
                }

            }
            else
            {
                errorMsg.Visible = false;
                warningMsg.Visible = false;

                if (ViewState["originalQtyMaster"] != null)
                    originalQtyMaster = (Dictionary<int, int>)ViewState["originalQtyMaster"];

                Label quantityVal = (Label)BatchGridView.Rows[e.RowIndex].Cells[2].FindControl("lblQuantity");
                Label prodId = (Label)BatchGridView.Rows[e.RowIndex].Cells[0].FindControl("lblProductId");
                originalQtyMaster[Convert.ToInt32(prodId.Text)] += Convert.ToInt32(quantityVal.Text);
                //originalQty[e.RowIndex] += Convert.ToInt32(quantityVal.Text);

                productInfo = (DataTable)ViewState["productInfo"];
                productInfo.Rows.RemoveAt(e.RowIndex);
                BatchGridView.DataSource = productInfo;
                BatchGridView.DataBind();

                if (BatchGridView.Rows.Count == 0)
                {
                    saveItems.Visible = false;
                    itemsHeader.Style.Add("display", "none");
                }

                ViewState["productInfo"] = productInfo;
                ViewState["originalQtyMaster"] = originalQtyMaster;
            }
        }

        public void SaveItemsClick(object sender, EventArgs e)
        {
            errorMsg.Visible = false;
            warningMsg.Visible = false;
            itemsHeader.Style.Add("display", "none");
            int retval;
            if (isOPDisplayed)
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_AddEquipmentOverallProjectCost", con);
                cmd.CommandType = CommandType.StoredProcedure; // productInfoMaunal.Rows[productDd.SelectedIndex - 1]["Quantity"]
                cmd.Parameters.AddWithValue("@OverallProjectCost", OverallProjectInfo.Rows[0]["Cost"]);
                cmd.Parameters.AddWithValue("@OverallProjectSuppCost", OverallProjectInfo.Rows[1]["Cost"]);
                cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@Quantites", OverallProjectInfo.Rows[0]["Quantity"]);
                //cmd.Parameters.AddWithValue("@NetID", Request.Cookies["ATUId"].Value);
                
                // Exchange with upper while adding to DB
                cmd.Parameters.AddWithValue("@NetID", "srao53");

                var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                output.Direction = ParameterDirection.Output;

                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                retval = Convert.ToInt32(returnParameter.Value);

                con.Close();
            }
            else
            {
                string str = BatchGridView.Rows[0].Cells[1].Text;
                Label prod;
                Label quantity;
                Hashtable prodQuantity = new Hashtable();

                //Merge all duplicate product entries into one 
                foreach (GridViewRow g in BatchGridView.Rows)
                {

                    prod = (Label)g.Cells[0].FindControl("lblProductId");
                    quantity = (Label)g.Cells[2].FindControl("lblQuantity");

                    if (prodQuantity.ContainsKey(prod.Text))
                        prodQuantity[prod.Text] = (int)prodQuantity[prod.Text] + Convert.ToInt32(quantity.Text);
                    else
                        prodQuantity.Add(prod.Text, Convert.ToInt32(quantity.Text));

                }


                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_AddEquipmentBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@Products", string.Join(",", prodQuantity.Keys.Cast<string>().ToArray()) + ",");
                cmd.Parameters.AddWithValue("@Quantites", string.Join(",", prodQuantity.Values.Cast<int>().Select(i => i.ToString()).ToArray()) + ",");
                //cmd.Parameters.AddWithValue("@NetID", Request.Cookies["ATUId"].Value);
                cmd.Parameters.AddWithValue("@NetID", "srao53");
                var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                output.Direction = ParameterDirection.Output;


                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                retval = Convert.ToInt32(returnParameter.Value);

                con.Close();
            }

            if (retval >= 0)
            {
                saveClicked = true;
                DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);

                ViewState["saveClicked"] = saveClicked;

            }
            else
            {
                errorMsg.InnerHtml = "Batching failed. Please contact administrator.";
                errorMsg.Visible = true;
            }
        }

        public void DisplayBatchedItems(string referralId)
        {
            try
            {
                errorMsg.Visible = false;
                warningMsg.Visible = false;
                BatchGridView.Visible = false;
                productInfo.Clear();
                saveItems.Visible = false;
                batchedItems.Clear();


                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_GetBatchedProductsInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReferralId", referralId);

                //----------------------------------------------

                //var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(batchedItems);

                con.Close();

                SqlConnection con1 = null;
                SqlCommand cmd1 = null;

                // Taking data of Overall Project and Adding to regular batched item object
                con1 = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con1.Open();
                cmd1 = new SqlCommand("USP_GetOverallProjectInfo", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@ReferralId", referralId);

                //----------------------------------------------

                //var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd1)) myAdapter.Fill(OverallProjectCostItems);

                con1.Close();

                foreach (DataRow dr in OverallProjectCostItems.Rows)
                {
                    batchedItems.Rows.Add(dr.ItemArray);
                }

                UpdateDeliveredItems();

                saveClicked = ViewState["saveClicked"] != null ? (bool)ViewState["saveClicked"] : false;

                saveClicked = saveClicked == null ? false : saveClicked;

                if (saveClicked)
                {
                    ValidateMinimumQuantity(referralId);
                    saveClicked = false;
                    ViewState["saveClicked"] = saveClicked;

                }
            }
            catch(Exception ex)
            {

            }
        }

        private void UpdateDeliveredItems()
        {
            try
            {
                delivered.Clear();
                returned.Clear();

                batchedItems.DefaultView.Sort = "InputDate" + " " + "DESC";
                batchedItems = batchedItems.DefaultView.ToTable();

                if (batchedItems.Rows.Count > 0)
                {
                    for (int i = 0; i < batchedItems.Rows.Count; i++)
                    {
                        if (batchedItems.Rows[i]["Status"] != DBNull.Value) //Checking for a delivered/returned row. If value is null, then it means that its still pending delivery action
                        {
                            // if there is more than one row in datatable with the same batchid and prodid,
                            // it means that a product has been both delivered and returned, in some quantities.
                            // In that case, get the duplicate rows, save delivered and returned quantities and remove the duplicate entry in the datatable(since we need to see only a single row in gridview).

                            if (batchedItems.Select("EquipmentBatchId = " + batchedItems.Rows[i]["EquipmentBatchId"] + "AND EquipmentProductId = " + batchedItems.Rows[i]["EquipmentProductId"]).Length > 1)
                            {
                                DataRow[] duplicates = batchedItems.Select("EquipmentBatchId = " + batchedItems.Rows[i]["EquipmentBatchId"] + "AND EquipmentProductId = " + batchedItems.Rows[i]["EquipmentProductId"]);

                                if ((Convert.ToInt32(duplicates[0]["Status"])) == 1)
                                {
                                    delivered.Add(batchedItems.Rows.IndexOf(duplicates[0]), Convert.ToInt32(duplicates[0]["Qty"]));
                                    returned.Add(batchedItems.Rows.IndexOf(duplicates[0]), Convert.ToInt32(duplicates[1]["Qty"])); //same index for key, but the quantity of the returned item for value in retunred dict
                                }
                                else
                                {
                                    delivered.Add(batchedItems.Rows.IndexOf(duplicates[0]), Convert.ToInt32(duplicates[1]["Qty"]));
                                    returned.Add(batchedItems.Rows.IndexOf(duplicates[0]), Convert.ToInt32(duplicates[0]["Qty"])); //same index for key, but the quantity of the returned item for value in retunred dict

                                }


                                batchedItems.Rows.Remove(duplicates[1]);

                            }

                            // otherwise just go with the flow

                            else if (Convert.ToInt32(batchedItems.Rows[i]["Status"]) == 1)
                                delivered.Add(i, Convert.ToInt32(batchedItems.Rows[i]["Qty"]));

                            else if (Convert.ToInt32(batchedItems.Rows[i]["Status"]) == -1)
                                returned.Add(i, Convert.ToInt32(batchedItems.Rows[i]["Qty"]));
                        }
                    }
                }

                BatchedItemsGV.DataSource = batchedItems;
                BatchedItemsGV.DataBind();
                BatchedItemsGV.Visible = true;

                ViewState["batchedItems"] = batchedItems;
                ViewState["delivered"] = delivered;
                ViewState["returned"] = returned;

                if (BatchedItemsGV.Rows.Count > 0)
                {
                    UpdateBatchedItemsGridView(BatchedItemsGV.PageIndex); //changes all the delivered and returned button states and updates other date and input user information
                    prevBatch.Style.Add("display", "inherit");
                    genReportBtn.Visible = true;

                }
                else
                    prevBatch.Style.Add("display", "none");
            }
            catch(Exception ex)
            {

            }
        }


        private void UpdateBatchedItemsGridView(int pageNum)
        {
            batchedItems = (DataTable)ViewState["batchedItems"];
            delivered = (Dictionary<int, int>)ViewState["delivered"];
            returned = (Dictionary<int, int>)ViewState["returned"];

            int pgSize = BatchedItemsGV.PageSize;
            pageNum++; //pgindex starts fromn 0 and we need it to start from 1

            var dList = delivered.Keys.ToList().FindAll(x => x >= (pageNum * pgSize - pgSize) && x < pageNum * pgSize);
            dList.Sort();

            var rList = returned.Keys.ToList().FindAll(x => x >= (pageNum * pgSize - pgSize) && x < pageNum * pgSize);
            rList.Sort();


            if (dList.Count > 0)
            {
                foreach (int keyVal in dList)
                {
                    int key = keyVal % pgSize;
                    ChangeDeliverBtnState(key, delivered[keyVal]); //passing individual items because the batcheditems datatable is not always up to date
                    UpdateDeliveryDetails(key, Convert.ToDateTime(batchedItems.Rows[keyVal]["ActionDate"]), Convert.ToString(batchedItems.Rows[keyVal]["InputBy"]));
                }
            }

            if (rList.Count > 0)
            {
                foreach (int keyVal in rList)
                {
                    int key = keyVal % pgSize;
                    ChangeReturnBtnState(key, returned[keyVal]);
                    UpdateDeliveryDetails(key, Convert.ToDateTime(batchedItems.Rows[keyVal]["ActionDate"]), Convert.ToString(batchedItems.Rows[keyVal]["InputBy"]));
                }
            }

            genReportClicked = ViewState["genReportClicked"] != null ? (bool)ViewState["genReportClicked"] : false;

            if (ViewState["selectedRowsForReport"] != null)
                selectedRowsForReport = (Dictionary<int,List<int>>)ViewState["selectedRowsForReport"];
           
            if (genReportClicked && selectedRowsForReport.Keys.Contains(pageNum))
            {
                foreach (int i in selectedRowsForReport[pageNum])
                {
                    CheckBox box = (BatchedItemsGV.Rows[i].Cells[0].FindControl("chkItem") as CheckBox);
                    box.Checked = true;
                }
            }
        }
       
        public void DeliverItemClick(object sender, GridViewUpdateEventArgs e)
        {
            var batchId = (BatchedItemsGV.Rows[e.RowIndex].Cells[1].FindControl("lblBatchId") as Label).Text;
            var prodId = (BatchedItemsGV.Rows[e.RowIndex].Cells[2].FindControl("lblProductId") as Label).Text;
            var qty = (BatchedItemsGV.Rows[e.RowIndex].Cells[4].FindControl("lblBatchedQuantity") as Label).Text;
            
            var descriptionText = (BatchedItemsGV.Rows[e.RowIndex].Cells[3].FindControl("lblDescription") as Label).Text;
            if(descriptionText.Contains("Overall Project Cost($)"))
            {
                prodId = "OverallProject";
            }

            MarkProductDelivered(batchId, prodId, Convert.ToInt32(qty),1);

            DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);
            //ChangeDeliverBtnState(e.RowIndex, Convert.ToInt32(qty));
            //UpdateDeliveryDetails(e.RowIndex, DateTime.Now, Request.Cookies["ATUId"].Value);
        }

        //Edited 02/17/2021
        /* 
        protected void BatchedItemsGV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[6].Controls[0]).OnClientClick = "if (!window.confirm('Are you sure you want to delete this item?')) return false;"; // add any JS you want here
            }
 
            foreach (GridViewRow row in BatchedItemsGV.Rows)
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
                    //lb.Click += new EventHandler(lb_Click);
                    lb.Font.Size = FontUnit.Small;

                    lbl_LomnWritten.Visible = false;
                    lb.Text = str;
                    //if (row.Cells[2].FindControl("DoctorLink_" + DocID) == null)
                    if (row.Cells[3].FindControl("DoctorLink_" + DocID + "_" + InputByNetId) == null)
                        row.Cells[3].Controls.Add(lb);
                }
            }
            
    }*/
        public void BatchedItemsGV_DataBound(object sender, EventArgs e)
                {
                    if (Session != null)
                    {
                        if (!Convert.ToBoolean(Session["IsAdmin"]))
                        {
                            foreach (GridViewRow row in BatchedItemsGV.Rows)
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
        public void DeleteBatchedItemClick(object sender, GridViewEditEventArgs e)
        {
            var batchId = (BatchedItemsGV.Rows[e.NewEditIndex].Cells[1].FindControl("lblBatchId") as Label).Text;
            var prodId = (BatchedItemsGV.Rows[e.NewEditIndex].Cells[2].FindControl("lblProductId") as Label).Text;
            var qty = (BatchedItemsGV.Rows[e.NewEditIndex].Cells[4].FindControl("lblBatchedQuantity") as Label).Text;

            var descriptionText = (BatchedItemsGV.Rows[e.NewEditIndex].Cells[3].FindControl("lblDescription") as Label).Text;
            if (descriptionText.Contains("Overall Project Cost($)"))
            {
                prodId = "OverallProject";
            }

            //MarkProductDelivered(batchId, prodId, Convert.ToInt32(qty),1);
            MarkBatchDelete(batchId, prodId);
            //UpdateQuantity(batchId, prodId, qty); Added by shr not needed in prod
            DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);
            //ChangeDeliverBtnState(e.RowIndex, Convert.ToInt32(qty));
            //UpdateDeliveryDetails(e.RowIndex, DateTime.Now, Request.Cookies["ATUId"].Value);
        }

        public void ReturnItemClick(object sender, GridViewDeleteEventArgs e)
        {
            errorMsg.Visible = false;
            warningMsg.Visible = false;

            int actualQty = Convert.ToInt32((BatchedItemsGV.Rows[e.RowIndex].Cells[4].FindControl("lblBatchedQuantity") as Label).Text);
            int returnQty = Convert.ToInt32(sendQty.Value);           
            var batchId = (BatchedItemsGV.Rows[e.RowIndex].Cells[1].FindControl("lblBatchId") as Label).Text;
            var prodId = (BatchedItemsGV.Rows[e.RowIndex].Cells[2].FindControl("lblProductId") as Label).Text;

            var descriptionText = (BatchedItemsGV.Rows[e.RowIndex].Cells[3].FindControl("lblDescription") as Label).Text;
            if (descriptionText.Contains("Overall Project Cost($)"))
            {
                prodId = "OverallProject";
            }
                if (returnQty > actualQty)
                {
                    errorMsg.InnerHtml = "Entered value is greater than the batched quantity";
                    errorMsg.Visible = true;
                }
                else if (actualQty == returnQty)
                {

                    //before major changes

                    //SqlConnection con = null;
                    //SqlCommand cmd = null;

                    //con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    //con.Open();
                    //cmd = new SqlCommand("USP_UpdateReturnedProductStatus", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@BatchId", batchId);
                    //cmd.Parameters.AddWithValue("@ProductId", prodId);
                    //cmd.Parameters.AddWithValue("@Quantity", returnQty);

                    //if (con.State == ConnectionState.Open)
                    //    con.Close();

                    //con.Open();
                    //cmd.ExecuteNonQuery();
                    //con.Close();

                    MarkProductDelivered(batchId, prodId, returnQty, -1);
                    DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);
                }
                else
                {
                    MarkProductDelivered(batchId, prodId, actualQty - returnQty, 1);
                    MarkProductDelivered(batchId, prodId, returnQty, -1);
                    DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);
                }

            if (BatchedItemsGV.Rows.Count == 0)
                prevBatch.Style.Add("display", "none");            
        }

        private void UpdateDeliveryDetails(int rownum, DateTime deliveredOn, string deliveredBy)
        {
            try
            {
                Label details;

                //details = BatchedItemsGV.Rows[rownum].Cells[3].FindControl("lblBatchedQuantity") as Label;
                //details.Text = qty.ToString();

                details = BatchedItemsGV.Rows[rownum].Cells[5].FindControl("lblInputDate") as Label;
                details.Text = deliveredOn.ToString();

                details = BatchedItemsGV.Rows[rownum].Cells[6].FindControl("lblBatchedBy") as Label;
                details.Text = deliveredBy.ToString();
            }
            catch(Exception ex)
            {

            }

        }

        public Boolean IsVisible()
        {
            return true;
        }

        protected void BatchedItemsGV_RowCommand1(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditReturn")
            {
                bool isOverallProject = false;
                bool isReturn = false;
                GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                int RowIndex = gvr.RowIndex;

                var batchId = (BatchedItemsGV.Rows[RowIndex].Cells[1].FindControl("lblBatchId") as Label).Text;
                var prodId = (BatchedItemsGV.Rows[RowIndex].Cells[2].FindControl("lblProductId") as Label).Text;
                var qty = (BatchedItemsGV.Rows[RowIndex].Cells[4].FindControl("lblBatchedQuantity") as Label).Text;

                var descriptionText = (BatchedItemsGV.Rows[RowIndex].Cells[3].FindControl("lblDescription") as Label).Text;
                var returnButtonData = (BatchedItemsGV.Rows[RowIndex].Cells[8].FindControl("removeItem") as Button).Text;
                if (descriptionText.Contains("Overall Project Cost($)"))
                {
                    isOverallProject = true;
                }

                if (!returnButtonData.Equals("Return"))
                {
                    isReturn = true;
                }

                MarkEditReturnItem(batchId, prodId, qty, isOverallProject, isReturn);
                DisplayBatchedItems(ddlst_ReferralDate.SelectedItem.Value);

                //int index = Convert.ToInt32(e.CommandArgument);
                //GridViewRow row = Gridview1.Rows[index];
                // Do what ever you want....
            }
        }

        public void MarkEditReturnItem(string batchId, string prodId, string qty, bool isOverallProject, bool isReturn)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                int retval;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("MarkEditReturnItem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@batchId", batchId);
                cmd.Parameters.AddWithValue("@prodId", prodId);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@isOverallProject", isOverallProject);
                cmd.Parameters.AddWithValue("@isReturn", isReturn);

                var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                output.Direction = ParameterDirection.Output;

                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                retval = Convert.ToInt32(returnParameter.Value);
                con.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void ChangeDeliverBtnState(int rownum, int qty)
        {
            var btn = BatchedItemsGV.Rows[rownum].Cells[7].FindControl("deliverItem") as Button;
            btn.Text = qty.ToString()+ " Delivered";
            btn.CssClass="btn btn-sm btn-success p-2";
            btn.Enabled = false;

            var ret = BatchedItemsGV.Rows[rownum].Cells[8].FindControl("removeItem") as Button;
            ret.Visible=false;

            //Edit button visibility 
            var returnEdit = BatchedItemsGV.Rows[rownum].Cells[10].FindControl("returnEdit") as ImageButton;
            returnEdit.Visible = true;
        }

        private void ChangeReturnBtnState(int rownum, int qty)
        {
            var btn = BatchedItemsGV.Rows[rownum].Cells[8].FindControl("removeItem") as Button;
            btn.Text = qty.ToString() + " Returned";
            btn.Enabled = false;
            btn.Visible = true;

            var delbtn = BatchedItemsGV.Rows[rownum].Cells[7].FindControl("deliverItem") as Button;
            if (delbtn.Enabled)
                delbtn.Visible = false;

            //Edit button visibility 
            var returnEdit = BatchedItemsGV.Rows[rownum].Cells[10].FindControl("returnEdit") as ImageButton;
            returnEdit.Visible = true;

        }

        private void MarkProductDelivered(string batchId, string prodId, int qty, int status)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();

            if (prodId == "OverallProject")
            {
                cmd = new SqlCommand("[USP_UpdateOPStatusInfo]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@Status", status);
                //cmd.Parameters.AddWithValue("@InputNetId", Request.Cookies["ATUId"].Value); Uncomment when adding to prod code
                cmd.Parameters.AddWithValue("@InputNetId", "srao53");
            }
            else
            {
                cmd = new SqlCommand("[USP_AddProductDeliveryInfo]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@ProductId", prodId);
                cmd.Parameters.AddWithValue("@Status", status);
                //cmd.Parameters.AddWithValue("@InputNetId", Request.Cookies["ATUId"].Value);
                cmd.Parameters.AddWithValue("@InputNetId", "srao53");
                cmd.Parameters.AddWithValue("@Quantity", qty);
            }

            if (con.State == ConnectionState.Open)
                con.Close();

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private void MarkBatchDelete(string batchId, string prodId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();

            if (prodId == "OverallProject")
            {
                cmd = new SqlCommand("DELETE FROM EquipmentBatchedOverallProject WHERE OverallCostID = @BatchId", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchId", Convert.ToInt32(batchId));
            }
            else
            {
                //cmd = new SqlCommand("[USP_AddProductDeliveryInfo]", con);
                cmd = new SqlCommand("DELETE FROM EquipmentBatchedProduct WHERE EquipmentBatchId = @BatchId and EquipmentProductId = @ProductId", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchId", Convert.ToInt32(batchId));
                cmd.Parameters.AddWithValue("@ProductId", prodId);
            }

            //if (con.State == ConnectionState.Open)
            //    con.Close();

            //con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateQuantity(string batchId, string prodId, string qty)
        {
            if (prodId != "OverallProject")
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("UPDATE [dbo].[EquipmentProduct] SET [Quantity] = [Quantity] + @Quantity WHERE [EquipmentProductId] = @ProductId", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Quantity", qty);
                cmd.Parameters.AddWithValue("@ProductId", prodId);
                cmd.ExecuteNonQuery();
                con.Close();
            }

        }

        private void ValidateMinimumQuantity(string referralId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetBatchedProductsRemainingQtyInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReferralId", referralId);

            //----------------------------------------------
            var temp = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(temp);

            var alertTable = new DataTable();
            alertTable = temp.Clone();

            if (temp.Rows.Count > 0)
            {
                foreach (DataRow row in temp.Rows)
                {
                    if (Convert.ToInt32(row["MinQuantity"]) >= Convert.ToInt32(row["Quantity"]))
                    {
                        alertTable.Rows.Add(row.ItemArray);
                    }
                }

                if (alertTable.Rows.Count > 0)
                {
                    warningMsg.InnerHtml = "One or more items are running low on remaning quantity.";
                    warningMsg.Visible = true;
                    SendRemaningQtyLowAlert(alertTable);
                }
            }
        }

        private void SendRemaningQtyLowAlert(DataTable alertTable)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetEquipmentManagementEmailDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
           
            //----------------------------------------------
            var emailDetails = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(emailDetails);

            if (emailDetails.Rows.Count > 0)
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = Convert.ToString(emailDetails.Rows[0]["smtpClient"]);
                MailMessage mail = new MailMessage();

                //////Setting From , To and CC
                String fromEmail = Request.Cookies["ATUId"].Value + "@uic.edu";
                mail.From = new MailAddress(fromEmail, Convert.ToString(emailDetails.Rows[0]["response"]));
                mail.To.Add(new MailAddress(Convert.ToString(emailDetails.Rows[0]["toEmail"])));

                if (Convert.ToString(emailDetails.Rows[0]["ccEmail"]) != String.Empty)
                    mail.CC.Add(new MailAddress(Convert.ToString(emailDetails.Rows[0]["ccEmail"])));

                mail.Subject = Convert.ToString(emailDetails.Rows[0]["emailSubject"]);
                mail.Body = "<p>Item(s) low on quantity:</p><hr/>";
                mail.IsBodyHtml = true;

                try
                {
                    foreach(DataRow dr in alertTable.Rows)
                    {
                        
                        mail.Body = mail.Body + "<p>Product name: " + dr["Description"] + "</p>" +
                                                "<p>Min Qty: " + dr["MinQuantity"] + "</p>" +
                                                "<p>Remaining Qty: " + dr["Quantity"] + "</p>"+
                                                "<hr/><br/>";   
                    }

                    smtpClient.Send(mail);

                    mail.Body = string.Empty;
                }
                catch (Exception ex)
                {
                    errorMsg.InnerHtml = "Failed sending out quantity alerts to admins.";
                    errorMsg.Visible = true;
                }

            }

        }

        private void ResetControls()
        {
            errorMsg.Visible = false;
            warningMsg.Visible = false;

            BatchedItemsGV.Visible = false;
            genReportBtn.Visible = false;
            BatchedItemsGV.Columns[0].Visible = false;
            //genReportBtn.CssClass = "btn btn-sm btn-info mb-5 mt-3 pull-right";
            //genReportBtn.Text = "Make selections for report";

            BatchGridView.Visible = false;
            itemsHeader.Style.Add("display", "none");
            prevBatch.Style.Add("display", "none");
            saveItems.Visible = false;
            genReportClicked = false;
            ViewState["genReportClicked"] = genReportClicked;
            selectedRowsForReport.Clear();

            tier1Id = tier2Id = equipId = null;
            AtAreaDd.Items.Clear();
            Tier1Dd.Items.Clear();
            Tier2Dd.Items.Clear();
            EquipDd.Items.Clear();
            productDd.Items.Clear();

            originalQtyMaster.Clear();

        }

        protected void BatchGridView_pageChanging(object sender, GridViewPageEventArgs e)
        {
            BatchGridView.PageIndex = e.NewPageIndex;
            BatchGridView.DataSource = productInfo;
            BatchGridView.DataBind();

        }

        protected void BatchedItemsGV_pageChanging(object sender, GridViewPageEventArgs e)
        {

            batchedItems = (DataTable)ViewState["batchedItems"];

            if(genReportClicked)
                GetCheckedRowsForReport();

            BatchedItemsGV.PageIndex = e.NewPageIndex;
            BatchedItemsGV.DataSource = batchedItems;
            BatchedItemsGV.DataBind();

            UpdateBatchedItemsGridView(e.NewPageIndex);
        }

        private void GetCheckedRowsForReport()
        {
            List<int> rowNums = new List<int>();
            List<string> selectedProducts = new List<string>();

            if(ViewState["selectedRowsForReport"]!=null)
                selectedRowsForReport = (Dictionary<int,List<int>>)ViewState["selectedRowsForReport"];

            foreach (GridViewRow row in BatchedItemsGV.Rows)
            {
                CheckBox chkRow = (row.Cells[0].FindControl("chkItem") as CheckBox);

                if (chkRow.Checked)
                {
                    rowNums.Add(row.RowIndex);
                    selectedProducts.Add((row.Cells[1].FindControl("lblBatchId") as Label).Text + "-" + (row.Cells[2].FindControl("lblProductId") as Label).Text);
                }
            }
            if (!selectedRowsForReport.Keys.Contains(BatchedItemsGV.PageIndex))
            {
                selectedRowsForReport.Add(BatchedItemsGV.PageIndex, rowNums);
                selectedProductCodesForReport.Add(BatchedItemsGV.PageIndex, selectedProducts);
            }
            else
            {
                selectedRowsForReport[BatchedItemsGV.PageIndex] = rowNums;
                selectedProductCodesForReport[BatchedItemsGV.PageIndex] = selectedProducts;
            }

            ViewState["selectedRowsForReport"] = selectedRowsForReport;
            ViewState["selectedProductCodesForReport"] = selectedProductCodesForReport;
        }



        public void genImplementationReport(object sender, EventArgs e)
        {
            batchedItems = (DataTable)ViewState["batchedItems"];

            errorMsg.Visible = false;

            //Edit by Akshat (09/30/2020 3:36pm)
            //DataRow[] result = batchedItems.Select("ActionDate >= '" + DateTime.Today + "' AND InputBy = '" + Request.Cookies["ATUId"].Value.ToString() + "'");
            //DataRow[] result = batchedItems.Select("InputBy = '" + Request.Cookies["ATUId"].Value.ToString() + "'");

            //if (result.Length>=1)
            //{
                String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fImplementation&rs:Command=Render&";
                navigateUrl += "ReferralId=" + ddlst_ReferralDate.SelectedItem.Value.ToString() + "&netid=" + Request.Cookies["ATUId"].Value.ToString();
                navigateUrl += "&rs:Format=EXCEL&rc:Parameters=false&rs:ClearSession=True";
                string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                Response.Write(redirect);
                
            //}
            //else
            //{
            //    errorMsg.InnerHtml = "You do not have any delivered/returned items to report today.";
            //    errorMsg.Visible = true;
            //}
        }

        //The below method handles the selection of records in the batched items table and
        //generates a customized report for the same. This is currently unused.

        public void genReportClick(object sender, EventArgs e)
        {
            errorMsg.Visible = false;

            genReportClicked = ViewState["genReportClicked"]!=null?(bool)ViewState["genReportClicked"]:false;

            if (!genReportClicked)
            {
                BatchedItemsGV.Columns[0].Visible = true;
                genReportBtn.CssClass = "btn btn-sm btn-success mb-5 mt-3 pull-right";
                genReportBtn.Text = "Generate Report";
                genReportClicked = true;
                ViewState["genReportClicked"] = genReportClicked;
            }
            else
            {
                genReportClicked = false;
                ViewState["genReportClicked"] = genReportClicked;

                GetCheckedRowsForReport();

                List<string> selectedProducts = new List<string>();

                if (ViewState["selectedProductCodesForReport"] != null)
                    selectedProductCodesForReport = (Dictionary<int, List<string>>)ViewState["selectedProductCodesForReport"];

                foreach (KeyValuePair<int, List<string>> keyVal in selectedProductCodesForReport)
                    selectedProducts.AddRange(keyVal.Value);

                if (selectedProducts.Count > 0)
                {
                    //SqlConnection con = null;
                    //SqlCommand cmd = null;

                    //con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    //con.Open();
                    //cmd = new SqlCommand("CreateImplmentationData", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@ReferralId", ddlst_ReferralDate.SelectedItem.Value);
                    //cmd.Parameters.AddWithValue("@Products", string.Join(",", selectedProducts.ToArray()) + ",");
                    
                    //if (con.State == ConnectionState.Open)
                    //    con.Close();

                    //con.Open();
                    //cmd.ExecuteNonQuery();

                    //con.Close();

                }
                else
                {
                    errorMsg.InnerHtml = "Please select at least one record to generate the report.";
                    errorMsg.Visible = true;
                }

            }

        }

    }
}

