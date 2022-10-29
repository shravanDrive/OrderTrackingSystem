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
    public partial class Inventory : System.Web.UI.Page
    {
        private int? atAreaId, tier1Id, tier2Id, equipId;


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

        //Bind the AT Area control

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

            con.Close();

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

                Tier1.Style.Add("display", "inherit");

                Tier2Dd.Items.Clear();

                EquipDd.Items.Clear();
               
            }
            
        }

        public void Tier1_SelectionChanged(object sender, EventArgs e)
        {
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
                    Tier2.Style.Add("display", "inherit");
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
            tier2Id = Convert.ToInt32(Tier2Dd.SelectedItem.Value);
            ViewState["tier2Id"] = tier2Id;
            LoadEquipDd();
        }

        private void LoadEquipDd()
        {
            
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

            Equip.Style.Add("display", "inherit");
            
        }

        public void Equip_SelectionChanged(object sender, EventArgs e)
        {
            equipId = Convert.ToInt32(EquipDd.SelectedItem.Value);

            ViewState["equipId"] = equipId;

            if (equipId != 0)
            {
                price.Style.Add("display", "inherit");
                desc.Style.Add("display", "inherit");
                quantity.Style.Add("display", "inherit");
                alertQty.Style.Add("display", "inherit");
                bar1.Style.Add("display", "inherit");
                submit.Style.Add("display", "inherit");
                
            }

        }

       #endregion


    #region Button click events

        protected void AddBtnClick(object sender, EventArgs e)
        {
            Button add = (Button)sender;

            if (add.ID == add1.ID)
                bar2.Style.Add("display", "inherit");
            else if (add.ID == add2.ID)
                bar3.Style.Add("display", "inherit");
            else if (add.ID == add3.ID)
                bar4.Style.Add("display", "inherit");


        }

        protected void SaveBtnClick(object sender, EventArgs e)
        {
            int res;
            if (description.Text == string.Empty)
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Description cannot be empty"; 
            }
                
            else if (priceTB.Text == string.Empty)
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Price cannot be empty";
            }
            else if (quantityTB.Text == string.Empty)
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Quantity cannot be empty";
            }
            else if (alertQtyTB.Text == string.Empty)
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Alert quantity cannot be empty";
            }
            else if (!int.TryParse(quantityTB.Text,out res))
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Quantity has to be a whole number";
            }
            else if (!int.TryParse(alertQtyTB.Text, out res))
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Alert quantity has to be a whole number";
            }
            else if (Convert.ToInt32(alertQtyTB.Text)>Convert.ToInt32(quantityTB.Text))
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Alert quantity has to be less than the actual quantity";
            }
            else if (barcode1.Text == string.Empty)
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "First barcode cannot be empty";
            }
            else if (ProductExists(barcode1.Text))
            {
                errorMsg.Visible = true;
                errorMsg.InnerHtml = "Product already exists. Please go to manage section for further modifications.";
            }
            else
            {
                errorMsg.Visible = false;

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("USP_AddEquipmentProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Description", description.Text);
                cmd.Parameters.AddWithValue("@Price", priceTB.Text);
                cmd.Parameters.AddWithValue("@Quantity", quantityTB.Text);
                cmd.Parameters.AddWithValue("@AlertQuantity", alertQtyTB.Text==string.Empty? "1" : alertQtyTB.Text);
                cmd.Parameters.AddWithValue("@EquipmentId", (int)ViewState["equipId"]);
                cmd.Parameters.AddWithValue("@NetID", Request.Cookies["ATUId"].Value);

                List<string> barcodes = new List<string>();

                barcodes.Add(barcode1.Text);
                if (!(barcode2.Text == string.Empty)) barcodes.Add(barcode2.Text);
                if (!(barcode3.Text == string.Empty)) barcodes.Add(barcode3.Text);
                if (!(barcode4.Text == string.Empty)) barcodes.Add(barcode4.Text);

                cmd.Parameters.AddWithValue("@Barcodes", string.Join(",", barcodes.ToArray()) + ",");

                var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                output.Direction = ParameterDirection.Output;


                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                int retval = Convert.ToInt32(returnParameter.Value);

                con.Close();

                atAreaId = tier1Id = tier2Id = equipId  = null;

                if (retval >= 0)
                {
                    content.Style.Add("display", "none");
                    success.Style.Add("display", "block");
                    Response.AddHeader("REFRESH", "4;URL=Inventory.aspx");

                }
                else
                    content.Visible = false;
                //result.InnerHtml = output.Value.ToString(); 

            }

        }

        #endregion


    #region External Validation

        private bool ProductExists(string barcode)
        {
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

            return (tempTable.Rows.Count>0);

        }

        #endregion

    }
}
