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
    public partial class ATCPStudentmarks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {

                    var UIN = Request.QueryString["UIN"];
                    searchTextBox.Text = UIN;
                    LoadSubjectGrades();
                    PopulateGridView();
                }
                catch (Exception ex)
                {

                }
            }
        }

        [WebMethod]
        public static string LoadSubjectGrades()
        {
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<CourseHistory> courseHistoryList = new List<CourseHistory>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    CourseHistory courseHistory = new CourseHistory();
                    courseHistory.Number = tempTable.Rows[i]["CourseNumber"].ToString();
                    //courseHistory.CourseName = tempTable.Rows[i]["CourseName"].ToString();
                    courseHistory.Year = tempTable.Rows[i]["CourseYear"].ToString();
                    courseHistory.Term = tempTable.Rows[i]["Term"].ToString();
                    courseHistory.Credits = tempTable.Rows[i]["Credits"].ToString();
                    courseHistory.Grade = tempTable.Rows[i]["Grade"].ToString();
                    courseHistoryList.Add(courseHistory);
                }

                ////return studentList;
                returnObj = JsonConvert.SerializeObject(courseHistoryList);
            }
            catch (Exception ex)
            {
                returnObj = null;
            }

            return returnObj;
        }

        protected void Checker(object sender, EventArgs e)
        {
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<CourseHistory> courseHistoryList = new List<CourseHistory>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    CourseHistory courseHistory = new CourseHistory();
                    courseHistory.Number = tempTable.Rows[i]["CourseNumber"].ToString();
                    //courseHistory.CourseName = tempTable.Rows[i]["CourseName"].ToString();
                    courseHistory.Year = tempTable.Rows[i]["CourseYear"].ToString();
                    courseHistory.Term = tempTable.Rows[i]["Term"].ToString();
                    courseHistory.Credits = tempTable.Rows[i]["Credits"].ToString();
                    courseHistory.Grade = tempTable.Rows[i]["Grade"].ToString();
                    courseHistoryList.Add(courseHistory);
                }

                ////return studentList;
                returnObj = JsonConvert.SerializeObject(courseHistoryList);
            }
            catch (Exception ex)
            {
                returnObj = null;
            }
        }

        public void PopulateGridView()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                LblStudentName.Text = GetStudentName(HttpContext.Current.Request.QueryString["UIN"]);

                var data = HttpContext.Current.Request.QueryString["UIN"];
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if(tempTable.Rows.Count > 0)
                {
                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                }
                else
                {
                    tempTable.Rows.Add(tempTable.NewRow());
                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                    GridView1.Rows[0].Cells.Clear();
                    GridView1.Rows[0].Cells.Add(new TableCell());
                    GridView1.Rows[0].Cells[0].ColumnSpan = tempTable.Columns.Count;
                    GridView1.Rows[0].Cells[0].Text = "No Data Found";
                    GridView1.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }

            }
            catch(Exception ex)
            {

            }
        }

        protected void UpdateEmail_Click(object sender, EventArgs e)
        {

        }

        protected void btnRun_Click(object sender, EventArgs e)
        {

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
                cmd = new SqlCommand("ATCP_UpdateCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);
                cmd.Parameters.AddWithValue("@Grade", (GridView1.Rows[e.RowIndex].FindControl("txtGrade") as TextBox).Text.Trim());
                cmd.Parameters.AddWithValue("@CourseNumber", (GridView1.Rows[e.RowIndex].FindControl("txtCourse") as Label).Text.Trim());

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
            catch(Exception ex)
            {

            }
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                
                var UIN = searchTextBox.Text;
                LblStudentName.Text = GetStudentName(UIN);
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", UIN);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if (tempTable.Rows.Count > 0)
                {
                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                }
                else
                {
                    tempTable.Rows.Add(tempTable.NewRow());
                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                    GridView1.Rows[0].Cells.Clear();
                    GridView1.Rows[0].Cells.Add(new TableCell());
                    GridView1.Rows[0].Cells[0].ColumnSpan = tempTable.Columns.Count;
                    GridView1.Rows[0].Cells[0].Text = "No Data Found";
                    GridView1.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }

            }
            catch (Exception ex)
            {

            }
        }

        protected string GetStudentName(string UIN)
        {
            string returnVal = null;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetStudentName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", UIN);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if (tempTable.Rows.Count > 0)
                {
                    returnVal = tempTable.Rows[0]["fname"].ToString() + " " + tempTable.Rows[0]["lname"].ToString();
                }
                else
                {
                }
            }
            catch(Exception ex)
            {
            }

            return returnVal;
        }
    }
}