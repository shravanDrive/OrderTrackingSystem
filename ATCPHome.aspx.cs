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
    public partial class ATCPHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string GetBaseData()
        {
            string returnObj;
            try
            {
                SqlConnection con = null;
                SqlCommand cmd = null;

                con = new SqlConnection(WebConfigurationManager.AppSettings["AppServices"]);
                con.Open();
                cmd = new SqlCommand("ATCP_GetAllStudents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Barcode", barcode);

                //----------------------------------------------

                var tempTable = new DataTable();
                using (var myAdapter = new SqlDataAdapter(cmd)) myAdapter.Fill(tempTable);
                con.Close();

                List<Students> studentList = new List<Students>();
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    Students student = new Students();
                    student.UIN = tempTable.Rows[i]["UIN"].ToString();
                    student.Lname = tempTable.Rows[i]["Lname"].ToString();
                    student.FName = tempTable.Rows[i]["Fname"].ToString();
                    student.EntryTerm = tempTable.Rows[i]["EntryTerm"].ToString();
                    student.EntryYear = tempTable.Rows[i]["EntryYear"].ToString();
                    student.ProgramStatus = tempTable.Rows[i]["ProgramStatus_Name"].ToString();
                    student.Advisor = tempTable.Rows[i]["AdvisorName"].ToString();
                    studentList.Add(student);
                }

                ////return studentList;
                returnObj =  JsonConvert.SerializeObject(studentList);

            }
            catch (Exception ex)
            {
                returnObj = null;
            }

            return returnObj;
        }

        protected void Checker(object sender, EventArgs e)
        {
            
        }
    }
}
