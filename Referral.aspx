<%@ Page Language="C#" Title="Referral" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="Referral.aspx.cs" Inherits="ATUClient.Referral" %>

<%--<html>
<head id="Head1" runat="server">
<title></title>
<script type="text/javascript">
    var myWindow = null;
    function myFunction() {
        myWindow = window.showModalDialog("Referral.aspx", "MsgWindow", "width=100, height=50");
        myWindow.document.write("<p>This window's name is: " + myWindow.name + "</p>");
        myWindow.document.write("<input type='text'>");
    }
</script>
</head></html>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="scripts/jquery-ui.min.css" rel="stylesheet" type="text/css" />

    <script src="scripts/jquery-1.11.0.min.js" type="text/javascript"></script>

    <script src="scripts/jquery-ui.min.js" type="text/javascript"></script>

    <script src="scripts/bootstrap.js" type="text/javascript"></script>

    <link href="Styles/bootstrap.css" rel="stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript">

        var currentString = "";
        var tempString = "";
        var clinicianCurrentString = "";
        var checkboxName = "";
        var serviceLabel = "";
        var taskLabel = "";


        //Modal window to add new contact
        $(function() {

            $(".datepicker").datepicker({ dateFormat: 'mm/dd/yy', changeMonth: true,
                changeYear: true
            })

            var newContactDialog = $("#newContactDialog").dialog({
                autoOpen: false,
                height: 450,
                width: 600,
                show: "slide",
                hide: "fold",
                buttons: {
                    Cancel: function() {
                        $(this).dialog("close");
                        $('[id*=PNewContactCheckBox]').attr('checked', false);
                        $('[id*=SNewContactCheckBox]').attr('checked', false);
                    },
                    Save: function() {
                        if ($("[id*=CNameTextBox]").val() == "") {
                            $("[id*=ContactErrorLabel]").show();
                        }
                        else {
                            $(this).dialog("close");
                            $('[id*=PNewContactCheckBox]').attr('checked', false);
                            $('[id*=SNewContactCheckBox]').attr('checked', false);
                            $("[id*=ContactSaveButton]").click();
                        }
                    }
                }
            });

            newContactDialog.parent().appendTo($("form:first"));

            $('[id*=PNewContactCheckBox]').change(function() {
                if ($(this).is(':checked')) {
                    $("#newContactDialog").dialog("open");
                    $('[id*=CAgencyDropDownList]').val($('[id*=PAgencyDropDownList]').val())
                    return false;
                }
            });

            $('[id*=SNewContactCheckBox]').change(function() {
                if ($(this).is(':checked')) {
                    $("#newContactDialog").dialog("open");
                    $('[id*=CAgencyDropDownList]').val($('[id*=SAgencyDropDownList]').val())
                    return false;
                }
            });

            $(".newContact").click(function() {
                $("#newContactDialog").dialog("open");
                return false;
            });

            $(".newAgency").click(function() {
                $("#newAgencyDialog").dialog("open");
                return false;
            });

            var newAgencyDialog = $("#newAgencyDialog").dialog({
                autoOpen: false,
                height: 600,
                width: 600,
                show: "slide",
                hide: "fold",
                buttons: {
                    Cancel: function() {
                        $(this).dialog("close");
                    },
                    Save: function() {
                        if ($("[id*=NAgencyNameText]").val() == "") {
                            $("[id*=NAgencyErrorLabel]").show();
                        }
                        else {
                            $(this).dialog("close");
                            $("[id*=NASaveButton]").click();
                        }
                    }
                }
            });

            newAgencyDialog.parent().appendTo($("form:first"));

            $("[id*=btnDeleteRefInfo]").click(function() {
                if (confirm('Are you sure you want to delete the referral?')) {
                    return true;
                } else {
                    return false;
                }
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
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
    <asp:Table ID="ReferralTable" runat="server" HorizontalAlign="left" Width="100%">
        <asp:TableRow ID="ReferralRow" runat="server">
            <asp:TableCell ID="ReferralCell" runat="server" HorizontalAlign="Left" VerticalAlign="Top"
                Width="50%">
                <asp:Table ID="ReferralInfoTable" runat="server">
                    <%--Search Criteria--%><asp:TableRow ID="ClientSearchLabelRow" runat="server">
                        <asp:TableCell ID="ClientSearchLabelCell" runat="server">
                            <asp:Label ID="ClientSearchLabel" runat="server" Font-Bold="true" Text="Client Search"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="SearchRow" runat="server">
                        <asp:TableCell ID="SearchCellFN" runat="server">
                            <asp:Label ID="LastNameLabel" runat="server" BorderStyle="None" Text="Last Name"></asp:Label>
                            <br />
                            <asp:TextBox ID="LastNameText" Width="150" runat="server"></asp:TextBox>
                            <br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="SearchCellLN" runat="server">
                            <asp:Label ID="FirstNameLabel" runat="server" BorderStyle="None" Text="First Name"></asp:Label>
                            <br />
                            <asp:TextBox ID="FirstNameText" Width="150" runat="server"></asp:TextBox>
                            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"
                                CausesValidation="false" />
                            <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"
                                CausesValidation="false" /><br />
                            <br />
                        </asp:TableCell>
                        <%--<asp:TableCell ID="SearchButtonTableCell" runat="server"><br /--%>
                        <%--<asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click" CausesValidation="false"/>--%>
                        <%--<asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click" CausesValidation="false"/><br /><br />--%>
                        <%--</asp:TableCell>--%>
                    </asp:TableRow>
                    <%--Referral Method--%>
                    <asp:TableRow ID="RMethodRow" runat="server">
                        <asp:TableCell ID="RMethodCell" runat="server">
                            <asp:Label ID="RMethodLabel" runat="server" BorderStyle="None" Text="Referral Method :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="RMethodValueCell" runat="server">
                            <asp:DropDownList ID="RMethodDropDownList" Width="150" runat="server">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Referral Source--%>
                    <asp:TableRow ID="RSourceRow" runat="server">
                        <asp:TableCell ID="RSourceCell" runat="server">
                            <asp:Label ID="RSourceLabel" runat="server" BorderStyle="None" Text="Referral Source :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="RSourceValueCell" runat="server">
                            <asp:DropDownList ID="RSourceDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ReferralSource_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Referal Date--%>
                    <asp:TableRow ID="RDateRow" runat="server">
                        <asp:TableCell ID="RDateCell" runat="server">
                            <asp:Label ID="RDateLabel" runat="server" BorderStyle="None" Text="Referral Date :"></asp:Label><br />
                        </asp:TableCell>
                        <asp:TableCell ID="RDateValueCell" runat="server">
                            <asp:TextBox ID="RDateTextBox" Width="150" runat="server" CssClass="datepicker"></asp:TextBox>
                            <%--<asp:CustomValidator ID="RDateCustomValidator" runat="server" ControlToValidate="RDateTextBox" ErrorMessage="Date is in incorrect format" OnServerValidate="RDateCustomValidator_Validate" />--%>
                            <asp:Label ID="RefDateValidationLabel" runat="server" BorderStyle="None" Text="Invalid Date Format"
                                Visible="false" ForeColor="Red"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Service Types--%>
                    <asp:TableRow ID="ServiceTypeRow" runat="server">
                        <asp:TableCell ID="ServiceTypeCell" runat="server">
                            <asp:Label ID="ServiceTypeLabel" runat="server" BorderStyle="None" Text="Service Type :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="ServiceTypeDataCell" runat="server">
                            <asp:CheckBoxList ID="ServiceTypeCheckBoxList" runat="server" RepeatDirection="Horizontal"
                                RepeatColumns="4">
                            </asp:CheckBoxList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--DHS-DRS Bureau--%>
                    <asp:TableRow ID="DRSBureauRow" runat="server" Visible="false">
                        <asp:TableCell ID="DRSBureauCell" runat="server">
                            <asp:Label ID="DRSBureauLabel" runat="server" BorderStyle="None" Text="DHS-DRS Bureau :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="DRSBureauValueCell" runat="server">
                            <asp:DropDownList ID="DRSBureauDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="DRS_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--DHS-DRS ReferralId (For HS & VR)10/21/15 Daniel--%>
                    <asp:TableRow ID="DRSRefIdRow" runat="server" Visible="false">
                        <asp:TableCell ID="DRSRefIdCell" runat="server">
                            <asp:Label ID="DRSRefIdLabel" runat="server" BorderStyle="None" Text="ReferralId :"></asp:Label>
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="DRSRefIdValueCell" runat="server">
                            <asp:TextBox ID="DRSRefIdTextBox" Width="150" runat="server"></asp:TextBox>
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Referral ClientId (For HS & VR)--%>
                    <asp:TableRow ID="RefClientIdRow" runat="server" Visible="false">
                        <asp:TableCell ID="RefClientIdCell" runat="server">
                            <asp:Label ID="RefClientIdLabel" runat="server" BorderStyle="None" Text="Referral ClientId :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="RefClientIdValueCell" runat="server">
                            <asp:TextBox ID="RefClientIdTextBox" Width="150" runat="server"></asp:TextBox>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Agency--%>
                    <asp:TableRow ID="AgencyLabelRow" runat="server">
                        <asp:TableCell ID="AgencyLabelCell" runat="server">
                            <asp:Label ID="AgencyLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="Agency :"></asp:Label>
                        </asp:TableCell>
                        <%--  <asp:TableCell ID="NewContactLabelCell" runat="server">
                            <asp:Label ID="NewContactLabel" runat="server" BorderStyle="None" Font-Bold="true"
                                Text="New Contact for Agency :"></asp:Label>
                        </asp:TableCell>   --%>
                    </asp:TableRow>
                    <%--New Agency --%>
                    <asp:TableRow ID="NewAgencyRow" runat="server">
                        <asp:TableCell ID="NewAgencyCell" runat="server">
                            <asp:Button ID="NewAgencyButton" runat="server" Text="New Agency" class="newAgency" />
                        </asp:TableCell>
                        <%--   <asp:TableCell ID="NewContactCell" runat="server">
                            <asp:Button class="newContact" ID="NewContactButton" runat="server" Text="New Contact" />
                        </asp:TableCell> --%>
                    </asp:TableRow>
                    <%--P&SAgencies--%>
                    <asp:TableRow ID="AgenciesRow" runat="server">
                        <asp:TableCell ID="AgenciesCell" runat="server">
                            <asp:CheckBoxList ID="AgenciesCheckBoxList" CellPadding="10" AutoPostBack="true"
                                RepeatDirection="Vertical" OnSelectedIndexChanged="CheckBoxList_Selection_Changed"
                                runat="server">
                                <asp:ListItem>Primary</asp:ListItem>
                                <asp:ListItem>Secondary</asp:ListItem>
                            </asp:CheckBoxList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Primary Agency--%>
                    <asp:TableRow ID="PAgencyRow" runat="server" Visible="false">
                        <asp:TableCell ID="PAgencyCell" runat="server">
                            <asp:Label ID="PAgencyLabel" runat="server" BorderStyle="None" Text="Primary Agency :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="PAgencyValueCell" runat="server">
                            <asp:DropDownList ID="PAgencyDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="Primary_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Primary Contact--%>
                    <asp:TableRow ID="PContactRow" runat="server" Visible="false">
                        <asp:TableCell ID="PContactCell" runat="server">
                            <asp:Label ID="PContactLabel" runat="server" BorderStyle="None" Text="Primary Contact :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="PContactValueCell" runat="server">
                            <asp:DropDownList ID="PContactDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="PContact_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--P New Contact--%>
                    <asp:TableRow ID="PNewContactRow" runat="server" Visible="false">
                        <asp:TableCell ID="PNewContactCell" runat="server">
                            <asp:CheckBox ID="PNewContactCheckBox" runat="server" Text="New Contact" class="primaryNewContact" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                    &nbsp;
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Secondary Agency--%>
                    <asp:TableRow ID="SAgencyRow" runat="server" Visible="false">
                        <asp:TableCell ID="SAgencyCell" runat="server">
                            <asp:Label ID="SAgencyLabel" runat="server" BorderStyle="None" Text="Secondary Agency :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="SAgencyValueCell" runat="server">
                            <asp:DropDownList ID="SAgencyDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="Secondary_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Secondary Contact--%>
                    <asp:TableRow ID="SContactRow" runat="server" Visible="false">
                        <asp:TableCell ID="SContactCell" runat="server">
                            <asp:Label ID="SContactLabel" runat="server" BorderStyle="None" Text="Secondary Contact :"></asp:Label><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="SContactValueCell" runat="server">
                            <asp:DropDownList ID="SContactDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="SContact_Changed">
                            </asp:DropDownList>
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--S New Contact--%>
                    <asp:TableRow ID="SNewContactRow" runat="server" Visible="false">
                        <asp:TableCell ID="SNewContactCell" runat="server">
                            <asp:CheckBox ID="SNewContactCheckBox" runat="server" Text="New Contact" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="NewContactModalRow" runat="server">
                        <asp:TableCell ID="NewContactModalCell" runat="server">
                            <div id="newContactDialog" title="Contact Information">
                                <form>
                                <b>Enter Contact Information :</b><br />
                                <br />
                                <br />
                                <%--Modal Window Controls --%>
                                <%--Entity Name--%>
                                <asp:Table ID="Table1" runat="server">
                                    <%--Agency Name--%>
                                    <asp:TableRow ID="AgencyRow" runat="server" Visible="true">
                                        <asp:TableCell ID="AgencyCell" runat="server">
                                            <asp:Label ID="CAgencyLabel" runat="server" BorderStyle="None" Text="Contact's Agency"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="CAgencyValueCell" runat="server">
                                            <asp:DropDownList ID="CAgencyDropDownList" runat="server" Width="60%">
                                            </asp:DropDownList>
                                            <asp:Label ID="SelectAgencyLabel" runat="server" BorderStyle="None" Text="Select an Agency"
                                                Style="display: none" ForeColor="Red"></asp:Label>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--Contact Name--%>
                                    <asp:TableRow ID="CNameRow" runat="server" Visible="true">
                                        <asp:TableCell ID="CNameCell" runat="server">
                                            <asp:Label ID="CNameLabel" runat="server" BorderStyle="None" Text="Contact Name"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="CNameValueCell" runat="server">
                                            <asp:TextBox ID="CNameTextBox" Width="60%" runat="server"></asp:TextBox>
                                            <asp:Label ID="ContactErrorLabel" runat="server" BorderStyle="None" Text="Enter Contact Name."
                                                Style="display: none" ForeColor="Red"></asp:Label>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--Phone--%>
                                    <asp:TableRow ID="PhoneRow" runat="server" Visible="true">
                                        <asp:TableCell ID="PhoneCell" runat="server">
                                            <asp:Label ID="PhoneLabel" runat="server" BorderStyle="None" Text="Phone #"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="PhoneValueCell" runat="server">
                                            <asp:TextBox ID="PhoneTextBox" Width="60%" runat="server"></asp:TextBox>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--Extension--%>
                                    <asp:TableRow ID="ExtensionRow" runat="server" Visible="true">
                                        <asp:TableCell ID="ExtensionCell" runat="server">
                                            <asp:Label ID="ExtensionLabel" runat="server" BorderStyle="None" Text="Extn"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="ExtensionValueCell" runat="server">
                                            <asp:TextBox ID="ExtensionTextBox" Width="60%" runat="server"></asp:TextBox>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--Fax--%>
                                    <asp:TableRow ID="FaxRow" runat="server" Visible="true">
                                        <asp:TableCell ID="FaxCell" runat="server">
                                            <asp:Label ID="FaxLabel" runat="server" BorderStyle="None" Text="Fax"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="FaxValueCell" runat="server">
                                            <asp:TextBox ID="FaxTextBox" Width="60%" runat="server"></asp:TextBox>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--Email--%>
                                    <asp:TableRow ID="EmailRow" runat="server" Visible="true">
                                        <asp:TableCell ID="EmailCell" runat="server">
                                            <asp:Label ID="EmailLabel" runat="server" BorderStyle="None" Text="E-mail :"></asp:Label><br />
                                        </asp:TableCell>
                                        <asp:TableCell ID="EmailValueCell" runat="server">
                                            <asp:TextBox ID="EmailTextBox" Width="60%" runat="server"></asp:TextBox>
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="ContactSaveButtonRow" runat="server">
                                        <asp:TableCell ID="ContactSaveButtonCell" runat="server" HorizontalAlign="Center"
                                            VerticalAlign="Middle">
                                            <asp:Button ID="ContactSaveButton" runat="server" Text="Save" OnClick="ContactSave_Click"
                                                Style="display: none" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <%--End of Modal Window Controls --%>
                                </form>
                            </div>
                            <asp:HiddenField ID="ContactInfoHiddenField" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="NewAgencyModalRow" runat="server">
                        <asp:TableCell ID="NewAgencyModalCell" runat="server">
                            <div id="newAgencyDialog" title="Agency Information">
                                <b>Enter Agency Information :</b><br />
                                <br />
                                <br />
                                <%--Modal Window Controls --%>
                                <%--Entity Name--%>
                                <asp:Table ID="Table2" runat="server">
                                    <asp:TableRow ID="NAgencyNameRow" runat="server">
                                        <asp:TableCell ID="NAgencyLabelCell" runat="server">
                                            <asp:Label ID="NAgencyNameLabel" runat="server" BorderStyle="None" Text="Agency Name : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAgencyTextCell" runat="server">
                                            <asp:TextBox ID="NAgencyNameText" Width="150" runat="server"></asp:TextBox>
                                            <asp:Label ID="NAgencyErrorLabel" runat="server" BorderStyle="None" Text="Enter Agency Name."
                                                Style="display: none" ForeColor="Red"></asp:Label>
                                            <%--<asp:requiredfieldvalidator id="AgencyNameRequiredFieldValidator"
                                                        controltovalidate="AgencyNameText"
                                                        validationgroup="AgencyInfoGroup"
                                                        errormessage="Enter Agency Name."
                                                        runat="Server" EnableClientScript="false">
                            </asp:requiredfieldvalidator> --%>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <%--  <asp:TableRow ID="NAContactRow" runat="server">
                                        <asp:TableCell ID="NAContactCell" runat="server">
                                            <asp:Label ID="NAContactLabel" runat="server" BorderStyle="None" Text="Contact Name : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAContactTextCell" runat="server">
                                            <asp:TextBox ID="NAContactTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>--%>
                                    <asp:TableRow ID="NAAddressRow" runat="server">
                                        <asp:TableCell ID="NAAddressCell" runat="server">
                                            <asp:Label ID="NAAddressLabel" runat="server" BorderStyle="None" Text="Address : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAAddressTextCell" runat="server">
                                            <asp:TextBox ID="NAAddressTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NACityRow" runat="server">
                                        <asp:TableCell ID="NACityCell" runat="server">
                                            <asp:Label ID="NACityLabel" runat="server" BorderStyle="None" Text="City : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NACityTextCell" runat="server">
                                            <asp:TextBox ID="NACityTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NAStateRow" runat="server">
                                        <asp:TableCell ID="NAStateCell" runat="server">
                                            <asp:Label ID="NAStateLabel" runat="server" BorderStyle="None" Text="State : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAStateTextCell" runat="server">
                                            <asp:DropDownList ID="NAStateDropDownList" Width="156" runat="server">
                                            </asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NAZipRow" runat="server">
                                        <asp:TableCell ID="NAZipCell" runat="server">
                                            <asp:Label ID="NAZipLabel" runat="server" BorderStyle="None" Text="Zip : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAZipTextCell" runat="server">
                                            <asp:TextBox ID="NAZipTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NAPhoneRow" runat="server">
                                        <asp:TableCell ID="NAPhoneCell" runat="server">
                                            <asp:Label ID="NAPhoneLabel" runat="server" BorderStyle="None" Text="Phone : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAPhoneTextCell" runat="server">
                                            <asp:TextBox ID="NAPhoneTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NAFaxRow" runat="server">
                                        <asp:TableCell ID="NAFaxCell" runat="server">
                                            <asp:Label ID="NAFaxLabel" runat="server" BorderStyle="None" Text="Fax : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAFaxTextCell" runat="server">
                                            <asp:TextBox ID="NAFaxTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NAEmailRow" runat="server">
                                        <asp:TableCell ID="NAEmailCell" runat="server">
                                            <asp:Label ID="NAEmailLabel" runat="server" BorderStyle="None" Text="Email : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NAEmailTextCell" runat="server">
                                            <asp:TextBox ID="NAEmailTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NATTYRow" runat="server">
                                        <asp:TableCell ID="NATTYCell" runat="server">
                                            <asp:Label ID="NATTYLabel" runat="server" BorderStyle="None" Text="TTY : "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="NATTYTextCell" runat="server">
                                            <asp:TextBox ID="NATTYTextBox" Width="150" runat="server"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow ID="NASaveButtonRow" runat="server">
                                        <asp:TableCell ID="NASaveButtonCell" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
                                            <asp:Button ID="NASaveButton" runat="server" Text="Save" OnClick="NASave_Click" Style="display: none" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <%--End of Modal Window Controls --%>
                            </div>
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
            <asp:TableCell ID="SearchGridCell" runat="server" VerticalAlign="Top">
                <asp:Table ID="GridsTable" runat="server" HorizontalAlign="Left">
                    <%--Search Grid--%>
                    <asp:TableRow ID="CurrentClientRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                        <asp:TableCell ID="CurrentClientCell" runat="server">
                            <asp:Label ID="CurrentClientLabel" runat="server" BorderStyle="None" Text="Current Client: "
                                Font-Bold="true" ForeColor="Red"></asp:Label>
                            <asp:Label ID="CurrentClientValueLabel" runat="server" BorderStyle="None" Text=""></asp:Label><br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="GridsRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                        <asp:TableCell ID="GridsCell" runat="server">
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command" PageSize="20">
                            </asp:GridView>
                            <asp:GridView ID="GridView2" runat="server" Visible="false">
                            </asp:GridView>
                            <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                Text="No search results found" Visible="false"></asp:Label><br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Original Referral Date--%>
                    <asp:TableRow ID="OReferralDateRow" runat="server" Visible="false">
                        <asp:TableCell ID="OReferralDateCell" runat="server">
                            <asp:Label ID="OReferralDateLabel" runat="server" Text="Existing Referral Date :   "></asp:Label>
                            <asp:DropDownList ID="OReferralDateDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ReferralDate_Changed">
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Mark Inactive Row--%>
                    <asp:TableRow ID="MarkInactiveRow" runat="server">
                        <asp:TableCell ID="MarkInactiveCell1" runat="server" HorizontalAlign="Left" VerticalAlign="Middle">
                            <asp:Panel ID="updatepanel_MarkInactive" runat="server">
                                <asp:CheckBox ID="MarkInactiveCheckbox" runat="server" Text="Mark Referral as Inactive: "
                                    AutoPostBack="true" TextAlign="Left" OnCheckedChanged="MarkInactiveCheckbox_CheckedChanged"
                                    Font-Bold="true" />
                                <br />
                                <asp:DropDownList ID="MarkInactiveReason" runat="server" Width="250px" Visible="false" />
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="AgenciesInfoRow" runat="server" Visible="false">
                        <asp:TableCell ID="AgenciesInfoCell" runat="server" BorderWidth="1" HorizontalAlign="Left"
                            VerticalAlign="Top">
                            <asp:Panel ID="AgenciesInfoPanel" runat="server" ScrollBars="Both" Height="600">
                                <asp:Table ID="RefInactiveTable" runat="server" HorizontalAlign="Left" Width="100%"
                                    Font-Names="Verdana" Font-Size="11" ForeColor="Black">
                                    <asp:TableRow ID="RefInactiveRow" runat="server">
                                        <asp:TableCell ID="RefInactiveCell" runat="server">
                                            <asp:Label ID="RefInactiveReason_label" Text="Referral Marked as Inactive with reason"
                                                runat="server" BackColor="LightGray" Font-Bold="true" />
                                            <br />
                                            <asp:Label ID="RefInactiveReason_Show" runat="server" ForeColor="Red" />
                                            <br />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:Table ID="RefInfoTable" runat="server" HorizontalAlign="Left" Width="100%" Font-Names="Verdana"
                                    Font-Size="11" ForeColor="Black" Font-Bold="true">
                                    <asp:TableRow ID="RefInfoLabelRow" runat="server">
                                        <asp:TableCell ID="RefInfoLabelCell" runat="server">
                                Referral Information 
                                        </asp:TableCell>
                                        <%--Referral Info Grid EDit --- Mrinal Dhawan--%>
                                        <asp:TableCell ID="RefInfoEditCell" runat="server">
                                            <asp:Button ID="btnEditRefInfo" runat="server" Text="Edit" OnClick="btnEditRefInfo_Click"
                                                Visible="false" />
                                            <asp:Button ID="btnDeleteRefInfo" runat="server" Text="Delete" OnClick="btnDeleteRefInfo_Click"
                                                Visible="false" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <asp:Table ID="RefInfoGridTable" runat="server" Height="83px" HorizontalAlign="left"
                                    Width="100%" Font-Names="Verdana" Font-Size="10" ForeColor="Gray" Font-Bold="True">
                                    <%--Referral Info Grid--%>
                                    <asp:TableRow ID="RefInfoGridRow" runat="server">
                                        <asp:TableCell ID="RefInfoGridCell" runat="server">
                                            <asp:GridView ID="RefInfoGridGridView" ShowHeader="false" GridLines="Horizontal"
                                                AlternatingRowStyle-BackColor="Info" BackColor="AliceBlue" CellSpacing="10" CellPadding="5"
                                                runat="server">
                                            </asp:GridView>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <asp:Table ID="PAgencyInfoLabelTable" runat="server" Font-Names="Verdana" HorizontalAlign="left"
                                    Width="100%" Font-Size="11" ForeColor="#333333" Font-Bold="True">
                                    <asp:TableRow ID="PAgencyLabelRow" runat="server">
                                        <asp:TableCell ID="PAgencyLabelCell" runat="server">
                                Primary Agency 
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <asp:Table ID="PAgencyGridTable" runat="server" Height="83px" HorizontalAlign="left"
                                    Width="500" Font-Names="Verdana" Font-Size="10" ForeColor="Gray" Font-Bold="True">
                                    <%--PAgency Grid--%>
                                    <asp:TableRow ID="PAgencyGridRow" runat="server">
                                        <asp:TableCell ID="PAgencyGridCell" runat="server">
                                            <asp:GridView ID="PAgencyGridView" ShowHeader="false" GridLines="Horizontal" CellSpacing="10"
                                                AlternatingRowStyle-BackColor="Info" BackColor="AliceBlue" CellPadding="5" runat="server">
                                            </asp:GridView>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <asp:Table ID="SAgencyInfoLabelTable" runat="server" Font-Names="Verdana" HorizontalAlign="left"
                                    Width="100%" Font-Size="11" ForeColor="#333333" Font-Bold="True">
                                    <asp:TableRow ID="SAgencyInfoLabelRow" runat="server">
                                        <asp:TableCell ID="SAgencyInfoLabelCell" runat="server">
                                Secondary Agency 
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <asp:Table ID="SAgencyGridTable" runat="server" Height="83px" HorizontalAlign="left"
                                    Width="100%" Font-Names="Verdana" Font-Size="10" ForeColor="Gray" Font-Bold="True">
                                    <%--SAgency Grid--%>
                                    <asp:TableRow ID="SAgencyGridRow" runat="server">
                                        <asp:TableCell ID="SAgencyGridCell" runat="server">
                                            <asp:GridView ID="SAgencyGridView" ShowHeader="false" GridLines="Horizontal" CellSpacing="10"
                                                AlternatingRowStyle-BackColor="Info" BackColor="AliceBlue" CellPadding="5" runat="server">
                                            </asp:GridView>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <%--Save Button Row--%>
        <asp:TableRow ID="SaveRow" runat="server">
            <asp:TableCell ID="SaveCell" runat="server" HorizontalAlign="Center" VerticalAlign="Middle"
                ColumnSpan="3">
                <br />
                <br />
                <asp:Button ID="Button1" runat="server" Text="Back" OnClick="Back_Click" />
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="Save_Click" />
                <asp:Button ID="CancelUpdateButton" runat="server" Text="Cancel" OnClick="CancelUpdate_Click" />
                <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="Update_Click" />
                <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="Continue_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
<%--javascript:myFunction()
<html><head></head>
<body>
<script type="text/javascript">
    var myWindow = null;
    function myFunction() {
        myWindow = window.showModalDialog("Referral.aspx", "MsgWindow", "width=100, height=50");
        myWindow.document.write("<p>This window's name is: " + myWindow.name + "</p>");
        myWindow.document.write("<input type='text'>");
    }
</script>
</body>
</html>--%>