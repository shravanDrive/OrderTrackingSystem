<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="ATUClient.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<link href="scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />
<script src="scripts/jquery-1.11.0.min.js" type="text/javascript"></script>
<script src="scripts/jquery-ui.min.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
    // Current option
    var currentString = "";
    var tempString = "";

    $(function(){
        //////////////////////////////////////////////
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
                    $("[id*=DatesCheckBoxList] input:checked").each(function() {
                        tempString = $(this).next().text().trim() + "\n";
                        currentString += tempString;
                    });
                    $("#<%= DatesTextBox.ClientID%>").val(currentString);
                    $("#<%= DatesHiddenField.ClientID%>").val(currentString);
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////
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
                        clinicianCurrentString += tempString;
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

    <asp:Table ID="ReportsTable" runat="server" HorizontalAlign="Left" >
      <asp:TableRow ID="atuReportRow" runat="server">
      <asp:TableCell ID="atuReportCell1" runat="server" HorizontalAlign="Left" VerticalAlign="Top" Width="1000">
         <%--<h1>This Page <br/> <B> UNDER </B> <br/> <B>CONSTRUCTION </B> <br/> Check Back Soon</h1>--%>
         <br />

         <asp:Table ID="ReportsInfoTable" runat="server" Font-Names="Verdana"  HorizontalAlign="Left" 
            Font-Size="11" ForeColor="#333333" Font-Bold="True" Visible="true">

             <asp:TableRow ID="ReportsTypeRow" runat="server">
                <asp:TableCell ID="ReportsTypeLabelCell" runat="server" >
                    <asp:label ID="ReportTypeLabel" runat="server" Text="Report Type: "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="ReportsTypeDropDownCell" runat="server" >
                    <asp:DropDownList ID="ReportDropDownList" runat="server" Width="150" OnSelectedIndexChanged="Report_Selected" AutoPostBack="true">
                    </asp:DropDownList>
                     <asp:Label ID="ReportTypeValidationLabel" runat="server" BorderStyle="None" Text="Choose Report" Visible="false" ForeColor="Red"></asp:Label>
                </asp:TableCell>
             </asp:TableRow>
             
             <asp:TableRow ID="StartDateRow" runat="server">
                <asp:TableCell ID="StartDateCell" runat="server" >
                    <asp:label ID="StartDateLabel" runat="server" Text="Start Date: "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="StartDateCellCalendar" runat="server">
                    <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers> 
                    <asp:AsyncPostBackTrigger ControlID="StartDateCalendar" EventName="SelectionChanged" />
                    </Triggers>
                    <ContentTemplate>
                    <asp:TextBox ID="StartDateTextBox" Width="150" runat="server" CausesValidation="true"></asp:TextBox>
                    <asp:Button ID="StartDateButton" runat="server" Text="..." OnClick="StartDateCalender_Click"/>
                    <asp:Label ID="StartDateValidationLabel" runat="server" BorderStyle="None" Text="Invalid Date Format" Visible="false" ForeColor="Red"></asp:Label>
                    <asp:Calendar ID="StartDateCalendar" runat="server" 
                            OnSelectionChanged="StartDateCalendar_SelectionChanged" BackColor="White" 
                            BorderColor="Black" BorderStyle="Solid" CellSpacing="1" Font-Names="Verdana" 
                            Font-Size="9pt" ForeColor="Black" Height="50px" NextPrevFormat="ShortMonth" Visible="false"
                            Width="100px">
                        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" 
                            Height="8pt" />
                        <DayStyle BackColor="#CCCCCC" />
                        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="White" />
                        <OtherMonthDayStyle ForeColor="#999999" />
                        <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                        <TitleStyle BackColor="#333399" BorderStyle="Solid" Font-Bold="True" 
                            Font-Size="12pt" ForeColor="White" Height="12pt" />
                        <TodayDayStyle BackColor="#999999" ForeColor="White" />
                        </asp:Calendar>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                
                    <br />
                </asp:TableCell>
             </asp:TableRow>
             
             <asp:TableRow ID="EndDateRow" runat="server">
                <asp:TableCell ID="EndDateCell" runat="server" >
                    <asp:label ID="EndDateLabel" runat="server" Text="End Date: "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="EndDateCellCalendar" runat="server">
                    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <%--<Triggers> 
                    <asp:AsyncPostBackTrigger ControlID="EndDateCalendar" EventName="SelectionChanged" />
                    </Triggers>--%>
                    <ContentTemplate>
                    <asp:TextBox ID="EndDateTextBox" Width="150" runat="server" CausesValidation="true"></asp:TextBox>
                    <asp:Button ID="EndDateButton" runat="server" Text="..." OnClick="EndDateCalender_Click"/>
                    <asp:Label ID="EndDateValidationLabel" runat="server" BorderStyle="None" Text="Invalid Date Format" Visible="false" ForeColor="Red"></asp:Label>
                    <asp:Calendar ID="EndDateCalendar" runat="server" 
                            OnSelectionChanged="EndDateCalendar_SelectionChanged" BackColor="White" 
                            BorderColor="Black" BorderStyle="Solid" CellSpacing="1" Font-Names="Verdana" 
                            Font-Size="9pt" ForeColor="Black" Height="50px" NextPrevFormat="ShortMonth" Visible="false"
                            Width="100px">
                        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" 
                            Height="8pt" />
                        <DayStyle BackColor="#CCCCCC" />
                        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="White" />
                        <OtherMonthDayStyle ForeColor="#999999" />
                        <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                        <TitleStyle BackColor="#333399" BorderStyle="Solid" Font-Bold="True" 
                            Font-Size="12pt" ForeColor="White" Height="12pt" />
                        <TodayDayStyle BackColor="#999999" ForeColor="White" />
                        </asp:Calendar>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                
                    <br />
                </asp:TableCell>
             </asp:TableRow>
             
             <asp:TableRow ID="DatesRow" runat="server">
                <asp:TableCell ID="DatesCell" runat="server" >
                    <asp:label ID="DatesLabel" runat="server" Text="Date Columns: "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="DatesValueCell">
                    <asp:TextBox ID="DatesTextBox" runat="server" TextMode="MultiLine" Visible="true" ReadOnly="true" Width="300px"></asp:TextBox>
                    <asp:HiddenField ID="DatesHiddenField" runat="server" />
                    <asp:Label ID="DatesColumnValidation" runat="server" BorderStyle="None" Text="Choose dates." Visible="false" ForeColor="Red"></asp:Label> 
                    <input class='modal' type="button" value="Select Dates" id="DatesButton" runat="server" /><br /><br />
                     <div id="dialog" title="DatesColumns">
                      <form>
                        <b> Select Dates :</b><br />
                        <asp:CheckBoxList ID="DatesCheckBoxList" runat="server" RepeatDirection="Vertical"  >
                                        </asp:CheckBoxList><br /><br />
                      </form>
                      </div>
                     <%--asp:Label ID="DatesColumnValidation" runat="server" BorderStyle="None" Text="Choose dates." Visible="false" ForeColor="Red"></asp:Label--%> 
                </asp:TableCell>
             </asp:TableRow>

             <asp:TableRow ID="CidRow" runat="server">
                <asp:TableCell ID="CidCell" runat="server" >
                    <asp:label ID="CidLabel" runat="server" Text="Client Id (Enter for Equipment Per Client Report): "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="CidValueCell">
                    <asp:TextBox ID="CidTextBox" runat="server" Visible="true" Width="100px"></asp:TextBox>
                    <asp:HiddenField ID="CidHiddenField" runat="server" />
                    <asp:Label ID="CidColumnValidation" runat="server" BorderStyle="None" Text="Enter Client id" Visible="false" ForeColor="Red"></asp:Label> 
                     <%--asp:Label ID="CidColumnValidation" runat="server" BorderStyle="None" Text="Choose dates." Visible="false" ForeColor="Red"></asp:Label--%> 
                </asp:TableCell>
             </asp:TableRow>

             <asp:TableRow ID="YearRow" runat="server">
                <asp:TableCell ID="YearCell" runat="server" >
                    <asp:label ID="YearLabel" runat="server" Text="Year (Enter for Referral Counts Per Month Report): "></asp:label>
                </asp:TableCell>
                <asp:TableCell ID="YearValueCell">
                    <asp:TextBox ID="YearTextBox" runat="server" Visible="true" Width="100px"></asp:TextBox>
                    <asp:HiddenField ID="YearHiddenField" runat="server" />
                    <asp:Label ID="YearColumnValidation" runat="server" BorderStyle="None" Text="Enter Year" Visible="false" ForeColor="Red"></asp:Label> 
                     <%--asp:Label ID="YearColumnValidation" runat="server" BorderStyle="None" Text="Choose dates." Visible="false" ForeColor="Red"></asp:Label--%> 
                </asp:TableCell>
             </asp:TableRow>
             
             <%--Referral Source--%>
                    <asp:TableRow ID="RSourceRow" runat="server">
                        <asp:TableCell ID="RSourceCell" runat="server">
                            <asp:Label ID="RSourceLabel" runat="server" BorderStyle="None" Text="Referral Source (Enter for Referral Counts Per Month Report):"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="RSourceValueCell" runat="server">
                            <asp:DropDownList ID="RSourceDropDownList" Width="150" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
             
             <%--ATU Clinician--%>
                <asp:TableRow ID="ClinicianRow" runat="server">
                 <asp:TableCell ID="ClinicianLabelCell" runat="server">
                     <asp:Label ID="ClinicianLabel" runat="server" Font-Bold="true" Text="ATU Clinician (s): "></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="ClinicianCell" runat="server">
                    <asp:TextBox ID="ClinicianTextBox" runat="server" TextMode="MultiLine" Visible="true" Width="150" ReadOnly="true" ></asp:TextBox>
                    <asp:HiddenField ID="ClinicianHiddenField" runat="server" />
                    <input class='modal1' type="button" value="Clinicians" id="ClinicianButton" runat="server" /><br /><br />
                     <div id="dialog1" title="Clinicians">
                      <form>
                        <b> Select Clinicians :</b><br />
                        <asp:CheckBoxList ID="CliniciansCheckBoxList" runat="server" RepeatDirection="Vertical"  >
                                        </asp:CheckBoxList><br /><br />
                      </form>
                      </div>
                </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="SelectAllRow" runat="server">
                    <asp:TableCell ID="SelectAllCell" runat="server">
                        <asp:CheckBox ID="SelectAllCheckbox" runat="server" Text="Select All (Clinicians)" OnCheckedChanged="OnSelectAll_Checked" AutoPostBack="true"/>
                    </asp:TableCell>
                </asp:TableRow>
             
             <asp:TableRow ID="ReportButtonRow" runat="server">
                <asp:TableCell ID="ReportButtonCell" runat="server">
                    <br /><asp:Button ID="ReportButton" runat="server" Text="Generate Report" OnClick="GenerateReport_Click" ValidationGroup="PersonalInfoGroup" CausesValidation="true"/>
                </asp:TableCell>
             </asp:TableRow>
        </asp:Table>
        </asp:TableCell>
        <asp:TableCell ID="atuReportCell2" runat="server" >
            <asp:GridView ID="ReportGridView" ShowHeader="true" GridLines="Horizontal" CellSpacing="10" CellPadding="5" runat="server">
            </asp:GridView>   
        </asp:TableCell>  
    </asp:TableRow> 
    </asp:Table>
     
</asp:Content>
