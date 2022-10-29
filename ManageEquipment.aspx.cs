//contains a lot of repetitive code due to the lack of existing infrastructure
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
using System.Reflection;

namespace ATUClient
{
    public partial class ManageEquipment : System.Web.UI.Page
    {

        private int? atAreaId, tier1Id, tier2Id, equipId, prodId, quantityNow;
        private DataTable productInfoMaunal;
        private DataTable productManangeInfo;
        private List<string> barcodes = new List<string>();
        private DataTable OPInfo = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                BindATArea();

                //AtAreaDd.SelectedIndexChanged += new EventHandler(ATArea_SelectionChanged);
                //Next1.Click += new EventHandler(this.NextBtnClick);
                //add1.Click += new EventHandler(this.AddBtnClick);
                //add2.Click += new EventHandler(this.AddBtnClick);
                //add3.Click += new EventHandler(this.AddBtnClick);

            }

        }

        //radiobutton selection change

        public void ManageTypeCheckedChanged(Object sender, EventArgs e)
        {
            cardGridOP.Style.Add("display", "none");
            errorMsg.Visible = false;
            RadioButton rbtn = (RadioButton)sender;
            if (rbtn.ID == ManualRB.ID)
            {
                barcodeTextgroup.Style.Add("display", "none");
                scanBtn.Style.Add("display", "none");
                manualSelection.Style.Add("display", "block");
            }

            else if (rbtn.ID == ScanRB.ID)
            {
                scanBtn.Style.Add("display", "inherit");
                manualSelection.Style.Add("display", "none");
                barcodeTextgroup.Style.Add("display", "none");
            }
            else
            {
                barcodeTB.Text = string.Empty;
                scanBtn.Style.Add("display", "none");
                manualSelection.Style.Add("display", "none");
                barcodeTextgroup.Style.Add("display", "inherit");
            }

        }

        //Manual selection - Bind the parent control

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

        #region Dropdown selection changed & Bind controls

        public void ATArea_SelectionChanged(object sender, EventArgs e)
        {
            if (AtAreaDd.SelectedIndex != 0)
            {
                UnloadForATArea();
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

                con.Close();

                Tier1Dd.Items.Clear();
                Tier1Dd.DataSource = tab;
                Tier1Dd.DataTextField = "Tier1";
                Tier1Dd.DataValueField = "Tier1Id";
                Tier1Dd.DataBind();

                Tier1Dd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                Tier1.Style.Add("display", "flex");

                Tier2Dd.Items.Clear();

                EquipDd.Items.Clear();

            }

        }


        public void Tier1_SelectionChanged(object sender, EventArgs e)
        {
            if (Tier1Dd.SelectedIndex != 0)
            {
                //var data = Request.Cookies["ATUId"].Value;
                UnloadForTier1();
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

                con.Close();

                Tier2Dd.Items.Clear();
                Tier2Dd.DataSource = tab;
                Tier2Dd.DataTextField = "Tier2";
                Tier2Dd.DataValueField = "Tier2Id";
                Tier2Dd.DataBind();

                Tier2Dd.Items.Insert(0, new ListItem("--Choose one--", "0"));

                if (Tier2Dd.Items.Count > 2)
                {
                    EquipDd.Items.Clear();
                    Tier2.Style.Add("display", "flex");
                }
                else if (Tier2Dd.Items.Count == 2)
                {
                    Tier2.Style.Add("display", "none");
                    tier2Id = Convert.ToInt32(Tier2Dd.Items[1].Value);
                    ViewState["tier2Id"] = tier2Id;
                    LoadEquipDd();
                }
                else
                {
                    Tier2.Style.Add("display", "none");
                    tier2Id = null;
                    ViewState["tier2Id"] = tier2Id;
                    LoadEquipDd();
                }


            }
        }

        public void Tier2_SelectionChanged(object sender, EventArgs e)
        {
            UnloadForTier2();
            tier2Id = Convert.ToInt32(Tier2Dd.SelectedItem.Value);
            ViewState["tier2Id"] = tier2Id;
            LoadEquipDd();
        }

        private void LoadEquipDd()
        {

            UnloadForEquip();
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

            con.Close();

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
                EquipDd.Style.Add("display", "none");
                equipId = null;
                ViewState["equipId"] = equipId;
                submitOP.Style.Add("display", "flex");
                AddButton.Style.Add("display", "flex");
            }
            else
            {
                Equip.Style.Add("display", "flex");
                EquipDd.Style.Add("display", "flex");
            }
        }

        private void UnloadForTier1()
        {
            Tier2.Style.Add("display", "none");
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
            AddButton.Style.Add("display", "none");
        }

        private void UnloadForTier2()
        {
            Equip.Style.Add("display", "none");
            EquipDd.Style.Add("display", "none");
            UnloadForEquip();
        }

        private void UnloadForATArea()
        {
            Tier1.Style.Add("display", "none");
            
            UnloadForTier1();
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

            con.Close();

            ViewState["productInfoMaunal"] = productInfoMaunal;

            productDd.Items.Clear();
            productDd.DataSource = productInfoMaunal;
            productDd.DataTextField = "Description";
            productDd.DataValueField = "EquipmentProductId";
            productDd.DataBind();

            productDd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

            prod.Style.Add("display", "flex");


        }

        public void RemoveItemClick(object sender, GridViewDeleteEventArgs e)
        { }

        #endregion


            //Click to get the barcode, find the product and display accordingly
        public void HandleBarcode(object sender, EventArgs e)
        {
            errorMsg.Visible = false;
            DisplayProductPage(GetEquipmentProductManageInfo(-1, send.Value));

        }
        
        public void ManangeBtnClick(object sender, EventArgs e)
        {
            errorMsg.Visible = false;

            if (productDd.SelectedIndex == 0)
            {
                errorMsg.InnerHtml = "Please select a product to proceed.";
                errorMsg.Visible = true;
            }
            else
            {
                prodId = Convert.ToInt32(productDd.SelectedItem.Value);
                ViewState["prodId"] = prodId;
                DisplayProductPage(GetEquipmentProductManageInfo(prodId, string.Empty));

            }
        }

        public void AddOPCost(object sender, EventArgs e)
        {
            cardGridOP.Style.Add("display", "flex");
            OPInfo.Columns.Add("Overallproject", typeof(string)); // dt.Columns.Add("Id", typeof(int));
            //OPInfo.Columns["Id"].Caption = "my id";
            OPInfo.Columns.Add("Amount", typeof(string));
            OPInfo.Columns.Add("InputUser", typeof(string));
            //OPInfo.Rows.Add(GetHeaders(dt));
            OPInfo.Rows.Add("Overall Project ($)", "32.45", "srao53");
            BatchGridView.DataSource = OPInfo;
            BatchGridView.DataBind();
            BatchGridView.Visible = true;
        }

        protected void BatchGridView_pageChanging(object sender, GridViewPageEventArgs e)
        { }

        public void ManangeBarcodeBtnClick(object sender, EventArgs e)
        {
            errorMsg.Visible = false;

            if(string.IsNullOrEmpty(barcodeTB.Text))
            {
                errorMsg.InnerHtml = "Barcode cannot be empty.";
                errorMsg.Visible = true;
            }
            else
            {
              DisplayProductPage(GetEquipmentProductManageInfo(-1, barcodeTB.Text));

            }
        }

        //helper function to get the product data from db
        private DataTable GetEquipmentProductManageInfo(int? productId, string barcode)
        {

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_GetEquipmentProductManageInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProdId", productId);
            cmd.Parameters.AddWithValue("@Barcode", barcode);

            //----------------------------------------------
            productManangeInfo = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(productManangeInfo);

            con.Close();

            ViewState["productManangeInfo"] = productManangeInfo;

            if (productId == -1 && productManangeInfo.Rows.Count>0)
                prodId = Convert.ToInt32(productManangeInfo.Rows[0]["EquipmentProductId"]);

            ViewState["prodId"] = prodId;

            return productManangeInfo;

        }

        //method that displays the product info page
        private void DisplayProductPage(DataTable productInfo)
        {
            if (productInfo.Rows.Count > 0)
            {
                barcodes.Clear();

                prodName.InnerHtml = productInfo.Rows[0]["Description"].ToString();
                descriptionTB.Text = productInfo.Rows[0]["Description"].ToString();

                var cost = Convert.ToDouble(productInfo.Rows[0]["Cost"]);

                priceTB.Text = cost.ToString("F", CultureInfo.InvariantCulture);

                existingQty.Text = "Quantity present: " + productInfo.Rows[0]["Quantity"].ToString();

                quantityNow = Convert.ToInt32(productInfo.Rows[0]["Quantity"]);

                ViewState["quantityNow"] = quantityNow;

                minQtyTB.Text = productInfo.Rows[0]["MinQuantity"].ToString();

                foreach (DataRow row in productInfo.Rows)
                    barcodes.Add(row[0].ToString());

                ViewState["barcodes"] = barcodes;

                barcode1.Text = barcodes[0];

                if (barcodes.Count >= 2)
                {
                    barcode2.Text = barcodes[1];
                    bar2.Style.Add("display", "inherit");
                }

                if (barcodes.Count >= 3)
                {
                    barcode3.Text = barcodes[2];
                    bar3.Style.Add("display", "inherit");
                }

                if (barcodes.Count == 4)
                {
                    barcode4.Text = barcodes[3];
                    bar4.Style.Add("display", "inherit");
                }

                if (barcodes.Count == 1)
                    add1.Visible = true;

                else if (barcodes.Count == 2)
                    add2.Visible = true;

                else if (barcodes.Count == 3)
                    add3.Visible = true;

                back.Style.Add("display", "block");

                selectProduct.Style.Add("display", "none");

                productDetails.Style.Add("display", "flex");
            }

            else
            {
                errorMsg.InnerHtml = "Match not found. Please check the inventory for the item.";
                errorMsg.Visible = true;
            }

        }

        //to add additional quantity to the existing product

        public void AddQtyBtnClick(object sender, EventArgs e)
        {
            errorMsgDetails.Visible = false;
            int addQty = Convert.ToInt32(send.Value);

            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("USP_UpdateExistingProductQuantity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProdId", (int)ViewState["prodId"]);
            cmd.Parameters.AddWithValue("@Quantity", addQty);

            var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;

            if (con.State == ConnectionState.Open)
                con.Close();

            con.Open();
            cmd.ExecuteNonQuery();
            int retval = Convert.ToInt32(returnParameter.Value);

            con.Close();

            if (retval >= 0)
            {
                existingQty.Text = "Quantity updated to: " + retval;
                quantityNow = retval;
                ViewState["quantityNow"] = quantityNow;

            }
            else
            {
                errorMsgDetails.InnerHtml = "Could not update quantity. Please contact administrator.";
                errorMsgDetails.Visible = true;
            }

        }

        //adding a new barcode to an exisiting product

        public void AddBarcodeBtnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == add1.ID)
            {
                bar2.Style.Add("display", "inherit");
                add2.Visible = true;
                add1.Visible = false;
            }
            else if (btn.ID == add2.ID)
            {
                bar3.Style.Add("display", "inherit");
                add3.Visible = true;
                add2.Visible = false;
            }
            else if (btn.ID == add3.ID)
            {
                bar4.Style.Add("display", "inherit");
                add3.Visible = false;
            }

        }

        //save individual barcode
        public void SaveBarcodeBtnClick(object sender, EventArgs e)
        {
            errorMsgDetails.Visible = false;

            Button saveBtn = (Button)sender;
            Label success;
            string barcode;
            int pos;
            barcodes = (List<string>)ViewState["barcodes"];

            if (saveBtn.ID == save1.ID)
            {
                success = success1;
                barcode = barcode1.Text;
                pos = 0;

            }
            else if (saveBtn.ID == save2.ID)
            {
                success = success2;
                barcode = barcode2.Text;
                pos = 1;

            }
            else if (saveBtn.ID == save3.ID)
            {
                success = success3;
                barcode = barcode3.Text;
                pos = 2;

            }
            else
            {
                success = success4;
                barcode = barcode4.Text;
                pos = 3;

            }

            if (string.IsNullOrEmpty(barcode))
            {
                errorMsgDetails.InnerHtml = "Barcode cannot be empty";
                errorMsgDetails.Visible = true;
            }
            else if (pos >= barcodes.Count)
            {
                if (barcodes.Contains(barcode))
                {
                    errorMsgDetails.InnerHtml = "Barcode already present";
                    errorMsgDetails.Visible = true;
                }
                else
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();
                    cmd = new SqlCommand("USP_AddNewBarcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProdId", (int)ViewState["prodId"]);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);

                    if (con.State == ConnectionState.Open)
                        con.Close();

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    barcodes.Add(barcode);
                    success.Visible = true;
                }
            }
            else
            {

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_UpdateExistingBarcode", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProdId", (int)ViewState["prodId"]);
                cmd.Parameters.AddWithValue("@OldBarcode", barcodes[pos]);
                cmd.Parameters.AddWithValue("@NewBarcode", barcode);

                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                barcodes[pos] = barcode;
                success.Visible = true;
            }
        }

        //save the top most fields to the database and display updated info

        public void UpdateAllBtnClick(object sender, EventArgs e)
        {
            int res;
            errorMsgDetails.Visible = false;

            if (string.IsNullOrEmpty(descriptionTB.Text))
            {
                errorMsgDetails.Visible = true;
                errorMsgDetails.InnerHtml = "Description cannot be empty";
            }
            else if (priceTB.Text == string.Empty)
            {
                errorMsgDetails.Visible = true;
                errorMsgDetails.InnerHtml = "Price cannot be empty";
            }
            else if (minQtyTB.Text == string.Empty)
            {
                errorMsgDetails.Visible = true;
                errorMsgDetails.InnerHtml = "Alert quantity cannot be empty";
            }
            else if (!int.TryParse(minQtyTB.Text, out res))
            {
                errorMsgDetails.Visible = true;
                errorMsgDetails.InnerHtml = "Min quantity has to be a whole number";
            }
            else if (Convert.ToInt32(minQtyTB.Text) > (int)ViewState["quantityNow"])
            {
                errorMsgDetails.Visible = true;
                errorMsgDetails.InnerHtml = "Min quantity has to be less than the actual quantity";
            }
            else
            {
                errorMsgDetails.Visible = false;

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_UpdateExistingProductInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProdId", (int)ViewState["prodId"]);
                cmd.Parameters.AddWithValue("@MinQuantity", minQtyTB.Text == string.Empty ? "1" : minQtyTB.Text);
                cmd.Parameters.AddWithValue("@Price", priceTB.Text);
                cmd.Parameters.AddWithValue("@Description", descriptionTB.Text);

                var dt = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(dt);

                con.Close();

                DisplayUpdatedDetails(dt);

            }

        }

        private void DisplayUpdatedDetails(DataTable dt)
        {
            errorMsg.Visible = false;

            if(ViewState["barcodes"]!=null)
                barcodes = (List<string>)ViewState["barcodes"];

            if (dt.Rows.Count > 0)
            {

                productDetails.Style.Add("display", "none");
                selectProduct.Style.Add("display", "none");

                descLabel.Text = "Description: " + dt.Rows[0]["Description"].ToString();

                priceLabel.Text = "Price: " + dt.Rows[0]["Cost"].ToString().Substring(0, 4);

                qtyLabel.Text = "Quantity: " + dt.Rows[0]["Quantity"].ToString();

                minQtyLabel.Text = "Alert Quantity: " + dt.Rows[0]["MinQuantity"].ToString();


                barcode1Label.Text = "Barcode 1: " + barcodes[0];

                if (barcodes.Count >= 2)
                {
                    barcode2Label.Text = "Barcode 2: " + barcodes[1];
                    updatedBar2.Style.Add("display", "flex");
                }

                if (barcodes.Count >= 3)
                {
                    barcode3Label.Text = "Barcode 3: " + barcodes[2];
                    updatedBar3.Style.Add("display", "flex");
                }

                if (barcodes.Count == 4)
                {
                    barcode4Label.Text = "Barcode 4: " + barcodes[3];
                    updatedBar4.Style.Add("display", "flex");
                }

                okBtn.Style.Add("display", "block");

                updatedDetails.Style.Add("display", "block");

            }
            else
            {
                errorMsgDetails.InnerHtml = "Unable to update changes. Please contact administrator.";
                errorMsgDetails.Visible = true;
            }

        }

        //clear out all saved memebers when back is clicked

        public void BackBtnClick(object sender, EventArgs e)
        {
            Response.AddHeader("REFRESH", "0;URL=ManageEquipment.aspx");
            atAreaId = tier1Id = tier2Id = equipId = prodId = quantityNow = null;
            if (productInfoMaunal != null) productInfoMaunal.Clear();
            if (productManangeInfo != null) productManangeInfo.Clear();
            if (barcodes != null) barcodes.Clear();
            if (AtAreaDd.SelectedItem != null) AtAreaDd.Items.Clear();
            if (Tier1Dd.SelectedItem != null) Tier1Dd.Items.Clear();
            if (Tier2Dd.SelectedItem != null) Tier2Dd.Items.Clear();
            if (EquipDd.SelectedItem != null) EquipDd.Items.Clear();
            if (productDd.SelectedItem != null) productDd.Items.Clear();

        }

        //Reports; both use a template from the reporting services project

        public void GetHierarchy(object sender, EventArgs e)
        {
            String navigateUrl = "http://support.iidd.uic.edu/ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentMasterListReport&rs:Command=Render&";
            navigateUrl += "ListId=1";
            navigateUrl += "&rs:Format=Excel&rc:Parameters=false&rs:ClearSession=True";
            string redirect = "<script>window.open('" + navigateUrl + "');</script>";
            Response.Write(redirect);
        }

        public void GetAllProductsList(object sender, EventArgs e)
        {
            String navigateUrl = "http://support.iidd.uic.edu/ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentMasterListReport&rs:Command=Render&";
            navigateUrl += "ListId=2";
            navigateUrl += "&rs:Format=Excel&rc:Parameters=false&rs:ClearSession=True";
            string redirect = "<script>window.open('" + navigateUrl + "');</script>";
            Response.Write(redirect);
        }


        
    }
}
