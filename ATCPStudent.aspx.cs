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
    public partial class ATCPStudent : System.Web.UI.Page
    {
        public enum MessageType { Success, Error, Info, Warning };
        public static string FormattedDate  { get; set; }

        public static int Counter { get; set; }

        public static string UIN { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    UIN = Request.QueryString["UIN"];
                    //searchTextBox.Text = UIN;
                    UINTextBox.Text = UIN;
                    Counter = 0;
                    // Loading All dropdowns
                    LoadGenderDropdown();
                    LoadRaceDropdown();
                    LoadProgramStatusDropdown();
                    LoadProfessionalCategory();
                    //LoadFacultyDropdownDetails();
                    LoadConcentrationDetails();
                    // Loading single student data using UIN
                    Get_SingleStudentData(UIN);
                    PopulateGridView();
                    projectedCourseGridView();
                }
            }
            catch(Exception ex)
            {

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

        public void projectedCourseGridView()
        {
            //ATCP_GetProjectedCourseHistoryData
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                //LblStudentName.Text = GetStudentName(HttpContext.Current.Request.QueryString["UIN"]);

                var data = HttpContext.Current.Request.QueryString["UIN"];
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetProjectedCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                if (tempTable.Rows.Count > 0)
                {
                    projectedCourse.DataSource = tempTable;
                    projectedCourse.DataBind();
                    //GridView1.Rows[tempTable.Rows.Count].Cells.Clear();
                }
                else
                {
                    tempTable.Rows.Add(tempTable.NewRow());
                    projectedCourse.DataSource = tempTable;
                    projectedCourse.DataBind();
                    projectedCourse.Rows[0].Cells.Clear();
                    projectedCourse.Rows[0].Cells.Add(new TableCell());
                    projectedCourse.Rows[0].Cells[0].ColumnSpan = tempTable.Columns.Count;
                    projectedCourse.Rows[0].Cells[0].Text = "No Data Found";
                    projectedCourse.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }
            }
            catch(Exception ex)
            {

            }
        }

        protected void ShowMessage(string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }

        public void PopulateGridView()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                //LblStudentName.Text = GetStudentName(HttpContext.Current.Request.QueryString["UIN"]);

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

                if (tempTable.Rows.Count > 0)
                {
                    GridView1.DataSource = tempTable;
                    GridView1.DataBind();
                    //GridView1.Rows[tempTable.Rows.Count].Cells.Clear();
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
                //GridView1.Rows[tempTable.Rows.Count].Cells.Clear();
                //removeEmptyRows() RemoveEmptyRows()
                //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "RemoveEmptyRows()", true);                
            }
            catch (Exception ex)
            {

            }
        }

        public void Get_SingleStudentData(string UIN)
        {
            if (!String.IsNullOrEmpty(UIN))
            {               
                try
                {
                    DataTable tempTable = SPCallToGetData(UIN);
                    SingleStudent UIData = StructureData(tempTable);
                    PopulateData(UIData);
                }
                catch (Exception ex)
                { }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //var datedata = Request.Form["dueDate"]; getting the date
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
                //ShowMessageOperation("Record updated successfully", MessageType.Success);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "RemoveEmptyRows()", true);
                //lblSuccessMessage.Text = "Selected Row Updated";
                //lblErrorMessage.Text = "";

            }
            catch (Exception ex)
            {
                //ShowMessageOperation("Record not updated", MessageType.Error);
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

        public void PopulateData(SingleStudent UIData)
        {
            // Populate data in dropdowns
            //facultyAdvisor.SelectedValue = UIData.AdvisorID;
            UINTextBox.Text = UIData.UIN;
            firstNameTextBox.Text = UIData.Fname;
            lastNameTextBox.Text = UIData.Lname;
            concentrationID.SelectedValue = UIData.ConcentrationID;
            int outputVal;
            if(int.TryParse(UIData.GenderId, out outputVal))
                genderDropDown.SelectedIndex = Convert.ToInt32(UIData.GenderId);

            if(int.TryParse(UIData.RaceId, out outputVal))
                raceDropDown.SelectedIndex = Convert.ToInt32(UIData.RaceId);

            //if (int.TryParse(UIData.ProgramStatusId, out outputVal))
            //    programStatus.SelectedIndex = Convert.ToInt32(UIData.ProgramStatusId);
            programStatus.SelectedValue = UIData.ProgramStatusId;

            isHispanic.Checked = UIData.isHispanic == "True" ? true : false;
            passedATP.Checked = UIData.Passed_ATP_Exam == "True" ? true : false;
            attemptedATP.Checked = UIData.Attempted_ATP_Exam == "True" ? true : false;

            facultyAdvisorText.Text = UIData.AdvisorName;
            entryYear.Text = UIData.Entry_Year;
            //concentrationArea.Text = UIData.Area_Of_Concentration;
            proCategoryDropdown.SelectedValue = UIData.Professional_Category;
            uicProgram.Text = UIData.UIC_Academic_Program;
            uicEmail.Text = UIData.UICEmail;
            phone.Text = UIData.Phone;
            personalEmail.Text = UIData.PEmail;
            address.Text = UIData.StreetAddress;
            state.Text = UIData.State;
            city.Text = UIData.City;
            postalCode.Text = UIData.PostalCode;
            Country.Text = UIData.Country;
            advisoryNotes.Text = UIData.Advisor_Notes;

            bInstitution.Text = UIData.Bachelor_Institute;
            bMajor.Text = UIData.Bachelor_major;
            MInstitution.Text = UIData.Master_Institute;
            MMajor.Text = UIData.Master_Major;
            DInstitution.Text = UIData.Doctoral_Institute;
            DMajor.Text = UIData.Doctoral_Major;

            CEmployer.Text = UIData.Current_Employer;
            CTitle.Text = UIData.Current_Job_Title;

            CompletionTerm.Text = UIData.Project_Completion_Term;
            CompletionYear.Text = UIData.Project_Completion_Year;
            FormattedDate = DateTime.Parse(UIData.Intent_To_Completion).ToString("dd/MM/yyyy");
            //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "PopulateDatePicker('" + FormattedDate + "')", true);

            //AtAreaDd.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
        }

        public SingleStudent StructureData(DataTable tempTable)
        {
            try
            {
                List<SingleStudent> studentList = new List<SingleStudent>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    SingleStudent student = new SingleStudent();
                    student.UIN = tempTable.Rows[i]["UIN"].ToString();
                    student.Lname = tempTable.Rows[i]["Lname"].ToString();
                    student.Fname = tempTable.Rows[i]["Fname"].ToString();
                    student.Phone = tempTable.Rows[i]["Phone"].ToString();
                    student.UICEmail = tempTable.Rows[i]["UICEmail"].ToString();
                    student.PEmail = tempTable.Rows[i]["PEmail"].ToString();
                    student.GenderId = tempTable.Rows[i]["GenderId"].ToString();
                    student.RaceId = tempTable.Rows[i]["RaceId"].ToString();
                    student.isHispanic = tempTable.Rows[i]["isHispanic"].ToString();
                    student.StreetAddress = tempTable.Rows[i]["StreetAddress"].ToString();
                    student.City = tempTable.Rows[i]["City"].ToString();
                    student.State = tempTable.Rows[i]["State"].ToString();
                    student.PostalCode = tempTable.Rows[i]["PostalCode"].ToString();
                    student.Country = tempTable.Rows[i]["Country"].ToString();
                    student.AdvisorID = tempTable.Rows[i]["AdvisorID"].ToString();
                    student.Bachelor_Institute = tempTable.Rows[i]["Bachelor_Institute"].ToString();
                    student.UIC_Academic_Program = tempTable.Rows[i]["UIC_Academic_Program"].ToString();                   
                    student.Bachelor_major = tempTable.Rows[i]["Bachelor_major"].ToString();
                    student.Master_Institute = tempTable.Rows[i]["Master_Institute"].ToString();
                    student.Master_Major = tempTable.Rows[i]["Master_Major"].ToString();
                    student.Doctoral_Institute = tempTable.Rows[i]["Doctoral_Institute"].ToString();
                    student.ConcentrationID = tempTable.Rows[i]["ConcentrationID"].ToString();
                    student.Doctoral_Major = tempTable.Rows[i]["Doctoral_Major"].ToString();
                    student.Current_Employer = tempTable.Rows[i]["Current_Employer"].ToString();
                    student.Current_Job_Title = tempTable.Rows[i]["Current_Job_Title"].ToString();
                    student.Entry_Term = tempTable.Rows[i]["Entry_Term"].ToString();
                    student.Entry_Year = tempTable.Rows[i]["Entry_Year"].ToString();
                    student.Project_Completion_Term = tempTable.Rows[i]["Project_Completion_Term"].ToString();
                    student.Project_Completion_Year = tempTable.Rows[i]["Project_Completion_Year"].ToString();
                    student.UIC_Academic_Program = tempTable.Rows[i]["UIC_Academic_Program"].ToString();
                    student.ProgramStatusId = tempTable.Rows[i]["ProgramStatusId"].ToString();
                    student.AdvisorName = tempTable.Rows[i]["AdvisorName"].ToString();
                    student.Area_Of_Concentration = tempTable.Rows[i]["Area_Of_Concentration"].ToString();
                    student.Intent_To_Completion = tempTable.Rows[i]["Intent_To_Completion"].ToString();
                    student.Approved_Completion = tempTable.Rows[i]["Approved_Completion"].ToString();
                    student.Actual_Completion_Year = tempTable.Rows[i]["Actual_Completion_Year"].ToString();
                    student.Attempted_ATP_Exam = tempTable.Rows[i]["Attempted_ATP_Exam"].ToString();
                    student.Passed_ATP_Exam = tempTable.Rows[i]["Passed_ATP_Exam"].ToString();
                    student.Advisor_Notes = tempTable.Rows[i]["Advisor_Notes"].ToString();
                    student.Professional_Category = tempTable.Rows[i]["Professional_Category"].ToString();
                    studentList.Add(student);
                }

                return studentList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable SPCallToGetData(string UIN)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetSingleStudentView", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", UIN);

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

        public void Search_Click_Advisor(object sender, EventArgs e)
        {
        }

        public void Search_Click_Student(object sender, EventArgs e)
        {
            //var UIN = searchTextBox.Text;
            // Loading single student data using UIN
            //Get_SingleStudentData(UIN);
        }

        public void ATArea_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        public void DateChange(object sender, EventArgs e)
        {

        }

        public void LoadGenderDropdown()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("ATCPGetGenderInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            //genderDropDown.Items.Clear();
            genderDropDown.DataSource = tab;
            genderDropDown.DataTextField = "Gender_name"; 
            genderDropDown.DataValueField = "GenderId";
            genderDropDown.DataBind();

            genderDropDown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            genderDropDown.Style.Add("display", "table-row");

        }

        public void LoadRaceDropdown()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("ATCP_GetRaceInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            //raceDropDown.Items.Clear();
            raceDropDown.DataSource = tab;
            raceDropDown.DataTextField = "Race";
            raceDropDown.DataValueField = "RaceId";
            raceDropDown.DataBind();

            raceDropDown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            raceDropDown.Style.Add("display", "table-row");
            
        }

        public void LoadProgramStatusDropdown()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("GetProgramStatusInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            //raceDropDown.Items.Clear();
            programStatus.DataSource = tab;
            programStatus.DataTextField = "ProgramStatus_Name";
            programStatus.DataValueField = "ProgramStatusId";
            programStatus.DataBind();

            //programStatus.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            //programStatus.Style.Add("display", "table-row");

        }

        public void LoadConcentrationDetails()
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("GetATCPAreaOfConcentration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
                //----------------------------------------------

                var tab = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

                //raceDropDown.Items.Clear();
                concentrationID.DataSource = tab;
                concentrationID.DataTextField = "ConcentrationName";
                concentrationID.DataValueField = "ConcentrationID";
                concentrationID.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        //public void LoadFacultyDropdownDetails()
        //{
        //    try
        //    {
        //        SqlConnection con = null;
        //        SqlCommand cmd = null;

        //        con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
        //        con.Open();
        //        cmd = new SqlCommand("GetFacultyDropdownDetails", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
        //        //----------------------------------------------

        //        var tab = new DataTable();
        //        using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

        //        //raceDropDown.Items.Clear();
        //        facultyAdvisor.DataSource = tab;
        //        facultyAdvisor.DataTextField = "Faculty_Name";
        //        facultyAdvisor.DataValueField = "FacultyID";
        //        facultyAdvisor.DataBind();
        //        //UINTextBox.ReadOnly = true;
        //        //UINTextBox.BackColor = System.Drawing.ColorTranslator.FromHtml("#F2F0E1"); ;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public void LoadProfessionalCategory()
        {
            
            SqlConnection con = null;
            SqlCommand cmd = null;

            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
            con.Open();
            cmd = new SqlCommand("ATCP_GetAllProfesionalCategory", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ATAreaId", atAreaId);
            //----------------------------------------------

            var tab = new DataTable();
            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tab);

            //raceDropDown.Items.Clear();
            proCategoryDropdown.DataSource = tab;
            proCategoryDropdown.DataTextField = "Pro_CategoryName";
            proCategoryDropdown.DataValueField = "Pro_CategoryId";
            proCategoryDropdown.DataBind();

            //proCategoryDropdown.Items.Insert(0, new ListItem("--Choose one--", "Choose"));
            //proCategoryDropdown.Style.Add("display", "table-row");

        }

        protected void facultyAdvisor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void concentrationArea_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void projectedCourse_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                int retval;
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_UpdateProjectedCourseHistoryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);
                cmd.Parameters.AddWithValue("@CourseNumber", (projectedCourse.Rows[e.RowIndex].FindControl("CourseNumber") as Label).Text.Trim());
                cmd.Parameters.AddWithValue("@Term", (projectedCourse.Rows[e.RowIndex].FindControl("Term") as Label).Text.Trim());
                cmd.Parameters.AddWithValue("@CourseYear", (projectedCourse.Rows[e.RowIndex].FindControl("CourseYear") as Label).Text.Trim());
                
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

                PopulateGridView();
                projectedCourseGridView();
                //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "PopulateDatePicker('" + FormattedDate + "')", true);
            }
            catch (Exception ex)
            {

            }
        }

        protected void ViewMode_Click(object sender, EventArgs e)
        {
            Response.Redirect("ATCPViewStudent.aspx?UIN=" + UIN, false);
        }

        protected void UpdateID_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                SqlConnection con = null;
                SqlCommand cmd = null;

                int retval;
                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_UpdateSingleStudent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UIN", HttpContext.Current.Request.QueryString["UIN"]);
                cmd.Parameters.AddWithValue("@firstNameTextBox", firstNameTextBox.Text);
                cmd.Parameters.AddWithValue("@lastNameTextBox", lastNameTextBox.Text);
                cmd.Parameters.AddWithValue("@genderDropDown", genderDropDown.SelectedValue);
                cmd.Parameters.AddWithValue("@isHispanic", isHispanic.Checked);
                cmd.Parameters.AddWithValue("@raceDropDown", raceDropDown.SelectedValue);
                cmd.Parameters.AddWithValue("@bInstitution", bInstitution.Text);
                cmd.Parameters.AddWithValue("@bMajor", bMajor.Text);
                cmd.Parameters.AddWithValue("@MInstitution", MInstitution.Text);
                cmd.Parameters.AddWithValue("@MMajor", MMajor.Text);
                cmd.Parameters.AddWithValue("@DInstitution", DInstitution.Text);
                cmd.Parameters.AddWithValue("@DMajor", DMajor.Text);
                cmd.Parameters.AddWithValue("@CEmployer", CEmployer.Text);
                cmd.Parameters.AddWithValue("@CTitle", CTitle.Text);
                cmd.Parameters.AddWithValue("@facultyAdvisorText", facultyAdvisorText.Text);
                cmd.Parameters.AddWithValue("@proCategoryDropdown", proCategoryDropdown.SelectedValue);
                cmd.Parameters.AddWithValue("@entryYear", entryYear.Text);
                cmd.Parameters.AddWithValue("@concentrationID", concentrationID.SelectedValue);
                cmd.Parameters.AddWithValue("@programStatus", programStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@uicProgram", uicProgram.Text);
                cmd.Parameters.AddWithValue("@advisoryNotes", advisoryNotes.Text);
                cmd.Parameters.AddWithValue("@uicEmail", uicEmail.Text);
                cmd.Parameters.AddWithValue("@phone", phone.Text);
                cmd.Parameters.AddWithValue("@personalEmail", personalEmail.Text);
                cmd.Parameters.AddWithValue("@address", address.Text);
                cmd.Parameters.AddWithValue("@state", state.Text);
                cmd.Parameters.AddWithValue("@city", city.Text);
                cmd.Parameters.AddWithValue("@postalCode", postalCode.Text);
                cmd.Parameters.AddWithValue("@Country", Country.Text);
                cmd.Parameters.AddWithValue("@CompletionTerm", CompletionTerm.Text);
                cmd.Parameters.AddWithValue("@CompletionYear", CompletionYear.Text);

                string dateIntent = Request.Form["dueDate"];
                string convertedDate = null;
                if (!string.IsNullOrEmpty(dateIntent))
                {
                    convertedDate = DateTime.ParseExact(dateIntent, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                
                cmd.Parameters.AddWithValue("@IntentToComplete", convertedDate);
                cmd.Parameters.AddWithValue("@approvedCompletion", approvedCompletion.Checked);
                cmd.Parameters.AddWithValue("@passedATP", passedATP.Checked);
                cmd.Parameters.AddWithValue("@attemptedATP", attemptedATP.Checked);
                cmd.Parameters.AddWithValue("@CreditsEarned", CreditsEarned.Text);

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
                Response.Redirect("ATCPViewStudent.aspx?UIN=" + UIN, false);                   

                //ShowMessageOperation("Record submitted successfully", MessageType.Success);

            }
            catch(Exception ex)
            {

            }
            
        }
    }
}


