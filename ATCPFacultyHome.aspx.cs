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
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web.Services;

namespace ATCPClient
{
    public partial class ATCPFacultyHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    PopulateGridView();
                }
                catch (Exception ex)
                {

                }
            }
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
        }

        public void PopulateGridView()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;                

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetFacultyData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if (tempTable.Rows.Count > 0)
                {

                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                    LblStudentName.Text = "Faculty Info";
                }
                else
                {
                    LblStudentName.Text = "Faculty Info - No Data Found (Please Enter Faculty Data)";
                    //tempTable.Rows.Add(tempTable.NewRow());
                    //GridView1.DataSource = tempTable;
                    //GridView1.DataBind();
                    //GridView1.Rows[0].Cells.Clear();
                    //GridView1.Rows[0].Cells.Add(new TableCell());
                    //GridView1.Rows[0].Cells[0].ColumnSpan = tempTable.Columns.Count;
                    //GridView1.Rows[0].Cells[0].Text = "No Data Found";
                    //GridView1.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }

            }
            catch (Exception ex)
            {

            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            PopulateGridView();
        }

        protected void GridView1_CancellingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            PopulateGridView();
        }
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                int retval;
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_UpdateFacultyData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NetID", (GridView1.Rows[e.RowIndex].FindControl("txtNetID") as Label).Text.Trim());
                cmd.Parameters.AddWithValue("@Fname", (GridView1.Rows[e.RowIndex].FindControl("txtFname") as TextBox).Text.Trim());
                cmd.Parameters.AddWithValue("@Lname", (GridView1.Rows[e.RowIndex].FindControl("txtLname") as TextBox).Text.Trim());
                cmd.Parameters.AddWithValue("@Adjunct", (GridView1.Rows[e.RowIndex].FindControl("txtAdjunct") as CheckBox).Checked.ToString());
                cmd.Parameters.AddWithValue("@UICEmail", (GridView1.Rows[e.RowIndex].FindControl("txtUICEmail") as TextBox).Text.Trim());
                cmd.Parameters.AddWithValue("@OfficePhone", (GridView1.Rows[e.RowIndex].FindControl("txtOfficePhone") as TextBox).Text.Trim());
                cmd.Parameters.AddWithValue("@WorkCellPhone", (GridView1.Rows[e.RowIndex].FindControl("txtWorkCellPhone") as TextBox).Text.Trim());

                //----------------------------------------------

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

                GridView1.EditIndex = -1;
                PopulateGridView();
                lblSuccessMessage.Text = "Selected Row Updated";
                lblErrorMessage.Text = "";
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                int retval;
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_DeleteFacultyData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NetID", (GridView1.Rows[e.RowIndex].FindControl("txtNetID") as Label).Text.Trim());

                //----------------------------------------------

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

                GridView1.EditIndex = -1;
                PopulateGridView();
                lblSuccessMessage.Text = "Selected Row Deleted";
                lblErrorMessage.Text = "";
            }
            catch(Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("AddNew"))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    int retval;
                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();
                    cmd = new SqlCommand("ATCP_AddFacultyData", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NetID", (GridView1.FooterRow.FindControl("txtNetIDFooter") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@Fname", (GridView1.FooterRow.FindControl("txtFnameFooter") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@Lname", (GridView1.FooterRow.FindControl("txtLnameFooter") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@Adjunct", (GridView1.FooterRow.FindControl("txtAdjunctFooter") as CheckBox).Checked.ToString());
                    cmd.Parameters.AddWithValue("@UICEmail", (GridView1.FooterRow.FindControl("txtUICEmailFooter") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@OfficePhone", (GridView1.FooterRow.FindControl("txtOfficePhoneFooter") as TextBox).Text.Trim());
                    cmd.Parameters.AddWithValue("@WorkCellPhone", (GridView1.FooterRow.FindControl("txtWorkCellPhoneFooter") as TextBox).Text.Trim());

                    //----------------------------------------------

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

                    //GridView1.EditIndex = -1;
                    PopulateGridView();

                    if (output.SqlValue.ToString().Equals("Record exisits in DB"))
                    {
                        lblErrorMessage.Text = "Record with the same NetID exisits in Database";
                        lblSuccessMessage.Text = "";
                    }
                    else
                    {
                        lblSuccessMessage.Text = "New Row Added";
                        lblErrorMessage.Text = "";
                    }
                    
                }
            }
            catch(Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }
    }
}