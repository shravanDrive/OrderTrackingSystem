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
    public partial class ATCPCourseDetails : System.Web.UI.Page
    {
        public enum MessageType { Success, Error, Info, Warning };
        protected void Page_Load(object sender, EventArgs e)
        {
            var messageType = Request.QueryString["MessageType"];
            if (!string.IsNullOrEmpty(messageType))
            {
                switch (messageType)
                {
                    case "Success":
                        //ShowMessage("Record submitted successfully", MessageType.Success);
                        ShowMessageOperation("Record submitted successfully", MessageType.Success);
                        break;
                    case "Error":
                        ShowMessageOperation("Record not updated", MessageType.Error);
                        //ShowMessage("Record not updated", MessageType.Error);
                        break;
                }
            }

            var DeleteOperaton = Request.QueryString["DeleteOperaton"];
            if (string.Equals(DeleteOperaton, "True"))
            {
                var courseNumber = Request.QueryString["CourseNumber"];
                var term = Request.QueryString["Term"];
                
                if (PerformDeleteOperation(courseNumber, term))
                {
                    ShowMessageOperation("Record deleted successfully", MessageType.Success);
                }
                else
                {
                    ShowMessageOperation("Record not deleted", MessageType.Error);
                }
            }
        }

        public void ShowMessageOperation(string message, MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Success:
                    ShowMessage(message, MessageType.Success);
                    break;
                case MessageType.Error:
                    ShowMessage(message, MessageType.Error);
                    break;
            }
        }

        public bool PerformDeleteOperation(string CourseNumber, String Term)
        {
            bool returnVal = false;
            try
            {
                if ((!String.IsNullOrEmpty(CourseNumber)) && (!string.IsNullOrEmpty(Term)))
                {
                    SqlConnection con = null;
                    SqlCommand cmd = null;
                    int retval;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                    con.Open();
                    cmd = new SqlCommand("ATCPDeleteCourseInfo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseNumberTextBox", CourseNumber);
                    cmd.Parameters.AddWithValue("@Term", Term);

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
                    if (output.Value.Equals("Success"))
                    {
                        returnVal = true;
                    }
                    else
                    {
                        returnVal = false;
                    }
                }
            }
            catch(Exception ex)
            {
                returnVal = false;
            }

            return returnVal;
        }

        [WebMethod]
        public static string GetAllCourses()
        {
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetAllCoursesNew", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Barcode", barcode);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<CourseListDetails> courseList = new List<CourseListDetails>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    CourseListDetails courseData = new CourseListDetails();
                    courseData.CourseNumber = tempTable.Rows[i]["CourseNumber"].ToString();
                    courseData.CourseName = tempTable.Rows[i]["CourseName"].ToString();
                    courseData.TermsOffered = tempTable.Rows[i]["TermsOffered"].ToString();
                    courseList.Add(courseData);
                }

                returnObj = JsonConvert.SerializeObject(courseList);
            }
            catch (Exception ex)
            {
                returnObj = null;
            }
            return returnObj;
        }

        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void AddID_Click(object sender, EventArgs e)
        {

        }
    }
}
