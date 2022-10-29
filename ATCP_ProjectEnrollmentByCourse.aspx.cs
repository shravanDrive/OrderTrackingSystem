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
    public partial class ATCP_ProjectEnrollmentByCourse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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

                List<CourseDetails> courseList = new List<CourseDetails>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    CourseDetails courseData = new CourseDetails();
                    courseData.CourseNumber = tempTable.Rows[i]["CourseNumber"].ToString();
                    courseData.CourseName = tempTable.Rows[i]["CourseName"].ToString();
                    courseData.TermsOffered = tempTable.Rows[i]["TermsOffered"].ToString();
                    courseList.Add(courseData);
                }

                returnObj = JsonConvert.SerializeObject(courseList);
            }
            catch(Exception ex)
            {
                returnObj = null;
            }
            return returnObj;
        }

        [WebMethod]
        public static string GetCourseEnrollment()
        {
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                //string courseNumber

                var courseNumber = HttpContext.Current.Request.QueryString["courseNumber"];
                var term = HttpContext.Current.Request.QueryString["TermsOffered"];
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetEnrollmentCount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseNumber", courseNumber);
                cmd.Parameters.AddWithValue("@term", term);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<EnrollmentData> enrollmentList = new List<EnrollmentData>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    EnrollmentData enrollment = new EnrollmentData();

                    enrollment.Enrollment_Projection = tempTable.Rows[i]["Enrollment_Projection"].ToString();
                    enrollment.Year = tempTable.Rows[i]["CourseYear"].ToString();
                    enrollment.Term = tempTable.Rows[i]["Term"].ToString();
                    enrollmentList.Add(enrollment);
                }

                returnObj = JsonConvert.SerializeObject(enrollmentList);
            }
            catch (Exception ex)
            {
                returnObj = null;
            }
            return returnObj;
        }

        [WebMethod]
        public static string GetIndividualStudentDetails()
        {
            
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                //string courseNumber

                var courseNumber = HttpContext.Current.Request.QueryString["courseNumber"];
                var Term = HttpContext.Current.Request.QueryString["Term"];
                var Year = HttpContext.Current.Request.QueryString["Year"];

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetIndividualStudentDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseNumber", courseNumber);
                cmd.Parameters.AddWithValue("@Term", Term);
                cmd.Parameters.AddWithValue("@CourseYear", Year);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<StudentCourseView> studentViewList = new List<StudentCourseView>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    StudentCourseView studentcourseView = new StudentCourseView();

                    studentcourseView.FirstName = tempTable.Rows[i]["Fname"].ToString();
                    studentcourseView.LastName = tempTable.Rows[i]["Lname"].ToString();
                    studentcourseView.Year = tempTable.Rows[i]["CourseYear"].ToString();
                    studentcourseView.Term = tempTable.Rows[i]["Term"].ToString();
                    studentcourseView.UIN = tempTable.Rows[i]["UIN"].ToString();
                    studentViewList.Add(studentcourseView);
                }

                returnObj = JsonConvert.SerializeObject(studentViewList);
            }
            catch (Exception ex)
            {
                return null;
            }

            return returnObj;
        }


        protected void Checker(object sender, EventArgs e)
        {
        }

    }
}
