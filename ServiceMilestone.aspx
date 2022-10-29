<%@ Page Title="Service" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ServiceMilestone.aspx.cs" Inherits="ATUClient.ServiceMilestone" %>

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
        .style7
        {
            width: 30%;
            height: 22px;
        }
        .style8
        {
            width: 40%;
            height: 22px;
        }
        .hiddencol
        {
            display: none;
        }
    </style>
    <%--<link href="https://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.min.css" rel="stylesheet" type="text/css" />
<script src="https://code.jquery.com/jquery-1.11.0.min.js"></script>
<script src="https://code.jquery.com/ui/1.10.4/jquery-ui.min.js"></script>--%>
    <link href="scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />

    <script src="scripts/jquery-1.11.0.min.js" type="text/javascript"></script>

    <script src="scripts/jquery-ui.min.js" type="text/javascript"></script>

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
                        $("[id*=CPTCodesCheckBoxList] input:checked").each(function() {
                            tempString = $(this).next().text() + "\n";
                            tempString = tempString.substr(0, 5);
                            currentString += tempString + "\n";
                        });
                        $("#<%= CPTCodeListTextBox.ClientID%>").val(currentString);
                        $("#<%= HiddenCPTCodes.ClientID%>").val(currentString);
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
        <div class="alert alert-danger alert-dismissable fade in" style="display: none;"
            id="alertUpdateFailed">            
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a> <strong>
                Update Failed</strong> Please contact the Admin for Assistance.
        </div>
    </div>
    <br />    
    <div style="width: 50%; float: left">        
        <div class="alert alert-danger alert-dismissable fade in" style="display: none;"
            id="alertDeleteFailed">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a> <strong>
                Delete Failed</strong> Please contact the Admin for Assistance.
        </div>
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
                                    BorderWidth="1" OnSelectedIndexChanged="ServiceType_Changed" Width="100%" Font-Size="Small">
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
                            </asp:Panel>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                                <asp:Label ID="TaskLabel" runat="server" Font-Bold="true" Text="Tasks : "> </asp:Label>
                            </td>
                            <td colspan="2" style="width: 70%">
                                <asp:DropDownList ID="ddlist_tasks" runat="server" OnSelectedIndexChanged="On_Tasks_Changed"
                                    AutoPostBack="true" Width="70%">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" Visible="true" AutoPostBack="true"
                                    RepeatDirection="Horizontal" OnSelectedIndexChanged="On_RadioValue_Changed">
                                </asp:RadioButtonList>
                            </td>
                            <td style="width: 30%">
                                &nbsp;
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle;" class="style7">
                                <asp:Label ID="ActionDatesLabel" runat="server" BorderStyle="None" Text="Action Dates"></asp:Label>
                            </td>
                            <td class="style7">
                            </td>
                            <td class="style8">
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                                <asp:DropDownList ID="ActionDatesDropDownList" runat="server" Width="150" AutoPostBack="true"
                                    OnSelectedIndexChanged="On_ActionDate_Changed">
                                    <asp:ListItem Text="--Choose one--" Value="" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="width: 30%">
                                <asp:TextBox ID="ActionDatesTextBox" runat="server" CssClass="datepicker"></asp:TextBox>
                            </td>
                            <td style="width: 40%">
                                <asp:Label ID="ActionDateValidationLabel" runat="server" BorderStyle="None" Text="Invalid Date Format"
                                    Visible="true" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:UpdatePanel ID="updatepanel_FabRow" runat="server">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="FabricationHourLabel" runat="server" BorderStyle="None" Text="Fabrication Hour :"></asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                    <asp:TextBox ID="FabTextBox" runat="server" class="positiveinteger_deciamalOnly"></asp:TextBox>
                                                </td>
                                                <td style="width: 40%">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Panel ID="updatePanel_BillingInfo" runat="server">
                                    <contenttemplate>
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="BillingInfoLabel" runat="server" BorderStyle="None" Font-Bold="true"
                                                        Text="Billing Information"></asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                    <input class='modal' type="button" value="CPT Codes" id="CPTCodeButton" runat="server" /><br />
                                                    <div id="dialog" title="CPT Codes">
                                                        <b>Select CPT Codes :</b><br />
                                                        <asp:CheckBoxList ID="CPTCodesCheckBoxList" runat="server" RepeatDirection="Vertical">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </td>
                                                <td style="width: 40%">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:UpdatePanel ID="updatepanel_BillingInfoRow" runat="server">
                                    <ContentTemplate>
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="BillingCodeLabel" runat="server" Text="Code:"> </asp:Label>
                                                    <asp:DropDownList ID="DropDownList1" runat="server" Width="115" AutoPostBack="true"
                                                        Visible="true" OnSelectedIndexChanged="CPTCode_Changed">
                                                    </asp:DropDownList>
                                                    <asp:HiddenField ID="HiddenCPTCodes" runat="server" />
                                                </td>
                                                <td style="width: 30%">
                                                    <asp:TextBox ID="CPTCodeListTextBox" runat="server" TextMode="MultiLine" Visible="true"
                                                        Width="115" ReadOnly="true" AutoPostBack="true"> </asp:TextBox>
                                                </td>
                                                <td style="width: 40%">
                                                    <asp:Label ID="BillingUnitLabel" runat="server" Text="Minutes:"> </asp:Label>
                                                    <asp:TextBox ID="TextBox1" runat="server" Width="30" MaxLength="3" class="positiveintegerOnly"> </asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- UpdatePanel Changed to Normal Panel. Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_CaseClosedRow" runat="server">
                                    <contenttemplate>
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="CaseClosedReasonLabel" runat="server" BorderStyle="None" Text="Reason :"> </asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                    <asp:DropDownList ID="CaseClosedReasonDropDownList" runat="server" Width="150" AutoPostBack="true"
                                                        OnSelectedIndexChanged="On_CaseCloseReason_Changed">
                                                        <asp:ListItem Text="--Choose one--" Value="" Selected="True"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="width: 40%">
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- Update Panel Changed to Normal Panel (to prevent partial postpoack). Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_OtherReasonRow" runat="server">
                                    <contenttemplate>
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="OtherReasonLabel" runat="server" BorderStyle="None" Text="Enter Reason :"> </asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                    <asp:TextBox ID="OtherReasonTextBox" runat="server" Width="145" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                                <td style="width: 40%">
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- Update Panel Changed to Normal Panel. Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_DoctorRow" runat="server">
                                    <contenttemplate>
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="DoctorLabel" runat="server" BorderStyle="None" Text="Doctor :"> </asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                    <asp:DropDownList ID="DoctorDropDownList" Width="150" runat="server" AutoPostBack="true"
                                                        OnSelectedIndexChanged="Doctor_Changed">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="width: 40%">
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- Update Panel Changed to Normal Panel (to prevent partial postpoack). Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_DoctorCheckBoxRow" runat="server">
                                    <contenttemplate>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="DoctorCheckBox" Text="Create Doctor Contact" runat="server" AutoPostBack="true"
                                                        OnCheckedChanged="Doctor_Checked" />
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- Update Panel Changed to Normal Panel (to prevent partial postpoack). Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_DoctorInfo" runat="server">
                                    <contenttemplate>
                                        <asp:Panel ID="panel_DoctorInfo" runat="server">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="DocInfoLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="Doctor Contact"> </asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="MDFNameLabel" runat="server" Text="First Name :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="MDFNameTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="MDLNameLabel" runat="server" Text="Last Name :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="MDLNameTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="NPILabel" runat="server" Text="NPI :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="NPITextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="AddressLabel" runat="server" Text="Address :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="AddressTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="CityLabel" runat="server" Text="City :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="CityTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="StateLabel" runat="server" Text="State :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:DropDownList ID="StateDropDownList" Width="150" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="ZipLabel" runat="server" Text="Zip :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="ZipTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="FaxLabel" runat="server" Text="Fax :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="FaxTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Label ID="EmailLabel" runat="server" Text="E-Mail :"></asp:Label>
                                                    </td>
                                                    <td style="width: 30%">
                                                        <asp:TextBox ID="EmailTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                        <asp:Button ID="DoctorSaveButton" runat="server" Text="Save Doctor Contact" OnClick="Doctor_Save_Click" />
                                                    </td>
                                                    <td style="width: 30%">
                                                    </td>
                                                    <td style="width: 40%">
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <%-- Update Panel Changed to Normal Panel (to prevent partial postpoack). Didnot change the ID    -Mrinal Dhawan, 6/19/2017 --%>
                                <asp:Panel ID="updatepanel_DoctorMessageRow" runat="server">
                                    <contenttemplate>
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="text-align: left; vertical-align: middle; width: 30%;">
                                                    <asp:Label ID="DoctorMessageLabel" runat="server" Text="Label" ForeColor="Red"></asp:Label>
                                                </td>
                                                <td style="width: 30%">
                                                </td>
                                                <td style="width: 40%">
                                                </td>
                                            </tr>
                                        </table>
                                    </contenttemplate>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="text-align: left;">
                                &nbsp;
                            </td>
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
                                <asp:Label ID="ReferralTabSTLabel" runat="server" Text="" BorderStyle="None" ForeColor="Red"
                                    Visible="true"> </asp:Label>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:UpdatePanel ID="updatepanel_CaseCloseMessageRow" runat="server">
                                    <ContentTemplate>
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="CaseCloseMessageLabel" runat="server" BorderStyle="None" Text="Referral is cancelled for the selected referral date. "
                                                        Font-Bold="true" ForeColor="Red"> </asp:Label>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                    OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command" PageSize="20">
                                </asp:GridView>
                                <asp:GridView ID="GridView2" runat="server" Visible="true">
                                </asp:GridView>
                                <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                    Text="No search results found" Visible="true"> </asp:Label>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="StatusPanel" runat="server" ScrollBars="Auto" Height="600px">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_Status" runat="server" Text="Status" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%--<asp:GridView ID="GridView3" ShowHeader="false" GridLines="Horizontal" CellSpacing="10"
                                                    CellPadding="5" runat="server" OnRowDataBound="GridView_RowDataBound">
                                                </asp:GridView>--%>
                                                <asp:GridView ID="GridView3" runat="server" EnablePersistedSelection="true" AlternatingRowStyle-BackColor="LightGray"
                                                    AutoGenerateColumns="false" Width="100%" OnRowDataBound="GridView3_RowDataBound"
                                                    OnDataBound="GridView3_DataBound" OnRowEditing="GridView3_RowEdit" DataKeyNames="ServiceId"
                                                    OnRowDeleting="GridView3_RowDelete">
                                                    <Columns>
                                                        <asp:CommandField ShowEditButton="true" />
                                                        <asp:TemplateField HeaderText="Service Type" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_TypeOfService" runat="server" Font-Size="Small" Text='<%# Eval("TypeofService") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Task" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_TaskName" runat="server" Font-Size="Small" Text='<%# Eval("Task") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_ActionName" runat="server" Font-Size="Small" Text='<%# Eval("Action") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_ActionDate" runat="server" Font-Size="Small" Text='<%# Eval("Date") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Clinician" ItemStyle-Width="40%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_Clinician" runat="server" Font-Size="Small" Text='<%# Eval("Clinician") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" DeleteImageUrl="Images\61391.png"
                                                            DeleteText="Delete Record" ItemStyle-Height="5%" ItemStyle-Width="5%" ButtonType="Image"
                                                            ControlStyle-Height="30%" ControlStyle-Width="40%" ItemStyle-HorizontalAlign="Center">
                                                        </asp:CommandField>
                                                        <asp:TemplateField HeaderText="Doctor ID" ItemStyle-Width="20%" ItemStyle-CssClass="hiddencol"
                                                            HeaderStyle-CssClass="hiddencol">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorID" runat="server" Font-Size="Small" Text='<%# Eval("LOMNContactId") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="InputNetId" ItemStyle-Width="20%" ItemStyle-CssClass="hiddencol"
                                                            HeaderStyle-CssClass="hiddencol">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_InputNetId" runat="server" Font-Size="Small" Text='<%# Eval("InputNetId") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                                <asp:Label ID="NoDataLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                                    Text="No record to display" Visible="true"> </asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="DoctorInfoLabel" runat="server" ForeColor="Black" Font-Bold="true"
                                                    BorderStyle="None" Text="Doctor Contact Information" Visible="true"> </asp:Label>
                                                <asp:GridView ID="GridView4" runat="server" EnablePersistedSelection="true" DataKeyNames="LOMNContactId"
                                                    AlternatingRowStyle-BackColor="LightGray" OnRowEditing="GridView4_RowEdit" OnDataBound="GridView4_DataBound"
                                                    AutoGenerateColumns="false" Width="100%">
                                                    <Columns>
                                                        <asp:CommandField ShowEditButton="true" />
                                                        <asp:TemplateField HeaderText="First Name" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorFN" runat="server" Font-Size="Small" Text='<%# Eval("FirstName") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Last Name" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorLN" runat="server" Font-Size="Small" Text='<%# Eval("LastName") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="NPI" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorNPI" runat="server" Font-Size="Small" Text='<%# Eval("NPI") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="FAX" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorFAX" runat="server" Font-Size="Small" Text='<%# Eval("FAX") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Email" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorEmail" runat="server" Font-Size="Small" Text='<%# Eval("Email") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                                <asp:GridView ID="GridView5" runat="server" EnablePersistedSelection="true" AlternatingRowStyle-BackColor="LightGray"
                                                    AutoGenerateColumns="false" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Address" ItemStyle-Width="45%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorAddress" runat="server" Font-Size="Small" Text='<%# Eval("Address") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="City" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorCity" runat="server" Font-Size="Small" Text='<%# Eval("City") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="State" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorState" runat="server" Font-Size="Small" Text='<%# Eval("State") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Zip" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DoctorZip" runat="server" Font-Size="Small" Text='<%# Eval("Zip") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="Back_Click" />
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="Save_Click" ValidationGroup="atsave"
                    CausesValidation="true" />
                <asp:Button ID="CancelUpdateButton" runat="server" Text="Cancel" OnClick="CancelUpdate_Click" />
                <asp:Button ID="UpdateButton" runat="server" Text="Update" 
                    OnClick="Update_Click" ValidationGroup="atsave" />
                <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
