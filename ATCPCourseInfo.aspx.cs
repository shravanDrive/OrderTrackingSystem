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
    public partial class ATCPCourseInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
                    Response.Expires = -1;
                    Response.CacheControl = "No-cache";

                    var CourseNumber = HttpContext.Current.Request.QueryString["CourseNumber"];
                    var Term = HttpContext.Current.Request.QueryString["Term"];
                    LoadFacultyDropDown();
                    LoadCreditlistDropDown();
                    LoadConcentrationDetails();
                    LoadAllCourseDetailsInfo(CourseNumber, Term);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void PopulateData(CourseInfoDetails UIData)
        {
            //searchTextBox.Text = UIData.CourseNumber;
            CourseName.Text = UIData.CourseName;
            CourseNumberTextBox.Text = UIData.CourseNumber;
            courseDescription.Text = UIData.CourseDescription;
            CreditList.SelectedValue = UIData.CourseCredits;
            CourseObjective.Text = UIData.CourseObjective;
            isFall.Checked = UIData.isFall;
            isSpring.Checked = UIData.isSpring;
            isSummer.Checked = UIData.isSummer;
            majorAssignments.Text = UIData.MajorAssignments;
            primaryInstructorDropDown.SelectedValue = UIData.PrimaryInstructor1;
            //CourseAreaOfConcentration.Text = UIData.AreaOfConcentration;
            assessmentCourseID.Checked = UIData.isAssessmentCourse;
            secondaryInstructorID1.SelectedValue = UIData.SecondaryInstructor1;
            secondaryInstructorID2.SelectedValue = UIData.SecondaryInstructor2;
            secondaryInstructorID3.SelectedValue = UIData.SecondaryInstructor3;
            GuestLecture.Text = UIData.GuestLecturer;
            CourseAreaOfConcentration.SelectedValue = UIData.AreaOfConcentration;
        }

        public void LoadAllCourseDetailsInfo(string CourseNumber, string Term)
        {
            //GetAllCourseDetailsInfo
            try
            {
                if((!string.IsNullOrEmpty(CourseNumber)) && (!string.IsNullOrEmpty(Term)))
                {
                    DataTable tempTable = SPCallToGetData(CourseNumber, Term);
                    if (tempTable !=null)
                    {
                        CourseInfoDetails UIData = StructureCourseData(tempTable);
                        if(UIData != null)
                        {
                            PopulateData(UIData);
                        } 
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public CourseInfoDetails StructureCourseData(DataTable tempTable)
        {
            try
            {
                List<CourseInfoDetails> courseList = new List<CourseInfoDetails>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    CourseInfoDetails courseInfo = new CourseInfoDetails();
                    courseInfo.CourseName = tempTable.Rows[i]["CourseName"].ToString();
                    courseInfo.CourseNumber = tempTable.Rows[i]["CourseNumber"].ToString();
                    courseInfo.CourseDescription = tempTable.Rows[i]["CourseDescription"].ToString();
                    courseInfo.CourseCredits = tempTable.Rows[i]["CourseCredits"].ToString();
                    string TermsOffered = tempTable.Rows[i]["TermsOffered"].ToString();
                    switch (TermsOffered)
                    {
                        case "Spring":
                            courseInfo.isSpring = true;
                            break;
                        case "Fall":
                            courseInfo.isFall = true;
                            break;
                        case "Summer":
                            courseInfo.isSummer = true;
                            break;
                    }
                    courseInfo.CourseObjective = tempTable.Rows[i]["CourseObjective"].ToString();
                    courseInfo.MajorAssignments = tempTable.Rows[i]["MajorAssignments"].ToString();
                    courseInfo.PrimaryInstructor1 = tempTable.Rows[i]["PrimaryInstructorID"].ToString();
                    courseInfo.SecondaryInstructor1 = tempTable.Rows[i]["SecondaryInstructor_OneID"].ToString();
                    courseInfo.SecondaryInstructor2 = tempTable.Rows[i]["SecondaryInstructor_TwoID"].ToString();
                    courseInfo.SecondaryInstructor3 = tempTable.Rows[i]["SecondaryInstructor_ThreeID"].ToString();
                    courseInfo.GuestLecturer = tempTable.Rows[i]["Guest_LectureID"].ToString();
                    courseInfo.AreaOfConcentration = tempTable.Rows[i]["CourseAreaOfConcentration"].ToString();

                    string AssessmentValue = tempTable.Rows[i]["AssessmentCourse"].ToString();
                    switch (AssessmentValue)
                    {
                        case "True":
                            courseInfo.isAssessmentCourse = true;
                            break;
                        case "False":
                            courseInfo.isAssessmentCourse = false;
                            break;
                    }
                    courseList.Add(courseInfo);
                }

                //CourseInfoDetails ReturncourseInfo = new CourseInfoDetails();

                //if (courseList.Count > 1)
                //{
                //    CourseInfoDetails courseInfoBaseEntry = courseList.FirstOrDefault();
                //    foreach(var item in courseList)
                //    {
                //        if (item.isFall)
                //        {
                //            courseInfoBaseEntry.isFall = true;
                //        }
                //        else if (item.isSpring)
                //        {
                //            courseInfoBaseEntry.isSpring = true;
                //        }
                //        else if (item.isSummer)
                //        {
                //            courseInfoBaseEntry.isSummer = true;
                //        }
                //    }
                //    return courseInfoBaseEntry;
                //}
                //else
                //{
                    return courseList.FirstOrDefault();
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public DataTable SPCallToGetData(string CourseNumber, string Term)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetSingleCourseInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseNumber", CourseNumber);
                cmd.Parameters.AddWithValue("@Term", Term);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();
                return tempTable;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void LoadConcentrationDetails()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCPGetConcentrationInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                // PRIMARY INSTRUCTOR 
                //genderDropDown.Items.Clear();
                CourseAreaOfConcentration.DataSource = tab;
                CourseAreaOfConcentration.DataTextField = "ConcentrationAreaName";
                CourseAreaOfConcentration.DataValueField = "ConcentrationAreaID";
                CourseAreaOfConcentration.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        public void LoadFacultyDropDown()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCPGetFacultyInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                // PRIMARY INSTRUCTOR 
                //genderDropDown.Items.Clear();
                primaryInstructorDropDown.DataSource = tab;
                primaryInstructorDropDown.DataTextField = "Faculty_Name";
                primaryInstructorDropDown.DataValueField = "FacultyID";
                primaryInstructorDropDown.DataBind();

                // SECONDARY INSTRUCTOR 1
                secondaryInstructorID1.DataSource = tab;
                secondaryInstructorID1.DataTextField = "Faculty_Name";
                secondaryInstructorID1.DataValueField = "FacultyID";
                secondaryInstructorID1.DataBind();


                // SECONDARY INSTRUCTOR 2
                secondaryInstructorID2.DataSource = tab;
                secondaryInstructorID2.DataTextField = "Faculty_Name";
                secondaryInstructorID2.DataValueField = "FacultyID";
                secondaryInstructorID2.DataBind();

                // SECONDARY INSTRUCTOR 3
                secondaryInstructorID3.DataSource = tab;
                secondaryInstructorID3.DataTextField = "Faculty_Name";
                secondaryInstructorID3.DataValueField = "FacultyID";
                secondaryInstructorID3.DataBind();

                //genderDropDown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
                //genderDropDown.Style.Add("display", "table-row");
            }
            catch (Exception ex)
            {

            }
        }

        public void LoadCreditlistDropDown()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetCreditListInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                // PRIMARY INSTRUCTOR 
                //genderDropDown.Items.Clear();
                CreditList.DataSource = tab;
                CreditList.DataTextField = "CreditListNumber";
                CreditList.DataValueField = "CreditListID";
                CreditList.DataBind();
            }
            catch (Exception ex)
            {

            }
        }
        protected void primaryInstructorDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void secondaryInstructorID1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void secondaryInstructorID2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void secondaryInstructorID3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CreditList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void FallCheckBoxID_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void SpringCheckBoxID_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void SummerCheckBoxID_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void CourseAreaOfConcentration_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                int retval;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCPUpdateCourseInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseName", CourseName.Text);
                cmd.Parameters.AddWithValue("@CourseNumberTextBox", CourseNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@courseDescription", courseDescription.Text);
                cmd.Parameters.AddWithValue("@CreditList", CreditList.SelectedValue);
                cmd.Parameters.AddWithValue("@CourseObjective", CourseObjective.Text);

                if (isFall.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term","Fall");
                }
                else if (isSpring.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term", "Spring");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Term", "Summer");
                }

                cmd.Parameters.AddWithValue("@majorAssignments", majorAssignments.Text);
                cmd.Parameters.AddWithValue("@primaryInstructorDropDown", primaryInstructorDropDown.SelectedValue);
                cmd.Parameters.AddWithValue("@assessmentCourseID", assessmentCourseID.Checked);
                cmd.Parameters.AddWithValue("@secondaryInstructorID1", secondaryInstructorID1.SelectedValue);
                cmd.Parameters.AddWithValue("@secondaryInstructorID2", secondaryInstructorID2.SelectedValue);
                cmd.Parameters.AddWithValue("@secondaryInstructorID3", secondaryInstructorID3.SelectedValue);
                cmd.Parameters.AddWithValue("@GuestLecture", GuestLecture.Text);
                cmd.Parameters.AddWithValue("@CourseAreaOfConcentration", CourseAreaOfConcentration.SelectedValue);

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
                lblSuccessMessage.Text = "Data Updated Successfully";
                lblErrorMessage.Text = string.Empty;

            }
            catch(Exception ex)
            {
                lblSuccessMessage.Text = string.Empty;
                lblErrorMessage.Text = ex.Message;
            }
        }
    }
}
