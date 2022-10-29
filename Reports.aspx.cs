using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Globalization;

namespace ATUClient
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Session["ATUID"] = Request.Cookies["ATUId"].Value; shr
                LoadData();
            }
        }

        protected void GenerateReport_Click(object sender, EventArgs e)
        {
            bool isValidStartDate = false;
            if (ReportDropDownList.SelectedItem.Value.Equals("EPC"))
                {
                    string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                                        String navigateUrl = null;
                                        navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentPerClient&rs:Command=Render";    
                                        navigateUrl += //"&StartDate=" + StartDateTextBox.Text +
                                                        //"&EndDate=" + EndDateTextBox.Text +
                                                        "&ClientId=" + CidTextBox.Text;
                                        navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                                        string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                                        Response.Write(redirect);
                }
            if (ReportDropDownList.SelectedItem.Value.Equals("RCPM"))
                {
                    string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                                        String navigateUrl = null;
                                        navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fReferralCountsPerMonth&rs:Command=Render";    
                                        navigateUrl += //"&StartDate=" + StartDateTextBox.Text +
                                                        //"&EndDate=" + EndDateTextBox.Text +
                                                        //"&ClientId=" + CidTextBox.Text;
                                                        "&Year=" + YearTextBox.Text +
                                                        "&SourceID=" + RSourceDropDownList.SelectedValue;
                                        navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                                        string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                                        Response.Write(redirect);
                }
            #region StartDateValidation
            if (!String.IsNullOrEmpty(StartDateTextBox.Text))
            {
                string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                DateTime d;
                isValidStartDate = DateTime.TryParseExact(StartDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (isValidStartDate)
                {
                    StartDateValidationLabel.Visible = false;
                }
                else
                {
                    StartDateValidationLabel.Visible = true;
                    StartDateValidationLabel.Text = "Invalid Date Format";
                }
            }
            else
            {
                StartDateValidationLabel.Visible = true;
                StartDateValidationLabel.Text = "Enter start date.";
            }

            #endregion StartDateValidation

            #region EndDateValidation
            bool isValidEndDate = false;
            if (!String.IsNullOrEmpty(EndDateTextBox.Text))
            {
                string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                DateTime d;
                isValidEndDate = DateTime.TryParseExact(EndDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (isValidEndDate)
                {
                    EndDateValidationLabel.Visible = false;
                }
                else
                {
                    EndDateValidationLabel.Visible = true;
                    EndDateValidationLabel.Text = "Invalid Date Format";
                }
            }
            else
            {
                EndDateValidationLabel.Visible = true;
                EndDateValidationLabel.Text = "Enter end date.";
            }
            #endregion EndDateValidation

            #region datesParameter
            bool isValidDatesColumns = true;
            Dictionary<string, string> tempDatesDict = (Dictionary<string, string>)Session["DatesDictionary"];
            string selectedDatesColumnsString = string.Empty;

            string[] tempDates = DatesHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tempDates.Length; i++)
            {
                if (string.IsNullOrEmpty(selectedDatesColumnsString))
                    selectedDatesColumnsString = tempDatesDict[tempDates[i].Trim()] + ",";
                else
                    selectedDatesColumnsString = selectedDatesColumnsString + tempDatesDict[tempDates[i].Trim()] + ",";
            }
            if (string.IsNullOrEmpty(selectedDatesColumnsString))
            {
                isValidDatesColumns = false;
                DatesColumnValidation.Visible = true;
            }
            else
            {
                isValidDatesColumns = true;
                DatesColumnValidation.Visible = false;
            }

            #endregion datesParameter

            #region clinicianParameter
            Dictionary<string, string> tempCliniciansDict = (Dictionary<string, string>)Session["CliniciansDictionary"];
            string clinicianIds = string.Empty;

            string[] tempClinicians = ClinicianHiddenField.Value.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tempClinicians.Length; i++)
            {
                if (string.IsNullOrEmpty(clinicianIds))
                    clinicianIds = tempCliniciansDict[tempClinicians[i].Trim()];
                else
                    clinicianIds = clinicianIds + "," + tempCliniciansDict[tempClinicians[i].Trim()];
            }
            if (string.IsNullOrEmpty(clinicianIds))
                clinicianIds = (string)Session["ATUStaffID"];
            #endregion clinicianParameter

            #region ReportTypeValidation
            bool isValidReport = false;
            if (ReportDropDownList.SelectedValue.Equals("Choose") || ReportDropDownList.SelectedIndex == -1)
            {
                isValidReport = false;
                ReportTypeValidationLabel.Visible = true;
            }
            else
            {
                isValidReport = true;
                ReportTypeValidationLabel.Visible = false;
            }

            #endregion ReportTypeValidation

            #region PopulateReport
            if (isValidStartDate && isValidEndDate && isValidDatesColumns && isValidReport)
            {
                //if (ReportDropDownList.SelectedItem.Text.Equals("VR Report") || ReportDropDownList.SelectedItem.Text.Equals("Home Services Report") || ReportDropDownList.SelectedItem.Text.Equals("BBS Report"))
                if (ReportDropDownList.SelectedItem.Value.Equals("VR") || ReportDropDownList.SelectedItem.Value.Equals("HSP"))
                {
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fStandardDRSReport&rs:Command=Render";
                    navigateUrl += "&ClinicianIds=" + clinicianIds + "&StartDate=" + StartDateTextBox.Text + "&EndDate=" + EndDateTextBox.Text + "&AllClinician=" + SelectAllCheckbox.Checked + "&ActionIds=" + selectedDatesColumnsString + "&ReportCode=" + ReportDropDownList.SelectedValue;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("DD Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("IL-DHS-DD"))
                {
                    String navigateUrl = "http://support.iidd.uic.edu/ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fStandardDDReport&rs:Command=Render";
                    navigateUrl += "&ClinicianIds=" + clinicianIds + "&StartDate=" + StartDateTextBox.Text + "&EndDate=" + EndDateTextBox.Text + "&AllClinician=" + SelectAllCheckbox.Checked + "&ActionIds=" + selectedDatesColumnsString + "&ReportCode=" + ReportDropDownList.SelectedValue;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Custom Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("CR"))
                {
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fClinicianCustomReport&rs:Command=Render";
                    navigateUrl += "&ClinicianIds=" + clinicianIds + "&StartDate=" + StartDateTextBox.Text + "&EndDate=" + EndDateTextBox.Text + "&AllClinician=" + SelectAllCheckbox.Checked + "&ActionIds=" + selectedDatesColumnsString + "&ReportCode=" + ReportDropDownList.SelectedValue;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //Code by Vivek
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Client Status Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("COL-CS"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fColbertClinicalServiceReport&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ClinicianIds=" + clinicianIds +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Cases Status Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("COL-ACS"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fColbertActiveCasesStatusReport&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ClinicianIds=" + clinicianIds +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Evaluation Turnaround Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("COL-AET"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fColbertEvaluationTurnAround&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }

                //else if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Cases Phase Summary Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("COL-ACPS"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fColbertActiveCasePhasesStatusReport&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }

                //else if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Clinical Services Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("COL-CSR"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fColbertClinicalServiceReport&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Williams Clinical Services Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("WIL-CSR"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fWilliamsClinicalServiceReport&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }
                //else if (ReportDropDownList.SelectedItem.Text.Equals("Home Service Client Address") || ReportDropDownList.SelectedItem.Text.Equals("VR Client Address"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("HSP-CA") || ReportDropDownList.SelectedItem.Value.Equals("VR-CA"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fDRSClientAddress&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString + "&ReportCode=" + ReportDropDownList.SelectedValue;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }

                //else if (ReportDropDownList.SelectedItem.Text.Equals("Home Services 2019 Report") || ReportDropDownList.SelectedItem.Text.Equals("VR 2019 Report"))
                else if (ReportDropDownList.SelectedItem.Value.Equals("HSP-TA") || ReportDropDownList.SelectedItem.Value.Equals("VR-TA"))
                {
                    selectedDatesColumnsString = selectedDatesColumnsString.TrimEnd(',');

                    //String navigateUrl = "https:" + "//" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + 
                    String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fDRSFY2019&rs:Command=Render";
                    navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                    "&EndDate=" + EndDateTextBox.Text +
                                    "&ActionIds=" + selectedDatesColumnsString + "&ReportCode=" + ReportDropDownList.SelectedValue;
                    navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                    string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                    Response.Write(redirect);
                }

                //else if (ReportDropDownList.SelectedItem.Text.Equals("Equipment Per Client"))
                /* else if (ReportDropDownList.SelectedItem.Value.Equals("EPC"))
                {
                    string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                                        String navigateUrl = null;
                                        navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentPerClient&rs:Command=Render";    
                                        navigateUrl += //"&StartDate=" + StartDateTextBox.Text +
                                                        //"&EndDate=" + EndDateTextBox.Text +
                                                        "&ClientId=" + CidTextBox.Text;
                                        navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                                        string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                                        Response.Write(redirect);
                }*/
            }    
            else
                //if (ReportDropDownList.SelectedItem.Text.Equals("Guidance Report") || ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued"))
                if (ReportDropDownList.SelectedItem.Value.Equals("COL-G") || ReportDropDownList.SelectedItem.Value.Equals("COL-EI") || ReportDropDownList.SelectedItem.Value.Equals("COL-EIC") || ReportDropDownList.SelectedItem.Value.Equals("COL-EIW") || ReportDropDownList.SelectedItem.Value.Equals("COL-EIBA") || ReportDropDownList.SelectedItem.Value.Equals("WIL-EIBA") || ReportDropDownList.SelectedItem.Value.Equals("TAP-S"))
                {
                    bool valid_start_date = false, valid_end_date = false;
                    string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                    DateTime d;

                    if (!String.IsNullOrEmpty(StartDateTextBox.Text))
                    {
                        valid_start_date = DateTime.TryParseExact(StartDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);

                        if (valid_start_date)
                        {
                            if (!String.IsNullOrEmpty(EndDateTextBox.Text))
                            {
                                valid_end_date = DateTime.TryParseExact(EndDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);

                                if (valid_end_date)
                                {
                                    if (Convert.ToDateTime(StartDateTextBox.Text) <= Convert.ToDateTime(EndDateTextBox.Text))
                                    {
                                        String navigateUrl = null;
                                        //if (ReportDropDownList.SelectedItem.Text.Equals("Guidance Report"))
                                        if (ReportDropDownList.SelectedItem.Value.Equals("COL-G"))
                                            navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fGuidanceReport&rs:Command=Render";
                                        else
                                            //if (ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued"))
                                            if (ReportDropDownList.SelectedItem.Value.Equals("COL-EI"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssued&rs:Command=Render";
                                            else if (ReportDropDownList.SelectedItem.Value.Equals("COL-EIC"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssuedColbert&rs:Command=Render";
                                            else if (ReportDropDownList.SelectedItem.Value.Equals("COL-EIW"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssuedWilliams&rs:Command=Render";
                                            else if (ReportDropDownList.SelectedItem.Value.Equals("COL-EIBA"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssuedByAreaColbert&rs:Command=Render";
                                            else if (ReportDropDownList.SelectedItem.Value.Equals("WIL-EIBA"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssuedByAreaWilliams&rs:Command=Render";
                                            else if (ReportDropDownList.SelectedItem.Value.Equals("TAP-S"))
                                                navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fServiceTap&rs:Command=Render";
                                        
                                        navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                                        "&EndDate=" + EndDateTextBox.Text;
                                        navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                                        string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                                        Response.Write(redirect);
                                    }
                                    else
                                    {
                                        DatesColumnValidation.Visible = false;
                                        EndDateValidationLabel.Visible = true;
                                        EndDateValidationLabel.Text = "End date cannot be smaller than the start date.";
                                    }
                                }
                                else
                                {
                                    DatesColumnValidation.Visible = false;
                                    EndDateValidationLabel.Visible = true;
                                    EndDateValidationLabel.Text = "Invalid Date Format";
                                }

                            }
                            else
                            {
                                DatesColumnValidation.Visible = false;
                                EndDateValidationLabel.Visible = true;
                                EndDateValidationLabel.Text = "Enter End Date.";
                            }
                        }
                        else
                        {
                            DatesColumnValidation.Visible = false;
                            StartDateValidationLabel.Visible = true;
                            StartDateValidationLabel.Text = "Invalid Date Format";
                        }
                    }
                    else
                    {
                        DatesColumnValidation.Visible = false;
                        StartDateValidationLabel.Visible = true;
                        StartDateValidationLabel.Text = "Enter Start Date.";
                    }
                }

                else
                    //if (ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued"))
                    if (ReportDropDownList.SelectedItem.Value.Equals("COL-EI"))
                    {
                        bool valid_start_date = false, valid_end_date = false;
                        string[] validFormats = { "MM/dd/yyyy", "M/d/yyyy" };
                        DateTime d;

                        if (!String.IsNullOrEmpty(StartDateTextBox.Text))
                        {
                            valid_start_date = DateTime.TryParseExact(StartDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);

                            if (valid_start_date)
                            {
                                if (!String.IsNullOrEmpty(EndDateTextBox.Text))
                                {
                                    valid_end_date = DateTime.TryParseExact(EndDateTextBox.Text, validFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);

                                    if (valid_end_date)
                                    {
                                        if (Convert.ToDateTime(StartDateTextBox.Text) <= Convert.ToDateTime(EndDateTextBox.Text))
                                        {
                                            String navigateUrl = "../ReportServer/Pages/ReportViewer.aspx?%2fATUClientReports%2fEquipmentIssued&rs:Command=Render";
                                            navigateUrl += "&StartDate=" + StartDateTextBox.Text +
                                                            "&EndDate=" + EndDateTextBox.Text;
                                            navigateUrl += "&rc:Parameters=false" + "&rs:ClearSession=True";
                                            string redirect = "<script>window.open('" + navigateUrl + "');</script>";
                                            Response.Write(redirect);
                                        }
                                        else
                                        {
                                            DatesColumnValidation.Visible = false;
                                            EndDateValidationLabel.Visible = true;
                                            EndDateValidationLabel.Text = "End date cannot be smaller than the start date.";
                                        }
                                    }
                                    else
                                    {
                                        DatesColumnValidation.Visible = false;
                                        EndDateValidationLabel.Visible = true;
                                        EndDateValidationLabel.Text = "Invalid Date Format";
                                    }

                                }
                                else
                                {
                                    DatesColumnValidation.Visible = false;
                                    EndDateValidationLabel.Visible = true;
                                    EndDateValidationLabel.Text = "Enter End Date.";
                                }
                            }
                            else
                            {
                                DatesColumnValidation.Visible = false;
                                StartDateValidationLabel.Visible = true;
                                StartDateValidationLabel.Text = "Invalid Date Format";
                            }
                        }
                        else
                        {
                            DatesColumnValidation.Visible = false;
                            StartDateValidationLabel.Visible = true;
                            StartDateValidationLabel.Text = "Enter Start Date.";
                        }
                    }
            #endregion PopulateReport
        }

        protected void StartDateCalender_Click(object sender, EventArgs e)
        {
            if (!StartDateCalendar.Visible)
                StartDateCalendar.Visible = true;
            else
                StartDateCalendar.Visible = false;
        }

        protected void EndDateCalender_Click(object sender, EventArgs e)
        {
            if (!EndDateCalendar.Visible)
                EndDateCalendar.Visible = true;
            else
                EndDateCalendar.Visible = false;
        }

        protected void StartDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            PutDateInStartBox();
        }

        protected void EndDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            PutDateInEndBox();
        }
  
        protected void Report_Selected(object sender, EventArgs e)
        {
            if (!ReportDropDownList.SelectedValue.Equals("Choose"))
            {   if (ReportDropDownList.SelectedItem.Text.Equals("Equipment Per Client"))
                {
                    ReportButton.Enabled = true;
                    DatesButton.Disabled = true;
                    DatesTextBox.Enabled = false;
                    ClinicianButton.Disabled = true;
                    ClinicianTextBox.Enabled = false;
                    SelectAllCheckbox.Enabled = false;
                    CidTextBox.Enabled = true;
                    YearTextBox.Enabled = false; 
                    RSourceDropDownList.Enabled = false;
                    StartDateTextBox.Enabled=false;
                    EndDateTextBox.Enabled=false;
                    StartDateButton.Enabled=false;
                    EndDateButton.Enabled=false;

                    string datesTextBoxValues = string.Empty;
                    List<string> reportColumnList = new List<string>();
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                    var reportColumnMappingTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                    foreach (DataRow row in reportColumnMappingTable.Rows)
                    {
                        reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                    }

                    for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                    {
                        if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                        {
                            DatesCheckBoxList.Items[i].Selected = true;
                            datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                        }
                    }
                    DatesHiddenField.Value = datesTextBoxValues;
                    DatesTextBox.Text = datesTextBoxValues;
                    con.Close();
                }
                else if (ReportDropDownList.SelectedItem.Text.Equals("Referral Counts Per Month"))
                {
                    ReportButton.Enabled = true;
                    DatesButton.Disabled = true;
                    DatesTextBox.Enabled = false;
                    ClinicianButton.Disabled = true;
                    ClinicianTextBox.Enabled = false;
                    SelectAllCheckbox.Enabled = false;
                    CidTextBox.Enabled= false;
                    YearTextBox.Enabled = true;
                    RSourceDropDownList.Enabled = true;
                    StartDateTextBox.Enabled=false;
                    EndDateTextBox.Enabled=false;
                    StartDateButton.Enabled=false;
                    EndDateButton.Enabled=false;

                    string datesTextBoxValues = string.Empty;
                    List<string> reportColumnList = new List<string>();
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                    var reportColumnMappingTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                    foreach (DataRow row in reportColumnMappingTable.Rows)
                    {
                        reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                    }

                    for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                    {
                        if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                        {
                            DatesCheckBoxList.Items[i].Selected = true;
                            datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                        }
                    }
                    DatesHiddenField.Value = datesTextBoxValues;
                    DatesTextBox.Text = datesTextBoxValues;
                    con.Close();
                }
                else if (ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued") || ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued Colbert") || ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued Williams") || ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued By Area Colbert") || ReportDropDownList.SelectedItem.Text.Equals("Equipment Issued By Area Williams") || ReportDropDownList.SelectedItem.Text.Equals("TAP Service Report"))
                {
                    ReportButton.Enabled = true;
                    DatesButton.Disabled = true;
                    DatesTextBox.Enabled = false;
                    ClinicianButton.Disabled = true;
                    ClinicianTextBox.Enabled = false;
                    SelectAllCheckbox.Enabled = false;
                    CidTextBox.Enabled= false;
                    YearTextBox.Enabled = false;
                    RSourceDropDownList.Enabled = false;
                    StartDateTextBox.Enabled=true;
                    EndDateTextBox.Enabled=true;
                    StartDateButton.Enabled=true;
                    EndDateButton.Enabled=true;

                    string datesTextBoxValues = string.Empty;
                    List<string> reportColumnList = new List<string>();
                    SqlConnection con = null;
                    SqlCommand cmd = null;

                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                    cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                    var reportColumnMappingTable = new DataTable();
                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                    foreach (DataRow row in reportColumnMappingTable.Rows)
                    {
                        reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                    }

                    for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                    {
                        if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                        {
                            DatesCheckBoxList.Items[i].Selected = true;
                            datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                        }
                    }
                    DatesHiddenField.Value = datesTextBoxValues;
                    DatesTextBox.Text = datesTextBoxValues;
                    con.Close();
                }
                else
                    if (ReportDropDownList.SelectedItem.Text.Equals("Guidance Report"))
                    {
                        ReportButton.Enabled = true;
                        DatesButton.Disabled = true;
                        DatesTextBox.Enabled = false;
                        ClinicianButton.Disabled = true;
                        ClinicianTextBox.Enabled = false;
                        SelectAllCheckbox.Enabled = false;
                        CidTextBox.Enabled= false;
                        YearTextBox.Enabled = false;
                        RSourceDropDownList.Enabled = false;
                        StartDateTextBox.Enabled=true;
                        EndDateTextBox.Enabled=true;
                        StartDateButton.Enabled=true;
                        EndDateButton.Enabled=true;

                        string datesTextBoxValues = string.Empty;
                        List<string> reportColumnList = new List<string>();
                        SqlConnection con = null;
                        SqlCommand cmd = null;

                        con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                        cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                        var reportColumnMappingTable = new DataTable();
                        using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                        foreach (DataRow row in reportColumnMappingTable.Rows)
                        {
                            reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                        }

                        for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                        {
                            if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                            {
                                DatesCheckBoxList.Items[i].Selected = true;
                                datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                            }
                        }
                        DatesHiddenField.Value = datesTextBoxValues;
                        DatesTextBox.Text = datesTextBoxValues;
                        con.Close();
                    }
                    else
                        //if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Client Status Report") || ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Cases Status Report"))
                        if (ReportDropDownList.SelectedItem.Value.Equals("COL-CS") || ReportDropDownList.SelectedItem.Value.Equals("COL-ACS"))
                        {
                            ReportButton.Enabled = true;
                            DatesButton.Disabled = true;
                            DatesTextBox.Enabled = true;
                            ClinicianButton.Disabled = false;
                            ClinicianTextBox.Enabled = true;
                            SelectAllCheckbox.Enabled = true;
                            CidTextBox.Enabled= false;
                            YearTextBox.Enabled = false;
                            RSourceDropDownList.Enabled = false;
                            StartDateTextBox.Enabled=true;
                            EndDateTextBox.Enabled=true;
                            StartDateButton.Enabled=true;
                            EndDateButton.Enabled=true;

                            string datesTextBoxValues = string.Empty;
                            List<string> reportColumnList = new List<string>();
                            SqlConnection con = null;
                            SqlCommand cmd = null;

                            con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                            cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                            var reportColumnMappingTable = new DataTable();
                            using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                            foreach (DataRow row in reportColumnMappingTable.Rows)
                            {
                                reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                            }

                            for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                            {
                                if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                                {
                                    DatesCheckBoxList.Items[i].Selected = true;
                                    datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                                }
                            }
                            DatesHiddenField.Value = datesTextBoxValues;
                            DatesTextBox.Text = datesTextBoxValues;
                            con.Close();
                        }
                        else
                            //if (ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Evaluation Turnaround Report") || ReportDropDownList.SelectedItem.Text.Equals("Colbert Active Cases Phase Summary Report") || ReportDropDownList.SelectedItem.Text.Equals("Colbert Clinical Services Report") || ReportDropDownList.SelectedItem.Text.Equals("Williams Clinical Services Report")
                            //    || ReportDropDownList.SelectedItem.Text.Equals("Home Service Client Address") || ReportDropDownList.SelectedItem.Text.Equals("VR Client Address") || ReportDropDownList.SelectedItem.Text.Equals("VR 2019 Report")
                            //    || ReportDropDownList.SelectedItem.Text.Equals("Home Services 2019 Report"))
                            if (ReportDropDownList.SelectedItem.Value.Equals("COL-AET") || ReportDropDownList.SelectedItem.Value.Equals("COL-ACPS") || ReportDropDownList.SelectedItem.Value.Equals("COL-CSR")
                                || ReportDropDownList.SelectedItem.Value.Equals("HSP-CA") || ReportDropDownList.SelectedItem.Value.Equals("VR-CA") || ReportDropDownList.SelectedItem.Value.Equals("VR-TA")
                                || ReportDropDownList.SelectedItem.Value.Equals("HSP-TA") || ReportDropDownList.SelectedItem.Value.Equals("WIL-CSR"))
                            {
                                ReportButton.Enabled = true;
                                ClinicianButton.Disabled = true;
                                ClinicianTextBox.Enabled = false;
                                SelectAllCheckbox.Enabled = false;
                                CidTextBox.Enabled= false;
                                YearTextBox.Enabled = false;
                                RSourceDropDownList.Enabled = false;
                                StartDateTextBox.Enabled=true;
                                EndDateTextBox.Enabled=true;
                                StartDateButton.Enabled=true;
                                EndDateButton.Enabled=true;

                                string datesTextBoxValues = string.Empty;
                                List<string> reportColumnList = new List<string>();
                                SqlConnection con = null;
                                SqlCommand cmd = null;

                                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                                cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId = (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                                cmd.CommandType = CommandType.Text;

                                cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                                var reportColumnMappingTable = new DataTable();
                                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                                foreach (DataRow row in reportColumnMappingTable.Rows)
                                {
                                    reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                                }

                                for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                                {
                                    if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                                    {
                                        DatesCheckBoxList.Items[i].Selected = true;
                                        datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                                    }
                                }
                                DatesHiddenField.Value = datesTextBoxValues;
                                DatesTextBox.Text = datesTextBoxValues;
                                con.Close();
                            }
                            else
                            {
                                DatesTextBox.Enabled = true;
                                ClinicianButton.Disabled = false;
                                ClinicianTextBox.Enabled = true;
                                SelectAllCheckbox.Enabled = true;
                                CidTextBox.Enabled= false;
                                YearTextBox.Enabled = false;
                                RSourceDropDownList.Enabled = false;
                                StartDateTextBox.Enabled=true;
                                EndDateTextBox.Enabled=true;
                                StartDateButton.Enabled=true;
                                EndDateButton.Enabled=true;

                                try
                                {
                                    if (ReportDropDownList.SelectedItem.Text.Equals("Case Management Report"))
                                        ReportButton.Enabled = false;
                                    else
                                        ReportButton.Enabled = true;

                                    if (!ReportDropDownList.SelectedItem.Text.Equals("Custom Report"))
                                        DatesButton.Disabled = true;
                                    else
                                        DatesButton.Disabled = false;
                                    string datesTextBoxValues = string.Empty;
                                    List<string> reportColumnList = new List<string>();
                                    SqlConnection con = null;
                                    SqlCommand cmd = null;

                                    con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                                    cmd = new SqlCommand("select * from dbo.ReportColumnsMapping where ReportId in (select ReportId from LookUpReports where ReportCode = @ReportCode)", con);
                                    cmd.CommandType = CommandType.Text;

                                    cmd.Parameters.AddWithValue("@ReportCode", ReportDropDownList.SelectedValue);

                                    var reportColumnMappingTable = new DataTable();
                                    using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportColumnMappingTable);

                                    foreach (DataRow row in reportColumnMappingTable.Rows)
                                    {
                                        reportColumnList.Add(row["TaskId"] + "-" + row["ActionId"]);
                                    }

                                    for (int i = 0; i < DatesCheckBoxList.Items.Count; i++)
                                    {
                                        if (reportColumnList != null && reportColumnList.Contains(DatesCheckBoxList.Items[i].Value))
                                        {
                                            DatesCheckBoxList.Items[i].Selected = true;
                                            datesTextBoxValues += DatesCheckBoxList.Items[i].Text + "\n";
                                        }
                                    }
                                    DatesHiddenField.Value = datesTextBoxValues;
                                    DatesTextBox.Text = datesTextBoxValues;
                                    con.Close();
                                }
                                catch (Exception ex)
                                {
                                    ErrorHandler(ex);
                                }
                            }
            }
            else
            {
                DatesHiddenField.Value = string.Empty;
                DatesTextBox.Text = string.Empty;
            }
        }

        protected void OnSelectAll_Checked(object sender, EventArgs e)
        {
            if (DatesHiddenField.Value != null)
                DatesTextBox.Text = DatesHiddenField.Value;

            string clinicianTextBoxValues = string.Empty;
            if (SelectAllCheckbox.Checked)
            {
                for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
                {
                    CliniciansCheckBoxList.Items[i].Selected = true;
                    clinicianTextBoxValues += CliniciansCheckBoxList.Items[i].Text + "\n";
                }
            }
            else
            {
                clinicianTextBoxValues = string.Empty;
                for (int i = 0; i < CliniciansCheckBoxList.Items.Count; i++)
                {
                    CliniciansCheckBoxList.Items[i].Selected = false;
                }
            }
            ClinicianTextBox.Text = clinicianTextBoxValues;
            ClinicianHiddenField.Value = clinicianTextBoxValues;
        }

        protected void PutDateInStartBox()
        {
            StartDateTextBox.Text = StartDateCalendar.SelectedDate.ToString("MM/dd/yyyy");
            if (!StartDateCalendar.Visible)
                StartDateCalendar.Visible = true;
            else
                StartDateCalendar.Visible = false;
        }

        protected void PutDateInEndBox()
        {
            EndDateTextBox.Text = EndDateCalendar.SelectedDate.ToString("MM/dd/yyyy");
            if (!EndDateCalendar.Visible)
                EndDateCalendar.Visible = true;
            else
                EndDateCalendar.Visible = false;
        }

        private void LoadData()
        {
            try
            {
                #region DBCall

                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);

                #region Report Type
                cmd = new SqlCommand("select * from dbo.LookUpReports where Active = 1 order by SortOrder", con);
                cmd.CommandType = CommandType.Text;

                var reportTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(reportTable);

                ReportDropDownList.DataSource = reportTable;
                ReportDropDownList.DataTextField = "ReportName";
                ReportDropDownList.DataValueField = "ReportCode";
                ReportDropDownList.DataBind();

                ReportDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Report Type
                
                #region Referral Source
                cmd = new SqlCommand("Select * from dbo.LookUpReferralSource where Active = 1 Order by DisplayOrder ASC", con);
                cmd.CommandType = CommandType.Text;

                var refSourceTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(refSourceTable);

                RSourceDropDownList.DataSource = refSourceTable;
                RSourceDropDownList.DataTextField = "ReferralSource";
                RSourceDropDownList.DataValueField = "ReferralSourceId";
                RSourceDropDownList.DataBind();
                RSourceDropDownList.Items.Insert(0, new ListItem("--Choose one--", "Choose"));

                #endregion Referral Source
                #region Dates
                cmd = new SqlCommand("select * from dbo.V_LookupActionReport", con);
                cmd.CommandType = CommandType.Text;

                var datesTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(datesTable);

                DatesCheckBoxList.DataSource = datesTable;
                DatesCheckBoxList.DataTextField = "Display";
                DatesCheckBoxList.DataValueField = "ReportActionId";
                DatesCheckBoxList.DataBind();

                Dictionary<string, string> dictActionDates = new Dictionary<string, string>();
                foreach (DataRow row in datesTable.Rows)
                {
                    dictActionDates.Add(row[2].ToString(), row[0].ToString());
                    //dictActionDates.Add(row[0].ToString(), row[2].ToString());
                }

                Session["DatesDictionary"] = dictActionDates;


                #endregion Dates

                #region ClinicianVisibility

                string netId = Session["ATUID"].ToString();
                cmd = new SqlCommand("select Role from dbo.LookUpRole where roleid = (select [role] from dbo.LookupATUStaff where netid = @NetId and Active = 1)", con);
                cmd.Parameters.AddWithValue("@NetId", netId);

                var clinicianVisibleTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(clinicianVisibleTable);


                if (clinicianVisibleTable.Rows != null && clinicianVisibleTable.Rows.Count > 0 && clinicianVisibleTable.Rows[0][0].ToString() == "Case Manager")
                {
                    //Case Manager
                    ClinicianRow.Visible = true;
                    SelectAllRow.Visible = true;
                    SelectAllCheckbox.Checked = false;
                }
                else
                {
                    //Clinician
                    ClinicianRow.Visible = false;
                    SelectAllRow.Visible = false;
                }
                #endregion ClinicianVisibility

                #region ATUClinician

                cmd = new SqlCommand("select * from LookupATUStaff where Active = 1 order by Name", con);
                cmd.CommandType = CommandType.Text;

                var clinicianTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(clinicianTable);

                CliniciansCheckBoxList.DataSource = clinicianTable;
                CliniciansCheckBoxList.DataTextField = "Name";
                CliniciansCheckBoxList.DataValueField = "NetID";
                CliniciansCheckBoxList.DataBind();


                Dictionary<string, string> dictClinicians = new Dictionary<string, string>();
                foreach (DataRow row in clinicianTable.Rows)
                {
                    dictClinicians.Add(row[1].ToString(), row[0].ToString());
                    //to fetch the ATUStaffId of the login user
                    if (row[2].ToString().Equals((string)Session["ATUID"]))
                        Session["ATUStaffID"] = row[0].ToString();
                }

                Session["CliniciansDictionary"] = dictClinicians;

                #endregion ATUClinician

                con.Close();

                #endregion DBCall
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Method to log errors and send error email
        /// </summary>
        /// <param name="ex"></param>
        private void ErrorHandler(Exception ex)
        {
            //Logging Errors
            string errorHandlerSource = Request.QueryString["handler"];
            if (errorHandlerSource == null)
            {
                errorHandlerSource = "Error Page";
            }
            ExceptionFile.ExceptionUtility.LogException(ex, errorHandlerSource);
            Response.Write("<script language=javascript>alert('Error!!! Email sent to the support team. Please contact for further info.')</script>");

            //Email Notification to the support team
            if (Session != null && Session["ATUID"] != null)
            {
                ExceptionFile.EmailUtility.SendEmail(Session["ATUID"].ToString(), ex.StackTrace);
            }
        }
    }
}