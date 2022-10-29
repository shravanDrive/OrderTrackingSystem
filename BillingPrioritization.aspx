<%@ Page Language="C#" Title="BillingPrioritization" AutoEventWireup="true" MasterPageFile="~/Site.Master"
CodeBehind="BillingPrioritization.aspx.cs" Inherits="ATUClient.BillingPrioritization" %>


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
    var clinicianCurrentString = "";
    var checkboxName = "";
    var serviceLabel = "";
    var taskLabel = "";

    //Modal window to add new contact
    $(function() {
        $("#dialog").dialog({
            autoOpen: false,
            height: 600,
            width: 600,
            modal: true,
            buttons: {
                Cancel: function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    currentString = $("[id*=ENameLabel]").text() + $("[id*=ENameTextBox]").val() + "\n";
                    currentString += $("[id*=CNameLabel]").text() + $("[id*=CNameTextBox]").val() + "\n";
                    currentString += $("[id*=AddressLabel]").text() + $("[id*=AddressTextBox]").val() + "\n";
                    currentString += $("[id*=CityLabel]").text() + $("[id*=CityTextBox]").val() + "\n";
                    if ($("[id*=StateDropDownList]").val() != -1)
                        currentString += $("[id*=StateLabel]").text() + $("[id*=StateDropDownList]").val() + "\n";
                    else
                        currentString += $("[id*=StateLabel]").text() + "" + "\n";
                    currentString += $("[id*=ZipLabel]").text() + $("[id*=ZipTextBox]").val() + "\n";
                    currentString += $("[id*=PhoneLabel]").text() + $("[id*=PhoneTextBox]").val() + "\n";
                    currentString += $("[id*=ExtensionLabel]").text() + $("[id*=ExtensionTextBox]").val() + "\n";
                    currentString += $("[id*=FaxLabel]").text() + $("[id*=FaxTextBox]").val() + "\n";
                    currentString += $("[id*=EmailLabel]").text() + $("[id*=EmailTextBox]").val() + "\n";
                    $("#<%= ContactInfoTextBox.ClientID%>").val(currentString);
                    $("#<%= ContactInfoHiddenField.ClientID%>").val(currentString);
                    $(this).dialog("close");
                }
            }
        });

        $(".modal").click(function() {
            // Open your modal
            if ($(this).is(':checked')) {
                $("#dialog").dialog("open");
            }
        });
        //Modal window dialog for Service Types and billing phases
        $("#dialog1").dialog({
            autoOpen: false,
            height: 650,
            width: 650,
            modal: true,
            buttons: {
                Cancel: function() {
                    $(this).dialog("close");
                },
                "Save": function() {
                    currentString = "";
                    for (var i = 0; i < parseInt($("[id*=ServiceCount]").val()); i++) {
                        checkboxName = "CheckBoxList" + i;
                        serviceLabel = "ServiceLabel" + i;
                        var selectedItems = $("[id$='" + checkboxName + "'] input:checked");
                        if (selectedItems.length > 0) {
                            currentString += $("[id$='" + serviceLabel + "']").text().trim() + ":";
                            for (var j = 0; j < selectedItems.length; j++) {
                                taskLabel = "TaskLabel" + selectedItems[j].name.split("$")[selectedItems[j].name.split("$").length - 1];
                                if (currentString.charAt(currentString.length - 1) == ':')
                                    currentString += " " + $("[id$='" + taskLabel + "']").text().trim();
                                else
                                    currentString += ", " + $("[id$='" + taskLabel + "']").text().trim();
                            }
                            currentString += "\n";
                        }
                    }
                    $("#<%= ServicePhaseTextBox.ClientID%>").val(currentString);
                    $("#<%= ServicePhaseHiddenField.ClientID%>").val(currentString);
                    $(this).dialog("close");
                }
            }
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
<asp:Table ID="ReferralTable" runat="server" HorizontalAlign="left">
    <asp:TableRow ID="ReferralRow" runat="server">
        
        <asp:TableCell ID="ReferralCell" runat="server" HorizontalAlign="Left" VerticalAlign="Top" Width="600" Height="250" >
            <asp:Table ID="ReferralInfoTable" runat="server">
            
            <%--Search Criteria--%>
            <asp:TableRow ID="ClientSearchLabelRow" runat="server">
                <asp:TableCell ID ="ClientSearchLabelCell" runat="server">
                    <asp:Label ID="ClientSearchLabel" runat="server" Font-Bold="true" Text="Client Search"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
                
            <asp:TableRow ID="SearchRow" runat="server">
                <asp:TableCell ID="SearchCellFN" runat="server">
                    <asp:Label ID="LastNameLabel" runat="server" BorderStyle="None" Text="Last Name"></asp:Label><br />
                    <asp:TextBox ID="LastNameText" Width="150" runat="server" ></asp:TextBox><br /><br />
                </asp:TableCell>
                
                <asp:TableCell ID="SearchCellLN" runat="server">
                    <asp:Label ID="FirstNameLabel" runat="server" BorderStyle="None" Text="First Name"></asp:Label><br />
                    <asp:TextBox ID="FirstNameText" Width="150" runat="server" ></asp:TextBox>
                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"/>
                    <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"/><br /><br />
                </asp:TableCell>
                
            </asp:TableRow>
            
            <%--Referral Dates--%>
            <asp:TableRow ID="ReferralDateRow" runat="server">
                <asp:TableCell ID="ReferralDateCell" runat="server">
                    <asp:Label ID="ReferralDateLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="Referral Date :"></asp:Label><br /><br />
                </asp:TableCell>
                <asp:TableCell ID="ReferralDateValueCell" runat="server">
                    <asp:DropDownList ID="ReferralDateDropDownList" Width="150" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReferralDate_Changed">
                    </asp:DropDownList><br /><br />
                </asp:TableCell>
             </asp:TableRow>
             
            <%--Service Type and Tasks Modal Window --%>
             <asp:TableRow ID="ServicePhaseRow" runat="server" Visible="true">
                <asp:TableCell ID="ServicePhaseCell" runat="server">
                    <asp:Label ID="ServicePhaseLabel" runat="server" Visible="true" BorderStyle="None" Font-Bold="true" Text="Service Phases : "></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="ServicePhaseValueCell" runat="server" VerticalAlign="Top">
                    <asp:TextBox ID="ServicePhaseTextBox" runat="server" TextMode="MultiLine" Visible="true" Height="110" Width="150" ReadOnly="true" ></asp:TextBox>
                    <asp:HiddenField ID="ServicePhaseHiddenField" runat="server" />
                    
                    <input class='modal1' type="button" value="Choose Phases" id="ServicePhaseButton" runat="server" /><br /><br />
                     <div id="dialog1" title="Service Phases">
                      <form>
                        <asp:Label ID="ReferralTabSTLabel" runat="server" Text="" BorderStyle="None" ForeColor="Red" Visible="false"></asp:Label><br />
                        <b> Select Phases :</b><br />
                        <asp:Table ID="ServicePhasesTable" runat="server">
                        </asp:Table>
                        <asp:Label ID="EvalLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="E - Evaluation &nbsp&nbsp"></asp:Label>
                        <asp:Label ID="IntakeLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="I - Intake &nbsp&nbsp"></asp:Label>
                        <asp:Label ID="FollowLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="F - Followup &nbsp&nbsp"></asp:Label>
                        <asp:Label ID="EquipmentLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="Eq - Equipment"></asp:Label>
                        <asp:HiddenField ID="ServiceCount" runat="server"/>
                      </form>
                      </div>
                    
                </asp:TableCell>
             </asp:TableRow> 
            
            <%--Third Party Payer--%>
            <asp:TableRow ID="TPPRow" runat="server">
                <asp:TableCell ID="TPPCell" runat="server">
                    <asp:Label ID="TPPLabel" runat="server" BorderStyle="None" Font-Bold="true" Text="Third Party Payer :"></asp:Label><br /><br />
                </asp:TableCell>
                <asp:TableCell ID="TPPValueCell" runat="server">
                    <asp:DropDownList ID="TPPDropDownList" Width="150" runat="server" OnSelectedIndexChanged="ThirdPartyPayer_Changed" AutoPostBack="true">
                    </asp:DropDownList><br /><br />
                </asp:TableCell>
             </asp:TableRow>
             
             <%--New Contact Checkbox --%>
             <asp:TableRow ID="NewContactCheckboxRow" runat="server" Visible="false">
                <asp:TableCell ID="NewContactCheckboxCell" runat="server">
                    <input class='modal' type="checkbox" value="Add Contact" id="ContactCheckBox" runat="server" /> Contact<br />
                    <div id="dialog" title="Contact Information">
                      <form>
                        <b> Enter Contact Information :</b><br />
                        <%--Modal Window Controls --%>
                            <%--Entity Name--%>
                            <asp:Table runat="server">
                            <asp:TableRow ID="EnameRow" runat="server" Visible="true">
                                <asp:TableCell ID="ENameCell" runat="server">
                                    <asp:Label ID="ENameLabel" runat="server" BorderStyle="None" Text="Entity Name :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="ENameValueCell" runat="server">
                                    <asp:TextBox ID="ENameTextBox" Width="150" runat="server"></asp:TextBox>
                                    <asp:requiredfieldvalidator id="ENameRequiredfieldvalidator"
                                      controltovalidate="ENameTextBox"
                                      validationgroup="PersonalInfoGroup"
                                      errormessage="Enter entity name."
                                      runat="Server" EnableClientScript="false">
                                    </asp:requiredfieldvalidator>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                            <%--Contact Name--%>
                            <asp:TableRow ID="CNameRow" runat="server" Visible="true">
                                <asp:TableCell ID="CNameCell" runat="server">
                                    <asp:Label ID="CNameLabel" runat="server" BorderStyle="None" Text="Contact Name :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="CNameValueCell" runat="server">
                                    <asp:TextBox ID="CNameTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                            <%--Address--%>
                            <asp:TableRow ID="AddressRow" runat="server" Visible="true">
                                <asp:TableCell ID="AddressCell" runat="server">
                                    <asp:Label ID="AddressLabel" runat="server" BorderStyle="None" Text="Address :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="AddressValueCell" runat="server">
                                    <asp:TextBox ID="AddressTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                             <%--City--%>
                            <asp:TableRow ID="CityRow" runat="server" Visible="true">
                                <asp:TableCell ID="CityCell" runat="server">
                                    <asp:Label ID="CityLabel" runat="server" BorderStyle="None" Text="City :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="CityValueCell" runat="server">
                                    <asp:TextBox ID="CityTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                             <%--State--%>
                            <asp:TableRow ID="StateRow" runat="server" Visible="true">
                                <asp:TableCell ID="StateCell" runat="server">
                                    <asp:Label ID="StateLabel" runat="server" BorderStyle="None" Text="State :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="StateValueCell" runat="server">
                                    <asp:DropDownList ID="StateDropDownList" runat="server" Width="156" ></asp:DropDownList>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                             <%--Zip--%>
                            <asp:TableRow ID="ZipRow" runat="server" Visible="true">
                                <asp:TableCell ID="ZipCell" runat="server">
                                    <asp:Label ID="ZipLabel" runat="server" BorderStyle="None" Text="Zip :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="ZipValueCell" runat="server">
                                    <asp:TextBox ID="ZipTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                            
                            <%--Phone--%>
                            <asp:TableRow ID="PhoneRow" runat="server" Visible="true">
                                <asp:TableCell ID="PhoneCell" runat="server">
                                    <asp:Label ID="PhoneLabel" runat="server" BorderStyle="None" Text="Phone # :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="PhoneValueCell" runat="server">
                                    <asp:TextBox ID="PhoneTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                            <%--Extension--%>
                            <asp:TableRow ID="ExtensionRow" runat="server" Visible="true">
                                <asp:TableCell ID="ExtensionCell" runat="server">
                                    <asp:Label ID="ExtensionLabel" runat="server" BorderStyle="None" Text="Extn :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="ExtensionValueCell" runat="server">
                                    <asp:TextBox ID="ExtensionTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>

                            <%--Fax--%>
                            <asp:TableRow ID="FaxRow" runat="server" Visible="true">
                                <asp:TableCell ID="FaxCell" runat="server">
                                    <asp:Label ID="FaxLabel" runat="server" BorderStyle="None" Text="Fax :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="FaxValueCell" runat="server">
                                    <asp:TextBox ID="FaxTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow>
                            
                            <%--Email--%>
                            <asp:TableRow ID="EmailRow" runat="server" Visible="true">
                                <asp:TableCell ID="EmailCell" runat="server">
                                    <asp:Label ID="EmailLabel" runat="server" BorderStyle="None" Text="E-mail :"></asp:Label><br />
                                </asp:TableCell>
                                <asp:TableCell ID="EmailValueCell" runat="server">
                                    <asp:TextBox ID="EmailTextBox" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </asp:TableCell>
                            </asp:TableRow> 
                            </asp:Table>
                        <%--End of Modal Window Controls --%>
                        <p style="color:Red">Note: Previous Contact will be inaccessible.</p>
                      </form>
                      </div>
                </asp:TableCell>
                <asp:TableCell ID="NewContactLabelCell" runat="server">
                    <asp:Label ID="NewContactLabel" runat="server" Visible="true" Text="(For Atypical Payer)" Font-Size="Small" ForeColor="Brown"></asp:Label><br />
                </asp:TableCell>
             </asp:TableRow>
             
             <%--New Contact Information --%>
             <asp:TableRow ID="ContactInfoRow" runat="server" Visible="false">
                <asp:TableCell ID="ContactInfoCell" runat="server">
                    <asp:Label ID="ContactInfoLabel" runat="server" Visible="true" Text="Contact Info : "></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="ContactInfoValueCell" runat="server">
                    <asp:TextBox ID="ContactInfoTextBox" runat="server" TextMode="MultiLine" Visible="true" Height="200" Width="150" ReadOnly="true" ></asp:TextBox>
                    <asp:HiddenField ID="ContactInfoHiddenField" runat="server" />
                </asp:TableCell>
             </asp:TableRow>
             
            
             
             
            
           </asp:Table> 
        </asp:TableCell>
        
        <asp:TableCell ID="SearchGridCell" runat="server" VerticalAlign="Top" Height="250" Width="400">
            <asp:Table ID="GridsTable" runat="server" HorizontalAlign="Left" >
                <%--Search Grid--%>
                <asp:TableRow ID="CurrentClientRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                <asp:TableCell ID="CurrentClientCell" runat="server">
                    <asp:Label ID="CurrentClientLabel" runat="server" BorderStyle="None" Text="Current Client: " Font-Bold="true" ForeColor="Red"></asp:Label>
                    <asp:Label ID="CurrentClientValueLabel" runat="server" BorderStyle="None" Text="" ></asp:Label><br /><br />
                </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow ID="CaseCloseMessageRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left" Visible="false">
                    <asp:TableCell ID="CaseCloseMessageCell" runat="server">
                        <asp:Label ID="CaseCloseMessageLabel" runat="server" BorderStyle="None" Text="Referral is cancelled for the selected referral date. " Font-Bold="true" ForeColor="Red"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow ID="GridsRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                    <asp:TableCell ID="GridsCell" runat="server">
                        <asp:GridView ID="GridView1" runat="server" Width="400" AutoGenerateSelectButton="true" AllowPaging="true" OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command" PageSize="4">
                        </asp:GridView>
                        <asp:GridView ID="GridView2" runat="server" Width="400" Visible = "false">
                        </asp:GridView>
                        <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None" Text="No search results found" Visible="false"></asp:Label><br />
                    </asp:TableCell>
                </asp:TableRow>

                <%--Added by Akshat--%>
                <asp:TableRow ID="RefInfoLabelRow" runat="server">
                    <asp:TableCell ID="RefInfoLabelCell" runat="server"> 
                        BillingByPhase Information
                    </asp:TableCell>
                    <asp:TableCell ID="RefInfoEditCell" runat="server">
                        <asp:Button ID="btnEditRefInfo" runat="server" Text="Edit" OnClick="btnEditRefInfo_Click"
                            Visible="false" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow ID="BillingStatusRow" runat="server">
                    <asp:TableCell ID="BillingStatusCell" runat="server">
                        <asp:GridView ID="BStatusGridView" ShowHeader="false" GridLines="Horizontal" CellSpacing="10" CellPadding="5" runat="server">
                        </asp:GridView>
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow ID="ContactInfoGridRow" runat="server">
                    <asp:TableCell ID="ContactInfoGridCell" runat="server">
                        <asp:GridView ID="ContactInfoGridView" ShowHeader="false" GridLines="Horizontal" CellSpacing="10" CellPadding="5" runat="server">
                        </asp:GridView>
                    </asp:TableCell>
                </asp:TableRow>
                
            </asp:Table>
          </asp:TableCell>
        
    </asp:TableRow>
    
    <%--Buttons--%>
    <asp:TableRow ID="SaveRow" runat="server">
        <asp:TableCell ID="SaveCell" runat="server" HorizontalAlign="Center" VerticalAlign="Middle" ColumnSpan="3">
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
