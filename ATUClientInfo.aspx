<%@ Page Language="C#" Title="ATUClientInfo" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ATUClientInfo.aspx.cs" Inherits="ATUClient.ATUClientInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%--<link href="https://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.min.css" rel="stylesheet" type="text/css" />
<script src="https://code.jquery.com/jquery-1.11.0.min.js"></script>
<script src="https://code.jquery.com/ui/1.10.4/jquery-ui.min.js"></script>--%>
    <link href="scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />

    <script src="scripts/jquery-1.11.0.min.js" type="text/javascript"></script>

    <script src="scripts/jquery-ui.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        // Current option
        var currentString = "";
        var tempString = "";

        $(function() {
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
                        $("[id*=DisabilityCheckBoxList] input:checked").each(function() {
                            tempString = $(this).next().text().trim() + "\n";
                            currentString += tempString;
                        });
                        $("#<%= DisabilityTextBox.ClientID%>").val(currentString);
                        $("#<%= DisabilityHiddenField.ClientID%>").val(currentString);
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
        });
    </script>

    <style type="text/css">
        .style1
        {
            width: 100%;
        }
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
            height: 15px;
        }
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
    <table class="style1">
        <tr>
            <td>
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
            </td>
        </tr>
        <tr>
            <td>
                <div id="sidebar">
                    <table style="width: 99%">
                        <tr>
                            <td class="style4" style="text-align: left; width: 30%;">
                                <b>Client Search</b>
                            </td>
                            <td style="text-align: left; width: 30%;">
                                &nbsp;
                            </td>
                            <td style="text-align: leftwidth: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                Last Name :
                            </td>
                            <td style="width: 30%">
                                First Name :
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                <asp:TextBox ID="LastNameText" Width="97%" runat="server" MaxLength="50"></asp:TextBox>
                                <br />
                            </td>
                            <td style="width: 30%">
                                <asp:TextBox ID="FirstNameText" Width="97%" runat="server" MaxLength="50"></asp:TextBox>
                                <br />
                            </td>
                            <td style="width: 40%">
                                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"
                                    CausesValidation="false" Width="49%"></asp:Button>
                                <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"
                                    CausesValidation="false" Width="49%"></asp:Button>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                &nbsp;
                                <asp:RequiredFieldValidator ID="LastNameRequiredFieldValidator" ControlToValidate="LastNameText"
                                    ValidationGroup="PersonalInfoGroup" ErrorMessage="Enter last name." runat="Server"
                                    EnableClientScript="false">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%">
                                &nbsp;
                                <asp:RequiredFieldValidator ID="FirstNameRequiredfieldvalidator" ControlToValidate="FirstNameText"
                                    ValidationGroup="PersonalInfoGroup" ErrorMessage="Enter first name." runat="Server"
                                    EnableClientScript="false">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Panel ID="updatepanel_Section1" runat="server">
                                   <%-- <ContentTemplate>--%>
                                        <table class="style1" style="border: thin solid #C0C0C0;">
                                            <tr>
                                                <td colspan="2">
                                                    <table style="width: 100%; border: thin solid #CCCCCC" 
                                                        bgcolor="#FFCC66">
                                                        <tr>
                                                            <td colspan="1">
                                                                <asp:Label ID="GeneralLabel" runat="server" BackColor="Wheat" Style="font-weight: 700;
                                                                    font-style: italic; color: Gray; font-size: Medium; text-align: left; vertical-align: middle"
                                                                    Text="General Info" Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lbl_SectionNumber1" runat="server" BackColor="Wheat" Style="font-weight: 700;
                                                                    font-style: italic; color: Gray; font-size: Small; text-align: right; vertical-align: middle"
                                                                    Text="Section 1 of 4" Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <%--SSN--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="SSNLabel" runat="server" BorderStyle="None" Text="SSN :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="SSNValueTextBox" runat="server" CausesValidation="True" MaxLength="3"
                                                        Visible="True" Width="35"></asp:TextBox>
                                                    <asp:Label ID="Label1" runat="server" BorderStyle="None" Text="-"></asp:Label>
                                                    <asp:TextBox ID="SSNValueTextBox1" runat="server" CausesValidation="True" MaxLength="2"
                                                        Visible="True" Width="25"></asp:TextBox>
                                                    <asp:Label ID="Label2" runat="server" BorderStyle="None" Text="-"></asp:Label>
                                                    <asp:TextBox ID="SSNValueTextBox2" runat="server" CausesValidation="True" MaxLength="4"
                                                        Visible="True" Width="60"></asp:TextBox>
                                                    <asp:Label ID="SSNFormatLabel" runat="server" BorderStyle="None" ForeColor="Red"
                                                        Text="(###-##-####)" Visible="False" Font-Size="Small"></asp:Label>
                                                    <%--<asp:RegularExpressionValidator ID="SSNValidator"
                                                    runat="server"
                                                    ErrorMessage="(###-##-####)"
                                                    ValidationExpression="^\d{3}-\d{2}-\d{4}$"
                                                    ControlToValidate="SSNValueTextBox"
                                                    validationgroup="PersonalInfoGroup" EnableClientScript="false">
                                                        </asp:RegularExpressionValidator>--%>
                                                </td>
                                            </tr>
                                            <%--Date of Birth--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="DOBLabel" runat="server" BorderStyle="None" Text="Date of Birth :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="Calendar1" EventName="SelectionChanged" />
                                                        </Triggers>
                                                        <ContentTemplate>
                                                            <asp:TextBox ID="DOBTextBox" Width="150" runat="server" CausesValidation="true"></asp:TextBox>
                                                            <asp:Button ID="Button1" runat="server" Text="..." OnClick="Calender_Click" />
                                                            <asp:Label ID="DOBValidationLabel" runat="server" BorderStyle="None" Text="Invalid Date Format"
                                                                Visible="False" ForeColor="Red" Font-Size="Small"></asp:Label>
                                                            <asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Calendar1_SelectionChanged"
                                                                BackColor="White" BorderColor="Black" BorderStyle="Solid" CellSpacing="1" Font-Names="Verdana"
                                                                Font-Size="9pt" ForeColor="Black" Height="50px" NextPrevFormat="ShortMonth" Visible="false"
                                                                Width="100px">
                                                                <DayHeaderStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" Height="8pt" />
                                                                <DayStyle BackColor="#CCCCCC" />
                                                                <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="White" />
                                                                <OtherMonthDayStyle ForeColor="#999999" />
                                                                <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                                                                <TitleStyle BackColor="#333399" BorderStyle="Solid" Font-Bold="True" Font-Size="12pt"
                                                                    ForeColor="White" Height="12pt" />
                                                                <TodayDayStyle BackColor="#999999" ForeColor="White" />
                                                            </asp:Calendar>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                    <%--<asp:TextBox ID="DOBTextBox" Width="150" runat="server" CausesValidation="true"></asp:TextBox>--%>
                                                    <%--<asp:Label ID="DateFormatLabel" runat="server" BorderStyle="None" Text=" (mm/dd/yyyy)"></asp:Label>--%>
                                                    <%--<asp:RegularExpressionValidator ID="DOBValidator"
                                                                            runat="server"
                                                                            ErrorMessage="(dd/mm/yyyy)"
                                                                            ValidationExpression="/^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/"
                                                                            ControlToValidate="DOBTextBox">
                                                    </asp:RegularExpressionValidator>--%>
                                                    <%--<asp:CustomValidator ID="DOBCustomValidator" ValidationGroup="PersonalInfoGroup" runat="server" ControlToValidate="DOBTextBox" ErrorMessage="Date is in incorrect format" OnServerValidate="DOBCustomValidator_Validate" />--%>
                                                </td>
                                            </tr>
                                            <%--Gender--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="GenderLabel" runat="server" BorderStyle="None" Text="Gender :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="GenderDropDownList" Width="150" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <%--Race--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="RaceLabel" runat="server" BorderStyle="None" Text="Race :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="RaceDropDownList" Width="150" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <%--Ethnicity--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="EthnicityLabel" runat="server" BorderStyle="None" Text="Ethnicity :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="EthnicityDropDownList" Width="150" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <%--Mobility Code--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="MobilityLabel" runat="server" BorderStyle="None" Text="Mobility Code :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="MobilityDropDownList" Width="150" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <%--Disability--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="DisabilityLabel" runat="server" BorderStyle="None" Text="Disability :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="DisabilityTextBox" runat="server" TextMode="MultiLine" Visible="true"
                                                        Width="320" ReadOnly="true">
                                                    </asp:TextBox>
                                                    <asp:HiddenField ID="DisabilityHiddenField" runat="server" />
                                                    <br />
                                                    <input class='modal' type="button" value="Disabilities" id="DisabilityButton" runat="server" /><br />
                                                    <br />
                                                    <div id="dialog" title="Disability">
                                                        <form>
                                                        <b>Select Disabilities :</b><br />
                                                        <asp:CheckBoxList ID="DisabilityCheckBoxList" runat="server" RepeatDirection="Vertical">
                                                        </asp:CheckBoxList>
                                                        </form>
                                                    </div>
                                                    <%--<asp:ListBox ID="DisabilityListBox" runat="server" SelectionMode="Multiple" Width="150"></asp:ListBox>--%>
                                                    <%--<br /><br />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    <%--</ContentTemplate>--%>
                                </asp:Panel>
                                <asp:UpdatePanel ID="updatepanel_Section2" runat="server">
                                    <ContentTemplate>
                                        <table class="style1" style="border: thin solid #C0C0C0">
                                            <tr>
                                                <td colspan="2">
                                                    <table style="width: 100%; border: thin solid #CCCCCC"
                                                    bgcolor="#FFCC66">
                                                        <tr>
                                                            <td colspan="1">
                                                                <asp:Label ID="AddressLabel" runat="server" Style="font-weight: 700; font-style: italic;
                                                                    color: Gray; font-size: Medium; text-align: left; vertical-align: middle" Text="Address"
                                                                    Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lbl_SectionNumber2" runat="server" Style="font-weight: 700; font-style: italic;
                                                                    color: Gray; font-size: Small; text-align: right; vertical-align: middle" Text="Section 2 of 4"
                                                                    Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <%--Address Information--%>
                                            <%--Street --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="StreetLabel" runat="server" BorderStyle="None" Text="Street :" 
                                                        Font-Bold="False"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="StreetTextBox" Width="150" runat="server" Font-Size="Small"></asp:TextBox>
                                                    <asp:Label ID="lbl_S2StreetVal" runat="server" Font-Bold="True" Font-Size="Large"
                                                        ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <%--City --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="CityLabel" runat="server" BorderStyle="None" Text="City :" 
                                                        Font-Bold="False"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="CityTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                    <asp:Label ID="lbl_S2CityVal" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="Red"
                                                        Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <%--State --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="StateLabel" runat="server" BorderStyle="None" Text="State :" 
                                                        Font-Bold="False"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="StateDropDownList" Width="156px" runat="server">
                                                    </asp:DropDownList>
                                                    <asp:Label ID="lbl_S2StateVal" runat="server" Font-Bold="True" Font-Size="Large"
                                                        ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <%--ZipCode --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="ZipLabel" runat="server" BorderStyle="None" Text="Zip Code :" 
                                                        Font-Bold="False"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="ZipTextBox" Width="150" runat="server"></asp:TextBox>
                                                    <asp:Label ID="lbl_S2ZipVal" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="Red"
                                                        Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <%--IL Senate District # --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="SenateLabel" runat="server" BorderStyle="None" Text="IL Senate District # :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="SenateTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--IL House District # --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="HouseLabel" runat="server" BorderStyle="None" Text="IL House District # :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="HouseTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--GeoCode --%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="GeoCodeLabel" runat="server" BorderStyle="None" Text="GEO Code :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="GeoCodeTextBox" Width="150" MaxLength="5" runat="server">
                                                    </asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="GeoCodeValidator" runat="server" EnableClientScript="false"
                                                        ErrorMessage="Only 5 digits" ValidationExpression="^\d{5}$" 
                                                        ControlToValidate="GeoCodeTextBox" Font-Size="Small"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="updatepanel_Section3" runat="server">
                                    <ContentTemplate>
                                        <table class="style1" style="border: thin solid #C0C0C0">
                                            <tr>
                                                <td colspan="2">
                                                    <table style="width: 100%; border: thin solid #CCCCCC"
                                                    bgcolor="#FFCC66">
                                                        <tr>
                                                            <td colspan="1">
                                                                <asp:Label ID="FContactLabel" runat="server" Style="width: 100%; font-weight: 900;
                                                                    font-style: italic; color: Gray; font-size: medium; text-align: left; vertical-align: middle"
                                                                    Text="Facility Info" Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lbl_SectionNumber3" runat="server" Style="font-weight: 700; font-style: italic;
                                                                    color: Gray; font-size: Small; text-align: right; vertical-align: middle" Text="Section 3 of 4"
                                                                    Width="100%">
                                                                </asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <%--Facility Contact Information--%>
                                            <%--Facility Name--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FNameLabel" runat="server" BorderStyle="None" Text="Name :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <%-- Future Work Daniel 3/10/16 --%><asp:TextBox ID="FNameTextBox" Width="150" runat="server"></asp:TextBox>
                                                    <%-- <asp:CheckBox ID="ChkBoxAddNewFName" runat="server" Text="Add New Facility" AutoPostBack="true"
                                                        OnCheckedChanged="ChkBoxAddNewFName_CheckedChanged" /> --%>
                                                </td>
                                            </tr>
                                            <%--Facility Contact Name--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FacilityStreetLabel" runat="server" BorderStyle="None" Font-Bold="False"
                                                        Text="Street :"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FacilityStreetTextBox" runat="server" Width="150"></asp:TextBox>
                                                    <asp:Label ID="lbl_S3StreetVal" runat="server" Font-Bold="True" Font-Size="Large"
                                                        ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FacilityCityLabel" runat="server" BorderStyle="None" Font-Bold="False"
                                                        Text="City :"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FacilityCityTextBox" runat="server" Width="150"></asp:TextBox>
                                                    <asp:Label ID="lbl_S3CityVal" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="Red"
                                                        Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FacilityStateLabel" runat="server" BorderStyle="None" Font-Bold="False"
                                                        Text="State :"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:DropDownList ID="FacilityStateDropDownList" runat="server" Width="156px">
                                                    </asp:DropDownList>
                                                    <asp:Label ID="lbl_S3StateVal" runat="server" Font-Bold="True" Font-Size="Large"
                                                        ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FacilityZipLabel" runat="server" BorderStyle="None" Font-Bold="False"
                                                        Text="Zip Code :"></asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FacilityZipTextBox" runat="server" Width="150"></asp:TextBox>
                                                    <asp:Label ID="lbl_S3ZipVal" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="Red"
                                                        Text="*"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FCNameLabel" runat="server" BorderStyle="None" Text="Contact Name :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FCNameTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--Facility Phone--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FPhoneLabel" runat="server" BorderStyle="None" Text="Phone # :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FPhoneTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--Facility Fax--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FFaxLabel" runat="server" BorderStyle="None" Text="Fax # :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FFaxTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--Facility Email--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="FEmailLabel" runat="server" BorderStyle="None" Text="E-Mail :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="FEmailTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--Facility TTY--%>
                                            <tr>
                                                <td style="width: 30%">
                                                    <asp:Label ID="TTYLabel" runat="server" BorderStyle="None" Text="TTY # :">
                                                    </asp:Label>
                                                </td>
                                                <td style="width: 70%">
                                                    <asp:TextBox ID="TTYTextBox" Width="150" runat="server">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Panel ID="panel_Section4" runat="server" Width="100%">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:UpdatePanel ID="updatepanel_Section4" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <table class="style1" style="border: thin solid #C0C0C0">
                                                            <tr>
                                                                <td colspan="3">
                                                                    <table style="width: 100%; border: thin solid #CCCCCC"
                                                                    bgcolor="#FFCC66">
                                                                        <tr>
                                                                            <td colspan="1">
                                                                                <asp:Label ID="ContactLabel" runat="server" BackColor="Wheat" Style="font-weight: 700;
                                                                                    font-style: italic; color: Gray; font-size: Medium; text-align: left; vertical-align: middle"
                                                                                    Text="Contact Info" Width="100%">
                                                                                </asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="lbl_SectionNumber4" runat="server" BackColor="Wheat" Style="font-weight: 700;
                                                                                    font-style: italic; color: Gray; font-size: Small; text-align: right; vertical-align: middle"
                                                                                    Text="Section 4 of 4" Width="100%">
                                                                                </asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <%--Contact Information--%>
                                                            <%--Relationship--%>
                                                            <tr>
                                                                <td style="width: 30%">
                                                                    <asp:Label ID="RelationshipLabel" runat="server" BorderStyle="None" Text="Relationship :"
                                                                        Font-Bold="true">
                                                                    </asp:Label>
                                                                </td>
                                                                <td style="width: 70%" colspan="2">
                                                                    <asp:DropDownList ID="RelationshipDropDownList" Width="156px" runat="server" AutoPostBack="true"
                                                                        OnSelectedIndexChanged="Relationship_Changed">
                                                                    </asp:DropDownList>
                                                                    <asp:Label ID="lbl_S4RelationshipVal" runat="server" Font-Bold="True" Font-Size="Large"
                                                                        ForeColor="Red" Text="*"></asp:Label>
                                                                    <%-- <asp:requiredfieldvalidator id="RelationRequiredfieldvalidator"
                                                          controltovalidate="RelationshipDropDownList"
                                                          validationgroup="PersonalInfoGroup"
                                                          errormessage="Select relation." InitialValue="-1"
                                                          runat="Server"
                                                          EnableClientScript="false">
                                                        </asp:requiredfieldvalidator> --%>
                                                                </td>
                                                            </tr>
                                                            <%--Name--%>
                                                            <tr>
                                                                <td style="width: 30%">
                                                                    <asp:Label ID="RNameLabel" runat="server" BorderStyle="None" Text="Name :">
                                                                    </asp:Label>
                                                                </td>
                                                                <td style="width: 70%" colspan="2">
                                                                    <asp:TextBox ID="RNameTextBox" Width="150" runat="server">
                                                                    </asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <%--Home Phone--%>
                                                            <tr>
                                                                <td class="style5">
                                                                    <asp:Label ID="HPhoneLabel" runat="server" BorderStyle="None" Text="Home Phone :"
                                                                        Font-Bold="true">
                                                                    </asp:Label>
                                                                </td>
                                                                <td width="20%">
                                                                    <asp:TextBox ID="HPhoneTextBox" Width="150" runat="server"></asp:TextBox>
                                                                    <%--<asp:requiredfieldvalidator id="HPhoneRequiredfieldvalidator"
                                                          controltovalidate="HPhoneTextBox"
                                                          validationgroup="PersonalInfoGroup"
                                                          errormessage="Enter phone#."
                                                          runat="Server" EnableClientScript="false">
                                                        </asp:requiredfieldvalidator> --%>
                                                                </td>
                                                                <td rowspan="2" style="width: 50%">
                                                                    <asp:Label ID="lbl_S4PhoneVal" runat="server" Font-Bold="False" Font-Size="Small"
                                                                        ForeColor="Red"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <%--Work Phone--%>
                                                            <tr>
                                                                <td class="style5">
                                                                    <asp:Label ID="CellPhoneLabel" runat="server" BorderStyle="None" Font-Bold="True"
                                                                        Text="Cell Phone :"></asp:Label>
                                                                </td>
                                                                <td width="20%">
                                                                    <asp:TextBox ID="CellPhoneTextBox" runat="server" Width="150"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 30%">
                                                                    <asp:Label ID="WPhoneLabel" runat="server" BorderStyle="None" Text="Work Phone :">
                                                                    </asp:Label>
                                                                </td>
                                                                <td style="width: 70%" colspan="2">
                                                                    <asp:TextBox ID="WPhoneTextBox" Width="150" runat="server">
                                                                    </asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <%--Email Address--%>
                                                            <tr>
                                                                <td style="width: 30%">
                                                                    <asp:Label ID="EmailLabel" runat="server" BorderStyle="None" Text="Email :">
                                                                    </asp:Label>
                                                                </td>
                                                                <td style="width: 70%" colspan="2">
                                                                    <asp:TextBox ID="EmailTextBox" Width="150" runat="server">
                                                                    </asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <%--Save Contact Button--%>
                                        <tr>
                                            <td>
                                                <asp:Button ID="SaveContactButton" runat="server" Text="Save Contact" OnClick="SaveContact_Click"
                                                    ValidationGroup="PersonalInfoGroup1"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <%--Save Contact Button--%>
                        <tr>
                            <td colspan="3" width="50%" style="text-align: center">
                                <asp:Button ID="btn_PreviousSection" runat="server" Text="&lt;&lt; Previous" OnClick="btn_PreviousSection_Click">
                                </asp:Button>
                                <asp:Button ID="btn_NextSection" runat="server" Text="Next &gt;&gt;" OnClick="btn_NextSection_Click">
                                </asp:Button>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" 
                                style="border-color: #000000; text-align: center; border-bottom-style: ridge; border-bottom-width: medium;">
                                <asp:Label ID="lbl_Validation" runat="server" BorderStyle="None" ForeColor="Red"
                                    Font-Size="Small">Please fill the fields marked asterisk (*)</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" style="text-align: center" height="7px">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" style="text-align: center">
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="Save_Click" ValidationGroup="PersonalInfoGroup"
                    CausesValidation="true" Width="125px" />
                &nbsp;<asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="Continue_Click" 
                                    Width="125px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" style="text-align: center">
                <asp:UpdatePanel ID="updatepanel_ErrorMessageRow" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Label ID="MessageLabel" runat="server" BorderStyle="None" Text="Could be a duplicate record. Click save again to save this record."
                            ForeColor="Red"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" 
                                style="border-width: thin; border-color: #000000; text-align: center; border-bottom-style: double;">
                <asp:UpdatePanel ID="updatepanel_ResponseLabelRow" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Label ID="lbl_Response" runat="server" BorderStyle="None" ForeColor="Green">
                        </asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="main-content">
                    <table class="style1">
                        <%--Save Contact Button--%>
                        <tr>
                            <td>
                                <asp:Label ID="CurrentClientLabel" runat="server" BorderStyle="None" Text="Current Client: "
                                    Font-Bold="true" ForeColor="Red"></asp:Label>
                                <asp:Label ID="CurrentClientValueLabel" runat="server" BorderStyle="None" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="GridView1" runat="server" AllowSorting="true" OnSorting="Sorting_gridView"
                                    AutoGenerateSelectButton="true" AllowPaging="true" OnPageIndexChanging="grdSearch_Changing"
                                    OnRowCommand="gridSelect_Command" PageSize="20">
                                </asp:GridView>
                                <asp:GridView ID="GridView2" runat="server" Visible="false">
                                </asp:GridView>
                                <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                    Text="No search results found" Visible="false">
                                </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="ClientInfoGridView" ShowHeader="false" GridLines="Horizontal" CellSpacing="10"
                                    CellPadding="2" runat="server" OnRowDataBound="GridView_RowDataBound" 
                                    Font-Size="Small" Width="80%">
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <%--Save Contact Button--%>
        <tr align="center">
            <td align="center" width="100%">
                &nbsp;</td>
        </tr>
        <%--Save Contact Button--%>
    </table>
</asp:Content>
