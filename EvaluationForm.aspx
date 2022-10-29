<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EvaluationForm.aspx.cs" Inherits="ATUClient.EvaluationForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <style type="text/css">
        .Control
        {
            height: 13px;
            width: 285px;
        }
        .style4
        {
            height: 23px;
        }
        #content
        {
            padding: 15px 30px 0 0;
        }
        #main-content
        {
            margin-left: 51%;
            max-width: 50%;
        }
        #sidebar
        {
            width: 50%;
            float: left;
            padding: 0 0 0 5px;
        }
        #sidebar ul
        {
            list-style: inside;
        }
        .style5
        {
            width: 30%;
            height: 31px;
        }
        .style6
        {
            height: 31px;
        }
        .hiddencol
        {
            display: none;
        }
        .uploadmenu
        {
            margin-left: 15%;
         }
         p {
            color: red;
            margin: 4px;
        }
    </style>

    <link href="scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery-1.11.0.min.js" type="text/javascript"></script>   
    <script src="scripts/xlsx.core.min.js" type="text/javascript"></script>
    <script src="scripts/FileSaver.min.js" type="text/javascript"></script>
    <script src="scripts/EvalForm/helper.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/EvalForm/data.json"></script>
    <link rel="stylesheet" href="Styles/EvalForm.css"/>
    
    <script src="scripts/bootstrap.js" type="text/javascript"></script>
    <link href="Styles/bootstrap.css" rel="stylesheet" type="text/css" />
    
      <script language="javascript" type="text/javascript">
          // Current option
          var currentString = "";
          var tempString = "";
          var clinicianCurrentString = "";

          $(function() {

              $(".datepicker").datepicker({ dateFormat: 'mm/dd/yy', changeMonth: true,
                  changeYear: true
              })

              // Define the event for your modal
              $("#dialog").dialog({
                  autoOpen: false,
                  height: 600,
                  width: 1050,
                  modal: true,
                  buttons: {
                      Cancel: function() {
                          $(this).dialog("close");
                      },
                      "Save": function() {
                          currentString = "";
                          $(this).dialog("close");
                      }
                  },
                  open: function(event, ui) {
                      $(".ui-dialog-titlebar-close", $(this).parent()).hide();
                  },
                  closeOnEscape: false
              });

              // When your modal is clicked, open the modal and store a reference to the element that was clicked
              $(".modal").click(function() {
                  // Open your modal
                  $("#dialog").dialog("open");
              });


              ///PUT class="positiveintegerOnly" in all the textboxes where you want only positive numbers.
              $('.positiveintegerOnly').keyup(function() {
                  this.value = this.value.replace(/[^0-9]/g, '');
                  if (this.value < 0) this.value = 0;
              });
              //positiveinteger_decimal
              $('.positiveinteger_deciamalOnly').keyup(function() {
                  this.value = this.value.replace(/[^0-9\.]/g, '');
                  if (this.value < 0) this.value = 0;
              });


              //            $(".deleteService").click(function() {           
              //                return confirm('Are you sure you want to delete this service?');                
              //            });
              //////////////////////////////////////////////
              $("#dialog1").dialog({
                  autoOpen: false,
                  height: 600,
                  width: 1050,
                  modal: true,
                  buttons: {
                      Cancel: function() {
                          $(this).dialog("close");
                      },
                      "Save": function() {
                          clinicianCurrentString = "";
                          $("[id*=CliniciansCheckBoxList] input:checked").each(function() {
                              tempString = $(this).next().text() + "\n";
                              //tempString = tempString.substr(0, 5);
                              clinicianCurrentString += tempString; //+ "\n";
                          });
                          $("#<%= ClinicianTextBox.ClientID%>").val(clinicianCurrentString);
                          $("#<%= ClinicianHiddenField.ClientID%>").val(clinicianCurrentString);
                          $(this).dialog("close");
                      }
                  },
                  open: function(event, ui) {
                      $(".ui-dialog-titlebar-close", $(this).parent()).hide();
                  },
                  closeOnEscape: false
              });

              // When your modal is clicked, open the modal and store a reference to the element that was clicked
              $(".modal1").click(function() {
                  // Open your modal
                  $("#dialog1").dialog("open");
              });
          });        
    </script>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <div style="width: 50%; float: left">
    <br />           
     </div>
    <table style="width: 100%">
        <tr>
            <td>
                <div id="sidebar">
                    <table style="width: 100%">
                        <tr>
                            <td class="style4" style="text-align: left; width: 30%;">
                                <b>Search</b>
                                <asp:Label ID="RoleLabel" Visible="true" runat="server" Text=""></asp:Label>
                            </td>
                            <td class="style4" style="text-align: left; width: 30%;">
                                &nbsp;
                            </td>
                            <td class="style4" style="text-align: leftwidth: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                Last Name:
                            </td>
                            <td style="width: 30%">
                                First Name:
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                <asp:TextBox ID="LastNameText" runat="server" Width="96%"></asp:TextBox>
                            </td>
                            <td style="width: 30%">
                                <asp:TextBox ID="FirstNameText" runat="server" Width="96%"></asp:TextBox>
                            </td>
                            <td style="width: 40%">
                                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"
                                    Width="49%" />
                                <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"
                                    Width="49%" /><br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                &nbsp;
                                <asp:ScriptManager ID="ScriptManager1" runat="server">
                                </asp:ScriptManager>
                            </td>
                            <td style="width: 30%">
                                &nbsp;
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                                <asp:Label ID="ReferralDateLabel" runat="server" BorderStyle="None" Font-Bold="True"
                                    Text="Referral Date :"></asp:Label><br />
                            </td>
                            <td style="width: 30%">
                                <asp:DropDownList ID="ReferralDateDropDownList" Width="150" runat="server" OnSelectedIndexChanged="ReferralDate_Changed"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 40%">
                                &nbsp;</td>
                        </tr>
                        
                                              <tr>
                            <td style="text-align: left; vertical-align: middle;" class="style5">
                                
                            </td>
                            <td colspan="2" class="style6">
                                <asp:CheckBoxList ID="ServiceTypeCheckBoxList" runat="server" RepeatDirection="Horizontal"
                                    RepeatColumns="5" AutoPostBack="true" BorderStyle="Solid" BorderColor="LightGray"
                                    BorderWidth="1" OnSelectedIndexChanged="ServiceType_Changed" Width="30%" Font-Size="Small">
                                </asp:CheckBoxList>
                                <asp:CustomValidator runat="server" ID="CVServiceType" onservervalidate="ServiceTypeValidation" ValidationGroup="atsave"
                                    ErrorMessage="Please Select At least One Service Type"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <asp:Panel ID="updatepanel_ClinicianRow" runat="server">
                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                    <asp:Label ID="ClinicianLabel" runat="server" Font-Bold="true" Text="ATU Clinician (s): "> </asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:CheckBox ID="chkbx_ShowClinicians" runat="server" AutoPostBack="true" OnCheckedChanged="chkbx_ShowClinicians_CheckedChanged"
                                        Text="Show Clinicians" />
                                    <asp:CheckBoxList ID="CliniciansCheckBoxList" runat="server" RepeatDirection="Vertical"
                                        Font-Size="Small" RepeatColumns="2" Width="100%" BorderStyle="Solid" BorderColor="LightGray"
                                        BorderWidth="1" AutoPostBack="true" OnSelectedIndexChanged="CliniciansCheckBoxList_SelectedIndexChanged">
                                    </asp:CheckBoxList>
                                    <asp:TextBox ID="ClinicianTextBox" runat="server" TextMode="MultiLine" Visible="False"
                                        Width="75%" ReadOnly="true" Font-Size="Small" Height="70px"></asp:TextBox>
                                    <asp:HiddenField ID="ClinicianHiddenField" runat="server" />
                                </td>
                                <td>
                                 <asp:Button CssClass="btn btn-sm btn-primary" ID="addPatientBtn" Text="Add Patient" OnClick="AddPatient"  runat="server" AutoPostBack="True" /> 
                                </td>
                            </asp:Panel>
                        </tr>   
</table>
</div>

<div id="main-content">
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="CurrentClientLabel" runat="server" Font-Bold="True" ForeColor="Red"
                                    Text="Current Client: "> </asp:Label>
                                <asp:Label ID="CurrentClientValueLabel" runat="server" Text="No user selected" Font-Bold="False"
                                    ForeColor="Black"> </asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        
                        <tr>
                            <td>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                    OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command" PageSize="20">
                                </asp:GridView>
                                <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                    Text="No search results found" Visible="true"> </asp:Label>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
</td>
</tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server">
<div id ="report-generator">
    <h1 id="status"> Report Generator</h1>
    <div>
       <b>Generate Template</b> : <input type="file" id="patient" class = "uploadmenu"><br />
     <br />
    </div>
     <div id="wrapper">
        <strong>Upload the xlsx file below</strong>
        <input type="file" id="input-excel" class = "uploadmenu"/>
    </div>

    <div>
        <textarea id="text">
        </textarea>
        <br>
        <button id="export">Export</button>
        <p>Click to open report in Microsoft Word </p>
    </div>
    </div>
    <script type="text/javascript">
        var patient;
        var myjson = {};
        var val = {};
        n = new Date();
        y = n.getFullYear();
        m = n.getMonth() + 1;
        d = n.getDate();

        var filename;

        $(window).load(function() {
          var data = $.getJSON("scripts/EvalForm/data.json", function(data) {
            myjson = data;
            console.log(myjson);
          }).fail(function() {
            console.log("failed");
          });
        });

        $("#patient").change(function(e) {
          var reader = new FileReader();
          reader.readAsArrayBuffer(e.target.files[0]);
          reader.onload = function(e) {
            var data = new Uint8Array(reader.result);
            var wb = XLSX.read(data, { type: "array" });
            var sheet_name_list = wb.SheetNames;
            var ws = wb.Sheets[sheet_name_list[0]];
            var jsonob = XLSX.utils.sheet_to_json(ws, {
              range: 0,
              header: ["Column 1", "Column 2", "Column 3"]
            });
            console.log(jsonob);
            filename = jsonob[1]["Column 3"];
            patient = addPatient(jsonob);
            wb = XLSX.utils.book_new();
            var ws_data = downloadExcel(patient, myjson);
            wb.Props = {
              Title: "Report Generator",
              Subject: "Patient",
              Author: "ATU",
              CreatedDate: new Date(y, m, d)
            };

            wb.SheetNames.push("Test Sheet");
            ws = XLSX.utils.aoa_to_sheet(ws_data, {
              headers: [
                "Column 1",
                "Column 2",
                "Column 3",
                "Column 4",
                "Column 5",
                "Column 6"
              ]
            });
            wb.Sheets["Test Sheet"] = ws;
            var wbout = XLSX.write(wb, { bookType: "xlsx", type: "binary" });

            function s2ab(s) {
              var buf = new ArrayBuffer(s.length);
              var view = new Uint8Array(buf);
              for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xff;
              return buf;
            }
            saveAs(
              new Blob([s2ab(wbout)], { type: "application/octet-stream" }),
              filename + ".xlsx"
            );
          };
        });

        $("#input-excel").change(function(e) {
          var reader = new FileReader();
          reader.readAsArrayBuffer(e.target.files[0]);
          reader.onload = function(e) {
            var data = new Uint8Array(reader.result);
            var wb = XLSX.read(data, { type: "array" });
            var sheet_name_list = wb.SheetNames;
            var ws = wb.Sheets[sheet_name_list[0]];
            var jsonob = XLSX.utils.sheet_to_json(ws, {
              range: 0,
              header: [
                "Column 1",
                "Column 2",
                "Column 3",
                "Column 4",
                "Column 5",
                "Column 6"
              ]
            });

            console.log(jsonob);
            function extractCodes(ob) {
              return ob["Column 1"] == 1;
            }

            val = jsonob.filter(extractCodes);
            console.log(val)
            description = "description";
            var clientName = jsonob[1]["Column 3"];
            var clientAddress = jsonob[2]["Column 3"];
            var telephone = jsonob[3]["Column 3"];
            var dateOfService = jsonob[4]["Column 3"];
            var typeOfService = jsonob[5]["Column 3"];
            var clinicianName = jsonob[6]["Column 3"];
            var clinicianTitle = jsonob[5]["Column 3"];
            
            filename = clientName;
            var patient_details =
              "Client:\t" +
              clientName +
              "\n" +
              "Address:\t" +
              clientAddress +
              "\n" +
              "Telephone:\t" +
              telephone +
              "\n" +
              "Date Of Service:\t" +
              dateOfService +
              "\n" +
              "Type of Service:\t" +
              typeOfService +
              "\n" +
              "Clinician:\t" +
              clinicianName +
              "\n\t\t" +
              clinicianTitle;
             $("#text").val("\t\t\tUniversity Of Illinois at Chicgao \n \t\t\tDisability and Human Development \n \t\t\tAssistive Technology Unit\n\n\n" + patient_details);
            
            for (var i = 0; i <= val.length; i++) {
              var content = $("#text").val();
              var newcontent = content + "\n" + myjson[val[i]["Column 2"]].description;
              //Pavan
              if (val[i]["Column 3"] != null) {
                newcontent += "\n" + val[i]["Column 3"];
              }
           //   if (val[i]["Column 4"] != null) {
             //   newcontent += "\n" + val[i]["Column 4"];
              //}
              if (val[i]["Column 5"] != null) {
                newcontent += "\n" + val[i]["Column 5"] + "\t:\t" + val[i]["Column 6"];
              }
              $("#text").val("\n\n"+newcontent);
            } 
          };
        });

        window.export.onclick = function() {
          var html, link, blob, url, css;
          // EU A4 use: size: 841.95pt 595.35pt;
          // US Letter use: size:11.0in 8.5in;
          html = document.getElementById("text").value;
          blob = new Blob(["\ufeff", html], {
            type: "application/msword"
          });
          url = URL.createObjectURL(blob);
          link = document.createElement("A");
          link.href = url;
          // Set default file name.
          // Word will append file extension - do not add an extension here.
          link.download = filename;
          document.body.appendChild(link);
          if (navigator.msSaveOrOpenBlob) console.log("clicked");
          // navigator.msSaveOrOpenBlob(blob, "Document.docx");
          // IE10-11
          else link.click(); // other browsers
          document.body.removeChild(link);
        };
    </script>
</asp:Content>