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
    public partial class ATCPAddCourse : System.Web.UI.Page
    {
        public enum MessageType { Success, Error, Info, Warning };
        public bool checkBoxValid = false;
        public bool primaryInstructorValid = false;
        public bool courseNumberValid = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    var CourseNumber = HttpContext.Current.Request.QueryString["CourseNumber"];
                    var Term = HttpContext.Current.Request.QueryString["Term"];
                    LoadFacultyDropDown();
                    LoadCreditlistDropDown();
                    LoadConcentrationDetails();
                    //LoadAllCourseDetailsInfo(CourseNumber, Term);
                }
            }
            catch (Exception ex)
            {

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
            catch (Exception ex)
            {
                return null;
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                if (isFall.Checked || isSpring.Checked || isSummer.Checked)
                {

                }
                else
                {
                    ChecBoxValidation.Text = "Required Field";
                }
                //SqlConnection con = null;
                //SqlCommand cmd = null;
                //int retval;

                //con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                //con.Open();
                //cmd = new SqlCommand("ATCPAddCourseInfo", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CourseName", CourseName.Text);
                //cmd.Parameters.AddWithValue("@CourseNumberTextBox", CourseNumberTextBox.Text);
                //cmd.Parameters.AddWithValue("@courseDescription", courseDescription.Text);
                //cmd.Parameters.AddWithValue("@CreditList", CreditList.SelectedValue);
                //cmd.Parameters.AddWithValue("@CourseObjective", CourseObjective.Text);

                //if (isFall.Checked)
                //{
                //    cmd.Parameters.AddWithValue("@Term", "Fall");
                //}
                //else if (isSpring.Checked)
                //{
                //    cmd.Parameters.AddWithValue("@Term", "Spring");
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@Term", "Summer");
                //}

                //cmd.Parameters.AddWithValue("@majorAssignments", majorAssignments.Text);
                //cmd.Parameters.AddWithValue("@primaryInstructorDropDown", primaryInstructorDropDown.SelectedValue);
                //cmd.Parameters.AddWithValue("@assessmentCourseID", assessmentCourseID.Checked);
                //cmd.Parameters.AddWithValue("@secondaryInstructorID1", secondaryInstructorID1.SelectedValue);
                //cmd.Parameters.AddWithValue("@secondaryInstructorID2", secondaryInstructorID2.SelectedValue);
                //cmd.Parameters.AddWithValue("@secondaryInstructorID3", secondaryInstructorID3.SelectedValue);
                //cmd.Parameters.AddWithValue("@GuestLecture", GuestLecture.Text);
                //cmd.Parameters.AddWithValue("@CourseAreaOfConcentration", CourseAreaOfConcentration.SelectedValue);

                ////----------------------------------------------
                //var returnParameter = cmd.Parameters.Add("@Retval", SqlDbType.Int);
                //returnParameter.Direction = ParameterDirection.ReturnValue;

                //var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                //output.Direction = ParameterDirection.Output;

                //if (con.State == ConnectionState.Open)
                //    con.Close();

                //con.Open();
                //cmd.ExecuteNonQuery();
                //retval = Convert.ToInt32(returnParameter.Value);

                //con.Close();
                //lblSuccessMessage.Text = "Data Updated Successfully";
                //lblErrorMessage.Text = string.Empty;

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = string.Empty;
                lblErrorMessage.Text = ex.Message;
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
        protected void primaryInstructorDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (primaryInstructorDropDown.SelectedValue.ToString().Trim() != "18")
            {               
                primaryInstructorValidation.Text = "";
            }
            else
            {
                primaryInstructorValidation.Text = "Required Field";
            }
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

                //var primaryFacultyTab = tab.Clone();

                //foreach (DataRow row in tab.Rows)
                //{
                //    if (row["FacultyID"].ToString() != "18")
                //    {
                //        primaryFacultyTab.ImportRow(row);
                //    }
                //}

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

        protected void AddID_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (isFall.Checked || isSpring.Checked || isSummer.Checked)
                {
                    checkBoxValid = true;
                    ChecBoxValidation.Text = "";
                }
                else
                {
                    ChecBoxValidation.Text = "Required Field";           
                }

                if (primaryInstructorDropDown.SelectedValue.ToString().Trim() != "18")
                {
                    primaryInstructorValid = true;
                    primaryInstructorValidation.Text = "";
                }
                else
                {
                    primaryInstructorValid = false;
                    primaryInstructorValidation.Text = "Required Field";
                }

                if (!string.IsNullOrEmpty(CourseNumberTextBox.Text))
                {
                    courseNumberValid = true;
                    courseNumberValidatorID.Text = "";
                }
                else
                {
                    courseNumberValid = false;
                    courseNumberValidatorID.Text = "Required Field";
                }

                if (checkBoxValid && primaryInstructorValid && courseNumberValid)
                {
                    CourseNumberValidations();
                    if (courseNumberValid)
                    {
                        // DoInsert Operation
                        if (InsertRecordInDB())
                        {
                            Response.Redirect("ATCPCourseDetails.aspx?MessageType=Success", false);
                        }
                        else
                        {
                            Response.Redirect("ATCPCourseDetails.aspx?MessageType=Error", false);
                        }
                    }

                    //ShowMessage("Record submitted successfully", MessageType.Success);
                    //Response.Redirect("ATCPCourseDetails.aspx");
                }
            }
            catch(Exception ex)
            {
                Response.Redirect("ATCPCourseDetails.aspx?MessageType=Error", false);
            }
        }

        public bool CheckForDuplicates()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                int retval;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCPCheckCoursePresent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseName", CourseNumberTextBox.Text);

                if (isFall.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term", "Fall");
                }
                else if (isSpring.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term", "Spring");
                }
                else if(isSummer.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term", "Summer");
                }

                var output = cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 500);
                output.Direction = ParameterDirection.Output;

                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                if (output.Value.Equals("true"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool InsertRecordInDB()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;
                int retval;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCPAddCourseInfo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseName", CourseName.Text);
                cmd.Parameters.AddWithValue("@CourseNumberTextBox", CourseNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@courseDescription", courseDescription.Text);
                cmd.Parameters.AddWithValue("@CreditList", CreditList.SelectedValue);
                cmd.Parameters.AddWithValue("@CourseObjective", CourseObjective.Text);

                if (isFall.Checked)
                {
                    cmd.Parameters.AddWithValue("@Term", "Fall");
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
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        protected void isFall_CheckedChanged(object sender, EventArgs e)
        {
            isSpring.Checked = false;
            isSummer.Checked = false;
            if (!isFall.Checked)
            {
                ChecBoxValidation.Text = "Required Field";
            }
            else
            {
                ChecBoxValidation.Text = "";
            }
        }

        protected void isSpring_CheckedChanged(object sender, EventArgs e)
        {
            isFall.Checked = false;
            isSummer.Checked = false;
            if (!isSpring.Checked)
            {
                ChecBoxValidation.Text = "Required Field";
            }
            else
            {
                ChecBoxValidation.Text = "";
            }
        }

        protected void isSummer_CheckedChanged(object sender, EventArgs e)
        {
            isFall.Checked = false;
            isSpring.Checked = false;
            if (!isSummer.Checked)
            {
                ChecBoxValidation.Text = "Required Field";
            }
            else
            {
                ChecBoxValidation.Text = "";
            }
        }

        //protected void CourseNumberTextBox_TextChanged(object sender, EventArgs e)
        //{
        //    CourseNumberValidations();
        //}

        public void CourseNumberValidations()
        {
            if (!string.IsNullOrEmpty(CourseNumberTextBox.Text))
            {
                courseNumberValid = true;
                courseNumberValidatorID.Text = "";
                if (CheckForDuplicates())
                {
                    courseNumberValid = false;
                    courseNumberValidatorID.Text = "Record Exists in Database";
                }
            }
            else
            {
                courseNumberValid = false;
                courseNumberValidatorID.Text = "Required Field";
            }
        }
    }
}
