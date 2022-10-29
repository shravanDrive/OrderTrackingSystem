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
    public partial class FollowUp : System.Web.UI.Page
    {
        private DataTable FollowUpData = new DataTable();
        private DataTable StatusDropDown = new DataTable();
        public static string FormattedDate { get; set; }
        public static int Counter { get; set; }

        public string DateSet = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Counter = 0;
                //FollowUpGridLoad();

            }
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //StatusDropDown = new DataTable();
                //StatusDropDown.Columns.Add("StatusListID", typeof(string));
                //StatusDropDown.Columns.Add("StatusListValue", typeof(string));
                //StatusDropDown.Rows.Add("1", "1-Week");
                //StatusDropDown.Rows.Add("2", "1-Month");
                //StatusDropDown.Rows.Add("3", "3-Month");

                //Find the DropDownList in the Row


                DropDownList statusList = (e.Row.FindControl("ListID") as DropDownList);
                statusList.DataSource = LoadFollowUpStatus();


                //statusList.DataSource = GetData("SELECT DISTINCT Country FROM Customers");
                statusList.DataTextField = "StatusCode";
                statusList.DataValueField = "StatusID";
                statusList.DataBind();
                statusList.Items.Insert(0, new ListItem("--Choose one--", "0"));

                string selectedCity = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                statusList.Items.FindByValue(selectedCity).Selected = true;

                DropDownList CommentList = (e.Row.FindControl("CommentID") as DropDownList);
                CommentList.DataSource = LoadCommentDropdown();


                //statusList.DataSource = GetData("SELECT DISTINCT Country FROM Customers");
                CommentList.DataTextField = "CommentName";
                CommentList.DataValueField = "CommentId";
                CommentList.DataBind();
                CommentList.Items.Insert(0, new ListItem("--Choose one--", "0"));

                string commentVal = DataBinder.Eval(e.Row.DataItem, "CommentVal").ToString();
                CommentList.Items.FindByValue(commentVal).Selected = true;

                //Add Default Item in the DropDownList
                //statusList.Items.Insert(0, new ListItem("Please select"));

                //Select the Country of Customer in DropDownList
                //string country = (e.Row.FindControl("lblCountry") as Label).Text;
                //ddlCountries.Items.FindByValue(country).Selected = true;
            }
        }

        public string GetNewDate()
        {
            if (Counter == 0)
            {
                Counter++;
                return FormattedDate;
            }
            else
            {
                FormattedDate = Request.Form["dueDate"];
                return FormattedDate;
            }
        }

        public DataTable LoadCommentDropdown()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("ATCPFollowUpCommentInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);
            con.Close();
            return tab;
            //genderDropDown.Items.Clear();
            //CommentID.DataSource = tab;
            //CommentID.DataTextField = "CommentName";
            //CommentID.DataValueField = "CommentId";
            //CommentID.DataBind();

            //genderDropDown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            // genderDropDown.Style.Add("display", "table-row");

        }

        public DataTable LoadFollowUpStatus()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("ATCPFollowUpStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);
            con.Close();
            return tab;
            //genderDropDown.Items.Clear();
            //CommentID.DataSource = tab;
            //CommentID.DataTextField = "CommentName";
            //CommentID.DataValueField = "CommentId";
            //CommentID.DataBind();

            //genderDropDown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            // genderDropDown.Style.Add("display", "table-row");

        }

        protected void FollowUpGrid_DataBound(object sender, EventArgs e)
        {

            FollowUpData = new DataTable();
            FollowUpData.Columns.Add("LastName", typeof(string));
            FollowUpData.Columns.Add("FirstName", typeof(string));
            FollowUpData.Columns.Add("APC", typeof(string));
            FollowUpData.Columns.Add("1Week", typeof(string));
            FollowUpData.Columns.Add("1Month", typeof(string));
            FollowUpData.Columns.Add("3Month", typeof(string));
            FollowUpData.Columns.Add("Comment", typeof(string));

            FollowUpData.Rows.Add("Smith","John","01/15/2022","01/22/2022","02/15/2022","04/15/2022","To be filled by Caller");

            FollowUpGrid.DataSource = FollowUpData;
            FollowUpGrid.DataBind();
            FollowUpGrid.Visible = true;

        }

        protected void FollowUpGridLoad()
        {
            FollowUpData = new DataTable();
            FollowUpData.Columns.Add("LastName", typeof(string));
            FollowUpData.Columns.Add("FirstName", typeof(string));
            FollowUpData.Columns.Add("APC", typeof(string));
            FollowUpData.Columns.Add("1Week", typeof(string));
            FollowUpData.Columns.Add("1Month", typeof(string));
            FollowUpData.Columns.Add("3Month", typeof(string));
            FollowUpData.Columns.Add("Comment", typeof(string));

            FollowUpData.Rows.Add("Smith", "John", "01/15/2022", "01/22/2022", "02/15/2022", "04/15/2022", "TextBox: To be filled by Caller");

            FollowUpGrid.DataSource = FollowUpData;
            FollowUpGrid.DataBind();
            FollowUpGrid.Visible = true;
        }

        public void PopulateGridView(string datePeriod)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("FollowUpReportClients", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InputDate", datePeriod);

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if (tempTable.Rows.Count > 0)
                {
                    FollowUpGrid.DataSource = tempTable;
                    FollowUpGrid.DataBind();
                    Session["FollowUpGridDataTable"] = tempTable;
                    //GridView1.Rows[tempTable.Rows.Count].Cells.Clear();
                }
                else
                {
                    tempTable.Rows.Add(tempTable.NewRow());
                    FollowUpGrid.DataSource = tempTable;
                    FollowUpGrid.DataBind();
                    FollowUpGrid.Rows[0].Cells.Clear();
                    FollowUpGrid.Rows[0].Cells.Add(new TableCell());
                    FollowUpGrid.Rows[0].Cells[0].ColumnSpan = tempTable.Columns.Count;
                    FollowUpGrid.Rows[0].Cells[0].Text = "No Data Found";
                    FollowUpGrid.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }
                //GridView1.Rows[tempTable.Rows.Count].Cells.Clear();
                //removeEmptyRows() RemoveEmptyRows()
                //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "RemoveEmptyRows()", true);                
            }
            catch (Exception ex)
            {

            }
        }

        protected void StatusList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void facultyAdvisor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void FollowUpGrid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Search_Click(object sender, EventArgs e)
        {
            string dateIntent = Request.Form["dueDate"];
            //string convertedDate = null;
            //if (!string.IsNullOrEmpty(dateIntent))
            //{
            //    convertedDate = DateTime.ParseExact(dateIntent, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            //        .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            //}
            DateSet = dateIntent;
            PopulateGridView(DateSet);
        }

        protected void FollowUpGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (Session != null && Session["FollowUpGridDataTable"] != null)
            {
                FollowUpGrid.DataSource = Session["FollowUpGridDataTable"];
                FollowUpGrid.PageIndex = e.NewPageIndex;
                FollowUpGrid.DataBind();
            }
        }

        protected void FollowUpGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Save")
            {
                GridViewRow gvr = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                int RowIndex = gvr.RowIndex;

                var ClientId = (FollowUpGrid.Rows[RowIndex].Cells[1].FindControl("lblClientId") as Label).Text;
                var StatusValue = (FollowUpGrid.Rows[RowIndex].Cells[1].FindControl("ListID") as DropDownList).Text;
                var APC = (FollowUpGrid.Rows[RowIndex].Cells[1].FindControl("lblAPC") as Label).Text;
                var comment = (FollowUpGrid.Rows[RowIndex].Cells[1].FindControl("CommentID") as DropDownList).Text;

                UpdateValuesinDB(ClientId,StatusValue,APC,comment);

                //var CommentsValue1 = (FollowUpGrid.Rows[RowIndex].Cells[1].FindControl("lstBoxTest") as CheckBoxList).Text;
                //var ClientId = (FollowUpGrid.Rows[e.RowIndex].Cells[1].FindControl("lblClientId") as Label).Text;
            }
        }

        public void UpdateValuesinDB(string ClientId, string StatusValue, string APC, string comment)
        {
            try
            {
                bool success = false;
                SqlConnection con = null;
                SqlCommand cmd = null;

                int retval;
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("FollowUpReportAddData", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@clientID", ClientId);
                cmd.Parameters.AddWithValue("@StatusValue", StatusValue);
                cmd.Parameters.AddWithValue("@CommentValue", comment);
                cmd.Parameters.AddWithValue("@APC", APC);


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
                PopulateGridView(DateSet);

                //ShowMessageOperation("Record submitted successfully", MessageType.Success);

            }
            catch (Exception ex)
            {

            }
        }

        protected void CommentID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ListID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
